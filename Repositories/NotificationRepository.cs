using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;
        
        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await _context.Notifications
                .Where(n => n.IsDelete == false || n.IsDelete == null)
                .Include(n => n.Sender)
                .Include(n => n.Class)
                .Include(n => n.TestExam)
                .Include(n => n.NotificationsReceivers)
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(int id)
        {
            return await _context.Notifications
                .Include(n => n.Sender)
                .Include(n => n.Class)
                .Include(n => n.TestExam)
                .Include(n => n.NotificationsReceivers)
                .FirstOrDefaultAsync(n => n.Id == id && (n.IsDelete == false || n.IsDelete == null));
        }

        public async Task AddAsync(Notification notification)
        {
            notification.CreateAt = DateTime.UtcNow;
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Notification notification)
        {
            notification.UpdateAt = DateTime.UtcNow;
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                notification.IsDelete = true;
                notification.UpdateAt = DateTime.UtcNow;
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();
            }
        }
    }
}
