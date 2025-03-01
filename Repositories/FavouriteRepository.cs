using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories;

public class FavouriteRepository : IFavouriteRepository
{
    private readonly ApplicationDbContext _context;

    public FavouriteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Favourite> GetByIdAsync(int id)
    {
        return await _context.Favourites.FindAsync(id);
    }

    public async Task<IEnumerable<Favourite>> GetAllAsync()
    {
        return await _context.Favourites.ToListAsync();
    }

    public async Task AddAsync(Favourite entity)
    {
        await _context.Favourites.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Favourite entity)
    {
        _context.Favourites.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var favourite = await GetByIdAsync(id);
        if (favourite != null)
        {
            _context.Favourites.Remove(favourite);
            await _context.SaveChangesAsync();
        }
    }
}