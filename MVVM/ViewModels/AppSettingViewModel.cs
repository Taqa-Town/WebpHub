// Ignore Spelling: App


using Microsoft.Windows.AppLifecycle;

namespace WebpHub.MVVM.ViewModels;

public partial class AppSettingViewModel: ObservableObject
{
    [ObservableProperty] public partial bool ViolateCondition { get; set; } = false;
    [ObservableProperty] public partial bool EnableDark { get; set; } = false;
    [ObservableProperty] public partial bool EnableLight { get; set; } = false;

    public AppSettingViewModel()
    {
        string theme = File.ReadAllText(App.SettingFilePath);
        if (theme.Contains("Light"))
            (EnableDark, EnableLight) = (true, false);
        else if (theme.Contains("Dark"))
            (EnableDark, EnableLight) = (false, true);
    }

    public void ToDark()
    {
        if (App.IsProcessing is false)
        {
            string theme = "Dark";
            File.WriteAllText(App.SettingFilePath, theme);
            var error = AppInstance.Restart("--restart");
        }
        else
        {
            ViolateCondition = true;
        }

    }

    public void ToLight()
    {
        if (App.IsProcessing is false)
        {
            string theme = "Light";
            File.WriteAllText(App.SettingFilePath, theme);
            var error = AppInstance.Restart("--restart");
        }
        else
        {
            ViolateCondition = true;
        }

    }

}
