using Project_LMS.DTOs.Response;
using System.IO;
using System.Threading.Tasks;

namespace Project_LMS.Interfaces.Services
{
    public interface IExcelService
    {
        Task<string> ExportSchoolAndBranchesToExcelAsync(SchoolResponse school, int schoolId);
    }
}