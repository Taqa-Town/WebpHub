// Ignore Spelling: Popup Prog Infobar

namespace WebpHub.MVVM.ViewModels;

[WinRT.GeneratedBindableCustomProperty]
public partial class DecodeViewModel: ObservableObject
{
    #region Properties

    [ObservableProperty] public partial string FolderPath { get; set; } = App.DefaultFolderPath;
    [ObservableProperty] public partial object ButtonContent { get; set; } = "Decode";
    [ObservableProperty] public partial string FullPath { get; set; } = string.Empty;

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

    public IAsyncRelayCommand DecodeCommand { get; set; }
    public IAsyncRelayCommand ImportCommand { get; set; }
    public IAsyncRelayCommand FolderCommand { get; set; }
    public IAsyncRelayCommand ClosePopupCommand { get; set; }
    public DecodeViewModel()
    {
        DecodeCommand = new AsyncRelayCommand(Decode);
        ImportCommand = new AsyncRelayCommand(Import);
        FolderCommand = new AsyncRelayCommand(Folder);
        ClosePopupCommand = new AsyncRelayCommand(ClosePopup);
    }

    public async Task Decode()
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
            WarningMessage = "Please import an Image";
        }
        else
        {
            ButtonContent = new ProgressRing { IsIndeterminate = true };
            ViolateCondition = false;
            bool isDone = await Task.Run(() => WebpCenterModel.ScriptRunner(App.DwebpFilePath, FullPath, FolderPath, DecodeView.WebpManager.Options, DecodeView.FormatType));

            if (isDone is true)
            {
                var name = System.IO.Path.GetFileNameWithoutExtension(FullPath) + $"{DecodeView.FormatType}";
                var newImagePath = System.IO.Path.Combine(FolderPath, name);
                var newImageStorageFile = await StorageFile.GetFileFromPathAsync(newImagePath);
                var newData = new DataExtractorService(newImageStorageFile);

                if (DecodeView.FormatType.Contains(".png") || DecodeView.FormatType.Contains(".tiff"))
                    NewImageData = new(newData);
                else
                    NewImageData = new(newData) { FullPath = App.DummyImage };

                ButtonContent = "Encode";
                OpenPop = true;
                InfobarOpen = true;
            }
            ButtonContent = "Encode";
        }

        App.IsProcessing = false;
    }


    public async Task Import()
    {
        var openPicker = new FileOpenPicker { ViewMode = PickerViewMode.Thumbnail, FileTypeFilter = { ".webp" } };

        var hWnd = WindowNative.GetWindowHandle(App.MWindow);
        InitializeWithWindow.Initialize(openPicker, hWnd);

        var file = await openPicker.PickSingleFileAsync();
        bool check = WebpCenterModel.IsAnimatedWebp(file.Path);
        if (check is true)
        {
            ViolateCondition = true;
            WarningMessage = "the picture is an animated webp, it can't be decoded";
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
