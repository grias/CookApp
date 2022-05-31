using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CookApp.Domain.Core;
using CookApp.Domain.Models;
using CookApp.Domain.Models.CreatedEntities;
using CookApp.Domain.Models.DbEntities;
using CookApp.Domain.UtilityClasses;
using CookApp.WebApi.Models;
using AutoMapper;


namespace CookApp.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class RecipeController : ControllerBase
{
    private readonly IRecipeCollection _recipes;
    private readonly IMapper _mapper;

    public RecipeController(IRecipeCollection recipes, IMapper mapper)
    {
        _recipes = recipes;
        _mapper = mapper;
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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Recipe))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Recipe>> CreateRecipe([FromBody] RecipeCreationRequest recipe)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var recipeDto = _mapper.Map<RecipeCreationDto>(recipe);

        var createdRecipe = await _recipes.AddRecipeAsync(recipeDto);
        return CreatedAtAction(nameof(GetRecipe), new { id = createdRecipe.Id }, createdRecipe);
    }
}
