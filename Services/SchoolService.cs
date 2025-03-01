using AutoMapper;
using Project_LMS.Interfaces.Services;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;
using Project_LMS.Exceptions;
using Project_LMS.Helpers;

namespace Project_LMS.Services
{
    public class SchoolService : ISchoolService
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly IMapper _mapper;

        public SchoolService(ISchoolRepository schoolRepository, IMapper mapper)
        {
            _schoolRepository = schoolRepository;
            _mapper = mapper;
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

            await _schoolRepository.AddAsync(school);
            return _mapper.Map<SchoolResponse>(school);
        }

        public async Task<SchoolResponse> UpdateAsync(int id, SchoolRequest schoolRequest)
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

            _mapper.Map(schoolRequest, school);
            school.UserUpdate = 1;

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
    }
}