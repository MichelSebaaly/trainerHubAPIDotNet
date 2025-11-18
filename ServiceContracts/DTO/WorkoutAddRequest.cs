using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class WorkoutAddRequest
    {

        public int userId { get; set; }

        [Required(ErrorMessage = "Please workout title is required")]
        public string Title { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
