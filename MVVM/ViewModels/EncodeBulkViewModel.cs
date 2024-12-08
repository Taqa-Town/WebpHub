using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebpHub.MVVM.Models;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;

namespace WebpHub.MVVM.ViewModels;

public partial class EncodeBulkViewModel: ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ImageModel> _imagesList = [];

    [ObservableProperty]
    private string _folderPath = App.DefaultFolderPath;

    [ObservableProperty]
    private bool _infobarOpen = false;

    [ObservableProperty]
    private bool _progISActive = false;

    [ObservableProperty]
    private bool _passedTheLimit = false;

    [ObservableProperty]
    private string _passedTheLimitMessage;

    [ObservableProperty]
    private bool _violateCondition = false;

    [ObservableProperty]
    private string _warrningMessage;

    [RelayCommand]
    public async Task Encode()
    {
        App.IsProcessing = true;

        if (ImagesList.Count <= 0)
        {
            ViolateCondition = true;
            WarrningMessage = "You must import Images before Encoding";
        }
        else if (string.IsNullOrWhiteSpace(FolderPath) || string.IsNullOrEmpty(FolderPath))
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
            
            if (ImagesList.Count >= 1000)
            {
                var lists = TOListOfList(ImagesList.ToList());
                await Task.Run(
                () => Parallel.ForEach(lists, async (subList) =>
                {
                    await EncodeBulkView.WebpManager.ScriptRunnerBulk(App.CwebpFilePath, subList, FolderPath, EncodeBulkView.WebpManager.Options);
                }));
            }
            else
            {
                await Task.Run(() => EncodeBulkView.WebpManager.ScriptRunnerBulk(App.CwebpFilePath, ImagesList, FolderPath, EncodeBulkView.WebpManager.Options));
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
        var openPicker = new FileOpenPicker { ViewMode = PickerViewMode.Thumbnail, FileTypeFilter = { ".png", ".jpg", ".webp", ".tif" } };

        var hWnd = WindowNative.GetWindowHandle(App.MWindow);
        InitializeWithWindow.Initialize(openPicker, hWnd);

        var files = await openPicker.PickMultipleFilesAsync();
        int id = 0;
        int voilate = 0;
        int isAnimated = 0;
        if (files != null)
        {
            foreach (var item in files)
            {
                bool check = EncodeBulkView.WebpManager.IsAnimatedWebp(item.Path);
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
            }
        }
        if (voilate > 0)
        {
            PassedTheLimitMessage = $"{voilate} file(s) couldn't be uploaded because they surpassed the 105mb limit";
            PassedTheLimit = true;
        }
        if (isAnimated > 0)
        {
            ViolateCondition = true;
            WarrningMessage = $"{isAnimated} file(s) is animated webp, they can't be encoded";
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
        ImagesList.Remove(SelectedImage);
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

    private static ObservableCollection<ObservableCollection<ImageModel>> TOListOfList(List<ImageModel> ogList)
    {
        ObservableCollection<ObservableCollection<ImageModel>> newlist = [];
        int totalSize = ogList.Count;
        int partSize = totalSize / 3;
        int remainder = totalSize % 3;

        List<ImageModel> list1 = ogList.GetRange(0, partSize + (remainder > 0 ? 1 : 0));
        List<ImageModel> list2 = ogList.GetRange(list1.Count, partSize + (remainder > 1 ? 1 : 0));
        List<ImageModel> list3 = ogList.GetRange(list1.Count + list2.Count, partSize);
        newlist.Add(new ObservableCollection<ImageModel>(list1)); 
        newlist.Add(new ObservableCollection<ImageModel>(list2));
        newlist.Add(new ObservableCollection<ImageModel>(list3));
        return newlist;
    }

}
