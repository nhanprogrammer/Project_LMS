using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Project_LMS.Data;
using Project_LMS.DTOs;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_LMS.Services
{
    public class ClassService : IClassService
    {
        private readonly ApplicationDbContext _context;

        public ClassService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<PaginatedResponse<ClassListResponse>>> GetClassList(int AcademicYearId, int DepartmentId, int PageNumber = 1, int PageSize = 10)
        {
            if (PageNumber < 1) PageNumber = 1;
            if (PageSize < 1) PageSize = 10;

            var query = _context.Classes
                .Include(c => c.User) // Đảm bảo User được nạp vào
                .Where(c => c.AcademicYearId == AcademicYearId &&
                            c.DepartmentId == DepartmentId &&
                            c.IsDelete == false);

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / PageSize);

            var classes = await query
                .OrderBy(c => c.Id)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var response = new PaginatedResponse<ClassListResponse>
            {
                Items = classes.Select(c => new ClassListResponse
                {
                    Id = c.Id,
                    ClassCode = c.ClassCode,
                    ClassName = c.Name,
                    HomeroomTeacher = c.User != null ? c.User.FullName : "Chưa có giáo viên"
                }).ToList(),
                PageNumber = PageNumber,
                PageSize = PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = PageNumber > 1,
                HasNextPage = PageNumber < totalPages
            };

            return new ApiResponse<PaginatedResponse<ClassListResponse>>(0, "Lấy danh sách lớp học thành công", response);
        }



        public async Task SaveClass(ClassSaveRequest classSaveRequest)
        {
            if (classSaveRequest == null)
            {
                throw new ArgumentNullException(nameof(classSaveRequest), "Dữ liệu không hợp lệ.");
            }

            var classEntity = await _context.Classes
                .FirstOrDefaultAsync(c => c.Id == classSaveRequest.Id);

            if (classEntity == null) // Trường hợp thêm mới
            {
                classEntity = new Class
                {
                    AcademicYearId = classSaveRequest.AcademicYearId,
                    DepartmentId = classSaveRequest.DepartmentId,
                    ClassTypeId = classSaveRequest.ClassTypeId,
                    UserId = classSaveRequest.UserId,
                    ClassCode = StringHelper.NormalizeClassCode(classSaveRequest.ClassName)
                                + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    Name = classSaveRequest.ClassName,
                    StudentCount = classSaveRequest.StudentCount,
                    Description = classSaveRequest.Description,
                    ClassLink = "NaN",
                    PasswordClass = "123456",
                    IsDelete = false
                };

                _context.Classes.Add(classEntity);
            }
            else // Trường hợp cập nhật
            {
                classEntity.AcademicYearId = classSaveRequest.AcademicYearId;
                classEntity.DepartmentId = classSaveRequest.DepartmentId;
                classEntity.ClassTypeId = classSaveRequest.ClassTypeId;
                classEntity.UserId = classSaveRequest.UserId;
                classEntity.Name = classSaveRequest.ClassName;
                classEntity.StudentCount = classSaveRequest.StudentCount;
                classEntity.Description = classSaveRequest.Description;
                classEntity.IsDelete = false;

                _context.Classes.Update(classEntity);
            }

            await _context.SaveChangesAsync();
        }


        // Lấy danh sách môn học, nhưng loại trừ các môn có ID trong danh sách đã chọn
        public async Task<ApiResponse<List<SubjectListResponse>>> GetSubjectsExcluding(List<int> excludedSubjectIds)
        {
            excludedSubjectIds ??= new List<int>(); // Đảm bảo danh sách không null

            var response = await _context.Subjects
                .Where(s => s.IsDelete != true && !excludedSubjectIds.Contains(s.Id)) // Sửa lỗi nullable bool
                .Select(s => new SubjectListResponse
                {
                    Id = s.Id,
                    SubjectName = s.SubjectName
                })
                .ToListAsync();

            if (!response.Any())
            {
                return new ApiResponse<List<SubjectListResponse>>(1, "Không tìm thấy môn học phù hợp", new List<SubjectListResponse>());
            }

            return new ApiResponse<List<SubjectListResponse>>(0, "Lấy danh sách môn học thành công", response);
        }



        // Lấy danh sách môn học mà khối này đã sử dụng từ khóa trước
        public async Task<ApiResponse<List<SubjectListResponse>>> GetInheritedSubjects(int academicYearId, int departmentId)
        {
            // Lấy lớp đầu tiên theo năm học và khoa
            var firstClass = await _context.Classes
                .Where(c => c.IsDelete == false && c.AcademicYearId == academicYearId && c.DepartmentId == departmentId)
                .FirstOrDefaultAsync();

            if (firstClass == null)
            {
                return new ApiResponse<List<SubjectListResponse>>(1, "Không tìm thấy lớp học", new List<SubjectListResponse>());
            }

            // Lấy danh sách môn học từ TeachingAssignments của lớp đầu tiên
            var subjects = await _context.TeachingAssignments
                .Where(ta => ta.IsDelete == false && ta.ClassId == firstClass.Id && ta.Subject != null) // Kiểm tra null
                .Select(ta => ta.Subject)
                .Where(s => s.IsDelete != true) // Sửa lỗi nullable bool
                .Distinct() // Chỉ lọc sau khi đảm bảo không có null
                .ToListAsync();

            var response = subjects.Select(s => new SubjectListResponse
            {
                Id = s.Id,
                SubjectName = s.SubjectName
            }).ToList();

            return new ApiResponse<List<SubjectListResponse>>(0, "Lấy thông tin chi tiết lớp học thành công", response);
        }


        public async Task<bool> DeleteClass(int classId)
        {
            var classEntity = await _context.Classes.FindAsync(classId);
            if (classEntity != null)
            {
                classEntity.IsDelete = true;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<ApiResponse<ClassDetailResponse>> GetClassDetail(int classId)
        {
            try
            {
                var classEntity = await _context.Classes
                    .AsNoTracking()
                    .Include(c => c.AcademicYear)
                    .Include(c => c.Department)
                    .Include(c => c.ClassType)
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == classId);

                if (classEntity == null)
                {
                    return new ApiResponse<ClassDetailResponse>(1, "Không tìm thấy lớp học");
                }

                var classStudentEntity = await _context.ClassStudents
                    .AsNoTracking()
                    .Where(cs => cs.ClassId == classId)
                    .Include(cs => cs.User)
                    .ThenInclude(u => u.StudentStatus)
                    .ToListAsync();

                // ✅ Bọc try-catch cho truy vấn subjectClass
                List<Subject> subjectClass = new();
                try
                {
                    subjectClass = await _context.Subjects
                        .AsNoTracking()
                        .Where(s => s.ClassSubjects != null && s.ClassSubjects.Any(cs => cs.ClassId == classId))
                        .ToListAsync();

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARNING] Không thể tải danh sách môn học: {ex.Message}");
                }

                var response = new ClassDetailResponse
                {
                    Id = classEntity.Id,
                    AcademicYearName = classEntity.AcademicYear != null
                        ? $"{classEntity.AcademicYear.StartDate:yyyy} - {classEntity.AcademicYear.EndDate:yyyy}"
                        : "N/A",
                    DepartmentName = classEntity.Department?.Name ?? "N/A",
                    ClassCode = classEntity.ClassCode,
                    ClassName = classEntity.Name,
                    HomeroomTeacher = classEntity.User?.FullName ?? "Chưa có",
                    StudentCount = classEntity.StudentCount > 0 ? classEntity.StudentCount.ToString() : "0",
                    ClassType = classEntity.ClassType?.Name ?? "N/A",
                    SubjectCount = subjectClass.Any() ? subjectClass.Count.ToString() : "0",
                    Description = classEntity.Description,

                    ClassDetailStudentResponse = classStudentEntity.Select(s => new ClassDetailStudentResponse
                    {
                        Id = s.User?.Id ?? 0,
                        StudentCode = s.User?.UserCode ?? "N/A",
                        StudentName = s.User?.FullName ?? "N/A",
                        AcademicYear = classEntity.AcademicYear != null
                            ? $"{classEntity.AcademicYear.StartDate:yyyy} - {classEntity.AcademicYear.EndDate:yyyy}"
                            : "N/A",
                        AdmissionDate = s.User?.StartDate.HasValue == true
                            ? s.User.StartDate.Value.ToString("dd/MM/yyyy")
                            : "N/A",
                        StudentStatus = s.User?.StudentStatus?.StatusName ?? "N/A",
                        StudentStatusId = s.User?.StudentStatusId ?? 0
                    }).ToList(),

                    // ✅ Kiểm tra danh sách môn học
                    ClassDetailSubjectResponse = subjectClass.Any()
                        ? subjectClass.Select(s => new ClassDetailSubjectResponse
                        {
                            SubjectCode = s.SubjectCode,
                            SubjectName = s.SubjectName,
                            SubjectType = s.SubjectType?.Name ?? "N/A",
                            Semester1LessonCount = s.Semester1PeriodCount?.ToString() ?? "0",
                            Semester2LessonCount = s.Semester2PeriodCount?.ToString() ?? "0"
                        }).ToList()
                        : new List<ClassDetailSubjectResponse>()
                };

                return new ApiResponse<ClassDetailResponse>(0, "Lấy thông tin chi tiết lớp học thành công", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return new ApiResponse<ClassDetailResponse>(-1, "Lỗi hệ thống, vui lòng thử lại sau");
            }
        }





        public async Task<bool> SaveStudentStatus(int studentId, int statusId)
        {
            if (studentId <= 0 || statusId <= 0)
            {
                return false;
            }

            var student = await _context.Users.FindAsync(studentId);
            if (student == null)
            {
                return false;
            }

            student.StudentStatusId = statusId;
            _context.Users.Update(student);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<FileContentResult> ExportClassListToExcel(int academicYearId, int departmentId)
        {
            var query = _context.Classes
                .Where(c => c.AcademicYearId == academicYearId &&
                            c.DepartmentId == departmentId &&
                            c.IsDelete == false)
                .Include(c => c.User); // Đảm bảo load thông tin giáo viên chủ nhiệm

            var classes = await query
                .OrderBy(c => c.Id)
                .ToListAsync();

            // Tạo danh sách dữ liệu để export
            var classList = classes.Select(c => new ClassListResponse
            {
                Id = c.Id,
                ClassCode = c.ClassCode,
                ClassName = c.Name,
                HomeroomTeacher = c.User != null ? c.User.FullName : "Chưa có giáo viên" // Kiểm tra null
            }).ToList();

            using (var package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Danh sách lớp học");

                // Header
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Mã lớp";
                worksheet.Cells[1, 3].Value = "Tên lớp";
                worksheet.Cells[1, 4].Value = "Giáo viên chủ nhiệm";

                using (var range = worksheet.Cells[1, 1, 1, 4])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Dữ liệu
                int row = 2;
                foreach (var item in classList)
                {
                    worksheet.Cells[row, 1].Value = item.Id;
                    worksheet.Cells[row, 2].Value = item.ClassCode;
                    worksheet.Cells[row, 3].Value = item.ClassName;
                    worksheet.Cells[row, 4].Value = item.HomeroomTeacher;
                    row++;
                }

                worksheet.Cells.AutoFitColumns();

                // Xuất file
                var excelData = package.GetAsByteArray();
                return new FileContentResult(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"ClassList_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
                };
            }
        }


        public async Task CreateClassByFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File không hợp lệ.");
            }

            // Kiểm tra định dạng file (chỉ chấp nhận .xlsx)
            var fileExtension = Path.GetExtension(file.FileName);
            if (!string.Equals(fileExtension, ".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Chỉ hỗ trợ file Excel (.xlsx).");
            }

            var classList = new List<ClassSaveRequest>();
            var errorMessages = new List<string>();

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Lấy sheet đầu tiên
                        if (worksheet == null)
                        {
                            throw new Exception("File Excel không chứa dữ liệu.");
                        }

                        int rowCount = worksheet.Dimension?.Rows ?? 0;
                        if (rowCount < 2)
                        {
                            throw new Exception("File Excel không có dữ liệu hợp lệ.");
                        }

                        for (int row = 2; row <= rowCount; row++) // Bỏ qua dòng tiêu đề
                        {
                            try
                            {
                                var academicYearId = Convert.ToInt32(worksheet.Cells[row, 1].Value);
                                var departmentId = Convert.ToInt32(worksheet.Cells[row, 2].Value);
                                var classTypeId = Convert.ToInt32(worksheet.Cells[row, 3].Value);
                                var userId = Convert.ToInt32(worksheet.Cells[row, 4].Value);
                                var className = worksheet.Cells[row, 5].Text.Trim();
                                var studentCount = Convert.ToInt32(worksheet.Cells[row, 6].Value);
                                var description = worksheet.Cells[row, 7].Text.Trim();

                                if (string.IsNullOrEmpty(className))
                                {
                                    errorMessages.Add($"Lỗi tại dòng {row}: ClassName không được để trống.");
                                    continue;
                                }

                                var classSaveRequest = new ClassSaveRequest
                                {
                                    AcademicYearId = academicYearId,
                                    DepartmentId = departmentId,
                                    ClassTypeId = classTypeId,
                                    UserId = userId,
                                    ClassName = className,
                                    StudentCount = studentCount,
                                    Description = description
                                };

                                classList.Add(classSaveRequest);
                            }
                            catch (Exception ex)
                            {
                                errorMessages.Add($"Lỗi tại dòng {row}: {ex.Message}");
                            }
                        }
                    }
                }

                // Nếu có lỗi, ném ngoại lệ
                if (errorMessages.Count > 0)
                {
                    throw new Exception("Đã xảy ra lỗi khi đọc file Excel:\n" + string.Join("\n", errorMessages));
                }

                // Lưu danh sách lớp học
                foreach (var classItem in classList)
                {
                    await SaveClass(classItem);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xử lý file Excel: {ex.Message}");
            }
        }
    }
}