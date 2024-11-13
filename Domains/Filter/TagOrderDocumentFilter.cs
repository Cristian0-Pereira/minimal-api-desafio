using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class TagOrderDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // Defina a ordem das tags conforme desejado
        swaggerDoc.Tags =
        [
            new() { Name = "Administradores" },
            new() { Name = "Home" },
            new() { Name = "Cars" }
        ];
    }
}
