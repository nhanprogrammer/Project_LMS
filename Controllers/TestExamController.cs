using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestExamController : ControllerBase
    {
        private readonly ITestExamService _testExamService;
        public TestExamController(ITestExamService testExamService)
        {
            _testExamService = testExamService;
        }
        [HttpGet]
        public Task<ApiResponse<List<TestExamResponse>>> getAll()
        {
            return _testExamService.GetAll();
        }
        [HttpPost]
        public Task<ApiResponse<TestExamResponse>> Create(TestExamRequest request)
        {
            return _testExamService.Create(request);
        }
        [HttpPut("{id}")]
        public Task<ApiResponse<TestExamResponse>> Update(int id, TestExamRequest request)
        {
            return _testExamService.Update(id, request);
        }
        [HttpDelete("{id}")]
        public Task<ApiResponse<TestExamResponse>> Delete(int id)
        {
            return _testExamService.Delete(id);
        }
        [HttpGet("{id}")]
        public Task<ApiResponse<TestExamResponse>> Search(int id)
        {
            return _testExamService.Search(id);
        }
    }
}
