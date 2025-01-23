// Ignore Spelling: img

namespace WebpHub.MVVM.Models;
public class ImageModel
{
    public string Location { get; init; }
    public int ID { get; init; }
    public long Size { get; init; }

    public ImageModel(string path, int id, long size) => (Location, ID, Size) = (path, id, size);
}
