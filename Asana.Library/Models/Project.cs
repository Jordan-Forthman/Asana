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
    }
}
