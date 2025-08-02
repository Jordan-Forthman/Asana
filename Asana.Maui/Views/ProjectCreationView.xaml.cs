using Asana.Library.Models;

namespace Asana.Maui.Views;

public partial class ProjectCreationView : ContentPage
{
	public ProjectCreationView()
	{
		InitializeComponent();
	}

    private void ContentPage_NavigatedFrom(object sender, NavigatedFromEventArgs e)
    {

    }

    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        //BindingContext = new ProjectCreationViewModel(ProjectId);
    }
}