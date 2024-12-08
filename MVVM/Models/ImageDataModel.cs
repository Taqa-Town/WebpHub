using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebpHub.InternalServices;

namespace WebpHub.MVVM.Models;

public class ImageDataModel
{
    public string FullPath { get; set; }
    public string FileName { get; set; }
    public string ImageExtension { get; set; }
    public string ImageSize { get; set; }
    public string ImageResolution { get; set; }

    public ImageDataModel(DataExtractorService data)
    {
        FullPath = data.FullPath;
        FileName = data.FileName;
        ImageExtension = data.ImageExtension;
        ImageSize = data.ImageSize;
        ImageResolution = data.ImageResolution;
    }
}
