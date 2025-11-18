using System;
using System.Collections.Generic;
using Data.Entities;
namespace RepositoryContracts
{
    public interface IWorkoutsRepository
    {
        public Task<List<Workout>> GetAllWorkouts(int? userId);
        public Task<Workout> GetWorkoutById(int id);
        public Task AddWorkout(Workout workout);
        public Task<bool> DeleteWorkout(Workout workout);
        public Task SaveChangesAsync();
    }
}
