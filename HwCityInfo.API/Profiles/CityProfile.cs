using AutoMapper;

namespace HwCityInfo.API.Profiles;

public class CityProfile : Profile
{
    // To add a mapping configuration, i can use the constructor
    public CityProfile() 
    {
        // Crate a map from the City entity to destination type CityWithoutPointsOfInterestDto
        CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDto>();
        CreateMap<Entities.City, Models.CityDto>();
    }
}
