using Asana.Library.Models;
using Asana.Maui.ViewModels;

namespace Asana.Maui.Views;

//Code behind for project management main page

[QueryProperty(nameof(ProjectId), "projectId")]

public partial class ProjectManView : ContentPage
{
    public int ProjectId { get; set; }

    public ProjectManView()
	{
		InitializeComponent();
	}

    private void CancelClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//MainPage");
    }

    private void AddClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//ProjectDetails");
    }

    private void EditClicked(object sender, EventArgs e)
    {
        var selectedId = (BindingContext as ProjectManViewModel)?.SelectedProjectId ?? 0;
        Shell.Current.GoToAsync($"//ProjectDetails?projectId={selectedId}");
        //Shell.Current.GoToAsync($"//ProjectDetails?projectId={selectedId}");
    }

    private void OkClicked(object sender, EventArgs e)
    {
        (BindingContext as ProjectDetailViewModel)?.AddOrUpdateProject();
        Shell.Current.GoToAsync("//MainPage");
    }


    private void InLineDeleteProjectClicked(object sender, EventArgs e)
    {
        (BindingContext as ProjectManViewModel)?.RefreshPage();
    }

    private void ContentPage_NavigatedFrom(object sender, NavigatedFromEventArgs e)
    {

    }

    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        BindingContext = new ProjectManViewModel(ProjectId);
    }
}