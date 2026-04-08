using System;
using System.Collections.Generic;
using System.Text;

namespace CleanMonolith.Application.DTOs
{
    public class ChatRequest
    {
        public required string Message { get; set; }
    }

    public class ChatResponse
    {
        public required string Reply { get; set; }
    }
}
