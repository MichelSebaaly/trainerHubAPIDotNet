using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Data.Entities;
using Services.CustomExceptions;

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
        public async Task AddWorkout(WorkoutAddRequest request)
        {
            Workout workout = request.ToWorkout();
            await _workoutsRepository.AddWorkout(workout);
        }

        public Task AddWorkoutDuration(TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        public Task DeleteWorkout(int workoutId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<WorkoutResponse>> GetAllWorkouts(int? userId)
        {
            int? loggedInUser = _currentUserService.UserId;
            if (loggedInUser != userId)
            {
                throw new ForbiddenException();
            }
            List<Workout> workout = await _workoutsRepository.GetAllWorkouts(userId);
            return workout.Select(w => w.ToWorkoutResponse()).ToList();
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
