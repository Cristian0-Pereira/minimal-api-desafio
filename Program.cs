using Microsoft.EntityFrameworkCore;
using MinimalAPI.Infrastructure.Db;
using MinimalAPI.Domains.Interfaces;
using MinimalAPI.Domains.Services;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Domains.ModelViews;
using minimal_api.Domains.Entities;
using MinimalApi.DTOs;
using MinimalAPI.DTOs;
using MinimalAPI.Domains.Enuns;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;

#region Builder

var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if(string.IsNullOrEmpty(key) || key.Length < 32)
{
    // Gera uma chave aleatória
    key = Convert.ToBase64String(Guid.NewGuid().ToByteArray()); 
}

builder.Services.AddAuthentication(option => {
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option => {
    option.TokenValidationParameters = new TokenValidationParameters{
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidAudience = "minimal-api-audiencia",
        ClockSkew = TimeSpan.Zero,
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<ICarServico, CarServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.DocumentFilter<TagOrderDocumentFilter>(); // Adicione essa linha para ativar o filtro de ordenação
});
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT aqui"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference 
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Remover a duplicação, mantendo apenas uma linha
    options.DocumentFilter<TagOrderDocumentFilter>();
});

// Configuração do banco de dados
builder.Services.AddDbContext<DbContexto>(options => 
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySql"))
    );
});

var app = builder.Build();
#endregion

#region Home

app.MapGet("/", () => Results.Redirect("/swagger")).AllowAnonymous().WithTags("Home");
#endregion

#region Administradores

string GerarTokenJwt(Administrador administrador){
    if(string.IsNullOrEmpty(key)) return string.Empty;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new("Email", administrador.Email),
        new("Perfil", administrador.Perfil),
        new(ClaimTypes.Role, administrador.Perfil),
        new Claim(JwtRegisteredClaimNames.Aud, "minimal-api-audiencia"),
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires : DateTime.Now.AddDays(1),
        signingCredentials : credentials
    );
    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => {
    var adm = administradorServico.Login(loginDTO);
    if (adm != null) 
    {
        string token = GerarTokenJwt(adm);
        return Results.Ok(new AdmLogado
        {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }    
    else
        return Results.Unauthorized();
}).AllowAnonymous().WithTags("Administradores");

app.MapGet("/administradores", ([FromQuery] int? page,  IAdministradorServico administradorServico) => {
    var adms = new List<AdmModelView>();
    var administradores = administradorServico.All(page);    
    foreach (var adm in administradores)
    {
        adms.Add(new AdmModelView{
            Id = adm.Id,
            Email = adm.Email,
            Perfil =  adm.Perfil
        });
    }
    return Results.Ok(adms);
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Administradores");

app.MapGet("/Administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) => 
{
    var administrador = administradorServico.SearchById(id);
    if(administrador == null) return Results.NotFound();
    return Results.Ok(new AdmModelView{
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil =  administrador.Perfil
    });
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Administradores");

app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) => {
    var validation = new ValidationErrors{
        Messages = new List<string>()
    };

    if(string.IsNullOrEmpty(administradorDTO.Email))
        validation.Messages.Add("Email não pode ser vazio");
    if(string.IsNullOrEmpty(administradorDTO.Senha))
        validation.Messages.Add("Senha não pode ser vazia");
    if(administradorDTO.Perfil == null)
        validation.Messages.Add("Perfil não pode ser vazio");

    if(validation.Messages.Count > 0)
        return Results.BadRequest(validation);

    var administrador = new Administrador
    {
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString(),
    };
    administradorServico.Include(administrador);

    return Results.Created($"/administrador/{administrador.Id}", new AdmModelView{
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil =  administrador.Perfil
    });
    
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Administradores");
#endregion

#region Car

ValidationErrors validDTO(CarDTO carDTO)
{
    var validation = new ValidationErrors{
        Messages = new List<string>()
    };

        if(string.IsNullOrEmpty(carDTO.Name))
        validation.Messages.Add("The name cannot be empty");

        if(string.IsNullOrEmpty(carDTO.Model))
        validation.Messages.Add("The model cannot be left blank");    
        
        if(carDTO.Year < 1950)
        validation.Messages.Add("Very old car, only years older than 1950 are accepted");

        return validation;
}

app.MapPost("cars", ([FromBody] CarDTO carDTO, ICarServico carServico) => 
{
    var validation = validDTO(carDTO);
    if(validation.Messages.Count > 0)
        return Results.BadRequest(validation);

    var car = new Car{
        Name = carDTO.Name,
        Model = carDTO.Model,
        Year = carDTO.Year,
    };
    carServico.Include(car);

    return Results.Created($"/car/{car.Id}", car);
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm Editor" })
.WithTags("Cars");

app.MapGet("cars", ([FromQuery] int? page, ICarServico carServico) => 
{
    var cars = carServico.All(page);

    return Results.Ok(cars);
}).RequireAuthorization().WithTags("Cars");

app.MapGet("cars/{id}", ([FromRoute] int id, ICarServico carServico) => 
{
    var car = carServico.SearchById(id);
    if(car == null) return Results.NotFound();
    return Results.Ok(car);
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
.WithTags("Cars");

app.MapPut("cars/{id}", ([FromRoute] int id, CarDTO carDTO, ICarServico carServico) => 
{
    var car = carServico.SearchById(id);
    if(car == null) return Results.NotFound();
    
    var validation = validDTO(carDTO);
    if(validation.Messages.Count > 0)
        return Results.BadRequest(validation);
    
    car.Name = carDTO.Name;
    car.Model = carDTO.Model;
    car.Year = carDTO.Year;

    carServico.Update(car);

    return Results.Ok(car);
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Cars");

app.MapDelete("cars/{id}", ([FromRoute] int id, ICarServico carServico) => 
{
    var car = carServico.SearchById(id);
    if(car == null) return Results.NotFound();

    carServico.Delete(car);

    return Results.NoContent();
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Cars");
#endregion

#region App

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion