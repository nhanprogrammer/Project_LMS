using AutoMapper;
using FluentValidation;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;
using Project_LMS.Repositories;

namespace Project_LMS.Services
{
    public class DisciplinesService : IDisciplinesService
    {
        private readonly IDisciplineRepository _disciplineRepository;
        private readonly IValidator<DisciplineRequest> _validator;
        private readonly IMapper _mapper;
        private readonly IStudentRepository _studentRepository;
        private readonly IClassStudentRepository _classStudentRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public DisciplinesService(IDisciplineRepository disciplineRepository, IValidator<DisciplineRequest> validator, IMapper mapper, IStudentRepository studentRepository, IClassStudentRepository classStudentRepository, ICloudinaryService cloudinaryService)
        {
            _disciplineRepository = disciplineRepository;
            _validator = validator;
            _mapper = mapper;
            _studentRepository = studentRepository;
            _classStudentRepository = classStudentRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ApiResponse<object>> AddAsync(DisciplineRequest request)
        {
            var errors = new List<string>();
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return new ApiResponse<object>(1, "Thêm kỷ luật thất bại.")
                {
                    Data = errors
                };
            }
            var discipline = _mapper.Map<Discipline>(request);
            try
            {
                if (request.FileName != null)
                {
                    discipline.FileName = await _cloudinaryService.UploadDocAsync(request.FileName);
                    Console.WriteLine("url : " + discipline.FileName);
                }

                var student = await _studentRepository.FindStudentByUserCode(request.UserCode);
                discipline.UserId = student.Id;
                discipline.DisciplineDate = DateTime.Now;
                discipline.CreateAt = DateTime.Now;
                await _disciplineRepository.AddAsync(discipline);
                return new ApiResponse<object>(0, "Thêm kỷ luật thành công.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(1, "Thêm kỷ luật thất bại.")
                {
                    Data = "error : " + ex.Message
                };
            }


        }



        public async Task<ApiResponse<object>> UpdateAsync(UpdateDisciplineRequest request)
        {
            var errors = new List<string>();
            var discipline = await _disciplineRepository.GetByIdAsync(request.id);
            if (discipline == null) return new ApiResponse<object>(1, "Kỷ luật không tồn tại.");
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                errors = (validationResult.Errors.Select(e => e.ErrorMessage)).ToList();
                return new ApiResponse<object>(1, "Cập nhật kỷ luật thất bại.")
                {
                    Data = errors
                };
            }

            string Name = discipline.FileName;
            try
            {
                discipline = _mapper.Map(request, discipline);
                if (request.FileName != null)
                {
                    Name = await _cloudinaryService.UploadDocAsync(request.FileName);
                }

                discipline.FileName = Name;

                var student = await _studentRepository.FindStudentByUserCode(request.UserCode);
                discipline.UserId = student.Id;
                discipline.UpdateAt = DateTime.Now;
                await _disciplineRepository.UpdateAsync(discipline);
                return new ApiResponse<object>(0, "Cập nhật kỷ luật thành công.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(1, "Cập nhật kỷ luật thất bại.")
                {
                    Data = "error : " + ex.Message
                };
            }

        }

        public async Task<ApiResponse<object>> DeleteAsync(int id)
        {
            var discipline = await _disciplineRepository.GetByIdAsync(id);
            if (discipline == null)
            {
                return new ApiResponse<object>(1, "Kỷ luật không tồn tại.");
            }
            discipline.IsDelete = true;
            await _disciplineRepository.UpdateAsync(discipline);
            return new ApiResponse<object>(0, "Xóa kỷ luật thành công.");
        }

        public async Task<ApiResponse<object>> GetByIdAsync(int id)
        {
            var discipline = await _disciplineRepository.GetByIdAsync(id);
            if (discipline == null) return new ApiResponse<object>(1, "Kỷ luật không tồn tại.");
            //var classStudent = await _classStudentRepository.FindStudentByIdIsActive(discipline.UserId ?? 0);
            //string className = classStudent.Class.Name.ToString();
            var disciplineResponse = new
            {
                discipline.Id,
                discipline.DisciplineContent,
                discipline.FileName,
                discipline.DisciplineDate,
                discipline?.User?.FullName,
                //className,
                disciplineName = discipline?.Semester?.Name
            };
            return new ApiResponse<object>(0, "Đã tìm thấy kỷ luật.")
            {
                Data = disciplineResponse
            };
        }




    }
}