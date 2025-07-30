using Asana.Maui.ViewModels;

namespace Asana.Maui.Views;

//Code behind for project management main page
public partial class ProjectsView : ContentPage
{
	public ProjectsView()
	{
		InitializeComponent();
        BindingContext = new ProjectsPageViewModel();
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
        Shell.Current.GoToAsync("//ProjectDetails");
    }

    private void DeleteClicked(object sender, EventArgs e)
    {
        (BindingContext as ProjectViewModel)?.DoDeleteProject();
    }
}