﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;
using Project_LMS.Services;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachingAssignmentController : ControllerBase
    {
        private readonly ITeachingAssignmentService _service;

        public TeachingAssignmentController(ITeachingAssignmentService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result == null)
            {
                Console.WriteLine($"API GetById: Không tìm thấy TeachingAssignment với ID: {id}");
                return NotFound(new ApiResponse<object>(1, $"Không tìm thấy phân công giảng dạy với ID: {id}"));
            }
            return Ok(new ApiResponse<object>(0, "Tìm thấy phân công giảng dạy thành công!", result));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TeachingAssignmentRequestCreate request)
        {
            try
            {
                //Console.WriteLine($"Bắt đầu tạo phân công: UserId={request.UserId}, ClassId={request.ClassId}, SubjectId={request.SubjectId}");

                var response = await _service.Create(request);
                if (response != null)
                {
                    return Ok(new ApiResponse<object>(0, "Phân công giảng dạy đã được tạo!", response));
                }

                return BadRequest(new ApiResponse<object>(1, "Tạo phân công thất bại!"));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<object>(1, ex.Message));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiResponse<object>(1, ex.Message));
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Lỗi khi lưu vào database: {dbEx.InnerException?.Message ?? dbEx.Message}");
                return StatusCode(500,
                    new ApiResponse<object>(1, $"Lỗi khi lưu dữ liệu vào database: " + (dbEx.InnerException?.Message ?? dbEx.Message)));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(1, ex.Message));
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateById([FromBody] TeachingAssignmentRequestUpdate request)
        {
            try
            {
                var response = await _service.UpdateById(request);

                if (response != null)
                {
                    return Ok(new ApiResponse<object>(0, "Cập nhật thành công!", response));
                }

                return BadRequest(new ApiResponse<object>(1, "Cập nhật thất bại!"));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<object>(1, ex.Message));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiResponse<object>(1, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(1, ex.Message));
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteRequest request)
        {
            var success = await _service.Delete(request.ids);
            return success
                ? Ok(new ApiResponse<object>(0, "Xóa phân công giảng dạy thành công!"))
                : NotFound(new ApiResponse<object>(1, "Không tìm thấy phân công giảng dạy!"));
        }

        //Xem lại academicYear
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllTeachingAssignments(
           [FromQuery] int? academicYearId,
           [FromQuery] int? subjectGroupId,
           [FromQuery] int? userId,
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10)
        {
            try
            { var result = await _service.GetTeachingAssignments(academicYearId, subjectGroupId, userId, pageNumber, pageSize);
                return Ok(new ApiResponse<object>(0, "Lấy dữ liệu thành công!", result));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<object>(1, ex.Message));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiResponse<object>(1, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(1, ex.Message));
            }
           
        }

        [HttpGet("topics")]
        public async Task<IActionResult> GetTopicsByAssignment([FromQuery] int TeachingAssignmentId)
        {
            var topics = await _service.GetTopicsByAssignmentIdAsync(TeachingAssignmentId);

            if (topics == null || !topics.Any())
            {
                return BadRequest(new ApiResponse<object>(1, "Không tìm thấy dữ liệu chủ đề"));
            }

            return Ok(new ApiResponse<object>(0, "Lấy dữ liệu thành công!", topics));
        }
        [HttpGet("class/search")]
        public async Task<IActionResult> SearchClass([FromQuery] string? keyword)
        {
            var result = await _service.SearchClass(keyword);
            if (!result.Any() || result == null)
                return Ok(new ApiResponse<object>(1, "Không tìm thấy lớp học"));
            return Ok(new ApiResponse<object>(0, "Tìm thành công lớp học", result));
        }
    }
}

