using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;
using Project_LMS.Repositories;

namespace Project_LMS.Services
{
    public class AcademicHoldsService : IAcademicHoldsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAcademicHoldRepository _academicHoldRepository;
        private readonly IMapper _mapper;

        public AcademicHoldsService(ApplicationDbContext context,IAcademicHoldRepository academicHoldRepository ,IMapper mapper)
        {
            _context = context;
            _academicHoldRepository = academicHoldRepository;
            _mapper = mapper;

        }

        public async Task<PaginatedResponse<AcademicHoldResponse>> GetPagedAcademicHolds(PaginationRequest request)
        {
            var query = from ah in _academicHoldRepository.GetQueryable().Where(ah => !ah.IsDelete)
                        join c in _context.Classes on ah.UserId equals c.UserId into classGroup
                        from classInfo in classGroup.DefaultIfEmpty()
                        join u in _context.Users on ah.UserId equals u.Id into userGroup
                        from userInfo in userGroup.DefaultIfEmpty()
                        select new AcademicHoldResponse
                        {
                            Id = ah.Id,
                            UserId = ah.UserId,
                            UserCode = userInfo != null ? userInfo.UserCode : null,
                            FullName = userInfo != null ? userInfo.FullName : null,
                            BirthDate = userInfo != null ? userInfo.BirthDate : null,
                            Gender = userInfo != null ? userInfo.Gender[0] : null,
                            ClassName = classInfo != null ? classInfo.Name : null,
                            HoldDate = ah.HoldDate,
                            HoldDuration = ah.HoldDuration,
                            Reason = ah.Reason,
                            //FileName = ah.FileName,
                            //CreateAt = ah.CreateAt,
                            //UserCreate = ah.UserCreate,
                            //IsDelete = ah.IsDelete,
                        };

            int totalItems = await query.CountAsync();
            int pageSize = request.PageSize > 0 ? request.PageSize : 10;

            var academicHoldsList = await query
                .Skip((request.PageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<AcademicHoldResponse>
            {
                Items = _mapper.Map<List<AcademicHoldResponse>>(academicHoldsList),
                PageNumber = request.PageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                HasPreviousPage = request.PageNumber > 1,
                HasNextPage = request.PageNumber * pageSize < totalItems
            };
        }

        public async Task<User_AcademicHoldResponse> GetById(int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.StudentStatus)
                .Include(u => u.Departments)
                .Include(u => u.Classes)
                .FirstOrDefaultAsync(u => u.Id == id && !(u.IsDelete ?? false));

            if (user == null) return null;

            var academicYear = await _context.AcademicYears
                .OrderByDescending(a => a.StartDate)
                .FirstOrDefaultAsync();

            return new User_AcademicHoldResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Gender = user.Gender,
                BirthDate = user.BirthDate,
                PlaceOfBirth = user.PlaceOfBirth,
                Ethnicity = user.Ethnicity,
                Religion = user.Religion,
                AcademicStartDate = academicYear?.StartDate?.Year.ToString(),
                AcademicEndDate = academicYear?.EndDate?.Year.ToString(),
                //DepartmentName = string.Join(", ", user.Departments.Select(d => d.Name)),
                DepartmentName = user.Departments?.FirstOrDefault()?.Name,
                //ClassName = string.Join(", ", user.Classes.Select(d => d.Name)),
                ClassName = user.Classes?.FirstOrDefault()?.Name,
                UserCode = user.UserCode,
                StartDate = user.StartDate,
                AdmissionType = user.AdmissionType,
                StatusName_Student = user.StudentStatus?.StatusName,
                Address = user.Address,
                Email = user.Email,
                Phone = user.Phone,
                FullnameFather = user.FullnameFather,
                FullnameMother = user.FullnameMother,
                FullnameGuardianship = user.FullnameGuardianship,
                BirthFather = user.BirthFather,
                BirthMother = user.BirthMother,
                BirthGuardianship = user.BirthGuardianship,
                WorkFather = user.WorkFather,
                WorkMother = user.WorkMother,
                WorkGuardianship = user.WorkGuardianship,
                PhoneFather = user.PhoneFather,
                PhoneMother = user.PhoneMother,
                PhoneGuardianship = user.PhoneGuardianship,
            };
        }

        public async Task AddAcademicHold(CreateAcademicHoldRequest academicHold)
        {
            // Lấy thông tin Class từ ClassId nếu cần
            var classInfo = await _context.Classes
                .Where(c => c.Id == academicHold.ClassId && !(c.IsDelete ?? false))
                .FirstOrDefaultAsync();

            if (classInfo == null)
            {
                throw new Exception("Class không tồn tại!");
            }

            var newAcademicHold = new AcademicHold
            {
                UserId = academicHold.UserId,
                HoldDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                HoldDuration = academicHold.HoldDuration,
                Reason = academicHold.Reason,
                FileName = academicHold.FileName,
                CreateAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                UserCreate = academicHold.UserCreate,
                IsDelete = false
            };

            academicHold.UserId = newAcademicHold.UserId;
            _context.AcademicHolds.Add(newAcademicHold);
            await _context.SaveChangesAsync();

        }


        public async Task UpdateAcademicHold(UpdateAcademicHoldRequest academicHold)
        {
            var existingHold = await _context.AcademicHolds
                                             .Where(ah => ah.Id == academicHold.Id && !ah.IsDelete)
                                             .FirstOrDefaultAsync();

            if (existingHold == null)
            {
                throw new Exception("Academic Hold không tồn tại!");
            }

            var classInfo = await _context.Classes
                .Where(c => c.Id == academicHold.ClassId && !(c.IsDelete ?? false))
                .FirstOrDefaultAsync();

            if (classInfo == null)
            {
                throw new Exception("Class không tồn tại!");
            }

            existingHold.UserId = academicHold.UserId;
            existingHold.HoldDate = academicHold.HoldDate;
            existingHold.HoldDuration = academicHold.HoldDuration;
            existingHold.Reason = academicHold.Reason;
            existingHold.FileName = academicHold.FileName;
            existingHold.UpdateAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            existingHold.UserUpdate = academicHold.UserUpdate;

            _context.AcademicHolds.Update(existingHold);
            await _context.SaveChangesAsync();

        }

        public async Task<bool> DeleteAcademicHold(int id)
        {
            var existingAcademicYear = await _academicHoldRepository.GetByIdAsync(id);
            if (existingAcademicYear == null)
            {
                return false;
            }

            existingAcademicYear.IsDelete = true;
            await _academicHoldRepository.UpdateAsync(existingAcademicYear);

            return true;
        }
    }
}