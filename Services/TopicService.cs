using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;
﻿using Project_LMS.Interfaces.Services;


namespace Project_LMS.Services;

public class TopicService : ITopicService
{

    private readonly ApplicationDbContext _context;
    public TopicService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ApiResponse<PaginatedResponse<TopicResponse>>> GetAllTopicsAsync(string? keyword, int pageNumber, int pageSize)
    {
        var query = _context.Topics.Where(t => !t.IsDelete.HasValue || !t.IsDelete.Value);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim().ToLower();
            query = query.Where(t => t.Title.ToLower().Contains(keyword) || t.Description.ToLower().Contains(keyword));
        }

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var topics = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        var topicResponses = topics.Select(ToTopicResponse).ToList();

        var paginatedResponse = new PaginatedResponse<TopicResponse>
        {
            Items = topicResponses,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasPreviousPage = pageNumber > 1,
            HasNextPage = pageNumber < totalPages
        };

        return new ApiResponse<PaginatedResponse<TopicResponse>>(0, "Success", paginatedResponse);
    }

    public async Task<ApiResponse<TopicResponse>> GetTopicByIdAsync(int id)
    {
        var topic = await _context.Topics.FindAsync(id);
        if (topic == null || topic.IsDelete == true)
            return new ApiResponse<TopicResponse>(1, "Topic not found", null);

        return new ApiResponse<TopicResponse>(0, "Success", ToTopicResponse(topic));
    }

    public async Task<ApiResponse<TopicResponse>> CreateTopicAsync(TopicRequest request)
    {
        try
        {
            var topic = ToTopic(request);
            topic.CreateAt = DateTime.UtcNow.ToLocalTime();
            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();
            return new ApiResponse<TopicResponse>(0, "Topic created successfully", ToTopicResponse(topic));
        }
        catch (Exception ex)
        {
            return new ApiResponse<TopicResponse>(1, $"Error creating Topic: {ex.Message}", null);
        }
    }

    public async Task<ApiResponse<TopicResponse>> UpdateTopicAsync(int id, TopicRequest request)
    {
        try
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null || topic.IsDelete == true)
                return new ApiResponse<TopicResponse>(1, "Topic not found", null);

            topic.Title = request.Title;
            topic.FileName = request.FileName;
            topic.Description = request.Description;
            topic.CloseAt = request.CloseAt;
            topic.UpdateAt = DateTime.UtcNow.ToLocalTime();

            await _context.SaveChangesAsync();
            return new ApiResponse<TopicResponse>(0, "Topic updated successfully", ToTopicResponse(topic));
        }
        catch (Exception ex)
        {
            return new ApiResponse<TopicResponse>(1, $"Error updating Topic: {ex.Message}", null);
        }
    }

    public async Task<ApiResponse<bool>> DeleteTopicAsync(int id)
    {
        try
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null || topic.IsDelete == true)
                return new ApiResponse<bool>(1, "Topic not found", false);

            topic.IsDelete = true;
            topic.UpdateAt = DateTime.UtcNow.ToLocalTime();

            await _context.SaveChangesAsync();
            return new ApiResponse<bool>(0, "Topic deleted successfully", true);
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>(1, $"Error deleting Topic: {ex.Message}", false);
        }
    }

    private TopicResponse ToTopicResponse(Topic topic)
    {
        return new TopicResponse
        {
            Id = topic.Id,
            TeachingAssignmentId = topic.TeachingAssignmentId,
            UserId = topic.UserId,
            TopicId = topic.TopicId,
            UpdateAt = topic.UpdateAt,
            CreateAt = topic.CreateAt,
            UserCreate = topic.UserCreate,
            UserUpdate = topic.UserUpdate,
            IsDelete = topic.IsDelete,
            Title = topic.Title,
            FileName = topic.FileName,
            Description = topic.Description,
            CloseAt = topic.CloseAt
        };
    }

    private Topic ToTopic(TopicRequest request)
    {
        return new Topic
        {
            TeachingAssignmentId = request.TeachingAssignmentId,
            UserId = request.UserId,
            TopicId = request.TopicId,
            UserCreate = request.UserCreate,
            UserUpdate = request.UserUpdate,
            Title = request.Title,
            FileName = request.FileName,
            Description = request.Description,
            CloseAt = request.CloseAt
        };
    }

}
