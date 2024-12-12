namespace WebpHub.MVVM.ViewModels;

public partial class MainViewModel: ObservableObject
{
    [ObservableProperty]
    public string _iconPath = string.Empty;
    public MainViewModel()
    {
        IconPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "AppIcon.ico");
    }
}
