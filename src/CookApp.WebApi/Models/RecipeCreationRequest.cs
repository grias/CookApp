using System.ComponentModel.DataAnnotations;

namespace CookApp.WebApi.Models;

public class RecipeCreationRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; init; }
    public string? Description { get; init; }
    public string? Process { get; init; }
}
