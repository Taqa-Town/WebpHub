using WebpHub.MVVM.ViewModels;

namespace WebpHub.MVVM.Views.Pages;


public sealed partial class AppSettingView : Page
{
    public AppSettingViewModel ViewModel { get; private set; }
    public AppSettingView()
    {
        ViewModel = new();
        InitializeComponent();
    }
}
