using Asana.Library.Models;
using Asana.Library.Util;
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
            // See ToDoServiceProxy: an unreachable API yields a null body.
            var projectData = new WebRequestHandler().Get("/Project").Result;
            _projectsList = Json.FromResponse<List<Project>>(projectData) ?? new List<Project>();
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
                return project;
            }
            var isNewProject = project.Id == 0;
            var projectData = new WebRequestHandler().Post("/Project", project).Result;
            var newProject = Json.FromResponse<Project>(projectData);

            if (newProject != null)
            {
                if (!isNewProject)
                {
                    var existingProject = _projectsList.FirstOrDefault(t => t.Id == newProject.Id);
                    if (existingProject != null)
                    {
                        var index = _projectsList.IndexOf(existingProject);
                        _projectsList.RemoveAt(index);
                        _projectsList.Insert(index, newProject);
                    }
                }
                else
                {
                    _projectsList.Add(newProject);
                }
            }
            return newProject;
        }

        public void DeleteProject(int id)
        {
            if (id == 0)
            {
                return;
            }
            var projectData = new WebRequestHandler().Delete($"/Project/{id}").Result;
            var projectToDelete = Json.FromResponse<Project>(projectData);
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

        /*
        public bool AddToDoToProject(int projectId, int toDoId)
        {
            var project = GetById(projectId);
            var toDo = ToDoServiceProxy.Current.GetById(toDoId);

            System.Diagnostics.Debug.WriteLine($"projectId: {projectId}, toDoId: {toDoId}, project: {project}, toDo: {toDo}");

            if (project == null || toDo == null)
                return false;

            project.ToDoList ??= new List<ToDo>();

            // Prevent duplicate ToDo by Id
            if (project.ToDoList.Any(t => t.Id == toDo.Id))
                return false;

            project.ToDoList.Add(toDo);

            // Persist the updated project to the backend/filebase
            var updatedProject = AddOrUpdate(project);

            // Return true if persistence succeeded
            return updatedProject != null;
        }
        */

        public bool AddToDoToProject(int pId, int toDoId)
        {
            // Step 1: Get the project and ToDo objects
            var project = GetById(pId);
            var toDo = ToDoServiceProxy.Current.GetById(toDoId);

            // Step 2: If either is null, return false
            if (project == null || toDo == null)
            {
                return false;
            }

            // Step 3: Send ToDo to server and get the updated ToDo object
            var todoData = new WebRequestHandler().Post("/ToDo", toDo).Result;
            var newToDo = Json.FromResponse<ToDo>(todoData);

            // Step 4: Add the new ToDo to the project's ToDoList if not already present
            if (newToDo != null)
            {
                project.ToDoList ??= new List<ToDo>();
                if (!project.ToDoList.Any(td => td.Id == newToDo.Id))
                {
                    project.ToDoList.Add(newToDo);
                    return true;
                }
            }
            return false;
        }


        public bool RemoveToDoFromProject(int projectId, int toDoId) // For CLI
        {
            var project = GetById(projectId);
            var toDo = ToDoServiceProxy.Current.GetById(toDoId);

            if (project == null || toDo == null)
                return false;

            project.ToDoList ??= new List<ToDo>();

            // Prevent duplicate ToDo by Id
            if (!project.ToDoList.Any(t => t.Id == toDo.Id))
                return false; // ToDo not found, nothing to remove

            project.ToDoList.Remove(toDo);

            // Persist the updated project to the backend/filebase
            var updatedProject = AddOrUpdate(project);

            // Return true if persistence succeeded
            return updatedProject != null;
        }
    }
}



