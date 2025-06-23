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

namespace Asana.Maui.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private ToDoServiceProxy _toDoSvc;
        private ProjectServiceProxy _projectSvc;

        public MainPageViewModel()
        {
            _toDoSvc = ToDoServiceProxy.Current;
            _projectSvc = ProjectServiceProxy.Current;
        }

        public ObservableCollection<ToDoDetailViewModel> ToDos
        {
            get
            {
                var toDos = _toDoSvc.ToDos
                    .Select(t => new ToDoDetailViewModel(t));
                if (!IsShowCompleted)
                {
                    toDos = toDos.Where(t => !t?.Model?.IsCompleted ?? false);
                }
                return new ObservableCollection<ToDoDetailViewModel>(toDos);
            }
        }

        public ObservableCollection<ProjectDetailViewModel> Projects
        {
            get
            {
                return new ObservableCollection<ProjectDetailViewModel>(
                    _projectSvc.Projects.Select(p => new ProjectDetailViewModel(p)));
            }
        }

        public ToDoDetailViewModel? SelectedToDo { get; set; }
        public ProjectDetailViewModel? SelectedProject { get; set; }

        public int SelectedToDoId => SelectedToDo?.Model?.Id ?? 0;
        public int SelectedProjectId => SelectedProject?.Model?.Id ?? 0;

        private bool isShowCompleted;
        public bool IsShowCompleted
        {
            get => isShowCompleted;
            set
            {
                if (isShowCompleted != value)
                {
                    isShowCompleted = value;
                    NotifyPropertyChanged(nameof(ToDos));
                }
            }
        }

        public void DeleteToDo()
        {
            if (SelectedToDo == null)
            {
                return;
            }
            _toDoSvc.DeleteToDo(SelectedToDo.Model);
            NotifyPropertyChanged(nameof(ToDos));
        }

        public void DeleteProject()
        {
            if (SelectedProject == null)
            {
                return;
            }
            _projectSvc.DeleteProject(SelectedProject.Model);
            NotifyPropertyChanged(nameof(Projects));
        }

        public void RefreshPage()
        {
            NotifyPropertyChanged(nameof(ToDos));
            NotifyPropertyChanged(nameof(Projects));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}