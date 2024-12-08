using Microsoft.UI;
using Microsoft.UI.Windowing;
//using Windows.UI.WindowManagement;
using WinUIEx;


namespace WebpHub;

public sealed partial class MainWindow : Window
{
    public static AppWindowTitleBar AppTitleBar { get; private set; }
    public MainViewModel ViewModel { get; private set; }

    public MainWindow()
    {
        ViewModel = new();
        InitializeComponent();
        this.Maximize();
        AppWindow.SetIcon("AppIcon.ico");
        Title = "WebpHub";
        ExtendsContentIntoTitleBar = true;
        AppTitleBar = AppWindow.TitleBar;
        //SetTitleBar(AppTitleBar);
    }

    private void MainNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        var selectedItem = (NavigationViewItem)args.SelectedItem;
        if (selectedItem is not null)
        {
            var pageName = $"WebpHub.MVVM.Views.Pages.{selectedItem.Tag}View";
            var pageType = Type.GetType(pageName);
            ContentFrame.Navigate(pageType);
        }
    }


}

