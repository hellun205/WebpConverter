using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GroupDocs.Conversion;
using GroupDocs.Conversion.FileTypes;
using GroupDocs.Conversion.Options.Convert;

namespace WebpConverter;

public class ImageConverter
{
  private List<string> pathList = new List<string>();
  private int progressMax;
  private int progress;

  public delegate void _onProgressed(string statusText);
  public delegate void _onProgressedBar(int max, int value);
  public delegate void _beforeExecute();
  public delegate void _afterExecute();

  public event _onProgressed? OnProgress;
  public event _onProgressedBar? OnProgressBar;
  public event _beforeExecute? BeforeExecute;
  public event _afterExecute? AfterExecute;

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

    progress += 8;
    OnProgressBar?.Invoke(progressMax, progress);
  }

  private void FindImages(string path, ImageFileType ext, bool subfolders)
  {
    try
    {
      string[] dirs = Directory.GetDirectories(path);
      string[] files = Directory.GetFiles(path, $"*.{ext.Extension}");
      pathList.AddRange(files);

      foreach (var fPath in files)
      {
        OnProgress?.Invoke("find: " + fPath);
        progressMax += 10;
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

  private void ConvertImages(ImageFileType originalExt, ImageFileType convertToExt)
  {
    foreach (var file in pathList)
    {
      ConvertImage(file, originalExt, convertToExt);
    }
  }

  private void DeleteImages()
  {
    foreach (var file in pathList)
    {
      File.Delete(file);
      OnProgress?.Invoke("delete: " + file);
      progress += 1;
      OnProgressBar?.Invoke(progressMax, progress);
    }
  }

  public void Execute(string path, ImageFileType originalExt, ImageFileType convertToExt, bool subfolders)
  {
    Thread findTask = new Thread(new ThreadStart(() =>
    {
      BeforeExecute?.Invoke();
      FindImages(path, originalExt, subfolders);
      ConvertImages(originalExt, convertToExt);
      DeleteImages();
      OnProgress?.Invoke("convert completed!");
      AfterExecute?.Invoke();
      Clear();
    }));


    findTask.Start();
  }

  public void Clear()
  {
    pathList.Clear();
    progressMax = 0;
    progress = 0;
  }
}