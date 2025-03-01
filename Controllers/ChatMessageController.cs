using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatMessageController : ControllerBase
    {
        private readonly IChatMessagesService _service;

        public ChatMessageController(IChatMessagesService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ChatMessageResponse>>> GetById(int id)
        {
            try
            {
                var result = await _service.GetChatMessageById(id);
                if (result == null)
                {
                    return NotFound(new ApiResponse<ChatMessageResponse>(1, "Chat message not found", null));
                }
                return Ok(new ApiResponse<ChatMessageResponse>(0, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ChatMessageResponse>>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllChatMessages();
                return Ok(new ApiResponse<IEnumerable<ChatMessageResponse>>(0, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CreateChatMessageRequest>>> Add([FromBody] CreateChatMessageRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResponse<CreateChatMessageRequest>(1, "Invalid request", null));
                }

                return Ok(new ApiResponse<CreateChatMessageRequest>(0, "Chat message added successfully", request));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<UpdateChatMessageRequest>>> Update(int id, [FromBody] UpdateChatMessageRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResponse<UpdateChatMessageRequest>(1, "Invalid request", null));
                }

                return Ok(new ApiResponse<UpdateChatMessageRequest>(0, "Chat message updated successfully", request));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteChatMessage(id);
                if (!result)
                {
                    return NotFound(new ApiResponse<string>(1, "Chat message not found", null));
                }

                return Ok(new ApiResponse<string>(0, "Chat message deleted successfully", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }
    }
}
