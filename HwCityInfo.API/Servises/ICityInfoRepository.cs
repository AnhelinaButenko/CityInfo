using HwCityInfo.API.Entities;
using HwCityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HwCityInfo.API.Servises;

// This is the contract that your repository implementation will have to add here to.

// The Repository Pattern It`s an abstraction that reduces complexity(умен сложость)
// and aims to(стремится)make the code, safe for the repository implementation
public interface ICityInfoRepository
{
    // I was adding async methods to get data.
    // that freeing up threads so they can be used for
    // other tasks, which improves the scalability application

    Task<IEnumerable<City>> GetCitiesAsync();

    Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(
        string? name, string? searchQuery, int pageNumber, int pageSize);

    // This can return null if the city isn`t found,so the result of the return Task can be null.
    Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);

    Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);

    Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);

    // check if exists city
    Task<bool> CityExistsAsync(int cityId);

    Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);

    void DeletePointOfInterest(PointOfInterest pointOfInterest);

    Task<bool> SaveChangesAsync();
}
