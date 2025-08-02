using Api.ToDoApplication.Persistence;
using Asana.API.Database;
using Asana.Library.Models;

namespace Asana.API.Enterprise
{
    public class ProjectEC
    {
        public IEnumerable<Project>? Get(bool Expand = false)
        {
            var projects = ProjectFilebase.Current.Projects;
            if (Expand)
            {
                var allToDos = ToDoFilebase.Current.ToDos;
                foreach (var project in projects)
                {
                    project.ToDoList = allToDos.Where(t => t.ProjectId == project.Id).ToList();
                }
            }
            return projects.Take(100);
        }

        public IEnumerable<Project> GetProjects()
        {
            return ProjectFilebase.Current.Projects.Take(100);
        }

        public Project? GetById(int id)
        {
            var project = ProjectFilebase.Current.Projects.FirstOrDefault(p => p.Id == id);
            if (project != null)
            {
                project.ToDoList = ToDoFilebase.Current.ToDos.Where(t => t.ProjectId == id).ToList();
            }
            return project;
        }

        public Project? AddOrUpdate(Project? project)
        {
            if (project == null)
            {
                return null;
            }
            ProjectFilebase.Current.AddOrUpdate(project);
            return project;
        }

        public Project? Delete(int id)
        {
            var projectToDelete = GetById(id);
            if (projectToDelete != null)
            {
                ProjectFilebase.Current.Delete("Project", id.ToString());
            }
            return projectToDelete;
        }

        public Project? AddToDoToProject(int projectId, ToDo toDo)
        {
            var project = GetById(projectId);
            if (project == null)
                return null;

            // Initialize ToDoList if null
            if (project.ToDoList == null)
                project.ToDoList = new List<ToDo>();

            // Add the ToDo
            project.ToDoList.Add(toDo);

            // Persist the updated project
            ProjectFilebase.Current.AddOrUpdate(project);
            return project;
        }
    }
}
