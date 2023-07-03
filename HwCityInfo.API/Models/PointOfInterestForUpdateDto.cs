using System.ComponentModel.DataAnnotations;

namespace HwCityInfo.API.Models
{
    public class PointOfInterestForUpdateDto
    {
        // City Dto is only used to get data, not to store it

        //I specify such properties with requirements if length is greater 50 we can see message
        [Required(ErrorMessage = "You should provide a name value.")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty; // assign string.Empty to avoid null reference issues.

        [MaxLength(200)]
        public string? Description { get; set; } //mark it optional parameter
    }
}
