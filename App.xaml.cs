// Ignore Spelling: App Gif Webp Dwebp Cwebp

global using CommunityToolkit.Mvvm.ComponentModel;
global using Microsoft.UI.Xaml;
global using Microsoft.UI.Xaml.Controls;
global using System;
global using System.Collections.Generic;
global using System.IO;
global using WebpHub.MVVM.Models;
global using WebpHub.MVVM.ViewModels;
global using WebpHub.MVVM.Views.Pages;
global using CommunityToolkit.Mvvm.Input;
global using System.Threading.Tasks;
global using WebpHub.InternalServices;
global using Windows.Storage.Pickers;
global using Windows.Storage;
global using WinRT.Interop;
global using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI;


namespace WebpHub;

public partial class App : Application
{
    public static MainWindow? MWindow { get; private set; }
    public static string DefaultFolderPath { get; private set; } = string.Empty;
    public static string CwebpFilePath { get; private set; } = string.Empty;
    public static string DwebpFilePath { get; private set; } = string.Empty;
    public static string Gif2WebpFilePath { get; private set; } = string.Empty;
    public static string DummyImage { get; private set; } = string.Empty;
    public static string DummyImage2 { get; private set; } = string.Empty;
    public static bool IsProcessing { get; set; } = false;
    public static string SettingFilePath { get; private set; } = string.Empty;
    public static string AppIcon { get; private set; } = string.Empty;

    public App()
    {
        GenerateFoldersAndFiles();
        InitializeComponent();
        string theme = File.ReadAllText(SettingFilePath);
        if (theme.Contains("Dark"))
            RequestedTheme = ApplicationTheme.Dark;
        else if (theme.Contains("Light"))
            RequestedTheme = ApplicationTheme.Light;
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MWindow = new();
        MWindow.Activate();

        if (Current.RequestedTheme == ApplicationTheme.Light)
            MainWindow.AppTitleBar!.ButtonForegroundColor = Colors.Black;
        else
            MainWindow.AppTitleBar!.ButtonForegroundColor = Colors.White;
    }

    private static void GenerateFoldersAndFiles()
    {
        //default folder
        string picPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        DefaultFolderPath = Path.Combine(picPath, "WebpHub");
        if (!Directory.Exists(DefaultFolderPath))
            Directory.CreateDirectory(DefaultFolderPath);
        //settings
        string folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        SettingFilePath = Path.Combine(folder, "WebpHub", "Settings.txt");
        string? folder2 = Path.GetDirectoryName(SettingFilePath);
        if (!Directory.Exists(folder2))
            Directory.CreateDirectory(folder2);
        if (!File.Exists(SettingFilePath))
        {
            File.Create(SettingFilePath).Dispose();
            string theme = "Light";
            File.WriteAllText(SettingFilePath, theme);
        }
        // files paths
        CwebpFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ExeFiles", "cwebp.exe");
        DwebpFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ExeFiles", "dwebp.exe");
        Gif2WebpFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ExeFiles", "gif2webp.exe");
        DummyImage = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "unsopprtedFormat.png");
        AppIcon = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "AppIcon.ico");
        DummyImage2 = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "dummy2.png");
    }
}
