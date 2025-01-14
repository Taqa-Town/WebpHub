// Ignore Spelling: Webp

namespace WebpHub.MVVM.Views.Pages;

public sealed partial class DecodeBulkView : Page
{
    public static WebpCenterModel WebpManager { get; private set; } = new();
    public DecodeBulkViewModel ViewModel { get; private set; } = new();
    public static string FormatType { get; set; } = ".png";

    public DecodeBulkView()
    {
        DataContext = ViewModel;
        InitializeComponent();    
    }
}
