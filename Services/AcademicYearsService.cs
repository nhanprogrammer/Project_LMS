using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class AcademicYearsService : IAcademicYearsService
    {
        private readonly ISemesterRepository _semesterRepository;
        private readonly IAcademicYearRepository _academicYearRepository;
        private readonly IMapper _mapper;

        public AcademicYearsService(IAcademicYearRepository academicYearRepository, ISemesterRepository semesterRepository, IMapper mapper)
        {
            _semesterRepository = semesterRepository;
            _academicYearRepository = academicYearRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<AcademicYearResponse>> GetPagedAcademicYears(PaginationRequest request)
        {
            var query = _academicYearRepository.GetQueryable();

            int totalItems = await query.CountAsync();

            int pageSize = request.PageSize > 0 ? request.PageSize : 10;

            var academicYearsList = await query
                .Skip((request.PageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<AcademicYearResponse>
            {
                Items = _mapper.Map<List<AcademicYearResponse>>(academicYearsList),
                PageNumber = request.PageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                HasPreviousPage = request.PageNumber > 1,
                HasNextPage = request.PageNumber * pageSize < totalItems
            };
        }

        public async Task<AcademicYearResponse> GetByIdAcademicYear(int id)
        {
            var academicYear = await _academicYearRepository.GetByIdAsync(id);
            return _mapper.Map<AcademicYearResponse>(academicYear);
        }

        public async Task AddAcademicYear(CreateAcademicYearRequest request)
        {
            var academicYear = _mapper.Map<AcademicYear>(request);
            if (request.Semesters != null && request.Semesters.Any())
            {
                academicYear.Semesters = _mapper.Map<List<Semester>>(request.Semesters);
                foreach (var semester in academicYear.Semesters)
                {
                    await _semesterRepository.AddAsync(semester);
                }
            }
            await _academicYearRepository.AddAsync(academicYear);
        }

        public async Task UpdateAcademicYear(int id, UpdateAcademicYearRequest request)
        {
            var existingAcademicYear = await _academicYearRepository.GetByIdAsync(id);
            if (existingAcademicYear == null)
            {
                throw new Exception("Academic Year not found");
            }

            _mapper.Map(request, existingAcademicYear);
            await _academicYearRepository.UpdateAsync(existingAcademicYear);
        }

        public async Task<bool> DeleteAcademicYear(int id)
        {
            var existingAcademicYear = await _academicYearRepository.GetByIdAsync(id);
            if (existingAcademicYear == null)
            {
                return false;
            }

            await _academicYearRepository.DeleteAsync(id);
            return true;
        }
    }
}