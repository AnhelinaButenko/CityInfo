using HwCityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace HwCityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterst(int cityId, int pointsofinterestId)
        {
            var city = CitiesDataStore.Current.Cities
             .FirstOrDefault(x => x.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointsofinterest = city.PointsOfInterest
                .FirstOrDefault(x => x.Id == pointsofinterestId);

            if (pointsofinterest == null)
            {
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }
    }
}
