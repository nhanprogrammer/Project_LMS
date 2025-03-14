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

        public SchoolBranchService(ISchoolBranchRepository schoolBranchRepository, ISchoolRepository schoolRepository, IMapper mapper)
        {
            _schoolBranchRepository = schoolBranchRepository;
            _schoolRepository = schoolRepository;
            _mapper = mapper;
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

        public async Task<SchoolBranchResponse> CreateAsync(SchoolBranchRequest branchRequest)
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
            branch.UserCreate = 1;
            branch.IsDelete = false;

            await _schoolBranchRepository.AddAsync(branch);

            return _mapper.Map<SchoolBranchResponse>(branch);
        }

        public async Task<SchoolBranchResponse> UpdateAsync(int id, SchoolBranchRequest branchRequest)
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

            _mapper.Map(branchRequest, branch);
            branch.UserUpdate = 1;

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

            branch.IsDelete = true;
            branch.UserUpdate = 1;

            await _schoolBranchRepository.UpdateAsync(branch);

            return _mapper.Map<SchoolBranchResponse>(branch);
        }
    }
}