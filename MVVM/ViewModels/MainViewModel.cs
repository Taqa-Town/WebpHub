
namespace WebpHub.MVVM.ViewModels;

public partial class MainViewModel: ObservableObject
{
    [ObservableProperty] public partial string TitleBarIcon { get; set; }
    public MainViewModel()
    {
        TitleBarIcon = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "AppIcon.ico");
    }
}
