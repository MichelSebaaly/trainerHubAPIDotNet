using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone_number {  get; set; } = string.Empty;
        public string? Profile_pic { get; set; }
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;

        public override string ToString()
        {
            return $"Name: {Name}, Email: {Email}";
        }
    }
}
