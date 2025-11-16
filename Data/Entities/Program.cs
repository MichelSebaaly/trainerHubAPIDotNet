using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Program
    {
        public int Id { get; set; }
        public int Trainer_Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? File_URL { get; set; }
        public string? Cover_Photo { get; set; }
        public string? Equipment {  get; set; }
        public string? Goal {  get; set; }
        public User? Trainer { get; set; }
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;
        public override string ToString()
        {
            return $"Title: {Title}, Price: {Price}, Equipment: {Equipment}";
        }
    }
}
