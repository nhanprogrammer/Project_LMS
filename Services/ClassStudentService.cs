
using OfficeOpenXml;
using Org.BouncyCastle.Asn1.Ocsp;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Services
{
    public class ClassStudentService : IClassStudentService
    {
        private readonly IClassRepository _classRepository;
        private readonly IClassStudentRepository _classStudentRepository;
        private readonly ICloudinaryService _cloudinaryService;
        public ClassStudentService(IClassRepository classRepository, IClassStudentRepository classStudentRepository, ICloudinaryService cloudinaryService)
        {
            _classRepository = classRepository;
            _classStudentRepository = classStudentRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ApiResponse<object>> ExportAllStudentExcel(int academicId, int departmentId, string column, bool orderBy, string searchItem)
        {
            var classes = await _classRepository.GetAllClassByAcademicDepartment(academicId, departmentId);
            if (classes == null || !classes.Any())
            {
                return new ApiResponse<object>(1, "No classes found.");
            }
            var classesId = classes.Select(c => c.Id).ToList();
            var classStudents = await _classStudentRepository.GetAllByClasses(classesId, null, column, orderBy, searchItem);
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
                    worksheet.Cells[row, 4].Value = (cs.User?.Gender != null && cs.User.Gender.Length > 0 && cs.User.Gender[0]) ? "Nam" : "Nữ";
                    worksheet.Cells[row, 5].Value = cs.User?.Ethnicity?.ToString();
                    worksheet.Cells[row, 6].Value = cs.Class?.Name?.ToString();
                    worksheet.Cells[row, 7].Value = cs.User?.StudentStatus?.StatusName?.ToString();
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

        public async Task<ApiResponse<object>> GetClassStudentByClass(int classId, int studentId)
        {
            var cs = await _classStudentRepository.FindStudentByClassAndStudent(classId, studentId);
            if (cs == null) return new ApiResponse<object>(1, "Class not found");
            var studentResponse = (object)new
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
            return new ApiResponse<object>(0, "Get Studet sucesss.")
            {
                Data = studentResponse
            };
        }

        public async Task<ApiResponse<PaginatedResponse<object>>> GetAllByAcademicAndDepartment(int academicId, int departmentId, PaginationRequest request, string column, bool orderBy, string searchItem)
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
            var classStudents = await _classStudentRepository.GetAllByClasses(classesId, request, column, orderBy, searchItem);
            if (classStudents == null || !classStudents.Any())
            {
                return new ApiResponse<PaginatedResponse<object>>(3, "No class students found.")
                {
                    Data = new PaginatedResponse<object> { Items = new List<object>() }
                };
            }
            var classStudentsResponse = classStudents.Select(cs => (object)new
            {
                cs.User?.UserCode,
                cs.User?.FullName,
                cs.User?.BirthDate,
                gender = (cs.User?.Gender != null && cs.User.Gender.Length > 0) ? cs.User.Gender[0] : false, // Ép kiểu từ BitArray sang bool
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
    }
}
