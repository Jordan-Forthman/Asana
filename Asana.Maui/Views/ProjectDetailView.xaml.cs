using Asana.Library.Models;
using Asana.Library.Services;
using Asana.Maui.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

// Code behind for single project info display

namespace Asana.Maui.Views;

[QueryProperty(nameof(ProjectId), "projectId")]
public partial class ProjectDetailView : INotifyPropertyChanged
{
    public ProjectDetailView()
    {
        InitializeComponent();
    }

    private int _projectId;
    public int ProjectId { get; set; } = 0;

    public Project? Model { get; set; }

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
            foreach (var toDo in viewModel.CurrentToDos.Where(t => t.IsSelected))
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
            foreach (var toDo in viewModel.CurrentToDos.Where(t => t.IsSelected))
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

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        if (ProjectId == 0)
        {
            BindingContext = new ProjectsPageViewModel();
        }
        else
        {
            var project = ProjectServiceProxy.Current.GetById(ProjectId);
            BindingContext = project != null ? new ProjectsPageViewModel(project) : new ProjectsPageViewModel();
        }
    }
}