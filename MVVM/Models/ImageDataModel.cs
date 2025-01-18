
using WebpHub.InternalServices;

namespace WebpHub.MVVM.Models;

[WinRT.GeneratedBindableCustomProperty]
public partial class ImageDataModel
{
    public string FullPath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ImageExtension { get; set; } = string.Empty;
    public string ImageSize { get; set; } = string.Empty;
    public string ImageResolution { get; set; } = string.Empty;
    public ImageDataModel() { }


    public ImageDataModel(DataExtractorService data)
    {
        FullPath = data.FullPath;
        FileName = data.FileName;
        ImageExtension = data.ImageExtension;
        ImageSize = data.ImageSize;
        ImageResolution = data.ImageResolution;
    }
}
