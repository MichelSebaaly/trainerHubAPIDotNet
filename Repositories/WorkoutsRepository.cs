using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repositories
{
    public class WorkoutsRepository : IWorkoutsRepository
    {
        private readonly AppDbContext _db;
        public WorkoutsRepository(AppDbContext db)
        {
            _db     = db;
        }

        public async Task AddWorkout(Workout workout)
        {
            _db.workouts.Add(workout);
        }
        public async Task<bool> DeleteWorkout(Workout workout)
        {
            _db.workouts.Remove(workout);
            return true;
        }

        public async Task<List<Workout>> GetAllWorkouts(int? userId)
        {
            return await _db.workouts.Where(w => w.UserId == userId).ToListAsync();
        }

        public async Task<Workout> GetWorkoutById(int id)
        {
            return await _db.workouts.FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
