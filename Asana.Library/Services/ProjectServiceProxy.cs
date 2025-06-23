using Asana.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asana.Library.Services
{
    public class ProjectServiceProxy
    {
        private List<Projects> _projectList;

        public List<Projects> Projects
        {
            get
            {
                return _projectList.Take(100).ToList();
            }
            private set
            {
                if (value != _projectList)
                {

                }
                _projectList = value;
            }
        }

        private ProjectServiceProxy()
        {
            Projects = new List<Projects>
            {
                new Projects { Id = 1, Name = "Project Alpha", Description = "First project", CompletePercent = 50 },
                new Projects { Id = 2, Name = "Project Beta", Description = "Second project", CompletePercent = 20 },
                new Projects { Id = 3, Name = "Project Gamma", Description = "Third project", CompletePercent = 80 }
            };
        }

        private static ProjectServiceProxy? instance;

        private int nextKey
        {
            get
            {
                if (Projects.Any())
                {
                    return Projects.Select(p => p.Id).Max() + 1;
                }
                return 1;
            }
        }

        public static ProjectServiceProxy Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new ProjectServiceProxy();
                }
                return instance;
            }
        }

        public Projects? GetById(int id)
        {
            return Projects.FirstOrDefault(t => t.Id == id);
        }

        public Projects? AddOrUpdate(Projects? project)
        {
            if (project != null)
            {
                if (project.Id == 0)
                {
                    project.Id = nextKey;
                    project.ToDos ??= new List<ToDo>();
                    _projectList.Add(project);
                }
                else
                {
                    var existing = GetById(project.Id);
                    if (existing != null)
                    {
                        existing.Name = project.Name;
                        existing.Description = project.Description;
                        existing.CompletePercent = project.CompletePercent;
                        // Preserve existing ToDos list
                        existing.ToDos = project.ToDos ?? existing.ToDos;
                    }
                    else
                    {
                        project.ToDos ??= new List<ToDo>();
                        _projectList.Add(project);
                    }
                }
            }
            return project;
        }

        public void DeleteProject(Projects? project)
        {
            if (project == null)
            {
                return;
            }
            _projectList.Remove(project);
        }
        public void DisplayProjects()
        {
            Projects.ForEach(Console.WriteLine);
        }

        public bool AddToDoToProject(int projectId, int toDoId)
        {
            var project = GetById(projectId);
            var toDo = ToDoServiceProxy.Current.GetById(toDoId);
            if (project != null && toDo != null)
            {
                project.ToDos ??= new List<ToDo>();
                if (!project.ToDos.Contains(toDo))
                {
                    project.ToDos.Add(toDo);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveToDoFromProject(int projectId, int toDoId)
        {
            var project = GetById(projectId);
            var toDo = ToDoServiceProxy.Current.GetById(toDoId);
            if (project != null && toDo != null && project.ToDos != null)
            {
                return project.ToDos.Remove(toDo);
            }
            return false;
        }
    }
}
