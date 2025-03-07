using AutoMapper;
using Project_LMS.Interfaces.Services;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;
using Project_LMS.Exceptions;
using Project_LMS.Helpers;
using Project_LMS.Interfaces.Responsitories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_LMS.Services
{
    public class SchoolTransferService : ISchoolTransferService
    {
        private readonly ISchoolTransferRepository _schoolTransferRepository;
        private readonly ISchoolBranchRepository _schoolBranchRepository;
        private readonly IMapper _mapper;

        public SchoolTransferService(
            ISchoolTransferRepository schoolTransferRepository,
            ISchoolBranchRepository schoolBranchRepository,
            IMapper mapper)
        {
            _schoolTransferRepository = schoolTransferRepository;
            _schoolBranchRepository = schoolBranchRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SchoolTransferResponse>> GetAllAsync()
        {
            var schoolTransfers = await _schoolTransferRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SchoolTransferResponse>>(schoolTransfers);
        }

        public async Task<SchoolTransferResponse> GetByIdAsync(int id)
        {
            var schoolTransfer = await _schoolTransferRepository.GetByIdAsync(id);
            if (schoolTransfer == null)
            {
                throw new NotFoundException("Không tìm thấy thông tin chuyển trường.");
            }

            return _mapper.Map<SchoolTransferResponse>(schoolTransfer);
        }

        public async Task<SchoolTransferResponse> CreateAsync(SchoolTransferRequest transferRequest)
        {
            var errors = new List<ValidationError>();

            if (!IntValidator.IsValid(transferRequest.UserId.ToString() ?? ""))
                errors.Add(new ValidationError { Field = "StudentId", Error = "StudentId phải là số nguyên" });

            if (!IntValidator.IsValid(transferRequest.SchoolBranchesId.ToString() ?? ""))
                errors.Add(new ValidationError { Field = "SchoolBranchesId", Error = "SchoolBranchesId phải là số nguyên" });

            if (!DateTimeValidator.IsValidDateTime(transferRequest.TransferDate.ToString() ?? ""))
                errors.Add(new ValidationError { Field = "TransferDate", Error = "Ngày chuyển trường không hợp lệ" });

            if (errors.Any())
            {
                throw new BadRequestException("Validation failed.", errors);
            }

            // var student = await _studentRepository.GetByIdAsync(transferRequest.StudentId ?? 0);
            // if (student == null)
            // {
            //     throw new NotFoundException("Không tìm thấy học sinh với ID đã cho");
            // }

            var schoolBranch = await _schoolBranchRepository.GetByIdAsync(transferRequest.SchoolBranchesId ?? 0);
            if (schoolBranch == null)
            {
                throw new NotFoundException("Không tìm thấy chi nhánh trường với ID đã cho");
            }

            // var province = await _provinceRepository.GetByIdAsync(transferRequest.ProvinceId ?? 0);
            // if (province == null)
            // {
            //     throw new NotFoundException("Không tìm thấy tỉnh với ID đã cho");
            // }

            // var district = await _districtRepository.GetByIdAsync(transferRequest.DistrictId ?? 0);
            // if (district == null)
            // {
            //     throw new NotFoundException("Không tìm thấy huyện với ID đã cho");
            // }

            // var ward = await _wardRepository.GetByIdAsync(transferRequest.WardId ?? 0);
            // if (ward == null)
            // {
            //     throw new NotFoundException("Không tìm thấy xã với ID đã cho");
            // }

            var transfer = _mapper.Map<SchoolTransfer>(transferRequest);
            transfer.UserCreate = 1;
            transfer.IsDelete = false;

            await _schoolTransferRepository.AddAsync(transfer);

            return _mapper.Map<SchoolTransferResponse>(transfer);
        }

        public async Task<SchoolTransferResponse> UpdateAsync(int id, SchoolTransferRequest transferRequest)
        {
            var errors = new List<ValidationError>();

            if (!IntValidator.IsValid(transferRequest.UserId.ToString() ?? ""))
                errors.Add(new ValidationError { Field = "StudentId", Error = "StudentId phải là số nguyên" });

            if (!IntValidator.IsValid(transferRequest.SchoolBranchesId.ToString() ?? ""))
                errors.Add(new ValidationError { Field = "SchoolBranchesId", Error = "SchoolBranchesId phải là số nguyên" });

            if (!DateTimeValidator.IsValidDateTime(transferRequest.TransferDate.ToString() ?? ""))
                errors.Add(new ValidationError { Field = "TransferDate", Error = "Ngày chuyển trường không hợp lệ" });

            if (errors.Any())
            {
                throw new BadRequestException("Validation failed.", errors);
            }

            var transfer = await _schoolTransferRepository.GetByIdAsync(id);
            if (transfer == null)
            {
                throw new NotFoundException("Không tìm thấy thông tin chuyển trường.");
            }

            // var student = await _studentRepository.GetByIdAsync(transferRequest.StudentId ?? 0);
            // if (student == null)
            // {
            //     throw new NotFoundException("Không tìm thấy học sinh với ID đã cho");
            // }

            var schoolBranch = await _schoolBranchRepository.GetByIdAsync(transferRequest.SchoolBranchesId ?? 0);
            if (schoolBranch == null)
            {
                throw new NotFoundException("Không tìm thấy chi nhánh trường với ID đã cho");
            }

            // var province = await _provinceRepository.GetByIdAsync(transferRequest.ProvinceId ?? 0);
            // if (province == null)
            // {
            //     throw new NotFoundException("Không tìm thấy tỉnh với ID đã cho");
            // }

            // var district = await _districtRepository.GetByIdAsync(transferRequest.DistrictId ?? 0);
            // if (district == null)
            // {
            //     throw new NotFoundException("Không tìm thấy huyện với ID đã cho");
            // }

            // var ward = await _wardRepository.GetByIdAsync(transferRequest.WardId ?? 0);
            // if (ward == null)
            // {
            //     throw new NotFoundException("Không tìm thấy xã với ID đã cho");
            // }

            _mapper.Map(transferRequest, transfer);
            transfer.UserUpdate = 1;

            await _schoolTransferRepository.UpdateAsync(transfer);

            return _mapper.Map<SchoolTransferResponse>(transfer);
        }

        public async Task<SchoolTransferResponse> DeleteAsync(int id)
        {
            var transfer = await _schoolTransferRepository.GetByIdAsync(id);
            if (transfer == null)
            {
                throw new NotFoundException("Không tìm thấy thông tin chuyển trường.");
            }

            transfer.IsDelete = true;
            transfer.UserUpdate = 1;

            await _schoolTransferRepository.UpdateAsync(transfer);

            return _mapper.Map<SchoolTransferResponse>(transfer);
        }
    }
}