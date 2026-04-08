using System;
using System.Collections.Generic;
using System.Text;

namespace CleanMonolith.Application.DTOs
{
    public class UpdatePasswordDto
    {
        public string LoginId { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
