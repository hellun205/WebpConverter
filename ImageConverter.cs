
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GroupDocs.Conversion;
using GroupDocs.Conversion.FileTypes;
using GroupDocs.Conversion.Options.Convert;

namespace WebpConverter;

public class ImageConverter
{
  private List<string> pathList = new List<string>();
  public int progressMax;
  public int progress;

  public static void ConvertImage(string path, ImageFileType convertTo)
  {
    var fileInfo = new FileInfo(path);
    var fileName = fileInfo.Name;
    var dirPath = fileInfo.Directory.FullName;
    
    using (Converter converter = new Converter(path))
    {
      ImageConvertOptions options = new ImageConvertOptions
      { // Set the conversion format to JPG
        Format = convertTo
      };
      converter.Convert($"{dirPath}\\{fileName}.{convertTo.Extension}", options);
    }
  }

  public void FindImg(string path, ImageFileType ext, bool subfolders)
  {
    pathList.Clear();
    FindImage(path, ext, subfolders);
  }

  private bool FindImage(string path, ImageFileType ext, bool subfolders)
  {
    try
    {
      string[] dirs = Directory.GetDirectories(path);
      string[] files = Directory.GetFiles(path, $"*.{ext.Extension}");
      pathList.AddRange(files);
                    
      if (subfolders == true && dirs.Length > 0)
      {
        foreach(string dir in dirs)
        {
          var a = FindImage(dir, ext, true);
          Console.WriteLine($"{dir}: {a}");
        }
      }

      return true;
    }
    catch(Exception ex)
    {
      return false;
    }  
  }

  public void Execute(string path, ImageFileType originalExt, ImageFileType convertToExt, bool subfolders)
  {
    Task findTask = new Task(() =>
    {
      FindImg(path, originalExt, subfolders);
    });
    Task convertTask = new Task(() =>
    {
      foreach (var file in pathList)
      {
        ConvertImage(file, originalExt);
      }
    });
    Task deleteTask = new Task(() =>
    {
      foreach (var file in pathList)
      {
        File.Delete(file);
      }
    });

    findTask.Start();
    findTask.Wait();
    
    convertTask.Start();
    convertTask.Wait();
    
    deleteTask.Start();
    deleteTask.Wait();
  }
  
  
}