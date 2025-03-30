using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface IAssignmentRepository : IRepository<Assignment>
{
    Task<List<Assignment>> GetAllByClassAndSubjectAndSemesterAndSearch(int classId, int subjectId,int semesterId, string searchItem);
    Task<double> AvgScoreByStudentAndClassAndSubjectAndSearch(int studentId,int classId, int subjectId);
}