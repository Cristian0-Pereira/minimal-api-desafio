using Microsoft.EntityFrameworkCore;
using MinimalAPI.Infrastructure.Db;
using MinimalAPI.Domains.Interfaces;
using MinimalAPI.Domains.Services;
using MinimalApi.DTOs;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();

builder.Services.AddDbContext<DbContexto>(options => {
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();

app.MapGet("/", () => "Hello Cristiano!!!");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => {
    if (administradorServico.Login(loginDTO) != null) 
        return Results.Ok("Login com sucesso!");
    else
        return Results.Unauthorized();
});

app.Run();

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