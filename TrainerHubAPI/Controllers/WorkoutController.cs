using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.CustomExceptions;

namespace TrainerHubAPI.Controllers
{
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
            
            try
            {
                List<WorkoutResponse> workouts = await _workoutsService.GetAllWorkouts(userId);
                
                return Ok(workouts);
            }
            catch (ForbiddenException ex) 
            {
                return Forbid(ex.Message);
            }
        }
    }
}
