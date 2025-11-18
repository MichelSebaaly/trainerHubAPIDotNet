using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class WorkoutUpdateRequest
    {
        public string? Title { get; set; }
        public string? Note { get; set; }
    }
}
