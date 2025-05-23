using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Models;
using Project_LMS.Exceptions;
using Project_LMS.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_LMS.Services
{
    public class SchoolBranchService : ISchoolBranchService
    {
        private readonly ISchoolBranchRepository _schoolBranchRepository;
        private readonly ISchoolRepository _schoolRepository;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;

        public SchoolBranchService(ISchoolBranchRepository schoolBranchRepository, ISchoolRepository schoolRepository, IMapper mapper, ICloudinaryService cloudinaryService)
        {
            _schoolBranchRepository = schoolBranchRepository;
            _schoolRepository = schoolRepository;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<IEnumerable<SchoolBranchResponse>> GetAllAsync()
        {
            var branches = await _schoolBranchRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SchoolBranchResponse>>(branches);
        }

        public async Task<SchoolBranchResponse> GetByIdAsync(int id)
        {
            var branch = await _schoolBranchRepository.GetByIdAsync(id);
            if (branch == null)
            {
                throw new NotFoundException("Không tìm thấy chi nhánh trường");
            }

            return _mapper.Map<SchoolBranchResponse>(branch);
        }

        public async Task<SchoolBranchResponse> CreateAsync(SchoolBranchRequest branchRequest, int userId)
        {
            var errors = new List<ValidationError>();
            if (!IntValidator.IsValid(branchRequest.SchoolId.ToString() ?? ""))
                errors.Add(new ValidationError { Field = "SchoolId", Error = "SchoolId phải là số nguyên" });

            if (!StringValidator.IsOnlyLettersAndNumbers(branchRequest.BranchName))
                errors.Add(new ValidationError { Field = "BranchName", Error = "BranchName không hợp lệ" });

            if (StringValidator.ContainsSpecialCharacters(branchRequest.Address))
                errors.Add(new ValidationError { Field = "Address", Error = "Địa chỉ không được chứa ký tự đặc biệt" });

            if (!EmailValidator.IsValid(branchRequest.Email))
                errors.Add(new ValidationError { Field = "Email", Error = "Email không hợp lệ" });

            if (!PhoneValidator.IsValid(branchRequest.Phone))
                errors.Add(new ValidationError { Field = "Phone", Error = "Số điện thoại không hợp lệ" });

            if (errors.Any())
            {
                throw new BadRequestException("Validation failed.", errors);
            }

            var school = await _schoolRepository.GetByIdAsync(branchRequest.SchoolId ?? 0);
            if (school == null)
            {
                throw new NotFoundException("Không tìm thấy trường với ID đã cho");
            }

            var branch = _mapper.Map<SchoolBranch>(branchRequest);
            branch.UserCreate = userId;
            branch.IsDelete = false;
            if (!string.IsNullOrEmpty(branchRequest.Image))
            {
                school.Image = await _cloudinaryService.UploadImageAsync(branchRequest.Image);
            }
            await _schoolBranchRepository.AddAsync(branch);

            return _mapper.Map<SchoolBranchResponse>(branch);
        }

        public async Task<SchoolBranchResponse> UpdateAsync(int id, SchoolBranchRequest branchRequest, int userId)
        {
            var errors = new List<ValidationError>();
            if (!IntValidator.IsValid(branchRequest.SchoolId.ToString() ?? ""))
                errors.Add(new ValidationError { Field = "SchoolId", Error = "SchoolId phải là số nguyên" });

            if (!StringValidator.IsOnlyLettersAndNumbers(branchRequest.BranchName))
                errors.Add(new ValidationError { Field = "BranchName", Error = "BranchName không hợp lệ" });

            if (StringValidator.ContainsSpecialCharacters(branchRequest.Address))
                errors.Add(new ValidationError { Field = "Address", Error = "Địa chỉ không được chứa ký tự đặc biệt" });

            if (!EmailValidator.IsValid(branchRequest.Email))
                errors.Add(new ValidationError { Field = "Email", Error = "Email không hợp lệ" });

            if (!PhoneValidator.IsValid(branchRequest.Phone))
                errors.Add(new ValidationError { Field = "Phone", Error = "Số điện thoại không hợp lệ" });

            if (errors.Any())
            {
                throw new BadRequestException("Validation failed.", errors);
            }

            var branch = await _schoolBranchRepository.GetByIdAsync(id);
            if (branch == null)
            {
                throw new NotFoundException("Không tìm thấy chi nhánh trường");
            }

            if (branchRequest.SchoolId.HasValue)
            {
                var school = await _schoolRepository.GetByIdAsync(branchRequest.SchoolId.Value);
                if (school == null)
                {
                    throw new NotFoundException("Không tìm thấy trường với ID đã cho");
                }
            }
            if (!string.IsNullOrEmpty(branchRequest.Image))
            {
                // Xóa file cũ nếu tồn tại
                if (!string.IsNullOrEmpty(branch.Image))
                {
                    try
                    {
                        await _cloudinaryService.DeleteFileByUrlAsync(branch.Image);
                    }
                    catch (Exception ex)
                    {
                        // Ghi log lỗi nhưng không ném ngoại lệ để tiếp tục upload file mới
                        Console.WriteLine($"Lỗi khi xóa file cũ: {ex.Message}");
                    }
                }

                // Upload file mới
                branch.Image = await _cloudinaryService.UploadImageAsync(branchRequest.Image);
                Console.WriteLine(branch.Image + " IMMMMMMM");
            }

            _mapper.Map(branchRequest, branch);
            branch.UserUpdate = 1;
            branch.UserUpdate = userId;
            await _schoolBranchRepository.UpdateAsync(branch);

            return _mapper.Map<SchoolBranchResponse>(branch);
        }

        public async Task<SchoolBranchResponse> DeleteAsync(int id)
        {
            var branch = await _schoolBranchRepository.GetByIdAsync(id);
            if (branch == null)
            {
                throw new NotFoundException("Không tìm thấy chi nhánh trường");
            }
            if (branch.IsDelete == true)
            {
                throw new BadRequestException("Chi nhánh trường đã bị xóa", new List<ValidationError>());
            }

            branch.IsDelete = true;
            branch.UserUpdate = 1;

            await _schoolBranchRepository.UpdateAsync(branch);

            return _mapper.Map<SchoolBranchResponse>(branch);
        }
    }
}