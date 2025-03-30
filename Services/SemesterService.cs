using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Models;
using Project_LMS.Exceptions;
using Project_LMS.Helpers;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Data;
using Microsoft.EntityFrameworkCore;

namespace Project_LMS.Services
{
    public class SemesterService : ISemesterService
    {
        private readonly ISemesterRepository _semesterRepository;
        private readonly IAcademicYearRepository _academicYearRepository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public SemesterService(ISemesterRepository semesterRepository, IAcademicYearRepository academicYearRepository, IMapper mapper, ApplicationDbContext context)
        {
            _semesterRepository = semesterRepository;
            _academicYearRepository = academicYearRepository;
            _mapper = mapper;
            _context = context;

        }

        public async Task<List<SemesterDropdownResponse>> GetSemestersByAcademicYearIdAsync(int academicYearId)
        {
            var semesters = await _context.Semesters
                .Where(s => s.AcademicYearId == academicYearId && (s.IsDelete == null || s.IsDelete == false))
                .OrderBy(s => s.StartDate)
                .Select(s => new SemesterDropdownResponse
                {
                    Id = s.Id,
                    Name = s.Name ?? string.Empty
                })
                .ToListAsync();

            return semesters;
        }
    }
}