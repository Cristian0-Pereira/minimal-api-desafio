namespace MinimalAPI.DTOs;

public record CarDTO
{
    public string Name { get; set; } = default!;
    public string Model { get; set; } = default!;
    public int Year { get; set; } = default!;
}