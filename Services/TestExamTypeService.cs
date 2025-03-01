
﻿using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;


namespace Project_LMS.Services;

public class TestExamTypeService : ITestExamTypeService
{
    private readonly ApplicationDbContext _context;
    public TestExamTypeService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ApiResponse<TestExamTypeResponse>> Create(TestExamTypeRequest request)
    {
        try
        {
            var testExamType = ToTestExamTypeRequest(request);
            await _context.TestExamTypes.AddAsync(testExamType);
            await _context.SaveChangesAsync();
            return new ApiResponse<TestExamTypeResponse>(0, "Create TestExamType success.")
            {
                Data = ToTestExamType(testExamType)
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<TestExamTypeResponse>(1, "Create TestExamType error : " + ex);
        }
    }

    public async Task<ApiResponse<TestExamTypeResponse>> Delete(int id)
    {

        var testExamType = await _context.TestExamTypes.FindAsync(id);
        if (testExamType != null)
        {
            try
            {
                _context.TestExamTypes.Remove(testExamType);
                await _context.SaveChangesAsync();
                return new ApiResponse<TestExamTypeResponse>(0, "Delete TestExamType success.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<TestExamTypeResponse>(1, "Delete TestExamType error : " + ex);
            }
        }
        else
        {
            return new ApiResponse<TestExamTypeResponse>(1, "TestExamType does not exist.");
        }

    }

    public async Task<ApiResponse<List<TestExamTypeResponse>>> GetAll()
    {
        var testExamTypes = await _context.TestExamTypes.ToListAsync();
        var testExamTypeResponse = testExamTypes.Select(testExamType => ToTestExamType(testExamType)).ToList();
        if (testExamTypes.Any())
        {
            return new ApiResponse<List<TestExamTypeResponse>>(0, "GetAll TestExamType success.")
            {
                Data = testExamTypeResponse
            };
        }
        else
        {
            return new ApiResponse<List<TestExamTypeResponse>>(1, "No TestExamType found.");
        }
    }

    public async Task<ApiResponse<TestExamTypeResponse>> Search(int id)
    {
        var testExamType = await _context.TestExamTypes.FindAsync(id);
        if (testExamType != null)
        {
            return new ApiResponse<TestExamTypeResponse>(0, "Found success.")
            {
                Data = ToTestExamType(testExamType)

            };
        }
        else
        {
            return new ApiResponse<TestExamTypeResponse>(1, "Not found.");
        }
    }

    public TestExamTypeResponse ToTestExamType(TestExamType testExam)
    {
        return new TestExamTypeResponse
        {
            Id = testExam.Id,
            Name = testExam.Name,
            Description = testExam.Description
        };
    }

    public TestExamType ToTestExamTypeRequest(TestExamTypeRequest request)
    {
        return new TestExamType
        {
            Name = request.Name,
            Description = request.Description
        };
    }

    public async Task<ApiResponse<TestExamTypeResponse>> Update(int id, TestExamTypeRequest request)
    {
        var testExamType = await _context.TestExamTypes.FindAsync(id);
        if (testExamType != null)
        {
            try
            {
                testExamType.Name = request.Name;
                testExamType.Description = request.Description;
                await _context.SaveChangesAsync();
                return new ApiResponse<TestExamTypeResponse>(0, "Update TestExamType success.")
                {
                    Data = ToTestExamType(testExamType)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TestExamTypeResponse>(1, "Update TestExamType error : " + ex);
            }
        }
        else
        {
            return new ApiResponse<TestExamTypeResponse>(1, "TestExamType does not exist");
        }
    }

}