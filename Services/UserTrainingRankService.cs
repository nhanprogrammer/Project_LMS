
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;




namespace Project_LMS.Services;

public class UserTrainingRankService : IUserTrainingRankService
{

    private readonly ApplicationDbContext _context;
    public UserTrainingRankService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ApiResponse<UserTrainingRankResponse>> Create(UserTrainingRankRequest user)
    {
        try
        {
            var userTrain = ToUserTrainingRankRequest(user);
            var _user = await _context.Users.FindAsync(user.UserId);
            userTrain.User = _user;
            var _trainingRank = await _context.TrainingRanks.FindAsync(user.TrainingRankId);
            userTrain.TrainingRank = _trainingRank;
            await _context.UserTrainingRanks.AddAsync(userTrain);
            await _context.SaveChangesAsync();
            return new ApiResponse<UserTrainingRankResponse>(0, "Create UserTrainingRank success.")
            {
                Data = ToUserTrainingRank(userTrain)
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<UserTrainingRankResponse>(1, "Create UserTrainingRank error : " + ex);
        }


    }

    public async Task<ApiResponse<UserTrainingRankResponse>> Delete(int id)
    {
        var userTrain = await _context.UserTrainingRanks.FindAsync(id);
        if (userTrain != null)
        {
            try
            {
                _context.UserTrainingRanks.Remove(userTrain);
                await _context.SaveChangesAsync();
                return new ApiResponse<UserTrainingRankResponse>(0, "Delete UserTrainingRank success");
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserTrainingRankResponse>(1, "Delete UserTrainingRank error : " + ex);
            }
        }
        else
        {
            return new ApiResponse<UserTrainingRankResponse>(1, "UserTrainingRank dose not exist.");
        }
    }

    public async Task<ApiResponse<List<UserTrainingRankResponse>>> GetAll()
    {
        var userTrains = await _context.UserTrainingRanks.ToListAsync();
        if (userTrains.Any())
        {
            var userTrainReponses = userTrains.Select(userTrain => ToUserTrainingRank(userTrain)).ToList();
            return new ApiResponse<List<UserTrainingRankResponse>>(0, "GetAll UserTrainingRank success.")
            {
                Data = userTrainReponses
            };
        }
        else
        {
            return new ApiResponse<List<UserTrainingRankResponse>>(1, "No UserTrainingRank found.");
        }
    }

    public async Task<ApiResponse<UserTrainingRankResponse>> Search(int id)
    {
        var userTrain = await _context.UserTrainingRanks.FindAsync(id);
        if (userTrain != null)
        {
            return new ApiResponse<UserTrainingRankResponse>(0, "Found success.")
            {
                Data = ToUserTrainingRank(userTrain)
            };
        }
        else
        {
            return new ApiResponse<UserTrainingRankResponse>(1, "UserTrainingRank does not exist.");
        }
    }

    public UserTrainingRankResponse ToUserTrainingRank(UserTrainingRank user)
    {
        return new UserTrainingRankResponse
        {
            Id = user.Id,
            UserId = user.UserId,
            TrainingRankId = user.TrainingRankId
        };
    }

    public UserTrainingRank ToUserTrainingRankRequest(UserTrainingRankRequest user)
    {
        return new UserTrainingRank
        {
            UserId = user.UserId,
            TrainingRankId = user.TrainingRankId
        };
    }

    public async Task<ApiResponse<UserTrainingRankResponse>> Update(int id, UserTrainingRankRequest user)
    {
        var userTrain = await _context.UserTrainingRanks.FindAsync(id);
        if (userTrain != null)
        {
            try
            {
                var _user = await _context.Users.FindAsync(user.UserId);
                userTrain.User = _user;
                var _trainingRank = await _context.TrainingRanks.FindAsync(user.TrainingRankId);
                userTrain.TrainingRank = _trainingRank;
                await _context.SaveChangesAsync();
                return new ApiResponse<UserTrainingRankResponse>(0, "Update UserTrainingRank success.")
                {
                    Data = ToUserTrainingRank(userTrain)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserTrainingRankResponse>(1, "Update UserTrainingRank error : " + ex);
            }
        }
        else
        {
            return new ApiResponse<UserTrainingRankResponse>(1, "UserTrainingRank dose not exist.");
        }
    }
}