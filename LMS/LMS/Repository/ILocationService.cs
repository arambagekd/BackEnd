using LMS.DTOs;

namespace LMS.Repository
{
    public interface ILocationService
    {
        Task<List<LocationListDto>> GetAllLocation(string cupboardname);
    }
}
