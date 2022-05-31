using AutoMapper;
using CookApp.WebApi.Models;
using CookApp.Domain.Models.CreatedEntities;

namespace CookApp.WebApi.Mapper;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<RecipeCreationRequest, RecipeCreationDto>();
    }
}
