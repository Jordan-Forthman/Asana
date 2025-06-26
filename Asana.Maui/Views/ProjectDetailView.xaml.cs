using Asana.Library.Models;
using Asana.Maui.ViewModels;

namespace Asana.Maui.Views;

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
    private void ContentPage_NavigatedFrom(object sender, NavigatedFromEventArgs e)
    {

    }
    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        BindingContext = new ToDoDetailViewModel(ProjectId);
    }
}