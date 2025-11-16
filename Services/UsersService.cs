using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Npgsql.Internal;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services
{
    public class UsersService : IUserService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly ITokenService _tokenService;

        public UsersService(IUsersRepository usersRepository, ITokenService tokenService)
        {
            _usersRepository = usersRepository;
            _tokenService = tokenService;
        }

        public async Task ChangePassword(int id, ChangePasswordRequest request)
        {
            User? user = await _usersRepository.GetUserById(id);

            if (user == null)
            {
                throw new ArgumentException("User not found");
            }
            bool matchedPasswords = BCrypt.Net.BCrypt.Verify(request.oldPassword, user.Password);
            if (matchedPasswords)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.newPassword);
                await _usersRepository.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Wrong Password");
            }
        }

        public async Task<List<UserResponse>> GetAllUsers()
        {
            List<User> users = await _usersRepository.GetAllUsers();
            return users.Select(u => u.ToUserResponse()).ToList();
        }

        public async Task<LoginResponse?> Login(LoginRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            User? user = await _usersRepository.GetUserByEmail(request.Email);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!isPasswordValid)
            {
                throw new ArgumentException("Invalid Credentials");
            }
            string accesstoken = await _tokenService.GenerateJwtToken(user);

            return new LoginResponse()
            {
                Token = accesstoken,
            };
        }

        public async Task<UserResponse> Register(UserAddRequest userAddRequest)
        {
            if (userAddRequest == null)
            {
                throw new ArgumentNullException(nameof(userAddRequest));
            }
            User user = userAddRequest.ToUser();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Password = hashedPassword;

            await _usersRepository.Register(user);

            return user.ToUserResponse();
        }

        public async Task UpdateProfilePicture(int id, string imageURL)
        {
            var user = await _usersRepository.GetUserById(id);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            user.Profile_pic = imageURL;
            await _usersRepository.SaveChanges();
        }

        public async Task UpdateUserInfo(int id, UserUpdateRequest updateRequest)
        {
            if (updateRequest == null)
                throw new ArgumentNullException(nameof(updateRequest));

            User? user = await _usersRepository.GetUserById(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} was not found.");

            user.Name = updateRequest.Name;
            user.Email = updateRequest.Email;
            user.Phone_number = updateRequest.Phone_number;

            await _usersRepository.SaveChanges();
        }
    }
}
