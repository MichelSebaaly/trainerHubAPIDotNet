using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using System.Security.Claims;

namespace TrainerHubAPI.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _env;

        public UsersController(IUserService userService, IWebHostEnvironment env)
        {
            _userService = userService;
            _env = env;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserAddRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                UserResponse response = await _userService.Register(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Could not register {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                LoginResponse? response = await _userService.Login(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _userService.GetAllUsers());
        }

        [Authorize]
        [HttpPut("updateProfilePic")]
        public async Task<IActionResult> UpdateProfilePic([FromForm] UserUpdateProfilePicRequest request)
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (userId == 0)
            {
                return Unauthorized("User ID not found in token.");
            }

            if (request.Profile_pic == null || request.Profile_pic.Length == 0)
            {
                return BadRequest("No file provided");
            }
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "profile");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Create a unique filename
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(request.Profile_pic.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save the file
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Profile_pic.CopyToAsync(stream);
            }

            await _userService.UpdateProfilePicture(userId, uniqueFileName);
            string imageURL = $"{Request.Scheme}://{Request.Host}/uploads/profile/{uniqueFileName}";

            return Ok(new { profile_pic = imageURL });
        }

        [Authorize]
        [HttpPut("updateUserInfo")]
        public async Task<IActionResult> UpdateUserinfo([FromBody] UserUpdateRequest request)
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId == 0)
            {
                return Unauthorized("User ID not found in token.");
            }
            try
            {
                await _userService.UpdateUserInfo(userId, request);
                return Ok(new { info = request });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId == 0)
            {
                return Unauthorized("User ID not found in token.");
            }
            try
            {
                await _userService.ChangePassword(userId, request);
                return Ok(new { message = "Password Changed" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
