using LMS.DTOs;
using LMS.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase

    {
        //Create ILocationService Field
        private readonly ILocationService _locationService;

        //Constructor
        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

      
    
        [HttpPost("GetAllLocation")]
        public async Task<List<LocationListDto>> GetAllLocation(LocationDto location)
        {
            return await _locationService.GetAllLocation(location.CupboardName);
        }
    }
}
