using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;
using Twilio.Rest.Api.V2010.Account;

namespace Project_LMS.Services
{
    public class TranscriptService : ITranscriptService
    {
        private readonly IClassStudentRepository _classStudentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ITestExamTypeRepository _testExamTypeRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IAssignmentRepository _assignmentRepository;

        public TranscriptService(IClassStudentRepository classStudentRepository, IStudentRepository studentRepository, ITestExamTypeRepository testExamTypeRepository, ICloudinaryService cloudinaryService, IAssignmentRepository assignmentRepository)
        {
            _classStudentRepository = classStudentRepository;
            _studentRepository = studentRepository;
            _testExamTypeRepository = testExamTypeRepository;
            _cloudinaryService = cloudinaryService;
            _assignmentRepository = assignmentRepository;
        }

        public async Task<ApiResponse<object>> ExportExcelTranscriptAsync(TranscriptRequest request)
        {

            var student = await _studentRepository.FindStudentById(request.studentId);
            if (student == null)
                return new ApiResponse<object>(1, "Học viên không tồn tại.");

            var classStudents = await _classStudentRepository.FindStudentByStudentAcademic(student.Id, (int)request.DepartmentId);
            var classStudent = classStudents.FirstOrDefault(cs => cs.IsClassTransitionStatus == false);
            //if (classStudent == null)
            //    return new ApiResponse<object>(1, "Không tìm thấy lớp học của học viên.");

            var subjects = classStudent?.Class?.ClassSubjects?.Select(cs => cs.Subject).ToList() ?? new List<Subject?>();
            var assignments = classStudent?.User?.Assignments?
                .Where(asm => asm?.TestExam?.SemestersId == request.SemesterId)
                .ToList() ?? new List<Assignment>();

            var testExamTypes = await _testExamTypeRepository.GetAllAsync();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {

                var worksheet1 = package.Workbook.Worksheets.Add("Thông tin học viên");
                worksheet1.Cells["A1:E1"].Merge = true; // Gộp 5 cột đầu tiên ở hàng 1
                worksheet1.Cells["A1"].Value = "Thông tin"; // Đặt tiêu đề bảng
                                                            // 📌 Căn giữa và in đậm
                worksheet1.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet1.Cells["A1"].Style.Font.Bold = true;

                worksheet1.Cells[2, 1].Value = "Họ và tên";
                worksheet1.Cells[2, 2].Value = "Giới tính";
                worksheet1.Cells[2, 3].Value = "Ngày sinh";
                worksheet1.Cells[2, 4].Value = "Email";
                worksheet1.Cells[2, 5].Value = "Lớp";
                worksheet1.Cells[2, 6].Value = "GVCN";
                worksheet1.Cells[2, 7].Value = "Niên khóa";

                worksheet1.Cells[3, 1].Value = classStudent?.User?.FullName;
                worksheet1.Cells[3, 2].Value = classStudent?.User?.Gender?.Length > 0 ? (classStudent?.User.Gender[0] == true ? "Nam" : "Nữ") : "Chưa có dữ liệu";
                worksheet1.Cells[3, 3].Value = classStudent?.User?.BirthDate?.ToString("dd/MM/yyyy");
                worksheet1.Cells[3, 4].Value = classStudent?.User?.Email;
                worksheet1.Cells[3, 5].Value = classStudent?.Class?.Name;
                worksheet1.Cells[3, 6].Value = classStudent?.Class?.User?.FullName;
                worksheet1.Cells[3, 7].Value = $"{classStudent?.Class?.AcademicYear?.StartDate?.ToString("yyyy")} - {classStudent?.Class?.AcademicYear?.EndDate?.ToString("yyyy")}";

                var worksheet2 = package.Workbook.Worksheets.Add("Danh sách học viên");
                // Gộp 5 cột đầu tiên ở hàng 1 mà không dùng Range
                worksheet2.Cells["A1:E1"].Merge = true; // Gộp 5 cột đầu tiên ở hàng 1
                worksheet2.Cells["A1"].Value = "Bảng điểm"; // Đặt tiêu đề bảng
                                                            // 📌 Căn giữa và in đậm
                worksheet2.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet2.Cells["A1"].Style.Font.Bold = true;

                worksheet2.Cells[2, 1].Value = "STT";
                worksheet2.Cells[2, 2].Value = "Môn học";
                worksheet2.Cells[2, 3].Value = "Giảng viên";

                for (int index = 0; index < testExamTypes.Count; index++)
                {
                    worksheet2.Cells[2, 4 + index].Value = testExamTypes[index].PointTypeName;
                }
                worksheet2.Cells[2, testExamTypes.Count + 4].Value = "Tổng điểm trung bình";
                worksheet2.Cells[2, testExamTypes.Count + 5].Value = "Kết quả";
                worksheet2.Cells[2, testExamTypes.Count + 6].Value = "Ngày cập nhật";
                int row = 0;
                foreach (var subject in subjects)
                {
                    row++;
                    double totalScore = 0;
                    int totalCoefficient = 0;
                    int colmn = 3;
                    foreach (var testExamType in testExamTypes)
                    {
                        colmn++;
                        var assignment = classStudent?.User?.Assignments.FirstOrDefault(a => a.TestExam?.TestExamTypeId == testExamType.Id && a.TestExam.SubjectId == subject?.Id && a.TestExam.SemestersId == request.SemesterId && a.TestExam?.DepartmentId == request.DepartmentId);
                        if (assignment != null)
                        {
                            worksheet2.Cells[row + 2, colmn].Value = assignment.TotalScore;
                            totalScore += assignment.TotalScore * testExamType.Coefficient ?? 0;
                            totalCoefficient += testExamType.Coefficient ?? 1;
                        }
                        else
                        {
                            worksheet2.Cells[row + 2, colmn].Value = "Chưa có dữ liệu";
                        }
                    }

                    // Tính điểm trung bình
                    double averageScore = totalCoefficient > 0 ? (double)totalScore / totalCoefficient : 0;

                    // Lấy giáo viên dạy môn học này
                    var teachingAssignment = classStudent?.Class?.TeachingAssignments?
                        .FirstOrDefault(ta => ta?.Subject?.Id == subject?.Id && ta?.IsDelete == false);

                    worksheet2.Cells[row + 2, 1].Value = row;
                    worksheet2.Cells[row + 2, 2].Value = subject?.SubjectName;
                    worksheet2.Cells[row + 2, 3].Value = teachingAssignment?.User?.FullName ?? "Chưa có dữ liệu";
                    worksheet2.Cells[row + 2, colmn + 1].Value = averageScore;
                    worksheet2.Cells[row + 2, colmn + 2].Value = averageScore >= 5 ? "Đạt" : "Chưa đạt";
                    worksheet2.Cells[row + 2, colmn + 3].Value =
                        teachingAssignment?.UpdateAt?.ToString("dddd, dd/MM/yyyy HH:mm")
                        ?? teachingAssignment?.CreateAt?.ToString("dddd, dd/MM/yyyy HH:mm") ?? "Chưa có dữ liệu";


                }
                //======================
                worksheet2.Cells.AutoFitColumns();
                var filebytes = package.GetAsByteArray();
                string base64Excel = Convert.ToBase64String(filebytes);
                return new ApiResponse<object>(0, "Xuất excel thành công.") { Data = base64Excel };
            }
        }

