using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Project_LMS.Data;
using Project_LMS.DTOs;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
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

        private readonly ICloudinaryService _cloudinaryService;

        public ClassService(ApplicationDbContext context, ICloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ApiResponse<PaginatedResponse<ClassListResponse>>> GetClassList(ClassRequest classRequest)
        {
            if (classRequest.PageNumber < 1) classRequest.PageNumber = 1;
            if (classRequest.PageSize < 1) classRequest.PageSize = 10;

            var query = _context.Classes
                .Include(c => c.User)
                .Where(c =>
                    (classRequest.AcademicYearId == 0 || c.AcademicYearId == classRequest.AcademicYearId) &&
                    (classRequest.DepartmentId == 0 || c.DepartmentId == classRequest.DepartmentId) &&
                    (string.IsNullOrEmpty(classRequest.Key) ||
                        c.Name.Contains(classRequest.Key) ||
                        c.User.FullName.Contains(classRequest.Key)) &&
                    c.IsDelete == false
                );

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / classRequest.PageSize);

            var classes = await query
                .OrderBy(c => c.Id)
                .Skip((classRequest.PageNumber - 1) * classRequest.PageSize)
                .Take(classRequest.PageSize)
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
                PageNumber = classRequest.PageNumber,
                PageSize = classRequest.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = classRequest.PageNumber > 1,
                HasNextPage = classRequest.PageNumber < totalPages
            };

            return new ApiResponse<PaginatedResponse<ClassListResponse>>(0, "Lấy danh sách lớp học thành công", response);
        }



        public async Task SaveClass(ClassSaveRequest classSaveRequest)
        {
            if (classSaveRequest == null)
            {
                throw new ArgumentNullException(nameof(classSaveRequest), "Dữ liệu không hợp lệ.");
            }

            try
            {
                // Kiểm tra ID hợp lệ trước khi thực hiện logic
                var academicYear = await _context.AcademicYears
                    .FirstOrDefaultAsync(a => a.Id == classSaveRequest.AcademicYearId);
                if (academicYear == null)
                {
                    throw new NotFoundException("Niên khóa không tồn tại.");
                }
                var currentDate = DateTime.Now;
                if (academicYear.EndDate.HasValue && academicYear.EndDate.Value < currentDate)
                {
                    throw new InvalidOperationException("Không thể thêm hoặc cập nhật lớp học cho niên khóa đã kết thúc.");
                }

                if (!await _context.Departments.AnyAsync(d => d.Id == classSaveRequest.DepartmentId))
                {
                    throw new NotFoundException("Khoa khối không tồn tại.");
                }

                if (!await _context.ClassTypes.AnyAsync(ct => ct.Id == classSaveRequest.ClassTypeId))
                {
                    throw new NotFoundException("Loại lớp không tồn tại.");
                }

                if (!await _context.Users.AnyAsync(u => u.Id == classSaveRequest.UserId && u.TeacherStatusId.HasValue))
                {
                    throw new NotFoundException("Giáo viên chủ nhiệm không tồn tại hoặc không hợp lệ.");
                }

                // Kiểm tra StartDate và EndDate của niên khóa
                if (!academicYear.StartDate.HasValue || !academicYear.EndDate.HasValue)
                {
                    throw new InvalidOperationException("Niên khóa không có thông tin ngày bắt đầu hoặc ngày kết thúc hợp lệ.");
                }

                // Tạo ClassCode theo định dạng: ClassName-StartYear-EndYear
                var classCode = $"{classSaveRequest.ClassName}-{academicYear.StartDate.Value.Year}-{academicYear.EndDate.Value.Year}";

                // Kiểm tra nếu đã tồn tại giáo viên chủ nhiệm trong niên khóa này
                var existingTeacherClass = await _context.Classes
                    .Where(c => c.UserId == classSaveRequest.UserId
                           && c.AcademicYearId == classSaveRequest.AcademicYearId
                           && c.Id != classSaveRequest.Id  // Loại trừ lớp hiện tại nếu đang cập nhật
                           && !(c.IsDelete ?? false))
                    .FirstOrDefaultAsync();

                if (existingTeacherClass != null)
                {
                    throw new InvalidOperationException($"Giáo viên này đã là chủ nhiệm lớp '{existingTeacherClass.Name}' trong niên khóa này.");
                }

                // Kiểm tra nếu tên lớp đã tồn tại trong niên khóa này
                var existingClassWithSameName = await _context.Classes
                    .Where(c => c.Name == classSaveRequest.ClassName
                           && c.AcademicYearId == classSaveRequest.AcademicYearId
                           && c.Id != classSaveRequest.Id  // Loại trừ lớp hiện tại nếu đang cập nhật
                           && !(c.IsDelete ?? false))
                    .FirstOrDefaultAsync();

                if (existingClassWithSameName != null)
                {
                    throw new InvalidOperationException($"Lớp có tên '{classSaveRequest.ClassName}' đã tồn tại trong niên khóa này.");
                }

                // Kiểm tra ClassCode đã tồn tại chưa (ngoại trừ bản ghi hiện tại nếu đang cập nhật)
                var isDuplicateClassCode = await _context.Classes
                    .AnyAsync(c => c.ClassCode == classCode && c.Id != classSaveRequest.Id && !(c.IsDelete ?? false));
                if (isDuplicateClassCode)
                {
                    throw new InvalidOperationException($"Mã lớp '{classCode}' đã tồn tại. Vui lòng chọn tên lớp khác.");
                }

                Class classEntity;

                if (classSaveRequest.Id == 0) // Trường hợp thêm mới
                {
                    classEntity = new Class
                    {
                        AcademicYearId = classSaveRequest.AcademicYearId,
                        DepartmentId = classSaveRequest.DepartmentId,
                        ClassTypeId = classSaveRequest.ClassTypeId,
                        UserId = classSaveRequest.UserId,
                        ClassCode = classCode,
                        Name = classSaveRequest.ClassName,
                        StudentCount = classSaveRequest.StudentCount,
                        Description = classSaveRequest.Description,
                        PasswordClass = "123456",
                        IsDelete = false
                    };

                    _context.Classes.Add(classEntity);
                }
                else
                {
                    classEntity = await _context.Classes
                        .FirstOrDefaultAsync(c => c.Id == classSaveRequest.Id);

                    if (classEntity == null)
                    {
                        throw new NotFoundException("Không tìm thấy lớp học cần cập nhật.");
                    }

                    if (classEntity.IsDelete == true)
                    {
                        throw new InvalidOperationException("Không thể cập nhật lớp học đã bị xóa.");
                    }

                    // Cập nhật dữ liệu lớp học
                    classEntity.AcademicYearId = classSaveRequest.AcademicYearId;
                    classEntity.DepartmentId = classSaveRequest.DepartmentId;
                    classEntity.ClassTypeId = classSaveRequest.ClassTypeId;
                    classEntity.UserId = classSaveRequest.UserId;
                    classEntity.ClassCode = classCode;
                    classEntity.Name = classSaveRequest.ClassName;
                    classEntity.StudentCount = classSaveRequest.StudentCount;
                    classEntity.Description = classSaveRequest.Description;
                    classEntity.IsDelete = false;

                    classEntity.UpdateAt = DateTime.Now;
                    _context.Classes.Update(classEntity);
                }

                await _context.SaveChangesAsync();

                // Cập nhật môn học cho lớp
                if (classSaveRequest.Ids != null && classSaveRequest.Ids.Count > 0)
                {
                    // Phần còn lại giữ nguyên...
                    // Lấy danh sách ID môn học hợp lệ
                    var validSubjectIds = await _context.Subjects
                        .Where(s => classSaveRequest.Ids.Contains(s.Id))
                        .Select(s => s.Id)
                        .ToListAsync();

                    // Kiểm tra xem có môn học nào không hợp lệ
                    var invalidSubjects = classSaveRequest.Ids.Except(validSubjectIds).ToList();
                    if (invalidSubjects.Any())
                    {
                        throw new NotFoundException($"Các môn học không tồn tại: {string.Join(", ", invalidSubjects)}");
                    }

                    // Xóa tất cả các môn học hiện tại của lớp trước khi thêm mới
                    var existingClassSubjects = _context.ClassSubjects
                        .Where(cs => cs.ClassId == classEntity.Id);

                    _context.ClassSubjects.RemoveRange(existingClassSubjects);

                    // Thêm mới danh sách môn học
                    var newClassSubjects = classSaveRequest.Ids
                        .Select(subjectId => new ClassSubject
                        {
                            SubjectId = subjectId,
                            ClassId = classEntity.Id
                        })
                        .ToList();

                    await _context.ClassSubjects.AddRangeAsync(newClassSubjects);
                    await _context.SaveChangesAsync();
                }
            }
            catch (NotFoundException ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi lưu lớp học: {ex.Message}");
                throw new Exception("Đã xảy ra lỗi trong quá trình lưu lớp học. Vui lòng thử lại.");
            }
        }
        // Lấy danh sách môn học, nhưng loại trừ các môn có ID trong danh sách đã chọn
        public async Task<ApiResponse<List<SubjectListResponse>>> GetSubjectsExcluding(string excludedSubjectIds)
        {
            // Chuyển đổi chuỗi "1,2,3" thành List<int>
            var excludedIds = excludedSubjectIds?
                .Split(',')
                .Select(id => int.TryParse(id, out int parsedId) ? parsedId : (int?)null) // Chuyển đổi an toàn
                .Where(id => id.HasValue) // Loại bỏ null
                .Select(id => id.Value) // Lấy giá trị int
                .ToList() ?? new List<int>(); // Nếu null thì trả về danh sách rỗng

            var response = await _context.Subjects
                .Where(s => s.IsDelete != true && !excludedIds.Contains(s.Id)) // Lọc danh sách ID
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

            return new ApiResponse<List<SubjectListResponse>>(0, "Lấy thông tin môn học kế thừa thành công", response);
        }


        public async Task<bool> DeleteClass(List<int> classIds)
        {
            if (classIds == null || !classIds.Any())
            {
                throw new ArgumentException("Danh sách ID lớp học không hợp lệ.", nameof(classIds));
            }

            var classEntities = await _context.Classes
                .Where(c => classIds.Contains(c.Id))
                .ToListAsync();

            if (!classEntities.Any())
            {
                throw new NotFoundException("Không tìm thấy lớp nào để xóa.");
            }

            // Chỉ lấy các lớp chưa bị xóa để cập nhật
            var classesToDelete = classEntities.Where(c => c.IsDelete == false).ToList();

            if (!classesToDelete.Any())
            {
                return false; // Không có lớp nào hợp lệ để xóa
            }

            classesToDelete.ForEach(c =>
            {
                c.IsDelete = true;
                c.UpdateAt = DateTime.Now;
            });

            await _context.SaveChangesAsync();

            return true;
        }





        public async Task<ClassDetailResponse> GetClassDetail(int classId)
        {
            try
            {

                var classEntity = await _context.Classes
                    .AsNoTracking()
                    .Where(c => c.IsDelete == false)
                  .Include(c => c.AcademicYear)
                //  .Include(c => c.Department)
                   .Include(c => c.ClassType)
                  .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == classId);

                if (classEntity == null)
                {
                    throw new NotFoundException("Không tìm thấy lớp học");
                }
                string depar = _context.Departments
                                   .AsNoTracking()
                                   .Where(d => d.Id == classEntity.DepartmentId)
                                   .Select(d => d.Name)
                                   .FirstOrDefault() ?? "N/A";


                var classStudentEntity = await _context.ClassStudents
                    .AsNoTracking()
                    .Where(cs => cs.IsDelete == false && cs.ClassId == classId)
                    .Include(cs => cs.User).ThenInclude(u => u.StudentStatus)
                    .ToListAsync();

                var classSubjectEntity = await _context.ClassSubjects
                    .AsNoTracking()
                    .Where(cs => cs.IsDelete == false && cs.ClassId == classId)
                    .Include(cs => cs.Subject)
                        .ThenInclude(s => s.SubjectType)
                    .ToListAsync();

                string acade = classEntity.AcademicYear?.StartDate != null && classEntity.AcademicYear?.EndDate != null
     ? $"{classEntity.AcademicYear.StartDate:yyyy} - {classEntity.AcademicYear.EndDate:yyyy}"
     : "N/A";

                return new ClassDetailResponse
                {
                    Id = classEntity.Id,
                    AcademicYearName = acade,
                    DepartmentName = depar,
                    ClassCode = classEntity.ClassCode,
                    ClassName = classEntity.Name,
                    HomeroomTeacher = classEntity.User?.FullName ?? "Chưa có",
                    StudentCount = classEntity.StudentCount > 0 ? classEntity.StudentCount.ToString() : "0",
                    ClassType = classEntity.ClassType?.Name ?? "N/A",
                    SubjectCount = classSubjectEntity.Any() ? classSubjectEntity.Count.ToString() : "0",
                    Description = classEntity.Description,

                    ClassDetailStudentResponse = classStudentEntity.Select(s => new ClassDetailStudentResponse
                    {
                        Id = s.UserId ?? 0,
                        StudentCode = s.User?.UserCode ?? "N/A",
                        StudentName = s.User?.FullName ?? "N/A",
                        AcademicYear = acade,
                        AdmissionDate = s.User?.StartDate.HasValue == true
                            ? s.User.StartDate.Value.ToString("dd/MM/yyyy")
                            : "N/A",
                        StudentStatus = s.User?.StudentStatus?.StatusName ?? "N/A",
                        StudentStatusId = s.User?.StudentStatusId ?? 0
                    }).ToList(),

                    ClassDetailSubjectResponse = classSubjectEntity.Select(s => new ClassDetailSubjectResponse
                    {
                        SubjectCode = s.Subject?.SubjectCode ?? "N/A",
                        SubjectName = s.Subject?.SubjectName ?? "N/A",
                        SubjectType = s.Subject?.SubjectType?.Name ?? "N/A",
                        Semester1LessonCount = s.Subject?.Semester1PeriodCount?.ToString() ?? "0",
                        Semester2LessonCount = s.Subject?.Semester2PeriodCount?.ToString() ?? "0"
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Xảy ra lỗi: " + ex.Message);
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
            student.UpdateAt = DateTime.Now;
            _context.Users.Update(student);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<string> ExportClassListToExcel(int academicYearId, int departmentId)
        {
            var query = _context.Classes
                .Where(c => (academicYearId == 0 || c.AcademicYearId == academicYearId) &&
                            (departmentId == 0 || c.DepartmentId == departmentId) &&
                            c.IsDelete == false)
                .Include(c => c.User);

            var classes = await query.OrderBy(c => c.Id).ToListAsync();

            var classList = classes.Select(c => new ClassListResponse
            {
                Id = c.Id,
                ClassCode = c.ClassCode,
                ClassName = c.Name,
                HomeroomTeacher = c.User != null ? c.User.FullName : "Chưa có giáo viên"
            }).ToList();

            using (var package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Danh sách lớp học");

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

                var byteArray = package.GetAsByteArray();
                string excelBase64 = Convert.ToBase64String(byteArray);
                return await _cloudinaryService.UploadExcelAsync(excelBase64);
            }
        }

        public async Task CreateClassByBase64(string base64File)
        {
            if (string.IsNullOrWhiteSpace(base64File))
            {
                throw new ArgumentException("File không hợp lệ.");
            }

            byte[] fileBytes;
            try
            {
                fileBytes = Convert.FromBase64String(base64File);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Chuỗi Base64 không hợp lệ.");
            }

            if (!IsExcelFile(fileBytes))
            {
                throw new ArgumentException("Chỉ hỗ trợ file Excel (.xlsx).");
            }

            var classList = new List<ClassSaveRequest>();
            var errorMessages = new List<string>();

            try
            {
                using (var stream = new MemoryStream(fileBytes))
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    if (worksheet == null)
                    {
                        throw new Exception("File Excel không chứa dữ liệu.");
                    }

                    int rowCount = worksheet.Dimension?.Rows ?? 0;
                    if (rowCount < 2)
                    {
                        throw new Exception("File Excel không có dữ liệu hợp lệ.");
                    }

                    for (int row = 2; row <= rowCount; row++)
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
                            var subjectIds = worksheet.Cells[row, 8].Text.Split(',').Select(s => int.TryParse(s.Trim(), out var id) ? id : 0).Where(id => id > 0).ToList();

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
                                Description = description,
                                Ids = subjectIds
                            };

                            classList.Add(classSaveRequest);
                        }
                        catch (Exception ex)
                        {
                            errorMessages.Add($"Lỗi tại dòng {row}: {ex.Message}");
                        }
                    }
                }

                if (errorMessages.Count > 0)
                {
                    throw new Exception("Đã xảy ra lỗi khi đọc file Excel:\n" + string.Join("\n", errorMessages));
                }

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


        private bool IsExcelFile(byte[] fileBytes)
        {
            // Kiểm tra header của file (Magic Number)
            // File .xlsx có header: "50 4B 03 04" (PK ZIP format)
            if (fileBytes.Length < 4)
                return false;

            return fileBytes[0] == 0x50 && fileBytes[1] == 0x4B &&
                   fileBytes[2] == 0x03 && fileBytes[3] == 0x04;
        }


        public async Task<string> GenerateClassTemplate()
        {
            using (var package = new ExcelPackage())
            {
                // 1. Tạo sheet Template (Mẫu nhập liệu)
                var templateSheet = package.Workbook.Worksheets.Add("Template");
                var header = new string[]
                {
            "AcademicYearId", "DepartmentId", "ClassTypeId", "UserId", "ClassName", "StudentCount", "Description", "SubjectIds"
                };

                for (int col = 0; col < header.Length; col++)
                {
                    templateSheet.Cells[1, col + 1].Value = header[col];
                    templateSheet.Cells[1, col + 1].Style.Font.Bold = true;
                }

                // Thêm dữ liệu mẫu
                templateSheet.Cells[2, 1].Value = 1; // AcademicYearId
                templateSheet.Cells[2, 2].Value = 2; // DepartmentId
                templateSheet.Cells[2, 3].Value = 1; // ClassTypeId
                templateSheet.Cells[2, 4].Value = 3; // UserId
                templateSheet.Cells[2, 5].Value = "Lớp 12A1"; // ClassName
                templateSheet.Cells[2, 6].Value = 40; // StudentCount
                templateSheet.Cells[2, 7].Value = "Lớp chuyên toán"; // Description
                templateSheet.Cells[2, 8].Value = "1,3,5"; // SubjectIds (các môn học)

                // Ghi chú cho người dùng
                templateSheet.Cells[4, 1].Value = "Ghi chú:";
                templateSheet.Cells[5, 1].Value = "1. Các cột AcademicYearId, DepartmentId, ClassTypeId, UserId, SubjectIds phải khớp với dữ liệu bên sheet 'Data'.";
                templateSheet.Cells[6, 1].Value = "2. SubjectIds có thể nhập nhiều giá trị, cách nhau bằng dấu phẩy (ví dụ: 1,3,5).";

                // 2. Tạo sheet Data (Dữ liệu hỗ trợ nhập liệu)
                var dataSheet = package.Workbook.Worksheets.Add("Data");

                // Lấy dữ liệu từ DB
                var academicYears = await _context.AcademicYears
                    .Select(a => new { a.Id, Name = $"{a.StartDate:yyyy} - {a.EndDate:yyyy}" })
                    .ToListAsync();

                var departments = await _context.Departments
                    .Select(d => new { d.Id, d.Name })
                    .ToListAsync();

                var classTypes = await _context.ClassTypes
                    .Select(ct => new { ct.Id, ct.Name })
                    .ToListAsync();

                var users = await _context.Users
                    .Where(u => u.TeacherStatusId.HasValue)
                    .Select(u => new { u.Id, u.FullName })
                    .ToListAsync();

                var subjects = await _context.Subjects
                    .Select(s => new { s.Id, s.SubjectName })
                    .ToListAsync();

                // Điền dữ liệu vào sheet Data
                int startRow = 2;

                // AcademicYears
                dataSheet.Cells[1, 1].Value = "AcademicYearId";
                dataSheet.Cells[1, 2].Value = "AcademicYearName";
                for (int i = 0; i < academicYears.Count; i++)
                {
                    dataSheet.Cells[startRow + i, 1].Value = academicYears[i].Id;
                    dataSheet.Cells[startRow + i, 2].Value = academicYears[i].Name;
                }

                // Departments
                startRow = academicYears.Count + 4;
                dataSheet.Cells[startRow, 1].Value = "DepartmentId";
                dataSheet.Cells[startRow, 2].Value = "DepartmentName";
                for (int i = 0; i < departments.Count; i++)
                {
                    dataSheet.Cells[startRow + i + 1, 1].Value = departments[i].Id;
                    dataSheet.Cells[startRow + i + 1, 2].Value = departments[i].Name;
                }

                // ClassTypes
                startRow += departments.Count + 4;
                dataSheet.Cells[startRow, 1].Value = "ClassTypeId";
                dataSheet.Cells[startRow, 2].Value = "ClassTypeName";
                for (int i = 0; i < classTypes.Count; i++)
                {
                    dataSheet.Cells[startRow + i + 1, 1].Value = classTypes[i].Id;
                    dataSheet.Cells[startRow + i + 1, 2].Value = classTypes[i].Name;
                }

                // Users (Teachers)
                startRow += classTypes.Count + 4;
                dataSheet.Cells[startRow, 1].Value = "UserId";
                dataSheet.Cells[startRow, 2].Value = "TeacherName";
                for (int i = 0; i < users.Count; i++)
                {
                    dataSheet.Cells[startRow + i + 1, 1].Value = users[i].Id;
                    dataSheet.Cells[startRow + i + 1, 2].Value = users[i].FullName;
                }

                // Subjects
                startRow += users.Count + 4;
                dataSheet.Cells[startRow, 1].Value = "SubjectId";
                dataSheet.Cells[startRow, 2].Value = "SubjectName";
                for (int i = 0; i < subjects.Count; i++)
                {
                    dataSheet.Cells[startRow + i + 1, 1].Value = subjects[i].Id;
                    dataSheet.Cells[startRow + i + 1, 2].Value = subjects[i].SubjectName;
                }

                // Lưu file Excel vào byte array
                byte[] fileBytes = package.GetAsByteArray();

                // Chuyển đổi byte array sang chuỗi Base64
                string excelBase64 = Convert.ToBase64String(fileBytes);
                return await _cloudinaryService.UploadExcelAsync(excelBase64);
            }
        }

        public async Task<ApiResponse<PaginatedResponse<ClassFutureResponse>>> GetClassFuture(
    int? userId, string? keyword, int? subjectId, bool future, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;

                var currentDate = DateTime.Now;

                // Base query
                var query = _context.Lessons
                    .Include(l => l.TeachingAssignment)
                        .ThenInclude(ta => ta.Class)
                    .Include(l => l.TeachingAssignment)
                        .ThenInclude(ta => ta.Subject)
                    .Include(l => l.TeachingAssignment)
                        .ThenInclude(ta => ta.User)
                    .Where(l => l.TeachingAssignment.UserId == userId && // Filter by teacher ID
                               (!l.IsDelete.HasValue || !l.IsDelete.Value) &&
                               (!l.TeachingAssignment.IsDelete.HasValue || !l.TeachingAssignment.IsDelete.Value) &&
                               l.TeachingAssignment.Class != null &&
                               (!l.TeachingAssignment.Class.IsDelete.HasValue || !l.TeachingAssignment.Class.IsDelete.Value));

                // Debug: Log total records before filtering
                var totalRecords = await query.CountAsync();
                Console.WriteLine($"Total records before filtering: {totalRecords}");

                // Filter by subject if provided
                if (subjectId.HasValue && subjectId > 0)
                {
                    query = query.Where(l => l.TeachingAssignment.SubjectId == subjectId);
                }

                // Apply keyword search if provided 
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    keyword = keyword.ToLower().Trim();
                    query = query.Where(l =>
                        (l.TeachingAssignment.Class.ClassCode != null &&
                         l.TeachingAssignment.Class.ClassCode.ToLower().Contains(keyword)) ||
                        (l.TeachingAssignment.Class.Name != null &&
                         l.TeachingAssignment.Class.Name.ToLower().Contains(keyword)) ||
                        (l.TeachingAssignment.Subject.SubjectName != null &&
                         l.TeachingAssignment.Subject.SubjectName.ToLower().Contains(keyword))
                    );
                }

                // Get all lessons
                var allLessons = await query
                    .Select(l => new ClassFutureResponse
                    {
                        TeachingAssignmentId = l.TeachingAssignment.Id,
                        ClassCode = l.TeachingAssignment.Class.ClassCode,
                        SubjectName = l.TeachingAssignment.Subject.SubjectName,
                        StartDate = l.StartDate,
                        TeacherName = l.TeachingAssignment.User.FullName,
                        StatusClass = l.TeachingAssignment.Class.StatusClass
                    })
                    .ToListAsync();

                // Filter and sort by date
                var filteredLessons = future
                    ? allLessons.Where(l => l.StartDate >= currentDate).ToList()
                    : allLessons.Where(l => l.StartDate < currentDate).ToList();

                var sortedLessons = future
                    ? filteredLessons.OrderBy(l => l.StartDate).ToList()
                    : filteredLessons.OrderByDescending(l => l.StartDate).ToList();

                // Calculate pagination
                var totalItems = sortedLessons.Count;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                // Get paginated items
                var paginatedLessons = sortedLessons
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var paginatedResponse = new PaginatedResponse<ClassFutureResponse>
                {
                    Items = paginatedLessons,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    HasPreviousPage = pageNumber > 1,
                    HasNextPage = pageNumber < totalPages
                };

                var message = !future
                    ? (paginatedLessons.Any() ? "Lấy danh sách buổi học đã qua thành công" : "Không có bài học đã qua")
                    : (paginatedLessons.Any() ? "Lấy danh sách buổi học sắp tới thành công" : "Không có bài học sắp tới");

                return new ApiResponse<PaginatedResponse<ClassFutureResponse>>(0, message, paginatedResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginatedResponse<ClassFutureResponse>>(1,
                    $"Lỗi khi lấy danh sách bài học: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<TeachingAssignmentDetailResponse>> GetClassFutureDetail(int teachingAssignmentId)
        {
            try
            {
                var teachingAssignment = await _context.TeachingAssignments
                    .Include(ta => ta.Class)
                    .Include(ta => ta.Subject)
                    .Include(ta => ta.User)
                    .Include(ta => ta.Lessons.Where(l => !l.IsDelete.HasValue || !l.IsDelete.Value))
                        .ThenInclude(l => l.User)
                    .Where(ta => ta.Id == teachingAssignmentId &&
                                (!ta.IsDelete.HasValue || !ta.IsDelete.Value))
                    .Select(ta => new TeachingAssignmentDetailResponse
                    {
                        Id = ta.Id,
                        ClassName = ta.Class.Name,
                        Description = ta.Class.Description,
                        Name = ta.Class.Name,
                        SubjectName = ta.Subject.SubjectName,
                        TeacherName = ta.User.FullName,
                        StartDate = ta.StartDate,
                        EndDate = ta.EndDate,
                        TotalLessons = ta.Lessons.Count(l => !l.IsDelete.HasValue || !l.IsDelete.Value),
                        Lessons = ta.Lessons
                            .Where(l => !l.IsDelete.HasValue || !l.IsDelete.Value)
                            .OrderBy(l => l.StartDate) // Sort by StartDate ascending
                            .Select(l => new ClassLessonResponse
                            {
                                Id = l.Id,
                                ClassLessonCode = l.ClassLessonCode,
                                Description = l.Description,
                                PaswordLeassons = l.PaswordLeassons,
                                Topic = l.Topic,
                                Duration = l.Duration,
                                StartDate = l.StartDate,
                                EndDate = l.EndDate,
                                IsResearchable = l.IsResearchable,
                                IsAutoStart = l.IsAutoStart,
                                IsSave = l.IsSave,
                                LessonLink = l.LessonLink,
                                TeacherName = l.User.FullName
                            })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                if (teachingAssignment == null)
                {
                    return new ApiResponse<TeachingAssignmentDetailResponse>(1, "Không tìm thấy phân công giảng dạy", null);
                }

                return new ApiResponse<TeachingAssignmentDetailResponse>(0, "Lấy thông tin phân công giảng dạy thành công", teachingAssignment);
            }
            catch (Exception ex)
            {
                return new ApiResponse<TeachingAssignmentDetailResponse>(1, $"Lỗi khi lấy thông tin phân công giảng dạy: {ex.Message}", null);
            }
        }


        public async Task<ApiResponse<PaginatedResponse<ClassFutureStudentResponse>>> GetClassLessonStudent(
    int? userId, string? keyword, int? subjectId, int status, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;

                var currentDate = DateTime.Now;

                // Query từ Lessons để lấy từng buổi học riêng lẻ
                var query = _context.Lessons
                    .Include(l => l.TeachingAssignment)
                        .ThenInclude(ta => ta.Class)
                            .ThenInclude(c => c.ClassStudents)
                    .Include(l => l.TeachingAssignment)
                        .ThenInclude(ta => ta.Subject)
                    .Include(l => l.TeachingAssignment)
                        .ThenInclude(ta => ta.User)
                    .Where(l =>
                        l.TeachingAssignment.Class.ClassStudents.Any(cs =>
                            cs.UserId == userId &&
                            (!cs.IsDelete.HasValue || !cs.IsDelete.Value)) &&
                        (!l.IsDelete.HasValue || !l.IsDelete.Value) &&
                        (!l.TeachingAssignment.IsDelete.HasValue || !l.TeachingAssignment.IsDelete.Value) &&
                        l.TeachingAssignment.Class != null &&
                        (!l.TeachingAssignment.Class.IsDelete.HasValue || !l.TeachingAssignment.Class.IsDelete.Value));

                // Filter by subject
                if (subjectId.HasValue && subjectId > 0)
                {
                    query = query.Where(l => l.TeachingAssignment.SubjectId == subjectId);
                }

                // Apply keyword search
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    keyword = keyword.ToLower().Trim();
                    query = query.Where(l =>
                        (l.TeachingAssignment.Class.ClassCode != null &&
                         l.TeachingAssignment.Class.ClassCode.ToLower().Contains(keyword)) ||
                        (l.TeachingAssignment.Class.Name != null &&
                         l.TeachingAssignment.Class.Name.ToLower().Contains(keyword)) ||
                        (l.TeachingAssignment.Subject.SubjectName != null &&
                         l.TeachingAssignment.Subject.SubjectName.ToLower().Contains(keyword))
                    );
                }

                // Get all lessons first
                var allLessons = await query
                    .Select(l => new
                    {
                        TeachingAssignmentId = l.TeachingAssignment.Id,
                        ClassCode = l.TeachingAssignment.Class.ClassCode,
                        SubjectName = l.TeachingAssignment.Subject.SubjectName,
                        StartDate = l.StartDate,
                        TeacherName = l.TeachingAssignment.User.FullName,
                        StatusClass = l.TeachingAssignment.Class.StatusClass,
                        IsCompleted = l.StartDate < currentDate
                    })
                    .ToListAsync();

                // Group by TeachingAssignment to get totals
                var teachingAssignmentTotals = allLessons
                    .GroupBy(l => l.TeachingAssignmentId)
                    .ToDictionary(
                        g => g.Key,
                        g => new
                        {
                            TotalLessons = g.Count(),
                            CompletedLessons = g.Count(l => l.IsCompleted)
                        });

                // Filter based on status
                var filteredLessons = status switch
                {
                    0 => allLessons, // All lessons
                    1 => allLessons.Where(l => l.StartDate >= currentDate), // Upcoming lessons
                    2 => allLessons.Where(l => l.StartDate < currentDate), // Completed
                    3 => allLessons
        .GroupBy(l => l.TeachingAssignmentId)
        .Where(g =>
            // Điều kiện 1: Có ít nhất 1 buổi đã qua và có buổi sắp tới
            (g.Any(l => l.StartDate < currentDate) && g.Any(l => l.StartDate >= currentDate)) ||
            // Điều kiện 2: Chỉ có 1 buổi và buổi đó chưa diễn ra
            (g.Count() == 1 && g.First().StartDate >= currentDate)
        )
        .Select(g => new
        {
            TeachingAssignmentId = g.Key,
            ClassCode = g.First().ClassCode,
            SubjectName = g.First().SubjectName,
            StartDate = g.Where(l => l.StartDate >= currentDate)
                        .OrderBy(l => l.StartDate)
                        .First().StartDate,
            TeacherName = g.First().TeacherName,
            StatusClass = g.First().StatusClass,
            IsCompleted = g.First().IsCompleted
        })
        .ToList(),
                    _ => allLessons
                };

                // Sort based on status
                var sortedLessons = status switch
                {
                    0 => filteredLessons.OrderBy(l => l.StartDate),
                    1 => filteredLessons.OrderBy(l => l.StartDate),
                    2 => filteredLessons.OrderByDescending(l => l.StartDate),
                    3 => filteredLessons.OrderBy(l => l.StartDate),
                    _ => filteredLessons.OrderBy(l => l.StartDate)
                };

                // Convert to response type
                var items = sortedLessons.Select(l => new ClassFutureStudentResponse
                {
                    TeachingAssignmentId = l.TeachingAssignmentId,
                    ClassCode = l.ClassCode,
                    SubjectName = l.SubjectName,
                    StartDate = l.StartDate,
                    TeacherName = l.TeacherName,
                    StatusClass = l.StatusClass,
                    TotalLessons = teachingAssignmentTotals[l.TeachingAssignmentId].TotalLessons,
                    CompletedLessons = teachingAssignmentTotals[l.TeachingAssignmentId].CompletedLessons,
                    CompletionPercentage = teachingAssignmentTotals[l.TeachingAssignmentId].TotalLessons > 0
                        ? Math.Round((double)teachingAssignmentTotals[l.TeachingAssignmentId].CompletedLessons /
                                   teachingAssignmentTotals[l.TeachingAssignmentId].TotalLessons * 100, 2)
                        : 0
                }).ToList();

                // Apply pagination
                var totalItems = items.Count;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var paginatedItems = items
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var paginatedResponse = new PaginatedResponse<ClassFutureStudentResponse>
                {
                    Items = paginatedItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    HasPreviousPage = pageNumber > 1,
                    HasNextPage = pageNumber < totalPages
                };

                var message = status switch
                {
                    0 => paginatedItems.Any() ? "Lấy danh sách tất cả buổi học thành công" : "Không có buổi học nào",
                    1 => paginatedItems.Any() ? "Lấy danh sách buổi học sắp tới thành công" : "Không có buổi học sắp tới",
                    2 => paginatedItems.Any() ? "Lấy danh sách lớp học đã hoàn thành thành công" : "Không có lớp học đã hoàn thành",
                    3 => paginatedItems.Any() ? "Lấy danh sách lớp học chưa hoàn thành thành công" : "Không có lớp học chưa hoàn thành",
                    _ => "Lấy danh sách buổi học thành công"
                };

                return new ApiResponse<PaginatedResponse<ClassFutureStudentResponse>>(0, message, paginatedResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginatedResponse<ClassFutureStudentResponse>>(1,
                    $"Lỗi khi lấy danh sách buổi học: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<TeachingAssignmentDetailResponse>> GetClassLessonStudentDetail(int teachingAssignmentId)
        {
            try
            {
                var teachingAssignment = await _context.TeachingAssignments
                    .Include(ta => ta.Class)
                        .ThenInclude(c => c.ClassStudents)
                    .Include(ta => ta.Subject)
                    .Include(ta => ta.User)
                    .Include(ta => ta.Lessons.Where(l => !l.IsDelete.HasValue || !l.IsDelete.Value))
                        .ThenInclude(l => l.User)
                    .Where(ta => ta.Id == teachingAssignmentId &&
                                (!ta.IsDelete.HasValue || !ta.IsDelete.Value) &&
                                ta.Class != null &&
                                (!ta.Class.IsDelete.HasValue || !ta.Class.IsDelete.Value))
                    .Select(ta => new TeachingAssignmentDetailResponse
                    {
                        Id = ta.Id,
                        ClassName = ta.Class.Name,
                        Description = ta.Class.Description,
                        Name = ta.Class.Name,
                        SubjectName = ta.Subject.SubjectName,
                        TeacherName = ta.User.FullName,
                        StartDate = ta.StartDate,
                        EndDate = ta.EndDate,
                        TotalLessons = ta.Lessons.Count(l => !l.IsDelete.HasValue || !l.IsDelete.Value),
                        Lessons = ta.Lessons
                            .Where(l => !l.IsDelete.HasValue || !l.IsDelete.Value)
                            .OrderBy(l => l.StartDate) // Sort by StartDate ascending
                            .Select(l => new ClassLessonResponse
                            {
                                Id = l.Id,
                                ClassLessonCode = l.ClassLessonCode,
                                Description = l.Description,
                                PaswordLeassons = l.PaswordLeassons,
                                Topic = l.Topic,
                                Duration = l.Duration,
                                StartDate = l.StartDate,
                                EndDate = l.EndDate,
                                IsResearchable = l.IsResearchable,
                                IsAutoStart = l.IsAutoStart,
                                IsSave = l.IsSave,
                                LessonLink = l.LessonLink,
                                TeacherName = l.User.FullName
                            })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                if (teachingAssignment == null)
                {
                    return new ApiResponse<TeachingAssignmentDetailResponse>(1, "Không tìm thấy phân công giảng dạy", null);
                }

                return new ApiResponse<TeachingAssignmentDetailResponse>(0, "Lấy thông tin phân công giảng dạy thành công", teachingAssignment);
            }
            catch (Exception ex)
            {
                return new ApiResponse<TeachingAssignmentDetailResponse>(1,
                    $"Lỗi khi lấy thông tin phân công giảng dạy: {ex.Message}", null);
            }
        }


        public async Task<List<Class_UserResponse>> GetClassesByAcademicYear(int academicYearId)
        {
            var query = _context.Classes
                .Where(c => c.AcademicYearId == academicYearId && c.IsDelete == false);

            var classes = await query
                .Select(c => new Class_UserResponse
                {
                    ClassId = c.Id,
                    ClassName = c.Name ?? string.Empty,
                })
                .ToListAsync();

            return classes;
        }

        public async Task<List<ClassDropdownResponse>> GetClassesDropdown(int academicYearId, int departmentId)
        {
            var classes = await _context.Classes
                .Where(c => c.AcademicYearId == academicYearId && c.DepartmentId == departmentId && c.IsDelete == false)
                .OrderBy(c => c.Name)
                .Select(c => new ClassDropdownResponse
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            return classes;
        }
    }

}