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
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            int? userId = _currentUserService.UserId;
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }
            request.userId = userId.Value;

            Workout workout = request.ToWorkout();
            await _workoutsRepository.AddWorkout(workout);
            await _workoutsRepository.SaveChangesAsync();
        }

        public Task AddWorkoutDuration(TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteWorkout(int workoutId)
        {
            Workout workout = await _workoutsRepository.GetWorkoutById(workoutId);
            if (workout == null)
            {
                throw new NotFoundException();
            }
            int? userId = _currentUserService.UserId;
            if (workout.UserId != userId)
            {
                throw new ForbiddenException();
            }
            bool isDeleted = await _workoutsRepository.DeleteWorkout(workout);
            if (isDeleted)
            {
                await _workoutsRepository.SaveChangesAsync();
                return true;
            }else
            {
                return false;
            }
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
