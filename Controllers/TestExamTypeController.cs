﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestExamTypeController : ControllerBase
    {
        private readonly ITestExamTypeService _service;
        public TestExamTypeController(ITestExamTypeService service)
        {
            _service = service;
        }
        [HttpGet]
        public Task<ApiResponse<List<TestExamTypeResponse>>> GetAll()
        {

            return _service.GetAll();
        }
        [HttpPost]
        public Task<ApiResponse<TestExamTypeResponse>> Create(TestExamTypeRequest request)
        {
            return _service.Create(request);
        }
        [HttpPut("{id}")]
        public Task<ApiResponse<TestExamTypeResponse>> Update(int id,TestExamTypeRequest request)
        {
            return _service.Update(id, request);
        } 
        [HttpDelete("{id}")]
        public Task<ApiResponse<TestExamTypeResponse>> Delete(int id)     
        {
            return _service.Delete(id);
        }    
        [HttpGet("{id}")]
        public Task<ApiResponse<TestExamTypeResponse>> Search(int id)     
        {
            return _service.Search(id);
        }
        
    }
}
