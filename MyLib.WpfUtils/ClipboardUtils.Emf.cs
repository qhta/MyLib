using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Xml;
using BCDev.XamlToys;
using MyLib.HtmlUtils;

namespace MyLib.WpfUtils
{

  public static partial class ClipboardUtils
  {
    public static Metafile CreateMetafile(int width, int height)
    {
      Graphics offScreenBufferGraphics;
      Metafile m;
      MemoryStream stream = new MemoryStream();
      {
        offScreenBufferGraphics = Graphics.FromHwndInternal(IntPtr.Zero);
        {
          IntPtr deviceContextHandle = offScreenBufferGraphics.GetHdc();
          m = new Metafile(
          stream,
          deviceContextHandle,
          new RectangleF(0, 0, width, height),
          MetafileFrameUnit.Pixel,
          EmfType.EmfPlusOnly);
          offScreenBufferGraphics.ReleaseHdc();
        }
      }
      return m;
    }

    public static Graphics CreateGraphics(Metafile m, int width, int height)
    {
      Graphics g = Graphics.FromImage(m);
     
      // Set everything to high quality
      g.SmoothingMode = SmoothingMode.HighQuality;
      g.InterpolationMode = InterpolationMode.HighQualityBicubic;
      g.PixelOffsetMode = PixelOffsetMode.HighQuality;
      g.CompositingQuality = CompositingQuality.HighQuality;

      MetafileHeader metafileHeader = m.GetMetafileHeader();
      //g.ScaleTransform(
      //  metafileHeader.DpiX / g.DpiX,
      //  metafileHeader.DpiY / g.DpiY);

      g.PageUnit = GraphicsUnit.Pixel;
      g.SetClip(new RectangleF(0, 0, width, height));
      return g;
    }
    public static DataObject CreateEmfDataObject(Metafile mf)
    {
      var hEMF = mf.GetHenhmetafile();
      var dataObject = new DataObject();
      var hEMF2 = CopyEnhMetaFile(hEMF, new IntPtr(0));
      dataObject.SetData(DataFormats.EnhancedMetafile, hEMF2);
      return dataObject;
    }

    public static void CopyToClipboard(Metafile emf)
    {
      var dataObject = CreateEmfDataObject(emf);
      Clipboard.SetDataObject(dataObject, true);
    }

