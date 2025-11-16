using Data.Entities;
using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;

namespace ServiceContracts
{
    public interface IProgramsService
    {
        Task<List<ProgramResponse>> GetAllPrograms();
        Task AddProgram(ProgramAddRequest request, string? role);
        Task<string> DownloadProgramFile(int programId);
        Task<bool> UpdateProgramInfo(int programId, int trainerId, ProgramUpdateRequest request);
        Task<string> UpdateCoverPhoto(int programID, int trainerId, IFormFile coverPhoto);
        Task<bool> DeleteProgram(int programId);
    }
}
