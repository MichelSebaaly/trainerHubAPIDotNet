using System;
using System.Collections.Generic;
using Data.Entities;

namespace ServiceContracts.DTO
{
    public class ProgramResponse
    {
        public int Id { get; set; }
        public int Trainer_Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? File_URL { get; set; }
        public string? Cover_Photo { get; set; }
        public string? Equipment { get; set; }
        public string? Goal { get; set; }
        public string? TrainerName { get; set; }
        public string? TrainerProfilePic { get; set; }

    }

    public static class ProgramExtensions
    {
        public static ProgramResponse ToProgramResponse(this Program program)
        {
            return new ProgramResponse()
            {
                Id = program.Id,
                Trainer_Id = program.Trainer_Id,
                Title = program.Title,
                Description = program.Description,
                Price = program.Price,
                File_URL = program.File_URL,
                Cover_Photo = program.Cover_Photo,
                Equipment = program.Equipment,
                Goal = program.Goal,
                TrainerName = program.Trainer?.Name,
                TrainerProfilePic = program.Trainer?.Profile_pic
            };
        }
    }
}
