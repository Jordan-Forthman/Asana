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
        private List<Project> projects;
        public List<Project> Projects
        {
            get
            {
                return projects;
            }
            private set // For CLI
            {
                if (value != projects)
                {
                    projects = value;
                }
            }
        }
        private ProjectServiceProxy() {
            /*projects = new List<Project>
            {
                new Project{Id = 1, Name = "Project 1"},
                new Project{Id = 2, Name = "Project 2"},
                new Project{Id = 3, Name = "Project 3"}
            }; */
            var projectData = new WebRequestHandler().Get("/Project/Expand").Result;
            projects = JsonConvert.DeserializeObject<List<Project>>(projectData) ?? new List<Project>();
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
                    projects.Add(project);
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
                        projects.Add(project);
                    }
                }
            }
            return project;
        }

        public void DeleteProject(Project? project) // For CLI
        {
            if (project == null)
            {
                return;
            }
            projects.Remove(project);
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
