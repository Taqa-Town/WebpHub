using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace WebpHub.MVVM.ViewModels;

public partial class DecodeBulkViewModel: ObservableObject
{
    public ObservableCollection<ImageModel> ImagesList { get; set; } = [];
    public List<ImmutableImageModel> IMImageList { get; private set; } = [];

    [ObservableProperty]
    private string _folderPath = App.DefaultFolderPath;

    [ObservableProperty]
    private bool _infobarOpen = false;

    [ObservableProperty]
    private bool _progISActive = false;

    [ObservableProperty]
    private bool _passedTheLimit = false;

    [ObservableProperty]
    private string _passedTheLimitMessage = string.Empty;

    [ObservableProperty]
    private bool _violateCondition = false;

    [ObservableProperty]
    private string _warrningMessage = string.Empty;

    [RelayCommand]
    public async Task Decode()
    {
        App.IsProcessing = true;

        if (ImagesList.Count <= 0)
        {
            ViolateCondition = true;
            WarrningMessage = "You must import Images before Encoding";
        }
        else if (string.IsNullOrEmpty(FolderPath) || string.IsNullOrWhiteSpace(FolderPath))
        {
            ViolateCondition = true;
            WarrningMessage = "Specify an output folder please";
        }
        else if (!Directory.Exists(FolderPath))
        {
            ViolateCondition = true;
            WarrningMessage = "The folder doesn't exist, use a valid folder path";
        }
        else
        {
            ProgISActive = true;
     
            if (IMImageList.Count >= 1000)
            {
                var lists = TOListOfList(IMImageList);
                await Task.Run(
                () => Parallel.ForEach(lists, async (subList) =>
                {
                    await WebpCenterModel.ScriptRunnerBulk(App.DwebpFilePath, subList, FolderPath, DecodeBulkView.FormatType, DecodeBulkView.WebpManager.Options);
                }));
            }
            else
            {
                await Task.Run(() => WebpCenterModel.ScriptRunnerBulk(App.DwebpFilePath, IMImageList, FolderPath, DecodeBulkView.FormatType, DecodeBulkView.WebpManager.Options));
            }
            InfobarOpen = true;
            ProgISActive = false;
            ViolateCondition = false;
        }

        App.IsProcessing = false;
    }

    [RelayCommand]
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
                ImagesList.Add(new(item.Path, id, info.Length));
                IMImageList.Add(new ImmutableImageModel { Location = item.Path, ID = id, Size = info.Length });
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
            WarrningMessage = $"{isAnimated} file(s) is animated webp, they can't be decoded";
        }
        InfobarOpen = false;
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
    public void Delete(object param)
    {
        var id = (Int32)param;
        var SelectedImage = ImagesList.First(x => x.ID == id);
        var SelectedImImage = IMImageList.First(x => x.ID == id);
        ImagesList.Remove(SelectedImage);
        IMImageList.Remove(SelectedImImage);
    }

    [RelayCommand]
    public async Task OpenExplorer()
    {
        await Task.Run(() => Process.Start("explorer.exe", FolderPath));
    }

    [RelayCommand]
    public void Clear()
    {
        ImagesList.Clear();
    }

    private static List<List<ImmutableImageModel>> TOListOfList(List<ImmutableImageModel> ogList)
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

}
