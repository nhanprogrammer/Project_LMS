using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories
{
    public class SubjectTypeRepository: ISubjectTypeService
    {
        private readonly ApplicationDbContext _context;

        public SubjectTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubjectType>> GetAll()
        {
            return await _context.SubjectTypes
                .Where(st => st.IsDelete == false)
                .ToListAsync();
        }

        public async Task<SubjectType?> GetById(int id)
        {
            return await _context.SubjectTypes.FindAsync(id);
        }

        public async Task<SubjectType> Add(SubjectType subjectType)
        {
            _context.SubjectTypes.Add(subjectType);
            await _context.SaveChangesAsync();
            return subjectType;
        }

        public async Task<SubjectType?> Update(int id, SubjectType subjectType)
        {
            var existing = await _context.SubjectTypes.FindAsync(id);
            if (existing == null) return null;

            existing.Name = subjectType.Name;
            existing.UpdateAt = DateTime.UtcNow;
            existing.UserUpdate = subjectType.UserUpdate;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> Delete(int id)
        {
            var subjectType = await _context.SubjectTypes.FindAsync(id);
            if (subjectType == null) return false;

            subjectType.IsDelete = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<ApiResponse<PaginatedResponse<SubjectTypeResponse>>> GetAllSubjectTypesAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<SubjectTypeResponse>> GetSubjectTypeByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<SubjectTypeResponse>> CreateSubjectTypeAsync(SubjectTypeRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<SubjectTypeResponse>> UpdateSubjectTypeAsync(int id, SubjectTypeRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> DeleteSubjectTypeAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
