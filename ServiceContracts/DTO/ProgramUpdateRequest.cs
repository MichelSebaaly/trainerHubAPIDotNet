using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace ServiceContracts.DTO
{
    public class ProgramUpdateRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public IFormFile? File{ get; set; }
        public string? Equipment { get; set; }
        public string? Goal { get; set; }
    }
}
