using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class UserUpdateProfilePicRequest
    {
        public IFormFile? Profile_pic {  get; set; }
    }
}
