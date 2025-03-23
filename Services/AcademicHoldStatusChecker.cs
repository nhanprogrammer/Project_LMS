using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class AcademicHoldStatusCheckerJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<AcademicHoldStatusCheckerJob> _logger;

        public AcademicHoldStatusCheckerJob(IServiceScopeFactory serviceScopeFactory, ILogger<AcademicHoldStatusCheckerJob> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Bắt đầu kiểm tra trạng thái bảo lưu tại thời điểm: {Now}", DateTime.UtcNow);

            var now = DateTime.UtcNow;
            _logger.LogInformation("Thời gian test được gán: {Now}", now);

            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Lấy danh sách các User đang bảo lưu
            var usersOnHold = await context.Users
                .Include(u => u.AcademicHolds)
                .Where(u => u.StudentStatusId == 2 && !(u.IsDelete ?? false))
                .ToListAsync(stoppingToken);

            _logger.LogInformation("Số lượng user đang bảo lưu: {Count}", usersOnHold.Count);
            if (usersOnHold.Count == 0)
            {
                _logger.LogInformation("Không có user nào đang bảo lưu. Kết thúc kiểm tra.");
                return;
            }

            // Kiểm tra học kỳ của thời gian hiện tại (now)
            var currentSemester = await context.Semesters
                .Where(s => s.IsDelete == false
                         && s.StartDate.HasValue && s.EndDate.HasValue
                         && s.StartDate.Value <= now
                         && s.EndDate.Value >= now)
                .FirstOrDefaultAsync(stoppingToken);

            if (currentSemester == null)
            {
                _logger.LogWarning("Không tìm thấy học kỳ tại thời điểm {Now}. Không mở bảo lưu.", now);
                return;
            }

            _logger.LogInformation("Học kỳ hiện tại: {SemesterName} ({StartDate} - {EndDate})",
                currentSemester.Name, currentSemester.StartDate, currentSemester.EndDate);

            foreach (var user in usersOnHold)
            {
                _logger.LogInformation("Kiểm tra user {UserId}: {FullName}", user.Id, user.FullName);

                // Lấy bản ghi bảo lưu gần nhất
                var activeHold = user.AcademicHolds
                    .Where(ah => !ah.IsDelete && ah.HoldDate.HasValue)
                    .OrderByDescending(ah => ah.HoldDate)
                    .FirstOrDefault();

                if (activeHold == null)
                {
                    _logger.LogInformation("User {UserId} không có bản ghi bảo lưu. Cập nhật trạng thái về Active.", user.Id);
                    user.StudentStatusId = 1;
                    context.Users.Update(user);
                    continue;
                }

                _logger.LogInformation("Bản ghi bảo lưu gần nhất của User {UserId}: HoldDate = {HoldDate}, HoldDuration = {HoldDuration}",
                    user.Id, activeHold.HoldDate, activeHold.HoldDuration);

                // Kiểm tra thời gian bảo lưu đã hết chưa
                var holdEndDate = activeHold.HoldDate; // Sửa lỗi: Tính ngày kết thúc bảo lưu
                _logger.LogInformation("Thời gian kết thúc bảo lưu của User {UserId}: {HoldEndDate}", user.Id, holdEndDate);

                if (holdEndDate >= now)
                {
                    _logger.LogInformation("User {UserId} vẫn đang trong thời gian bảo lưu (kết thúc: {HoldEndDate}). Giữ trạng thái OnHold.",
                        user.Id, holdEndDate);
                    continue;
                }

                // Lấy học kỳ của ngày bảo lưu (HoldDate)
                var holdSemester = await context.Semesters
                    .Where(s => s.IsDelete == false
                             && s.StartDate.HasValue && s.EndDate.HasValue
                             && s.StartDate.Value <= activeHold.HoldDate
                             && s.EndDate.Value >= activeHold.HoldDate)
                    .FirstOrDefaultAsync(stoppingToken);

                if (holdSemester == null)
                {
                    _logger.LogWarning("Không tìm thấy học kỳ cho ngày bảo lưu {HoldDate} của User {UserId}. Giữ trạng thái OnHold.",
                        activeHold.HoldDate, user.Id);
                    continue;
                }

                _logger.LogInformation("Học kỳ của ngày bảo lưu (User {UserId}): {SemesterName} ({StartDate} - {EndDate})",
                    user.Id, holdSemester.Name, holdSemester.StartDate, holdSemester.EndDate);

                // So sánh học kỳ
                if (holdSemester.Id == currentSemester.Id)
                {
                    _logger.LogInformation("User {UserId} bảo lưu trong cùng học kỳ ({SemesterName}). Giữ trạng thái OnHold.",
                        user.Id, holdSemester.Name);
                    continue;
                }

                // Nếu thời gian bảo lưu đã hết và học kỳ khác nhau, mở bảo lưu
                _logger.LogInformation("User {UserId} có bảo lưu đã hết hạn (kết thúc: {HoldEndDate}) và học kỳ hiện tại ({CurrentSemesterName}) khác học kỳ bảo lưu ({HoldSemesterName}). Cập nhật trạng thái về Active.",
                    user.Id, holdEndDate, currentSemester.Name, holdSemester.Name);
                user.StudentStatusId = 1; // Active
                context.Users.Update(user);
            }

            await context.SaveChangesAsync(stoppingToken);
            _logger.LogInformation("Hoàn tất kiểm tra trạng thái bảo lưu.");
        }
    }
}