using Microsoft.AspNetCore.Http.HttpResults;
using MinimalApi.DTOs;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello Cristiano!!!");

app.MapPost("/login", (LoginDTO loginDTO) => {
    if (loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456") 
        return Results.Ok("Login com sucesso!");
    else
        return Results.Unauthorized();
});

app.Run();
