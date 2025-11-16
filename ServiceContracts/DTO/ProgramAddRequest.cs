using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Data.Entities;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts.DTO
{
    public class ProgramAddRequest
    {
        [Required()]
        public int Trainer_Id { get; set; }
        
        [Required(ErrorMessage = "Title should not be empty")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description should be provided")]
        public string Description { get; set; } = string.Empty;
        
        public decimal Price { get; set; }
        public IFormFile? File_URL { get; set; }
        public IFormFile? Cover_Photo { get; set; }
        public string? Equipment { get; set; }
        public string? Goal { get; set; }

        public Program ToProgram(string? file_url, string? cover_photo)
        {
            return new Program()
            {
                Trainer_Id = Trainer_Id,
                Title = Title,
                Description = Description,
                Price = Price,
                File_URL = file_url,
                Cover_Photo = cover_photo,
                Equipment = Equipment,
                Goal = Goal,
            };
        }

        public override string ToString()
        {
            return $"Title: {Title}, File: {File_URL}, CoverPhoto: {Cover_Photo}";
        }
    }
}
