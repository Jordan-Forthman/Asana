﻿using Asana.Library.Models;
using Asana.Library.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


//Handles single project detailed view
namespace Asana.Maui.ViewModels
{
    public class ProjectDetailViewModel : INotifyPropertyChanged
    {
        public ProjectDetailViewModel()
        {
            Projects = new ObservableCollection<ProjectManViewModel>(ProjectServiceProxy.Current.Projects
                .Select(p => new ProjectManViewModel(p)));
            Model = new Project { ToDoList = new List<ToDo>() }; // Ensure ToDoList is not null
        }

        public ProjectDetailViewModel(int projectId)
        {
            var project = ProjectServiceProxy.Current.GetById(projectId) ?? new Project();
            project.ToDoList ??= new List<ToDo>();
            Model = project;
            Projects = new ObservableCollection<ProjectManViewModel>(
                ProjectServiceProxy.Current.Projects.Select(p => new ProjectManViewModel(p)));
        }

        public ProjectDetailViewModel(Project? model)
        {
            var project = model ?? new Project();
            project.ToDoList ??= new List<ToDo>();
            Model = project;
        }

        public ProjectManViewModel? SelectedProject { get; set; }
        public ObservableCollection<ProjectManViewModel> Projects { get; set; }

        private Project _model;
        public Project? Model
        {
            get => _model;
            set
            {
                _model = value;
                UpdateToDoLists();
                OnPropertyChanged(nameof(Model));
            }
        }


        private ObservableCollection<ToDoDetailViewModel> _currentToDos;
        public ObservableCollection<ToDoDetailViewModel> CurrentToDos
        {
                get => _currentToDos;
                set { _currentToDos = value; 
                OnPropertyChanged(nameof(CurrentToDos)); }
        }

        private ObservableCollection<ToDoDetailViewModel> _availableToDos;
        public ObservableCollection<ToDoDetailViewModel> AvailableToDos
        {
                get => _availableToDos;
                set {_availableToDos = value; OnPropertyChanged(nameof(AvailableToDos)); }
        }

        private ToDoDetailViewModel? _selectedToDo;
        public ToDoDetailViewModel? SelectedToDo
        {
            get => _selectedToDo;
            set
            {
                _selectedToDo = value;
                NotifyPropertyChanged();
            }
        }

        private void UpdateToDoLists()
        {
            if (Model == null)
            {
                Model = new Project { ToDoList = new List<ToDo>() };
            }
            Model.ToDoList ??= new List<ToDo>();

            CurrentToDos = new ObservableCollection<ToDoDetailViewModel>(
                Model.ToDoList.Select(t => new ToDoDetailViewModel(t)));
            var allToDos = ToDoServiceProxy.Current.ToDos;
            var projectToDos = Model.ToDoList;
            AvailableToDos = new ObservableCollection<ToDoDetailViewModel>(
                allToDos.Where(t => !projectToDos.Contains(t)).Select(t => new ToDoDetailViewModel(t)));
        }

        public void AddOrUpdateProject()
        {
            if (Model?.Id == 0)
            {
                // If the project is new, assign a new ID
                ProjectServiceProxy.Current.AddOrUpdate(Model);
            }
            else
            {
                // If the project already exists, update it
                ProjectServiceProxy.Current.AddOrUpdate(Model);
            }

            Projects.Clear();
            foreach (var p in ProjectServiceProxy.Current.Projects)
            {
                // Add each project to the Projects collection
                Projects.Add(new ProjectManViewModel(p));
            }


            /*
            // Refresh the project list after adding or updating
            Projects = new ObservableCollection<ProjectsViewModel>(
                ProjectServiceProxy.Current.Projects.Select(p => new ProjectsViewModel(p)));
            */
            /*Projects = ProjectServiceProxy.Current.Projects
                .Select(p => new ProjectsViewModel(p))
                .ToList();*/

            // Notify that the Projects property has changed
            OnPropertyChanged(nameof(Projects));
            //ProjectServiceProxy.Current.AddOrUpdate(Model);
        }

        public void AddToDo(int toDoId)
        {
            if (Model != null)
            {
                ProjectServiceProxy.Current.AddToDoToProject(Model.Id, toDoId);
                // Refresh Model from service to get updated ToDoList
                Model = ProjectServiceProxy.Current.GetById(Model.Id);
                // UpdateToDoLists() will be called by Model setter
            }
        }

        public void RemoveToDo(int toDoId)
        {
            if (Model != null)
            {
                ProjectServiceProxy.Current.RemoveToDoFromProject(Model.Id, toDoId);
                Model = ProjectServiceProxy.Current.GetById(Model.Id);
            }
        }

        public void RefreshPage()
        {
            NotifyPropertyChanged(nameof(CurrentToDos));
            NotifyPropertyChanged(nameof(AvailableToDos));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
