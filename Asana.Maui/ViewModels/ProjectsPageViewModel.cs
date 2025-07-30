using Asana.Library.Models;
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
    public class ProjectsPageViewModel : INotifyPropertyChanged
    {
        public List<ProjectViewModel> Projects { get; set; }
        public ProjectViewModel? SelectedProject {get; set;}

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

        public ProjectsPageViewModel(Project project = null)
        {
            Model = project ?? new Project();
        }

        public ProjectsPageViewModel()
        {
            Projects = ProjectServiceProxy.Current.Projects
                .Select(p => new ProjectViewModel(p))
                .ToList();
        }


        private ObservableCollection<ToDoDetailViewModel> _currentToDos;
        public ObservableCollection<ToDoDetailViewModel> CurrentToDos
        {
                get => _currentToDos;
                set { _currentToDos = value; OnPropertyChanged(nameof(CurrentToDos)); }
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
            CurrentToDos = new ObservableCollection<ToDoDetailViewModel>(
                Model?.ToDoList?.Select(t => new ToDoDetailViewModel(t)) ?? new List<ToDoDetailViewModel>());
            var allToDos = ToDoServiceProxy.Current.ToDos;
            var projectToDos = Model?.ToDoList ?? new List<ToDo>();
            AvailableToDos = new ObservableCollection<ToDoDetailViewModel>(
                allToDos.Where(t => !projectToDos.Contains(t)).Select(t => new ToDoDetailViewModel(t)));
        }
        public void AddOrUpdateProject()
        {
            ProjectServiceProxy.Current.AddOrUpdate(Model);
        }

        public void AddToDo(int toDoId)
        {
            if (Model != null)
            {
                ProjectServiceProxy.Current.AddToDoToProject(Model.Id, toDoId);
                RefreshPage();
            }
        }

        public void RemoveToDo(int toDoId)
        {
            if (Model != null)
            {
                ProjectServiceProxy.Current.RemoveToDoFromProject(Model.Id, toDoId);
                RefreshPage();
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
