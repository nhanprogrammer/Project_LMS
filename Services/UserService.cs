

using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger, ApplicationDbContext context)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _context = context;
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


    public async Task<ApiResponse<List<UserResponse>>> GetAll(int pageNumber, int pageSize)
    {
        var users = await _userRepository.GetAllAsync(pageNumber, pageSize);
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

}