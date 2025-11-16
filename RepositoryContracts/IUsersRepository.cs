using Data.Entities;
using ServiceContracts.DTO;

namespace RepositoryContracts
{
    public interface IUsersRepository
    {
        Task Register(User user);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserById(int id);
        Task<List<User>> GetAllUsers();
        Task SaveChanges();
    }
}
