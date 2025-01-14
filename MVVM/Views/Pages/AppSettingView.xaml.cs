// Ignore Spelling: App


namespace WebpHub.MVVM.Views.Pages;


public sealed partial class AppSettingView : Page
{
    public AppSettingViewModel ViewModel { get; private set; } = new();
    public AppSettingView()
    {
        InitializeComponent();
    }
}
