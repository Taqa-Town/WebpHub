using Microsoft.Windows.AppLifecycle;
using System.Text.Json;

namespace WebpHub.MVVM.ViewModels;

public partial class AppSettingViewModel: ObservableObject
{
    [ObservableProperty]
    private bool _violateCondition = false;

    [ObservableProperty]
    private bool _enableDark;

    [ObservableProperty]
    private bool _enableLight;

    public AppSettingViewModel()
    {
        string result = File.ReadAllText(App.SettingFilePath);
        var con = JsonSerializer.Deserialize<ThemeModel>(result);
        string theme = con.Theme;
        

        if (theme.Contains("Light"))
            (EnableDark, EnableLight) = (true, false);
        else if (theme.Contains("Dark"))
            (EnableDark, EnableLight) = (false, true);

    }

    public void ToDark()
    {
        if (App.IsProcessing is false)
        {
            var config = new ThemeModel { Theme = "Dark" };
            string JsonText = JsonSerializer.Serialize(config);
            File.WriteAllText(App.SettingFilePath, JsonText);
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
            var config = new ThemeModel { Theme = "Light" };
            string JsonText = JsonSerializer.Serialize(config);
            File.WriteAllText(App.SettingFilePath, JsonText);
            var error = AppInstance.Restart("--restart");
        }
        else
        {
            ViolateCondition = true;
        }

    }

}
