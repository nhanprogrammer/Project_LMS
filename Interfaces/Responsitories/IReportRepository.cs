using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public interface IReportRepository
    {
        Task<AcademicYear?> GetAcademicYearByIdAsync(int academicYearId);
        Task<List<ClassPerformanceReport>> GetClassPerformanceReportAsync(int academicYearId, int departmentId);
        Task<int> GetTotalStudentsAsync(int academicYearId);
        Task<int> GetTotalTeachersAsync(int academicYearId);
        Task<int> GetTotalClassesAsync(int academicYearId);
        Task<SchoolLevelStatisticsResponse> GetSchoolLevelStatisticsAsync(int academicYearId, bool isJuniorHigh);

        //Thống kê teacher
        Task<int> GetTotalClassesByTeacherAsync(int userId);
        Task<int> GetTotalOnlineClassesByTeacherAsync(int userId);
        Task<int> GetTotalUngradedAssignmentsByTeacherAsync(int userId);
        Task<int> GetTotalQuestionsReceivedByTeacherAsync(int userId);
        Task<List<ClassPerformanceReport>> GetClassPerformanceByTeacherAsync(int userId);
        Task<List<Semester>> GetSemestersByAcademicYearIdsAsync(List<int> academicYearIds);
        Task<List<TeachingAssignment>> GetTeachingAssignmentsByTeacherAsync(int teacherId);
        Task<Lesson?> GetFirstLessonByClassAndSubjectAsync(int classId, int subjectId, int teacherId);

        //Thống kê student
        Task<List<ClassStudent>> GetClassStudentsByStudentIdAsync(int studentId);
        Task<List<Assignment>> GetAssignmentsByStudentAndClassAsync(int studentId, int classId);
        Task<List<Subject>> GetSubjectsByClassIdAsync(int classId);
        Task<List<ClassSubject>> GetClassSubjectsWithSubjectsByClassIdAsync(int classId);
        Task<Lesson?> GetFirstLessonByClassAndSubjectAsync(int classId, int subjectId);


    }
}
