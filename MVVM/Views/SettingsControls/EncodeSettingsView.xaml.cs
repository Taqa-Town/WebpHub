using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WebpHub.InternalServices;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace WebpHub.MVVM.Views.SettingsControls;

public sealed partial class EncodeSettingsView : UserControl
{
    public EncodeOptionsBuilderService OptionsBuilder { get; private set; }
    private readonly DispatcherTimer timer;

    public EncodeSettingsView()
    {
        InitializeComponent();
        OptionsBuilder = new();
        timer = new();
    }

    private void SaveBtn_Click(object sender, RoutedEventArgs e)
    {
        OptionsBuilder.Quality = $"-q {QualityTxt.Text}";

        var CompressionMethodItem = CompressionMethod.SelectedItem as ComboBoxItem;
        OptionsBuilder.CompressionMethod = $"-m {(string)CompressionMethodItem.Content}";

        var PresetMethodItem = PresetMethod.SelectedItem as ComboBoxItem;
        if (PresetMethodItem != null)
            OptionsBuilder.Preset = $"-preset {(string)PresetMethodItem.Content}";

        if (ExactCheck.IsChecked == true)
            OptionsBuilder.Exact = "-exact";

        if (MultiThreadedCheck.IsChecked == true)
            OptionsBuilder.Exact = "-mt";

        if (NoASMCheck.IsChecked == true)
            OptionsBuilder.Exact = "-noasm";

        if (LowMemoryCheck.IsChecked == true)
            OptionsBuilder.Exact = "-low_memory";

        if (LosslessCheck.IsChecked == true)
            OptionsBuilder.Exact = "-lossless";

        if (NearLosslessCheck.IsChecked == true)
            OptionsBuilder.NearLossless = $"near_lossless {NearLosslessTxt.Text}";

        if (AlphaQualityTxt.IsEnabled == true)
            OptionsBuilder.AlphaQuality = $"-alpha_q {AlphaQualityTxt.Text}";

        if (AlphaMethod.IsEnabled == true)
        {
            var AlphaMethodItem = AlphaMethod.SelectedItem as ComboBoxItem;
            OptionsBuilder.CompressionMethod = $"-alpha_method {(string)AlphaMethodItem.Content}";
        }

        if (AlphaFilterMethod.IsEnabled == true)
        {
            var AlphaFilterMethodItem = AlphaFilterMethod.SelectedItem as ComboBoxItem;
            OptionsBuilder.CompressionMethod = $"-alpha_filter {(string)AlphaFilterMethodItem.Content}";
        }

        //Lossy Options section
        if (SizeTxt.IsEnabled == true)
            OptionsBuilder.LossySize = $"-size {SizeTxt.Text}";

        if (PSNRTxt.IsEnabled == true)
            OptionsBuilder.LossyPsnr = $"-psnr {PSNRTxt.Text}";

        if (JpegLikeCheck.IsEnabled == true && JpegLikeCheck.IsChecked == true)
            OptionsBuilder.LossyJpegLike = "-jpeg_like";

        if (AutoFilterCheck.IsEnabled == true && AutoFilterCheck.IsChecked == true)
            OptionsBuilder.LossyAutofilter = "-af";

        if (PassMethod.IsEnabled == true)
        {
            var PassMethodItem = PassMethod.SelectedItem as ComboBoxItem;
            OptionsBuilder.LossyPass = $"-pass {(string)PassMethodItem.Content}";
        }

        //advanced Options

        if (DeblockingStrengthTxt.IsEnabled == true)
            OptionsBuilder.DeblockingFilterStrength = $"-f {DeblockingStrengthTxt.Text}";

        if (EnableStrong.IsEnabled == true && EnableStrong.IsChecked == true)
            OptionsBuilder.Strong = "-strong";

        if (DisableStrong.IsEnabled == true && DisableStrong.IsChecked == true)
            OptionsBuilder.NonStrong = "-nostrong";

        if (SharpnessMethod.IsEnabled == true)
        {
            var SharpnessMethodItem = SharpnessMethod.SelectedItem as ComboBoxItem;
            OptionsBuilder.Sharpness = $"-sharpness {(string)SharpnessMethodItem.Content}";
        }

        if (SharpYUVCheck.IsEnabled == true && SharpYUVCheck.IsChecked == true)
            OptionsBuilder.SharpYuv = "-sharp_yuv";

        if (SNSTxt.IsEnabled == true)
            OptionsBuilder.SNS = $"-sns {SNSTxt.Text}";

        if (SegmentsMethod.IsEnabled == true)
        {
            var SegmentsMethodItem = SegmentsMethod.SelectedItem as ComboBoxItem;
            OptionsBuilder.Segments = $"-segments {(string)SegmentsMethodItem.Content}";
        }

        if (PartitionLimitTxt.IsEnabled == true)
            OptionsBuilder.PartitionLimit = $"-partition_limit {PartitionLimitTxt.Text}";

        EncodeView.WebpManager.Options = OptionsBuilder.ConstructOptions();

        buttonText.Text = "Saved!";
        timer.Interval = TimeSpan.FromSeconds(2);
        timer.Tick += Time_Tick;
        timer.Start();
    }

