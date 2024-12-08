
namespace WebpHub.MVVM.Views.Pages;

public sealed partial class EncodeView : Page
{
    public static WebpCenterModel WebpManager { get; private set; }
    public EncodeViewModel ViewModel { get; private set; }
    public EncodeView()
    {
        WebpManager = new();
        ViewModel = new();
        InitializeComponent();
        DataContext = ViewModel;
    }
}

