using AutoMapper;
using FluentValidation;
using OfficeOpenXml;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services;

public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly IMapper _mapper;
    private readonly ITeacherClassSubjectRepository _teacherClassSubjectRepository;
    private readonly IValidator<TeacherRequest> _validator;
    private readonly IStudentService _studentService;
    private readonly IEmailService _emailService;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IClassRepository _classRepository;
    private readonly ITeachingAssignmentRepository _assignmentRepository;

    public TeacherService(ITeacherRepository teacherRepository, IMapper mapper, ITeacherClassSubjectRepository teacherClassSubjectRepository, IValidator<TeacherRequest> validator, IStudentService studentService, IEmailService emailService, ICloudinaryService cloudinaryService, IClassRepository classRepository, ITeachingAssignmentRepository assignmentRepository)
    {
        _teacherRepository = teacherRepository;
        _mapper = mapper;
        _teacherClassSubjectRepository = teacherClassSubjectRepository;
        _validator = validator;
        _studentService = studentService;
        _emailService = emailService;
        _cloudinaryService = cloudinaryService;
        _classRepository = classRepository;
        _assignmentRepository = assignmentRepository;
    }

    public async Task<ApiResponse<object>> AddAsync(TeacherRequest request)
    {

        if (await _teacherRepository.FindTeacherByUserCode(request.UserCode) != null) 
            return new ApiResponse<object>(1, "UserCode đã tồn tại"); 
        if (await _teacherRepository.FindTeacherByEmailOrderUserCode(request.Email,null) != null) 
            return new ApiResponse<object>(1, "Email đã tồn tại");
        
        var teacher = _mapper.Map<User>(request);
        var username = await _studentService.GeneratedUsername(request.Email);
        var password = await _studentService.GenerateSecurePassword(10);
        try
        {
            if (request.Image != null)
            {
                teacher.Image = await _cloudinaryService.UploadImageAsync(request.Image);
            }
            teacher.Username = username;
            teacher.Password = BCrypt.Net.BCrypt.HashPassword(password);
            teacher = await _teacherRepository.AddAsync(teacher);
            foreach (int item in request.TeacherSubjectIds)
            {
                TeacherClassSubject teacherClass = new TeacherClassSubject();
                teacherClass.SubjectsId = item;
                teacherClass.UserId = teacher.Id;
                if (item == request.SubjectId)
                {
                    teacherClass.IsPrimary = true;
                }
                await _teacherClassSubjectRepository.AddAsync(teacherClass);

            }



            Task.Run(async () =>
            {
                await ExecuteEmail(teacher.Email, teacher.FullName, username, password);
            });

            return new ApiResponse<object>(0, "Tạo tài khoản giảng viên thành công.");
        }
        catch (Exception ex)
        {
            return new ApiResponse<object>(0, "Tạo tài khoản giảng viên thất bại.")
            {
                Data = "error : " + ex.ToString()
            };
        }
    }

    public async Task<ApiResponse<object>> DeleteAsync(List<string> userCodes)
    {
        if (userCodes.Count <= 0) return new ApiResponse<object>(1, "UserCodes phải có ít nhất 1 phần tử.");
        List<string> userCodeDeleteSuccess = new List<string>();
        List<string> userCodeDeleteError = new List<string>();
        foreach (var userCode in userCodes)
        {
            var teacherFindFind = await _teacherRepository.FindTeacherByUserCode(userCode);
            if (teacherFindFind != null && userCode != null)
            {
                teacherFindFind.IsDelete = true;
                await _teacherRepository.DeleteAsync(teacherFindFind);
                userCodeDeleteSuccess.Add(userCode);
            }
            else
            {
                userCodeDeleteError.Add(userCode);
            }
        };
        if (userCodeDeleteSuccess.Count > 0)
        {
            return new ApiResponse<object>(0, "Xóa giảng viên thành công.")
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
            return new ApiResponse<object>(1, "Xóa giảng viên thất bại.")
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

    public async Task<ApiResponse<object>> FindTeacherByUserCode(string userCode)
    {
        var teacher = await _teacherRepository.FindTeacherByUserCode(userCode);
        if (teacher == null) return new ApiResponse<object>(1, "UserCode không tồn tại trong hệ thống");
        var teacherResponse = _mapper.Map<TeacherResponse>(teacher);
        var subject = await _teacherClassSubjectRepository.FindSubjectByTeacherIsPrimary(teacher.Id);
        if (subject != null)
        {
            teacherResponse.SubjectId = subject.Id;
        }
        var subjects = await _teacherClassSubjectRepository.GetAllByTeacher(teacher.Id);

        if (subjects.Count > 0)
        {
            teacherResponse.TeacherSubjectIds = subjects.Select(s => (int?)s.Id).ToList();
        }
        return new ApiResponse<object>(0, "Đã tìm thấy giáo viên.")
        {
            Data = teacherResponse
        };
    }

    public async Task<ApiResponse<object>> UpdateAsync(TeacherRequest request)
    {
        var teacher = await _teacherRepository.FindTeacherByUserCode(request.UserCode);
        if (await _teacherRepository.FindTeacherByUserCode(request.UserCode) == null)
            return new ApiResponse<object>(1, "UserCode không tồn tại");
        if (await _teacherRepository.FindTeacherByEmailOrderUserCode(request.Email, request.UserCode) != null)
            return new ApiResponse<object>(1, "Email đã tồn tại");

        var emailOld = teacher.Email;
        var imageOld = teacher.Image;

        var username = await _studentService.GeneratedUsername(request.Email);
        var password = await _studentService.GenerateSecurePassword(10);

        try
        {
            if (request.Image != null)
            {
                imageOld = await _cloudinaryService.UploadImageAsync(request.Image);
            }
            teacher = _mapper.Map(request, teacher);
            teacher.Image = imageOld;
            if (!request.Email.Equals(emailOld))
            {
                teacher.Username = username;
                teacher.Password = BCrypt.Net.BCrypt.HashPassword(password);
            }
            teacher = await _teacherRepository.UpdateAsync(teacher);
            Task task1 = Task.Run(async () =>
            {
                var subjects = await _teacherClassSubjectRepository.GetAllByTeacher(teacher.Id);

                    foreach (var item in subjects)
                    {
                        if (!request.TeacherSubjectIds.Contains(item.Id))
                        {
                            await _teacherClassSubjectRepository.DeleteAsync(item);
                        }
                    }
         
                Task task2 = Task.Run(async () =>
                {
                    foreach (int item in request.TeacherSubjectIds)
                    {
                        if (!subjects.Select(s => s.Id).ToList().Contains(item))
                        {
                            TeacherClassSubject teacherClassSubject = new TeacherClassSubject();
                            teacherClassSubject.UserId = teacher.Id;
                            teacherClassSubject.SubjectsId = item;
                            if (request.SubjectId == item)
                            {
                                teacherClassSubject.IsPrimary = true;
                            }
                            await _teacherClassSubjectRepository.AddAsync(teacherClassSubject);
                        }
                    }
                });
            });

            Task task2 = new Task(async () =>
            {
                if (!request.Email.Equals(emailOld))
                {
                    await ExecuteEmail(teacher.Email, teacher.FullName, username, password);
                }
            });
            if (!request.Email.Equals(emailOld))
            {
                task2.Start();
            }
            //Task.WhenAll(task1);
            return new ApiResponse<object>(0, "Cập nhật tài khoản giảng viên thành công.");
        }
        catch (Exception ex)
        {
            return new ApiResponse<object>(1, "Cập nhật tài khoản giảng viên thất bại.")
            {
                Data = "error : " + ex.Message
            };
        }
    }
    public async Task ExecuteEmail(string email, string fullname, string username, string password)
    {
        var content = $@"
                            <html>
                            <head>
                                <meta charset='UTF-8'>
                                <title>Thông báo tạo tài khoản giảng viên</title>
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
                                    <div class='header'>📢 Thông Báo Tạo Tài Khoản Giảng Viên</div>
                                    <div class='content'>
                                        <p>Xin chào <b>{fullname}</b>,</p>
                                        <p>Trường <b>ABC</b> đã tạo tài khoản giảng viên cho bạn. Dưới đây là thông tin đăng nhập:</p>

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
            await _emailService.SendMailAsync(email, "Thông báo tạo tài khoản giảng viên", content);
            Console.WriteLine($"Email sent to {email}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email to {email}: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PaginatedResponse<object>>> GetAllByAcademic(int acadimicId, PaginationRequest request, bool orderBy, string column, string searchItem)
    {
        var classes = await _classRepository.GetAllClassByAcademic(acadimicId);
        var teachingAssignment = await _assignmentRepository.GetAllByClasses(classes.Select(c => c.Id).ToList());
        var teacherIds = teachingAssignment.Select(a => (int)a.UserId).ToList();
        var teachers = await _teacherRepository.GetAllByIds(teacherIds, request, orderBy, column, searchItem);
        var teacherResponse = teachers.Select(t => (object)new
        {
            t.UserCode,
            t.FullName,
            t.BirthDate,
            gender = (t?.Gender != null && t.Gender.Length > 0) ? t.Gender[0] : false,
            t?.SubjectGroup?.Name,
            t?.Ethnicity,
            roleName = t?.Role?.Name,
            t?.TeacherStatus?.StatusName
        }).ToList();
        int totalItems = await _teacherRepository.CountByClasses(teacherIds, searchItem);
        var paginatedResponse = new PaginatedResponse<object>
        {
            Items = teacherResponse,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize),
            HasPreviousPage = request.PageNumber > 1,
            HasNextPage = request.PageNumber < (int)Math.Ceiling(totalItems / (double)request.PageSize)
        };
        return new ApiResponse<PaginatedResponse<object>>(0, "Lấy danh sách giảng viên thành công")
        {
            Data = paginatedResponse
        };
    }

    public async Task<ApiResponse<object>> ExportExcelByAcademic(int acadimicId, bool orderBy, string column, string searchItem)
    {
        var classes = await _classRepository.GetAllClassByAcademic(acadimicId);
        var teachingAssignment = await _assignmentRepository.GetAllByClasses(classes.Select(c => c.Id).ToList());
        var teacherIds = teachingAssignment.Select(a => (int)a.UserId).ToList();
        var teachers = await _teacherRepository.GetAllByIds(teacherIds, orderBy, column, searchItem);
        int totalItems = await _teacherRepository.CountByClasses(teacherIds, searchItem);
        if (teachers.Count <= 0)
        {
            return new ApiResponse<object>(1, "Danh sách rỗng k thể export.");
        }
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Danh sách giảng viên");
            // Gộp 5 cột đầu tiên ở hàng 1 mà không dùng Range
            worksheet.Cells["A1:G1"].Merge = true; // Gộp 5 cột đầu tiên ở hàng 1
            worksheet.Cells["A1"].Value = "Danh sách giảng viên"; // Đặt tiêu đề bảng

            worksheet.Cells[2, 1].Value = "Mã giảng viên";
            worksheet.Cells[2, 2].Value = "Tên giảng viên";
            worksheet.Cells[2, 3].Value = "Ngày sinh";
            worksheet.Cells[2, 4].Value = "Giới tính";
            worksheet.Cells[2, 5].Value = "Dân tộc";
            worksheet.Cells[2, 6].Value = "Lớp";
            worksheet.Cells[2, 7].Value = "Tình trạng";

            int row = 3;
            foreach (var cs in teachers.ToList())
            {
                worksheet.Cells[row, 1].Value = cs.UserCode?.ToString();
                worksheet.Cells[row, 2].Value = cs?.FullName?.ToString();
                worksheet.Cells[row, 3].Value = cs?.BirthDate?.ToString("dd-MM-yyyy");
                worksheet.Cells[row, 4].Value = (cs?.Gender != null && cs.Gender.Length > 0 && cs.Gender[0]) ? "Nam" : "Nữ";
                worksheet.Cells[row, 5].Value = cs?.Ethnicity?.ToString();
                worksheet.Cells[row, 6].Value = cs?.Role?.Name?.ToString();
                worksheet.Cells[row, 7].Value = cs?.TeacherStatus?.StatusName?.ToString();
            }
            worksheet.Cells.AutoFitColumns();
            var filebytes = package.GetAsByteArray();
            string base64Excel = Convert.ToBase64String(filebytes);
            return new ApiResponse<object>(0, "Export excel success.")
            {
                Data = await _cloudinaryService.UploadExcelAsync(base64Excel)
            };
        }
    }

}