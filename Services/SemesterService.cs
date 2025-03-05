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
    public class SemesterService : ISemesterService
    {
        private readonly ISemesterRepository _semesterRepository;
        private readonly IMapper _mapper;

        public SemesterService(ISemesterRepository semesterRepository, IMapper mapper)
        {
            _semesterRepository = semesterRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SemesterResponse>> GetAllAsync()
        {
            var semesters = await _semesterRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SemesterResponse>>(semesters);
        }

        public async Task<SemesterResponse> GetByIdAsync(int id)
        {
            var semester = await _semesterRepository.GetByIdAsync(id);
            if (semester == null)
            {
                throw new NotFoundException("Không tìm thấy học kỳ.");
            }

            return _mapper.Map<SemesterResponse>(semester);
        }

        public async Task<SemesterResponse> CreateAsync(SemesterRequest request)
        {
            var errors = new List<ValidationError>();

            if (StringValidator.ContainsSpecialCharacters(request.Name))
                errors.Add(new ValidationError { Field = "Name", Error = "Tên học kỳ không được chứa ký tự đặc biệt" });

            if (!request.DateStart.HasValue || !DateTimeValidator.IsValidDateTime(request.DateStart?.ToString() ?? ""))
                errors.Add(new ValidationError { Field = "DateStart", Error = "Ngày bắt đầu không hợp lệ" });

            if (!request.DateEnd.HasValue || !DateTimeValidator.IsValidDateTime(request.DateEnd?.ToString() ?? ""))
                errors.Add(new ValidationError { Field = "DateEnd", Error = "Ngày kết thúc không hợp lệ" });

            if (request.DateStart.HasValue && request.DateEnd.HasValue)
            {
                if (request.DateStart > request.DateEnd)
                    errors.Add(new ValidationError { Field = "DateRange", Error = "Ngày bắt đầu không thể lớn hơn ngày kết thúc" });

                if (DateTimeValidator.IsPastDate(request.DateStart.Value))
                    errors.Add(new ValidationError { Field = "DateStart", Error = "Ngày bắt đầu không thể ở quá khứ" });
            }

            if (errors.Any())
            {
                throw new BadRequestException("Validation failed.", errors);
            }

            var semester = _mapper.Map<Semester>(request);
            semester.UserCreate = 1;

            await _semesterRepository.AddAsync(semester);

            return _mapper.Map<SemesterResponse>(semester);
        }

        public async Task<SemesterResponse> UpdateAsync(int id, SemesterRequest request)
        {
            var semester = await _semesterRepository.GetByIdAsync(id);
            if (semester == null)
            {
                throw new NotFoundException("Không tìm thấy học kỳ.");
            }

            var errors = new List<ValidationError>();

            if (StringValidator.ContainsSpecialCharacters(request.Name))
                errors.Add(new ValidationError { Field = "Name", Error = "Tên học kỳ không được chứa ký tự đặc biệt" });

            if (!request.DateStart.HasValue || !DateTimeValidator.IsValidDateTime(request.DateStart?.ToString() ?? ""))
                errors.Add(new ValidationError { Field = "DateStart", Error = "Ngày bắt đầu không hợp lệ" });

            if (!request.DateEnd.HasValue || !DateTimeValidator.IsValidDateTime(request.DateEnd?.ToString() ?? ""))
                errors.Add(new ValidationError { Field = "DateEnd", Error = "Ngày kết thúc không hợp lệ" });

            if (request.DateStart.HasValue && request.DateEnd.HasValue)
            {
                if (request.DateStart > request.DateEnd)
                    errors.Add(new ValidationError { Field = "DateRange", Error = "Ngày bắt đầu không thể lớn hơn ngày kết thúc" });

            }

            // if (request.DateStart.HasValue && request.DateStart < semester.DateStart)
            // {
            //     errors.Add(new ValidationError { Field = "DateStart", Error = "Ngày bắt đầu không thể nhỏ hơn ngày bắt đầu trước đó." });
            // }

            if (errors.Any())
            {
                throw new BadRequestException("Validation failed.", errors);
            }

            _mapper.Map(request, semester);
            semester.UserUpdate = 1;

            await _semesterRepository.UpdateAsync(semester);

            return _mapper.Map<SemesterResponse>(semester);
        }

        public async Task<SemesterResponse> DeleteAsync(int id)
        {
            var semester = await _semesterRepository.GetByIdAsync(id);
            if (semester == null)
            {
                throw new NotFoundException("Không tìm thấy học kỳ.");
            }

            semester.IsDelete = true;
            semester.UserUpdate = 1;

            await _semesterRepository.UpdateAsync(semester);

            return _mapper.Map<SemesterResponse>(semester);
        }
    }
}