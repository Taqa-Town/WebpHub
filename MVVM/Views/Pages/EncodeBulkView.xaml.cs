// Ignore Spelling: Webp

namespace WebpHub.MVVM.Views.Pages;

public sealed partial class EncodeBulkView : Page
{
    public static WebpCenterModel WebpManager { get; private set; } = new();
    public EncodeBulkViewModel ViewModel { get; private set; } = new();

    public EncodeBulkView()
    {
        DataContext = ViewModel;
        InitializeComponent();
    }
}
