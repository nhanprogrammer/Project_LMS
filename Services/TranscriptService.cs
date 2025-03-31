
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

using QuestPDF.Fluent;

using QuestPDF.Helpers;
using QuestPDF.Infrastructure;



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

            var student = await _studentRepository.FindStudentById(request.StudentId);
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
                return new ApiResponse<object>(0, "Xuất excel thành công.") { Data = await _cloudinaryService.UploadExcelAsync(base64Excel) };
            }
        }

        public async Task<ApiResponse<object>> ExportExcelTranscriptByTeacherAsync(TranscriptTeacherRequest request)
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
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                int coutColumScore = testExamTypes.Count;
                var worksheet1 = package.Workbook.Worksheets.Add("Danh sách học viên");

                // Merge hàng 1 từ cột 1 đến cột (7 + coutColumScore)
                worksheet1.Cells[1, 1, 1, 7 + coutColumScore].Merge = true;
                worksheet1.Cells["A1"].Value = "Bảng điểm học viên";
                // Căn giữa và in đậm tiêu đề
                worksheet1.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet1.Cells["A1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                worksheet1.Cells["A1"].Style.Font.Bold = true;

                // Thiết lập tiêu đề cho hàng 2
                // Cột A: "STT"
                worksheet1.Cells[2, 1, 3, 1].Merge = true;
                worksheet1.Cells["A2"].Value = "STT";
                worksheet1.Cells["A2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet1.Cells["A2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                // Cột B: "Họ Và tên"
                worksheet1.Cells[2, 2, 3, 2].Merge = true;
                worksheet1.Cells["B2"].Value = "Họ Và tên";
                worksheet1.Cells["B2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet1.Cells["B2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                // Cột C: "Ngày sinh"
                worksheet1.Cells[2, 3, 3, 3].Merge = true;
                worksheet1.Cells["C2"].Value = "Ngày sinh";
                worksheet1.Cells["C2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet1.Cells["C2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                // Merge cho tiêu đề của nhóm điểm (tùy chỉnh theo số cột điểm)
                // Ví dụ: merge từ ô D2 đến ô (cột: coutColumScore + 1) của hàng 2
                worksheet1.Cells[2, 4, 2, coutColumScore + 1].Merge = true;
                worksheet1.Cells[2, 4].Value = assignments.Count > 0
                    ? assignments[0]?.TestExam?.Semesters?.Name
                    : "Chưa có dữ liệu";
                worksheet1.Cells[2, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet1.Cells[2, 4].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                // Gán giá trị cho từng loại điểm (không cần merge lại nếu chỉ là 1 ô)
                for (int colm = 0; colm < testExamTypes.Count; colm++)
                {
                    worksheet1.Cells[3, 4 + colm].Value = testExamTypes[colm].PointTypeName;
                    worksheet1.Cells[3, 4 + colm].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet1.Cells[3, 4 + colm].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                }

                // Cột "Trung bình"
                worksheet1.Cells[3, 4 + coutColumScore].Value = "Trung bình";
                worksheet1.Cells[3, 4 + coutColumScore].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet1.Cells[3, 4 + coutColumScore].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                // Cột "Điểm trung bình cả năm"
                worksheet1.Cells[2, 5 + coutColumScore, 3, 5 + coutColumScore].Merge = true;
                worksheet1.Cells[2, 5 + coutColumScore].Value = "Điểm trung bình cả năm";
                worksheet1.Cells[2, 5 + coutColumScore].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet1.Cells[2, 5 + coutColumScore].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                // Cột "Đạt"
                worksheet1.Cells[2, 6 + coutColumScore, 3, 6 + coutColumScore].Merge = true;
                worksheet1.Cells[2, 6 + coutColumScore].Value = "Đạt";
                worksheet1.Cells[2, 6 + coutColumScore].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet1.Cells[2, 6 + coutColumScore].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                // Cột "Ngày cập nhật"
                worksheet1.Cells[2, 7 + coutColumScore, 3, 7 + coutColumScore].Merge = true;
                worksheet1.Cells[2, 7 + coutColumScore].Value = "Ngày cập nhật";
                worksheet1.Cells[2, 7 + coutColumScore].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet1.Cells[2, 7 + coutColumScore].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                int row = 3;
                foreach (var user in users)
                {
                    row++;
                    worksheet1.Cells[row, 1].Value = row-3;
                    worksheet1.Cells[row,  1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet1.Cells[row,  1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    
                    worksheet1.Cells[row, 2].Value = user.Key.FullName;
                    worksheet1.Cells[row,  2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet1.Cells[row,  2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    
                    worksheet1.Cells[row, 3].Value = user.Key.BirthDate;
                    worksheet1.Cells[row,  3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet1.Cells[row,  3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    double totalScore = 0;
                    int totalCoefficient = 0;
                    int column = 3;
                    foreach (TestExamType test in testExamTypes)
                    {
                        column++;
                        var pointTypeName = new Dictionary<string, object>();
                        Assignment asm = assignments.Where(asm => asm.UserId == user.Key.Id && asm.TestExamId == test.Id).FirstOrDefault();
                        if (asm != null)
                        {
                            worksheet1.Cells[row, column].Value = asm.TotalScore;
                            worksheet1.Cells[row, column].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            worksheet1.Cells[row, column].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                            totalScore += (double)asm.TotalScore * asm.TestExam.TestExamType.Coefficient ?? 1;
                            totalCoefficient += asm.TestExam.TestExamType.Coefficient ?? 0;
                        }
                        else
                        {
                            worksheet1.Cells[row, column].Value = "Chưa có dữ liệu";
                            worksheet1.Cells[row, column].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            worksheet1.Cells[row, column].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        }

                    }
                    //totalScore / (totalCoefficient > 0 ? totalCoefficient : 1
                    worksheet1.Cells[row, column + 1].Value = (totalScore / (totalCoefficient > 0 ? totalCoefficient : 1));
                    worksheet1.Cells[row, column + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet1.Cells[row, column + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    worksheet1.Cells[row, column + 2].Value = await _assignmentRepository.AvgScoreByStudentAndClassAndSubjectAndSearch(user.Key.Id, request.ClassId, request.SubjectId);
                    worksheet1.Cells[row, column + 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet1.Cells[row, column + 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    worksheet1.Cells[row, column + 3].Value = await _assignmentRepository.AvgScoreByStudentAndClassAndSubjectAndSearch(user.Key.Id, request.ClassId, request.SubjectId) > 5 ? "Đạt" : "Chưa đạt";
                    worksheet1.Cells[row, column + 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet1.Cells[row, column + 3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    worksheet1.Cells[row, column + 4].Value = user.Key.UpdateAt?.ToString("dddd, dd/MM/yyyy, HH:mm:ss");
                    worksheet1.Cells[row, column + 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet1.Cells[row, column + 4].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                }
                worksheet1.Cells.AutoFitColumns();
                var filebytes = package.GetAsByteArray();
                string base64Excel = Convert.ToBase64String(filebytes);
                return new ApiResponse<object>(0, "Xuất excel thành công.") { Data = await _cloudinaryService.UploadExcelAsync(base64Excel) };

            }

        }



        public async Task<ApiResponse<object>> ExportPdfTranscriptByTeacherAsync(TranscriptTeacherRequest request)
        {
            var assignments = await _assignmentRepository.GetAllByClassAndSubjectAndSemesterAndSearch(
                request.ClassId, request.SubjectId, request.SemesterId, request.searchItem);

            var users = assignments.GroupBy(x => new
            {
                x.User.Id,
                x.User.FullName,
                x.User.BirthDate,
                x.User.UpdateAt
            }).ToList();

            var testExamTypes = await _testExamTypeRepository.GetAllAsync();
            QuestPDF.Settings.License = LicenseType.Community;

            var processedUsers = new List<(int rowNum, string fullName, string birthDate, List<string> scores, double avgSemester, double yearlyAvg, string status, string updatedAt)>();

            int rowNum = 0;
            foreach (var user in users)
            {
                rowNum++;

                string birthDate = user.Key.BirthDate?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? "N/A";

                double totalScore = 0;
                int totalCoefficient = 0;
                List<string> scores = new List<string>();

                foreach (var test in testExamTypes)
                {
                    var assignment = assignments.FirstOrDefault(asm =>
                        asm.UserId == user.Key.Id && asm.TestExamId == test.Id);

                    string scoreText = assignment != null
                        ? assignment.TotalScore.ToString()
                        : "N/A";

                    scores.Add(scoreText);

                    int coefficient = assignment?.TestExam?.TestExamType?.Coefficient ?? 1;
                    totalScore += (assignment?.TotalScore ?? 0) * coefficient;
                    totalCoefficient += coefficient;
                }

                double avgSemester = totalCoefficient > 0 ? totalScore / totalCoefficient : 0;
                double yearlyAvg = await _assignmentRepository.AvgScoreByStudentAndClassAndSubjectAndSearch(user.Key.Id, request.ClassId, request.SubjectId);
                string status = yearlyAvg > 5 ? "Đạt" : "Chưa đạt";
                string updatedAt = user.Key.UpdateAt?.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) ?? "N/A";

                processedUsers.Add((rowNum, user.Key.FullName, birthDate, scores, avgSemester, yearlyAvg, status, updatedAt));
            }

            // 📌 Tạo tài liệu PDF bằng QuestPDF
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A3.Landscape()); // Tăng chiều ngang bằng chế độ Landscape
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                    page.Header().Text("Bảng điểm học viên").FontSize(18).SemiBold().AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40); // STT
                            columns.RelativeColumn(2); // Họ và tên (co giãn nhiều hơn)
                            columns.ConstantColumn(90); // Ngày sinh
                            foreach (var _ in testExamTypes)
                                columns.RelativeColumn(1); // Cột điểm (co giãn)
                            columns.ConstantColumn(70); // Trung bình kỳ
                            columns.ConstantColumn(70); // Trung bình năm
                            columns.ConstantColumn(80); // Đạt/Chưa đạt
                            columns.ConstantColumn(140); // Ngày cập nhật
                        });

                        // Hàng tiêu đề
                        table.Header(header =>
                        {
                            header.Cell().Text("STT").Bold().AlignCenter();
                            header.Cell().Text("Họ và tên").Bold().AlignCenter();
                            header.Cell().Text("Ngày sinh").Bold().AlignCenter();

                            foreach (var test in testExamTypes)
                                header.Cell().Text(test.PointTypeName).Bold().AlignCenter();

                            header.Cell().Text("Trung bình kỳ").Bold().AlignCenter();
                            header.Cell().Text("Trung bình năm").Bold().AlignCenter();
                            header.Cell().Text("Đạt").Bold().AlignCenter();
                            header.Cell().Text("Ngày cập nhật").Bold().AlignCenter();
                        });

                        // Dữ liệu từng học sinh
                        foreach (var user in processedUsers)
                        {
                            table.Cell().Text(user.rowNum.ToString()).AlignCenter();
                            table.Cell().Text(user.fullName).WrapAnywhere();
                            table.Cell().Text(user.birthDate).WrapAnywhere();

                            foreach (var score in user.scores)
                                table.Cell().Text(score).WrapAnywhere().AlignCenter();

                            table.Cell().Text(user.avgSemester.ToString("F2")).AlignCenter();
                            table.Cell().Text(user.yearlyAvg.ToString("F2")).AlignCenter();
                            table.Cell().Text(user.status).WrapAnywhere().AlignCenter();
                            table.Cell().Text(user.updatedAt).WrapAnywhere().AlignCenter();
                        }
                    });
                });
            });

            // 📌 Lưu PDF vào MemoryStream
            using var memoryStream = new MemoryStream();
            document.GeneratePdf(memoryStream);
            var fileBytes = memoryStream.ToArray();
            string base64Pdf = Convert.ToBase64String(fileBytes);

            return new ApiResponse<object>(0, "Xuất PDF thành công.")
            {
                Data = await _cloudinaryService.UploadDocAsync(base64Pdf)
            };
        }






        public async Task<ApiResponse<object>> GetTranscriptAsync(TranscriptRequest request)
        {
            //if (string.IsNullOrWhiteSpace(request.UserCode))
            //    return new ApiResponse<object>(1, "UserCode không được bỏ trống.");

            var student = await _studentRepository.FindStudentById(request.StudentId);
            if (student == null)
                return new ApiResponse<object>(1, "Học viên không tồn tại.");

            var classStudents = await _classStudentRepository.FindStudentByStudentAcademic(student.Id, (int)request.DepartmentId);
            var classStudent = classStudents.FirstOrDefault(cs => cs.IsClassTransitionStatus == false);


            var subjects = classStudent?.Class?.ClassSubjects?.Select(cs => cs.Subject).ToList() ?? new List<Subject?>();

            var testExamTypes = await _testExamTypeRepository.GetAllAsync();
            var transcript = new List<object>();


            //Console.WriteLine("subjects out " + subjects.Count);
            foreach (var subject in subjects)
            {
                //Console.WriteLine("subjects in " + subjects.Count);
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
                    Assignment asm = assignments.Where(asm => asm.UserId == user.Key.Id && asm.TestExamId == test.Id).FirstOrDefault();
                    {
                        if (asm != null)
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

