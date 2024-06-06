using LMS.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Repository
{
    public class LocationService:ILocationService
    {
        private readonly DataContext _Context;
        public LocationService(DataContext context)
        {
            _Context = context;
        }
        public async Task<List<LocationListDto>> GetAllLocation(string cupboardname)
        {
            var locations = _Context.Locations.ToList();

            // Group by CupboardId and process each group
            var locationListDtos = new List<LocationListDto>();

            foreach (var group in locations.GroupBy(cs => cs.CupboardId))
            {
                // Fetch the cupboard info asynchronously
                var cupboard = await _Context.Cupboard.FirstOrDefaultAsync(e => e.cupboardID == group.Key);

                // Create the LocationListDto object
                var count = _Context.Resources
                              .Where(s => _Context.Locations.Any(e => e.CupboardId == cupboard.cupboardID && e.LocationNo == s.BookLocation))
                              .Count();

                var locationListDto = new LocationListDto
                {
                    CupboardId = cupboard.cupboardID.ToString(),
                    CupboardName = cupboard.name,
                    ShelfNo = group.Select(cs => cs.ShelfNo.ToString()).ToList(),
                    count = count
                };

                locationListDtos.Add(locationListDto);
            }
            if(cupboardname != "")
            locationListDtos = locationListDtos.Where(e => e.CupboardName == cupboardname).ToList();

            return locationListDtos;
        }
    }
}
