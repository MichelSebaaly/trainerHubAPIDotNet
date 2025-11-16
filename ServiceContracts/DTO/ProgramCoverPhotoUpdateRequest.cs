using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class ProgramCoverPhotoUpdateRequest
    {
        public IFormFile? Cover_Photo { get; set; }
    }
}
