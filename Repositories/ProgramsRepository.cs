using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class ProgramsRepository : IProgramsRepository
    {
        private readonly AppDbContext _db;
        public ProgramsRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task<Program> AddProgram(Program program)
        {
            _db.Add(program);
            await _db.SaveChangesAsync();
            return program;
        }

        public async Task<bool> DeleteProgram(Program programToDelete)
        {
            _db.programs.Remove(programToDelete);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<string> DownloadProgramFile(int programId)
        {
            return await _db.programs.Where(p => p.Id == programId).Select(p => p.File_URL).FirstOrDefaultAsync();
        }

        public async Task<List<ProgramResponse>> GetAllPrograms()
        {
            return await _db.programs.Select(p => new ProgramResponse
            {
                Id = p.Id,
                Trainer_Id = p.Trainer_Id,
                Title = p.Title,
                Description = p.Description,
                Price = p.Price,
                File_URL = p.File_URL,
                Cover_Photo = p.Cover_Photo,
                Equipment = p.Equipment,
                Goal = p.Goal,
                TrainerName = p.Trainer.Name,
                TrainerProfilePic = p.Trainer.Profile_pic

            }).ToListAsync();
        }

        public async Task<Program> GetProgramById(int id)
        {
            return await _db.programs.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task SaveChanges()
        {
            await _db.SaveChangesAsync();
        }
    }
}
