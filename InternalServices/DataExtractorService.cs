using Windows.Storage;

namespace WebpHub.InternalServices;

public class DataExtractorService
{
    public string FullPath { get; } = string.Empty;
    public string FileName { get; } = string.Empty;
    public string ImageExtension { get; } = string.Empty;
    public string ImageSize { get; } = string.Empty;
    public string ImageResolution { get; } = string.Empty;
    public string VideoResolution { get; } = string.Empty;

    public DataExtractorService(StorageFile file)
    {
        FullPath = file.Path;
        FileName = file.Name;
        ImageExtension = file.FileType;
        ImageSize = ExtractSize(file);
        ImageResolution = ExtractImageResolution(file);
    }

    private static string ExtractSize(StorageFile file)
    {
        FileInfo fileInfo = new(file.Path);
        string SizeString;

        if (fileInfo.Length >= 1073741824) // 1 GB in bytes
            SizeString = $"{fileInfo.Length / 1073741824.0:f2} GB";
        else if (fileInfo.Length >= 1048576) // 1 MB in bytes
            SizeString = $"{fileInfo.Length / 1048576.0:f2} MB";
        else if (fileInfo.Length >= 1024) // 1 KB in bytes
            SizeString = $"{fileInfo.Length / 1024.0:f2} KB";
        else
            SizeString = $"{fileInfo.Length} bytes";

        return SizeString;
    }

    private static string ExtractImageResolution(StorageFile file)
    {
        var img = SixLabors.ImageSharp.Image.Load(file.Path);
        return $"{img.Width}x{img.Height}";
    }
}
