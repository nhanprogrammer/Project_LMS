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


        public async Task<Reward?> GetByIdAsync(int id)
        {
            return await _context.Rewards
                .Include(r=>r.User)
                .Include(r=>r.Semester)
                .Where(r=>r.Id == id && r.IsDelete == false && r.User.IsDelete == false)
                .FirstOrDefaultAsync();
        }

        public async Task<Reward> AddAsync(Reward reward)
        {
            await _context.Rewards.AddAsync(reward);
            await _context.SaveChangesAsync();
            return reward;
        }

        public async Task<Reward> UpdateAsync(Reward reward)
        {
            _context.Rewards.Update(reward);
            await _context.SaveChangesAsync();
            return reward;
        }

        public async Task DeleteAsync(Reward reward)
        {
            _context.Rewards.Remove(reward);
            await _context.SaveChangesAsync();
        }
    }
}