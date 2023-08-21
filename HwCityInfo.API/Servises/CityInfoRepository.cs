using HwCityInfo.API.DbContexts;
using HwCityInfo.API.Entities;
using HwCityInfo.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HwCityInfo.API.Servises;

// Implemented contract the ICityInfoRepository
// this is the place where provided persistence logic.

public class CityInfoRepository : ICityInfoRepository
{
    private readonly CityInfoContext _context;

    //injected it through constructor injection now store it private field and add a null check.
    public CityInfoRepository(CityInfoContext context) 
    {
        _context = context ?? throw new ArgumentException(nameof(context));
    }

    // Returned all the cities asynchronously that means that inside of this method i called into a method 
    // wanted to await, which also means that needed to apply the asynk to the method signature.
    public async Task<IEnumerable<City>> GetCitiesAsync()
    {
        return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(
        string? name, string? searchQuery, int pageNumber, int pageSize)
    {
        // collection to start from
        var collection = _context.Cities as IQueryable<City>;

        if (!string.IsNullOrWhiteSpace(name))
        {
            name = name.Trim();  // to get rid of line spaces 
            collection = collection.Where(c =>c.Name == name); // started filltering
        }

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            collection = collection.Where(a => a.Name.Contains(searchQuery) 
            || (a.Description != null && a.Description.Contains(searchQuery)));
        }

        var totalItemCount = await collection.CountAsync();

        var pagionationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

        // building a query and the query is only sent to the database at the end when the ToListAsync is reached
        var collectionToReturn = await collection.OrderBy(c => c.Name)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (collectionToReturn, pagionationMetadata);

        // Next I addeed this method in ICityInfoRepository contract
    }

    public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
    {
        if (includePointsOfInterest)
        {
            return await _context.Cities.Include(c => c.PointsOfInterest)
                .Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }

        return await _context.Cities.Where(c => c.Id == cityId).FirstOrDefaultAsync();
    }

    // add method CityExistsAsync and in it we call into AnyAsync. That will return true if a city with this ID
    // add this signature to the contract
    public async Task<bool> CityExistsAsync(int cityId)
    {
        return await _context.Cities.AnyAsync(c => c.Id == cityId);
    }

    public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
    {
        return await _context.PointsOfInterests
            .Where(p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
    {
        return await _context.PointsOfInterests.Where(p => p.CityId == cityId).ToListAsync();
    }

    public async Task AddPointOfInterestForCityAsync(int cityId, 
        PointOfInterest pointOfInterest)
    {
        var city = await GetCityAsync(cityId, false);

        if (city != null)
        {
            city.PointsOfInterest.Add(pointOfInterest);
        }
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await _context.SaveChangesAsync() >= 0);
    }

    public void DeletePointOfInterest(PointOfInterest pointOfInterest)
    {
        _context.PointsOfInterests.Remove(pointOfInterest);
    }
}
