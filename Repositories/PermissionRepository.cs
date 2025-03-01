using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ApplicationDbContext _context;
        
        public PermissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            return await _context.Permissions
                .Where(p => p.IsDelete == false || p.IsDelete == null)
                .Include(p => p.RolePermissions)
                .ToListAsync();
        }

        public async Task<Permission?> GetByIdAsync(int id)
        {
            return await _context.Permissions
                .Include(p => p.RolePermissions)
                .FirstOrDefaultAsync(p => p.Id == id && (p.IsDelete == false || p.IsDelete == null));
        }

        public async Task AddAsync(Permission permission)
        {
            permission.CreateAt = DateTime.UtcNow;
            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Permission permission)
        {
            permission.UpdateAt = DateTime.UtcNow;
            _context.Permissions.Update(permission);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission != null)
            {
                permission.IsDelete = true;
                permission.UpdateAt = DateTime.UtcNow;
                _context.Permissions.Update(permission);
                await _context.SaveChangesAsync();
            }
        }
    }
}