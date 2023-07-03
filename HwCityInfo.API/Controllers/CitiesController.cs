using HwCityInfo.API.Models;
using HwCityInfo.API.Servises;
using Microsoft.AspNetCore.Mvc;

namespace HwCityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    // CitiesController derived from ControllerBase(give access to the model state,
    // the current user and common methods for returning responses).
    public class CitiesController : ControllerBase
    {
        private readonly CitiesDataStore _citiesDataStore;

        public CitiesController(CitiesDataStore citiesDataStore) 
        {
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
        }

        [HttpGet] // to respond to Get request. we use HttpGet attribute
        public ActionResult<IEnumerable<CityDto>> GetCities()
        {
            return Ok(_citiesDataStore.Cities); //using OK method for returning correct status code
        }

        [HttpGet("{id}")] // we pass in as the routing templete with specify Id.
        public ActionResult<CityDto> GetCity(int id) 
        { 
            var cityToReturn = _citiesDataStore.Cities
                .FirstOrDefault(x => x.Id == id);

            if (cityToReturn == null)
            {
                return NotFound();
            }

            return Ok(cityToReturn);
        }
    }
}
