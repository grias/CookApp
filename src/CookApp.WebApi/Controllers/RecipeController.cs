using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CookApp.Domain.Core;
using CookApp.Domain.Models;
using CookApp.Domain.Models.CreatedEntities;
using CookApp.Domain.Models.DbEntities;
using CookApp.Domain.UtilityClasses;


namespace CookApp.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class RecipeController : ControllerBase
{
    private readonly IRecipeCollection _recipes;

    public RecipeController(IRecipeCollection recipes)
    {
        _recipes = recipes;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BriefRecipeWithBriefCategories>))]
    public async Task<ActionResult<List<BriefRecipeWithBriefCategories>>> GetPageBrief([FromQuery] int page = 1, [FromQuery] int page_size = 10)
    {
        var recipes = await _recipes.GetBriefRecipesWithBriefCategoriesAsync(new Pagination(page, page_size));
        return Ok(recipes.ToList());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RecipeWithBriefCategories))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Recipe>> GetRecipe(int id)
    {
        var recipe = await _recipes.GetRecipeWithCategoriesAsync(id);
        if (recipe == null)
        {
            return NotFound();
        }
        return Ok(recipe);
    }
}
