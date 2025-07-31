using Asana.Library.Models;
using Asana.Maui.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asana.Library.Services
{
    public  class ProjectServiceProxy
    {
        private List<Project> _projectsList;
        public List<Project> Projects
        {
            get
            {
                return _projectsList.ToList();
            }
            private set // For CLI
            {
                if (value != _projectsList)
                {
                    _projectsList = value;
                }
            }
        }
        private ProjectServiceProxy() 
        {
            var projectData = new WebRequestHandler().Get("/Project/Expand").Result;
            _projectsList = JsonConvert.DeserializeObject<List<Project>>(projectData) ?? new List<Project>();
        }

        private static object _lock = new object();
        private static ProjectServiceProxy? instance;
        public static ProjectServiceProxy Current
        {
            get
            {
                lock (_lock) {
                    if (instance == null)
                    {
                        instance = new ProjectServiceProxy();
                    }
                }
                return instance;
            }
        }

        private int nextKey // For CLI
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

        public Project? GetById(int id) // For CLI
        {
            return Projects.FirstOrDefault(t => t.Id == id);
        }

        public Project? AddOrUpdate(Project? project) // For CLI
        {
            if (project != null)
            {
                if (project.Id == 0)
                {
                    project.Id = nextKey;
                    project.ToDoList ??= new List<ToDo>();
                    _projectsList.Add(project);
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
                        existing.ToDoList = project.ToDoList ?? existing.ToDoList;
                    }
                    else
                    {
                        project.ToDoList ??= new List<ToDo>();
                        _projectsList.Add(project);
                    }
                }
            }
            return project;
        }

        public void DeleteProject(int id)
        {
            if (id == 0)
            {
                return;
            }
            var projectData = new WebRequestHandler().Delete($"/Project/{id}").Result;
            var projectToDelete = JsonConvert.DeserializeObject<Project>(projectData);
            if (projectToDelete != null)
            {
                var localProject = _projectsList.FirstOrDefault(t => t.Id == projectToDelete.Id);
                if (localProject != null)
                {
                    _projectsList.Remove(localProject);
                }
            }

        }
        public void DisplayProjects() // For CLI
        {
            Projects.ForEach(Console.WriteLine);
        }

        public bool AddToDoToProject(int projectId, int toDoId) // For CLI
        {
            var project = GetById(projectId);
            var toDo = ToDoServiceProxy.Current.GetById(toDoId);
            if (project != null && toDo != null)
            {
                project.ToDoList ??= new List<ToDo>();
                if (!project.ToDoList.Contains(toDo))
                {
                    project.ToDoList.Add(toDo);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveToDoFromProject(int projectId, int toDoId) // For CLI
        {
            var project = GetById(projectId);
            var toDo = ToDoServiceProxy.Current.GetById(toDoId);
            if (project != null && toDo != null && project.ToDoList != null)
            {
                return project.ToDoList.Remove(toDo);
            }
            return false;
        }
    }
}