        public async Task<ApiResponse<object>> GetTranscriptAsync(TranscriptRequest request)
        {
            //if (string.IsNullOrWhiteSpace(request.UserCode))
            //    return new ApiResponse<object>(1, "UserCode không được bỏ trống.");

            var student = await _studentRepository.FindStudentById(request.studentId);
            if (student == null)
                return new ApiResponse<object>(1, "Học viên không tồn tại.");

            var classStudents = await _classStudentRepository.FindStudentByStudentAcademic(student.Id, (int)request.DepartmentId);
            var classStudent = classStudents.FirstOrDefault(cs => cs.IsClassTransitionStatus == false);


            var subjects = classStudent?.Class?.ClassSubjects?.Select(cs => cs.Subject).ToList() ?? new List<Subject?>();

            var testExamTypes = await _testExamTypeRepository.GetAllAsync();
            var transcript = new List<object>();


            Console.WriteLine("subjects out " + subjects.Count);
            foreach (var subject in subjects)
            {
                Console.WriteLine("subjects in " + subjects.Count);
                double totalScore = 0;
                int totalCoefficient = 0;
                var testExamTypeItems = new List<Dictionary<string, object>>();

                foreach (var testExamType in testExamTypes)
                {
                    var testExamTypeItem = new Dictionary<string, object>();
                    var assignment = classStudent?.User?.Assignments.FirstOrDefault(a => a.TestExam?.TestExamTypeId == testExamType.Id && a.TestExam.SubjectId == subject?.Id && a.TestExam.SemestersId == request.SemesterId && a.TestExam?.DepartmentId == request.DepartmentId);

                    if (assignment != null)
                    {
                        testExamTypeItem[testExamType.PointTypeName ?? "N/A"] = assignment.TotalScore ?? 0;
                        totalScore += assignment.TotalScore * testExamType.Coefficient ?? 0;
                        totalCoefficient += testExamType.Coefficient ?? 1;
                    }
                    else
                    {
                        testExamTypeItem[testExamType.PointTypeName ?? "N/A"] = "Chưa có dữ liệu";
                    }

                    testExamTypeItems.Add(testExamTypeItem);
                }

                // Tính điểm trung bình
                double averageScore = totalCoefficient > 0 ? (double)totalScore / totalCoefficient : 0;
                testExamTypeItems.Add(new Dictionary<string, object> { { "averageScore", averageScore } });

                // Lấy giáo viên dạy môn học này
                var teachingAssignment = classStudent?.Class?.TeachingAssignments?
                    .FirstOrDefault(ta => ta?.Subject?.Id == subject?.Id && ta?.IsDelete == false);

                transcript.Add(new
                {
                    subjectName = subject?.SubjectName,
                    teacherName = teachingAssignment?.User?.FullName ?? "Chưa có giáo viên",
                    transcripts = testExamTypeItems,
                    updateAt = teachingAssignment?.UpdateAt ?? teachingAssignment?.CreateAt,
                });
            }
            var info = new
            {
                classStudent?.User?.Image,
                studentName = classStudent?.User?.FullName ?? "N/A",
                gender = (classStudent?.User?.Gender != null && classStudent.User?.Gender.Length > 0) ? classStudent.User?.Gender[0] : false,
                classStudent?.User?.BirthDate,
                classStudent?.User?.Email,
                className = classStudent?.Class?.Name ?? "N/A",
                teacherName = classStudent?.Class?.User?.FullName ?? "N/A",
                academic = $"{classStudent?.Class?.AcademicYear?.StartDate?.ToString("yyyy")} - {classStudent?.Class?.AcademicYear?.EndDate?.ToString("yyyy")}"
            };

            return new ApiResponse<object>(0, "Lấy bảng điểm học viên thành công.")
            {
                Data = new { info, transcript }
            };
        }

