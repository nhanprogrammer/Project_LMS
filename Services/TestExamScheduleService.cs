using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services;

public class TestExamScheduleService : ITestExamScheduleService
{
    private readonly ApplicationDbContext _context;

    public TestExamScheduleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<TestExamScheduleResponse>>> GetExamScheduleAsync(int? month, int? year,
        bool week, int? departmentId, DateTimeOffset? startDateOffWeek)
    {
        if (month.HasValue && (month < 1 || month > 12))
        {
            return new ApiResponse<List<TestExamScheduleResponse>>(0,
                "Tháng không hợp lệ. Vui lòng nhập giá trị từ 1 đến 12.", null);
        }

        if (departmentId.HasValue)
        {
            var departmentExists = await _context.Departments.AnyAsync(d => d.Id == departmentId);
            if (!departmentExists)
            {
                return new ApiResponse<List<TestExamScheduleResponse>>(0, "Khối lớp học không tồn tại.", null);
            }
        }

        DateTimeOffset now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7));
        int selectedYear = year ?? now.Year;
        int selectedMonth = month ?? now.Month;

        DateTimeOffset startOfMonth =
            new DateTimeOffset(selectedYear, selectedMonth, 1, 0, 0, 0, TimeSpan.FromHours(7));
        DateTimeOffset endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);

        DateTimeOffset startOfWeek = now.Date;
        DateTimeOffset endOfWeek = startOfWeek.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);

        if (week)
        {
            startOfWeek = startDateOffWeek?.Date ?? now.Date;
            endOfWeek = startOfWeek.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);
        }

        var testExams = await _context.TestExams
            .Include(te => te.Subject)
            .Include(te => te.Class)
            .Include(te => te.Department)
            .Where(te =>
                (
                    (week && te.StartDate.HasValue && te.StartDate.Value >= startOfWeek &&
                     te.StartDate.Value <= endOfWeek) ||
                    (!week && te.StartDate.HasValue && te.StartDate.Value >= startOfMonth &&
                     te.StartDate.Value <= endOfMonth) ||
                    (!week && !te.StartDate.HasValue)
                ) &&
                (!departmentId.HasValue || te.DepartmentId == departmentId)
            )
            .ToListAsync();

        if (testExams == null || !testExams.Any())
        {
            return new ApiResponse<List<TestExamScheduleResponse>>(1, "Không tìm thấy dữ liệu.", null);
        }

        var response = testExams.Select(te =>
        {
            var durationTimeSpan = te.Duration.HasValue
                ? te.Duration.Value.ToTimeSpan()
                : TimeSpan.Zero; // Sử dụng TimeSpan.Zero nếu Duration không có giá trị

            var hours = durationTimeSpan.Hours;
            var minutes = durationTimeSpan.Minutes;

            string durationString;
            if (hours > 0 && minutes > 0)
            {
                durationString = $"{hours} giờ {minutes} phút";
            }
            else if (hours > 0)
            {
                durationString = $"{hours} giờ";
            }
            else
            {
                durationString = $"{minutes} phút";
            }

            // Lấy danh sách ClassId của kỳ thi
            var classIds = _context.ClassTestExams
                .Where(cte => cte.TestExamId == te.Id)
                .Select(cte => cte.ClassId)
                .ToList();

// Lấy danh sách ClassTypeId của các lớp (loại bỏ trùng lặp)
            var classTypeIds = _context.Classes
                .Where(c => classIds.Contains(c.Id))
                .Select(c => c.ClassTypeId)
                .Distinct()
                .ToList();

// Lấy danh sách DepartmentId từ các lớp (loại bỏ trùng lặp)
            var departmentIds = _context.Classes
                .Where(c => classIds.Contains(c.Id))
                .Select(c => c.DepartmentId)
                .Distinct()
                .ToList();

            string classNameResult;

// Nếu tất cả lớp thuộc cùng một Department (khối), hiển thị tên Department
            if (departmentIds.Count == 1 && departmentIds.First().HasValue)
            {
                classNameResult = _context.Departments
                    .Where(d => d.Id == departmentIds.First().Value)
                    .Select(d => d.Name)
                    .FirstOrDefault() ?? "Không xác định";
            }
// Nếu tất cả lớp thuộc cùng một ClassType, hiển thị tên ClassType
            else if (classTypeIds.Count == 1 && classTypeIds.First().HasValue)
            {
                classNameResult = _context.ClassTypes
                    .Where(ct => ct.Id == classTypeIds.First().Value)
                    .Select(ct => ct.Name)
                    .FirstOrDefault() ?? "Không xác định";
            }
// Nếu không thoả cả hai điều kiện trên, hiển thị danh sách lớp
            else
            {
                var classNames = _context.Classes
                    .Where(c => classIds.Contains(c.Id))
                    .Select(c => c.Name)
                    .ToList();

                classNameResult = string.Join(", ", classNames);
            }

            return new TestExamScheduleResponse
            {
                SubjectAndDuration = $"{te.Subject.SubjectName} - {durationString}",
                ClassName = classNameResult,
                DepartmentName = te.Department.Name,
                StartDate = te.StartDate
            };
        }).ToList();

        return new ApiResponse<List<TestExamScheduleResponse>>(0, "Lấy danh sách thành công!", response);
    }


    public async Task<ApiResponse<List<TestExamScheduleDetailResponse>>> GetExamScheduleDetailAsync(
        DateTimeOffset startdate)
    {
        DateTimeOffset selectedDate = new DateTimeOffset(startdate.Year, startdate.Month, startdate.Day, 0, 0, 0,
            TimeSpan.FromHours(7));

        // Lọc danh sách lịch thi theo ngày đã chọn (so sánh chỉ theo ngày, tháng, năm)
        var testExams = await _context.TestExams
            .Include(te => te.ClassTestExams)
            .ThenInclude(cte => cte.Class)
            .ThenInclude(cl => cl.User)
            .Include(te => te.TestExamType)
            .Include(te => te.Subject) // Giả sử User là giảng viên
            .Where(te =>
                    te.StartDate.HasValue &&
                    te.StartDate.Value.Date == selectedDate.Date
                    && te.IsDelete == false
                // So sánh chỉ theo ngày, tháng và năm (bỏ qua giờ)
            )
            .ToListAsync();


        // Tạo danh sách phản hồi với thông tin chi tiết về bài thi
        var response = testExams.Select(te =>
        {
            // Kiểm tra xem Duration có giá trị không
            var durationTimeSpan =
                te.Duration.HasValue ? te.Duration.Value.ToTimeSpan() : TimeSpan.Zero; // Chuyển TimeOnly thành TimeSpan

            var hours = durationTimeSpan.Hours;
            var minutes = durationTimeSpan.Minutes;

            // Tạo chuỗi Duration theo định dạng giờ và phút
            string durationString;
            if (hours > 0 && minutes > 0)
            {
                durationString = $"{hours} giờ {minutes} phút";
            }
            else if (hours > 0)
            {
                durationString = $"{hours} giờ";
            }
            else
            {
                durationString = $"{minutes} phút";
            }

            // Trả về thông tin chi tiết lịch thi
            return new TestExamScheduleDetailResponse
            {
                SubjectName = te.Subject.SubjectName,
                TestExamId = te.Id,
                TeacherName = te.ClassTestExams.FirstOrDefault()?.Class.User?.FullName ?? "Không có giảng viên",
                Duration = durationString, // Sử dụng chuỗi durationString đã tạo
                TestExamType = te.TestExamType.PointTypeName,
                Form = te.Form
            };
        }).ToList();

        // Trả về kết quả API với thông tin chi tiết
        return new ApiResponse<List<TestExamScheduleDetailResponse>>(0, "Lấy chi tiết lịch thi thành công!", response);
    }

    public async Task<ApiResponse<List<TestExamScheduleDetailForStudentAndTeacherResponse>>>
        GetExamScheduleDetailForStudentAndTeacherAsync(
            DateTimeOffset startdate)
    {
        DateTimeOffset selectedDate = new DateTimeOffset(startdate.Year, startdate.Month, startdate.Day, 0, 0, 0,
            TimeSpan.FromHours(7));

        // Lọc danh sách lịch thi theo ngày đã chọn (so sánh chỉ theo ngày, tháng, năm)
        var testExams = await _context.TestExams
            .Include(te => te.ClassTestExams)
            .ThenInclude(cte => cte.Class)
            .ThenInclude(cl => cl.User)
            .Include(te => te.TestExamType)
            .Include(ts => ts.Subject) // Giả sử User là giảng viên
            .Where(te =>
                    te.StartDate.HasValue &&
                    te.StartDate.Value.Date == selectedDate.Date
                    && te.IsDelete == false
                // So sánh chỉ theo ngày, tháng và năm (bỏ qua giờ)
            )
            .ToListAsync();


        // Tạo danh sách phản hồi với thông tin chi tiết về bài thi
        var response = testExams.Select(te =>
        {
            // Kiểm tra xem Duration có giá trị không
            var durationTimeSpan =
                te.Duration.HasValue ? te.Duration.Value.ToTimeSpan() : TimeSpan.Zero; // Chuyển TimeOnly thành TimeSpan

            var hours = durationTimeSpan.Hours;
            var minutes = durationTimeSpan.Minutes;

            // Tạo chuỗi Duration theo định dạng giờ và phút
            string durationString;
            if (hours > 0 && minutes > 0)
            {
                durationString = $"{hours} giờ {minutes} phút";
            }
            else if (hours > 0)
            {
                durationString = $"{hours} giờ";
            }
            else
            {
                durationString = $"{minutes} phút";
            }

            var classIds = _context.ClassTestExams
                .Where(cte => cte.TestExamId == te.Id)
                .Select(cte => cte.ClassId)
                .ToList();

// Lấy danh sách ClassTypeId của các lớp (loại bỏ trùng lặp)
            var classTypeIds = _context.Classes
                .Where(c => classIds.Contains(c.Id))
                .Select(c => c.ClassTypeId)
                .Distinct()
                .ToList();

// Nếu tất cả lớp có cùng ClassType, chỉ hiển thị ClassType, ngược lại hiển thị danh sách lớp
            string classNameResult;
            if (classTypeIds.Count == 1 && classTypeIds.First().HasValue)
            {
                classNameResult = _context.ClassTypes
                    .Where(ct => ct.Id == classTypeIds.First().Value)
                    .Select(ct => ct.Name)
                    .FirstOrDefault() ?? "Không xác định";
            }
            else
            {
                var classNames = _context.ClassTestExams
                    .Where(cte => cte.TestExamId == te.Id)
                    .Select(cte => cte.Class.Name)
                    .ToList();

                classNameResult = string.Join(", ", classNames);
            }

            // Trả về thông tin chi tiết lịch thi
            return new TestExamScheduleDetailForStudentAndTeacherResponse()
            {
                SubjectName = te.Subject.SubjectName,
                ClassList = classNameResult,
                Duration = durationString, // Sử dụng chuỗi durationString đã tạo
                TestExamType = te.TestExamType.PointTypeName,
                Topic = te.Topic,
                Form = te.Form
            };
        }).ToList();

        // Trả về kết quả API với thông tin chi tiết
        return new ApiResponse<List<TestExamScheduleDetailForStudentAndTeacherResponse>>(0,
            "Lấy chi tiết lịch thi thành công!", response);
    }

    public async Task<ApiResponse<Object>> DeleteExamScheduleDetailByIdAsync(int id)
    {
        var testExam = await _context.TestExams.FindAsync(id);


        testExam.IsDelete = true;
        _context.TestExams.Update(testExam);
        await _context.SaveChangesAsync();
        return new ApiResponse<Object>(0, "XÓa thành công", null);
    }
}