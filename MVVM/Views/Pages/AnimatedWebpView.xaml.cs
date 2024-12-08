
using WebpHub.MVVM.Models;
using WebpHub.MVVM.ViewModels;

namespace WebpHub.MVVM.Views.Pages;

public sealed partial class AnimatedWebpView : Page
{
    public static WebpCenterModel WebpManager { get; private set; }
    public EncodeAnimatedWebpViewModel ViewModel { get; private set; }

    public AnimatedWebpView()
    {
        ViewModel = new();
        WebpManager = new();
        InitializeComponent();
        DataContext = ViewModel;
    }
}
