// Ignore Spelling: Infobar

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace WebpHub.MVVM.ViewModels;
[WinRT.GeneratedBindableCustomProperty]
public partial class EncodeBulkViewModel : ObservableObject
{
    #region Properties 

    [ObservableProperty] public partial ObservableCollection<ImageModel> ImagesList { get; set; } = [];
    public List<ImmutableImageModel> IMImageList { get; private set; } = [];
    [ObservableProperty] public partial string FolderPath { get; set; } = App.DefaultFolderPath;
    [ObservableProperty] public partial bool InfobarOpen { get; set; } = false;
    [ObservableProperty] public partial object ButtonContent { get; set; } = "Encode";
    [ObservableProperty] public partial object ImportButtonContent { get; set; } = "Import";
    [ObservableProperty] public partial bool PassedTheLimit { get; set; } = false;
    [ObservableProperty] public partial string PassedTheLimitMessage { get; set; } = string.Empty;
    [ObservableProperty] public partial bool ViolateCondition { get; set; } = false;
    [ObservableProperty] public partial string WarningMessage { get; set; } = string.Empty;

    #endregion

    #region Commands 
    
    public IAsyncRelayCommand EncodeCommand { get; set; }
    public IAsyncRelayCommand ImportCommand { get; set; }
    public IAsyncRelayCommand FolderCommand { get; set; }
    public IAsyncRelayCommand DeleteCommand { get; set; }
    public IAsyncRelayCommand ClearCommand { get; set; }
    public IAsyncRelayCommand OpenExplorerCommand { get; set; }

    public EncodeBulkViewModel()
    {
        EncodeCommand = new AsyncRelayCommand(Encode);
        ImportCommand = new AsyncRelayCommand(Import);
        FolderCommand = new AsyncRelayCommand(Folder);
        DeleteCommand = new AsyncRelayCommand(param => Delete(param));
        ClearCommand = new AsyncRelayCommand(Clear);
        OpenExplorerCommand = new AsyncRelayCommand(OpenExplorer);
    }

    public async Task Encode()
    {
        App.IsProcessing = true;

        if (ImagesList.Count <= 0)
        {
            ViolateCondition = true;
            WarningMessage = "You must import Images before Encoding";
        }
        else if (string.IsNullOrWhiteSpace(FolderPath) || string.IsNullOrEmpty(FolderPath))
        {
            ViolateCondition = true;
            WarningMessage = "Specify an output folder please";
        }
        else if (!Directory.Exists(FolderPath))
        {
            ViolateCondition = true;
            WarningMessage = "The folder doesn't exist, use a valid folder path";
        }
        else
        {
            ButtonContent = new ProgressRing { IsIndeterminate = true };
            if (IMImageList.Count >= 1000)
            {
                var lists = TOListOfLists(IMImageList);
                await Task.Run(
                () => Parallel.ForEach(lists, async (subList) =>
                {
                    await WebpCenterModel.ScriptRunnerBulk(App.CwebpFilePath, subList, FolderPath, EncodeBulkView.WebpManager.Options);
                }));
            }
            else
            {
                await Task.Run(() => WebpCenterModel.ScriptRunnerBulk(App.CwebpFilePath, IMImageList, FolderPath, EncodeBulkView.WebpManager.Options));
            }
            InfobarOpen = true;
            ButtonContent = "Encode";
            ViolateCondition = false;
        }

        App.IsProcessing = false;
    }

    public async Task Import()
    {
        var openPicker = new FileOpenPicker { ViewMode = PickerViewMode.Thumbnail, FileTypeFilter = { ".png", ".jpg", ".webp", ".tif" } };

        var hWnd = WindowNative.GetWindowHandle(App.MWindow);
        InitializeWithWindow.Initialize(openPicker, hWnd);

        var files = await openPicker.PickMultipleFilesAsync();
        int id = 0;
        int voilate = 0;
        int isAnimated = 0;

        if (files != null)
        {
            ImportButtonContent = new ProgressRing { IsIndeterminate = true };
            foreach (var item in files)
            {
                bool check = WebpCenterModel.IsAnimatedWebp(item.Path);
                FileInfo info = new(item.Path);
                if (info.Length > 110_100_480) // 105mb
                {
                    voilate++;
                    continue;
                }
                if (check is true)
                {
                    isAnimated++;
                    continue;
                }
                id++;
                ImagesList.Add(new(item.Path, id, info.Length));
                IMImageList.Add(new ImmutableImageModel { Location = item.Path, ID = id, Size = info.Length });
            }
            ImportButtonContent = "Import";
        }
        if (voilate > 0)
        {
            PassedTheLimitMessage = $"{voilate} file(s) couldn't be uploaded because they surpassed the 105mb limit";
            PassedTheLimit = true;
        }
        if (isAnimated > 0)
        {
            ViolateCondition = true;
            WarningMessage = $"{isAnimated} file(s) is animated webp, they can't be encoded";
        }
        InfobarOpen = false;
        ImportButtonContent = "Import";
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

    public async Task Delete(object param)
    {
        var id = (Int32)param;
        var SelectedImage = ImagesList.First(x => x.ID == id);
        var SelectedImImage = IMImageList.First(x => x.ID == id);
        ImagesList.Remove(SelectedImage);
        IMImageList.Remove(SelectedImImage);
        await Task.CompletedTask;
    }

    public async Task OpenExplorer()
    {
        await Task.Run(() => Process.Start("explorer.exe", FolderPath));
    }

    public async Task Clear()
    {
        ImagesList.Clear();
        await Task.CompletedTask;
    }

    private static List<List<ImmutableImageModel>> TOListOfLists(List<ImmutableImageModel> ogList)
    {
        List<List<ImmutableImageModel>> newlist = [];
        int totalSize = ogList.Count;
        int partSize = totalSize / 3;
        int remainder = totalSize % 3;

        List<ImmutableImageModel> list1 = ogList.GetRange(0, partSize + (remainder > 0 ? 1 : 0));
        List<ImmutableImageModel> list2 = ogList.GetRange(list1.Count, partSize + (remainder > 1 ? 1 : 0));
        List<ImmutableImageModel> list3 = ogList.GetRange(list1.Count + list2.Count, partSize);
        newlist.Add(list1);
        newlist.Add(list2);
        newlist.Add(list3);
        return newlist;
    }

    #endregion
}
