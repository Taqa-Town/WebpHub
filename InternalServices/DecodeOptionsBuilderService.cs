namespace WebpHub.InternalServices;

public class DecodeOptionsBuilderService
{
    public string NoDither { get; set; } = "-nodither";
    public string Dither { get; set; } = string.Empty;
    public string NoFancy { get; set; } = string.Empty;
    public string NoFilter { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;

    public string MultiThreading { get; set; } = string.Empty;
    public string NoAsm { get; set; } = string.Empty;

    public string ConstructOptions()
    {
        return $"{NoDither} {Dither} {NoFancy} {NoFilter} {Format} {MultiThreading} {NoAsm}";
    }
}
