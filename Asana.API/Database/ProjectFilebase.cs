using Asana.Library.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Asana.API.Database
{
    public class ProjectFilebase
    {
        private static ProjectFilebase? _instance;


        public static ProjectFilebase Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ProjectFilebase();
                }

                return _instance;
            }
        }

        // Storage locations come from FileStorage, configured at startup.
        private ProjectFilebase()
        {
        }

        public int LastKey
        {
            get
            {
                if (Projects.Any())
                {
                    return Projects.Select(x => x.Id).Max();
                }
                return 0;
            }
        }

        public Project AddOrUpdate(Project project)
        {
            if (project.Id <= 0)
            {
                project.Id = LastKey + 1;
            }

            string path = Path.Combine(FileStorage.ProjectRoot, $"{project.Id}.json");

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var projectToSave = new Project
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                IsCompleted = project.IsCompleted,
                CompletePercent = project.CompletePercent
                // ToDoList is intentionally excluded
            };

            File.WriteAllText(path, JsonConvert.SerializeObject(projectToSave));

            return project;
        }


        public List<Project> Projects
        {
            get
            {
                var root = new DirectoryInfo(FileStorage.ProjectRoot);
                var _projects = new List<Project>();
                foreach (var projectFile in root.GetFiles())
                {
                    var project = JsonConvert
                        .DeserializeObject<Project>
                        (File.ReadAllText(projectFile.FullName));
                    if (project != null)
                    {
                        _projects.Add(project);
                    }

                }
                return _projects;
            }
        }
        public bool Delete(string type, string id)
        {
            if (int.TryParse(id, out int idInt) && type == "Project")
            {
                string path = Path.Combine(FileStorage.ProjectRoot, $"{idInt}.json");
                if (File.Exists(path))
                {
                    File.Delete(path);
                    return true;
                }
            }
            return false;
        }

        public void UpdateProjectToDos(int projectId, List<ToDo> toDos)
        {
            var project = Projects.FirstOrDefault(p => p.Id == projectId);
            if (project != null)
            {
                project.ToDoList = toDos;
                AddOrUpdate(project); // Persist the updated project
            }
        }
    }



}
