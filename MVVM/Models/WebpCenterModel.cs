using SixLabors.ImageSharp;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebpHub.MVVM.Models;

public class WebpCenterModel
{
    public string Options { get; set; } = string.Empty;

    public static bool IsAnimatedWebp(string path)
    {
        var img = SixLabors.ImageSharp.Image.Load(path);
        var meta = img.Metadata;
        var origins = meta.DecodedImageFormat;
        var ext = origins!.FileExtensions;
        string? val = ext.FirstOrDefault(c => c.Contains("webp"));
        if(string.IsNullOrEmpty(val) || string.IsNullOrWhiteSpace(val) || !val.Contains("webp"))
            return false;
        
        var webp = meta.GetWebpMetadata();
        int check = webp.RepeatCount;
        if (check == 0 || check > 1)
            return true;
        else
            return false;
    }

    public static async Task<bool> ScriptRunner(string exe, string input, string folderPath, string options = "")
    {
        var filename = System.IO.Path.GetFileNameWithoutExtension(input);
        var newFilename = System.IO.Path.Combine(folderPath, $"{filename}.webp");

        var info = new ProcessStartInfo
        {
            FileName = exe,
            Arguments = $@" {options} ""{input}"" -o ""{newFilename}"" ",
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        using var proc = Process.Start(info);
        await proc.WaitForExitAsync();
        return true;
    }

    public static async Task<bool> ScriptRunner(string exe, string input, string folderPath, string options, string format)
    {
        var filename = System.IO.Path.GetFileNameWithoutExtension(input);
        var newFilename = System.IO.Path.Combine(folderPath, $"{filename}{format}");

        var info = new ProcessStartInfo
        {
            FileName = exe,
            Arguments = $@" {options} ""{input}"" -o ""{newFilename}"" ",
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        using var proc = Process.Start(info);
        await proc.WaitForExitAsync();
        return true;
    }

    public static async Task ScriptRunnerBulk(string exe, List<ImmutableImageModel> list, string folderPath, string options = "")
    {
        using var proc = new Process();
        proc.StartInfo.FileName = exe;
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.CreateNoWindow = true;
        int time = 3000;
        foreach (var item in list)
        {
            var filename = System.IO.Path.GetFileNameWithoutExtension(item.Location);
            var newFilename = System.IO.Path.Combine(folderPath, $"{filename}.webp");
            proc.StartInfo.Arguments = $@"{options} ""{item.Location}"" -o ""{newFilename}"" ";
            proc.Start();

            if (item.Size < 55_050_240 && item.Size > 20_971_520) // 50mb and 20mb
                time = 5000;
            else if (item.Size > 55_050_240) // above 50mb
                time = 10_000;

            if (!proc.WaitForExit(time))
                proc.Kill();
            time = 3000;
        }
        await Task.CompletedTask;
    }

    public static async Task ScriptRunnerBulk(string exe, List<ImmutableImageModel> list, string folderPath, string format, string options = "")
    {
        using var proc = new Process();
        proc.StartInfo.FileName = exe;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.CreateNoWindow = true;
        int time = 3000;
        foreach (var item in list)
        {
            var filename = System.IO.Path.GetFileNameWithoutExtension(item.Location);
            var newFilename = System.IO.Path.Combine(folderPath, $"{filename}{format}");
            proc.StartInfo.Arguments = $@"{options} ""{item.Location}"" -o ""{newFilename}"" ";
            proc.Start();

            if (item.Size < 55_050_240 && item.Size > 20_971_520) // 50mb and 20mb
                time = 5500;
            else if (item.Size > 55_050_240) // above 50mb
                time = 10_500;

            if (!proc.WaitForExit(time))
                proc.Kill();
            time = 3000;
        }
        await Task.CompletedTask;
    }

}
