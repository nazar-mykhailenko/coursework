using AgroPlaner.DAL.Models;
using AgroPlaner.DAL.Repositories.Interfaces;
using AgroPlaner.Services.Interfaces;

namespace AgroPlaner.Services.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;

        public LocationService(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<IEnumerable<Location>> GetAllAsync(string? userId = null)
        {
            if (userId == null)
            {
                return await _locationRepository.GetAllAsync();
            }

            return await _locationRepository.GetFilteredAsync(l => l.UserId == userId);
        }

        public async Task<Location?> GetByIdAsync(int id, string? userId = null)
        {
            var location = await _locationRepository.GetByIdAsync(id);

            if (location != null && userId != null && location.UserId != userId)
            {
                return null;
            }

            return location;
        }

        public async Task<Location> CreateAsync(Location location, string userId)
        {
            location.UserId = userId;
            return await _locationRepository.CreateAsync(location);
        }

        public async Task<Location> UpdateAsync(Location location, string userId)
        {
            var existingLocation = await _locationRepository.GetByIdAsync(location.LocationId);
            if (existingLocation == null || existingLocation.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    "User does not have permission to update this location"
                );
            }

            // Update the location details
            existingLocation.Name = location.Name;
            existingLocation.Latitude = location.Latitude;
            existingLocation.Longitude = location.Longitude;

            // Since the repository doesn't have an Update method, we need to create a new one
            await _locationRepository.DeleteAsync(location.LocationId);
            return await _locationRepository.CreateAsync(existingLocation);
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null || location.UserId != userId)
            {
                return false;
            }

            await _locationRepository.DeleteAsync(id);
            return true;
        }
    }
}
