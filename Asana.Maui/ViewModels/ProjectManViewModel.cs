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
using System.Windows.Input;

//Handles project management main page

namespace Asana.Maui.ViewModels
{
    public class ProjectManViewModel: INotifyPropertyChanged
    {
        private ProjectServiceProxy _projectSvc;

        public Project? Model { get; set; }

        public ProjectManViewModel()
        {
            _projectSvc = ProjectServiceProxy.Current;
            Query = string.Empty;
            Model = new Project();
            ToDoVisibility = Visibility.Visible;
            ToggleToDoVisibility = new Command(DoToggleToDoVisibility);
            DeleteCommand = new Command(DoDelete);
        }

        public ProjectManViewModel(int id)
        {
            Model = ProjectServiceProxy.Current.GetById(id) ?? new Project();
            _projectSvc = ProjectServiceProxy.Current;
            Query = string.Empty;
            Model = new Project();
            ToDoVisibility = Visibility.Visible;
            ToggleToDoVisibility = new Command(DoToggleToDoVisibility);
            DeleteCommand = new Command(DoDelete);
        }
        public ProjectManViewModel(Project? model)
        {
            _projectSvc = ProjectServiceProxy.Current;
            Query = string.Empty;
            Model = model;
            ToDoVisibility = Visibility.Visible;
            ToggleToDoVisibility = new Command(DoToggleToDoVisibility);
            DeleteCommand = new Command(DoDelete);
        }

        public void DoDelete()
        {
            ProjectServiceProxy.Current.DeleteProject(Model?.Id ?? 0);
        }

        public ObservableCollection<ToDoDetailViewModel> ToDos
        {
            get
            {
                if (Model == null || Model.ToDoList == null)
                {
                    return new ObservableCollection<ToDoDetailViewModel>();
                }

                return new ObservableCollection<ToDoDetailViewModel>(
                    Model.ToDoList.Select(t => new ToDoDetailViewModel(t)));
            }
        }

        public ObservableCollection<ProjectDetailViewModel> Projects
        {
            get
            {
                var projects = _projectSvc.Projects.Where(t => (t?.Name?.Contains(Query) ?? false) || (t?.Description?.Contains(Query) ?? false))
                        .Select(t => new ProjectDetailViewModel(t));
                if (!IsShowCompleted)
                {
                    projects = projects.Where(t => !t?.Model?.IsCompleted ?? false);
                }
                return new ObservableCollection<ProjectDetailViewModel>(projects);
            }
        }
        public ICommand? ToggleToDoVisibility {  get; set; }

        private Visibility toDoVisibility;
        public Visibility ToDoVisibility { 
            get
            {
                return toDoVisibility;
            }
            set
            {
                if (toDoVisibility != value)
                {
                    toDoVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public void DoToggleToDoVisibility()
        {
            if(ToDoVisibility == Visibility.Collapsed)
            {
                ToDoVisibility = Visibility.Visible;
            } else
            {
                ToDoVisibility = Visibility.Collapsed;
            }
            NotifyPropertyChanged(nameof(ToDoVisibility));
        }

        private bool isShowCompleted;
        public bool IsShowCompleted
        {
            get
            {
                return isShowCompleted;
            }

            set
            {
                if (isShowCompleted != value)
                {
                    isShowCompleted = value;
                    NotifyPropertyChanged(nameof(Projects));
                }
            }
        }

        private ProjectDetailViewModel _selectedProject;
        public ProjectDetailViewModel SelectedProject
        {
            get => _selectedProject;
            set
            {
                if (_selectedProject != value)
                {
                    _selectedProject = value;
                    SelectedProjectId = value?.Model?.Id ?? 0; // Update SelectedProjectId using Project.Id
                    NotifyPropertyChanged(nameof(SelectedProject));
                }
            }
        }

        public int SelectedProjectId { get; set; }

        private string query;
        public string Query
        {
            get { return query; }
            set
            {
                if (query != value)
                {
                    query = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // Functionality for updating project state after click events
        public void AddOrUpdateProject()
        {
            ProjectServiceProxy.Current.AddOrUpdate(Model);
        }

        public void RefreshPage()
        {
            NotifyPropertyChanged(nameof(Projects));
        }

        public ICommand? DeleteCommand { get; set; }


        public override string ToString()
        {
            return $"{Model?.Id ?? -1}. {Model?.Name}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
