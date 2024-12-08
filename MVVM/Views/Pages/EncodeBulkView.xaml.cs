
namespace WebpHub.MVVM.Views.Pages;

public sealed partial class EncodeBulkView : Page
{
    public static WebpCenterModel WebpManager { get; private set; }
    public EncodeBulkViewModel ViewModel { get; private set; }

    public EncodeBulkView()
    {
        WebpManager = new();
        ViewModel = new();
        InitializeComponent();
        DataContext = ViewModel;

    }
}
