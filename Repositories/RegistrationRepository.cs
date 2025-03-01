using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Models;
using Project_LMS.Interfaces.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_LMS.Repositories
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly ApplicationDbContext _context;

        public RegistrationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Registration>> GetAllAsync()
        {
            return await _context.Registrations.ToListAsync();
        }

        public async Task<Registration?> GetByIdAsync(int id)
        {
            return await _context.Registrations.FindAsync(id);
        }

        public async Task AddAsync(Registration registration)
        {
            await _context.Registrations.AddAsync(registration);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Registration registration)
        {
            _context.Registrations.Update(registration);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var registration = await _context.Registrations.FindAsync(id);
            if (registration != null)
            {
                _context.Registrations.Remove(registration);
                await _context.SaveChangesAsync();
            }
        }
    }
}