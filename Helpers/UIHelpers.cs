
namespace WebpHub.Helpers;

public class UIHelpers
{
    public static BitmapImage BitmapImageFromString(string source)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrWhiteSpace(source))
            return new BitmapImage();
        return new BitmapImage { UriSource = new Uri(source) };
    }

    public static Uri UriFromString(string source)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrWhiteSpace(source))
            return new Uri("ms-appx:///Assets/unsopprtedFormat.png");
        return new Uri(source);
    }

}
