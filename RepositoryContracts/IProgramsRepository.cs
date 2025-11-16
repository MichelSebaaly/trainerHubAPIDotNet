using System;
using System.Collections.Generic;
using Data.Entities;
using ServiceContracts.DTO;

namespace RepositoryContracts
{
    public interface IProgramsRepository
    {
        Task<Program> GetProgramById(int id);
        Task<List<ProgramResponse>> GetAllPrograms();
        Task<Program> AddProgram(Program program);
        Task<string> DownloadProgramFile(int programId);
        Task SaveChanges();
        //Task UpdateProgramInfo();
        //Task<string> UpdateCoverPhoto(int programID, string photoLocation);
        Task<bool> DeleteProgram(Program program);
    }
}
