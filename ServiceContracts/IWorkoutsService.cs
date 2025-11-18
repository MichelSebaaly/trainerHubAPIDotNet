using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IWorkoutsService
    {
        public Task<List<WorkoutResponse>> GetAllWorkouts(int userId);
        public Task AddWorkout(WorkoutAddRequest request);
        public Task AddWorkoutDuration(TimeSpan duration);
        public Task UpdateWorkoutNote(WorkoutUpdateRequest request);
        public Task UpdateWorkoutTitle(WorkoutUpdateRequest request);
        public Task DeleteWorkout(int workoutId);
    }
}
