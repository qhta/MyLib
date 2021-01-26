using Qhta.RegularExpressions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegExTaggerTest
{
  /// <summary>
  /// As we insert 1 space between each pattern item, we should evaluate start of each item in columns not in char positions
  /// 
  /// </summary>
  public class GraphItem
  {
    public GraphItem(RegExItem item)
    {
      Item = item;
      if (item.SubItems != null)
        SubItems = new GraphItems(item.SubItems);
    }

    public RegExItem Item { get; private set; }

    public string Str => Item.Str;

    public int Start => Item.Start;

    public int Length => Item.Length;

    public int ColStart { get; internal set; }

    public int ColCount
    {
      get
      {
        if (SubItems != null)
        {
          var n = BeginStr.Length;
          foreach (var item in SubItems)
            n += item.ColCount;
          n += EndStr.Length;
          return n;
        }
        else
          return Item.Length + 1;
      }
    }

    public string BeginStr
    {
      get
      {
        if (SubItems != null)
        {
          var firstItem = SubItems.FirstOrDefault();
          if (firstItem!=null)
          {
            return Str.Substring(0, firstItem.Start-this.Start) + " ";
          }
        }
        return Str+" ";
      }
    }

    public string EndStr
    {
      get
      {
        if (SubItems != null)
        {
          var lastItem = SubItems.LastOrDefault();
          if (lastItem != null)
          {
            return Str.Substring(lastItem.Start-this.Start+lastItem.Length) + " ";
          }
        }
        return "";
      }
    }

    public string PlainText
    {
      get
      {
        var s = BeginStr;
        if (SubItems != null)
        {
          foreach (var item in SubItems)
            s += item.PlainText;
          s += EndStr;
        }
        return s;
      }
    }

    public ColoredText ColoredText
    {
      get
      {
        var result = new ColoredText();
        result.Add(new ColoredString(BeginStr, Color));
        if (SubItems != null)
        {
          foreach (var item in SubItems)
            result.AddRange(item.ColoredText);
           result.Add(new ColoredString(EndStr, Color));
        }
        return result;
      }
    }

    public int Color { get; set; } = 0xFFFFFF;

    public GraphItems SubItems { get; private set; }

    public override string ToString()
    {
      return Item.ToString();
    }

  }
}
