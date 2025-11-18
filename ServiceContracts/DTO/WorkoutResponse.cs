using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class WorkoutResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public TimeSpan? Duration { get; set; }

        public override string ToString()
        {
            return $"Title: {Title}";
        }
    }
    public static class WorkoutExtensions
    {
        public static WorkoutResponse ToWorkoutResponse(this Workout workout)
        {
            return new WorkoutResponse
            {
                Id = workout.Id,
                UserId = workout.UserId,
                Title = workout.Title,
                Notes = workout.Notes,
                Duration = workout.Duration
            };

        }
    }
}
