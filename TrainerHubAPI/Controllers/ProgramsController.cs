using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using System.Security.Claims;

namespace TrainerHubAPI.Controllers
{
    [Route("api/[controller]")]
    public class ProgramsController : Controller
    {
        private readonly IProgramsService _programsService;

        public ProgramsController(IProgramsService programsService)
        {
            this._programsService = programsService;
        }

        [HttpGet("getAllPrograms")]
        public async Task<IActionResult> GetAllPrograms()
        {
            List<ProgramResponse> programs = await _programsService.GetAllPrograms();
            return Ok(programs);
        }

        [Authorize(Roles = "trainer")]
        [HttpPost("addProgram")]
        public async Task<IActionResult> AddProgram([FromForm] ProgramAddRequest request)
        {
            int trainerId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            string? role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (trainerId == 0)
            {
                return Unauthorized("User ID not found in token.");
            }
            request.Trainer_Id = trainerId;
            await _programsService.AddProgram(request, role);
            return Ok("Program Added");
        }
        [Authorize]
        [HttpGet("downloadProgramFile/{programId}")]
        public async Task<IActionResult> DownloadProgramFile(int programId)
        {
            string file = await _programsService.DownloadProgramFile(programId);

            if (file == null)
            {
                return NotFound("File Not Found");
            }

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", file);

            return PhysicalFile(filePath, "application/octet-stream", file);
        }
        [Authorize(Roles = "trainer")]
        [HttpDelete("deleteProgram/{programId}")]
        public async Task<IActionResult> DeleteProgram(int programId)
        {
            bool isDeleted = await _programsService.DeleteProgram(programId);
            if (isDeleted)
            {
                return Ok($"Program with id {programId} is deleted");
            }
            else
            {
                return BadRequest("Something wrong has happened");
            }
        }

        [Authorize(Roles = "trainer")]
        [HttpPut("updateProgramInfo/{programId}")]
        public async Task<IActionResult> UpdateProgramInfo(int programId, [FromForm] ProgramUpdateRequest request)
        {
            int trainerId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            bool isUpdated = await _programsService.UpdateProgramInfo(programId, trainerId, request);
            if (!isUpdated)
            {
                return Forbid("You are not allowed to update this program.");
            }

            return Ok(new { message = "Program updated successfully" });
        }

        [Authorize(Roles = "trainer")]
        [HttpPut("updateProgramCoverPhoto/{programId}")]
        public async Task<IActionResult> UpdateProgramCoverPhoto(int programId, [FromForm] ProgramCoverPhotoUpdateRequest request)
        {
            int trainerId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            string result = await _programsService.UpdateCoverPhoto(programId, trainerId, request.Cover_Photo);

            if (result.Equals("Not allowed"))
            {
                return Forbid("You are not allowed to update this program.");
            }

            if (result.Equals("No file provided"))
            {
                return BadRequest("No file uploaded.");
            }

            string imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/{result}";

            return Ok(new { cover_photo = imageUrl });
        }
    }
}
