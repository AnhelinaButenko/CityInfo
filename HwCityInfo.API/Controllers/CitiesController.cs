using AutoMapper;
using HwCityInfo.API.Models;
using HwCityInfo.API.Servises;
using Microsoft.AspNetCore.Mvc;

namespace HwCityInfo.API.Controllers;

[ApiController]
[Route("api/cities")]
// CitiesController derived from ControllerBase(give access to the model state,
// the current user and common methods for returning responses).
public class CitiesController : ControllerBase
{
    private readonly ICityInfoRepository _cityInfoRepository;
    private readonly IMapper _mapper;

    // add IMapper implementation instance
    // This is the contract AutoMapper`s must add here to
    // I can map a ource object to a destination object and it will use the profile i created for that
    public CitiesController(ICityInfoRepository cityInfoRepository,
        IMapper mapper) 
    {
        _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet] // to respond to Get request. we use HttpGet attribute
    public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities()
    {
        var cityEntities = await _cityInfoRepository.GetCitiesAsync();

        // To effectivelly map call mapper.Map pass in the type i want to map to in this case that`s an 
        //IEnumerable of CityWithoutPointsOfInterestDto
        // As created a mapping from the entity to the DTO
        return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
        //return Ok(_citiesDataStore.Cities); //using OK method for returning correct status code
    }

    [HttpGet("{id}")] // we pass in as the routing templete with specify Id.
    public async Task<ActionResult<CityDto>> GetCity(
        int id, bool includePointsOfInterest = false)
    {
        var city = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);
        if (city == null)
        {
            return NotFound();
        }

        if (includePointsOfInterest) 
        { 
            return Ok(_mapper.Map<CityDto>(city));
        }

        return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
    }
}
