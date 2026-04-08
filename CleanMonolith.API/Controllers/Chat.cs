using CleanMonolith.Application.DTOs;
using CleanMonolith.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanMonolith.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {

        private readonly IChatService _chatService;


        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("AIChatRequest")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            var reply = await _chatService.GetReplyAsync(request.Message);

            return Ok(new ChatResponse
            {
                Reply = reply
            });
        }
    }
}