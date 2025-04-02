using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;
using Aspose.Cells;
using AutoMapper;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet.Core;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;
using Project_LMS.Repositories;

namespace Project_LMS.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IClassStudentRepository _classStudentRepository;
    private readonly IClassRepository _classRepository;
    private readonly IStudentStatusRepository _studentStatusRepository;
    private readonly IEmailService _emailService;
    private readonly IClassSubjectRepository _classSubjectRepository;
    private readonly ITestExamTypeRepository _testExamTypeRepository;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IMapper _mapper;
    private readonly ILogger<StudentService> _logger;
    private readonly ICodeGeneratorService _codeGeneratorService;
    private readonly IAcademicYearRepository _academicYearRepository;

    private readonly List<string> _expectedHeaders = new List<string>
    {
        "studentStatusId", "classId", "userCode", "fullName", "email", "startDate", "image", "gender",
        "ethnicity", "religion", "placeOfBirth", "birthDate", "studyMode", "phone", "address",
        "provinceId", "districtId", "wardId", "alias", "admissionType", "national",
        "fullnameFather", "birthFather", "workFather", "phoneFather",
        "fullnameMother", "birthMother", "workMother", "phoneMother",
        "fullnameGuardianship", "birthGuardianship", "workGuardianship", "phoneGuardianship"
    };
    public StudentService(ITestExamTypeRepository testExamTypeRepository, IStudentRepository studentRepository, IClassStudentRepository classStudentRepository, IClassRepository classRepository, IStudentStatusRepository studentStatusRepository, IEmailService emailService, IClassSubjectRepository classSubjectRepository, ICloudinaryService cloudinaryService, ICodeGeneratorService codeGeneratorService, IMapper mapper, ILogger<StudentService> logger, IAcademicYearRepository academicYearRepository)
    {
        _academicYearRepository = academicYearRepository ?? throw new ArgumentNullException(nameof(academicYearRepository));
        _testExamTypeRepository = testExamTypeRepository ?? throw new ArgumentNullException(nameof(testExamTypeRepository));
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _classStudentRepository = classStudentRepository ?? throw new ArgumentNullException(nameof(classStudentRepository));
        _classRepository = classRepository ?? throw new ArgumentNullException(nameof(classRepository));
        _studentStatusRepository = studentStatusRepository ?? throw new ArgumentNullException(nameof(studentStatusRepository));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _classSubjectRepository = classSubjectRepository ?? throw new ArgumentNullException(nameof(classSubjectRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _codeGeneratorService = codeGeneratorService ?? throw new ArgumentNullException(nameof(codeGeneratorService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cloudinaryService = cloudinaryService ?? throw new ArgumentNullException(nameof(cloudinaryService));
    }

    public async Task<ApiResponse<object>> AddAsync(StudentRequest request)
    {
        var student = _mapper.Map<User>(request);

        student.IsDelete = false;
        student.CreateAt = DateTime.Now;
        student.Username = await GeneratedUsername(request.Email);
        string password = await GenerateSecurePassword(10);
        student.Password = BCrypt.Net.BCrypt.HashPassword(password);

        Console.WriteLine(request.UserCode + "usercode");
        if (string.IsNullOrEmpty(request.UserCode))
        {
            Console.WriteLine("UserCode null");
            student.UserCode = await _codeGeneratorService.GenerateCodeAsync("HS", async code =>
                await _studentRepository.FindStudentByUserCode(code) != null);
            Console.WriteLine(student.UserCode + "usercode sau khi gen code");
        }
        else
        {
            Console.WriteLine("UserCode không null");
            student.UserCode = request.UserCode;
        }
        var valids = await ValidateStudentRequest(request);
        if (await _studentRepository.FindStudentByUserCode(request.UserCode) != null)
        {
            valids.Add($"UserCode đã tồn tại.");
        }

        if (valids.Count > 0) return new ApiResponse<object>(1, "Thêm học viên thất bại.")
        {
            Data = valids
        };
        try
        {
            if (request.Image != null)
            {
                Console.WriteLine("Url Đã chạy vào");
                string url = await _cloudinaryService.UploadImageAsync(request.Image);
                student.Image = url;
            }
            Console.WriteLine(student.Image + " ảnh sau khi upload");
            var user = await _studentRepository.AddAsync(student);
            Task.Run(async () =>
            {
                await ExecuteEmail(user.Email, user.FullName, user.Username, password);
            });
            await _classStudentRepository.AddAsync(new ClassStudentRequest()
            {
                UserId = user.Id,
                ClassId = request.ClassId
            });


            _logger.LogInformation("Thêm học viên thành công.");
            return new ApiResponse<object>(0, "Thêm học viên thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Lỗi cơ sở dữ liệu khi thêm học viên.");
            return new ApiResponse<object>(1, "Lỗi cơ sở dữ liệu khi thêm học viên. Vui lòng thử lại sau.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi không xác định khi thêm học viên. Dữ liệu: {@Student}", student);
            return new ApiResponse<object>(1, "Đã xảy ra lỗi không xác định khi thêm học viên. Vui lòng thử lại sau: " + ex.Message);
        }

    }

    public async Task<ApiResponse<object>> DeleteAsync(List<string> userCodes)
    {
        if (userCodes.Count <= 0) return new ApiResponse<object>(1, "UserCodes phải có ít nhất 1 phần tử.");
        List<string> userCodeDeleteSuccess = new List<string>();
        List<string> userCodeDeleteError = new List<string>();
        foreach (var userCode in userCodes)
        {
            var studentFind = await _studentRepository.FindStudentByUserCode(userCode);
            if (studentFind != null && userCode != null)
            {
                studentFind.IsDelete = true;
                await _studentRepository.DeleteAsync(studentFind);
                userCodeDeleteSuccess.Add(userCode);
            }
            else
            {
                userCodeDeleteError.Add(userCode);
            }
        }
        ;
        if (userCodeDeleteSuccess.Count > 0)
        {
            return new ApiResponse<object>(0, "Xóa học viên thành công.")
            {
                Data =
                new
                {
                    Success = userCodeDeleteSuccess,
                    Error = userCodeDeleteError
                },

            };
        }
        else
        {
            return new ApiResponse<object>(1, "Xóa học viên thất bại.")
            {
                Data =
               new
               {
                   Success = userCodeDeleteSuccess,
                   Error = userCodeDeleteError
               },

            };
        }

    }

    public async Task ExecuteEmail(string email, string fullname, string username, string password)
    {
        var content = $@"
                            <html>
                            <head>
                                <meta charset='UTF-8'>
                                <title>Thông báo tạo tài khoản sinh viên</title>
                                <style>
                                    body {{ font-family: Arial, sans-serif; line-height: 1.6; background-color: #f4f4f4; padding: 20px; }}
                                    .container {{ max-width: 600px; margin: 0 auto; background: white; padding: 20px; border-radius: 10px; box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1); }}
                                    .header {{ background-color: #007BFF; color: white; text-align: center; padding: 15px; font-size: 22px; font-weight: bold; border-radius: 10px 10px 0 0; }}
                                    .content {{ padding: 20px; font-size: 16px; color: #333; }}
                                    .footer {{ font-size: 12px; text-align: center; color: gray; margin-top: 20px; }}
                                    .button {{ display: inline-block; padding: 12px 20px; margin-top: 10px; background-color: #28a745; color: white; text-decoration: none; border-radius: 5px; font-weight: bold; }}
                                    .button:hover {{ background-color: #218838; }}
                                </style>
                            </head>
                            <body>
                                <div class='container'>
                                    <div class='header'>📢 Thông Báo Tạo Tài Khoản Sinh Viên</div>
                                    <div class='content'>
                                        <p>Xin chào <b>{fullname}</b>,</p>
                                        <p>Trường <b>ABC</b> đã tạo tài khoản sinh viên cho bạn. Dưới đây là thông tin đăng nhập:</p>

                                        <ul>
                                            <li><b>🎓 Tên đăng nhập (Username):</b> {username}</li>
                                            <li><b>🔑 Mật khẩu tạm thời:</b> {password}</li>
                                        </ul>

                                        <p>🔹 <b>Bạn có thể đăng nhập vào hệ thống tại:</b> <a href='{"abc"}' target='_blank'>{"abc"}</a></p>
                                        <p>⚠ <b>Lưu ý:</b> Vui lòng <b>đổi mật khẩu</b> ngay sau khi đăng nhập để bảo mật tài khoản.</p>
                
                                        <p><a class='button' href='{"link"}' target='_blank'>🔒 Đổi Mật Khẩu Ngay</a></p>

                                        <p>Nếu có bất kỳ vấn đề nào, vui lòng liên hệ phòng Công Nghệ Thông Tin.</p>

                                        <p>Trân trọng,<br>📧 <b>Phòng Công Nghệ Thông Tin</b><br>{"ABC"}</p>
                                    </div>
                                    <div class='footer'>© 2024 {"ABC"}. Mọi quyền được bảo lưu.</div>
                                </div>
                            </body>
                            </html>";
        try
        {
            await _emailService.SendMailAsync(email, "Thông báo tạo tài khoản sinh viên", content);
            _logger.LogInformation($"Email sent to {email}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending email to {email}: {ex.Message}");
        }
    }

    public async Task<ApiResponse<object>> ExportExcelLearningProcess(int studentId, int classId)
    {
        // Kiểm tra dữ liệu đầu vào
        if (studentId <= 0 || classId <= 0)
            return new ApiResponse<object>(1, "ID học viên hoặc lớp học không hợp lệ");

        // Lấy thông tin lớp học và học viên
        var cs = await _classStudentRepository.FindStudentByClassAndStudent(classId, studentId);
        if (cs?.Class == null) // Kiểm tra cả Class để tránh null reference
            return new ApiResponse<object>(1, "Không tìm thấy học viên trong lớp học");
        List<Task> tasks = new List<Task>();
        //try
        //{
        // Khởi tạo workbook và worksheet
        using var workbook = new Workbook();
        Worksheet sheet1 = workbook.Worksheets[0];

        tasks.Add(Task.Run(() =>
        {
            // Định dạng tiêu đề chung
            sheet1.Cells["A1"].PutValue("Thông tin chung");
            sheet1.Cells.Merge(0, 0, 1, 9);
            var titleStyle = sheet1.Cells["A1"].GetStyle();
            titleStyle.IsTextWrapped = true;
            titleStyle.HorizontalAlignment = TextAlignmentType.Center;
            sheet1.Cells["A1"].SetStyle(titleStyle);

            // Tiêu đề cột
            string[] headers = new[]
            {
            "Niên khóa", "Khoa - khối", "Mã lớp học", "Tên lớp học",
            "Giáo viên chủ nhiệm", "Số lượng học viên", "Loại lớp học",
            "Số lượng môn học", "Mô tả"
    };

            for (int i = 0; i < headers.Length; i++)
            {
                sheet1.Cells[1, i].PutValue(headers[i]);
            }
        }));

        // Dữ liệu hàng
        var academicYear = cs.Class.AcademicYear;
        tasks.Add(Task.Run(() =>
        {
            sheet1.Cells[2, 0].PutValue(academicYear != null
                ? $"{academicYear.StartDate?.ToString("yyyy") ?? "N/A"} - {academicYear.EndDate?.ToString("yyyy") ?? "N/A"}"
                : "N/A");
            sheet1.Cells[2, 1].PutValue(cs.Class.Department?.Name ?? "N/A");
            sheet1.Cells[2, 2].PutValue(cs.Class.ClassCode ?? "N/A");
            sheet1.Cells[2, 3].PutValue(cs.Class.Name ?? "N/A");
            sheet1.Cells[2, 4].PutValue(cs.Class.User?.FullName ?? "N/A");
            sheet1.Cells[2, 5].PutValue(cs.Class.ClassStudents?.Count ?? 0);
            sheet1.Cells[2, 6].PutValue(cs.Class.ClassType?.Name ?? "N/A");
            sheet1.Cells[2, 7].PutValue(cs.Class.ClassSubjects?.Count ?? 0);
            sheet1.Cells[2, 8].PutValue(cs.Class.Description ?? "N/A");
            // Tự động điều chỉnh độ rộng cột
            sheet1.AutoFitColumns();
        }));
        //===========================================
        //Sheet2 Kết quả học tập 
        var student = await _studentRepository.FindStudentById(studentId);

        Worksheet sheet2 = workbook.Worksheets.Add("Kết quả học tập");
        // Khởi tạo worksheet và đặt tên
        tasks.Add(Task.Run(() =>
        {
            // Định dạng tiêu đề chính
            sheet2.Cells["A1"].PutValue("Kết quả học tập");
            sheet2.Cells.Merge(0, 0, 1, 9);
            var mainTitleStyle = sheet2.Cells["A1"].GetStyle();
            mainTitleStyle.IsTextWrapped = true;
            mainTitleStyle.HorizontalAlignment = TextAlignmentType.Center;
            mainTitleStyle.Font.IsBold = true; // Thêm in đậm cho tiêu đề chính
            sheet2.Cells["A1"].SetStyle(mainTitleStyle);
        }));


        // Định nghĩa và ghi tiêu đề học kỳ
        string[] semesterHeaders = { "Học kỳ 1", "Học kỳ 2", "Cả năm" };
        for (int colm = 0; colm < semesterHeaders.Length; colm++)
        {
            int startColumn = colm * 3; // Cột đầu của từng học kỳ
            sheet2.Cells[1, startColumn].PutValue(semesterHeaders[colm]);
            sheet2.Cells.Merge(1, startColumn, 1, 3);

            var semesterStyle = sheet2.Cells[1, startColumn].GetStyle();
            semesterStyle.IsTextWrapped = true;
            semesterStyle.HorizontalAlignment = TextAlignmentType.Center;
            semesterStyle.Font.IsBold = true;
            sheet2.Cells[1, startColumn].SetStyle(semesterStyle);
        }
        sheet2.AutoFitColumns();



        tasks.Add(Task.Run(() =>
            {
                string[] subHeaders = { "Học lực", "Hạnh kiểm", "Điểm trung bình" };
                // Thêm sub-header
                for (int i = 0; i < 9; i++)
                {
                    sheet2.Cells[2, i].PutValue(subHeaders[i % 3]);
                    var subHeaderStyle = sheet2.Cells[2, i].GetStyle();
                    subHeaderStyle.HorizontalAlignment = TextAlignmentType.Center;
                    sheet2.Cells[2, i].SetStyle(subHeaderStyle);
                }
                sheet2.AutoFitColumns();
            }));
        tasks.Add(Task.Run(() =>
        {
            var assignments = student.Assignments.Where(a => a.TestExam.ClassId == classId).ToList();
            double CalculateSemesterScore(string semesterName)
            {
                int count = 0;
                double score = assignments
                    .Where(a => a.TestExam.Semesters.Name.ToLower().Contains(semesterName))
                    .Sum(a =>
                    {
                        if (a.TotalScore > 0 && a.TotalScore != null)
                        {
                            count += a.TestExam?.TestExamType?.Coefficient ?? 1;
                            return (a.TotalScore * a.TestExam?.TestExamType?.Coefficient) ?? 0;
                        }
                        else
                        {
                            return 0;
                        }
                    });
                return (double)count > 0 ? score / count : score;
            }
            (string academicPerformance, string conduct, double score) EvaluatePerformance(double score) =>
           score == 0
               ? ("Không có dữ liệu", "không có dữ liệu", 0)
               : (score switch
               {
                   >= 8.0 => "Giỏi",
                   >= 6.5 => "Khá",
                   >= 5.0 => "Trung bình",
                   _ => "Yếu"
               },
                   score > 5 ? "Tốt" : "Khá",
                   score);
            var score1 = CalculateSemesterScore("học kỳ 1");
            var score2 = CalculateSemesterScore("học kỳ 2");
            var (perf1, cond1, avg1) = EvaluatePerformance(score1);
            var (perf2, cond2, avg2) = EvaluatePerformance(score2);
            var (perfYear, condYear, avgYear) = score1 == 0 || score2 == 0
                ? ("Không có dữ liệu", "không có dữ liệu", 0.0)
                : EvaluatePerformance((score2 * 2 + score1) / 3);
            // Ghi dữ liệu
            object[] data = { perf1, cond1, avg1, perf2, cond2, avg2, perfYear, condYear, avgYear };
            for (int i = 0; i < data.Length; i++)
            {
                sheet2.Cells[3, i].PutValue(data[i]);
                var cellStyle = sheet2.Cells[3, i].GetStyle();
                cellStyle.HorizontalAlignment = TextAlignmentType.Center;

            }
            sheet2.AutoFitColumns();
        }));
        // Tự động điều chỉnh độ rộng cột

        // Sheet 3: Subject Scores by Semester
        // Hàm lấy danh sách môn học
        var classSubjects = await _classSubjectRepository.GetAllByClass(classId);
        var subjects = classSubjects.Select(x => x.Subject).ToList();

        var classOfStudent = await _classRepository.FindClassById(classId);
        if (classOfStudent == null) return new ApiResponse<object>(1, "Không tìm thấy sinh viên trong lớp học");

        var testExamTypesId = classOfStudent.TestExams?
            .Where(te => te.TestExamTypeId.HasValue) // Loại bỏ null
            .Select(te => te.TestExamTypeId.Value) // Chuyển về List<int>
            .Distinct()
            .ToList() ?? new List<int>();
        var testExamTypes = await _testExamTypeRepository.GetAllByIds(testExamTypesId);
        Worksheet sheet3 = workbook.Worksheets.Add("Bảng điểm học kỳ 1");
        tasks.Add(Task.Run(() =>
        {

            // Định dạng tiêu đề chính
            sheet3.Cells["A1"].PutValue("Học kỳ 1");
            sheet3.Cells.Merge(0, 0, 1, testExamTypes.Count + 2);
            var titleStyle3 = sheet3.Cells["A1"].GetStyle();
            titleStyle3.IsTextWrapped = true;
            titleStyle3.HorizontalAlignment = TextAlignmentType.Center;
            titleStyle3.Font.IsBold = true;
            sheet3.Cells["A1"].SetStyle(titleStyle3);
            sheet3.Cells[1, 0].PutValue("Môn học");
        }));
        Worksheet sheet4 = workbook.Worksheets.Add("Bảng điểm học kỳ 2");
        tasks.Add(Task.Run(() =>
        {

            // Định dạng tiêu đề chính
            sheet4.Cells["A1"].PutValue("Học kỳ 2");
            sheet4.Cells.Merge(0, 0, 1, testExamTypes.Count + 2);
            var titleStyle4 = sheet4.Cells["A1"].GetStyle();
            titleStyle4.IsTextWrapped = true;
            titleStyle4.HorizontalAlignment = TextAlignmentType.Center;
            titleStyle4.Font.IsBold = true;
            sheet4.Cells["A1"].SetStyle(titleStyle4);

            sheet4.Cells[1, 0].PutValue("Môn học");
        }));


        tasks.Add(Task.Run(() =>
        {
            for (int row = 0; row < subjects.Count; row++)
            {
                sheet3.Cells[row + 2, 0].PutValue(subjects[row].SubjectName);
                sheet4.Cells[row + 2, 0].PutValue(subjects[row].SubjectName);
            }
        }));

        tasks.Add(Task.Run(() =>
        {
            for (int col = 0; col < testExamTypes.Count; col++)
            {
                sheet3.Cells[1, col + 1].PutValue(testExamTypes[col].PointTypeName);
                sheet4.Cells[1, col + 1].PutValue(testExamTypes[col].PointTypeName);
            }
        }));
        sheet3.Cells[1, testExamTypes.Count + 1].PutValue("Điểm trung bình");

        sheet4.Cells[1, testExamTypes.Count + 1].PutValue("Điểm trung bình");



        tasks.Add(Task.Run(() =>
        {
            for (int row = 0; row < subjects.Count; row++)
            {
                double totalScore = 0;
                int totalCoefficient = 0;
                for (int col = 0; col < testExamTypes.Count; col++)
                {
                    int index = 0;
                    var assignments = student.Assignments.Where(asm => asm.TestExam.Semesters.Name.ToLower().Contains("học kỳ 1")).ToList();
                    foreach (Assignment asm in assignments)
                    {
                        if (asm.TestExam.SubjectId == subjects[row].Id && asm.TestExam.TestExamTypeId == testExamTypes[col].Id && asm.TotalScore > 0)
                        {
                            sheet3.Cells[row + 2, col + 1].PutValue(asm.TotalScore);
                            totalScore += asm.TotalScore * asm.TestExam.TestExamType.Coefficient ?? 0;
                            totalCoefficient += asm.TestExam.TestExamType.Coefficient ?? 1;
                            index = 1;
                        }
                    }
                    if (index == 0)
                    {
                        sheet3.Cells[row + 2, col + 1].PutValue("Chưa có điểm");
                    }
                }
                sheet3.Cells[row + 2, testExamTypes.Count + 1].PutValue((double)totalScore / totalCoefficient);
            }
            sheet3.AutoFitColumns();
        }));

        tasks.Add(Task.Run(() =>
        {
            for (int row = 0; row < subjects.Count; row++)
            {
                double totalScore = 0;
                int totalCoefficient = 0;
                for (int col = 0; col < testExamTypes.Count; col++)
                {
                    int index = 0;

                    var assignments = student.Assignments.Where(asm => asm.TestExam.Semesters.Name.ToLower().Contains("học kỳ 2")).ToList();
                    foreach (var asm in assignments)
                    {
                        if (asm.TestExam.SubjectId == subjects[row].Id && asm.TestExam.TestExamTypeId == testExamTypes[col].Id && asm.TotalScore > 0)
                        {
                            sheet4.Cells[row + 2, col + 1].PutValue(asm.TotalScore);
                            totalScore += asm.TotalScore * asm.TestExam.TestExamType.Coefficient ?? 0;
                            totalCoefficient += asm.TestExam.TestExamType.Coefficient ?? 1;
                            index = 1;

                        }

                    }
                    if (index == 0)
                    {
                        sheet4.Cells[row + 2, col + 1].PutValue("Chưa có điểm");
                    }
                }
                sheet4.Cells[row + 2, testExamTypes.Count + 1].PutValue((double)totalScore / totalCoefficient);
            }
            sheet4.AutoFitColumns();
        }));
        //sheet5
        Worksheet sheet5 = workbook.Worksheets.Add("Danh sách khen thưởng");
        tasks.Add(Task.Run(() =>
        {
            // Định dạng tiêu đề chính
            sheet5.Cells["A1"].PutValue("Danh sách khen thưởng");
            sheet5.Cells.Merge(0, 0, 1, 4);
            var titleStyle5 = sheet5.Cells["A1"].GetStyle();
            titleStyle5.IsTextWrapped = true;
            titleStyle5.HorizontalAlignment = TextAlignmentType.Center;
            titleStyle5.Font.IsBold = true;
            sheet5.Cells["A1"].SetStyle(titleStyle5);
        }));
        tasks.Add(Task.Run(() =>
        {
            sheet5.Cells[1, 0].PutValue("STT");
            sheet5.Cells[1, 1].PutValue("Nội dung khen thưởng");
            sheet5.Cells[1, 2].PutValue("Quyết định khen thưởng");
            sheet5.Cells[1, 3].PutValue("Ngày quyết định");
        }));
        //Thêm dữ liệu
        var rewards = student.Rewards.ToList();
        tasks.Add(Task.Run(() =>
        {
            for (int row = 0; row < student.Rewards.Count; row++)
            {
                sheet5.Cells[row + 2, 0].PutValue(row + 1);
                sheet5.Cells[row + 2, 1].PutValue(rewards[row].RewardContent);
                sheet5.Cells[row + 2, 2].PutValue(rewards[row].RewardName);
                sheet5.Cells[row + 2, 3].PutValue(rewards[row].RewardDate?.ToString("dd/MM/yyyy") ?? "N/A");
            }
            sheet5.AutoFitColumns();
        }));

        //sheet5
        Worksheet sheet6 = workbook.Worksheets.Add("Danh sách kỷ luật");
        tasks.Add(Task.Run(() =>
        {
            // Định dạng tiêu đề chính
            sheet6.Cells["A1"].PutValue("Danh sách kỷ luật");
            sheet6.Cells.Merge(0, 0, 1, 4);
            var titleStyle6 = sheet6.Cells["A1"].GetStyle();
            titleStyle6.IsTextWrapped = true;
            titleStyle6.HorizontalAlignment = TextAlignmentType.Center;
            titleStyle6.Font.IsBold = true;
            sheet6.Cells["A1"].SetStyle(titleStyle6);
        }));
        tasks.Add(Task.Run(() =>
        {
            sheet6.Cells[1, 0].PutValue("STT");
            sheet6.Cells[1, 1].PutValue("Nội dung kỷ luật");
            sheet6.Cells[1, 2].PutValue("Quyết định kỷ luật");
            sheet6.Cells[1, 3].PutValue("Ngày quyết định");
        }));
        //Thêm dữ liệu
        var disciplines = student.Disciplines.ToList();
        tasks.Add(Task.Run(() =>
        {
            for (int row = 0; row < disciplines.Count; row++)
            {
                sheet6.Cells[row + 2, 0].PutValue(row + 1);
                sheet6.Cells[row + 2, 1].PutValue(disciplines[row].DisciplineContent);
                sheet6.Cells[row + 2, 2].PutValue(disciplines[row].Name);
                sheet6.Cells[row + 2, 3].PutValue(disciplines[row].DisciplineDate?.ToString("dd/MM/yyyy") ?? "N/A");
            }
            sheet6.AutoFitColumns();
        }));


        await Task.WhenAll(tasks);


        // Xuất file Excel
        using var stream = new MemoryStream();
        workbook.Save(stream, SaveFormat.Xlsx);
        byte[] bytes = stream.ToArray();
        string base64String = Convert.ToBase64String(bytes);

        //string url = await _cloudinaryService.UploadExcelAsync(base64String);
        //string url ="";

        return new ApiResponse<object>(0, "Xuất Excel thành công")
        {
            Data = await _cloudinaryService.UploadExcelAsync(base64String)
        };
        //}
        //catch (Exception ex)
        //{
        //    // Xử lý lỗi và trả về thông báo
        //    Console.WriteLine($"Error exporting Excel: {ex.Message}");
        //    return new ApiResponse<object>(1, $"Lỗi khi xuất Excel: {ex.Message}");
        //}
    }

    public async Task<ApiResponse<StudentResponse>> FindStudentByUserCodeAsync(string userCode)
    {
        if (string.IsNullOrEmpty(userCode))
            return new ApiResponse<StudentResponse>(1, "UserCode không được null hoặc rỗng.");

        // Tìm học viên theo UserCode
        var student = await _studentRepository.FindStudentByUserCode(userCode);
        if (student == null)
            return new ApiResponse<StudentResponse>(1, "Học viên không tồn tại.");

        // Ánh xạ dữ liệu học viên sang StudentResponse
        var studentResponse = _mapper.Map<StudentResponse>(student);

        // Tìm bản ghi ClassStudent hiện tại (dựa trên niên khóa mới nhất hoặc niên khóa hiện tại)
        var classStudent = await _classStudentRepository.FindStudentByIdIsActive(student.Id);
        if (classStudent != null)
        {
            // Tìm thông tin lớp từ bảng Class
            var classInfo = await _classRepository.FindClassById(classStudent.ClassId ?? 0);
            if (classInfo != null)
            {
                // Lấy thông tin niên khóa từ AcademicYear
                var academicYear = classInfo.AcademicYearId.HasValue
                    ? await _academicYearRepository.FindById(classInfo.AcademicYearId.Value)
                    : null;
                if (academicYear != null)
                {
                    studentResponse.AcademicYear = new IdNamePair
                    {
                        Id = academicYear.Id,
                        Name = academicYear.StartDate.HasValue && academicYear.EndDate.HasValue
                            ? $"{academicYear.StartDate.Value.Year}-{academicYear.EndDate.Value.Year}"
                            : "Không xác định"
                    };
                }

                // Khối (Grade)
                studentResponse.Department = new IdNamePair
                {
                    Id = classInfo.DepartmentId ?? 0,
                    Name = classInfo.Department?.Name ?? "Unknown" // Tên khối là số (ví dụ: "7")
                };
                Console.WriteLine(classInfo.Name + " khối");
                // Lớp (Class)
                studentResponse.Class = new IdNamePair
                {
                    Id = classInfo.Id,
                    Name = classInfo.Name // Tên lớp (ví dụ: "7A")
                };
            }
        }

        return new ApiResponse<StudentResponse>(0, "Đã tìm thấy học viên.")
        {
            Data = studentResponse
        };
    }
    public async Task<string> GeneratedUserCode(string code)
    {
        int number = 1; // Bắt đầu từ 1 (SV001)
        int digitLength = 3; // Độ dài số ban đầu là 3 chữ số (SV001 → SV999)
        string userCode;
        int index = 1;
        do
        {
            userCode = $"{code}{number.ToString($"D{digitLength}")}"; // SV001, SV002, ..., SV999
            number++;

            // Nếu đã vượt quá số lớn nhất có thể (999, 9999, 99999...), tăng số chữ số
            if (number > Math.Pow(10, digitLength) - 1)
            {
                digitLength++; // Tăng độ dài số
                number = (int)Math.Pow(10, digitLength - 1); // Bắt đầu từ 1000, 10000, ...
            }
            var student = await _studentRepository.FindStudentByUserCode(userCode);
            if (student == null)
            {
                index = 0;
            }

        } while (index == 1);
        return userCode;
    }

    public async Task<string> GeneratedUsername(string email)
    {
        int index = 1;
        do
        {
            email = email.Split('@')[0] + DateTime.Now.ToString("HHmmss");
            var student = await _studentRepository.FindStudentByEmail(email);
            if (student == null)
            {
                index = 0;
            }
        } while (index == 1);
        return email;

    }

    public async Task<string> GenerateSecurePassword(int length)
    {
        if (length < 8) length = 8; // Đảm bảo độ dài tối thiểu

        const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
        const string allChars = upperCase + lowerCase + digits + specialChars;

        Random random = new Random();
        char[] password = new char[length];

        // Đảm bảo ít nhất 1 ký tự từ mỗi loại
        password[0] = upperCase[random.Next(upperCase.Length)];
        password[1] = lowerCase[random.Next(lowerCase.Length)];
        password[2] = digits[random.Next(digits.Length)];
        password[3] = specialChars[random.Next(specialChars.Length)];

        // Điền các ký tự còn lại ngẫu nhiên
        for (int i = 4; i < length; i++)
        {
            password[i] = allChars[random.Next(allChars.Length)];
        }

        // Xáo trộn chuỗi
        for (int i = password.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            char temp = password[i];
            password[i] = password[j];
            password[j] = temp;
        }

        return new string(password);
    }

    public async Task<ApiResponse<PaginatedResponse<object>>> GetAllStudentOfRewardOrDisciplines(bool isReward, int academicId, int departmentId, PaginationRequest request, string columnm, bool orderBy, string searchItem)
    {
        var classes = await _classRepository.GetAllClassByAcademicDepartment(academicId, departmentId);
        var classesId = classes.Select(c => c.Id).ToList();
        var classStudents = await _classStudentRepository.getAllStudentByClasses(classesId);
        var studentsId = classStudents.Select(c => (int)c.UserId).ToList();
        var students = await _studentRepository.GetAllOfRewardByIds(isReward, studentsId, request, columnm, orderBy, searchItem);
        var studentResponse = new List<object>();
        if (isReward)
        {
            studentResponse = students.Select(st => (object)new
            {
                st.UserCode,
                st.FullName,
                st.BirthDate,
                gender = (st.Gender != null && st.Gender.Length > 0) ? st.Gender[0] : false,
                st.Rewards.Count

            }).ToList();
        }
        else
        {
            studentResponse = students.Select(st => (object)new
            {
                st.UserCode,
                st.FullName,
                st.BirthDate,
                gender = (st.Gender != null && st.Gender.Length > 0) ? st.Gender[0] : false,
                st.Disciplines.Count
            }).ToList();
        }
        var totalItems = await _studentRepository.CountStudentOfRewardByIds(isReward, studentsId, searchItem);
        var paginatedResponse = new PaginatedResponse<object>
        {
            Items = studentResponse,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize),
            HasPreviousPage = request.PageNumber > 1,
            HasNextPage = request.PageNumber < (int)Math.Ceiling(totalItems / (double)request.PageSize)
        };
        return new ApiResponse<PaginatedResponse<object>>(0, "Lấy danh sách học viên thành công.") { Data = paginatedResponse };
    }

    public async Task<ApiResponse<object>> LearningOutcomesOfStudent(int studentId, int classId)
    {
        if (studentId == null || classId == null) return new ApiResponse<object>(1, "ClassId và StudentId không được bỏ trống.");
        var student = await _studentRepository.FindStudentById(studentId);
        if (student?.Assignments == null) return new ApiResponse<object>(1, "Học viên không tồn tại.");
        var cls = await _classRepository.FindClassById(classId);
        if (cls == null) return new ApiResponse<object>(1, "Lớp học không tồn tại.");

        var cs = await _classStudentRepository.FindStudentByClassAndStudent(classId, studentId);
        if (cs == null) return new ApiResponse<object>(1, "Học viên không tồn tại trong lớp học.");
        var informaionOfStudent = (object)new
        {
            academic = cs.Class?.AcademicYear?.StartDate?.ToString("yyyy") + " - " + cs.Class?.AcademicYear?.EndDate?.ToString("yyyy"),
            department = cs.Class?.Department?.Name,
            ClassCode = cs.Class?.ClassCode,
            ClassName = cs.Class?.Name,
            homeroomteacher = cs.Class?.User?.FullName,
            countstudent = cs.Class?.ClassStudents.Count,
            classtype = cs.Class?.ClassType?.Name,
            countsubject = cs.Class?.ClassSubjects.Count,
            description = cs.Class?.Description

        };

        var assignments = student.Assignments.Where(a => a.TestExam.ClassId == classId).ToList();
        double CalculateSemesterScore(string semesterName)
        {
            int count = 0;
            double score = assignments
                .Where(a => a.TestExam.Semesters.Name.ToLower().Contains(semesterName))
                .Sum(a =>
                {
                    if (a.TotalScore > 0 && a.TotalScore != null)
                    {
                        count += a.TestExam?.TestExamType?.Coefficient ?? 1;
                        return (a.TotalScore * a.TestExam?.TestExamType?.Coefficient) ?? 0;
                    }
                    else
                    {
                        return 0;
                    }
                });
            return (double)count > 0 ? score / count : 0;
        }

        (string academicPerformance, string conduct, double score) EvaluatePerformance(double score) =>
            score == 0
                ? ("Không có dữ liệu", "không có dữ liệu", 0)
                : (score switch
                {
                    >= 8.0 => "Giỏi",
                    >= 6.5 => "Khá",
                    >= 5.0 => "Trung bình",
                    _ => "Yếu"
                },
                    score > 5 ? "Tốt" : "Khá",
                    score);

        var score1 = CalculateSemesterScore("học kỳ 1");
        var score2 = CalculateSemesterScore("học kỳ 2");
        var (perf1, cond1, avg1) = EvaluatePerformance(score1);
        var (perf2, cond2, avg2) = EvaluatePerformance(score2);

        // Cả năm
        var (perfYear, condYear, avgYear) = score1 == 0 || score2 == 0
            ? ("Không có dữ liệu", "không có dữ liệu", 0.0)
            : EvaluatePerformance((score2 * 2 + score1) / 3);



        Func<string, List<Dictionary<string, List<Dictionary<string, object>>>>> SemesterSubjectScore = (string semester) =>
        {
            var assignmentBySubject = student.Assignments
                .Where(st => st.TestExam.Semesters.Name.ToLower().Contains(semester))
                .GroupBy(asm => asm.TestExam.SubjectId)
                .ToList();

            var subjectScores = new List<Dictionary<string, List<Dictionary<string, object>>>>();

            foreach (var item in assignmentBySubject)
            {
                var assignmentOfSubject = student.Assignments
                    .Where(asm => asm.TestExam.SubjectId == item.Key)
                    .ToList();

                var typeScores = new List<Dictionary<string, object>>();
                double totalScore = 0;
                int totalCoefficient = 0;
                foreach (var item2 in assignmentOfSubject)
                {
                    if (item2.TestExam?.TestExamType?.PointTypeName != null && item2.TotalScore.HasValue)
                    {
                        var propertyName = item2.TestExam.TestExamType.PointTypeName;
                        totalCoefficient += (item2.TestExam.TestExamType.Coefficient ?? 1);
                        totalScore += (item2.TotalScore * (item2.TestExam.TestExamType.Coefficient ?? 1)) ?? 0;
                        typeScores.Add(new Dictionary<string, object>
                            {
                                { propertyName, item2.TotalScore }

                            });
                    }
                }
                typeScores.Add(new Dictionary<string, object>
                    {
                        {"averageScore",(double) totalScore/totalCoefficient }
                    });

                subjectScores.Add(new Dictionary<string, List<Dictionary<string, object>>>()
                    {
                        {item.First().TestExam.Subject.SubjectName, typeScores }
                });
            }

            return subjectScores;
        };

        var semesterData1 = SemesterSubjectScore("học kỳ 1");
        var semesterData2 = SemesterSubjectScore("học kỳ 2");
        //Khen thuong va ky luat 
        var disciplines = student.Disciplines.Select(dcl => (object)new
        {
            dcl.DisciplineContent,
            dcl.Name,
            dcl.DisciplineDate
        }).ToList();
        var rewards = student.Rewards.Select(r => new
        {
            r.RewardContent,
            r.RewardName,
            r.RewardDate
        }).ToList();

        var studentResponse = new
        {
            information = informaionOfStudent,
            semestertranscript = new
            {
                semsester1 = new { academicPerformance = perf1, conduct = cond1, averagescore = avg1 },
                semsester2 = new { academicPerformance = perf2, conduct = cond2, averagescore = avg2 },
                semsester = new { academicPerformance = perfYear, conduct = condYear, averagescore = avgYear }
            },
            subjecttranscript = new
            {
                semester1 = semesterData1.Count > 0 ? (object)semesterData1 : "không có dữ liệu",
                semester2 = semesterData2.Count > 0 ? (object)semesterData2 : "không có dữ liệu"

            },
            rewards,
            disciplines
        };

        return new ApiResponse<object>(0, "Lấy bảng điểm thành công.") { Data = studentResponse };
    }
    //Thêm thư viện Install-Package ExcelDataReader đọc file excel
    public async Task<ApiResponse<object>> ReadStudentsFromExcelAsync(IFormFile fileExcel)
    {
        if (fileExcel == null || fileExcel.Length == 0)
        {
            return new ApiResponse<object>(1, "Vui lòng chọn file Excel để upload!");
        }

        // Kiểm tra đuôi file (chỉ nhận .xls hoặc .xlsx)
        var allowedExtensions = new[] { ".xls", ".xlsx" };
        var fileExtension = Path.GetExtension(fileExcel.FileName).ToLower();
        if (!allowedExtensions.Contains(fileExtension))
        {
            return new ApiResponse<object>(1, "File không hợp lệ! Vui lòng chọn file có định dạng .xls hoặc .xlsx.");
        }




        var studentRequest = new List<StudentRequest>();
        //giúp chương trình có thể đọc được file Excel có sử dụng các bảng mã ký tự cũ không mặc định trong .NET Core
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        try
        {
            using (var stream = fileExcel.OpenReadStream())
            {
                if (!ValidateExcelFormat(stream, out string errorMessage))
                {
                    return new ApiResponse<object>(1, errorMessage);
                }
                Workbook workbook = new Workbook(stream); // Mở file Excel
                Worksheet worksheet = workbook.Worksheets[0]; // Lấy sheet đầu tiên
                Cells cells = worksheet.Cells;
                int rowCount = cells.MaxRow + 1;

                //int columnCount = cells.MaxColumn;
                for (int row = 1; row < rowCount; row++)
                {
                    StudentRequest student = new StudentRequest
                    {
                        StudentStatusId = Convert.ToInt32(cells[row, 0].Value),
                        ClassId = Convert.ToInt32(cells[row, 1].Value),
                        UserCode = cells[row, 2].StringValue,
                        FullName = cells[row, 3].StringValue,
                        Email = cells[row, 4].StringValue,
                        StartDate = Convert.ToDateTime(cells[row, 5].StringValue),
                        Image = cells[row, 6].StringValue,
                        Gender = Convert.ToBoolean(cells[row, 7].Value),
                        Ethnicity = cells[row, 8].StringValue,
                        Religion = cells[row, 9].StringValue,
                        PlaceOfBirth = cells[row, 10].StringValue,
                        BirthDate = Convert.ToDateTime(cells[row, 11].StringValue),
                        StudyMode = cells[row, 12].StringValue,
                        Phone = cells[row, 13].StringValue,
                        Address = cells[row, 14].StringValue,
                        ProvinceId = cells[row, 15].StringValue,
                        DistrictId = cells[row, 16].StringValue,
                        WardId = cells[row, 17].StringValue,
                        Alias = cells[row, 18].StringValue,
                        AdmissionType = cells[row, 19].StringValue,
                        National = cells[row, 20].StringValue,
                        FullnameFather = cells[row, 21].StringValue,
                        BirthFather = Convert.ToDateTime(cells[row, 22].StringValue),
                        WorkFather = cells[row, 23].StringValue,
                        PhoneFather = cells[row, 24].StringValue,
                        FullnameMother = cells[row, 25].StringValue,
                        BirthMother = Convert.ToDateTime(cells[row, 26].StringValue),
                        WorkMother = cells[row, 27].StringValue,
                        PhoneMother = cells[row, 28].StringValue,
                        FullnameGuardianship = cells[row, 29].StringValue,
                        BirthGuardianship = Convert.ToDateTime(cells[row, 30].StringValue),
                        WorkGuardianship = cells[row, 31].StringValue,
                        PhoneGuardianship = cells[row, 32].StringValue
                    };

                    studentRequest.Add(student);
                }

            }
            var errors = new Dictionary<string, List<string>>();
            foreach (StudentRequest request in studentRequest)
            {
                var valids = await ValidateStudentRequest(request);
                if (request.UserCode == null)
                {
                    valids.Add("Usercode không được để trống.");
                }

                if (await _studentRepository.FindStudentByUserCode(request.UserCode) != null)
                {
                    valids.Add($"UserCode đã tồn tại.");
                }
                if (valids.Count > 0)
                {
                    errors.Add($"Học viên có mã [{request.UserCode}] gặp lỗi", valids);
                    Console.WriteLine("vào rồi ");
                }
            }

            if (errors.Count > 0) return new ApiResponse<object>(1, "Thêm danh sách học viên thất bại.")
            {
                Data = errors
            };
            //foreach (StudentRequest request in studentRequest)
            //{
            //    var student = _mapper.Map<User>(request);
            //    student.Username = await GeneratedUsername(request.Email);
            //    student.IsDelete = false;
            //    student.CreateAt = DateTime.Now;
            //    string password = await GenerateSecurePassword(8);
            //    student.Password = BCrypt.Net.BCrypt.HashPassword(password);

            //    if (request.Image != null)
            //    {
            //        string url = await _cloudinaryService.UploadImageAsync(request.Image);
            //        student.Image = url;
            //        Console.WriteLine("Url : " + request.Image);
            //    }
            //    var user = await _studentRepository.AddAsync(student);
            //    Task.Run(async () =>
            //    {
            //        await ExecuteEmail(user.Email, user.FullName, user.Username, password);
            //    });
            //    await _classStudentRepository.AddAsync(new ClassStudentRequest()
            //    {
            //        UserId = user.Id,
            //        ClassId = request.ClassId
            //    });
            //}

            //Console.WriteLine("StudenRequest "+s)
            return new ApiResponse<object>(0, "Thêm danh sách học viên thành công.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi đọc file Excel: {ex.Message}");
            return new ApiResponse<object>(1, $"Lỗi khi đọc file Excel: {ex.Message}");
        }
    }
    public async Task<ApiResponse<object>> UpdateAsync(StudentRequest request)
    {
        // Tìm học viên theo UserCode
        var studentFind = await _studentRepository.FindStudentByUserCode(request.UserCode);
        if (studentFind == null)
            return new ApiResponse<object>(1, "Học viên không tồn tại.");

        request.UserCode = studentFind.UserCode;
        string url = studentFind.Image;

        // Validate dữ liệu đầu vào
        var valids = await ValidateStudentRequest(request);
        if (valids.Count > 0)
            return new ApiResponse<object>(1, "Cập nhật học viên thất bại. Vui lòng kiểm tra lại thông tin.")
            {
                Data = valids
            };

        try
        {
            var latestClassStudent = await _classStudentRepository.FindStudentByIdIsActive(studentFind.Id);
            if (latestClassStudent != null && latestClassStudent.Class != null && latestClassStudent.Class.AcademicYear != null)
            {
                var currentAcademicYear = latestClassStudent.Class.AcademicYearId.HasValue
                    ? await _academicYearRepository.FindById(latestClassStudent.Class.AcademicYearId.Value)
                    : null;
                if (currentAcademicYear != null && currentAcademicYear.StartDate.HasValue)
                {
                    // Lấy niên khóa từ request
                    var requestedAcademicYear = await _academicYearRepository.FindById(request.SchoolYear);
                    if (requestedAcademicYear == null || !requestedAcademicYear.StartDate.HasValue)
                    {
                        return new ApiResponse<object>(1, "Niên khóa trong yêu cầu không hợp lệ.");
                    }

                    // So sánh niên khóa
                    if (requestedAcademicYear.StartDate.Value < currentAcademicYear.StartDate.Value)
                    {
                        var now = DateTime.Now; // 02/04/2025
                        if (requestedAcademicYear.StartDate.HasValue && requestedAcademicYear.EndDate.HasValue && requestedAcademicYear.StartDate.Value < requestedAcademicYear.EndDate.Value)
                        {
                            if (now >= requestedAcademicYear.StartDate.Value && now <= requestedAcademicYear.EndDate.Value)
                            {
                                // Cho phép cập nhật lùi vì thời gian hiện tại nằm trong niên khóa yêu cầu
                                _logger.LogInformation("Cho phép cập nhật lùi niên khóa từ {CurrentYear} về {RequestedYear} vì thời gian hiện tại ({Now}) nằm trong niên khóa yêu cầu.",
                                    $"{currentAcademicYear.StartDate.Value.Year}-{currentAcademicYear.EndDate?.Year}",
                                    $"{requestedAcademicYear.StartDate.Value.Year}-{requestedAcademicYear.EndDate?.Year}",
                                    now);
                            }
                            else
                            {
                                // Không cho phép cập nhật lùi vì thời gian hiện tại không nằm trong niên khóa yêu cầu
                                return new ApiResponse<object>(1, $"Không được phép cập nhật lùi niên khóa từ {currentAcademicYear.StartDate.Value.Year}-{currentAcademicYear.EndDate?.Year} về {requestedAcademicYear.StartDate.Value.Year}-{requestedAcademicYear.EndDate?.Year} vì thời gian hiện tại ({now:dd/MM/yyyy}) không nằm trong niên khóa yêu cầu.");
                            }
                        }
                        else
                        {
                            // Niên khóa yêu cầu không hợp lệ (StartDate >= EndDate)
                            return new ApiResponse<object>(1, $"Niên khóa yêu cầu không hợp lệ: Ngày bắt đầu ({requestedAcademicYear.StartDate?.ToString("dd/MM/yyyy")}) phải nhỏ hơn ngày kết thúc ({requestedAcademicYear.EndDate?.ToString("dd/MM/yyyy")}).");
                        }
                    }
                }
            }

            // Tiếp tục xử lý như Phương án 1
            var student = _mapper.Map(request, studentFind);

            if (request.Image != null)
            {
                url = await _cloudinaryService.UploadImageAsync(request.Image);
            }
            student.Image = url;

            var currentClassStudent = await _classStudentRepository.FindByUserIdAndSchoolYearAndClassId(studentFind.Id, request.SchoolYear, request.ClassId);
            if (currentClassStudent == null)
            {
                await _classStudentRepository.AddAsync(new ClassStudentRequest
                {
                    UserId = studentFind.Id,
                    ClassId = request.ClassId
                });
                _logger.LogInformation("Thêm học viên vào lớp mới cho niên khóa {SchoolYear}: UserId={UserId}, ClassId={ClassId}",
                    request.SchoolYear, studentFind.Id, request.ClassId);
            }
            else
            {
                if (request.ClassId != currentClassStudent.ClassId && request.ClassId != 0)
                {
                    currentClassStudent.ClassId = request.ClassId;
                    await _classStudentRepository.UpdateClassIdAsync(currentClassStudent.UserId ?? 0, request.ClassId);
                    _logger.LogInformation("Cập nhật lớp cho học viên trong niên khóa {SchoolYear}: UserId={UserId}, ClassId={ClassId}",
                        request.SchoolYear, studentFind.Id, request.ClassId);
                }
            }

            student.UpdateAt = DateTime.Now;
            var user = await _studentRepository.UpdateAsync(student);
            _logger.LogInformation("Cập nhật học viên thành công.");

            if (request.Email != studentFind.Email)
            {
                student.Username = await GeneratedUsername(request.Email);
                string password = await GenerateSecurePassword(10);
                student.Password = BCrypt.Net.BCrypt.HashPassword(password);
                Task.Run(async () =>
                {
                    await ExecuteEmail(user.Email, user.FullName, user.Username, password);
                });
            }

            return new ApiResponse<object>(0, "Cập nhật học viên thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Lỗi cơ sở dữ liệu khi cập nhật học viên.");
            return new ApiResponse<object>(4, "Lỗi cơ sở dữ liệu khi cập nhật học viên. Vui lòng thử lại sau.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi không xác định khi cập nhật học viên.");
            return new ApiResponse<object>(5, "Đã xảy ra lỗi không xác định khi cập nhật học viên. Vui lòng thử lại sau.");
        }
    }

    public bool ValidateExcelFormat(Stream fileStream, out string errorMessage)
    {
        try
        {
            Workbook workbook = new Workbook(fileStream);
            Worksheet worksheet = workbook.Worksheets[0]; // Lấy sheet đầu tiên
            Cells cells = worksheet.Cells;

            // Đọc dòng tiêu đề
            List<string> fileHeaders = new List<string>();
            for (int col = 0; col < _expectedHeaders.Count; col++)
            {
                string header = cells[0, col].StringValue.Trim();
                fileHeaders.Add(header);
            }

            // So sánh với danh sách cột mong đợi
            if (!fileHeaders.SequenceEqual(_expectedHeaders))
            {
                errorMessage = "File Excel không đúng định dạng cột. Vui lòng kiểm tra lại!";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = $"Lỗi khi kiểm tra file Excel: {ex.Message}";
            return false;
        }
    }

    public async Task<List<string>> ValidateStudentRequest(StudentRequest student)
    {
        List<string> errors = new List<string>();

        if (student == null)
        {
            errors.Add("Dữ liệu không hợp lệ (null).");
            return errors;
        }

        Type type = student.GetType();
        PropertyInfo[] properties = type.GetProperties();

        foreach (var property in properties)
        {
            object value = property.GetValue(student);
            string propertyName = property.Name;

            // Kiểm tra số nguyên không hợp lệ
            if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
            {
                int? number = value as int?;
                if (number.HasValue && number.Value <= 0)
                {
                    errors.Add($"{propertyName} phải lớn hơn 0.");
                }
            }
            // Kiểm tra email hợp lệ
            if (propertyName.ToLower().Contains("email") && value is string email)
            {
                if (!new EmailAddressAttribute().IsValid(email))
                {
                    errors.Add($"{propertyName} không hợp lệ.");
                }
                if (await _studentRepository.FindStudentByEmailOrderUserCode(email, student.UserCode) != null)
                {
                    errors.Add($"{propertyName} đã tồn tại.");
                }
            }

            // Kiểm tra số điện thoại hợp lệ (10-15 số)
            if (propertyName.ToLower().Contains("phone") && value is string phone)
            {
                string pattern = @"^(03[2-9]|05[6-9]|07[0-9]|08[1-9]|09[0-9]|02[0-9]|04[0-9])\d{7}$";
                if (!Regex.IsMatch(phone, pattern))
                {
                    errors.Add($"{propertyName} không hợp lệ.");
                }
            }

            // Kiểm tra ngày sinh không thể là ngày trong tương lai
            if (property.PropertyType == typeof(DateTime?) && value is DateTime date)
            {
                if (date > DateTime.Now)
                {
                    errors.Add($"{propertyName} không thể là ngày trong tương lai.");
                }
            }
            if (propertyName.ToLower().Contains("classid") && value is int classId)
            {
                if (await _classRepository.FindClassById(student.ClassId) == null)
                {
                    errors.Add($"{propertyName} không tồn tại.");
                }
            }

            if (propertyName.ToLower().Contains("studentstatusid") && value is int studentStatusId)
            {
                if (await _studentStatusRepository.FindAsync(student.StudentStatusId ?? 0) == null)
                {
                    errors.Add($"{propertyName} không tồn tại.");
                }
            }

            if (propertyName.ToLower().Contains("username") && value is string username)
            {
                if (await _studentRepository.FindStudentByUsername(username) != null)
                {
                    errors.Add($"{propertyName} đã tồn tại.");
                }
            }

        }

        return errors;
    }
    public async Task<ApiResponse<object>> ExportSampleData()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (var package = new ExcelPackage())
        {
            // Tạo một worksheet mới
            var worksheet = package.Workbook.Worksheets.Add("Dữ liệu mẫu");

            // Danh sách cột (tiêu đề)
            string[] headers = { "studentStatusId", "classId", "userCode", "fullName", "email", "startDate", "image", "gender", "ethnicity", "religion", "placeOfBirth", "birthDate", "studyMode", "phone", "address", "provinceId", "districtId", "wardId", "alias", "admissionType", "national", "fullnameFather", "birthFather", "workFather", "phoneFather", "fullnameMother", "birthMother", "workMother", "phoneMother", "fullnameGuardianship", "birthGuardianship", "workGuardianship", "phoneGuardianship", "userCreate", "userUpdate" };

            // Ghi tiêu đề vào hàng 1 (bắt đầu từ A1)
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i]; // A1, B1, C1...
            }

            // Dữ liệu mẫu cho một sinh viên
            string[] students = { "1", "1", "SV001", "Dương Văn Kha", "kha1876543@gmail.com", "2025-03-20T06:19:01.279Z", "", "TRUE",
                              "Kinh", "Không", "Hà Nội", "2003-05-15T00:00:00.000Z", "Chính quy", "0987654321", "123 Đường ABC, Hà Nội",
                              "01", "001", "0001", "NgVanA", "Kỳ thi THPT Quốc gia", "Việt Nam",
                              "Nguyễn Văn B", "1975-07-10T00:00:00.000Z", "Kỹ sư", "0987123456",
                              "Trần Thị C", "1978-09-20T00:00:00.000Z", "Giáo viên", "0987234567",
                              "Nguyễn Văn D", "1980-01-01T00:00:00.000Z", "Luật sư", "0987345678",
                              "1001", "1002"};

            // Ghi dữ liệu vào hàng 2 (bắt đầu từ A2)
            for (int i = 0; i < students.Length; i++)
            {
                worksheet.Cells[2, i + 1].Value = students[i]; // A2, B2, C2...
            }

            // Tự động điều chỉnh kích thước cột
            worksheet.Cells.AutoFitColumns();

            // Xuất file Excel
            var fileBytes = package.GetAsByteArray();
            string base64Excel = Convert.ToBase64String(fileBytes);

            return new ApiResponse<object>(0, "Xuất dữ liệu mẫu thành công")
            {
                Data = await _cloudinaryService.UploadExcelAsync(base64Excel)
            };
        }
    }

}
