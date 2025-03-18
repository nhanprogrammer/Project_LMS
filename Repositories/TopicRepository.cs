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
            // Nếu pageNumber <= 0 hoặc pageSize <= 0 => lấy tất cả topic chưa xóa
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allItems = await _context.Topics
                    .Where(c => c.IsDelete == false)
                    .ToListAsync();

                return new PaginatedResponse<Topic>
                {
                    Items = allItems,
                    PageNumber = 1,
                    PageSize = allItems.Count,
                    TotalItems = allItems.Count,
                    TotalPages = 1,
                    HasPreviousPage = false,
                    HasNextPage = false
                };
            }

            // Nếu có phân trang => cũng phải lọc IsDelete == false
            var query = _context.Topics
                .Where(c => c.IsDelete == false);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
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
            // Lấy thông tin user dựa trên topic.UserId
            var user = await _context.Users.FindAsync(topic.UserId);
            if (user == null)
            {
                throw new Exception("User không tồn tại!");
            }

            // Kiểm tra xem user có phải là giáo viên
            if (user.RoleId != 2)
            {
                throw new Exception("Chỉ giáo viên mới được tạo topic.");
            }

            // Lấy thông tin phân công giảng dạy dựa trên topic.TeachingAssignmentId
            var teachingAssignment = await _context.TeachingAssignments.FindAsync(topic.TeachingAssignmentId);
            if (teachingAssignment == null)
            {
                throw new Exception("Phân công giảng dạy không tồn tại!");
            }

            // Kiểm tra xem giáo viên có thuộc phân công giảng dạy của lớp/bài học đó không
            if (teachingAssignment.UserId != topic.UserId)
            {
                throw new Exception("Giáo viên không thuộc phân công giảng dạy này!");
            }

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

        public async Task<IEnumerable<Topic?>> SearchTopic(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return await _context.Topics.ToListAsync();
            }

            keyword = keyword.Trim().ToLower();

            // Lấy danh sách topic khớp với từ khoá
            var topics = await _context.Topics
                .Where(x => x.Description != null && x.Title != null &&
                            (x.Title.ToLower().Contains(keyword) || x.Description.ToLower().Contains(keyword)))
                .ToListAsync();

            return topics;
        }

    }
}