    private void Time_Tick(object? sender, object e)
    {
        buttonText.Text = "Save";
        timer.Stop();
    }

    private void NoAlphaCheck_Checked(object sender, RoutedEventArgs e)
    {
        AlphaQualityTxt.IsEnabled = false;
        AlphaMethod.IsEnabled = false;
        AlphaFilterMethod.IsEnabled = false;
    }

    private void NoAlphaCheck_Unchecked(object sender, RoutedEventArgs e)
    {
        AlphaQualityTxt.IsEnabled = true;
        AlphaMethod.IsEnabled = true;
        AlphaFilterMethod.IsEnabled = true;
    }

    private void NearLosslessCheck_Checked(object sender, RoutedEventArgs e) => NearLosslessTxt.IsEnabled = true;

    private void NearLosslessCheck_Unchecked(object sender, RoutedEventArgs e) => NearLosslessTxt.IsEnabled = false;

    private void LosslessCheck_Checked(object sender, RoutedEventArgs e)
    {
        //disable lossy options
        SizeTxt.IsEnabled = false;
        PSNRTxt.IsEnabled = false;
        PassMethod.IsEnabled = false;
        JpegLikeCheck.IsEnabled = false;
        AutoFilterCheck.IsEnabled = false;
        NearLosslessCheck.IsEnabled = false;
    }

    private void LosslessCheck_Unchecked(object sender, RoutedEventArgs e)
    {
        //enable lossy options
        SizeTxt.IsEnabled = true;
        PSNRTxt.IsEnabled = true;
        PassMethod.IsEnabled = true;
        JpegLikeCheck.IsEnabled = true;
        AutoFilterCheck.IsEnabled = true;
        NearLosslessCheck.IsEnabled = true;
    }

    private void AdvancedCheck_Checked(object sender, RoutedEventArgs e)
    {
        DeblockingStrengthTxt.IsEnabled = true;
        StrenghtGroup.IsEnabled = true;
        EnableStrong.IsChecked = true;
        SharpnessMethod.IsEnabled = true;
        SharpYUVCheck.IsEnabled = true;
        SNSTxt.IsEnabled = true;
        SegmentsMethod.IsEnabled = true;
        PartitionLimitTxt.IsEnabled = true;
    }

    private void AdvancedCheck_Unchecked(object sender, RoutedEventArgs e)
    {
        DeblockingStrengthTxt.IsEnabled = false;
        StrenghtGroup.IsEnabled = false;
        EnableStrong.IsChecked = false;
        SharpnessMethod.IsEnabled = false;
        SharpYUVCheck.IsEnabled = false;
        SNSTxt.IsEnabled = false;
        SegmentsMethod.IsEnabled = false;
        PartitionLimitTxt.IsEnabled = false;
    }

}
