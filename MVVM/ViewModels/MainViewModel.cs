namespace WebpHub.MVVM.ViewModels;

public partial class MainViewModel: ObservableObject
{
    [ObservableProperty]
    public string _iconPath;
    public MainViewModel()
    {
        IconPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "AppIcon.ico");
    }
}
