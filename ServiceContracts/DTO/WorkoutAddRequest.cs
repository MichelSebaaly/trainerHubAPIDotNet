using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Data.Entities;

namespace ServiceContracts.DTO
{
    public class WorkoutAddRequest
    {
        public int userId { get; set; }

        [Required(ErrorMessage = "Please workout title is required")]
        public string Title { get; set; }
        public string? Notes { get; set; }
        public Workout ToWorkout()
        {
            return new Workout { 
                UserId = userId,
                Title = Title, 
                Notes = Notes 
            };
        }
    }
}
