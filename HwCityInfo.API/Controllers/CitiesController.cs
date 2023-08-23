using AutoMapper;
using HwCityInfo.API.Models;
using HwCityInfo.API.Servises;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HwCityInfo.API.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/cities")]
// CitiesController derived from ControllerBase(give access to the model state,
// the current user and common methods for returning responses).
public class CitiesController : ControllerBase
{
    private readonly ICityInfoRepository _cityInfoRepository;
    private readonly IMapper _mapper;
    const int maxCitiesPageSize = 20;

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
    public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(
        string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10) //specify defoult, if concumer choose 100
                                                                                  //it`s negative impecteve on  performance
    {
        if (pageSize > maxCitiesPageSize)
        {
            pageSize = maxCitiesPageSize;
        }

        var (cityEntities, paginationMetadata) = await _cityInfoRepository
            .GetCitiesAsync(name, searchQuery, pageNumber, pageSize); //instead of calling into GetCitiesAsync,call into an
                                                                      //overload for that, that one that accepts name, searchQuery
                                                                      //Return pagination metadata in a custom pagination header
        Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

        // To effectivelly map call mapper.Map pass in the type i want to map to in this case that`s an 
        // IEnumerable of CityWithoutPointsOfInterestDto
        // As created a mapping from the entity to the DTO.
        //using OK method for returning correct status code
        return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
    } // Let's get this running


    /// <summary>
    /// Get a city by id
    /// </summary>
    /// <param name="id">The id of the city to get</param>
    /// <param name="includePointsOfInterest">Whether or not to include the points of interest</param>
    /// <returns>An IActionResult</returns>
    /// <response code="200">Returns the requested city</response>
    [HttpGet("{id}")] // we pass in as the routing templete with specify Id.
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

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
