using AutoMapper;
using FluentValidation;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class TeacherStatusHistoryService : ITeacherStatusHistoryService
    {
        private readonly ITeacherStatusHistoryRepository _teacherStatusHistoryRepository;
        private readonly IValidator<TeacherStatusHistoryRequest> _validator;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;
        private readonly ITeacherRepository _teacherRepository;

        public TeacherStatusHistoryService(ITeacherStatusHistoryRepository teacherStatusHistoryRepository, IValidator<TeacherStatusHistoryRequest> validator, ICloudinaryService cloudinaryService, IMapper mapper, ITeacherRepository teacherRepository)
        {
            _teacherStatusHistoryRepository = teacherStatusHistoryRepository;
            _validator = validator;
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
            _teacherRepository = teacherRepository;
        }

        public async Task<ApiResponse<object>> AddAsync(string statusName, TeacherStatusHistoryRequest request)
        {
            var valid = await _validator.ValidateAsync(request);
            if (!valid.IsValid)
            {
                return new ApiResponse<object>(1, $"Thêm thất bại.")
                {
                    Data = valid.Errors.Select(er => er.ErrorMessage).ToList()
                };
            }
            //try
            //{
                if(request.FileName != null)
                {
                    request.FileName= await _cloudinaryService.UploadDocxAsync(request.FileName);
                }
                
                var teacher = await _teacherRepository.FindTeacherByUserCode(request.UserCode);
                var teacherstatus = _mapper.Map<TeacherStatusHistory>(request);
                teacherstatus.UserId = teacher.Id;
                teacherstatus.IsActive = true;
                teacherstatus =  await _teacherStatusHistoryRepository.AddAsync(teacherstatus, statusName);
                teacher.TeacherStatusId = teacherstatus.TeacherStatusId;
                await _teacherRepository.UpdateAsync(teacher);
                return new ApiResponse<object>(1, $"Thêm thành công.");
            //}
            //catch (Exception ex) {
            //    return new ApiResponse<object>(1, $"Thêm thất bại.")
            //    {
            //        Data = "error : "+ex.Message
            //    };
            //}

        }
    }
}
