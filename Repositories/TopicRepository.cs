using AutoMapper;
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
        private readonly IMapper _mapper;

        public TopicRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TopicResponse>> GetAllTopic()
        {
            // Lấy danh sách topic gốc (không phải comment)
            var topics = await _context.Topics
                .AsNoTracking()
                .Where(t => t.IsDelete == false && t.TopicId == null)
                .OrderByDescending(t => t.CreateAt) // Sắp xếp theo thời gian tạo, mới nhất trước
                .ToListAsync();

            // Map sang TopicResponse
            var topicResponses = _mapper.Map<List<TopicResponse>>(topics);

            // Tính Views và Replies cho từng topic
            foreach (var topic in topicResponses)
            {
                topic.Views = await _context.QuestionAnswerTopicViews
                    .CountAsync(qatv => qatv.TopicId == topic.Id && (qatv.IsDelete == false || qatv.IsDelete == null));
                topic.Replies = await _context.Topics
                    .CountAsync(t => t.TopicId == topic.Id && t.IsDelete == false);
                topic.Comments = new List<TopicResponse>(); // Khởi tạo Comments rỗng
            }

            return topicResponses;
        }

        public async Task<TopicResponse?> GetTopicById(int id)
        {
            var topic = await _context.Topics
                .Include(t => t.User)
                .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (topic == null)
            {
                return null;
            }

            // Map sang TopicResponse
            var topicResponse = _mapper.Map<TopicResponse>(topic);

            // Tính Views và Replies cho topic
            topicResponse.Views = await _context.QuestionAnswerTopicViews
                .CountAsync(qatv => qatv.TopicId == id && (qatv.IsDelete == false || qatv.IsDelete == null));
            topicResponse.Replies = await _context.Topics
                .CountAsync(t => t.TopicId == id && t.IsDelete == false);

            // Lấy danh sách comment nếu đây là topic gốc
            if (topic.TopicId == null)
            {
                var commentEntities = await _context.Topics
                    .AsNoTracking()
                    .Where(t => t.TopicId == id && t.IsDelete == false) // Chỉ lấy comment chưa bị xóa
                    .OrderByDescending(t => t.CreateAt)
                    .ToListAsync();

                var comments = _mapper.Map<List<TopicResponse>>(commentEntities);

                foreach (var comment in comments)
                {
                    comment.Views = await _context.QuestionAnswerTopicViews
                        .CountAsync(qatv =>
                            qatv.TopicId == comment.Id && (qatv.IsDelete == false || qatv.IsDelete == null));
                    comment.Replies = 0;
                    comment.Comments = new List<TopicResponse>();
                }

                topicResponse.Comments = comments;
            }
            else
            {
                topicResponse.Comments = new List<TopicResponse>();
            }

            return topicResponse;
        }

        public async Task<Topic> AddTopic(Topic topic)
        {
            try
            {
                // 1. Kiểm tra topic
                if (!topic.UserId.HasValue || topic.UserId <= 0)
                {
                    throw new ArgumentException("UserId là bắt buộc và phải lớn hơn 0!");
                }

                // 2. Lấy thông tin user dựa trên topic.UserId
                var user = await _context.Users.FindAsync(topic.UserId);
                if (user == null)
                {
                    throw new ArgumentException("Không tìm thấy user với Id đã cung cấp!");
                }

                // 3. Kiểm tra vai trò: Chỉ giáo viên (RoleId == 2) được tạo topic
                if (!topic.TopicId.HasValue) // Nếu là topic (không phải comment)
                {
                    if (user.RoleId != 2)
                    {
                        throw new Exception("Chỉ giáo viên mới được tạo topic!");
                    }
                }

                // 4. Kiểm tra TeachingAssignmentId
                if (!topic.TeachingAssignmentId.HasValue)
                {
                    throw new Exception("TeachingAssignmentId là bắt buộc!");
                }

                var teachingAssignment = await _context.TeachingAssignments.FindAsync(topic.TeachingAssignmentId);
                if (teachingAssignment == null)
                {
                    throw new Exception("Phân công giảng dạy không tồn tại!");
                }

                // 5. Kiểm tra xem giáo viên có thuộc phân công giảng dạy này không (chỉ áp dụng khi tạo topic)
                if (!topic.TopicId.HasValue && teachingAssignment.UserId != topic.UserId)
                {
                    throw new Exception("Giáo viên không thuộc phân công giảng dạy này!");
                }

                // 6. Nếu là comment (topic_id không null), kiểm tra topic cha
                if (topic.TopicId.HasValue)
                {
                    var parentTopic = await _context.Topics
                        .FirstOrDefaultAsync(t => t.Id == topic.TopicId && t.IsDelete == false);
                    if (parentTopic == null)
                    {
                        throw new Exception("Topic cha không tồn tại hoặc đã bị xóa!");
                    }

                    // Đảm bảo topic cha không phải là comment (topic cha phải có topic_id là null)
                    if (parentTopic.TopicId.HasValue)
                    {
                        throw new Exception("Topic cha không hợp lệ, không thể tạo comment cho một comment!");
                    }

                    // Đảm bảo TeachingAssignmentId của comment khớp với topic cha
                    if (parentTopic.TeachingAssignmentId != topic.TeachingAssignmentId)
                    {
                        throw new Exception("TeachingAssignmentId của comment phải khớp với topic cha!");
                    }
                }

                // 7. Gán các giá trị mặc định
                topic.CreateAt = TimeHelper.NowUsingTimeZone;
                topic.UpdateAt = TimeHelper.NowUsingTimeZone;
                topic.IsDelete = false;
                topic.UserCreate = topic.UserId;
                topic.UserUpdate = topic.UserId;

                // 8. Thêm bản ghi vào cơ sở dữ liệu
                _context.Topics.Add(topic);
                await _context.SaveChangesAsync();

                // 9. Id sẽ được sinh tự động bởi cơ sở dữ liệu sau khi lưu
                return topic;
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể thêm topic/comment: {ex.Message}", ex);
            }
        }

        public async Task<Topic?> UpdateTopict(Topic topic)
        {
            try
            {
                // 1. Kiểm tra Id của bản ghi cần cập nhật
                if (topic.Id <= 0)
                {
                    throw new ArgumentException("Id là bắt buộc và phải lớn hơn 0!");
                }

                // 2. Kiểm tra UserId
                if (!topic.UserId.HasValue || topic.UserId <= 0)
                {
                    throw new ArgumentException("UserId là bắt buộc và phải lớn hơn 0!");
                }

                // 3. Lấy thông tin user dựa trên topic.UserId
                var user = await _context.Users.FindAsync(topic.UserId);
                if (user == null)
                {
                    throw new ArgumentException("Không tìm thấy user với Id đã cung cấp!");
                }

                // 4. Tìm bản ghi hiện tại từ bảng Topics
                var existingTopic = await _context.Topics
                    .FirstOrDefaultAsync(t => t.Id == topic.Id && t.IsDelete == false);
                if (existingTopic == null)
                {
                    return null;
                }

                // 5. Kiểm tra quyền chỉnh sửa
                if (!topic.TopicId.HasValue) // Nếu là topic (không phải comment)
                {
                    // Chỉ giáo viên (RoleId == 2) được cập nhật topic
                    if (user.RoleId != 2)
                    {
                        throw new Exception("Chỉ giáo viên mới được cập nhật topic!");
                    }
                }
                else // Nếu là comment
                {
                    // Học sinh chỉ được cập nhật comment do chính họ tạo
                    if (user.RoleId != 2 && existingTopic.UserId != topic.UserId)
                    {
                        throw new Exception("Bạn chỉ có thể cập nhật comment do chính bạn tạo!");
                    }
                }

                // 6. Kiểm tra TeachingAssignmentId
                if (!topic.TeachingAssignmentId.HasValue)
                {
                    throw new Exception("TeachingAssignmentId là bắt buộc!");
                }

                var teachingAssignment = await _context.TeachingAssignments.FindAsync(topic.TeachingAssignmentId);
                if (teachingAssignment == null)
                {
                    throw new Exception("Phân công giảng dạy không tồn tại!");
                }

                // 7. Kiểm tra xem giáo viên có thuộc phân công giảng dạy này không (chỉ áp dụng khi cập nhật topic)
                if (!topic.TopicId.HasValue && teachingAssignment.UserId != topic.UserId)
                {
                    throw new Exception("Giáo viên không thuộc phân công giảng dạy này!");
                }

                // 8. Nếu là comment (TopicId không null), kiểm tra topic cha
                if (topic.TopicId.HasValue)
                {
                    var parentTopic = await _context.Topics
                        .FirstOrDefaultAsync(t => t.Id == topic.TopicId && t.IsDelete == false);
                    if (parentTopic == null)
                    {
                        throw new Exception("Topic cha không tồn tại hoặc đã bị xóa!");
                    }

                    // Đảm bảo topic cha không phải là comment (topic cha phải có topic_id là null)
                    if (parentTopic.TopicId.HasValue)
                    {
                        throw new Exception("Topic cha không hợp lệ, không thể cập nhật comment cho một comment!");
                    }

                    // Đảm bảo TeachingAssignmentId của comment khớp với topic cha
                    if (parentTopic.TeachingAssignmentId != topic.TeachingAssignmentId)
                    {
                        throw new Exception("TeachingAssignmentId của comment phải khớp với topic cha!");
                    }
                }

                // 9. Cập nhật các trường cần thiết
                existingTopic.Title = topic.Title ?? existingTopic.Title;
                existingTopic.Description = topic.Description ?? existingTopic.Description;
                existingTopic.FileName = topic.FileName ?? existingTopic.FileName;
                existingTopic.CloseAt = topic.CloseAt ?? existingTopic.CloseAt;
                existingTopic.TeachingAssignmentId = topic.TeachingAssignmentId ?? existingTopic.TeachingAssignmentId;
                existingTopic.TopicId = topic.TopicId; // Cho phép cập nhật TopicId
                existingTopic.UpdateAt = TimeHelper.NowUsingTimeZone;
                existingTopic.UserUpdate = topic.UserId;

                // 10. Cập nhật bản ghi vào cơ sở dữ liệu
                _context.Topics.Update(existingTopic);
                await _context.SaveChangesAsync();

                return existingTopic;
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể cập nhật topic/comment: {ex.Message}", ex);
            }
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

        public async Task<IEnumerable<TopicResponse>> SearchTopic(string? keyword)
        {
            // Nếu không có keyword, gọi GetAllTopic() để lấy tất cả topic gốc
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return await GetAllTopic();
            }

            keyword = keyword.Trim().ToLower();

            // Lấy danh sách topic khớp với từ khóa (giữ nguyên logic ban đầu)
            var topics = await _context.Topics
                .Where(x => x.IsDelete == false && x.TopicId == null && // Chỉ lấy topic gốc
                            x.Description != null && x.Title != null &&
                            (x.Title.ToLower().Contains(keyword) || x.Description.ToLower().Contains(keyword)))
                .OrderByDescending(t => t.CreateAt) // Sắp xếp theo thời gian tạo, mới nhất trước
                .ToListAsync();

            // Map sang TopicResponse
            var topicResponses = _mapper.Map<List<TopicResponse>>(topics);

            // Tính Views và Replies cho từng topic
            foreach (var topic in topicResponses)
            {
                topic.Views = await _context.QuestionAnswerTopicViews
                    .CountAsync(qatv => qatv.TopicId == topic.Id && (qatv.IsDelete == false || qatv.IsDelete == null));
                topic.Replies = await _context.Topics
                    .CountAsync(t => t.TopicId == topic.Id && t.IsDelete == false);
                topic.Comments = new List<TopicResponse>(); // Khởi tạo Comments rỗng
            }

            return topicResponses;
        }

        public async Task<bool> IsUserInClassAsync(int userId, int classId)
        {
            var isUserInClass = await _context.TeachingAssignments
                .Where(ta =>
                    ta.ClassId == classId && ta.UserId == userId && ta.IsDelete == false && ta.User.RoleId == 2)
                .Select(ta => true)
                .Union(
                    _context.ClassStudents
                        .Where(cs =>
                            cs.UserId == userId && cs.ClassId == classId && cs.IsDelete == false && cs.User.RoleId == 3)
                        .Select(cs => true)
                )
                .AnyAsync();
            return isUserInClass;
        }
    }
}