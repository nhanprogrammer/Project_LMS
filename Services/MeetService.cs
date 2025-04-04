using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Models;
using Twilio;
using Twilio.Jwt.AccessToken;
using Twilio.Rest.Conversations.V1.Conversation;
using Twilio.Rest.Video.V1;

namespace Project_LMS.Services
{
    public class MeetService : IMeetService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;
        private readonly string _twilioAccountSid;
        private readonly string _twilioAuthToken;
        private readonly string _twilioApiKey;
        private readonly string _twilioApiSecret;

        public MeetService(ApplicationDbContext context, IConfiguration configuration, IAuthService authService)
        {
            _context = context;
            _authService = authService;
            _twilioAccountSid = configuration["Twilio:AccountSid"] ?? throw new ArgumentNullException("Twilio:AccountSid");
            _twilioAuthToken = configuration["Twilio:AuthToken"] ?? throw new ArgumentNullException("Twilio:AuthToken");
            _twilioApiKey = configuration["Twilio:ApiKey"] ?? throw new ArgumentNullException("Twilio:ApiKey");
            _twilioApiSecret = configuration["Twilio:ApiSecret"] ?? throw new ArgumentNullException("Twilio:ApiSt");
            TwilioClient.Init(_twilioAccountSid, _twilioAuthToken);
        }

        public async Task<MeetResponse?> GetJitsiClassRoom(CreateRoomRequest request)
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                throw new UnauthorizedAccessException("Không thể xác thực user.");

