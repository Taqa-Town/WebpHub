using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebpHub.MVVM.Models;

public class ImageModel
{
    public string Location { get; set; }
    public int ID { get; set; }
    public long Size { get; set; }

    public ImageModel(string path, int id, long size) => (Location, ID, Size) = (path, id, size);
}
