using FastColoredTextBoxNS;

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;

namespace Qhta.RegularExpressions.FCTB.Tools
{
  public class RegExHighlighter: Component
  {

    public RegExHighlighter()
    {

    }

    public RegExHighlighter(IContainer container)
      : this()
    {
      if (container == null)
      {
        throw new ArgumentNullException("container");
      }

      foreach (var item in container.Components)
      {
        if (item is FastColoredTextBox fctb)
        {
          FCTB = fctb;
          break;
        }
      }

      container.Add(this);
    }

    [Category("Custom")]
    [Bindable(BindableSupport.Yes, BindingDirection.OneWay)]
    public FastColoredTextBox FCTB { get; set; }

    TextStyle CharsetStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
    TextStyle EscapeStyle = new TextStyle(Brushes.Brown, null, FontStyle.Regular);
    TextStyle NumberStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
    TextStyle NameStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Italic);
    TextStyle QuantifierStyle = new TextStyle(Brushes.DarkViolet, null, FontStyle.Regular);
    TextStyle GroupStyle = new TextStyle(Brushes.DarkSeaGreen, null, FontStyle.Regular);
    //MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));
    Style invisibleCharsStyle = new InvisibleCharsRenderer(Pens.Gray);

    public void HighlightOnTextChanged(TextChangedEventArgs e)
    {
      if (FCTB == null)
        return;
      FCTB.LeftBracket = '(';
      FCTB.RightBracket = ')';
      FCTB.LeftBracket2 = '[';
      FCTB.RightBracket2 = ']';
      //clear style of changed range
      e.ChangedRange.ClearStyle(CharsetStyle, EscapeStyle,  NumberStyle, NameStyle);

      // EscapedChar highlight
      e.ChangedRange.SetStyle(EscapeStyle, @"\\.");
      e.ChangedRange.SetStyle(NumberStyle, @"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b");
      e.ChangedRange.SetStyle(CharsetStyle, @"\[.+?\]");
      e.ChangedRange.SetStyle(NameStyle, @"<.+?>");
      e.ChangedRange.SetStyle(NameStyle, @"'.+?'");
      e.ChangedRange.SetStyle(QuantifierStyle, @"{.+?}");
      e.ChangedRange.SetStyle(GroupStyle, @"\(.+?\)");

      //clear folding markers
      e.ChangedRange.ClearFoldingMarkers();

      //set folding markers
      e.ChangedRange.SetFoldingMarkers(@"{", @"}");//allow to collapse braces block
      e.ChangedRange.SetFoldingMarkers(@"\[", @"]");//allow to collapse charset block
      e.ChangedRange.SetFoldingMarkers(@"\(", @"\)");//allow to collapse group subexpression
      e.ChangedRange.ClearStyle(invisibleCharsStyle);
      e.ChangedRange.SetStyle(invisibleCharsStyle, @".$|.\r\n|\s");
    }

    public void fctb_SelectionChangedDelayed(object sender, EventArgs e)
    {
      //if (FCTB == null)
      //  return;
      //FCTB.VisibleRange.ClearStyle(SameWordsStyle);
      //if (!FCTB.Selection.IsEmpty)
      //  return;//user selected diapason

      ////get fragment around caret
      //var fragment = FCTB.Selection.GetFragment(@"\w");
      //string text = fragment.Text;
      //if (text.Length == 0)
      //  return;
      ////highlight same words
      //var ranges = FCTB.VisibleRange.GetRanges("\\b" + text + "\\b").ToArray();
      //if (ranges.Length > 1)
      //  foreach (var r in ranges)
      //    r.SetStyle(SameWordsStyle);
    }
  }
}