    public static void CopyVisualToWmfClipboard(Visual visual, Window clipboardOwnerWindow)
    {
      MemoryStream xpsStream = new MemoryStream();
      Package package = Package.Open(xpsStream, FileMode.Create);
      XpsDocument doc = new XpsDocument(package);
      XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(doc);
      xpsWriter.Write(visual);
      doc.Close();

      XpsDocument docPrime = new XpsDocument(package);
      IXpsFixedDocumentSequenceReader fixedDocSeqReader = docPrime.FixedDocumentSequenceReader;

      Dictionary<string, string> fontList = new Dictionary<string, string>();

      foreach (IXpsFixedDocumentReader docReader in fixedDocSeqReader.FixedDocuments)
      {
        int pageNum = 0;
        foreach (IXpsFixedPageReader fixedPageReader in docReader.FixedPages)
        {
          while (fixedPageReader.XmlReader.Read())
          {
            string page = fixedPageReader.XmlReader.ReadOuterXml();
            string path = string.Empty;

            foreach (XpsFont font in fixedPageReader.Fonts)
            {
              string name = font.Uri.GetFileName();
              path = string.Format(CultureInfo.InvariantCulture, @"{0}\{1}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), name);

              if (!fontList.ContainsKey(font.Uri.OriginalString))
              {
                fontList.Add(font.Uri.OriginalString, path);
                font.SaveToDisk(path);
              }
            }

            //foreach (XpsImage image in fixedPageReader.Images)
            //{
            //    //here to get images
            //}

            foreach (KeyValuePair<string, string> val in fontList)
            {
              page = page.Replace(val.Key, val.Value);
              //RegEx not working right, the above should be pretty safe
              //page = page.ReplaceAttribute("FontUri", val.Key,val.Value); 
            }

            FixedPage fp = XamlReader.Load(new MemoryStream(Encoding.Default.GetBytes(page))) as FixedPage;

            XmlWriterSettings settings = new XmlWriterSettings();

            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            MemoryStream xpsPageStream = new MemoryStream();
            XmlWriter writer = XmlWriter.Create(xpsPageStream, settings);

            XamlDesignerSerializationManager manager = new XamlDesignerSerializationManager(writer);
            manager.XamlWriterMode = XamlWriterMode.Expression;
            manager.XamlWriterMode = XamlWriterMode.Value;

            XamlWriter.Save(fp, manager);

            xpsPageStream.Position = 0;
            CopyXAMLStreamToWmfClipBoard(xpsPageStream, clipboardOwnerWindow);

            pageNum++;
          }
        }
      }

      package.Close();

      // delete temp font files
      foreach (KeyValuePair<string, string> val in fontList)
        File.Delete(val.Value);
    }

    public static object LoadXamlFromStream(Stream stream)
    {
      using (Stream s = stream)
        return XamlReader.Load(s);
    }

    public static System.Drawing.Graphics CreateEmf(Stream wmfStream, Rect bounds)
    {
      if (bounds.Width == 0 || bounds.Height == 0) bounds = new Rect(0, 0, 1, 1);
      using (System.Drawing.Graphics refDC = System.Drawing.Graphics.FromImage(new System.Drawing.Bitmap(1, 1)))
      {
        System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(new System.Drawing.Imaging.Metafile(wmfStream, refDC.GetHdc(), bounds.ToGdiPlus(), System.Drawing.Imaging.MetafileFrameUnit.Pixel, System.Drawing.Imaging.EmfType.EmfPlusDual));
        return graphics;
      }
    }

    public static T GetDependencyObjectFromVisualTree<T>(DependencyObject startObject)
        // don't restrict to DependencyObject items, to allow retrieval of interfaces
        //where T : DependencyObject
        where T : class
    {
      //Walk the visual tree to get the parent(ItemsControl) 
      //of this control
      DependencyObject parent = startObject;
      while (parent != null)
      {
        T pt = parent as T;
        if (pt != null)
          return pt;
        else
          parent = VisualTreeHelper.GetParent(parent);
      }

      return null;
    }

    private static void CopyXAMLStreamToWmfClipBoard(Stream drawingStream, Window clipboardOwnerWindow)
    {
      // http://xamltoys.codeplex.com/
      var drawing = Utility.GetDrawingFromXaml(LoadXamlFromStream(drawingStream));

      var bounds = drawing.Bounds;
      Console.WriteLine("Drawing Bounds: {0}", bounds);

      MemoryStream wmfStream = new MemoryStream();

      using (var g = CreateEmf(wmfStream, bounds))
        Utility.RenderDrawingToGraphics(drawing, g);

      wmfStream.Position = 0;

      System.Drawing.Imaging.Metafile metafile = new System.Drawing.Imaging.Metafile(wmfStream);

      IntPtr hEMF, hEMF2;
      hEMF = metafile.GetHenhmetafile(); // invalidates mf
      if (!hEMF.Equals(new IntPtr(0)))
      {
        hEMF2 = CopyEnhMetaFile(hEMF, new IntPtr(0));
        if (!hEMF2.Equals(new IntPtr(0)))
        {
          if (OpenClipboard(((IWin32Window)clipboardOwnerWindow.OwnerAsWin32()).Handle))
          {
            if (EmptyClipboard())
            {
              SetClipboardData(14 /*CF_ENHMETAFILE*/, hEMF2);
              CloseClipboard();
            }
          }
        }
        DeleteEnhMetaFile(hEMF);
      }
    }
  }
}

