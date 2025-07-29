using Asana.Library.Models;
using Asana.Library.Services;
using Asana.Maui.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Asana.Maui.Views;

[QueryProperty(nameof(ProjectId), "projectId")]
public partial class ProjectDetailView : INotifyPropertyChanged
{
    public ProjectDetailView()
    {
        InitializeComponent();
    }

    public int ProjectId { get; set; }
    public Project? Model { get; set; }


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

    private void CancelClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//MainPage");
    }

    private void OkClicked(object sender, EventArgs e)
    {
        (BindingContext as ProjectsPageViewModel)?.AddOrUpdateProject();
        Shell.Current.GoToAsync("//MainPage");
    }

    private void RemoveToDosClicked(object sender, EventArgs e)
    {
        var viewModel = BindingContext as ProjectsPageViewModel;
        if (viewModel != null)
        {
            foreach (var toDo in viewModel.ToDos.Where(t => t.IsSelected))
            {
                viewModel.RemoveToDo(toDo.Model.Id);
            }
            viewModel.RefreshPage();
        }
    }

    private void AddToDosClicked(object sender, EventArgs e)
    {
        var viewModel = (ProjectsPageViewModel)BindingContext;
        if (viewModel != null)
        {
            foreach (var toDo in viewModel.ToDos.Where(t => t.IsSelected))
            {
                viewModel.AddToDo(toDo.Model.Id);
            }
            viewModel.RefreshPage();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}