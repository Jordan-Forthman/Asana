using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asana.Library.Models
{
    public class Projects
    {
        public Projects() 
        {
            Id = 0;
            IsCompleted = false;
            CompletePercent = 0;
        }
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsCompleted { get; set; }
        public int? CompletePercent { get; set; }

        public List<ToDo>? ToDos { get; set; }

        public override string ToString()
        {
            return $"[{Id}] {Name} - {Description}";
        }
    }
}
