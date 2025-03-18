using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface IStudentService
{
    public Task<ApiResponse<object>> AddAsync(StudentRequest request);
    public Task<ApiResponse<object>> UpdateAsync(UpdateStudentRequest request);
    public Task<ApiResponse<PaginatedResponse<object>>> GetAllStudentOfRewardOrDisciplines(bool isReward,int academicId, int departmentId,PaginationRequest request,string columnm,bool orderBy, string searchItem);
    public Task<ApiResponse<object>> LearningOutcomesOfStudent(int studentId, int classId);
   
}