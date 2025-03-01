
﻿
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;


namespace Project_LMS.Services;

public class UserService : IUserService
{

    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ApiResponse<UserResponse>> Create(UserRequest user)
    {
        var _user = ToUserRequest(user);
        try
        {
            var configuration = await _context.SystemSettings.FindAsync(user.ConfigurationId);
            _user.Configuration = configuration;
            await _context.Users.AddAsync(_user);
            await _context.SaveChangesAsync();
            return new ApiResponse<UserResponse>(0, "Create User success")
            {
                Data = ToUser(_user)
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<UserResponse>(1, "Create User error : " + ex);
        }
    }

    public async Task<ApiResponse<UserResponse>> Delete(int id)
    {
        var _user = await _context.Users.FindAsync(id);
        if (_user != null)
        {
            try
            {
                _context.Users.Remove(_user);
            }
            catch (Exception ex)
            {
                _user.IsDelete = true;

            }
            await _context.SaveChangesAsync();
            return new ApiResponse<UserResponse>(0, "Delete User success");
        }

        else
        {
            return new ApiResponse<UserResponse>(1, "User does not exist.");
        }
    }

    public async Task<ApiResponse<List<UserResponse>>> GetAll()
    {
        var users = await _context.Users.ToListAsync();
        if (users.Any())
        {
            var userResponses = users.Select(user => ToUser(user)).ToList();
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
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            return new ApiResponse<UserResponse>(0, "Found success.")
            {
                Data = ToUser(user)
            };
        }
        else
        {
            return new ApiResponse<UserResponse>(1, "User does not exist.");
        }
    }

    public UserResponse ToUser(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            ConfigurationId = user.ConfigurationId,
            Role = user.Role,
            Username = user.Username,
            Password = user.Password,
            FullName = user.FullName,
            Email = user.Email,
            Active = user.Active,
            IsDelete = user.IsDelete,
            CreateAt = user.CreateAt,
            UpdateAt = user.UpdateAt,
            UserCreate = user.UserCreate,
            UserUpdate = user.UserUpdate
        };

    }

    public User ToUserRequest(UserRequest user)
    {
        return new User
        {
            ConfigurationId = user.ConfigurationId,
            Role = user.Role,
            Username = user.Username,
            Password = user.Password,
            FullName = user.FullName,
            Email = user.Email,
            Active = user.Active,
            UserCreate = user.UserCreate,
            UserUpdate = user.UserUpdate
        };
    }

    public async Task<ApiResponse<UserResponse>> Update(int id, UserRequest user)
    {
        var _user = await _context.Users.FindAsync(id);
        if (_user != null)
        {
            try
            {
                _user.UpdateAt = DateTime.Now;
                var configuraton = await _context.SystemSettings.FindAsync(_user.ConfigurationId);
                _user.Configuration = configuraton;
                _user.Role = user.Role;
                _user.Username = user.Username;
                _user.Password = user.Password;
                _user.FullName = user.FullName;
                _user.Email = user.Email;
                _user.Active = user.Active;
                _user.UserCreate = user.UserCreate;
                _user.UserUpdate = user.UserUpdate;
                await _context.SaveChangesAsync();
                return new ApiResponse<UserResponse>(0, "Update User success.")
                {
                    Data = ToUser(_user)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserResponse>(1, "Update User error : " + ex);
            }
        }
        else
        {
            return new ApiResponse<UserResponse>(1, "User does not exist.");
        }
    }

}