using OfficeOpenXml;
using Org.BouncyCastle.Asn1.Ocsp;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Services
{
    public class ClassStudentService : IClassStudentService
    {
        private readonly IClassRepository _classRepository;
        private readonly IClassStudentRepository _classStudentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<ClassStudentService> _logger;
        private readonly INotificationsService _notificationsService;

        public ClassStudentService(IClassRepository classRepository, IClassStudentRepository classStudentRepository,
            IStudentRepository studentRepository, ICloudinaryService cloudinaryService,
            ILogger<ClassStudentService> logger, INotificationsService notificationsService)
        {
            _logger = logger;
            _notificationsService = notificationsService;
            _classRepository = classRepository;
            _classStudentRepository = classStudentRepository;
            _studentRepository = studentRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ApiResponse<object>> ExportAllStudentExcel(int academicId, int departmentId, string column,
            bool orderBy, string searchItem)
        {
            var classes = await _classRepository.GetAllClassByAcademicDepartment(academicId, departmentId);
            if (classes == null || !classes.Any())
            {
                return new ApiResponse<object>(1, "No classes found.");
            }

            var classesId = classes.Select(c => c.Id).ToList();
            var classStudents =
                await _classStudentRepository.GetAllByClasses(classesId, null, column, orderBy, searchItem);
            if (classStudents == null || !classStudents.Any())
            {
                return new ApiResponse<object>(3, "No class students found.");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Danh sách học vien");
                // Gộp 5 cột đầu tiên ở hàng 1 mà không dùng Range
                worksheet.Cells["A1:E1"].Merge = true; // Gộp 5 cột đầu tiên ở hàng 1
                worksheet.Cells["A1"].Value = "Danh sách học viên"; // Đặt tiêu đề bảng

                worksheet.Cells[2, 1].Value = "Mã học viên";
                worksheet.Cells[2, 2].Value = "Tên học viên";
                worksheet.Cells[2, 3].Value = "Ngày sinh";
                worksheet.Cells[2, 4].Value = "Giới tính";
                worksheet.Cells[2, 5].Value = "Dân tộc";
                worksheet.Cells[2, 6].Value = "Lớp";
                worksheet.Cells[2, 7].Value = "Tình trạng";

                int row = 3;
                foreach (var cs in classStudents)
                {
                    worksheet.Cells[row, 1].Value = cs.User?.UserCode?.ToString();
                    worksheet.Cells[row, 2].Value = cs.User?.FullName?.ToString();
                    worksheet.Cells[row, 3].Value = cs.User?.BirthDate?.ToString("dd-MM-yyyy");
                    worksheet.Cells[row, 4].Value =
                        (cs.User?.Gender != null && cs.User.Gender.Length > 0 && cs.User.Gender[0]) ? "Nam" : "Nữ";
                    worksheet.Cells[row, 5].Value = cs.User?.Ethnicity?.ToString();
                    worksheet.Cells[row, 6].Value = cs.Class?.Name?.ToString();
                    worksheet.Cells[row, 7].Value = cs.User?.StudentStatus?.StatusName?.ToString();
                }

                worksheet.Cells.AutoFitColumns();
                var filebytes = package.GetAsByteArray();
                string base64Excel = Convert.ToBase64String(filebytes);
                return new ApiResponse<object>(0, "Xuất excel thành công.")
                {
                    Data = await _cloudinaryService.UploadExcelAsync(base64Excel)
                };
            }
        }


        public async Task<ApiResponse<PaginatedResponse<object>>> GetAllByAcademicAndDepartment(int academicId,
            int departmentId, PaginationRequest request, string column, bool orderBy, string searchItem)
        {
            // Kiểm tra đầu vào
            if (request == null || request.PageSize <= 0 || request.PageNumber <= 0)
            {
                return new ApiResponse<PaginatedResponse<object>>(1, "Invalid pagination request.");
            }

            // Lấy danh sách Class theo academicId và departmentId
            var classes = await _classRepository.GetAllClassByAcademicDepartment(academicId, departmentId);
            if (classes == null || !classes.Any())
            {
                return new ApiResponse<PaginatedResponse<object>>(1, "No classes found.")
                {
                    Data = new PaginatedResponse<object> { Items = new List<object>() }
                };
            }

            var classesId = classes.Select(c => c.Id).ToList();

            // Lấy ClassStudents với phân trang (giả định phương thức đã được sửa)
            var classStudents =
                await _classStudentRepository.GetAllByClasses(classesId, request, column, orderBy, searchItem);
            if (classStudents == null || !classStudents.Any())
            {
                return new ApiResponse<PaginatedResponse<object>>(1, "No class students found.")
                {
                    Data = new PaginatedResponse<object> { Items = new List<object>() }
                };
            }

            var classStudentsResponse = classStudents.Select(cs => (object)new
            {
                cs.UserId,
                cs.User?.UserCode,
                cs.User?.FullName,
                cs.User?.BirthDate,
                gender = (cs.User?.Gender != null && cs.User.Gender.Length > 0)
                    ? cs.User.Gender[0]
                    : false, // Ép kiểu từ BitArray sang bool
                cs.User.Ethnicity,
                status = cs.User.StudentStatus?.StatusName ?? "Unknown",
                classstudent = cs.Class?.Name
            }).ToList();
            var totalItems = await _classStudentRepository.CountByClasses(classesId, searchItem);

            // Tạo PaginatedResponse
            var paginatedResponse = new PaginatedResponse<object>
            {
                Items = classStudentsResponse,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize),
                HasPreviousPage = request.PageNumber > 1,
                HasNextPage = request.PageNumber < (int)Math.Ceiling(totalItems / (double)request.PageSize)
            };
            return new ApiResponse<PaginatedResponse<object>>(0, "GetAll User success.") { Data = paginatedResponse };
        }

        public async Task<ApiResponse<object>> ChangeClassOfStudent(ClassStudentRequest request, int userId)
        {
            try
            {
                // Kiểm tra lớp học và học viên tồn tại
                var newClass = await _classRepository.FindClassById(request.ClassId);
                if (newClass == null)
                    return new ApiResponse<object>(1, "Lớp học không tồn tại.");

                var student = await _studentRepository.FindStudentById(request.UserId);
                if (student == null)
                    return new ApiResponse<object>(1, "Học viên không tồn tại.");

                // Tìm thông tin lớp học hiện tại của học viên
                var currentClassStudent = await _classStudentRepository.FindStudentByIdIsActive(request.UserId);

                if (currentClassStudent != null)
                {
                    // Kiểm tra nếu học viên đang ở cùng lớp
                    if (currentClassStudent.ClassId == request.ClassId)
                        return new ApiResponse<object>(1, "Học viên đã ở trong lớp này.");

                    // Lấy thông tin lớp hiện tại
                    var currentClass = await _classRepository.FindClassById(currentClassStudent.ClassId ?? 0);

                    // Kiểm tra khối của lớp cũ và lớp mới
                    if (currentClass?.DepartmentId != newClass.DepartmentId)
                        return new ApiResponse<object>(1, "Chỉ được phép chuyển lớp trong cùng một khối.");

                    // Đánh dấu bản ghi cũ là không hoạt động
                    currentClassStudent.IsActive = false;
                    currentClassStudent.IsDelete = false;
                    currentClassStudent.UserUpdate = userId;
                    await _classStudentRepository.UpdateAsync(currentClassStudent);
                    _logger.LogInformation(
                        $"Vô hiệu hóa bản ghi lớp cũ: Học viên {request.UserId} rời lớp {currentClassStudent.ClassId}");
                }

                // Thêm bản ghi mới
                if (!string.IsNullOrEmpty(request.FileName))
                {
                    request.FileName = await _cloudinaryService.UploadDocxAsync(request.FileName);
                }

                request.UserUpdate = userId;
                await _classStudentRepository.AddChangeClassAsync(request);
                _logger.LogInformation($"Thêm bản ghi lớp mới: Học viên {request.UserId} vào lớp {request.ClassId}");
                var oldClass = currentClassStudent != null
                    ? await _classRepository.FindClassById(currentClassStudent.ClassId ?? 0)
                    : null;

                // Tạo nội dung thông báo
                var subject = "Thông báo chuyển lớp";
                var content =
                    $"Học viên {student.FullName} đã được chuyển từ lớp {oldClass?.Name ?? "chưa có lớp"} sang lớp {newClass.Name}. Lý do: {request.Reason}";

                // Gửi thông báo cho học sinh
                await _notificationsService.SendUserListNotificationAsync(
                    userId, // người gửi (admin/giáo viên)
                    new List<int>(request.UserId), // người nhận (học sinh)
                    subject,
                    content
                );
                // Gửi thông báo cho lớp mới
                await _notificationsService.SendClassNotificationAsync(
                    userId,
                    request.ClassId,
                    subject,
                    $"Học viên mới {student.FullName} đã được chuyển vào lớp"
                );
                
                return new ApiResponse<object>(0, "Chuyển lớp thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi chuyển lớp cho học viên");
                return new ApiResponse<object>(5, "Đã xảy ra lỗi khi chuyển lớp. Vui lòng thử lại sau.");
            }
        }

        public async Task<ApiResponse<ClassStudentChangeResponse>> GetClassStudentChangeInfo(int userId, int classId)
        {
            try
            {
                var classStudent = await _classStudentRepository.GetClassStudentChangeInfo(userId, classId);

                if (classStudent == null)
                    return new ApiResponse<ClassStudentChangeResponse>(1,
                        "Không tìm thấy thông tin chuyển lớp của học viên.");

                var response = new ClassStudentChangeResponse
                {
                    UserId = classStudent.UserId ?? 0,
                    UserCode = classStudent.User?.UserCode,
                    FullName = classStudent.User?.FullName,
                    ClassId = classStudent.ClassId ?? 0,
                    ClassName = classStudent.Class?.Name,
                };

                return new ApiResponse<ClassStudentChangeResponse>(0, "Lấy thông tin chuyển lớp thành công.", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin chuyển lớp: userId={UserId}, classId={ClassId}", userId,
                    classId);
                return new ApiResponse<ClassStudentChangeResponse>(1, "Đã xảy ra lỗi khi lấy thông tin chuyển lớp.");
            }
        }
    }
}