// Ignore Spelling: Webp


namespace WebpHub.MVVM.Views.Pages;

public sealed partial class EncodeView : Page
{
    public static WebpCenterModel WebpManager { get; private set; } = new();
    public EncodeViewModel ViewModel { get; private set; }
    public EncodeView()
    {
        ViewModel = new();
        DataContext = ViewModel;
        InitializeComponent();
    }
}

