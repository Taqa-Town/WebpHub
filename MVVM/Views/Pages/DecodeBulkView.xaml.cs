
using WebpHub.MVVM.Models;
using WebpHub.MVVM.ViewModels;

namespace WebpHub.MVVM.Views.Pages;

public sealed partial class DecodeBulkView : Page
{
    public static WebpCenterModel WebpManager { get; private set; }
    public DecodeBulkViewModel ViewModel { get; private set; }
    public static string FormatType { get; set; } = ".png";

    public DecodeBulkView()
    {
        WebpManager = new();
        ViewModel = new();
        InitializeComponent();
        DataContext = ViewModel;
        
    }
}
