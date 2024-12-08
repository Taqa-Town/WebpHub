using MetadataExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    private string ExtractSize(StorageFile file)
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

    private string ExtractImageResolution(StorageFile file)
    {
        var dirs = ImageMetadataReader.ReadMetadata(file.Path);

        var selectedDirHeight = dirs.SelectMany(dir => dir.Tags)
            .Where(tag => tag.Name.Contains("Height"));

        var selectedDirWidth = dirs.SelectMany(dir => dir.Tags)
            .Where(tag => tag.Name.Contains("Width"));

        Tag tagHeight = selectedDirHeight.FirstOrDefault(tag => tag.Name.Contains("Height"));
        Tag tagWidth = selectedDirWidth.FirstOrDefault(tag => tag.Name.Contains("Width"));

        string Height = tagHeight.Description.Replace("pixels", "").Trim();
        string Width = tagWidth.Description.Replace("pixels", "").Trim();

        return $"{Width}x{Height}";
    }
}
