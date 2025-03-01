using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public class NotificationsReceiverRepository : INotificationsReceiverRepository
    {
        private readonly ApplicationDbContext _context;
        
        public NotificationsReceiverRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<NotificationsReceiver>> GetAllAsync()
        {
            return await _context.NotificationsReceivers
                .Where(nr => nr.IsDelete == false || nr.IsDelete == null)
                .Include(nr => nr.Notification)
                .Include(nr => nr.Receiver)
                .ToListAsync();
        }

        public async Task<NotificationsReceiver?> GetByIdAsync(int id)
        {
            return await _context.NotificationsReceivers
                .Include(nr => nr.Notification)
                .Include(nr => nr.Receiver)
                .FirstOrDefaultAsync(nr => nr.Id == id && (nr.IsDelete == false || nr.IsDelete == null));
        }

        public async Task AddAsync(NotificationsReceiver notificationsReceiver)
        {
            notificationsReceiver.CreateAt = DateTime.UtcNow;
            _context.NotificationsReceivers.Add(notificationsReceiver);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(NotificationsReceiver notificationsReceiver)
        {
            notificationsReceiver.UpdateAt = DateTime.UtcNow;
            _context.NotificationsReceivers.Update(notificationsReceiver);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var notificationsReceiver = await _context.NotificationsReceivers.FindAsync(id);
            if (notificationsReceiver != null)
            {
                notificationsReceiver.IsDelete = true;
                notificationsReceiver.UpdateAt = DateTime.UtcNow;
                _context.NotificationsReceivers.Update(notificationsReceiver);
                await _context.SaveChangesAsync();
            }
        }
    }
}
