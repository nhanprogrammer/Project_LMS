using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Models;
using Project_LMS.Interfaces.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_LMS.Repositories
{
    public class RewardRepository : IRewardRepository
    {
        private readonly ApplicationDbContext _context;

        public RewardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reward>> GetAllAsync()
        {
            return await _context.Rewards.ToListAsync();
        }

        public async Task<Reward?> GetByIdAsync(int id)
        {
            return await _context.Rewards.FindAsync(id);
        }

        public async Task AddAsync(Reward reward)
        {
            await _context.Rewards.AddAsync(reward);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Reward reward)
        {
            _context.Rewards.Update(reward);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var reward = await _context.Rewards.FindAsync(id);
            if (reward != null)
            {
                _context.Rewards.Remove(reward);
                await _context.SaveChangesAsync();
            }
        }
    }
}