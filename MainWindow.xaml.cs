// Ignore Spelling: App

using Microsoft.UI.Windowing;
using WinUIEx;


namespace WebpHub;

public sealed partial class MainWindow : Window
{
    public static AppWindowTitleBar? AppTitleBar { get; private set; }

    public MainWindow()
    {
        InitializeComponent();
        this.Maximize();
        this.SetIcon(App.AppIcon);
        Title = "WebpHub";
        ExtendsContentIntoTitleBar = true;
        AppTitleBar = AppWindow.TitleBar;
    }

    private void MainNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        var selectedItem = (NavigationViewItem)args.SelectedItem;
        if (selectedItem is not null)
        {
            string pageName = $"WebpHub.MVVM.Views.Pages.{selectedItem.Tag}View";
            Type pageType = Type.GetType(pageName);
            ContentFrame.Navigate(pageType);
        }
    }


}

