using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace WebpConverter;

public class ImageConverter
{
  private List<string> pathList = new List<string>();
  private List<string> failPathList = new List<string>();
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

  public void ConvertImage(string path, string afterExt)
  {
    var fileName = Path.GetFileNameWithoutExtension(path);
    OnProgress?.Invoke("convert: " + path);

    try
    {
      using (Image img = Image.FromFile(path))
      {
        img.Save($"{new FileInfo(path).Directory.FullName}\\{fileName}.{afterExt}", afterExt.ToImageFormat());
      }
    }
    catch
    {
      failPathList.Add(path);
      // MessageBox.Show($"Failed to convert image: {path}", "Error");
    }

    progress += 8;
    OnProgressBar?.Invoke(progressMax, progress);
  }

  private void FindImages(string path, string ext, bool subfolders)
  {
    try
    {
      string[] dirs = Directory.GetDirectories(path);
      string[] files = Directory.GetFiles(path, $"*.{ext}");
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

  private void ConvertImages(string convertToExt)
  {
    foreach (var file in pathList)
    {
      ConvertImage(file, convertToExt);
    }
  }

  private void DeleteImages()
  {
    pathList.RemoveAll(path => failPathList.Contains(path));
    foreach (var file in pathList)
    {
      OnProgress?.Invoke("delete: " + file);
      File.Delete(file);
      progress += 1;
      OnProgressBar?.Invoke(progressMax, progress);
    }
  }

  public void Execute(string path, string originalExt, string convertToExt, bool subfolders)
  {
    Thread findTask = new Thread(new ThreadStart(() =>
    {
      BeforeExecute?.Invoke();
      FindImages(path, originalExt, subfolders);
      ConvertImages(convertToExt);
      DeleteImages();
      if (failPathList.Count > 0)
      {
        OnProgress?.Invoke($"convert completed with fail: {failPathList.Count}");
        StringBuilder sb = new StringBuilder();
        foreach (var failPath in failPathList)
          sb.Append("\n").Append(failPath);
        MessageBox.Show($"convert fail:{sb.ToString()}", "fail", MessageBoxButton.OK, MessageBoxImage.Information);
      }
      else
      {
        OnProgress?.Invoke("convert completed!");
      }
      AfterExecute?.Invoke();

      Clear();
    }));


    findTask.Start();
  }

  public void Clear()
  {
    pathList.Clear();
    failPathList.Clear();
    progressMax = 0;
    progress = 0;
  }
}