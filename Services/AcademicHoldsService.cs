using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Helpers;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;
using Project_LMS.Repositories;
using System.Collections;

namespace Project_LMS.Services
{
    public class AcademicHoldsService : IAcademicHoldsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAcademicHoldRepository _academicHoldRepository;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;


        public AcademicHoldsService(ApplicationDbContext context, IAcademicHoldRepository academicHoldRepository, IMapper mapper, ICloudinaryService cloudinaryService)
        {
            _context = context;
            _academicHoldRepository = academicHoldRepository;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<PaginatedResponse<AcademicHoldResponse>> GetPagedAcademicHolds(PaginationRequest request)
        {
            var query = from ah in _academicHoldRepository.GetQueryable().Where(ah => !ah.IsDelete)
                        join u in _context.Users on ah.UserId equals u.Id into userGroup
                        from userInfo in userGroup.DefaultIfEmpty()
                        select new
                        {
                            ah,
                            UserInfo = userInfo
                        };

            // Lấy danh sách dữ liệu từ database
            var rawData = await query.AsNoTracking().ToListAsync();

            // Xử lý trên bộ nhớ
            var resultList = rawData
                .Select(data => new AcademicHoldResponse
                {
                    Id = data.ah.Id,
                    UserId = data.ah.UserId,
                    UserCode = data.UserInfo?.UserCode,
                    FullName = data.UserInfo?.FullName,
                    BirthDate = data.UserInfo?.BirthDate,
                    Gender = data.UserInfo?.Gender is BitArray bitArray && bitArray.Count > 0? (bitArray[0] ? "True" : "False"): null,
                    ClassName = _context.Classes
                                       .Where(c => c.UserId == data.ah.UserId)
                                       .Select(c => c.Name)
                                       .FirstOrDefault(),
                    HoldDate = data.ah.HoldDate,
                    HoldDuration = data.ah.HoldDuration,
                    Reason = data.ah.Reason
                })
                .OrderByDescending(x => x.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            int totalItems = rawData.Count();

            return new PaginatedResponse<AcademicHoldResponse>
            {
                Items = resultList,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize),
                HasPreviousPage = request.PageNumber > 1,
                HasNextPage = request.PageNumber * request.PageSize < totalItems
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

            if (academicHold.UserId <= 0)
                throw new Exception("UserId không hợp lệ!");

            if (academicHold.ClassId <= 0)
                throw new Exception("ClassId không hợp lệ!");

            if (string.IsNullOrWhiteSpace(academicHold.Reason))
                throw new Exception("Lý do không được để trống!");

            if (academicHold.HoldDuration <= 0)
                throw new Exception("Thời gian giữ không hợp lệ");

            if (academicHold.HoldDate.Date < DateTime.UtcNow.Date)
                throw new Exception("HoldDate không được nhỏ hơn ngày hiện tại!");


            string uploadedFileName = null;
            if (!string.IsNullOrEmpty(academicHold.FileName))
            {
                uploadedFileName = await _cloudinaryService.UploadDocxAsync(academicHold.FileName);
            }

            var newAcademicHold = new AcademicHold
            {
                UserId = academicHold.UserId,
                HoldDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                HoldDuration = academicHold.HoldDuration,
                Reason = academicHold.Reason,
                CreateAt = TimeHelper.NowUsingTimeZone,
                UserCreate = 1,
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

            if (academicHold.UserId <= 0)
                throw new Exception("UserId không hợp lệ!");

            if (academicHold.ClassId <= 0)
                throw new Exception("ClassId không hợp lệ!");

            if (string.IsNullOrWhiteSpace(academicHold.Reason))
                throw new Exception("Lý do không được để trống!");

            if (academicHold.HoldDuration <= 0)
                throw new Exception("Thời gian giữ không hợp lệ");

            if (academicHold.HoldDate.Date < DateTime.UtcNow.Date)
                throw new Exception("HoldDate không được nhỏ hơn ngày hiện tại!");

            if (!string.IsNullOrEmpty(academicHold.FileName))
            {
                existingHold.FileName = await _cloudinaryService.UploadDocxAsync(academicHold.FileName);
            }

            existingHold.UserId = academicHold.UserId;
            existingHold.HoldDate = academicHold.HoldDate;
            existingHold.HoldDuration = academicHold.HoldDuration;
            existingHold.Reason = academicHold.Reason;
            existingHold.UpdateAt = TimeHelper.NowUsingTimeZone;

            _context.AcademicHolds.Update(existingHold);
            await _context.SaveChangesAsync();
        }


        //public async Task<bool> DeleteAcademicHold(int id)
        //{
        //    var existingAcademicYear = await _academicHoldRepository.GetByIdAsync(id);
        //    if (existingAcademicYear == null)
        //    {
        //        return false;
        //    }

        //    existingAcademicYear.IsDelete = true;
        //    await _academicHoldRepository.UpdateAsync(existingAcademicYear);

        //    return true;
        //}
        public async Task<bool> DeleteAcademicHold(int id)
        {
            await _academicHoldRepository.DeleteAsync(id);
            return true;
        }
        public async Task<List<Class_UserResponse>> GetAllUser_Class()
        {
            var classes = await _context.Classes
                .Where(c => !(c.IsDelete ?? false))
                .Select(c => new Class_UserResponse
                {
                    Id = c.Id,
                    ClassName = c.Name
                })
                .ToListAsync();

            return classes;
        }

        public async Task<List<User_AcademicHoldsResponse>> GetAllUserName()
        {
            return await _context.Users
                .Where(u => !(u.IsDelete ?? false))
                .Select(u => new User_AcademicHoldsResponse
                {
                    Id = u.Id,
                    FullName = u.FullName,
                })
                .ToListAsync();
        }
    }
}
