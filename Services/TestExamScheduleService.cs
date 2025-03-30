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

    public async Task<ApiResponse<List<TestExamScheduleResponse>>> GetExamScheduleAsync(DateTimeOffset? mount,
        bool week)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7));

        // Nếu 'mount' có giá trị thì lấy start và end của tháng, nếu không lấy tháng hiện tại
        DateTimeOffset startOfMonth = mount.HasValue
            ? new DateTimeOffset(mount.Value.Year, mount.Value.Month, 1, 0, 0, 0, TimeSpan.FromHours(7))
            : new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.FromHours(7));

        DateTimeOffset endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);

        // Nếu lọc theo tuần, lấy tuần của tháng hiện tại
        DateTimeOffset startOfWeek = now.Date;
        DateTimeOffset endOfWeek = startOfWeek.AddDays(6).AddSeconds(59);

        if (week)
        {
            startOfWeek = now.Date;
            endOfWeek = startOfWeek.AddDays(6).AddSeconds(59);
        }

        // Lọc dữ liệu từ database, xử lý khi StartDate có thể null
        var testExams = await _context.TestExams
            .Include(te => te.Subject)
            .Include(te => te.Class)
            .Include(te => te.Department)
            .Where(te =>
                (week && te.StartDate.HasValue && te.StartDate.Value >= startOfWeek &&
                 te.StartDate.Value <= endOfWeek) ||
                (!week && te.StartDate.HasValue && te.StartDate.Value >= startOfMonth &&
                 te.StartDate.Value <= endOfMonth) ||
                // Xử lý trường hợp StartDate là null
                (!week && !te.StartDate.HasValue)
            )
            .ToListAsync();

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

            var classes = _context.ClassTestExams
                .Where(cte => cte.TestExamId == te.Id)
                .Select(cte => cte.Class.Name)
                .ToList();

            return new TestExamScheduleResponse
            {
                SubjectAndDuration = $"{te.Subject.SubjectName} - {durationString}",
                ClassName = string.Join(", ", classes),
                DepartmentName = te.Department.Name,
                StartDate = te.StartDate
            };
        }).ToList();

        return new ApiResponse<List<TestExamScheduleResponse>>(0, "Lấy danh sách thành công!", response);
    }

    public async Task<ApiResponse<List<TestExamScheduleResponse>>> GetExamScheduleStudentAndTeacherAsync(DateTimeOffset? mount,
        bool week, int? departmentId)
{
    DateTimeOffset now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7));

    // Nếu 'mount' có giá trị thì lấy start và end của tháng, nếu không lấy tháng hiện tại
    DateTimeOffset startOfMonth = mount.HasValue
        ? new DateTimeOffset(mount.Value.Year, mount.Value.Month, 1, 0, 0, 0, TimeSpan.FromHours(7))
        : new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.FromHours(7));

    DateTimeOffset endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);

    // Nếu lọc theo tuần, lấy tuần của tháng hiện tại
    DateTimeOffset startOfWeek = now.Date;
    DateTimeOffset endOfWeek = startOfWeek.AddDays(6).AddSeconds(59);

    if (week)
    {
        startOfWeek = now.Date;
        endOfWeek = startOfWeek.AddDays(6).AddSeconds(59);
    }

    // Lọc dữ liệu từ database, xử lý khi StartDate có thể null
    var testExams = await _context.TestExams
        .Include(te => te.Subject)
        .Include(te => te.Class)
        .Include(te => te.Department)
        .Where(te =>
            (
                (week && te.StartDate.HasValue && te.StartDate.Value >= startOfWeek && te.StartDate.Value <= endOfWeek) ||
                (!week && te.StartDate.HasValue && te.StartDate.Value >= startOfMonth && te.StartDate.Value <= endOfMonth) ||
                // Xử lý trường hợp StartDate là null
                (!week && !te.StartDate.HasValue)
            ) &&
            // Apply department filter only if departmentId is not null
            (!departmentId.HasValue || te.DepartmentId == departmentId)
        )
        .ToListAsync();

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

        var classes = _context.ClassTestExams
            .Where(cte => cte.TestExamId == te.Id)
            .Select(cte => cte.Class.Name)
            .ToList();

        return new TestExamScheduleResponse
        {
            SubjectAndDuration = $"{te.Subject.SubjectName} - {durationString}",
            ClassName = string.Join(", ", classes),
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
            .Include(te=> te.Subject)// Giả sử User là giảng viên
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
    
     public async Task<ApiResponse<List<TestExamScheduleDetailForStudentAndTeacherResponse>>> GetExamScheduleDetailForStudentAndTeacherAsync(
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
            .Include(ts => ts.Subject)// Giả sử User là giảng viên
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

            var classes = _context.ClassTestExams
                .Where(cte => cte.TestExamId == te.Id)
                .Select(cte => cte.Class.Name)
                .ToList();
            // Trả về thông tin chi tiết lịch thi
            return new TestExamScheduleDetailForStudentAndTeacherResponse()
            {
                SubjectName = te.Subject.SubjectName,
                ClassList = string.Join(", ", classes),
                Duration = durationString, // Sử dụng chuỗi durationString đã tạo
                TestExamType = te.TestExamType.PointTypeName,
                Topic = te.Topic,
                Form = te.Form
            };
        }).ToList();

        // Trả về kết quả API với thông tin chi tiết
        return new ApiResponse<List<TestExamScheduleDetailForStudentAndTeacherResponse>>(0, "Lấy chi tiết lịch thi thành công!", response);
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