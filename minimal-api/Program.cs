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

#region Builder

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<ICarServico, CarServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.DocumentFilter<TagOrderDocumentFilter>(); // Adicione essa linha para ativar o filtro de ordenação
});

builder.Services.AddDbContext<DbContexto>(options => 
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();
#endregion

#region Home

app.MapGet("/", () => Results.Redirect("/swagger")).WithTags("Home");
#endregion

#region Administradores

app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => {
    if (administradorServico.Login(loginDTO) != null) 
        return Results.Ok("Login realizado com sucesso!");
    else
        return Results.Unauthorized();
}).WithTags("Administradores");

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
}).WithTags("Administradores");

app.MapGet("/Administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) => 
{
    var administrador = administradorServico.SearchById(id);
    if(administrador == null) return Results.NotFound();
    return Results.Ok(new AdmModelView{
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil =  administrador.Perfil
    });
}).WithTags("Administradores");

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
    
}).WithTags("Administradores");
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
}).WithTags("Cars");

app.MapGet("cars", ([FromQuery] int? page, ICarServico carServico) => 
{
    var cars = carServico.All(page);

    return Results.Ok(cars);
}).WithTags("Cars");

app.MapGet("cars/{id}", ([FromRoute] int id, ICarServico carServico) => 
{
    var car = carServico.SearchById(id);
    if(car == null) return Results.NotFound();
    return Results.Ok(car);
}).WithTags("Cars");

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
}).WithTags("Cars");

app.MapDelete("cars/{id}", ([FromRoute] int id, ICarServico carServico) => 
{
    var car = carServico.SearchById(id);
    if(car == null) return Results.NotFound();

    carServico.Delete(car);

    return Results.NoContent();
}).WithTags("Cars");
#endregion

#region App

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion

// GPT exemplo

// app.MapPost("/login", async (LoginDTO loginDTO, DbContexto db) => {
//     var administrador = await db.Administradores
//         .FirstOrDefaultAsync(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha);
    
//     if (administrador != null) 
//         return Results.Ok("Login com sucesso!");
//     else
//         return Results.Unauthorized();
// });


// Abrir o Docker
// Abrir o Post
// Abrir o xampp - startar o apache e o mysql
// no terminal
// C:\xampp\mysql\bin\mysql -u root -p
// para poder entrar no mysql pelo vscode
// Verificar as conexões antes de iniciar os estudos

//   \! cls - para limpar a tela no mysql

// * Configurando Token JWT no projeto