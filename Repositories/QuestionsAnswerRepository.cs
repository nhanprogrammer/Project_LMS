using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Helpers;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public class QuestionsAnswerRepository : IQuestionsAnswerRepository
    {
        private readonly ApplicationDbContext _context;

        public QuestionsAnswerRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<PaginatedResponse<QuestionAnswer>> GetAllAsync(PaginationRequest request)
        {
            if (request.PageNumber <= 0 || request.PageSize <= 0)
            {
                var allItems = await _context.QuestionAnswers.Where(c => c.IsDelete == false).ToListAsync();
                return new PaginatedResponse<QuestionAnswer>
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

            var query = _context.QuestionAnswers.Where(c => c.IsDelete == false);
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PaginatedResponse<QuestionAnswer>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = request.PageNumber > 1,
                HasNextPage = request.PageNumber < totalPages
            };
        }

        public async Task<QuestionAnswer?> GetByIdAsync(int id)
        {
            return await _context.QuestionAnswers.FindAsync(id);
        }

        public async Task<QuestionAnswer?> AddAsync(QuestionAnswer request, int teachingAssignmentId)
        {
            // Kiểm tra UserId
            if (!request.UserId.HasValue)
            {
                throw new ArgumentException("UserId là bắt buộc");
            }

            // Kiểm tra TeachingAssignmentId
            var teachingAssignment = await _context.TeachingAssignments
                .FirstOrDefaultAsync(ta => ta.Id == request.TeachingAssignmentId);
            if (teachingAssignment == null)
            {
                throw new ArgumentException("TeachingAssignmentId không hợp lệ");
            }

            // Đặt giá trị mặc định
            request.CreateAt = TimeHelper.NowUsingTimeZone;
            request.UpdateAt = TimeHelper.NowUsingTimeZone;
            request.IsDelete = false;
            request.TeachingAssignmentId = request.TeachingAssignmentId;

            // Đặt UserCreate và UserUpdate
            request.UserCreate = request.UserId;
            request.UserUpdate = request.UserId;

            // Thêm vào context
            _context.QuestionAnswers.Add(request);
            await _context.SaveChangesAsync();

            return request;
        }

        public async Task<QuestionAnswer?> UpdateAsync(QuestionAnswer updatedComment,
            int? newTeachingAssignmentId = null)
        {
            try
            {
                // 1. Kiểm tra Id của bản ghi cần cập nhật
                if (updatedComment.Id <= 0)
                {
                    throw new ArgumentException("Id là bắt buộc và phải lớn hơn 0");
                }

                // 2. Tìm bản ghi hiện tại từ bảng QuestionAnswers
                var existingComment = await _context.QuestionAnswers
                    .FirstOrDefaultAsync(q => q.Id == updatedComment.Id && q.IsDelete == false);

                if (existingComment == null)
                {
                    return null;
                }

                // 3. Kiểm tra UserUpdate
                if (!updatedComment.UserUpdate.HasValue)
                {
                    throw new ArgumentException("UserUpdate là bắt buộc");
                }

                // 4. Kiểm tra và cập nhật newTeachingAssignmentId (nếu có)
                if (newTeachingAssignmentId.HasValue)
                {
                    var teachingAssignment = await _context.TeachingAssignments
                        .FirstOrDefaultAsync(ta => ta.Id == newTeachingAssignmentId.Value);
                    if (teachingAssignment == null)
                    {
                        throw new ArgumentException("TeachingAssignmentId mới không tồn tại");
                    }

                    if (teachingAssignment.IsDelete == true)
                    {
                        throw new ArgumentException("Phân công giảng dạy mới đã bị xóa");
                    }

                    existingComment.TeachingAssignmentId = newTeachingAssignmentId.Value;
                }

                // 5. Cập nhật các trường cần thiết
                existingComment.Message = updatedComment.Message ?? existingComment.Message;
                existingComment.FileName = updatedComment.FileName ?? existingComment.FileName;
                existingComment.UpdateAt = TimeHelper.NowUsingTimeZone;
                existingComment.UserUpdate = updatedComment.UserUpdate;

                // 6. Cập nhật vào context
                _context.QuestionAnswers.Update(existingComment);
                await _context.SaveChangesAsync();

                return existingComment;
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể cập nhật câu hỏi/câu trả lời: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var questionAnswer = await _context.QuestionAnswers
                .FirstOrDefaultAsync(qa => qa.Id == id && qa.IsDelete == false);
            if (questionAnswer == null)
            {
                return false;
            }

            questionAnswer.IsDelete = true;
            questionAnswer.UpdateAt = DateTime.UtcNow;
            questionAnswer.UserUpdate = userId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<QuestionAnswer>> GetAllQuestionAnswerByTopicIdAsync(int topicId)
        {
            var questionAnswers = await (
                from qa in _context.QuestionAnswers
                join qatv in _context.QuestionAnswerTopicViews
                    on qa.Id equals qatv.QuestionsAnswerId
                where qatv.TopicId == topicId && qa.IsDelete == false
                select qa
            ).ToListAsync();

            return questionAnswers;
        }

        public async Task<bool> IsUserInClassAsync(int userId, int classId)
        {
            // Kiểm tra nếu là giáo viên được phân công dạy lớp này
            var teacherAccess = await _context.TeachingAssignments
                .Where(ta => ta.ClassId == classId
                          && ta.UserId == userId
                          && ta.IsDelete == false
                          && ta.User.RoleId == 2)
                .AnyAsync();

            if (teacherAccess)
                return true;

            // Lấy bản ghi mới nhất (ID cao nhất) cho học sinh trong lớp
            var latestClassStudent = await _context.ClassStudents
                .Where(cs => cs.UserId == userId && cs.ClassId == classId)
                .OrderByDescending(cs => cs.Id) // Sắp xếp theo ID giảm dần
                .FirstOrDefaultAsync();

            // Kiểm tra bản ghi mới nhất
            return latestClassStudent != null &&
                   latestClassStudent.IsDelete == false &&
                   latestClassStudent.IsActive == true &&
                   latestClassStudent.User.RoleId == 3;
        }

        public async Task<ClassMembersWithStatsResponse> GetClassMembersByTeachingAssignmentAsync(
            int teachingAssignmentId, string? searchTerm = null)
        {
            if (teachingAssignmentId <= 0)
            {
                throw new ArgumentException("TeachingAssignmentId phải lớn hơn 0.", nameof(teachingAssignmentId));
            }

            // Kiểm tra teachingAssignmentId tồn tại
            var teachingAssignment = await _context.TeachingAssignments
                .FirstOrDefaultAsync(ta => ta.Id == teachingAssignmentId && ta.IsDelete == false);
            if (teachingAssignment == null)
            {
                throw new ArgumentException(
                    $"Phân công giảng dạy với ID {teachingAssignmentId} không tồn tại hoặc đã bị xóa.");
            }

            // Lấy danh sách các câu hỏi
            var questionsQuery = _context.QuestionAnswers
                .AsNoTracking()
                .Where(qa =>
                    qa.TeachingAssignmentId == teachingAssignmentId && qa.IsDelete == false &&
                    qa.QuestionsAnswerId == null);

            // Nếu có searchTerm, lọc câu hỏi theo nội dung (Message)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                questionsQuery = questionsQuery.Where(qa => qa.Message.ToLower().Contains(searchTerm));
            }

            var questionsList = await questionsQuery.ToListAsync();

            // Lấy danh sách lượt xem
            var viewsQuery = await _context.QuestionAnswerTopicViews
                .AsNoTracking()
                .Where(qatv => qatv.IsDelete == false || qatv.IsDelete == null)
                .GroupBy(qatv => qatv.QuestionsAnswerId)
                .Select(g => new
                {
                    QuestionId = g.Key,
                    Views = g.Count()
                })
                .ToListAsync();

            // Join và GroupBy trên client side để tính lượt xem cho từng câu hỏi
            var questionsWithViews = questionsList
                .GroupJoin(
                    viewsQuery,
                    qa => qa.Id,
                    v => v.QuestionId,
                    (qa, vGroup) => new
                    {
                        QuestionId = qa.Id,
                        UserId = qa.UserId,
                        Views = vGroup.Select(v => v.Views).FirstOrDefault()
                    })
                .Select(q => new
                {
                    QuestionId = q.QuestionId,
                    UserId = q.UserId,
                    Views = q.Views != null ? q.Views : 0 // Nếu không có lượt xem, gán 0
                })
                .ToList();

            // Tính tổng số lượt xem (TotalViews)
            var totalViews = questionsWithViews.Sum(q => q.Views);

            // Lấy danh sách user đã tạo câu hỏi (sau khi lọc theo searchTerm)
            var userIdsWithQuestions = questionsWithViews
                .Select(q => q.UserId)
                .Distinct()
                .ToList();

            // Tính số lượt xem của từng user
            var userViewsQuery = questionsWithViews
                .GroupBy(q => q.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    Views = g.Sum(q => q.Views)
                });

            var userViews = userViewsQuery
                .ToDictionary(q => q.UserId, q => q.Views);

            // Lấy danh sách câu hỏi của từng user (bao gồm create_at, avatar và fullName)
            var userQuestionsQuery = await _context.QuestionAnswers
                .AsNoTracking()
                .Where(qa =>
                    qa.TeachingAssignmentId == teachingAssignmentId && qa.IsDelete == false &&
                    qa.QuestionsAnswerId == null)
                .Join(
                    _context.Users,
                    qa => qa.UserId,
                    u => u.Id,
                    (qa, u) => new
                    {
                        qa.UserId,
                        Question = new QuestionInfo
                        {
                            Id = qa.Id,
                            Message = qa.Message,
                            CreateAt = qa.CreateAt,
                            Avatar = u.Image,
                            FullName = u.FullName
                        }
                    })
                .GroupBy(x => x.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    Questions = g.Select(x => x.Question).ToList()
                })
                .ToListAsync();

            var userQuestions = userQuestionsQuery
                .ToDictionary(uq => uq.UserId, uq => uq.Questions);

            // Lấy danh sách câu trả lời của từng user (bao gồm create_at, avatar và fullName)
            var userAnswersQuery = await _context.QuestionAnswers
                .AsNoTracking()
                .Where(qa =>
                    qa.TeachingAssignmentId == teachingAssignmentId && qa.IsDelete == false &&
                    qa.QuestionsAnswerId != null)
                .Join(
                    _context.Users,
                    qa => qa.UserId,
                    u => u.Id,
                    (qa, u) => new
                    {
                        qa.UserId,
                        Answer = new AnswerInfo
                        {
                            Id = qa.Id,
                            Message = qa.Message,
                            CreateAt = qa.CreateAt,
                            Avatar = u.Image,
                            FullName = u.FullName
                        }
                    })
                .GroupBy(x => x.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    Answers = g.Select(x => x.Answer).ToList()
                })
                .ToListAsync();

            var userAnswers = userAnswersQuery
                .ToDictionary(ua => ua.UserId, ua => ua.Answers);

            // Lấy danh sách giáo viên
            var teachersQuery = _context.TeachingAssignments
                .AsNoTracking()
                .Where(ta => ta.Id == teachingAssignmentId && ta.IsDelete == false && ta.User.RoleId == 2);

            if (userIdsWithQuestions.Any())
            {
                teachersQuery = teachersQuery.Where(ta => userIdsWithQuestions.Contains(ta.User.Id));
            }

            var teachers = await teachersQuery
                .Select(ta => new ClassMemberResponse
                {
                    UserId = ta.User.Id,
                    FullName = ta.User.FullName,
                    Avatar = ta.User.Image, // Thêm ánh xạ Avatar
                    Email = ta.User.Email,
                    Phone = ta.User.Phone,
                    Role = "teacher",
                    ClassId = ta.Class.Id,
                    ClassName = ta.Class.Name
                })
                .GroupBy(t => new { t.UserId, t.ClassId })
                .Select(g => g.First())
                .ToListAsync();

            // Lấy danh sách lớp học liên quan đến teaching assignment
            var relatedClassIds = await _context.TeachingAssignments
                .AsNoTracking()
                .Where(ta => ta.Id == teachingAssignmentId)
                .Where(ta => ta.ClassId.HasValue)
                .Select(ta => ta.ClassId.Value)
                .ToListAsync();

            // Lấy tất cả bản ghi class_students cho các lớp liên quan
            var allClassStudents = await _context.ClassStudents
                .AsNoTracking()
                .Where(cs => cs.ClassId.HasValue && relatedClassIds.Contains(cs.ClassId.Value))
                .Include(cs => cs.User) // Đảm bảo load thông tin User
                .Include(cs => cs.Class) // Đảm bảo load thông tin Class
                .ToListAsync();

            // Xử lý trong memory để lấy bản ghi mới nhất cho mỗi student
            var activeStudents = allClassStudents
                .GroupBy(cs => cs.UserId)
                .Select(g => g.OrderByDescending(cs => cs.Id).FirstOrDefault())
                .Where(cs => cs != null 
                          && cs.IsDelete == false 
                          && cs.User.RoleId == 3 
                          && cs.IsActive == true)
                .ToList();

            // Lọc theo userIdsWithQuestions nếu có
            if (userIdsWithQuestions.Any())
            {
                activeStudents = activeStudents
                    .Where(cs => userIdsWithQuestions.Contains(cs.UserId))
                    .ToList();
            }

            // Chuyển đổi sang ClassMemberResponse
            var students = activeStudents
                .Select(cs => new ClassMemberResponse
                {
                    UserId = cs.User.Id,
                    FullName = cs.User.FullName,
                    Avatar = cs.User.Image,
                    Email = cs.User.Email,
                    Phone = cs.User.Phone,
                    Role = "student",
                    ClassId = cs.ClassId.Value,
                    ClassName = cs.Class?.Name
                })
                .GroupBy(s => new { s.UserId, s.ClassId })
                .Select(g => g.First())
                .ToList();

            // Gán số lượt xem, câu hỏi, câu trả lời và số lượng câu hỏi/câu trả lời cho từng giáo viên
            foreach (var teacher in teachers)
            {
                teacher.Views = userViews.ContainsKey(teacher.UserId) ? userViews[teacher.UserId] : 0;
                teacher.Questions = userQuestions.ContainsKey(teacher.UserId)
                    ? userQuestions[teacher.UserId]
                    : new List<QuestionInfo>();
                teacher.Answers = userAnswers.ContainsKey(teacher.UserId)
                    ? userAnswers[teacher.UserId]
                    : new List<AnswerInfo>();
                teacher.TotalQuestions = teacher.Questions.Count; // Tính số lượng câu hỏi
                teacher.TotalAnswers = teacher.Answers.Count; // Tính số lượng câu trả lời
            }

            // Gán số lượt xem, câu hỏi, câu trả lời và số lượng câu hỏi/câu trả lời cho từng học sinh
            foreach (var student in students)
            {
                student.Views = userViews.ContainsKey(student.UserId) ? userViews[student.UserId] : 0;
                student.Questions = userQuestions.ContainsKey(student.UserId)
                    ? userQuestions[student.UserId]
                    : new List<QuestionInfo>();
                student.Answers = userAnswers.ContainsKey(student.UserId)
                    ? userAnswers[student.UserId]
                    : new List<AnswerInfo>();
                student.TotalQuestions = student.Questions.Count; // Tính số lượng câu hỏi
                student.TotalAnswers = student.Answers.Count; // Tính số lượng câu trả lời
            }

            // Kết hợp danh sách thành viên
            var classMembers = teachers.Concat(students)
                .OrderBy(m => m.Role)
                .ThenBy(m => m.FullName)
                .ToList();

            if (!classMembers.Any())
            {
                throw new ArgumentException("Không tìm thấy thành viên nào trong lớp học!");
            }

            return new ClassMembersWithStatsResponse
            {
                Members = classMembers,
                TotalViews = totalViews
            };
        }

        public async Task<TeachingAssignmentStudentsResponse> GetTeachingAssignmentStudentsAsync(
     int teachingAssignmentId)
        {
            // Lấy thông tin phân công giảng dạy, lớp học, và giáo viên
            var teachingAssignmentInfo = await _context.TeachingAssignments
                .AsNoTracking()
                .Where(ta => ta.Id == teachingAssignmentId)
                .Select(ta => new
                {
                    ta.Id,
                    ta.UserId,
                    TeacherFullName = ta.User.FullName,
                    ClassId = ta.ClassId,
                    ClassName = ta.Class.Name
                })
                .FirstOrDefaultAsync();

            if (teachingAssignmentInfo == null || teachingAssignmentInfo.ClassId == null)
            {
                return null; // Trả về null thay vì ném ngoại lệ
            }

            // Tách thành 2 bước để tránh lỗi dịch LINQ sang SQL
            // 1. Lấy tất cả dữ liệu class_students của lớp này
            var allClassStudents = await _context.ClassStudents
                .AsNoTracking()
                .Where(cs => cs.ClassId == teachingAssignmentInfo.ClassId)
                .ToListAsync();

            // 2. Xử lý trong memory để lấy bản ghi mới nhất cho mỗi học sinh
            var latestStudentRecords = allClassStudents
                .GroupBy(cs => cs.UserId)
                .Select(g => g.OrderByDescending(cs => cs.Id).FirstOrDefault())
                .Where(cs => cs != null && cs.IsDelete == false && cs.IsActive == true)
                .ToList();

            // Lấy thông tin chi tiết của học sinh
            var studentIds = latestStudentRecords.Select(cs => cs.UserId).ToList();
            var students = await _context.Users
                .AsNoTracking()
                .Where(u => studentIds.Contains(u.Id))
                .Select(u => new StudentInfoResponse
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    RoleName = u.Role.Name
                })
                .ToListAsync();

            // Tạo dữ liệu trả về
            var responseData = new TeachingAssignmentStudentsResponse
            {
                TeachingAssignmentId = teachingAssignmentInfo.Id,
                TeacherId = teachingAssignmentInfo.UserId ?? 0,
                TeacherFullName = teachingAssignmentInfo.TeacherFullName,
                ClassName = teachingAssignmentInfo.ClassName,
                Students = students
            };

            return responseData;
        }
    }
}