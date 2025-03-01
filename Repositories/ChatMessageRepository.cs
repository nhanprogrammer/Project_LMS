using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories;

public class ChatMessageRepository : IChatMessageRepository
{
    private readonly ApplicationDbContext _context;

    public ChatMessageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ChatMessage> GetByIdAsync(int id)
    {
        return await _context.ChatMessages.FindAsync(id);
    }

    public async Task<IEnumerable<ChatMessage>> GetAllAsync()
    {
        return await _context.ChatMessages.ToListAsync();
    }

    public async Task AddAsync(ChatMessage entity)
    {
        await _context.ChatMessages.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ChatMessage entity)
    {
        _context.ChatMessages.Update(entity);
        await _context.SaveChangesAsync();

    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        _context.ChatMessages.Remove(entity);
        await _context.SaveChangesAsync();
    }
}