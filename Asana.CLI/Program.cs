using Asana.Library.Models;
using Asana.Library.Services;
using System;

namespace Asana
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            var toDoSvc = ToDoServiceProxy.Current;
            var projectSvc = ProjectServiceProxy.Current;
            int choiceInt;
            do
            {
                Console.WriteLine("Choose a menu option:");
                Console.WriteLine("1. Create a ToDo");
                Console.WriteLine("2. List all ToDos");
                Console.WriteLine("3. List all outstanding ToDos");
                Console.WriteLine("4. Delete a ToDo");
                Console.WriteLine("5. Update a ToDo");
                Console.WriteLine("6. Exit");

                var choice = Console.ReadLine() ?? "6";

                if (int.TryParse(choice, out choiceInt))
                {
                    switch (choiceInt)
                    {
                        case 1:
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
                        case 2:
                            toDoSvc.DisplayToDos(true);
                            break;
                        case 3:
                            toDoSvc.DisplayToDos();
                            break;
                        case 4:
                            toDoSvc.DisplayToDos(true);
                            Console.Write("ToDo to Delete: ");
                            var toDoChoice4 = int.Parse(Console.ReadLine() ?? "0");

                            var reference = toDoSvc.GetById(toDoChoice4);
                            toDoSvc.DeleteToDo(reference?.Id ?? 0);
                            break;
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
                            toDoSvc.AddOrUpdate(updateReference);
                            break;
                        case 6: // Create a Project
                            Console.Write("Name:");
                            name = Console.ReadLine();
                            Console.Write("Description:");
                            description = Console.ReadLine();

                            projectSvc.AddOrUpdate(new Project
                            {
                                Name = name,
                                Description = description,
                                Id = 0,
                                CompletePercent = 0
                            });
                            break;

                        case 7: // List all Projects
                            projectSvc.DisplayProjects();
                            break;

                        case 8: // List all ToDos in a Project
                            Console.Write("Enter Project ID: ");
                            var projectIdInput = Console.ReadLine();
                            if (int.TryParse(projectIdInput, out int projectId))
                            {
                                var project = projectSvc.GetById(projectId);
                                if (project != null)
                                {
                                    if (project.ToDoList != null && project.ToDoList.Any())
                                    {
                                        project.ToDoList.ForEach(Console.WriteLine);
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
                            /*toDoSvc.DisplayToDos(true);
                            Console.Write("ToDo to Delete: ");
                            var toDoChoice4 = int.Parse(Console.ReadLine() ?? "0");

                            var reference = toDoSvc.GetById(toDoChoice4);
                            toDoSvc.DeleteToDo(reference?.Id ?? 0);*/
                            projectSvc.DisplayProjects();
                            Console.Write("Project to Delete: ");
                            var projectIdToDelete = Console.ReadLine();
                            if (int.TryParse(projectIdToDelete, out int deleteId))
                            {
                                var projectReference = projectSvc.GetById(deleteId);
                                if (projectReference != null)
                                {
                                    projectSvc.DeleteProject(projectReference?.Id ?? 0);
                                    Console.WriteLine($"Project [{deleteId}] deleted.");
                                }
                                else
                                {
                                    Console.WriteLine($"Project with ID {deleteId} not found.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("ERROR: Invalid Project ID.");
                            }
                            break;

                        case 10: // Update a Project
                            projectSvc.DisplayProjects();
                            Console.Write("Project to Update: ");
                            var projectIdToUpdate = Console.ReadLine();
                            if (int.TryParse(projectIdToUpdate, out int updateId))
                            {
                                var puReference = projectSvc.GetById(updateId);
                                if (puReference != null)
                                {
                                    Console.Write("Name:");
                                    puReference.Name = Console.ReadLine();
                                    Console.Write("Description:");
                                    puReference.Description = Console.ReadLine();
                                    Console.Write("Complete Percent (0-100): ");
                                    if (int.TryParse(Console.ReadLine(), out int completePercent))
                                    {
                                        puReference.CompletePercent = completePercent;
                                    }
                                    projectSvc.AddOrUpdate(puReference);
                                    Console.WriteLine($"Project [{updateId}] updated.");
                                }
                                else
                                {
                                    Console.WriteLine($"Project with ID {updateId} not found.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("ERROR: Invalid Project ID.");
                            }
                            break;

                        case 11: // Add ToDos to a Project
                            Console.Write("Enter Project ID: ");
                            var pIdInput = Console.ReadLine();
                            if (int.TryParse(pIdInput, out int pId))
                            {
                                var project = projectSvc.GetById(pId);
                                if (project != null)
                                {
                                    if (toDoSvc.ToDos.Any())
                                    {
                                        Console.WriteLine("Available ToDos:");
                                        toDoSvc.ToDos.ForEach(Console.WriteLine);
                                        Console.Write("Enter ToDo ID to add to project: ");
                                        var toDoIdInput = Console.ReadLine();
                                        if (int.TryParse(toDoIdInput, out int toDoId))
                                        {
                                            if (projectSvc.AddToDoToProject(pId, toDoId))
                                            {
                                                Console.WriteLine($"ToDo [{toDoId}] added to Project [{pId}].");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"Failed to add ToDo [{toDoId}] to Project [{pId}]. ToDo or Project not found, or ToDo already in Project.");
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
                }
                else
                {
                    Console.WriteLine($"ERROR: {choice} is not a valid menu selection");
                }

            } while (choiceInt != 12);
        }
    }
}