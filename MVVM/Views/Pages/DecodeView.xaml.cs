

namespace WebpHub.MVVM.Views.Pages;


public sealed partial class DecodeView : Page
{
    public static WebpCenterModel WebpManager { get; private set; }
    public DecodeViewModel ViewModel { get; private set; }
    public static string FormatType { get; set; } = ".png";

    public DecodeView()
    {
        WebpManager = new();
        ViewModel = new();
        InitializeComponent();
        DataContext = ViewModel;     
    }
}
