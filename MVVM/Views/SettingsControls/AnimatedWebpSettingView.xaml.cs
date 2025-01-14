// Ignore Spelling: Webp

using WebpHub.InternalServices;

namespace WebpHub.MVVM.Views.SettingsControls;

public sealed partial class AnimatedWebpSettingView : UserControl
{
    private readonly Gif2WebpOptionsBuilderService _optionsBuilder;
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

