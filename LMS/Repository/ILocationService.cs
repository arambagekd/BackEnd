using LMS.DTOs;

namespace LMS.Repository
{
    public interface ILocationService
    {
        Task<List<LocationListDto>> GetAllLocation(string cupboardname);
        Task<List<ResourceListDto>> SearchResources(SearchbookcupDto request);
        Task<bool> AddLocation(AddLocationDto location);
    }
}
