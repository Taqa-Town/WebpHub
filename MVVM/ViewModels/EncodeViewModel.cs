// Ignore Spelling: Infobar Prog Popup


namespace WebpHub.MVVM.ViewModels;

[WinRT.GeneratedBindableCustomProperty]
public partial class EncodeViewModel: ObservableObject
{
    #region Properties
    [ObservableProperty] public partial string FolderPath { get; set; } = App.DefaultFolderPath;

    [ObservableProperty] public partial string FullPath { get; set; } = string.Empty;

    [ObservableProperty] public partial object ButtonContent { get; set; } = "Encode";

    [ObservableProperty] public partial string FileName { get; set; } = string.Empty;

    [ObservableProperty] public partial string ImageResolution { get; set; } = string.Empty;

    [ObservableProperty] public partial string ImageExtension { get; set; } = string.Empty;

    [ObservableProperty] public partial string ImageSize { get; set; } = string.Empty;

    [ObservableProperty] public partial bool OpenPop { get; set; } = false;

    [ObservableProperty] public partial ImageDataModel? NewImageData { get; set; }

    [ObservableProperty] public partial bool InfobarOpen { get; set; } = false;

    [ObservableProperty] public partial bool ViolateCondition { get; set; } = false;

    [ObservableProperty] public partial string WarningMessage { get; set; } = string.Empty;
    #endregion

    #region Commands

    public IAsyncRelayCommand EncodeCommand { get; set; }
    public IAsyncRelayCommand ImportCommand { get; set; }
    public IAsyncRelayCommand FolderCommand { get; set; }
    public IAsyncRelayCommand ClosePopupCommand { get; set; }

    public EncodeViewModel()
    {
        EncodeCommand = new AsyncRelayCommand(Encode);
        ImportCommand = new AsyncRelayCommand(Import);
        FolderCommand = new AsyncRelayCommand(Folder);
        ClosePopupCommand = new AsyncRelayCommand(ClosePopup);
    }
    public async Task Encode()
    {
        if(App.IsProcessing == true)
        {
            ViolateCondition = true;
            WarningMessage = "The App is processing currently";
        }
        App.IsProcessing = true;

        if (!Directory.Exists(FolderPath))
        {
            ViolateCondition = true;
            WarningMessage = "The folder doesn't exist, use a valid folder path";
        }
        else if (string.IsNullOrEmpty(FullPath) || string.IsNullOrWhiteSpace(FullPath))
        {
            ViolateCondition = true;
            WarningMessage = "please import a picture";
        }
        else
        {
            ButtonContent = new ProgressRing { IsIndeterminate = true };
            bool isDone = await Task.Run(() => WebpCenterModel.ScriptRunner(App.CwebpFilePath, FullPath, FolderPath, EncodeView.WebpManager.Options));
            if (isDone is true)
            {
                var name = System.IO.Path.GetFileNameWithoutExtension(FullPath) + ".webp";
                var newImagePath = System.IO.Path.Combine(FolderPath, name);
                var newImageStorageFile = await StorageFile.GetFileFromPathAsync(newImagePath);
                NewImageData = new(new DataExtractorService(newImageStorageFile));
                OpenPop = true;
                InfobarOpen = true;
                ButtonContent = "Encode";
                ViolateCondition = false;
            }

        }

        App.IsProcessing = false;
        ButtonContent = "Encode";

    }

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
            WarningMessage = "the picture is an animated webp, it can't be encoded";
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

    public async Task Folder()
    {
        var Picker = new FolderPicker();
        var hWnd = WindowNative.GetWindowHandle(App.MWindow);
        InitializeWithWindow.Initialize(Picker, hWnd);

        StorageFolder folder = await Picker.PickSingleFolderAsync();
        if (folder != null)
            FolderPath = folder.Path;
    }

    public async Task ClosePopup()
    {
        OpenPop = false;
        await Task.CompletedTask;
    }

    #endregion
}
