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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WebpHub.MVVM.Views.SettingsControls;

public sealed partial class AnimatedWebpSettingView : UserControl
{
    private Gif2WebpOptionsBuilderService _optionsBuilder;
    private readonly DispatcherTimer timer;

    public AnimatedWebpSettingView()
    {
        InitializeComponent();
        _optionsBuilder = new();
        timer = new();
    }

    private void SaveBtn_Click(object sender, RoutedEventArgs e)
    {
        _optionsBuilder.Quality = $"-q {QualityTxt.Text}";

        var CompressionMethodItem = CompressionMethod.SelectedItem as ComboBoxItem;
        if (CompressionMethodItem != null)
            _optionsBuilder.CompressionMethod = $"-m {(string)CompressionMethodItem.Content}";

        if (MixedCheck.IsChecked == true)
            _optionsBuilder.Mixed = "-mixed";

        if (MultiThreadedCheck.IsChecked == true)
            _optionsBuilder.MultiThreading = "-mt";

        if (LoopCompatCheck.IsChecked == true)
            _optionsBuilder.LoopCompatibility = "-loop_compatibility";

        if (MinSizeCheck.IsChecked == true)
            _optionsBuilder.MinSize = "-min_size";

        if (LossyCheck.IsEnabled == true && LossyCheck.IsChecked == true)
            _optionsBuilder.Lossy = "-lossy";

        if (DeblockingTxt.IsEnabled == true)
            _optionsBuilder.LossyDeblockingFilterStrength = $"-f {DeblockingTxt.Text}";

        if (KmaxTxt.IsEnabled == true)
            _optionsBuilder.KMax = $"-kmax {KmaxTxt.Text}";

        if (KminTxt.IsEnabled == true)
            _optionsBuilder.KMin = $"-kmin {KminTxt.Text}";

        AnimatedWebpView.WebpManager.Options = _optionsBuilder.ConstructOptions();

        SaveBtn.Content = "Saved!";
        timer.Interval = TimeSpan.FromSeconds(2);
        timer.Tick += Time_Tick;
        timer.Start();
    }

    private void Time_Tick(object sender, object e)
    {
        SaveBtn.Content = "Save";
        timer.Stop();
    }

    private void MixedCheck_Checked(object sender, RoutedEventArgs e)
    {
        DeblockingTxt.IsEnabled = false;
        LossyCheck.IsEnabled = false;
    }

    private void MixedCheck_Unchecked(object sender, RoutedEventArgs e)
    {
        DeblockingTxt.IsEnabled = true;
        LossyCheck.IsEnabled = true;
    }

    private void LossyCheck_Checked(object sender, RoutedEventArgs e)
    {
        DeblockingTxt.IsEnabled = true;
    }

    private void LossyCheck_Unchecked(object sender, RoutedEventArgs e)
    {
        DeblockingTxt.IsEnabled = false;
    }

    private void KeyFrameCheck_Checked(object sender, RoutedEventArgs e)
    {
        KminTxt.IsEnabled = true;
        KmaxTxt.IsEnabled = true;
    }

    private void KeyFrameCheck_Unchecked(object sender, RoutedEventArgs e)
    {
        KminTxt.IsEnabled = false;
        KmaxTxt.IsEnabled = false;
    }
}

