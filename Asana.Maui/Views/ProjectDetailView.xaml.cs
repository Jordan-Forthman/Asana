using Asana.Library.Models;
using Asana.Maui.ViewModels;

namespace Asana.Maui.Views;

[QueryProperty(nameof(ProjectId), "projectId")]

public partial class ProjectDetailView : ContentPage
{
	public ProjectDetailView()
	{
		InitializeComponent();
	}
    public int ProjectId { get; set; }
    private void CancelClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//MainPage");
    }
    private void OkClicked(object sender, EventArgs e)
    {
        (BindingContext as ProjectDetailViewModel)?.AddOrUpdateProject();
        Shell.Current.GoToAsync("//MainPage");
    }

    private void AddToDosClicked(object sender, EventArgs e)
    {
        var viewModel = BindingContext as ProjectDetailViewModel;
        if (viewModel != null)
        {
            foreach (var toDo in viewModel.AvailableToDos.Where(t => t.IsSelected))
            {
                viewModel.AddToDo(toDo.Model.Id);
            }
            viewModel.RefreshPage();
        }
    }

    private void RemoveToDosClicked(object sender, EventArgs e)
    {
        var viewModel = BindingContext as ProjectDetailViewModel;
        if (viewModel != null)
        {
            foreach (var toDo in viewModel.ToDos.Where(t => t.IsSelected))
            {
                viewModel.RemoveToDo(toDo.Model.Id);
            }
            viewModel.RefreshPage();
        }
    }
    private void ContentPage_NavigatedFrom(object sender, NavigatedFromEventArgs e)
    {

    }
    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        BindingContext = new ProjectDetailViewModel(ProjectId);
    }
}