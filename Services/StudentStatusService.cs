using System.Net.WebSockets;
using AutoMapper;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Repositories;

namespace Project_LMS.Services
{
    public class StudentStatusService : IStudentStatusService
    {
        private readonly IStudentStatusRepository _studentStatusRepository;
        private readonly IMapper _mapper;
        public StudentStatusService(IStudentStatusRepository  studentStatusRepository, IMapper mapper)
        {
            _studentStatusRepository = studentStatusRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<StudentStatusResponse>>> GetAll()
        {
            var studentStatus = await _studentStatusRepository.GetAllAsync();
            if (studentStatus.Any())
            {
                var studentStatusResponses = studentStatus.Select(user => _mapper.Map<StudentStatusResponse>(studentStatus)).ToList();
                return new ApiResponse<List<StudentStatusResponse>>(0, "GetAll studentStatus success.")
                {
                    Data = studentStatusResponses
                };
            }
            else
            {
                return new ApiResponse<List<StudentStatusResponse>>(1, "No studentStatus found.");
            }
        }

        public async Task<ApiResponse<StudentStatusResponse>> Search(int id)
        {
            var studentStatus = await _studentStatusRepository.FindAsync(id);

            if (studentStatus == null)
            {
                return new ApiResponse<StudentStatusResponse>(1, "No studentstatus found.");
            }
            else
            {
                var studentStatusResponse = _mapper.Map<StudentStatusResponse>(studentStatus);
                return new ApiResponse<StudentStatusResponse>(0, "Successfully retrieved studentstatus.")
                {
                    Data = studentStatusResponse
                };
            }
        }

    }
}
