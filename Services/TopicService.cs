using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;


namespace Project_LMS.Services;

public class TopicService : ITopicService
{

    private readonly ApplicationDbContext _context;
    public TopicService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ApiResponse<TopicResponse>> Create(TopicRequest request)
    {
        var topic = ToTopicRequest(request);
        try
        {
            var teachingAssignment = await _context.TeachingAssignments.FindAsync(request.TeachingAssignmentId);
            topic.TeachingAssignment = teachingAssignment;
            var topicNavigation = await _context.Topics.FindAsync(request.TopicId);
            topic.TopicNavigation = topicNavigation;
            var user = await _context.Users.FindAsync(request.UserId);
            topic.User = user;
            topic.CreateAt = DateTime.Now;
            await _context.Topics.AddAsync(topic);
            _context.SaveChanges();
            return new ApiResponse<TopicResponse>(1, "Create Topic success")
            {
                Data = ToTopic(topic)
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<TopicResponse>(1, "Create Topic error : " + ex);
        }
    }

    public async Task<ApiResponse<TopicResponse>> Delete(int id)
    {
        var topic = await _context.Topics.FindAsync(id);
        if (topic != null)
        {
            try
            {
                _context.Topics.Remove(topic);

            }
            catch (Exception ex)
            {
                topic.IsDelete = true;
            }
            await _context.SaveChangesAsync();
            return new ApiResponse<TopicResponse>(0, "Delete Topic success");
        }
        else
        {
            return new ApiResponse<TopicResponse>(1, "Topic does not exist.");
        }
    }

    public async Task<ApiResponse<List<TopicResponse>>> GetAll()
    {
        var topics = await _context.Topics.ToListAsync();
        if (topics.Any())
        {
            var topicResponses = topics.Select(topic => ToTopic(topic)).ToList();
            return new ApiResponse<List<TopicResponse>>(0, "GetAll Topic success.")
            {
                Data = topicResponses
            };
        }
        else
        {
            return new ApiResponse<List<TopicResponse>>(1, "No Topic found.");
        }
        {

        }
    }

    public async Task<ApiResponse<TopicResponse>> Search(int id)
    {
        var topic = await _context.Topics.FindAsync(id);
        if (topic != null)
        {
            return new ApiResponse<TopicResponse>(0, "Found success.")
            {
                Data = ToTopic(topic)
            };
        }
        else
        {
            return new ApiResponse<TopicResponse>(1, "Topic does not exist.");
        }
    }

    public TopicResponse ToTopic(Topic topic)
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

    public Topic ToTopicRequest(TopicRequest topic)
    {
        return new Topic
        {
            TeachingAssignmentId = topic.TeachingAssignmentId,
            UserId = topic.UserId,
            TopicId = topic.TopicId,
            UserCreate = topic.UserCreate,
            UserUpdate = topic.UserUpdate,
            Title = topic.Title,
            FileName = topic.FileName,
            Description = topic.Description,
            CloseAt = topic.CloseAt
        };
    }

    public async Task<ApiResponse<TopicResponse>> Update(int id, TopicRequest request)
    {
        var topic = await _context.Topics.FindAsync(id);
        if (topic != null)
        {
            try
            {
                topic.TeachingAssignmentId = request.TeachingAssignmentId;
                topic.Title = request.Title;
                topic.FileName = request.FileName;
                topic.Description = request.Description;
                topic.CloseAt = request.CloseAt;

                var teachingAssignment = await _context.TeachingAssignments.FindAsync(request.TeachingAssignmentId);
                topic.TeachingAssignment = teachingAssignment;
                var topicNavigation = await _context.Topics.FindAsync(request.TopicId);
                topic.TopicNavigation = topicNavigation;
                var user = await _context.Users.FindAsync(request.UserId);
                topic.User = user;
                topic.UpdateAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return new ApiResponse<TopicResponse>(1, "Update Topic success : ")
                {
                    Data = ToTopic(topic)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TopicResponse>(1, "Update Topic error : " + ex);
            }
        }
        else
        {
            return new ApiResponse<TopicResponse>(1, "Topic does not exist.");
        }
    }
    

}
