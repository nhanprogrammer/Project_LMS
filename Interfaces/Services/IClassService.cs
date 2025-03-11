using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Interfaces
{
    public interface IClassService
    {
        Task<ApiResponse<PaginatedResponse<ClassListResponse>>> GetClassList(ClassRequest classRequest);
        Task SaveClass(ClassSaveRequest classSaveRequest);

        // Lấy danh sách môn học, nhưng loại trừ các môn có ID trong danh sách đã chọn
        Task<ApiResponse<List<SubjectListResponse>>> GetSubjectsExcluding(List<int> excludedSubjectIds);

        // Lấy danh sách môn học mà khối này đã sử dụng từ khóa trước
        Task<ApiResponse<List<SubjectListResponse>>> GetInheritedSubjects(int academicYearId, int departmentId);
        Task<bool> DeleteClass(List<int> classId);
        Task<ClassDetailResponse> GetClassDetail(int classId);
        Task<bool> SaveStudentStatus(int studentId, int statusId);
        Task<FileContentResult> ExportClassListToExcel(int academicYearId, int departmentId);
        Task CreateClassByFile(IFormFile file);
    }
}
