using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CleanMonolith.Application.Interfaces
{
    public interface IChatService
    {
        Task<string> GetReplyAsync(string message);
    }
}
