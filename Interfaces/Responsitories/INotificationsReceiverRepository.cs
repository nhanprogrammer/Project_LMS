using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface INotificationsReceiverRepository
{
    Task<IEnumerable<NotificationsReceiver>> GetAllAsync();
    Task<NotificationsReceiver?> GetByIdAsync(int id);
    Task AddAsync(NotificationsReceiver notificationsReceiver);
    Task UpdateAsync(NotificationsReceiver notificationsReceiver);
    Task DeleteAsync(int id);
}