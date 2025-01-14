

namespace WebpHub.MVVM.Models;
public class ImageModel
{
    public string Location { get; set; }
    public int ID { get; set; }
    public long Size { get; set; }

    public ImageModel(string path, int id, long size) => (Location, ID, Size) = (path, id, size);
}
