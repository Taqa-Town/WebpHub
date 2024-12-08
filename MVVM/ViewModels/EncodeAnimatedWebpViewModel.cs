using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using WebpHub.InternalServices;
using WebpHub.MVVM.Models;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace WebpHub.MVVM.ViewModels;

public partial class EncodeAnimatedWebpViewModel: ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EncodeCommand))]
    private string _folderPath = App.DefaultFolderPath;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EncodeCommand))]
    private string _fullPath;

    [ObservableProperty]
    private string _fileName;

    [ObservableProperty]
    private string _imageResolution;

    [ObservableProperty]
    private string _imageExtension;

    [ObservableProperty]
    private string _imageSize;

    [ObservableProperty]
    private bool _OpenPop = false;

    [ObservableProperty]
    private ImageDataModel _newImageData;

    [ObservableProperty]
    private bool _infobarOpen = false;

    [ObservableProperty]
    private string _infoMessage = string.Empty;

    [ObservableProperty]
    private string _error;

    [ObservableProperty]
    private bool _progISActive = false;

    [ObservableProperty]
    private bool _violateCondition = false;

    [ObservableProperty]
    private string _warrningMessage;

    [RelayCommand]
    public async Task Import()
    {
        var openPicker = new FileOpenPicker { ViewMode = PickerViewMode.Thumbnail, FileTypeFilter = { ".gif" } };

        var hWnd = WindowNative.GetWindowHandle(App.MWindow);
        InitializeWithWindow.Initialize(openPicker, hWnd);

        var file = await openPicker.PickSingleFileAsync();
        if (file != null)
        {
            DataExtractorService extractor = new(file);
            FullPath = extractor.FullPath;
            FileName = extractor.FileName;
            ImageExtension = extractor.ImageExtension;
            ImageResolution = extractor.ImageResolution;
            ImageSize = extractor.ImageSize;
        }
        OpenPop = false;
        InfobarOpen = false;
        ViolateCondition = false;
    }

    [RelayCommand]
    public async Task Folder()
    {
        var Picker = new FolderPicker();
        var hWnd = WindowNative.GetWindowHandle(App.MWindow);
        InitializeWithWindow.Initialize(Picker, hWnd);

        StorageFolder folder = await Picker.PickSingleFolderAsync();
        if (folder != null)
            FolderPath = folder.Path;
    }

    [RelayCommand]
    public void ClosePopup()
    {
        OpenPop = false;
    }

    [RelayCommand(CanExecute = nameof(CanEncode))]
    public async Task Encode()
    {
        App.IsProcessing = true;

        if (!Directory.Exists(FolderPath))
        {
            ViolateCondition = true;
            WarrningMessage = "The folder doesn't exist, use a valid folder path";
        }
        else
        {
            ProgISActive = true;
            bool isDone = await Task.Run(() => AnimatedWebpView.WebpManager.ScriptRunner(App.Gif2WebpFilePath, FullPath, FolderPath, AnimatedWebpView.WebpManager.Options));

            if (isDone is true)
            {
                ProgISActive = false;
                var name = System.IO.Path.GetFileNameWithoutExtension(FullPath) + ".webp";
                var newImagePath = System.IO.Path.Combine(FolderPath, name);
                var newImageStorageFile = await StorageFile.GetFileFromPathAsync(newImagePath);
                NewImageData = new(new DataExtractorService(newImageStorageFile));

                OpenPop = true;
                InfobarOpen = true;
                ViolateCondition = false;
            }
        }

        App.IsProcessing = false;
    }

    private bool CanEncode()
    {
        return !string.IsNullOrEmpty(FolderPath) && !string.IsNullOrEmpty(FullPath);
    }

}
