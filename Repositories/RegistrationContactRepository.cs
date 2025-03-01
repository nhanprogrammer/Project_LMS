using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Models;
using Project_LMS.Interfaces.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_LMS.Repositories
{
    public class RegistrationContactRepository : IRegistrationContactRepository
    {
        private readonly ApplicationDbContext _context;

        public RegistrationContactRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RegistrationContact>> GetAllAsync()
        {
            return await _context.RegistrationContacts.ToListAsync();
        }

        public async Task<RegistrationContact?> GetByIdAsync(int id)
        {
            return await _context.RegistrationContacts.FindAsync(id);
        }

        public async Task AddAsync(RegistrationContact registrationContact)
        {
            await _context.RegistrationContacts.AddAsync(registrationContact);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RegistrationContact registrationContact)
        {
            _context.RegistrationContacts.Update(registrationContact);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var registrationContact = await _context.RegistrationContacts.FindAsync(id);
            if (registrationContact != null)
            {
                _context.RegistrationContacts.Remove(registrationContact);
                await _context.SaveChangesAsync();
            }
        }
    }
}