        public async Task<ApiResponse<object>> GetTranscriptByTeacherAsync(TranscriptTeacherRequest request)
        {
            var assignments = await _assignmentRepository.GetAllByClassAndSubjectAndSemesterAndSearch(request.ClassId, request.SubjectId, request.SemesterId, request.searchItem);
            var users = assignments.GroupBy(x => new
            {
                x.User.Id,
                x.User.FullName,
                x.User.BirthDate,
                x.User.UpdateAt
            }).ToList();
            var transcripts = new List<object>();
            var testExamTypes = await _testExamTypeRepository.GetAllAsync();
            foreach (var user in users)
            {
                var pointTypeNames = new List<Dictionary<string, object>>();
                double totalScore = 0;
                int totalCoefficient = 0;
                foreach (TestExamType test in testExamTypes)
                {
                    var pointTypeName = new Dictionary<string, object>();
                    foreach (Assignment asm in assignments.Where(asm => asm.UserId == user.Key.Id).ToList())
                    {
                        if (asm.TestExamId == test.Id)
                        {
                            pointTypeName.Add(test.PointTypeName, asm.TotalScore);
                            totalScore += (double)asm.TotalScore * asm.TestExam.TestExamType.Coefficient ?? 1;
                            totalCoefficient += asm.TestExam.TestExamType.Coefficient ?? 0;
                        }
                        else
                        {
                            pointTypeName.Add(test.PointTypeName, "Chưa có dữ liệu");
                        }
                    }
                    pointTypeNames.Add(pointTypeName);
                }
                pointTypeNames.Add(new Dictionary<string, object>{
                    {
                        "avgScore", totalScore / (totalCoefficient > 0?totalCoefficient:1)
                    }
                });

                transcripts.Add(new
                {
                    fullName = user.Key.FullName,
                    birthDate = user.Key.BirthDate,
                    scores = pointTypeNames,
                    avgScoreYear = await _assignmentRepository.AvgScoreByStudentAndClassAndSubjectAndSearch(user.Key.Id, request.ClassId, request.SubjectId),
                    updateDate = user.Key.UpdateAt
                });
            }
            return new ApiResponse<object>(0, "Lấy danh sách bảng điểm thành công.")
            {
                Data = new
                {
                    info = new
                    {
                        subjectName = assignments.Count > 0 ? assignments[0].TestExam.Subject.SubjectName : "Không có dữ liệu",
                        className = assignments.Count > 0 ? assignments[0].TestExam.Class.Name : "Không có dữ liệu",
                        classCode = assignments.Count > 0 ? assignments[0].TestExam.Class.ClassCode : "Không có dữ liệu",
                        startDate = assignments.Count > 0 ? assignments[0].TestExam.Class.StartDate.ToString() : "Không có dữ liệu",

                    },
                    transcripts
                }
            };

        }
    }
}

