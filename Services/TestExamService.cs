
using System.Net.Mail;
using System.Net.WebSockets;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;


namespace Project_LMS.Services;

public class TestExamService : ITestExamService
{

    private readonly ApplicationDbContext _context;

    public TestExamService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ApiResponse<TestExamResponse>> Create(TestExamRequest request)
    {
        try
        {

            var testExam = ToTestExamRequest(request);
            testExam.CreateAt = DateTime.Now;
            testExam.IsDelete = false;
            var department = await _context.Departments.FindAsync(request.DepartmentId);
            testExam.Department = department;
            var testExamType = await _context.TestExamTypes.FindAsync(request.TestExamTypeId);
            testExam.TestExamType = testExamType;
            await _context.TestExams.AddAsync(testExam);
            await _context.SaveChangesAsync();
            return new ApiResponse<TestExamResponse>(0, "Create TestExam success.")
            {
                Data = ToTestExam(testExam)
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<TestExamResponse>(1, "Create TestExam Error : " + ex);
        }
    }

    public async Task<ApiResponse<TestExamResponse>> Delete(int id)
    {
        var testExam = await _context.TestExams.FindAsync(id);
        if (testExam != null)
        {
            try
            {
                _context.TestExams.Remove(testExam);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                testExam.IsDelete = true;
                await _context.SaveChangesAsync();
            }
            return new ApiResponse<TestExamResponse>(0, "Delete TestExam success.");
        }
        else
        {
            return new ApiResponse<TestExamResponse>(1, "TestExam does not exist.");
        }
    }

    public async Task<ApiResponse<List<TestExamResponse>>> GetAll()
    {
        var testExams = await _context.TestExams.ToListAsync();

        var testExamResponses = testExams.Select(testExam => ToTestExam(testExam)).ToList();

        if (testExams.Any())
        {
            ApiResponse<List<TestExamResponse>> response = new ApiResponse<List<TestExamResponse>>(0, "Get all TestExam success.")
            {
                Data = testExamResponses
            };

            return response;
        }

        return new ApiResponse<List<TestExamResponse>>(1, "No TestExams found.");
    }


    public async Task<ApiResponse<TestExamResponse>> Search(int id)
    {
        var testExam = await _context.TestExams.FindAsync(id);
        if (testExam != null)
        {
            return new ApiResponse<TestExamResponse>(0, "found success.")
            {
                Data = ToTestExam(testExam)
            };
        }
       return new ApiResponse<TestExamResponse>(1, "Not found.");
    }

    public TestExamResponse ToTestExam(TestExam testExam)
    {
        return new TestExamResponse
        {
            Id = testExam.Id,
            DepartmentId = testExam.DepartmentId,
            TestExamTypeId = testExam.TestExamTypeId,
            Topic = testExam.Topic,
            Form = testExam.Form,
            Duration = testExam.Duration,
            Classify = testExam.Classify,
            StartDate = testExam.StartDate,
            EndDate = testExam.EndDate,
            Description = testExam.Description,
            Attachment = testExam.Attachment,
            SubmissionFormat = testExam.SubmissionFormat,
            UserCreate = testExam.UserCreate,
            UserUpdate = testExam.UserUpdate

        };
    }

    public TestExam ToTestExamRequest(TestExamRequest testExam)
    {
        return new TestExam
        {
            DepartmentId = testExam.DepartmentId,
            TestExamTypeId = testExam.TestExamTypeId,
            Topic = testExam.Topic,
            Form = testExam.Form,
            Duration = testExam.Duration,
            Classify = testExam.Classify,
            StartDate = testExam.StartDate,
            EndDate = testExam.EndDate,
            Description = testExam.Description,
            Attachment = testExam.Attachment,
            SubmissionFormat = testExam.SubmissionFormat,
            UserCreate = testExam.UserCreate,
            UserUpdate = testExam.UserUpdate

        };
    }

    public async Task<ApiResponse<TestExamResponse>> Update(int id, TestExamRequest request)
    {
        var testExam = await _context.TestExams.FindAsync(id);
        if (testExam != null)
        {

            try
            {


                testExam.Topic = request.Topic;
                testExam.Form = request.Form;
                testExam.Duration = request.Duration;
                testExam.Classify = request.Classify;
                testExam.StartDate = request.StartDate;
                testExam.EndDate = request.EndDate;
                testExam.Description = request.Description;
                testExam.Attachment = request.Attachment;
                testExam.SubmissionFormat = request.SubmissionFormat;
                testExam.UserUpdate = request.UserUpdate;

                var department = await _context.Departments.FindAsync(request.DepartmentId);
                testExam.Department = department;
                var testExamType = await _context.TestExamTypes.FindAsync(request.TestExamTypeId);
                testExam.TestExamType = testExamType;
                testExam.UpdateAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return new ApiResponse<TestExamResponse>(0, "Update TestExam success.")
                {
                    Data = ToTestExam(testExam)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TestExamResponse>(1, "Update TestExam Error : " + ex);
            }

        }
        else
        {
            return new ApiResponse<TestExamResponse>(1, "TestExam does not exist.");
        }
    }

}