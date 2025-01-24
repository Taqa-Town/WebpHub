// Ignore Spelling: Webp Prog Infobar Popup

namespace WebpHub.MVVM.ViewModels;

public partial class EncodeAnimatedWebpViewModel : ObservableObject
{
    #region Properties

    [ObservableProperty] public partial string FolderPath { get; set; } = App.DefaultFolderPath;
    [ObservableProperty] public partial object ButtonContent { get; set; } = "Encode";
    [ObservableProperty] public partial string FullPath { get; set; } = string.Empty;
    [ObservableProperty] public partial string FileName { get; set; } = string.Empty;
    [ObservableProperty] public partial string ImageResolution { get; set; } = string.Empty;
    [ObservableProperty] public partial string ImageExtension { get; set; } = string.Empty;
    [ObservableProperty] public partial string ImageSize { get; set; } = string.Empty;
    [ObservableProperty] public partial bool OpenPop { get; set; } = false;
    [ObservableProperty] public partial ImageDataModel NewImageData { get; set; } = new();
    [ObservableProperty] public partial bool InfobarOpen { get; set; } = false;
    [ObservableProperty] public partial string InfoMessage { get; set; } = string.Empty;
    [ObservableProperty] public partial string Error { get; set; } = string.Empty;
    [ObservableProperty] public partial bool ViolateCondition { get; set; } = false;
    [ObservableProperty] public partial string WarningMessage { get; set; } = string.Empty;

    #endregion

    #region Commands

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
    public async Task ClosePopup()
    {
        OpenPop = false;
        await Task.CompletedTask;
    }

    [RelayCommand]
    public async Task Encode()
    {
        App.IsProcessing = true;

        if (!Directory.Exists(FolderPath))
        {
            ViolateCondition = true;
            WarningMessage = "The folder doesn't exist, use a valid folder path";
        }
        else if (string.IsNullOrEmpty(FullPath) || string.IsNullOrWhiteSpace(FullPath))
        {
            ViolateCondition = true;
            WarningMessage = "Please import an image";
        }
        else
        {
            ButtonContent = new ProgressRing { IsIndeterminate = true };
            bool isDone = await Task.Run(() => WebpCenterModel.ScriptRunner(App.Gif2WebpFilePath, FullPath, FolderPath, AnimatedWebpView.WebpManager.Options));

            if (isDone is true)
            {
                ButtonContent = "Encode";
                var name = System.IO.Path.GetFileNameWithoutExtension(FullPath) + ".webp";
                var newImagePath = System.IO.Path.Combine(FolderPath, name);
                var newImageStorageFile = await StorageFile.GetFileFromPathAsync(newImagePath);
                NewImageData = new(new DataExtractorService(newImageStorageFile));

                OpenPop = true;
                InfobarOpen = true;
                ViolateCondition = false;
            }
        }
        ButtonContent = "Encode";
        App.IsProcessing = false;
    }

    #endregion
}
