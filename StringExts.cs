using System.Drawing.Imaging;

namespace WebpConverter;

public static class StringExts
{
  public static ImageFormat ToImageFormat(this string ext)
  {
    return ext.ToLower() switch
    {
      "bmp" => ImageFormat.Bmp,
      "emf" => ImageFormat.Emf,
      "exif" => ImageFormat.Exif,
      "gif" => ImageFormat.Gif,
      "ico" => ImageFormat.Icon,
      "jpg" => ImageFormat.Jpeg,
      "jpeg" => ImageFormat.Jpeg,
      "png" => ImageFormat.Png,
      "tiff" => ImageFormat.Tiff,
      "wmf" => ImageFormat.Wmf,
    };
  }
}