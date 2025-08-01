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
    public class ProjectServiceProxy
    {
        private List<Project> _projectsList;
        public List<Project> Projects
        {
            get
            {
                return _projectsList.ToList();
            }
            private set
            {
                if (value != _projectsList)
                {
                    _projectsList = value;
                }
            }
        }
        private ProjectServiceProxy()
        {
            var projectData = new WebRequestHandler().Get("/Project").Result;

            //var projectData = new WebRequestHandler().Get("/Project/Expand").Result;
            _projectsList = JsonConvert.DeserializeObject<List<Project>>(projectData) ?? new List<Project>();
        }

        private static object _lock = new object();
        private static ProjectServiceProxy? instance;
        public static ProjectServiceProxy Current
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new ProjectServiceProxy();
                    }
                }
                return instance;
            }
        }

        public Project? GetById(int id)
        {
            return Projects.FirstOrDefault(t => t.Id == id);
        }

        // Improved AddOrUpdate method
        public Project? AddOrUpdate(Project? project)
        {
            if (project == null)
            {
                return null;
            }

            var isNewProject = project.Id == 0;
            var projectData = new WebRequestHandler().Post("/Project", project).Result;

            // Log the response for debugging
            Console.WriteLine($"[AddOrUpdate] POST /Project response: {projectData}");

            if (!string.IsNullOrEmpty(projectData) && !projectData.StartsWith("Error:"))
            {
                var updatedProject = JsonConvert.DeserializeObject<Project>(projectData);

                if (updatedProject != null)
                {
                    if (!isNewProject)
                    {
                        var existingProject = _projectsList.FirstOrDefault(p => p.Id == updatedProject.Id);
                        if (existingProject != null)
                        {
                            var index = _projectsList.IndexOf(existingProject);
                            _projectsList.RemoveAt(index);
                            _projectsList.Insert(index, updatedProject);
                        }
                    }
                    else
                    {
                        _projectsList.Add(updatedProject);
                    }
                    // Return the actual updated project (with correct ID)
                    return updatedProject;
                }
                else
                {
                    Console.WriteLine($"[AddOrUpdate] Server error: Could not deserialize response: {projectData}");
                }
            }
            else
            {
                Console.WriteLine($"[AddOrUpdate] Error or empty response from backend: {projectData}");
            }
            return null;
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
