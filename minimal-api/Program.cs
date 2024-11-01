using Microsoft.EntityFrameworkCore;
using minimal_api.Infrastructure.Db;
using MinimalApi.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbContexto>(options => {
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();

app.MapGet("/", () => "Hello Cristiano!!!");

app.MapPost("/login", (LoginDTO loginDTO) => {
    if (loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456") 
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

// C:\xampp\mysql\bin\mysql -u root -p

// Validando admin com login 

// Abrir o xampp - startar o apache e o mysql
// Abrir o Post
// Abrir o Docker
// Verificar as conexões antes de iniciar os estudos
