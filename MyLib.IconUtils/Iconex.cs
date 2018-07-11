using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using Size = System.Drawing.Size;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace MyLib.IconUtils
{
  /// <summary>
  /// Klasa reprezentująca ikonę
  /// </summary>
  [DataContract]
  public class Iconex: ICloneable
  {
    public Iconex()
    {
      Images = new IconImagesCollection();
    }

    /// <summary>
    /// Klonowanie ikony (kopia zawartości)
    /// </summary>
    /// <returns></returns>
    public object Clone()
    {
      Iconex newIcon = new Iconex();
      newIcon.Name = this.Name;
      newIcon.OriginalImageType = this.OriginalImageType;
      newIcon.OriginalImagesCount = this.OriginalImagesCount;
      foreach (IconImage item in Images)
      {        
        newIcon.Images.Add(new IconImage { 
          Size = item.Size, 
          ColorDepth = item.ColorDepth,
          Image = (Bitmap)item.Image.Clone() });
      }
      return newIcon;
    }

    ///// <summary>
    ///// Klonowanie ikony z dodaniem ozdobników
    ///// </summary>
    ///// <param name="adornments">nazwy ozdobników:"box"</param>
    ///// <returns></returns>
    //public IconImg CloneWithAdornments(string[] adornments)
    //{
    //  IconImg newIcon = new IconImg();
    //  newIcon.Name = this.Name;
    //  foreach (IconImage item in Images)
    //  {
    //    byte[] newBytes = new byte[item.Image.Length];
    //    item.Image.CopyTo(newBytes, 0);
    //    foreach (string adornment in adornments)
    //    {
    //      switch (adornment.ToLower())
    //      {
    //        case "box":
    //          newBytes = BoxImage(item.Size, newBytes);
    //          break;
    //      }
    //    }
    //    newIcon.Images.Add(new IconImage { Size = item.Size, Image = newBytes });
    //  }
    //  return newIcon;
    //}

    ///// <summary>
    ///// Klonowanie ikony z nałożeniem obrazów z innej ikony
    ///// </summary>
    ///// <param name="otherIcon">nazwy ozdobników:"box"</param>
    ///// <returns></returns>
    //public IconImg CloneWithMerge(IconImg otherIcon)
    //{
    //  IconImg newIcon = new IconImg();
    //  newIcon.Name = this.Name;
    //  foreach (IconImage thisImage in Images)
    //  {
    //    Bitmap newBitmap = (Bitmap)thisImage.Image.Clone();
    //    IconImage otherImage = otherIcon.SelectImage(thisImage.Size, thisImage.ColorDepth);
    //    Bitmap otherBitmap = otherImage.Image;
    //    if (otherImage != null)
    //    {
    //      using (var canvas = Graphics.FromImage(newBitmap))
    //      {
    //        canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
    //        canvas.DrawImage(otherBitmap,
    //                         new Rectangle(0,
    //                                       0,
    //                                       newBitmap.Width,
    //                                       newBitmap.Height),
    //                         new Rectangle(0,
    //                                       0,
    //                                       otherBitmap.Width,
    //                                       otherBitmap.Height),
    //                         GraphicsUnit.Pixel);
    //      }
    //    }        
    //    newIcon.Images.Add(new IconImage { Size = thisImage.Size, Image = newBitmap });
    //  }
    //  return newIcon;
    //}

    /// <summary>
    /// Nazwa ikony
    /// </summary>
    [DataMember]
    public string Name { get; set; }

    /// <summary>
    /// Oryginalny typ obrazu
    /// </summary>
    [DataMemberAttribute]
    public string OriginalImageType { get; set; }

    /// <summary>
    /// Oryginalna liczba obrazów
    /// </summary>
    [DataMemberAttribute]
    public int OriginalImagesCount { get; set; }


    /// <summary>
    /// Obrazy w różnych rozmiarach
    /// </summary>
    [DataMember]
    public IconImagesCollection Images { get; set; }


    /// <summary>
    /// Czy ikona jest zdefiniowana w określonym rozmiarze
    /// </summary>
    /// <param name="size">rozmiar (aktualnie 16 albo 32)</param>
    public bool HasImage(int size)
    {
      return Images.FirstOrDefault(item =>item.Size == size)!=null;
    }

    /// <summary>
    /// Czy ikona jest zdefiniowana w określonym rozmiarze z określoną głębią kolorów
    /// </summary>
    /// <param name="size">rozmiar (aktualnie 16 albo 32)</param>
    /// <param name="colorDepth">liczba bitów na piksel</param>
    public bool HasImage(int size, ColorDepth colorDepth)
    {
      return Images.FirstOrDefault(item => item.Size == size && item.ColorDepth == colorDepth) != null;
    }
    
    /// <summary>
    /// Podanie obrazu o określonym rozmiarze. 
    /// Jeśli jest kilka obrazów o tym samym rozmiarze,
    /// to zwracany jest obraz o największej głębi kolorów
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public IconImage GetImage(int size)
    {
      return Images.Where(item => item.Size == size).OrderByDescending(item => item.ColorDepth).FirstOrDefault();
    }

    /// <summary>
    /// Podanie obrazu o określonym rozmiarze i głębi kolorów
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public IconImage GetImage(int size, ColorDepth colorDepth)
    {
      return Images.FirstOrDefault(item => item.Size == size && item.ColorDepth == colorDepth);
    }

    /// <summary>
    /// Podanie obrazu o określonym rozmiarze. 
    /// Jeśli nie ma obrazu o takim samym rozmiarze, to wyszukiwany jest najmniejszy obraz o rozmiarze większym,
    /// a jeśli i takiego obrazu nie ma, to wyszukiwany jest największy obraz o rozmiarze mniejszym od pożądanego,
    /// Jeśli powyższe warunki spełnia wiele obrazów, to podawany jest obraz o największej głębi kolorów.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public IconImage SelectImage(int size)
    {
      IconImage result = Images.Where(item => item.Size == size).OrderByDescending(item => item.ColorDepth).FirstOrDefault();
      if (result != null)
        return result;
      result = Images.Where(item => item.Size >= size).OrderByDescending(item => item.ColorDepth).FirstOrDefault();
      if (result != null)
        return result;
      result = Images.Where(item => item.Size <= size).OrderByDescending(item => item.ColorDepth).FirstOrDefault();
      return result;
    }

    /// <summary>
    /// Podanie obrazu o określonym rozmiarze i określonej głębi kolorów.
    /// Jeśli takiego obrazu nie ma, to wyszukiwany jest obraz o tym samym rozmiarze, 
    /// ale innej (możliwie największej) głębi kolorów.
    /// Jeśli w ogóle nie ma obrazu o takim samym rozmiarze, to wyszukiwany jest najmniejszy obraz o rozmiarze większym,
    /// i o tej samej lub możliwie największej głębi kolorów.
    /// a jeśli i takiego obrazu nie ma, to wyszukiwany jest największy obraz o rozmiarze mniejszym od pożądanego
    /// i o tej samej lub możliwie największej głębi kolorów.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public IconImage SelectImage(int size, ColorDepth colorDepth)
    {
      IconImage result = Images.FirstOrDefault(item => item.Size == size && item.ColorDepth==colorDepth);
      if (result != null)
        return result;
      result = Images.Where(item => item.Size == size).OrderByDescending(item => item.ColorDepth).FirstOrDefault();
      if (result != null)
        return result;
      result = Images.FirstOrDefault(item => item.Size >= size && item.ColorDepth == colorDepth);
      if (result != null)
        return result;
      result = Images.Where(item => item.Size >= size).OrderByDescending(item => item.ColorDepth).FirstOrDefault();
      if (result != null)
        return result;
      result = Images.FirstOrDefault(item => item.Size <= size && item.ColorDepth == colorDepth);
      if (result != null)
        return result;
      result = Images.Where(item => item.Size <= size).OrderByDescending(item => item.ColorDepth).FirstOrDefault();
      return result;
    }

    /// <summary>
    /// Dodanie obrazu o określonym rozmiarze
    /// </summary>
    /// <param name="size"></param>
    /// <param name="image"></param>
    public IconImage AddImage(int size, Bitmap image)
    {
      if (Images == null)
        Images = new IconImagesCollection();
      int colorBpp = IconTools.PixelFormat2ColorDepth(image.PixelFormat);
      ColorDepth colorDepth = (ColorDepth)(colorBpp);
      if (colorBpp == 8 && IconTools.IsMonochrome(image))
        colorDepth = ColorDepth.Gray;
      var newImage = new IconImage { Size = size, ColorDepth = colorDepth, Image = image };
      Images.Add(newImage);
      return newImage;
    }

    /// <summary>
    /// Dodanie obrazu o określonym rozmiarze i głębokości koloru
    /// </summary>
    /// <param name="size"></param>
    /// <param name="colorDepth">liczba bitów na piksel</param>
    /// <param name="image"></param>
    public IconImage AddImage(int size, ColorDepth colorDepth, Bitmap image)
    {
      if (Images == null)
        Images = new IconImagesCollection();
      IconImage newImage = new IconImage { Size = size, ColorDepth = colorDepth, Image = image };
      Images.Add(newImage);
      return newImage;
    }

    //[DllImport("gdi32.dll", SetLastError = true)]
    //private static extern bool DeleteObject(IntPtr hObject);
    //[DllImport("gdi32.dll", SetLastError = true)]
    //private static extern bool DestroyIcon(IntPtr hObject);


  //  public byte[] BoxImage(int size, byte[] image)
  //  {
  //    MemoryStream mStream = new MemoryStream(image);
  //    System.Drawing.Bitmap bitmap = System.Drawing.Bitmap.FromStream(mStream) as System.Drawing.Bitmap;
  //    for (int x=0; x<size; x++)
  //    {
  //      bitmap.SetPixel(x, 0, System.Drawing.Color.Black);
  //      bitmap.SetPixel(x, size-1, System.Drawing.Color.Black);
  //    }
  //    for (int y = 0; y < size; y++)
  //    {
  //      bitmap.SetPixel(0, y, System.Drawing.Color.Black);
  //      bitmap.SetPixel(size - 1, y, System.Drawing.Color.Black);
  //    }
  //    mStream = new MemoryStream();
  //    bitmap.Save(mStream, System.Drawing.Imaging.ImageFormat.Png);
  //    image = mStream.ToArray();
  //    return image;
  //  }

  }

  public class IconImagesCollection: SortedSet<IconImage>
  {
  }

  /// <summary>
  /// Klasa reprezentująca pojedynczy obraz ikony.
  /// </summary>
  [DataContract]
  public class IconImage: IComparable<IconImage>
  {
    /// <summary>
    /// Rozmiar obrazu
    /// </summary>
    [DataMember]
    public int Size { get; set; }
    /// <summary>
    /// Liczba bitów na piksel
    /// </summary>
    [DataMember]
    public ColorDepth ColorDepth { get; set; }

    /// <summary>
    /// Oryginalny format obrazu
    /// </summary>
    [DataMember]
    public string OriginalFormat { get; set; }

    /// <summary>
    /// Czy obraz został wygenerowany automatycznie na podstawie innych obrazów
    /// </summary>
    [DataMember]
    public bool AutoGenerated { get; set; }

    /// <summary>
    /// Obraz w postaci bitmapy
    /// </summary>
    [IgnoreDataMember]
    public Bitmap Image;

    [DataMember]
    public byte[] Content
    {
      get
      {
        if (Image == null)
          return null;
        using (MemoryStream ms = new MemoryStream())
        {
          Image.Save(ms, ImageFormat.Png);
          return ms.ToArray();
        }
      }
      set
      {
        if (value==null)
          Image=null;
        using (MemoryStream ms = new MemoryStream(value))
        {
          Image = new Bitmap(ms);
        }
      }
    }


    public int CompareTo(IconImage other)
    {
      if (this.Size < other.Size)
        return -1;
      else if (this.Size > other.Size)
        return 1;
      else if (this.ColorDepth < other.ColorDepth)
        return -1;
      else if (this.ColorDepth > other.ColorDepth)
        return 1;
      else
        return 0;
    }
  }

  public enum ColorDepth
  {
    Mono = 1,
    Gray = 2,
    Idx16 = 4,
    Idx256 = 8,
    Idx65K = 16,
    RGB = 24,
    Full = 32
  }
}
