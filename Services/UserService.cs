
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using OfficeOpenXml;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;




namespace Project_LMS.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IJwtReponsitory _jwtReponsitory;
    private readonly IEmailService _emailService;
    public static  string otp = "";
    public static string nameUser = "";
    public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger, ApplicationDbContext context, ICloudinaryService cloudinaryService, IJwtReponsitory jwtReponsitory, IEmailService emailService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _context = context;
        _cloudinaryService = cloudinaryService;
        _jwtReponsitory = jwtReponsitory;
        _emailService = emailService;
    }

    public async Task<ApiResponse<object>> CheckUser(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new ApiResponse<object>(1, "Email hoặc username của bạn không tồn tại.");
        }
        nameUser = name;
        var user = await _context.Users
       .Include(user => user.Role)
       .AsNoTracking()
       .SingleOrDefaultAsync(user => user.Username.Equals(name) || user.Email.Equals(name));
        if (user == null) return new ApiResponse<object>(1, "User does not existed.");
        Random random = new Random();
        otp = random.Next(100000, 999999).ToString();
        await _emailService.SendMailAsync(user?.Email, "Quên mật khẩu","Mã OTP của bạn là "+otp);
        return new ApiResponse<object>(0, "Send email success.");
    }

    public async Task<ApiResponse<UserResponse>> Create(UserRequest request)
    {

        // Kiểm tra Role có tồn tại không
        if (request.RoleId.HasValue)
        {
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == request.RoleId);
            if (!roleExists)
            {
                return new ApiResponse<UserResponse>(1, "Role không tồn tại.");
            }
        }

        // Kiểm tra GroupRolePermission có tồn tại không
        var groupRoleExists = await _context.ModulePermissions.AnyAsync(g => g.Id == request.GroupRolePermission);
        if (!groupRoleExists)
        {
            return new ApiResponse<UserResponse>(1, "Group Role Permission không hợp lệ.");
        }

        // Kiểm tra StudentStatus & TeacherStatus
        var studentStatusExists = await _context.StudentStatuses.AnyAsync(s => s.Id == request.StudentStatusId);
        var teacherStatusExists = await _context.TeacherStatuses.AnyAsync(t => t.Id == request.TeacherStatusId);
        if (!studentStatusExists || !teacherStatusExists)
        {
            return new ApiResponse<UserResponse>(1, "StudentStatus hoặc TeacherStatus không hợp lệ.");
        }

        // Map dữ liệu từ request sang entity
        var user = _mapper.Map<User>(request);
        user.IsDelete = false;
        user.CreateAt = DateTime.Now;

        // Thêm vào database
        await _userRepository.AddAsync(user);

        return new ApiResponse<UserResponse>(0, "User created successfully.")
        {
            Data = _mapper.Map<UserResponse>(user)
        };


    }



    public async Task<ApiResponse<UserResponse>> Delete(int id)
    {
        try
        {
            var user = await _userRepository.FindAsync(id);
            if (user == null)
                return new ApiResponse<UserResponse>(1, "User not found.");

            user.IsDelete = true;
            user.UpdateAt = DateTime.Now;
            await _userRepository.UpdateAsync(user);

            return new ApiResponse<UserResponse>(0, "User deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting user with ID {id}.");
            return new ApiResponse<UserResponse>(-1, "An error occurred while deleting the user.");
        }
    }

    public async Task<ApiResponse<object>> ExportUsersToExcel()
    {
        try
        {
            var users = await _context.Users.Include(u => u.StudentStatus).ToListAsync();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Cấu hình LicenseContext

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Users");

                // Tiêu đề cột
                worksheet.Cells[1, 1].Value = "User Code";
                worksheet.Cells[1, 2].Value = "Full Name";
                worksheet.Cells[1, 3].Value = "Birth Date";
                worksheet.Cells[1, 4].Value = "Gender";
                worksheet.Cells[1, 5].Value = "Ethnicity";
                worksheet.Cells[1, 6].Value = "Status";

                // Ghi dữ liệu vào Excel
                int row = 2;
                foreach (var user in users)
                {
                    worksheet.Cells[row, 1].Value = user.UserCode;
                    worksheet.Cells[row, 2].Value = user.FullName;
                    worksheet.Cells[row, 3].Value = user.BirthDate?.ToString("dd-MM-yyyy");
                    worksheet.Cells[row, 4].Value = user.Gender != null && user.Gender.Length > 0 ? user.Gender[0] : false;
                    worksheet.Cells[row, 5].Value = user.Ethnicity;
                    worksheet.Cells[row, 6].Value = user.StudentStatus?.StatusName ?? "Unknown";
                    row++;
                }

                // Căn chỉnh cột
                worksheet.Cells.AutoFitColumns();

                // Chuyển đổi file Excel thành mảng byte
                //byte[] fileBytes = package.GetAsByteArray();
                var fileBytes = package.GetAsByteArray();
                string base64Excel = Convert.ToBase64String(fileBytes);
                string url = await _cloudinaryService.UploadExcelAsync(base64Excel);
                return new ApiResponse<object>(0, "Xuất file Excel thành công.")
                {
                    Data = url
                };
            }
        }
        catch (Exception ex)
        {
            return new ApiResponse<object>(1, $"Lỗi xuất Excel: {ex.Message}");
        }
    }

    public async Task<ApiResponse<object>> ChangePassword(ForgotPasswordRequest request)
    {
        var user = await _context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Username.Equals(nameUser) || user.Email.Equals(nameUser));
        if (string.IsNullOrWhiteSpace(request.Password)) return new ApiResponse<object>(1, "Mật khẩu không được null.");
        if (string.IsNullOrWhiteSpace(request.Confirm)) return new ApiResponse<object>(1, "Xác nhận mật khẩu không được null.");
     
        if (user == null) return new ApiResponse<object>(1, "Tài khoản không tồn tại.");

        if (request.Password != request.Confirm) return new ApiResponse<object>(1, "Xác nhận mật khẩu không đúng.");
        user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return new ApiResponse<object>(0, "Thay đổi mật khẩu thành công");
    }

    public async Task<ApiResponse<PaginatedResponse<object>>> GetAll(int pageNumber, int pageSize)
    {
        // Lấy danh sách users và tổng số user trong DB
        var users = await _userRepository.GetAllAsync(pageNumber, pageSize);
        var totalItems = await _userRepository.CountAsync(); // Lấy tổng số lượng bản ghi

        if (users.Any())
        {
            var userResponses = users.Select(user => (object)new
            {
                user.UserCode,
                user.FullName,
                user.BirthDate,
                Gender = (user.Gender != null && user.Gender.Length > 0) ? user.Gender[0] : false, // Ép kiểu từ BitArray sang bool
                user.Ethnicity,
                status = user.StudentStatus?.StatusName ?? "Unknown"
            }).ToList();

            var paginatedResponse = new PaginatedResponse<object>
            {
                Items = userResponses,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < (int)Math.Ceiling(totalItems / (double)pageSize)
            };

            return new ApiResponse<PaginatedResponse<object>>(0, "GetAll User success.") { Data = paginatedResponse };
        }
        else
        {
            return new ApiResponse<PaginatedResponse<object>>(1, "No User found.");
        }
    }


    public async Task<ApiResponse<List<UserResponse>>> GetAllByIds(List<int> ids, int pageNumber, int pageSize)
    {
        var users = await _userRepository.GetAllByIdsAsync(ids, pageNumber, pageSize);
        if (users.Any())
        {
            var userResponses = users.Select(user => _mapper.Map<UserResponse>(user)).ToList();
            return new ApiResponse<List<UserResponse>>(0, "GetAll User success.")
            {
                Data = userResponses
            };
        }
        else
        {
            return new ApiResponse<List<UserResponse>>(1, "No User found.");
        }
    }

    public async Task<ApiResponse<UserResponse>> Search(int id)
    {
        try
        {
            var user = await _userRepository.FindAsync(id);
            if (user == null)
                return new ApiResponse<UserResponse>(1, "User not found.");

            return new ApiResponse<UserResponse>(0, "User found.")
            {
                Data = _mapper.Map<UserResponse>(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while searching for user with ID {id}.");
            return new ApiResponse<UserResponse>(-1, "An error occurred while searching for the user.");
        }
    }


    public async Task<ApiResponse<UserResponse>> Update(int id, UserRequest request)
    {
        try
        {
            var user = await _userRepository.FindAsync(id);
            if (user == null)
                return new ApiResponse<UserResponse>(1, "User not found.");
            user.UpdateAt = DateTime.Now;
            _mapper.Map(request, user);
            await _userRepository.UpdateAsync(user);

            return new ApiResponse<UserResponse>(0, "User updated successfully.")
            {
                Data = _mapper.Map<UserResponse>(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while updating user with ID {id}.");
            return new ApiResponse<UserResponse>(-1, "An error occurred while updating the user.");
        }
    }

    public async Task<ApiResponse<object>> CheckOTP(string otpUser)
    {
        if (string.IsNullOrWhiteSpace(otpUser))
        {
            return new ApiResponse<object>(1, "Mã OTP không được null.");
        }

        if (!otpUser.Equals(otp)) 
        {
            return new ApiResponse<object>(2, "Mã OTP không hợp lệ.");
        }
        var user = _context.Users.Include(u=>u.Role).Where(u=>u.Username.Equals(nameUser) || u.Email.Equals(nameUser)).FirstOrDefault();
        return new ApiResponse<object>(0, "Xác nhận mã OTP hợp lệ.")
        {
            Data = new { token  = _jwtReponsitory.GenerateToken(user) } 
        };
    }
}