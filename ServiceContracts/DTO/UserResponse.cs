using System;
using System.Collections.Generic;
using Data.Entities;

namespace ServiceContracts.DTO
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone_number { get; set; } = string.Empty;
        public string? Profile_pic { get; set; }
        public string Role { get; set; } = string.Empty;
    }
    public static class UserExtensions
    {
        public static UserResponse ToUserResponse(this User user)
        {
            return new UserResponse()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone_number = user.Phone_number,
                Profile_pic = user.Profile_pic,
                Role = user.Role,
            };
        }
    }
}
