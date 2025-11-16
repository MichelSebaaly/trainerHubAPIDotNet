using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RepositoryContracts;
using ServiceContracts.DTO;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly AppDbContext _db;
        public UsersRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task<List<User>> GetAllUsers()
        {
            return await _db.users.ToListAsync();
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            User? user = await _db.users.FirstOrDefaultAsync(user => user.Email == email);
            return user;
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _db.users.FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task Register(User user)
        {
            _db.users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task SaveChanges()
        {
            await _db.SaveChangesAsync();
        }
    }
}
