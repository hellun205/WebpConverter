using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using GroupDocs.Conversion;
using GroupDocs.Conversion.FileTypes;
using GroupDocs.Conversion.Options.Convert;

namespace WebpConverter;

public class ImageConverter
{
  private delegate void tmx(string path, ImageFileType ext, bool subfol);

  private List<string> pathList = new List<string>();
  private int progressMax;
  private int progress;

  public delegate void _onProgressed(string statusText);

  public delegate void _onProgressedBar(int max, int value);

  public event _onProgressed? OnProgress;
  public event _onProgressedBar? OnProgressBar;

  public void ConvertImage(string path, ImageFileType before, ImageFileType convertTo)
  {
    var fileInfo = new FileInfo(path);
    var fileName = fileInfo.Name.Replace($".{before.Extension}", "");
    var dirPath = fileInfo.Directory.FullName;

    OnProgress?.Invoke("convert: " + path);

    using (Converter converter = new Converter(path))
    {
      ImageConvertOptions options = new ImageConvertOptions
      {
        // Set the conversion format to JPG
        Format = convertTo
      };
      converter.Convert($"{dirPath}\\{fileName}.{convertTo.Extension}", options);
    }

    progress += 1;
    OnProgressBar?.Invoke(progressMax, progress);
  }

  private async Task FindImages(string path, ImageFileType ext, bool subfolders)
  {
    try
    {
      string[] dirs = Directory.GetDirectories(path);
      string[] files = Directory.GetFiles(path, $"*.{ext.Extension}");
      pathList.AddRange(files);

      foreach (var fPath in files)
      {
        OnProgress?.Invoke("find: " + fPath);
        progressMax += 2;
        progress += 1;
        OnProgressBar?.Invoke(progressMax, progress);
      }

      if (subfolders == true && dirs.Length > 0)
      {
        foreach (string dir in dirs)
        {
          FindImages(dir, ext, true);
        }
      }
    }
    catch (Exception ex)
    {
    }
  }

  private async Task ConvertImages(ImageFileType originalExt, ImageFileType convertToExt)
  {
    foreach (var file in pathList)
    {
      ConvertImage(file, originalExt, convertToExt);
    }
  }

  private async Task DeleteImages()
  {
    foreach (var file in pathList)
    {
      File.Delete(file);
      OnProgress?.Invoke("delete: " + file);
      progress += 1;
      OnProgressBar?.Invoke(progressMax, progress);
    }
  }

  public async Task Execute(string path, ImageFileType originalExt, ImageFileType convertToExt, bool subfolders)
  {
    Task findTask = FindImages(path, originalExt, subfolders);
    Task convertTask = ConvertImages(originalExt, convertToExt);
    Task deleteTask = DeleteImages();

    await findTask;
    await convertTask;
    await deleteTask;
  }
}