using System;
using System.Collections.Generic;
using System.Text;

namespace CleanMonolith.Application.Interfaces
{
    public interface IAIService
    {
        Task<string> GetResponse(string prompt);
    }
}
