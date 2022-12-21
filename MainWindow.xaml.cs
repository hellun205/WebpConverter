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
using GroupDocs.Conversion.FileTypes;

namespace WebpConverter
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private ImageConverter imgConverter = new ImageConverter();

    public MainWindow()
    {
      InitializeComponent();
      imgConverter.OnProgress += ImgConverterOnOnProgress;
      imgConverter.OnProgressBar += ImgConverterOnOnProgressBar;
      foreach (var ext in ImageFileType.GetAll<ImageFileType>())
      {
        cbbBeforeExt.Items.Add(ext.Extension);
        cbbAfterExt.Items.Add(ext.Extension);
      }

      cbbBeforeExt.SelectedValue = ImageFileType.Webp.Extension;
      cbbAfterExt.SelectedValue = ImageFileType.Jpg.Extension;
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
      Dispatcher.BeginInvoke(DispatcherPriority.Background, () => { lbProgress.Content = statustext; });
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
      Task convert = imgConverter.Execute(tbPath.Text, (ImageFileType) (ImageFileType.FromExtension(cbbBeforeExt.Text)),
        (ImageFileType) (ImageFileType.FromExtension(cbbAfterExt.Text)), cbSubfolders.IsChecked.Value);

      convert.Start();
    }

   
  }
}