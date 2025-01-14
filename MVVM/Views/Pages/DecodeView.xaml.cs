// Ignore Spelling: Webp

namespace WebpHub.MVVM.Views.Pages;


public sealed partial class DecodeView : Page
{
    public static WebpCenterModel WebpManager { get; private set; } = new();
    public DecodeViewModel ViewModel { get; private set; } = new();
    public static string FormatType { get; set; } = ".png";

    public DecodeView()
    {
        DataContext = ViewModel;
        InitializeComponent();        
    }
}
