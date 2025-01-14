

namespace WebpHub.InternalServices;

public class EncodeOptionsBuilderService
{
    public string Quality { get; set; } = "-q 75";

    public string AlphaQuality { get; set; } = "-alpha_q 100";
    public string Lossless { get; set; } = string.Empty;
    public string NearLossless { get; set; } = string.Empty;
    public string Preset { get; set; } = string.Empty;
    public string CompressionMethod { get; set; } = "-m 4";
    public string Exact { get; set; } = string.Empty;
    public string MultiThreading { get; set; } = string.Empty;
    public string LossySize { get; set; } = string.Empty;

    public string LossyPsnr { get; set; } = string.Empty;

    public string LossyPass { get; set; } = string.Empty;

    public string LossyAutofilter { get; set; } = string.Empty;

    public string LossyJpegLike { get; set; } = string.Empty;

    public string DeblockingFilterStrength { get; set; } = "-f 50";

    public string Sharpness { get; set; } = "-sharpness 0";

    public string Strong { get; set; } = string.Empty;
    public string NonStrong { get; set; } = string.Empty;

    public string SharpYuv { get; set; } = string.Empty;

    public string Segments { get; set; } = "-segments 4";
    public string LowMemory { get; set; } = string.Empty;
    public string PartitionLimit { get; set; } = string.Empty;
    public string SNS { get; set; } = string.Empty;

    public string AlphaFilter { get; set; } = "-alpha_filter fast";
    public string AlphaMethod { get; set; } = "-alpha_method 1";
    public string NoAlpha { get; set; } = string.Empty;
    public string NoAsm { get; set; } = string.Empty;

    public string ConstructOptions()
    {
        return $"{Quality} {AlphaQuality} {Lossless} {NearLossless} {Preset} {CompressionMethod} {Exact} {MultiThreading} {LossySize} {LossyPsnr} {LossyPass} {LossyAutofilter} {LossyJpegLike} {DeblockingFilterStrength} {Sharpness} {Strong} {NonStrong} {SharpYuv} {Segments} {LowMemory} {PartitionLimit} {SNS} {AlphaFilter} {AlphaMethod} {NoAlpha} {NoAsm}";
    }
}
