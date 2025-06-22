using Asana.Library.Models;
using Asana.Library.Services;
using System;
using System.ComponentModel.Design;

namespace Asana
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            var toDos = new List<ToDo>();
            var projects = new List<Projects>();

            var toDoSvc = ToDoServiceProxy.Current;
            int choiceInt;
            do
            {
                Console.WriteLine("Choose a menu option:");
                Console.WriteLine("1. Create a ToDo");
                Console.WriteLine("2. List all ToDos");
                Console.WriteLine("3. List all outstanding ToDos");
                Console.WriteLine("4. Delete a ToDo");
                Console.WriteLine("5. Update a ToDo");
                Console.WriteLine("6. Create a Project");
                Console.WriteLine("7. List all Projects");
                Console.WriteLine("8. List all ToDos in Project");
                Console.WriteLine("9. Delete a Project");
                Console.WriteLine("10. Update a Project");
                Console.WriteLine("11. Add ToDos to a Project:");
                Console.WriteLine("12. Exit");

                var choice = Console.ReadLine() ?? "6";

                if (int.TryParse(choice, out choiceInt))
                {
                    switch (choiceInt)
                    {

                        case 1: // Create ToDo

                            Console.Write("Name:");
                            var name = Console.ReadLine();
                            Console.Write("Description:");
                            var description = Console.ReadLine();

                            toDoSvc.AddOrUpdate(new ToDo
                            {
                                Name = name,
                                Description = description,
                                IsCompleted = false,
                                Id = 0
                            });
                            break;

                        case 2: // List all ToDos
                            toDos.ForEach(Console.WriteLine);
                        case 2:
                            toDoSvc.DisplayToDos(true);
                            break;

                        case 3: // List all outstanding ToDos
                            toDos.Where(t => (t != null) && !(t?.IsCompleted ?? false))
                                .ToList()
                                .ForEach(Console.WriteLine);
                        case 3:
                            toDoSvc.DisplayToDos();
                            break;

                        case 4: // Delete a ToDo
                            toDos.ForEach(Console.WriteLine);
                        case 4:
                            toDoSvc.DisplayToDos(true);
                            Console.Write("ToDo to Delete: ");
                            toDoChoice = int.Parse(Console.ReadLine() ?? "0");

                            var reference = toDos.FirstOrDefault(t => t.Id == toDoChoice);
                            if (reference != null)
                            {
                                toDos.Remove(reference);
                            }
                            var toDoChoice4 = int.Parse(Console.ReadLine() ?? "0");

                            var reference = toDoSvc.GetById(toDoChoice4);
                            toDoSvc.DeleteToDo(reference);
                            break;

                        case 5: // Update a ToDo
                            toDos.ForEach(Console.WriteLine);
                        case 5:
                            toDoSvc.DisplayToDos(true);
                            Console.Write("ToDo to Update: ");
                            var toDoChoice5 = int.Parse(Console.ReadLine() ?? "0");
                            var updateReference = toDoSvc.GetById(toDoChoice5);

                            if (updateReference != null)
                            {
                                Console.Write("Name:");
                                updateReference.Name = Console.ReadLine();
                                Console.Write("Description:");
                                updateReference.Description = Console.ReadLine();
                            }

                            break;

                        case 6: // Create a Project
                            Console.Write("Name:");
                            name = Console.ReadLine();
                            Console.Write("Description:");
                            description = Console.ReadLine();

                            projects.Add(new Projects
                            {
                                Name = name,
                                Description = description,
                                Id = ++projectCount
                            });

                            break;

                        case 7: // List all Projects
                            projects.ForEach(Console.WriteLine);
                            break;

                        case 8: // List all ToDos in a Project
                            Console.Write("Enter Project ID: ");
                            var projectIdInput = Console.ReadLine();
                            if (int.TryParse(projectIdInput, out int projectId))
                            {
                                var project = projects.FirstOrDefault(p => p.Id == projectId);
                                if (project != null)
                                {
                                    if (project.ToDos != null && project.ToDos.Any())
                                    {
                                        project.ToDos.ForEach(Console.WriteLine);
                                    }
                                    else
                                    {
                                        Console.WriteLine("No ToDos found in this project.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Project with ID {projectId} not found.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("ERROR: Invalid Project ID.");
                            }
                            break;

                        case 9: // Delete a Project
                            projects.ForEach(Console.WriteLine);
                            Console.Write("Project to Delete: ");
                            projectChoice = int.Parse(Console.ReadLine() ?? "0");

                            var projectReference = projects.FirstOrDefault(p => p.Id == projectChoice);
                            if (projectReference != null)
                            {
                                projects.Remove(projectReference);
                            }
                            break;

                        case 10: // Update a Project
                            projects.ForEach(Console.WriteLine);
                            Console.Write("ToDo to Update: ");
                            projectChoice = int.Parse(Console.ReadLine() ?? "0");
                            var puReference = projects.FirstOrDefault(p => p.Id == projectChoice);

                            if (puReference != null)
                            {
                                Console.Write("Name:");
                                puReference.Name = Console.ReadLine();
                                Console.Write("Description:");
                                puReference.Description = Console.ReadLine();
                            }
                            toDoSvc.AddOrUpdate(updateReference);
                            break;

                        case 11: // Add ToDos to a Project
                            Console.Write("Enter Project ID: ");
                            var pIdInput = Console.ReadLine();
                            if (int.TryParse(pIdInput, out int pId))
                            {
                                var project = projects.FirstOrDefault(p => p.Id == pId);
                                if (project != null)
                                {
                                    if (projects.Any())
                                    {
                                        Console.WriteLine("Available ToDos:");
                                        toDos.ForEach(Console.WriteLine);
                                        Console.Write("Enter ToDo ID to add to project: ");
                                        var toDoIdInput = Console.ReadLine();
                                        if (int.TryParse(toDoIdInput, out int toDoId))
                                        {
                                            var toDo = toDos.FirstOrDefault(t => t.Id == toDoId);
                                            if (toDo != null)
                                            {
                                                // Initialize ToDos list if null
                                                project.ToDos ??= new List<ToDo>();
                                                // Add ToDo to project's ToDos list if not already present
                                                if (!project.ToDos.Contains(toDo))
                                                {
                                                    project.ToDos.Add(toDo);
                                                    Console.WriteLine($"ToDo [{toDo.Id}] added to Project [{project.Id}].");
                                                }
                                                else
                                                {
                                                    Console.WriteLine($"ToDo [{toDo.Id}] is already in Project [{project.Id}].");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine($"ToDo with ID {toDoId} not found.");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("ERROR: Invalid ToDo ID.");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("No ToDos available to add.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Project with ID {pId} not found.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("ERROR: Invalid Project ID.");
                            }
                            break;

                        case 12: // Exit
                            break;

                        default:
                            Console.WriteLine("ERROR: Unknown menu selection");
                            break;
                    }
                } else
                {
                    Console.WriteLine($"ERROR: {choice} is not a valid menu selection");
                }

            } while (choiceInt != 12);

        }
    }
}