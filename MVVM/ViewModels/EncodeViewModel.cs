using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using WebpHub.InternalServices;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace WebpHub.MVVM.ViewModels;

public partial class EncodeViewModel: ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EncodeCommand))]
    private string _folderPath = App.DefaultFolderPath;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EncodeCommand))]
    private string _fullPath = string.Empty;

    [ObservableProperty]
    private string _fileName = string.Empty;

    [ObservableProperty]
    private string _imageResolution = string.Empty;

    [ObservableProperty]
    private string _imageExtension = string.Empty;

    [ObservableProperty]
    private string _imageSize= string.Empty;

    [ObservableProperty]
    private bool _OpenPop = false;

    [ObservableProperty]
    private ImageDataModel? _newImageData;

    [ObservableProperty]
    private bool _infobarOpen = false;

    [ObservableProperty]
    private bool _progISActive = false;

    [ObservableProperty]
    private bool _violateCondition = false;

    [ObservableProperty]
    private string _warrningMessage = string.Empty;

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
            bool isDone = await Task.Run(() => WebpCenterModel.ScriptRunner(App.CwebpFilePath, FullPath, FolderPath, EncodeView.WebpManager.Options));
            if (isDone is true)
            {
                var name = System.IO.Path.GetFileNameWithoutExtension(FullPath) + ".webp";
                var newImagePath = System.IO.Path.Combine(FolderPath, name);
                var newImageStorageFile = await StorageFile.GetFileFromPathAsync(newImagePath);
                NewImageData = new(new DataExtractorService(newImageStorageFile));

                OpenPop = true;
                InfobarOpen = true;
                ProgISActive = false;
                ViolateCondition = false;
            }

        }

        App.IsProcessing = false;
    }

    [RelayCommand]
    public async Task Import()
    {
        var openPicker = new FileOpenPicker { ViewMode = PickerViewMode.Thumbnail, FileTypeFilter = { ".png", ".jpg", ".webp", ".tif" } };

        var hWnd = WindowNative.GetWindowHandle(App.MWindow);
        InitializeWithWindow.Initialize(openPicker, hWnd);

        var file = await openPicker.PickSingleFileAsync();
        bool check = WebpCenterModel.IsAnimatedWebp(file.Path);
        if (check is true)
        {
            ViolateCondition = true;
            WarrningMessage = "the picture is an animated webp, it can't be encoded";
        }
        else
        {
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

    private bool CanEncode()
    {
        return !string.IsNullOrEmpty(FolderPath) && !string.IsNullOrEmpty(FullPath);
    }

}