            // 🔹 Tìm lesson theo LessonId
            var lesson = await _context.Lessons
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == request.LessonId);

            if (lesson == null)
                throw new KeyNotFoundException($"Không tìm thấy lesson với Id: {request.LessonId}");

            if (string.IsNullOrEmpty(lesson.ClassLessonCode))
                throw new InvalidOperationException($"Lesson {request.LessonId} không có mã LessonCode.");

            // 🔹 Kiểm tra quyền của user (Teacher hay Student)
            if (user.Role == null)
                throw new InvalidOperationException("User không có Role xác định.");

            bool isTeacher = user.Role.Name.Equals("TEACHER", StringComparison.OrdinalIgnoreCase);

            // 🔹 Kiểm tra ClassOnline có tồn tại cho lesson này chưa
            var classOnline = await _context.ClassOnlines
                .FirstOrDefaultAsync(c => c.LessonCode == lesson.ClassLessonCode);

            if (isTeacher)
            {
                if (classOnline == null)
                {
                    // 🔹 Nếu chưa có, giáo viên tạo mới ClassOnline với mã phòng UUID
                    classOnline = new ClassOnline
                    {
                        LessonCode = lesson.ClassLessonCode,
                        ClassOnlineCode = Guid.NewGuid().ToString(), // Tạo mã duy nhất
                        UserId = user.Id,
                        UserCreate = user.Id
                    };

                    _context.ClassOnlines.Add(classOnline);
                    await _context.SaveChangesAsync();
                }

                return new MeetResponse
                {
                    ClassTitle = $"{lesson.Topic} - GV: {lesson.User?.FullName ?? "Không xác định"}",
                    Link = $"https://meet.jit.si/{classOnline.ClassOnlineCode}"
                };
            }
            else
            {
                if (classOnline == null)
                    throw new InvalidOperationException("Lớp học chưa được bắt đầu. Vui lòng chờ giáo viên.");

                return new MeetResponse
                {
                    ClassTitle = $"{lesson.Topic} - GV: {lesson.User?.FullName ?? "Không xác định"}",
                    Link = $"https://meet.jit.si/{classOnline.ClassOnlineCode}"
                };
            }
        }

        
        public async Task<ClassOnlineResponse?> GetOrCreateTeacherOnlineClass(CreateRoomRequest request)
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                throw new Exception("Không thể xác thực user.");

            // 🔹 Kiểm tra xem lớp học đã tồn tại chưa
            var classOnline = await _context.ClassOnlines
                .Include(c => c.Lesson)
                .FirstOrDefaultAsync(c => c.LessonId == request.LessonId);

            // 🔹 Nếu lớp học đã tồn tại, cập nhật ClassOnlineCode từ Twilio
            if (classOnline != null)
            {
                // 🔹 Tạo phòng mới trên Twilio
                var room = await RoomResource.CreateAsync(
                    uniqueName: Guid.NewGuid().ToString(),
                    type: RoomResource.RoomTypeEnum.Group
                );

                if (room == null || string.IsNullOrEmpty(room.Sid))
                    throw new Exception("Không thể tạo phòng trên Twilio.");

                // 🔹 Cập nhật mã phòng mới từ Twilio
                classOnline.ClassOnlineCode = room.Sid;
                classOnline.UserUpdate = user.Id;
                _context.ClassOnlines.Update(classOnline);
                await _context.SaveChangesAsync();

                return new ClassOnlineResponse
                {
                    Id = classOnline.Id,
                    ClassTitle = classOnline.ClassTitle,
                    Token = GenerateToken(user.Email, classOnline.ClassOnlineCode),
                    RoomCode = classOnline.ClassOnlineCode
                };
            }

            // 🔹 Nếu chưa tồn tại, tạo mới ClassOnline
            var newRoom = await RoomResource.CreateAsync(
                uniqueName: Guid.NewGuid().ToString(),
                type: RoomResource.RoomTypeEnum.Group
            );

            if (newRoom == null || string.IsNullOrEmpty(newRoom.Sid))
                throw new Exception("Không thể tạo phòng trên Twilio.");

            // 🔹 Tìm lesson liên quan
            var lesson = await _context.Lessons
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == request.LessonId);

            if (lesson == null)
                throw new Exception($"Không tìm thấy lesson với Id: {request.LessonId}");

            if (lesson.User == null)
                throw new Exception($"Lesson không có thông tin giáo viên: {request.LessonId}");

            // 🔹 Tạo mới ClassOnline
            var newClassOnline = new ClassOnline
            {
                ClassOnlineCode = newRoom.Sid,
                UserId = user.Id,
                LessonId = request.LessonId,
                ClassTitle = $"{lesson.Topic} - GV: {lesson.User.FullName}",
                UserCreate = user.Id
            };

            _context.ClassOnlines.Add(newClassOnline);
            await _context.SaveChangesAsync();

            return new ClassOnlineResponse
            {
                Id = newClassOnline.Id,
                ClassTitle = newClassOnline.ClassTitle,
                Token = GenerateToken(user.Email, newClassOnline.ClassOnlineCode),
                RoomCode = newClassOnline.ClassOnlineCode
            };
        }



        public async Task<ClassOnlineResponse?> JoinOnlineClass(CreateRoomRequest request)
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                throw new Exception("Không thể xác thực user.");

            // 🔹 Kiểm tra lớp học đã tồn tại chưa
            var classOnline = await _context.ClassOnlines
                .Include(c => c.Lesson)
                .FirstOrDefaultAsync(c => c.LessonId == request.LessonId);

            if (classOnline == null)
                throw new Exception("Lớp học chưa được tạo. Vui lòng liên hệ giáo viên.");

            return new ClassOnlineResponse
            {
                Id = classOnline.Id,
                // ClassCode = classOnline.ClassOnlineCode,
                ClassTitle = classOnline.ClassTitle,
                Token = GenerateToken(user.Email, classOnline.ClassOnlineCode),
                RoomCode = classOnline.ClassOnlineCode
            };
        }


        //  Đóng phòng học
        public async Task<bool> CloseRoom(MeetCloseRequest request)
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                throw new Exception("Không thể xác thực user.");

            var classOnline = await _context.ClassOnlines
                .Where(c => c.ClassOnlineCode == request.RoomId)
                .Select(c => new { c.UserId, c.ClassOnlineCode })
                .FirstOrDefaultAsync();

            if (classOnline == null)
                throw new Exception($"Không tìm thấy lớp học với mã: {classOnline.ClassOnlineCode}");

            if (classOnline.UserId != user.Id)
                throw new Exception("Chỉ giáo viên mới có quyền đóng phòng.");

            try
            {
                var room = await RoomResource.UpdateAsync(
                    classOnline.ClassOnlineCode,
                    status: RoomResource.RoomStatusEnum.Completed
                );

                if (room == null || room.Status != RoomResource.RoomStatusEnum.Completed)
                    throw new Exception("Không thể đóng phòng, vui lòng thử lại.");

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đóng phòng trên Twilio: {ex.Message}");
            }
        }

        // Kick user khỏi phòng (Vô hiệu hóa token)
        public async Task<bool> KickUserFromRoom(string RoomId, int userId)
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                throw new Exception("Không thể xác thực user.");

            // 🔹 Kiểm tra lớp học có tồn tại không và lấy ClassOnlineCode
            var classOnline = await _context.ClassOnlines
                .FirstOrDefaultAsync(c => c.ClassOnlineCode == RoomId);

            if (classOnline == null)
                throw new Exception($"Không tìm thấy lớp học với mã: {RoomId}");

            if (classOnline.UserId != user.Id)
                throw new Exception("Chỉ giáo viên mới có quyền kick user khỏi phòng.");

            // 🔹 Lấy email của user cần kick
            var userIdentity = await _context.Users
                .Where(u => u.Id == userId && u.IsDelete != true)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(userIdentity))
                throw new Exception("Không tìm thấy thông tin user cần kick.");

            try
            {
                // 🔹 Lấy danh sách participants để tìm participant SID theo identity
                var participants = await ParticipantResource.ReadAsync(classOnline.ClassOnlineCode);
                var participant = participants.FirstOrDefault(p => p.Identity == userIdentity);

                if (participant == null)
                    throw new Exception($"Không tìm thấy user {userIdentity} trong phòng {classOnline.ClassOnlineCode}.");

                // 🔹 Xóa participant khỏi phòng
                await ParticipantResource.DeleteAsync(classOnline.ClassOnlineCode, participant.Sid);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kick user {userIdentity}: {ex.Message}");
            }
        }

        public string GenerateToken(string email, string roomSid)
        {
            var grant = new VideoGrant { Room = roomSid };
            var token = new Token(_twilioAccountSid, _twilioApiKey, _twilioApiSecret, email, grants: new HashSet<IGrant> { grant });

            return token.ToJwt();
        }

        public async Task<bool> AddQuestionAnswer(QuestionAnswerRequest request)
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                throw new Exception("Không thể xác thực user.");

            var classOnline = await _context.ClassOnlines
                .FirstOrDefaultAsync(c => c.ClassOnlineCode == request.RoomId);

            if (classOnline == null)
                throw new Exception($"Không tìm thấy lớp học với mã: {request.RoomId}");
            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(c => c.Id == classOnline.LessonId);

            var questionAnswer = new QuestionAnswer
            {
                UserId = user.Id,
                TeachingAssignmentId = lesson.TeachingAssignmentId,
                Message = request.Content,
                UserCreate = user.Id,
                UserUpdate = user.Id
            };

            _context.QuestionAnswers.Add(questionAnswer);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
