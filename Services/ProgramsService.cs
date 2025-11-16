using Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Services
{
    public class ProgramsService : IProgramsService
    {
        private readonly IProgramsRepository _programsRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProgramsService(IProgramsRepository programsRepository, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _programsRepository = programsRepository;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task AddProgram(ProgramAddRequest request)
        {
            string file_name = string.Empty;
            string photo_name = string.Empty;
            if (request.File_URL != null && request.File_URL.Length > 0)
            {
                file_name = $"{Guid.NewGuid()}_{request.File_URL.FileName}";

                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string filePath = Path.Combine(uploadsFolder, file_name);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File_URL.CopyToAsync(stream);
                }
            }

            if (request.Cover_Photo != null && request.Cover_Photo.Length > 0)
            {
                photo_name = $"{Guid.NewGuid()}_{request.Cover_Photo.FileName}";

                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string filePath = Path.Combine(uploadsFolder, photo_name);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Cover_Photo.CopyToAsync(stream);
                }
            }

            Program program = request.ToProgram(file_name, photo_name);
            await _programsRepository.AddProgram(program);
        }

        public async Task<bool> DeleteProgram(int programId)
        {
            int trainerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            Program? programToDelete = await _programsRepository.GetProgramById(programId);
            if (programToDelete == null)
            {
                return false;
            }

            if (programToDelete.Trainer_Id != trainerId)
            {
                throw new UnauthorizedAccessException("You cannot delete a program you do not own");
            }

            if (!string.IsNullOrEmpty(programToDelete.File_URL))
            {
                string filePath = Path.Combine(_env.WebRootPath, "uploads", programToDelete.File_URL);
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            if (!string.IsNullOrEmpty(programToDelete.Cover_Photo))
            {
                string filePath = Path.Combine(_env.WebRootPath, "uploads", programToDelete.Cover_Photo);
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            return await _programsRepository.DeleteProgram(programToDelete);
        }

        public async Task<string> DownloadProgramFile(int programId)
        {
            return await _programsRepository.DownloadProgramFile(programId);
        }

        public async Task<List<ProgramResponse>> GetAllPrograms()
        {
            return await _programsRepository.GetAllPrograms();
        }

        public async Task<string> UpdateCoverPhoto(int programID, int trainerId ,IFormFile? coverPhoto)
        {
            Program? program = await _programsRepository.GetProgramById(programID);
            if (program == null)
            {
                return "Program not found";
            }

            if (program.Trainer_Id != trainerId)
            {
                return "Not allowed";
            }

            if (coverPhoto != null && coverPhoto.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                string file_name = $"{Guid.NewGuid()}_{coverPhoto.FileName}";
                string photoLocation = Path.Combine(uploadsFolder, file_name);

                using (FileStream stream = new FileStream(photoLocation, FileMode.Create))
                {
                    await coverPhoto.CopyToAsync(stream);
                }
                program.Cover_Photo = file_name;
                program.updatedAt = DateTime.UtcNow;

                return file_name;
            }


            await _programsRepository.SaveChanges();

            return "No File Provided";
        }

        public async Task<bool> UpdateProgramInfo(int programId, int trainerId, ProgramUpdateRequest request)
        {
            string file_name = string.Empty;
            Program programToUpdate = await _programsRepository.GetProgramById(programId);

            if (programToUpdate == null)
            {
                throw new Exception("Program not found");
            }

            if (programToUpdate.Trainer_Id != trainerId)
            {
                return false;
            }

            if (request.File != null && request.File.Length > 0)
            {
                if (!string.IsNullOrEmpty(programToUpdate.File_URL))
                {
                    string filePathToDelete = Path.Combine(_env.WebRootPath, "uploads", programToUpdate.File_URL);
                    if (File.Exists(filePathToDelete))
                        File.Delete(filePathToDelete);
                }
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                file_name = $"{Guid.NewGuid()}_{request.File.FileName}";
                string filePath = Path.Combine(uploadsFolder, file_name);

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }
                programToUpdate.File_URL = file_name;
            }
            programToUpdate.Title = request.Title;
            programToUpdate.Description = request.Description;
            programToUpdate.Price = request.Price;
            programToUpdate.Equipment = request.Equipment;
            programToUpdate.Goal = request.Goal;
            programToUpdate.updatedAt = DateTime.UtcNow;
            await _programsRepository.SaveChanges();
            return true;
        }
    }
}
