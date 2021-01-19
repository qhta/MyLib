using Qhta.RegularExpressions;

using System.Collections.Generic;

namespace RegExTaggerTest
{
  public class GraphItems : List<GraphItem>
  {
    public GraphItems(RegExItems items)
    {
      foreach (var item in items)
        Add(new GraphItem(item));
      Reposition(0);
    }

    private void Reposition(int fromColumn)
    {
      var column = fromColumn;
      foreach (var item in this)
      {
        item.ColStart = column;
        column += item.ColCount;
        if (item.SubItems != null)
          item.SubItems.Reposition(item.ColStart + item.BeginStr.Length);
      }
    }

    public List<GraphItem> ToList()
    {
      List<GraphItem> items = new List<GraphItem>();
      foreach (var item in this)
      {
        items.Add(item);
        if (item.SubItems != null)
        {
          items.AddRange(item.SubItems.ToList());
        }
      }
      return items;
    }

    public string PlainText
    {
      get
      {
        var result = "";
        foreach (var item in this)
          result += item.PlainText;
        return result;
      }
    }

    public ColoredText ColoredText
    { 
      get
      {
        var result = new ColoredText();
        foreach (var item in this)
          result.AddRange(item.ColoredText);
        return result;
      }
    }

  }

}
