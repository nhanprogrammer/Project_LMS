using Project_LMS.Models;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Data;

namespace Project_LMS.Repositories
{
    public class ExemptionRepository : IExemptionRepository
    {
        private readonly ApplicationDbContext _context;
        public ExemptionRepository(ApplicationDbContext context)
        {

            _context = context;
        }
        public async Task AddAsync(Exemption exemption)
        {
            await _context.Exemptions.AddAsync(exemption);
            await _context.SaveChangesAsync();
        }
    }
}
