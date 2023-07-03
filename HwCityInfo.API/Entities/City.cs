using HwCityInfo.API.Models;
using System.ComponentModel.DataAnnotations;

namespace HwCityInfo.API.Entities
{
    public class City
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public ICollection<PointOfInterest> PointOfInterests { get; set; }
        = new List<PointOfInterest>();

        public City(string name) 
        {
            Name = name;
        }
    }
}
