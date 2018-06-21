using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using MyLib.HtmlUtils;

namespace MyLib.WpfUtils
{
  public static partial class ClipboardUtils
  {
    public static bool CopyToClipboard(ListView listView)
    {
      bool success = true;
      Clipboard.Clear();
      if (listView is IClipboardMate clipboardMate)
        if (clipboardMate.CanCopy(out string[] dataFormats))
        {
          var dataObjects = clipboardMate.CopyToClipboard(dataFormats);
          foreach (var dataObject in dataObjects)
            Clipboard.SetDataObject(dataObject, true);
          return success;
        }
      return false;
    }

    //private static string MakeHtml(ListView listView, out string plainText)
    //{
    //  StringBuilder sbt = new StringBuilder();
    //  StringBuilder sb = new StringBuilder();
    //  if (listView.View is GridView gridView)
    //  {
    //    sb.Append("<table>\n");

    //    var columns = gridView.Columns.ToList();
    //    sb.Append("<tr>\n");
    //    foreach (var column in columns)
    //    {
    //      string hdr = column.Header.ToString();

    //      sb.Append("<th>");
    //      sb.Append(HtmlTextUtils.EncodeHtmlEntities(hdr== "" ? "\u00A0" : hdr));
    //      sb.Append("</th>");

    //      sbt.Append(hdr);
    //      if (column!=columns.Last())
    //        sbt.Append("\t");
    //    }
    //    sb.Append("</tr>\n");
    //    sbt.Append("\n");

    //    Binding selectedPropertyBinding = null;
    //    if (listView.ItemContainerStyle is Style rowStyle)
    //    {
    //      var selectedPropertySetter = rowStyle.Setters.Where
    //        (item => item is Setter && (item as Setter).Property == ListViewItem.IsSelectedProperty).FirstOrDefault() as Setter;
    //      if (selectedPropertySetter!=null)
    //        selectedPropertyBinding = selectedPropertySetter.Value as Binding;
    //    }

    //    if (listView.DataContext is IEnumerable<object> dataCollection)
    //    {
    //      List<object> dataSet = new List<object>(dataCollection);
    //      int selectedRowsCount = 0;
    //      // first trial - IsSelected checked; 
    //      // second trial - unconditional, 
    //      // but only when selectedRowsCount at the end of the first trial is 0
    //      for (int trial = 0; selectedRowsCount==0 && trial<=1; trial++)
    //      {
    //        int rowNo = 0;
    //        foreach (var obj in dataSet)
    //        {
    //          rowNo++;
    //          bool isSelected = true;
    //          if (trial==0 && selectedPropertyBinding!=null)
    //          {
    //            isSelected = (bool)selectedPropertyBinding.GetValue(obj);
    //          }
    //          if (isSelected)
    //          {
    //            selectedRowsCount++;
    //            sb.Append("<tr>\n");
    //            int colNo = 0;
    //            foreach (var column in columns)
    //            {
    //              colNo++;
    //              string txt = "";
    //              Uri uri = null;
    //              if (column.DisplayMemberBinding is Binding binding)
    //              {
    //                txt = binding.GetValue(obj)?.ToString() ?? "?";
    //              }
    //              else if (column.CellTemplate is DataTemplate dataTemplate)
    //              {
    //                var content=dataTemplate.LoadContent();
    //                if (content is FrameworkElement element)
    //                {
    //                  element.ApplyTemplate();
    //                  Binding binding1 = new Binding("Tokens");
    //                  //if (element.DataContext is Binding binding1)
    //                    element.DataContext = binding1.GetValue(obj);

    //                  var wpfBitmap = MakeRenderTargetBitmap(element);
    //                  PngBitmapEncoder encoder = new PngBitmapEncoder();
    //                  encoder.Frames.Add(BitmapFrame.Create(wpfBitmap));
    //                  string filename = String.Format("{0}_{1}.png", rowNo, colNo);
    //                  string filepath = Path.Combine(@"c:\temp\", filename);
    //                  using (Stream pngStream = File.Create(filepath))
    //                  {
    //                    encoder.Save(pngStream);
    //                  }
    //                  uri = new Uri(filepath);
    //                  txt = String.Format("<a href=\"{0}\">{1}</a>",uri.ToString(), filename);
    //                }
    //              }
    //              sb.Append("<td>");
    //              if (uri!=null)
    //                sb.Append(txt);
    //              else
    //                sb.Append(HtmlTextUtils.EncodeHtmlEntities(txt));
    //              sb.Append("</td>");

    //              sbt.Append(txt);
    //              if (column!=columns.Last())
    //                sbt.Append("\t");
    //            }
    //            sb.Append("</tr>\n");
    //            sbt.Append("\n");
    //            selectedRowsCount++;
    //          }
    //        }
    //      }
    //    }

    //    sb.Append("</table>\n");
    //  }
    //  plainText = sbt.ToString();
    //  return sb.ToString();
    //}

  }
}
