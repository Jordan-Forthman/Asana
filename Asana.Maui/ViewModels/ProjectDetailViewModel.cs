using Asana.Library.Models;
using Asana.Library.Services;
using Asana.Library.Models;
using Asana.Library.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Asana.Maui.ViewModels
{
    public class ProjectDetailViewModel : INotifyPropertyChanged
    {
        public ProjectDetailViewModel()
        {
            Model = new Projects();
            SaveCommand = new Command(Save);
            DeleteCommand = new Command(Delete);
        }

        public ProjectDetailViewModel(int id)
        {
            Model = ProjectServiceProxy.Current.GetById(id) ?? new Projects();
            SaveCommand = new Command(Save);
            DeleteCommand = new Command(Delete);
        }

        public ProjectDetailViewModel(Projects? model)
        {
            Model = model ?? new Projects();
            SaveCommand = new Command(Save);
            DeleteCommand = new Command(Delete);
        }

        public Projects? Model { get; set; }

        public ICommand? SaveCommand { get; set; }
        public ICommand? DeleteCommand { get; set; }

        public ObservableCollection<ToDoDetailViewModel> ToDos
        {
            get
            {
                return new ObservableCollection<ToDoDetailViewModel>(
                    Model?.ToDos?.Select(t => new ToDoDetailViewModel(t)) ?? new List<ToDoDetailViewModel>());
            }
        }

        public ObservableCollection<ToDoDetailViewModel> AvailableToDos
        {
            get
            {
                var allToDos = ToDoServiceProxy.Current.ToDos;
                var projectToDos = Model?.ToDos ?? new List<ToDo>();
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

        public void Save()
        {
            if (Model != null)
            {
                ProjectServiceProxy.Current.AddOrUpdate(Model);
            }
        }

        public void Delete()
        {
            if (Model != null)
            {
                ProjectServiceProxy.Current.DeleteProject(Model);
            }
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

