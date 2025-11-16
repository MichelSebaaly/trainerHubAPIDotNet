using ServiceContracts.DTO;
using Data.Entities;

namespace ServiceContracts
{
    public interface IUserService
    {
        Task<UserResponse> Register(UserAddRequest userAddRequest);
        Task<LoginResponse?> Login(LoginRequest request);
        Task<List<UserResponse>> GetAllUsers();
        Task UpdateProfilePicture(int id, string imageURL);
        Task UpdateUserInfo(int id, UserUpdateRequest updateRequest);
        Task ChangePassword(int id, ChangePasswordRequest request);
    }
}
