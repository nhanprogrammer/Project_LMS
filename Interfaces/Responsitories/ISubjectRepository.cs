using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Repositories
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<Subject>> GetAllSubjects(int pageNumber, int pageSize);
        Task<Subject?> GetSubjectById(int id);
        Task<Subject> AddSubject(Subject subject);
        Task<Subject?> UpdateSubject(int id, Subject subject);
        Task<bool> DeleteSubject(int id);
    }
}


