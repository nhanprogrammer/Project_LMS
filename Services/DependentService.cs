using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class DependentService : IDependentService
    {
        private readonly IDependentRepository _dependentRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IMapper _mapper;

        public DependentService(IDependentRepository dependentRepository, ITeacherRepository teacherRepository, IMapper mapper)
        {
            _dependentRepository = dependentRepository;
            _teacherRepository = teacherRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<object>> AddAsync(DependentRequest request)
        {
            try
            {
                var teacher = await _teacherRepository.FindTeacherByUserCode(request.UserCode);
                var dependent = _mapper.Map<Dependent>(request);
                dependent.UserId = teacher.Id;
                await _dependentRepository.AddAsync(dependent);
                return new ApiResponse<object>(0, "Thêm người liên hệ thành công");

            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(1, "Thêm người liên hệ thất bại.")
                {
                    Data = "error : " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<object>> DeleteAsync(int id)
        {
            var dependent = await _dependentRepository.FindByIdAsync(id);
            if (dependent == null) return new ApiResponse<object>(1, "Người liên hệ không tồn tại.");
            try
            {
                await _dependentRepository.DeleteAsync(dependent);
                return new ApiResponse<object>(0, "Xóa người liên hệ thành công");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(1, "Xóa người liên hệ thất bại.")
                {
                    Data = "error : " + ex.Message
                };
            }
        }
    }
}
