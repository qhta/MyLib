using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Qhta.HtmlUtils;

namespace Qhta.WPF.Utils
{
  public static partial class ClipboardUtils
  {
    public static bool CopyToClipboard(DataGrid dataGrid)
    {
      bool success = true;
      Clipboard.Clear();
      var html = MakeHtml(dataGrid, out string plainText);
      CopyToClipboard (html, plainText);
      return success;
    }

    private static string MakeHtml(DataGrid dataGrid, out string plainText)
    {
      StringBuilder sbt = new StringBuilder();
      StringBuilder sb = new StringBuilder();
      sb.Append("<table>\n");

      var columns = dataGrid.Columns.Where(column => column.Visibility==Visibility.Visible)
        .OrderBy(column => column.DisplayIndex).ToList();
      sb.Append("<tr>\n");
      foreach (var column in columns)
      {
        string hdr = column.Header.ToString();

        sb.Append("<th>");
        sb.Append(HtmlTextUtils.EncodeHtmlEntities(hdr== "" ? "\u00A0" : hdr));
        sb.Append("</th>");

        sbt.Append(hdr);
        if (column!=columns.Last())
          sbt.Append("\t");
      }
      sb.Append("</tr>\n");
      sbt.Append("\n");

      Binding selectedPropertyBinding = null;
      if (dataGrid.RowStyle is Style rowStyle)
      {
        var selectedPropertySetter = rowStyle.Setters.Where
          (item => item is Setter && (item as Setter).Property == DataGridRow.IsSelectedProperty).FirstOrDefault() as Setter;
        if (selectedPropertySetter!=null)
          selectedPropertyBinding = selectedPropertySetter.Value as Binding;
      }
      var sortedBy = new List<SortInfo>();
      foreach (var column in columns)
      {
        if (column.SortDirection!=null && column.ClipboardContentBinding is Binding binding)
          sortedBy.Add(new SortInfo { Binding=binding, SortDirection=(ListSortDirection)column.SortDirection });
      }

      if (dataGrid.DataContext is IEnumerable<object> dataCollection)
      {
        List<object> dataSet = new List<object>(dataCollection);
        if (sortedBy.Count>0)
          dataSet.Sort(new Comparator(sortedBy));
        int selectedRowsCount = 0;
        // first trial - IsSelected checked; 
        // second trial - unconditional, 
        // but only when selectedRowsCount at the end of the first trial is 0
        for (int trial = 0; selectedRowsCount==0 && trial<=1; trial++)
        {
          foreach (var obj in dataSet)
          {
            bool isSelected = true;
            if (trial==0 && selectedPropertyBinding!=null)
            {
              isSelected = (bool)selectedPropertyBinding.GetValue(obj);
            }
            if (isSelected)
            {
              selectedRowsCount++;
              sb.Append("<tr>\n");
              foreach (var column in columns)
              {
                string txt = "";
                if (column.ClipboardContentBinding is Binding binding)
                {
                  txt = binding.GetValue(obj)?.ToString() ?? "?";
                }
                sb.Append("<td>");
                sb.Append(HtmlTextUtils.EncodeHtmlEntities(txt));
                sb.Append("</td>");

                sbt.Append(txt);
                if (column!=columns.Last())
                  sbt.Append("\t");
              }
              sb.Append("</tr>\n");
              sbt.Append("\n");
              selectedRowsCount++;
            }
          }
        }
      }

      sb.Append("</table>\n");
      plainText = sbt.ToString();
      return sb.ToString();
    }


  }
}
