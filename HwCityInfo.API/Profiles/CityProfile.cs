using AutoMapper;

namespace HwCityInfo.API.Profiles;

public class CityProfile : Profile
{
    public CityProfile() 
    { 
        CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDto>();
        CreateMap<Entities.City, Models.CityDto>();
    }
}
