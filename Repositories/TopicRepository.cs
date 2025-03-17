using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;
using Project_LMS.Data;
using Project_LMS.DTOs.Response;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Helpers;

namespace Project_LMS.Repositories
{
    public class TopicRepository : ITopicRepository
    {
        private readonly ApplicationDbContext _context;

        public TopicRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResponse<Topic>> GetAllTopic(int pageNumber, int pageSize)
        {
            // Nếu pageNumber <= 0 hoặc pageSize <= 0, ta coi như "không phân trang" => lấy tất cả
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allItems = await _context.Topics.ToListAsync();
                return new PaginatedResponse<Topic>
                {
                    Items = allItems,
                    PageNumber = 1, // Hoặc 0
                    PageSize = allItems.Count, // Để phản ánh "lấy hết"
                    TotalItems = allItems.Count,
                    TotalPages = 1,
                    HasPreviousPage = false,
                    HasNextPage = false
                };
            }

            // Ngược lại, nếu pageNumber > 0 và pageSize > 0, ta phân trang bình thường
            var totalItems = await _context.Topics.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await _context.Topics
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Topic>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };
        }


        public async Task<Topic?> GetTopicById(int id)
        {
            return await _context.Topics.FindAsync(id);
        }

        public async Task<Topic> AddTopic(Topic topic)
        {
            topic.CreateAt = TimeHelper.NowUsingTimeZone;
            topic.IsDelete = false;

            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();
            return topic;
        }

        public async Task<Topic?> UpdateTopict(Topic topic)
        {
            var existingTopic = await _context.Topics.FindAsync(topic.Id);
            if (existingTopic == null)
            {
                return null;
            }

            existingTopic.Title = topic.Title;
            existingTopic.Description = topic.Description;
            existingTopic.FileName = topic.FileName;
            existingTopic.UpdateAt = DateTime.UtcNow;
            // Update other properties as needed

            _context.Topics.Update(existingTopic);
            await _context.SaveChangesAsync();
            return existingTopic;
        }

        public async Task<bool> DeleteTopic(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return false;
            }

            topic.IsDelete = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Topic?> SearchTopic(string? keyword)
        {
            // Nếu không có từ khoá thì có thể return null hoặc throw exception tuỳ logic.
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return null!;
            }

            // Xử lý chuỗi keyword
            keyword = keyword.Trim().ToLower();

            // Tìm kiếm trong Title hoặc Description 
            var topic = await _context.Topics
                .Where(x => x.Description != null && x.Title != null &&
                            (x.Title.ToLower().Contains(keyword) || x.Description.ToLower().Contains(keyword)))
                .FirstOrDefaultAsync();

            return topic;
        }
    }
}