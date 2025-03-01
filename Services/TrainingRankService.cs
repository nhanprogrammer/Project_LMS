
﻿
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;


namespace Project_LMS.Services;

public class TrainingRankService : ITrainingRankService
{

    private readonly ApplicationDbContext _context;
    public TrainingRankService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ApiResponse<TrainingRankResponse>> Create(TrainingRankRequest request)
    {
        var train = ToTrainingRankRequest(request);
        train.CreateAt = DateTime.Now;
        try
        {
            await _context.TrainingRanks.AddAsync(train);
            _context.SaveChanges();
            return new ApiResponse<TrainingRankResponse>(0, "Create TrainingRank success.")
            {
                Data = ToTrainingRank(train)
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<TrainingRankResponse>(1, "Create TrainingRank error : " + ex);

        }

    }

    public async Task<ApiResponse<TrainingRankResponse>> Delete(int id)
    {
        var train = await _context.TrainingRanks.FindAsync(id);
        if (train != null)
        {
            try
            {
                _context.TrainingRanks.Remove(train);
            }
            catch (Exception ex)
            {
                train.IsDelete = true;
                    }
            _context.SaveChanges();
            return new ApiResponse<TrainingRankResponse>(0, "Delete TrainingRank success.");
        }
        else
        {
            return new ApiResponse<TrainingRankResponse>(1, "TrainingRank does not exist.");
        }
    }

    public async Task<ApiResponse<List<TrainingRankResponse>>> GetAll()
    {
        var trains = await _context.TrainingRanks.ToListAsync();
        if (trains.Any())
        {
            var trainResponses = trains.Select(train=>ToTrainingRank(train)).ToList();
            return new ApiResponse<List<TrainingRankResponse>>(0, "GetAll TrainingRand success.")
            {
                Data = trainResponses
            };

        }
        else
        {
            return new ApiResponse<List<TrainingRankResponse>>(1, "No TrainingRank found.");
        }
    }

    public async Task<ApiResponse<TrainingRankResponse>> Search(int id)
    {
        var train = await _context.TrainingRanks.FindAsync(id);
        if (train != null) {
            return new ApiResponse<TrainingRankResponse>(0, "Found success.")
            {
                Data = ToTrainingRank(train)
            };
        }
        else
        {
            return new ApiResponse<TrainingRankResponse>(1, "TrainingRank does not exist.");
        }
    }

    public TrainingRankResponse ToTrainingRank(TrainingRank train)
    {
        return new TrainingRankResponse
        {
            Id = train.Id,
            EducationLevel = train.EducationLevel,
            FormTraining = train.FormTraining,
            IsYear = train.IsYear,
            IsModule = train.IsModule,
            Year = train.Year,
            SemesterYear = train.SemesterYear,
            RequiredModule = train.RequiredModule,
            ElectiveModule = train.ElectiveModule,
            IsActive = train.IsActive,
            UserCreate = train.UserCreate,
            UserUpdate = train.UserUpdate,
            UpdateAt = train.UpdateAt,
            CreateAt = train.CreateAt,
            IsDelete = train.IsDelete
        };
    }

    public TrainingRank ToTrainingRankRequest(TrainingRankRequest train)
    {
        return new TrainingRank
        {
            EducationLevel = train.EducationLevel,
            FormTraining = train.FormTraining,
            IsYear = train.IsYear,
            IsModule = train.IsModule,
            Year = train.Year,
            SemesterYear = train.SemesterYear,
            RequiredModule = train.RequiredModule,
            ElectiveModule = train.ElectiveModule,
            IsActive = train.IsActive,
            UserCreate = train.UserCreate,
            UserUpdate = train.UserUpdate
        };
    }

    public async Task<ApiResponse<TrainingRankResponse>> Update(int id, TrainingRankRequest request)
    {
       var train = await _context.TrainingRanks.FindAsync(id);
        if (train != null) {
            try
            {
                train.EducationLevel = request.EducationLevel;
                train.FormTraining = request.FormTraining;
                train.IsYear = request.IsYear;
                train.IsModule = request.IsModule;
                train.Year = request.Year;
                train.SemesterYear = request.SemesterYear;
                train.RequiredModule = request.RequiredModule;
                train.ElectiveModule = request.ElectiveModule;
                train.IsActive = request.IsActive;
                train.UserCreate = request.UserCreate;
                train.UserUpdate = request.UserUpdate;
                train.UpdateAt = DateTime.Now;
                _context.SaveChanges();
                return new ApiResponse<TrainingRankResponse>(0, "Update TrainingRank success.")
                {
                    Data = ToTrainingRank(train)
                };
            }
            catch (Exception ex) {
                return new ApiResponse<TrainingRankResponse>(1, "Update TrainingRank error : "+ex);
            }
        }
        else
        {
            return new ApiResponse<TrainingRankResponse>(1, "TrainingRank does not exist.");
        }
    }

}