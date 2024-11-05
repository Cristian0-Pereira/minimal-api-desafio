using Microsoft.EntityFrameworkCore;
using MinimalAPI.Infrastructure.Db;
using MinimalAPI.Domains.Interfaces;
using MinimalAPI.Domains.Services;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Domains.ModelViews;
using minimal_api.Domains.Entities;
using MinimalApi.DTOs;
using MinimalAPI.DTOs;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<ICarServico, CarServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options => {
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home()));
#endregion

#region Administradores
app.MapPost("administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => {
    if (administradorServico.Login(loginDTO) != null) 
        return Results.Ok("Login com sucesso!");
    else
        return Results.Unauthorized();
});
#endregion

#region Car
app.MapPost("cars", ([FromBody] CarDTO carDTO, ICarServico carServico) => {
    var car = new Car{
        Name = carDTO.Name,
        Model = carDTO.Model,
        Year = carDTO.Year,
    };
    carServico.Include(car);

    return Results.Created($"/car/{car.Id}", car);
});
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

// * Configurando o Swagger no app