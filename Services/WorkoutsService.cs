using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;

namespace Services
{
    public class WorkoutsService : IWorkoutsService
    {
        private readonly IWorkoutsRepository _workoutsRepository;
        private readonly ICurrentUserService _currentUserService;
        public WorkoutsService(IWorkoutsRepository workoutsRepository, ICurrentUserService currentUserService)
        {
            _workoutsRepository = workoutsRepository;
            _currentUserService = currentUserService;
        }
        public Task AddWorkout(WorkoutAddRequest request)
        {
            throw new NotImplementedException();
        }

        public Task AddWorkoutDuration(TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        public Task DeleteWorkout(int workoutId)
        {
            throw new NotImplementedException();
        }

        public Task<List<WorkoutResponse>> GetAllWorkouts(int userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateWorkoutNote(WorkoutUpdateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task UpdateWorkoutTitle(WorkoutUpdateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
