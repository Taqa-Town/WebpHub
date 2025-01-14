// Ignore Spelling: Gif Webp Deblocking


namespace WebpHub.InternalServices;

public class Gif2WebpOptionsBuilderService
{
    public string Quality { get; set; } = "-q 75";
    public string Mixed { get; set; } = string.Empty;
    public string Lossy { get; set; } = string.Empty;
    public string MinSize { get; set; } = string.Empty;
    public string CompressionMethod { get; set; } = "-m 4";
    public string KMin { get; set; } = string.Empty;
    public string KMax { get; set; } = string.Empty;
    public string LossyDeblockingFilterStrength { get; set; } = string.Empty;
    public string MultiThreading { get; set; } = string.Empty;

    public string LoopCompatibility { get; set; } = string.Empty;

    public string ConstructOptions()
    {
        return $"{Quality} {Mixed} {Lossy} {MinSize} {CompressionMethod} {KMin} {KMax} {LossyDeblockingFilterStrength} {MultiThreading} {LoopCompatibility}";
    }

}