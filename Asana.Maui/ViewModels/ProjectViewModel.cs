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
    public class ProjectViewModel: INotifyPropertyChanged
    {
        public Project? Model { get; set; }
        public ProjectViewModel? SelectedProject { get; set; }

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

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        public ProjectViewModel()
        {
            Model = new Project();
            ToDoVisibility = Visibility.Visible;
            ToggleToDoVisibility = new Command(DoToggleToDoVisibility);
        }
        public ProjectViewModel(Project? model)
        {
            Model = model;
            ToDoVisibility = Visibility.Visible;
            ToggleToDoVisibility = new Command(DoToggleToDoVisibility);
        }

        // Functionality for updating project state after click events
        public void AddOrUpdateProject()
        {
            ProjectServiceProxy.Current.AddOrUpdate(Model);
        }

        public void DoDeleteProject()
        {

            ProjectServiceProxy.Current.DeleteProject(Model?.Id ?? 0);
        }

        public override string ToString()
        {
            return $"{Model?.Id ?? -1}. {Model?.Name}";
        }
    }
}
