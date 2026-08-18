using Asana.API.Database;
using Asana.Library.Models;

namespace Asana.API.Enterprise
{
    public class ToDoEC
    {
        public ToDoEC() { 
            
        }

        public IEnumerable<ToDo> GetToDos()
        {
            return ToDoFilebase.Current.ToDos.Take(100);
        }

        public ToDo? GetById(int id)
        {
            return GetToDos().FirstOrDefault(t => t.Id == id);
        }

        public ToDo? Delete(int id)
        {
            var toDoToDelete = GetById(id);
            if (toDoToDelete != null)
            {
                ToDoFilebase.Current.Delete("ToDo", id.ToString());
            }
            return toDoToDelete;
        }

        public ToDo? AddOrUpdate(ToDo? toDo)
        {
            // A malformed or empty request body binds to null here. Passing it
            // through was dereferenced inside the filebase and surfaced as a
            // 500 rather than a handled response.
            if (toDo == null)
            {
                return null;
            }

            ToDoFilebase.Current.AddOrUpdate(toDo);
            return toDo;
        }
    }
}
