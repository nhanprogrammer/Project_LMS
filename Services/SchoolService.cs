using AutoMapper;
using Project_LMS.Interfaces.Services;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;
using Project_LMS.Exceptions;
using Project_LMS.Helpers;
using Project_LMS.Data;
using Microsoft.EntityFrameworkCore;

namespace Project_LMS.Services
{
    public class SchoolService : ISchoolService
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        private readonly ApplicationDbContext _context;
        private readonly ICloudinaryService _cloudinaryService;
        public SchoolService(ISchoolRepository schoolRepository, IMapper mapper, IServiceProvider serviceProvider, ApplicationDbContext context, ICloudinaryService cloudinaryService)
        {
            _schoolRepository = schoolRepository;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<IEnumerable<SchoolResponse>> GetAllAsync()
        {
            var schools = await _schoolRepository.GetAllAsync();
            return _mapper.Map<List<SchoolResponse>>(schools);
        }

        public async Task<SchoolResponse> GetByIdAsync(int id)
        {
            var school = await _schoolRepository.GetByIdAsync(id);
            if (school == null)
            {
                throw new NotFoundException("Không tìm thấy trường");
            }
            return _mapper.Map<SchoolResponse>(school);
        }

        public async Task<SchoolResponse> CreateAsync(SchoolRequest schoolRequest)
        {
            var errors = new List<ValidationError>();

            if (!StringValidator.IsOnlyLettersAndNumbers(schoolRequest.SchoolCode))
                errors.Add(new ValidationError { Field = "SchoolCode", Error = "SchoolCode không hợp lệ" });

            if (StringValidator.ContainsSpecialCharacters(schoolRequest.Name))
                errors.Add(new ValidationError { Field = "Name", Error = "Tên trường không được chứa ký tự đặc biệt" });

            if (!EmailValidator.IsValid(schoolRequest.Email))
                errors.Add(new ValidationError { Field = "Email", Error = "Email không hợp lệ" });

            if (!PhoneValidator.IsValid(schoolRequest.Phone))
                errors.Add(new ValidationError { Field = "Phone", Error = "Số điện thoại không hợp lệ" });

            if (!PhoneValidator.IsValid(schoolRequest.PrincipalPhone))
                errors.Add(new ValidationError { Field = "PrincipalPhone", Error = "Số điện thoại của hiệu trưởng không hợp lệ" });

            if (!Uri.IsWellFormedUriString(schoolRequest.Website, UriKind.Absolute))
                errors.Add(new ValidationError { Field = "Website", Error = "Website không hợp lệ" });

            if (!schoolRequest.EstablishmentDate.HasValue || !DateTimeValidator.IsValidDateTime(schoolRequest.EstablishmentDate.Value.ToString()))
                errors.Add(new ValidationError { Field = "EstablishmentDate", Error = "Ngày thành lập không hợp lệ" });

            if (errors.Any())
            {
                throw new BadRequestException("Validation failed.", errors);
            }

            var school = _mapper.Map<School>(schoolRequest);
            school.UserCreate = 1;

            school.IsDelete = false;
            if (!string.IsNullOrEmpty(schoolRequest.Image))
            {
                school.Image = await _cloudinaryService.UploadImageAsync(schoolRequest.Image);
            }
            await _schoolRepository.AddAsync(school);
            return _mapper.Map<SchoolResponse>(school);
        }

        public async Task<SchoolResponse> UpdateAsync(int id, SchoolRequest schoolRequest, int userId)
        {
            var errors = new List<ValidationError>();

            if (!StringValidator.IsOnlyLettersAndNumbers(schoolRequest.SchoolCode))
                errors.Add(new ValidationError { Field = "SchoolCode", Error = "SchoolCode không hợp lệ" });

            if (StringValidator.ContainsSpecialCharacters(schoolRequest.Name))
                errors.Add(new ValidationError { Field = "Name", Error = "Tên trường không được chứa ký tự đặc biệt" });

            if (!EmailValidator.IsValid(schoolRequest.Email))
                errors.Add(new ValidationError { Field = "Email", Error = "Email không hợp lệ" });

            if (!PhoneValidator.IsValid(schoolRequest.Phone))
                errors.Add(new ValidationError { Field = "Phone", Error = "Số điện thoại không hợp lệ" });

            if (!PhoneValidator.IsValid(schoolRequest.PrincipalPhone))
                errors.Add(new ValidationError { Field = "PrincipalPhone", Error = "Số điện thoại của hiệu trưởng không hợp lệ" });

            if (!Uri.IsWellFormedUriString(schoolRequest.Website, UriKind.Absolute))
                errors.Add(new ValidationError { Field = "Website", Error = "Website không hợp lệ" });

            if (!schoolRequest.EstablishmentDate.HasValue || !DateTimeValidator.IsValidDateTime(schoolRequest.EstablishmentDate.Value.ToString()))
                errors.Add(new ValidationError { Field = "EstablishmentDate", Error = "Ngày thành lập không hợp lệ" });

            if (errors.Any())
            {
                throw new BadRequestException("Validation failed.", errors);
            }

            var school = await _schoolRepository.GetByIdAsync(id);
            if (school == null)
            {
                throw new NotFoundException("Không tìm thấy trường");
            }


            school.UserUpdate = 1;
            if (!string.IsNullOrEmpty(schoolRequest.Image))
            {
                // Xóa file cũ nếu tồn tại
                if (!string.IsNullOrEmpty(school.Image))
                {
                    try
                    {
                        await _cloudinaryService.DeleteFileByUrlAsync(school.Image);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi khi xóa file cũ: {ex.Message}");
                    }
                }

                // Upload file mới
                school.Image = await _cloudinaryService.UploadImageAsync(schoolRequest.Image);
            }

            school.UserUpdate = userId;
            _mapper.Map(schoolRequest, school);

            await _schoolRepository.UpdateAsync(school);
            return _mapper.Map<SchoolResponse>(school);
        }

        public async Task<SchoolResponse> DeleteAsync(int id)
        {
            var school = await _schoolRepository.GetByIdAsync(id);
            if (school == null)
            {
                throw new NotFoundException("Không tìm thấy trường");
            }

            school.IsDelete = true;
            school.UserUpdate = 1;

            await _schoolRepository.UpdateAsync(school);

            return _mapper.Map<SchoolResponse>(school);
        }

        public async Task<ApiResponse<string[]>> GetEducationModelsAsync()
        {
            var educationModels = new string[] { "Chính quy", "Công lập", "Tư thục" };
            return new ApiResponse<string[]>(0, "Lấy danh sách mô hình đào tạo thành công", educationModels);
        }

        public async Task<SchoolResponse> GetSchoolAndBranchesAsync(int schoolId, List<int> schoolBranchIds)
        {
            var school = await _context.Schools
                .Where(s => s.Id == schoolId && (s.IsDelete == null || s.IsDelete == false))
                .Include(s => s.SchoolBranches.Where(sb => schoolBranchIds.Contains(sb.Id) && (sb.IsDelete == null || sb.IsDelete == false)))
                .FirstOrDefaultAsync();

            if (school == null)
            {
                return null;
            }

            return _mapper.Map<SchoolResponse>(school);
        }
    }
}