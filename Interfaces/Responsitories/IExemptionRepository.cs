using Project_LMS.DTOs.Request;
using Project_LMS.Models;
namespace Project_LMS.Interfaces.Responsitories
{
    public interface IExemptionRepository
    {
        public Task AddAsync(Exemption exemption);
    }
}
