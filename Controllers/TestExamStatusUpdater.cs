// using Microsoft.AspNetCore.SignalR;
// using Microsoft.EntityFrameworkCore;
// using Project_LMS.Data;
// using Project_LMS.Hubs;
//
// namespace Project_LMS.Controllers;
//
// public class TestExamStatusUpdater : BackgroundService
// {
//     private readonly IServiceScopeFactory _serviceScopeFactory;
//     private readonly IHubContext<TestExamHub> _hubContext;
//
//     public TestExamStatusUpdater(IServiceScopeFactory serviceScopeFactory, IHubContext<TestExamHub> hubContext)
//     {
//         _serviceScopeFactory = serviceScopeFactory;
//         _hubContext = hubContext;
//     }
//
//
//  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
// {
//     while (!stoppingToken.IsCancellationRequested)
//     {
//         bool hasChanges = false;
//
//         using (var scope = _serviceScopeFactory.CreateScope())
//         {
//             var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//             var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7));
//
//             // Lấy danh sách trạng thái từ DB
//             var statusList = await dbContext.ExamScheduleStatusEnumerable.ToListAsync();
//
//             // Tìm ID theo từ khóa
//             int notStartedId = statusList.FirstOrDefault(s => s.Names == "Chưa bắt đầu")?.Id ?? 0;
//             int inProgressId = statusList.FirstOrDefault(s => s.Names == "Đang thực hiện")?.Id ?? 0;
//             int completedId = statusList.FirstOrDefault(s => s.Names == "Đã kết thúc")?.Id ?? 0;
//
//             var exams = await dbContext.TestExams.Where(te => te.IsExam == false && te.IsDelete == false).ToListAsync();
//
//             foreach (var exam in exams)
//             {
//                 int newStatus = exam.ScheduleStatusId ?? 0;
//
//                 if (now < exam.StartDate)
//                     newStatus = notStartedId;  // ID của "Chưa bắt đầu"
//                 else if (now >= exam.StartDate && now <= exam.EndDate)
//                     newStatus = inProgressId;  // ID của "Đang thực hiện"
//                 else
//                     newStatus = completedId;  // ID của "Đã kết thúc"
//
//                 if (exam.ScheduleStatusId != newStatus) // Nếu trạng thái thay đổi
//                 {
//                     exam.ScheduleStatusId = newStatus;
//                     await _hubContext.Clients.All.SendAsync("ReceiveStatusUpdate", exam.Id, newStatus);
//                     hasChanges = true; // Đánh dấu là có thay đổi
//                 }
//             }
//
//             if (hasChanges)
//             {
//                 await dbContext.SaveChangesAsync(); // Lưu các thay đổi
//                 await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Delay 1 phút sau khi có thay đổi
//             }
//             else
//             {
//                 // Không có thay đổi, không cần delay nhiều, có thể giảm thời gian delay hoặc không delay.
//                 await Task.Delay(TimeSpan.FromDays(1), stoppingToken); // Delay lâu hơn nếu không có thay đổi
//             }
//         }
//     }
// }
//
//
//
// }