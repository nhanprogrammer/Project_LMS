using Project_LMS.DTOs.Request;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Repositories
{
    public interface ISchoolTransferRepository
    {
        Task<IEnumerable<SchoolTransfer>> GetAllAsync(int id, PaginationRequest request, bool isOrder, string column, string searchItem);
        Task<SchoolTransfer?> GetByIdAsync(int id);
        Task AddAsync(SchoolTransfer schoolTransfer);
        Task UpdateAsync(SchoolTransfer schoolTransfer);
        Task DeleteAsync(int id);
        Task<SchoolTransfer> GetByStudentId(int studentId);
    }
}