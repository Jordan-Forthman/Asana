using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asana.Library.Models
{
    public class Project
    {
        public Project()
        {
            Id = 0;
            IsCompleted = false;
        }
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsCompleted { get; set; }
        public int? CompletePercent { get; set; }

        public List<ToDo>? ToDoList { get; set; }

        // Without this the CLI's project listing printed
        // "Asana.Library.Models.Project" for every row, since it just
        // Console.WriteLine's each item. ToDo already had an override.
        public override string ToString()
        {
            var percent = CompletePercent.HasValue ? $" ({CompletePercent}% complete)" : string.Empty;
            return $"[{Id}] {Name} - {Description}{percent}";
        }
    }
}
