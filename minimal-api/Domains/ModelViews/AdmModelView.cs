using MinimalAPI.Domains.Enuns;

namespace MinimalAPI.Domains.ModelViews;

public record AdmModelView
{
    public int Id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Perfil { get; set; } = default!;
}