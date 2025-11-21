using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.CustomExceptions;

namespace TrainerHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutController : Controller
    {
        private readonly IWorkoutsService _workoutsService;
        private readonly ICurrentUserService _currentUserService;
        public WorkoutController(IWorkoutsService workoutsService, ICurrentUserService currentUserService)
        {
            _workoutsService = workoutsService;
            _currentUserService = currentUserService;
        }

        [HttpGet("getAllWorkouts")]
        [Authorize]
        public async Task<IActionResult> GetAllWorkouts()
        {
            int? userId = _currentUserService.UserId;
            List<WorkoutResponse> workouts = await _workoutsService.GetAllWorkouts(userId);
            return Ok(workouts);

        }

        [HttpPost("addWorkout")]
        [Authorize]

        public async Task<IActionResult> AddWorkout(WorkoutAddRequest request)
        {
            await _workoutsService.AddWorkout(request);
            return Ok("Workout addedd");
        }

        [HttpDelete("deleteWorkout/{workoutId}")]
        [Authorize]
        public async Task<IActionResult> DeleteWorkout(int workoutId)
        {
            bool isDeleted = await _workoutsService.DeleteWorkout(workoutId);
            if (isDeleted)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(500, "Failed To Delete Workout");
            }
        }
        [HttpPatch("addWorkoutDuration/{workoutId}")]
        [Authorize]
        public async Task<IActionResult> AddWorkoutDuration(int workoutId, WorkoutAddDurationRequest request)
        {
            await _workoutsService.AddWorkoutDuration(workoutId, request);
            return Ok("Workout Finished");
        }
    }
}
