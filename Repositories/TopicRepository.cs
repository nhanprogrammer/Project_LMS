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

        public async Task<IEnumerable<TopicResponse>> GetAllTopic(int userId, int teachingAssignmentId)
        {
            // Kiểm tra user
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("Người dùng không tồn tại!");
            }

            // Kiểm tra teachingAssignment
            var teachingAssignment = await _context.TeachingAssignments
                .FirstOrDefaultAsync(ta => ta.Id == teachingAssignmentId && ta.IsDelete == false);
            if (teachingAssignment == null)
            {
                throw new Exception("Phân công giảng dạy không tồn tại!");
            }

            if (teachingAssignment.ClassId == null)
            {
                throw new Exception("ClassId của phân công giảng dạy không hợp lệ!");
            }

            // Kiểm tra quyền truy cập
            bool hasAccess = false;
            if (user.RoleId == 1) // Admin
            {
                hasAccess = true;
            }
            else if (user.RoleId == 2) // Teacher
            {
                hasAccess = teachingAssignment.UserId == userId;
            }
            else // Student hoặc các vai trò khác
            {
                hasAccess = await _context.ClassStudents
                    .AnyAsync(cs => cs.ClassId == teachingAssignment.ClassId
                                    && cs.UserId == userId
                                    && (cs.IsDelete == false || cs.IsDelete == null));
            }

            if (!hasAccess)
            {
                var className = (await _context.Classes.FirstOrDefaultAsync(c => c.Id == teachingAssignment.ClassId))
                    ?.Name;
                throw new Exception($"Bạn không thuộc lớp học {className} để xem dữ liệu này!");
            }

            // Lấy danh sách topic gốc (TopicId == null) và join với Users và Roles
            var topicsQuery = await _context.Topics
                .AsNoTracking()
                .Where(t => t.IsDelete == false && t.TopicId == null && t.TeachingAssignmentId == teachingAssignmentId)
                .Join(
                    _context.Users,
                    t => t.UserId,
                    u => u.Id,
                    (t, u) => new { Topic = t, UserAvatar = u.Image, UserFullName = u.FullName, UserRoleId = u.RoleId })
                .Join(
                    _context.Roles,
                    tu => tu.UserRoleId,
                    r => r.Id,
                    (tu, r) => new { tu.Topic, tu.UserAvatar, tu.UserFullName, RoleName = r.Name })
                .OrderByDescending(x => x.Topic.CreateAt)
                .ToListAsync();

            // Lấy tất cả bình luận (sub-topics) cho các topic gốc
            var topicIds = topicsQuery.Select(x => x.Topic.Id).ToList();
            var commentsQuery = await _context.Topics
                .AsNoTracking()
                .Where(t => topicIds.Contains(t.TopicId.Value) && t.IsDelete == false)
                .Join(
                    _context.Users,
                    t => t.UserId,
                    u => u.Id,
                    (t, u) => new
                        { Comment = t, UserAvatar = u.Image, UserFullName = u.FullName, UserRoleId = u.RoleId })
                .Join(
                    _context.Roles,
                    cu => cu.UserRoleId,
                    r => r.Id,
                    (cu, r) => new { cu.Comment, cu.UserAvatar, cu.UserFullName, RoleName = r.Name })
                .ToListAsync();

            // Lấy tất cả ID của topic và bình luận để tính Views
            var commentIds = commentsQuery.Select(c => c.Comment.Id).ToList();
            var allTopicIds = topicIds.Concat(commentIds).ToList();

            // Tính số lượt xem cho từng topic và bình luận
            var viewsQuery = await _context.QuestionAnswerTopicViews
                .Where(qatv => qatv.TopicId.HasValue && allTopicIds.Contains(qatv.TopicId.Value)
                                                     && (qatv.IsDelete == false || qatv.IsDelete == null))
                .GroupBy(qatv => qatv.TopicId)
                .Select(g => new { TopicId = g.Key, ViewCount = g.Count() })
                .ToListAsync();

            // Tạo dictionary để tra cứu số lượt xem theo TopicId
            var viewsDict = viewsQuery.ToDictionary(v => v.TopicId.Value, v => v.ViewCount);

            // Map sang TopicResponse
            var topicResponses = topicsQuery.Select(x =>
            {
                var topicResponse = _mapper.Map<TopicResponse>(x.Topic);
                topicResponse.Avatar = x.UserAvatar;
                topicResponse.FullName = x.UserFullName;
                topicResponse.RoleName = x.RoleName;

                // Gán số lượt xem cho topic
                topicResponse.Views = viewsDict.ContainsKey(x.Topic.Id) ? viewsDict[x.Topic.Id] : 0;

                // Tính số câu trả lời (Replies) cho topic
                topicResponse.Replies = commentsQuery.Count(c => c.Comment.TopicId == x.Topic.Id);

                // Lấy danh sách bình luận cho topic này
                topicResponse.Comments = commentsQuery
                    .Where(c => c.Comment.TopicId == x.Topic.Id)
                    .Select(c =>
                    {
                        var commentResponse = _mapper.Map<TopicResponse>(c.Comment);
                        commentResponse.Avatar = c.UserAvatar;
                        commentResponse.FullName = c.UserFullName;
                        commentResponse.RoleName = c.RoleName;
                        commentResponse.Views = viewsDict.ContainsKey(c.Comment.Id) ? viewsDict[c.Comment.Id] : 0;
                        commentResponse.Replies = 0;
                        commentResponse.Comments = new List<TopicResponse>();
                        return commentResponse;
                    })
                    .ToList();

                return topicResponse;
            }).ToList();

            return topicResponses;
        }
        
        public async Task<TopicResponse?> GetTopicById(int userId, int teachingAssignmentId, int id)
        {
            // Kiểm tra user
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("Người dùng không tồn tại!");
            }

            // Kiểm tra teachingAssignment
            var teachingAssignment = await _context.TeachingAssignments
                .FirstOrDefaultAsync(ta => ta.Id == teachingAssignmentId && ta.IsDelete == false);
            if (teachingAssignment == null)
            {
                throw new Exception("Phân công giảng dạy không tồn tại!");
            }

            if (teachingAssignment.ClassId == null)
            {
                throw new Exception("ClassId của phân công giảng dạy không hợp lệ!");
            }

            // Kiểm tra quyền truy cập
            bool hasAccess = false;
            if (user.RoleId == 1) // Admin
            {
                hasAccess = true;
            }
            else if (user.RoleId == 2) // Teacher
            {
                hasAccess = teachingAssignment.UserId == userId;
            }
            else // Student hoặc các vai trò khác
            {
                hasAccess = await _context.ClassStudents
                    .AnyAsync(cs => cs.ClassId == teachingAssignment.ClassId
                                    && cs.UserId == userId
                                    && (cs.IsDelete == false || cs.IsDelete == null));
            }

            if (!hasAccess)
            {
                var className = (await _context.Classes.FirstOrDefaultAsync(c => c.Id == teachingAssignment.ClassId))
                    ?.Name;
                throw new Exception($"Bạn không thuộc lớp học {className} để xem dữ liệu này!");
            }

            // Lấy topic và join với Users và Roles
            var topicQuery = await _context.Topics
                .AsNoTracking()
                .Where(t => t.Id == id && t.IsDelete == false && t.TeachingAssignmentId == teachingAssignmentId)
                .Join(
                    _context.Users,
                    t => t.UserId,
                    u => u.Id,
                    (t, u) => new { Topic = t, UserAvatar = u.Image, UserFullName = u.FullName, UserRoleId = u.RoleId })
                .Join(
                    _context.Roles,
                    tu => tu.UserRoleId,
                    r => r.Id,
                    (tu, r) => new { tu.Topic, tu.UserAvatar, tu.UserFullName, RoleName = r.Name })
                .FirstOrDefaultAsync();

            if (topicQuery == null)
            {
                return null;
            }

            // Map sang TopicResponse
            var topicResponse = _mapper.Map<TopicResponse>(topicQuery.Topic);
            topicResponse.Avatar = topicQuery.UserAvatar;
            topicResponse.FullName = topicQuery.UserFullName;
            topicResponse.RoleName = topicQuery.RoleName;

            // Tính Views và Replies cho topic
            topicResponse.Views = await _context.QuestionAnswerTopicViews
                .CountAsync(qatv => qatv.TopicId == id && (qatv.IsDelete == false || qatv.IsDelete == null));
            topicResponse.Replies = await _context.Topics
                .CountAsync(t => t.TopicId == id && t.IsDelete == false);

            // Lấy danh sách comment nếu đây là topic gốc
            if (topicQuery.Topic.TopicId == null)
            {
                var commentsQuery = await _context.Topics
                    .AsNoTracking()
                    .Where(t => t.TopicId == id && t.IsDelete == false)
                    .Join(
                        _context.Users,
                        t => t.UserId,
                        u => u.Id,
                        (t, u) => new
                            { Comment = t, UserAvatar = u.Image, UserFullName = u.FullName, UserRoleId = u.RoleId })
                    .Join(
                        _context.Roles,
                        cu => cu.UserRoleId,
                        r => r.Id,
                        (cu, r) => new { cu.Comment, cu.UserAvatar, cu.UserFullName, RoleName = r.Name })
                    .OrderByDescending(x => x.Comment.CreateAt)
                    .ToListAsync();

                var commentIds = commentsQuery.Select(c => c.Comment.Id).ToList();
                var commentViewsQuery = await _context.QuestionAnswerTopicViews
                    .Where(qatv => qatv.TopicId.HasValue && commentIds.Contains(qatv.TopicId.Value)
                                                         && (qatv.IsDelete == false || qatv.IsDelete == null))
                    .GroupBy(qatv => qatv.TopicId)
                    .Select(g => new { TopicId = g.Key, ViewCount = g.Count() })
                    .ToListAsync();

                var commentViewsDict = commentViewsQuery.ToDictionary(v => v.TopicId.Value, v => v.ViewCount);

                topicResponse.Comments = commentsQuery.Select(c =>
                {
                    var commentResponse = _mapper.Map<TopicResponse>(c.Comment);
                    commentResponse.Avatar = c.UserAvatar;
                    commentResponse.FullName = c.UserFullName;
                    commentResponse.RoleName = c.RoleName;
                    commentResponse.Views =
                        commentViewsDict.ContainsKey(c.Comment.Id) ? commentViewsDict[c.Comment.Id] : 0;
                    commentResponse.Replies = 0;
                    commentResponse.Comments = new List<TopicResponse>();
                    return commentResponse;
                }).ToList();
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

                // 3. Kiểm tra TeachingAssignmentId
                if (!topic.TeachingAssignmentId.HasValue)
                {
                    throw new Exception("TeachingAssignmentId là bắt buộc!");
                }

                var teachingAssignment = await _context.TeachingAssignments
                    .FirstOrDefaultAsync(ta => ta.Id == topic.TeachingAssignmentId && ta.IsDelete == false);
                if (teachingAssignment == null)
                {
                    throw new Exception("Phân công giảng dạy không tồn tại!");
                }

                if (teachingAssignment.ClassId == null)
                {
                    throw new Exception("ClassId của phân công giảng dạy không hợp lệ!");
                }

                // 4. Kiểm tra quyền truy cập
                bool hasAccess = false;
                if (user.RoleId == 1) // Admin
                {
                    hasAccess = true;
                }
                else if (user.RoleId == 2) // Teacher
                {
                    hasAccess = teachingAssignment.UserId == topic.UserId;
                }
                else // Student hoặc các vai trò khác
                {
                    hasAccess = await _context.ClassStudents
                        .AnyAsync(cs => cs.ClassId == teachingAssignment.ClassId
                                        && cs.UserId == topic.UserId
                                        && (cs.IsDelete == false || cs.IsDelete == null));
                }

                if (!hasAccess)
                {
                    var className =
                        (await _context.Classes.FirstOrDefaultAsync(c => c.Id == teachingAssignment.ClassId))?.Name;
                    throw new Exception($"Bạn không thuộc lớp học {className} để thực hiện hành động này!");
                }

                // 5. Kiểm tra vai trò: Chỉ giáo viên (RoleId == 2) được tạo topic
                if (!topic.TopicId.HasValue) // Nếu là topic (không phải comment)
                {
                    if (user.RoleId != 2)
                    {
                        throw new Exception("Chỉ giáo viên mới được tạo topic!");
                    }
                }

                // 6. Kiểm tra xem giáo viên có thuộc phân công giảng dạy này không (chỉ áp dụng khi tạo topic)
                if (!topic.TopicId.HasValue && teachingAssignment.UserId != topic.UserId)
                {
                    throw new Exception("Giáo viên không thuộc phân công giảng dạy này!");
                }

                // 7. Nếu là comment (topic_id không null), kiểm tra topic cha
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

                // 8. Gán các giá trị mặc định
                topic.CreateAt = TimeHelper.NowUsingTimeZone;
                topic.UpdateAt = TimeHelper.NowUsingTimeZone;
                topic.IsDelete = false;
                topic.UserCreate = topic.UserId;
                topic.UserUpdate = topic.UserId;

                // 9. Thêm bản ghi vào cơ sở dữ liệu
                _context.Topics.Add(topic);
                await _context.SaveChangesAsync();

                return topic;
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể thêm topic/comment: {ex.Message}", ex);
            }
        }

        public async Task<Topic?> UpdateTopic(Topic topic)
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

                // 5. Kiểm tra TeachingAssignmentId
                if (!topic.TeachingAssignmentId.HasValue)
                {
                    throw new Exception("TeachingAssignmentId là bắt buộc!");
                }

                var teachingAssignment = await _context.TeachingAssignments
                    .FirstOrDefaultAsync(ta => ta.Id == topic.TeachingAssignmentId && ta.IsDelete == false);
                if (teachingAssignment == null)
                {
                    throw new Exception("Phân công giảng dạy không tồn tại!");
                }

                if (teachingAssignment.ClassId == null)
                {
                    throw new Exception("ClassId của phân công giảng dạy không hợp lệ!");
                }

                // 6. Kiểm tra quyền truy cập
                bool hasAccess = false;
                if (user.RoleId == 1) // Admin
                {
                    hasAccess = true;
                }
                else if (user.RoleId == 2) // Teacher
                {
                    hasAccess = teachingAssignment.UserId == topic.UserId;
                }
                else // Student hoặc các vai trò khác
                {
                    hasAccess = await _context.ClassStudents
                        .AnyAsync(cs => cs.ClassId == teachingAssignment.ClassId
                                        && cs.UserId == topic.UserId
                                        && (cs.IsDelete == false || cs.IsDelete == null));
                }

                if (!hasAccess)
                {
                    var className =
                        (await _context.Classes.FirstOrDefaultAsync(c => c.Id == teachingAssignment.ClassId))?.Name;
                    throw new Exception($"Bạn không thuộc lớp học {className} để thực hiện hành động này!");
                }

                // 7. Kiểm tra quyền chỉnh sửa
                if (!existingTopic.TopicId.HasValue) // Nếu là topic (không phải comment)
                {
                    // Chỉ giáo viên (RoleId == 2) được cập nhật topic
                    if (user.RoleId != 2)
                    {
                        throw new Exception("Chỉ giáo viên mới được cập nhật topic!");
                    }

                    // Giáo viên phải thuộc phân công giảng dạy này
                    if (teachingAssignment.UserId != topic.UserId)
                    {
                        throw new Exception("Giáo viên không thuộc phân công giảng dạy này!");
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
                existingTopic.TopicId = topic.TopicId;
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

        public async Task<bool> DeleteTopic(int userId, int teachingAssignmentId, int id)
        {
            // Kiểm tra user
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("Người dùng không tồn tại!");
            }

            // Kiểm tra teachingAssignment
            var teachingAssignment = await _context.TeachingAssignments
                .FirstOrDefaultAsync(ta => ta.Id == teachingAssignmentId && ta.IsDelete == false);
            if (teachingAssignment == null)
            {
                throw new Exception("Phân công giảng dạy không tồn tại!");
            }

            if (teachingAssignment.ClassId == null)
            {
                throw new Exception("ClassId của phân công giảng dạy không hợp lệ!");
            }

            // Kiểm tra quyền truy cập
            bool hasAccess = false;
            if (user.RoleId == 1) // Admin
            {
                hasAccess = true;
            }
            else if (user.RoleId == 2) // Teacher
            {
                hasAccess = teachingAssignment.UserId == userId;
            }
            else // Student hoặc các vai trò khác
            {
                hasAccess = await _context.ClassStudents
                    .AnyAsync(cs => cs.ClassId == teachingAssignment.ClassId
                                    && cs.UserId == userId
                                    && (cs.IsDelete == false || cs.IsDelete == null));
            }

            if (!hasAccess)
            {
                var className = (await _context.Classes.FirstOrDefaultAsync(c => c.Id == teachingAssignment.ClassId))
                    ?.Name;
                throw new Exception($"Bạn không thuộc lớp học {className} để thực hiện hành động này!");
            }

            // Tìm topic
            var topic = await _context.Topics
                .FirstOrDefaultAsync(t =>
                    t.Id == id && t.IsDelete == false && t.TeachingAssignmentId == teachingAssignmentId);
            if (topic == null)
            {
                return false;
            }

            // Kiểm tra quyền xóa
            if (!topic.TopicId.HasValue) // Nếu là topic
            {
                // Chỉ giáo viên (RoleId == 2) được xóa topic
                if (user.RoleId != 2)
                {
                    throw new Exception("Chỉ giáo viên mới được xóa topic!");
                }

                // Giáo viên phải thuộc phân công giảng dạy này
                if (teachingAssignment.UserId != userId)
                {
                    throw new Exception("Giáo viên không thuộc phân công giảng dạy này!");
                }
            }
            else // Nếu là comment
            {
                // Học sinh chỉ được xóa comment do chính họ tạo
                if (user.RoleId != 2 && topic.UserId != userId)
                {
                    throw new Exception("Bạn chỉ có thể xóa comment do chính bạn tạo!");
                }
            }

            // Xóa mềm
            topic.IsDelete = true;
            topic.UpdateAt = TimeHelper.NowUsingTimeZone;
            topic.UserUpdate = userId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TopicResponse>> SearchTopic(int userId, int teachingAssignmentId, string? keyword)
        {
            // Kiểm tra user
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("Người dùng không tồn tại!");
            }

            // Kiểm tra teachingAssignment
            var teachingAssignment = await _context.TeachingAssignments
                .FirstOrDefaultAsync(ta => ta.Id == teachingAssignmentId && ta.IsDelete == false);
            if (teachingAssignment == null)
            {
                throw new Exception("Phân công giảng dạy không tồn tại!");
            }

            if (teachingAssignment.ClassId == null)
            {
                throw new Exception("ClassId của phân công giảng dạy không hợp lệ!");
            }

            // Kiểm tra quyền truy cập
            bool hasAccess = false;
            if (user.RoleId == 1) // Admin
            {
                hasAccess = true;
            }
            else if (user.RoleId == 2) // Teacher
            {
                hasAccess = teachingAssignment.UserId == userId;
            }
            else // Student hoặc các vai trò khác
            {
                hasAccess = await _context.ClassStudents
                    .AnyAsync(cs => cs.ClassId == teachingAssignment.ClassId
                                    && cs.UserId == userId
                                    && (cs.IsDelete == false || cs.IsDelete == null));
            }

            if (!hasAccess)
            {
                var className = (await _context.Classes.FirstOrDefaultAsync(c => c.Id == teachingAssignment.ClassId))
                    ?.Name;
                throw new Exception($"Bạn không thuộc lớp học {className} để xem dữ liệu này!");
            }

            // Nếu không có keyword, gọi GetAllTopic()
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return await GetAllTopic(userId, teachingAssignmentId);
            }

            keyword = keyword.Trim().ToLower();

            // Lấy danh sách topic khớp với từ khóa
            var topicsQuery = await _context.Topics
                .AsNoTracking()
                .Where(t => t.IsDelete == false && t.TopicId == null && t.TeachingAssignmentId == teachingAssignmentId
                            && t.Description != null && t.Title != null
                            && (t.Title.ToLower().Contains(keyword) || t.Description.ToLower().Contains(keyword)))
                .Join(
                    _context.Users,
                    t => t.UserId,
                    u => u.Id,
                    (t, u) => new { Topic = t, UserAvatar = u.Image, UserFullName = u.FullName, UserRoleId = u.RoleId })
                .Join(
                    _context.Roles,
                    tu => tu.UserRoleId,
                    r => r.Id,
                    (tu, r) => new { tu.Topic, tu.UserAvatar, tu.UserFullName, RoleName = r.Name })
                .OrderByDescending(x => x.Topic.CreateAt)
                .ToListAsync();

            // Lấy tất cả bình luận (sub-topics) cho các topic gốc
            var topicIds = topicsQuery.Select(x => x.Topic.Id).ToList();
            var commentsQuery = await _context.Topics
                .AsNoTracking()
                .Where(t => topicIds.Contains(t.TopicId.Value) && t.IsDelete == false)
                .Join(
                    _context.Users,
                    t => t.UserId,
                    u => u.Id,
                    (t, u) => new
                        { Comment = t, UserAvatar = u.Image, UserFullName = u.FullName, UserRoleId = u.RoleId })
                .Join(
                    _context.Roles,
                    cu => cu.UserRoleId,
                    r => r.Id,
                    (cu, r) => new { cu.Comment, cu.UserAvatar, cu.UserFullName, RoleName = r.Name })
                .ToListAsync();

            // Lấy tất cả ID của topic và bình luận để tính Views
            var commentIds = commentsQuery.Select(c => c.Comment.Id).ToList();
            var allTopicIds = topicIds.Concat(commentIds).ToList();

            // Tính số lượt xem cho từng topic và bình luận
            var viewsQuery = await _context.QuestionAnswerTopicViews
                .Where(qatv => qatv.TopicId.HasValue && allTopicIds.Contains(qatv.TopicId.Value)
                                                     && (qatv.IsDelete == false || qatv.IsDelete == null))
                .GroupBy(qatv => qatv.TopicId)
                .Select(g => new { TopicId = g.Key, ViewCount = g.Count() })
                .ToListAsync();

            // Tạo dictionary để tra cứu số lượt xem theo TopicId
            var viewsDict = viewsQuery.ToDictionary(v => v.TopicId.Value, v => v.ViewCount);

            // Map sang TopicResponse
            var topicResponses = topicsQuery.Select(x =>
            {
                var topicResponse = _mapper.Map<TopicResponse>(x.Topic);
                topicResponse.Avatar = x.UserAvatar;
                topicResponse.FullName = x.UserFullName;
                topicResponse.RoleName = x.RoleName;
                topicResponse.Views = viewsDict.ContainsKey(x.Topic.Id) ? viewsDict[x.Topic.Id] : 0;
                topicResponse.Replies = commentsQuery.Count(c => c.Comment.TopicId == x.Topic.Id);

                topicResponse.Comments = commentsQuery
                    .Where(c => c.Comment.TopicId == x.Topic.Id)
                    .Select(c =>
                    {
                        var commentResponse = _mapper.Map<TopicResponse>(c.Comment);
                        commentResponse.Avatar = c.UserAvatar;
                        commentResponse.FullName = c.UserFullName;
                        commentResponse.RoleName = c.RoleName;
                        commentResponse.Views = viewsDict.ContainsKey(c.Comment.Id) ? viewsDict[c.Comment.Id] : 0;
                        commentResponse.Replies = 0;
                        commentResponse.Comments = new List<TopicResponse>();
                        return commentResponse;
                    })
                    .ToList();

                return topicResponse;
            }).ToList();

            return topicResponses;
        }

        public async Task<bool> IsUserInClassAsync(int userId, int classId)
        {
            var isUserInClass = await _context.TeachingAssignments
                .Where(ta => ta.ClassId == classId && ta.UserId == userId && ta.IsDelete == false)
                .Select(ta => true)
                .Union(
                    _context.ClassStudents
                        .Where(cs =>
                            cs.UserId == userId && cs.ClassId == classId &&
                            (cs.IsDelete == false || cs.IsDelete == null))
                        .Select(cs => true)
                )
                .AnyAsync();
            return isUserInClass;
        }
    }
}