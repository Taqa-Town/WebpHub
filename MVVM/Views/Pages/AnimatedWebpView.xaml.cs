

// Ignore Spelling: Webp

namespace WebpHub.MVVM.Views.Pages;

public sealed partial class AnimatedWebpView : Page
{
    public static WebpCenterModel? WebpManager { get;  set; } = new();
    public EncodeAnimatedWebpViewModel ViewModel { get;  set; } = new();

    public AnimatedWebpView()
    {
        DataContext = ViewModel;
        InitializeComponent();    
    }
}
