using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Workout
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string? Notes { get; set; }
        public TimeSpan? Duration { get; set; }
        public User? User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
