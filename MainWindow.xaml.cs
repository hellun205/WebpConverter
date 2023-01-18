using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WebpConverter
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private ImageConverter imgConverter = new ImageConverter();
    private string[] exts = new[] {"png", "webp", "jpeg", "jpg", "gif", "bmp"};

    public MainWindow()
    {
      InitializeComponent();
      imgConverter.OnProgress += ImgConverterOnOnProgress;
      imgConverter.OnProgressBar += ImgConverterOnOnProgressBar;
      imgConverter.AfterExecute += ImgConverterOnAfterExecute;
      imgConverter.BeforeExecute += ImgConverterOnBeforeExecute;

      foreach (var ext in exts)
      {
        cbbBeforeExt.Items.Add(ext);
        cbbAfterExt.Items.Add(ext);
      }

      cbbBeforeExt.SelectedValue = "webp";
      cbbAfterExt.SelectedValue = "jpg";
    }

    private void ImgConverterOnBeforeExecute()
    {
      Dispatcher.BeginInvoke(DispatcherPriority.Background, () => { SetEnabled(false); });
    }

    private void ImgConverterOnAfterExecute()
    {
      Dispatcher.BeginInvoke(DispatcherPriority.Background, () => { SetEnabled(true); });
    }

    private void SetEnabled(bool status)
    {
      btnExecute.IsEnabled = status;
      tbPath.IsEnabled = status;
      cbSubfolders.IsEnabled = status;
      cbbAfterExt.IsEnabled = status;
      cbbBeforeExt.IsEnabled = status;
      btnOpen.IsEnabled = status;
    }

    private void ImgConverterOnOnProgressBar(int max, int value)
    {
      Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
      {
        pbProgress.Maximum = max;
        pbProgress.Value = value;
      });
    }

    private void ImgConverterOnOnProgress(string statustext)
    {
      Dispatcher.BeginInvoke(DispatcherPriority.Background, () => { lbProgress.Text = statustext; });
    }

    private void BtnOpen_OnClick(object sender, RoutedEventArgs e)
    {
      var dialog = new System.Windows.Forms.FolderBrowserDialog();

      if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        tbPath.Text = dialog.SelectedPath;
      }
    }

    private async void BtnExecute_OnClick(object sender, RoutedEventArgs e)
    {
      imgConverter.Execute(tbPath.Text, cbbBeforeExt.Text, cbbAfterExt.Text, cbSubfolders.IsChecked.Value,
        cbDeleteOriginals.IsChecked.Value);
    }
  }
}