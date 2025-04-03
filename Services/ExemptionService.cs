using Aspose.Cells.Charts;
using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;
using Project_LMS.Repositories;

namespace Project_LMS.Services
{
    public class ExemptionService : IExemptionService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IExemptionRepository _exemptionRepository;
        private readonly IMapper _mapper;

        public ExemptionService(IStudentRepository studentRepository, IExemptionRepository exemptionRepository, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _exemptionRepository = exemptionRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<object>> AddAsync(ExemptionRequest request)
        {
            var student = await _studentRepository.FindStudentByUserCode(request.UserCode);
            if (student == null) return new ApiResponse<object>(1, "Học viên không tồn tại");
            var exemption =  _mapper.Map<Exemption>(request);
            try
            {
                exemption.UserId = student.Id;
                await _exemptionRepository.AddAsync(exemption);
                return new ApiResponse<object>(0, "Thêm miên giảm thành công");
            }
            catch (Exception ex) {
                return new ApiResponse<object>(1, "Thêm miễn giảm thất bại");
            }


        }
    }
}
