// Ignore Spelling: Infobar Prog

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace WebpHub.MVVM.ViewModels;

[WinRT.GeneratedBindableCustomProperty]
public partial class DecodeBulkViewModel : ObservableObject
{
    #region Properties
    [ObservableProperty] public partial ObservableCollection<ImageModel> ImagesList { get; set; } = [];

    [ObservableProperty] public partial string FolderPath { get; set; } = App.DefaultFolderPath;

    [ObservableProperty] public partial bool InfobarOpen { get; set; } = false;

    [ObservableProperty] public partial bool ProgISActive { get; set; } = false;

    [ObservableProperty] public partial bool PassedTheLimit { get; set; } = false;

    [ObservableProperty] public partial string PassedTheLimitMessage { get; set; } = string.Empty;

    [ObservableProperty] public partial bool ViolateCondition { get; set; } = false;

    [ObservableProperty] public partial string WarningMessage { get; set; } = string.Empty;
    #endregion

    #region Commands

    public IAsyncRelayCommand DecodeCommand { get; set; }
    public IAsyncRelayCommand ImportCommand { get; set; }
    public IAsyncRelayCommand FolderCommand { get; set; }
    public IAsyncRelayCommand<object> DeleteCommand { get; set; }
    public IAsyncRelayCommand ClearCommand { get; set; }
    public IAsyncRelayCommand OpenExplorerCommand { get; set; }

    public DecodeBulkViewModel()
    {
        DecodeCommand = new AsyncRelayCommand(Decode);
        ImportCommand = new AsyncRelayCommand(Import);
        FolderCommand = new AsyncRelayCommand(Folder);
        DeleteCommand = new AsyncRelayCommand<object>(Delete);
        ClearCommand = new AsyncRelayCommand(Clear);
        OpenExplorerCommand = new AsyncRelayCommand(OpenExplorer);
    }


    public async Task Decode()
    {
        App.IsProcessing = true;

        if (ImagesList.Count <= 0)
        {
            ViolateCondition = true;
            WarningMessage = "You must import Images before Encoding";
        }
        else if (string.IsNullOrEmpty(FolderPath) || string.IsNullOrWhiteSpace(FolderPath))
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
            ProgISActive = true;

            if (ImagesList.Count >= 1000)
            {
                var lists = TOListOfList([.. ImagesList]);
                await Task.Run(
                () => Parallel.ForEach(lists, async (subList) =>
                {
                    await WebpCenterModel.ScriptRunnerBulk(App.DwebpFilePath, subList, FolderPath, DecodeBulkView.FormatType, DecodeBulkView.WebpManager.Options);
                }));
            }
            else
            {
                await Task.Run(() => WebpCenterModel.ScriptRunnerBulk(App.DwebpFilePath, [.. ImagesList], FolderPath, DecodeBulkView.FormatType, DecodeBulkView.WebpManager.Options));
            }
            InfobarOpen = true;
            ProgISActive = false;
            ViolateCondition = false;
        }

        App.IsProcessing = false;
    }

    public async Task Import()
    {
        var openPicker = new FileOpenPicker { ViewMode = PickerViewMode.Thumbnail, FileTypeFilter = { ".webp" } };

        var hWnd = WindowNative.GetWindowHandle(App.MWindow);
        InitializeWithWindow.Initialize(openPicker, hWnd);

        var files = await openPicker.PickMultipleFilesAsync();
        int id = 0;
        int voilate = 0;
        int isAnimated = 0;
        if (files != null)
        {
            ProgISActive = true;
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
       
                ImagesList.Add( new ImageModel(item.Path, id, info.Length) );
            }
            ProgISActive = false;
        }
        if (voilate > 0)
        {
            PassedTheLimitMessage = $"{voilate} file(s) couldn't be uploaded because it/they surpassed the 105mb limit";
            PassedTheLimit = true;
        }
        if (isAnimated > 0)
        {
            ViolateCondition = true;
            WarningMessage = $"{isAnimated} file(s) is animated webp, they can't be decoded";
        }
        InfobarOpen = false;
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

    public async Task Delete(object? param)
    {
        var id = (Int32)param;
        var SelectedImage = ImagesList.First(x => x.ID == id);
        ImagesList.Remove(SelectedImage);
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

    private static List<List<ImageModel>> TOListOfList(List<ImageModel> ogList)
    {
        List<List<ImageModel>> newlist = [];
        int totalSize = ogList.Count;
        int partSize = totalSize / 3;
        int remainder = totalSize % 3;

        List<ImageModel> list1 = ogList.GetRange(0, partSize + (remainder > 0 ? 1 : 0));
        List<ImageModel> list2 = ogList.GetRange(list1.Count, partSize + (remainder > 1 ? 1 : 0));
        List<ImageModel> list3 = ogList.GetRange(list1.Count + list2.Count, partSize);
        newlist.Add(list1);
        newlist.Add(list2);
        newlist.Add(list3);
        return newlist;
    }

    #endregion
}
