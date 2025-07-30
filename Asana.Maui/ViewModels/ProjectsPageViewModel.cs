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
        public Project? Model { get; set; }

        public ProjectsPageViewModel()
        {
            Projects = ProjectServiceProxy.Current.Projects
                .Select(p => new ProjectViewModel(p))
                .ToList();
        }

        public ObservableCollection<ToDoDetailViewModel> ToDos
        {
            get
            {
                return new ObservableCollection<ToDoDetailViewModel>(
                    Model?.ToDoList?.Select(t => new ToDoDetailViewModel(t)) ?? new List<ToDoDetailViewModel>());
            }
        }

        public ObservableCollection<ToDoDetailViewModel> AvailableToDos
        {
            get
            {
                var allToDos = ToDoServiceProxy.Current.ToDos;
                var projectToDos = Model?.ToDoList ?? new List<ToDo>();
                return new ObservableCollection<ToDoDetailViewModel>(
                    allToDos.Where(t => !projectToDos.Contains(t)).Select(t => new ToDoDetailViewModel(t)));
            }
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
        public void AddOrUpdateProject()
        {
            ProjectServiceProxy.Current.AddOrUpdate(Model);
        }

        public void AddToDo(int toDoId)
        {
            if (Model != null)
            {
                ProjectServiceProxy.Current.AddToDoToProject(Model.Id, toDoId);
                NotifyPropertyChanged(nameof(ToDos));
                NotifyPropertyChanged(nameof(AvailableToDos));
            }
        }

        public void RemoveToDo(int toDoId)
        {
            if (Model != null)
            {
                ProjectServiceProxy.Current.RemoveToDoFromProject(Model.Id, toDoId);
                NotifyPropertyChanged(nameof(ToDos));
                NotifyPropertyChanged(nameof(AvailableToDos));
            }
        }

        public void RefreshPage()
        {
            NotifyPropertyChanged(nameof(ToDos));
            NotifyPropertyChanged(nameof(AvailableToDos));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
