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
        (BindingContext as ProjectsPageViewModel)?.AddOrUpdateProject();
        Shell.Current.GoToAsync("//MainPage");
    }
}