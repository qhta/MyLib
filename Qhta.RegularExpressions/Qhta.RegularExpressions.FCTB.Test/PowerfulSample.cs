﻿using FastColoredTextBoxNS;

using Qhta.RegularExpressions.FCTB.Tools;

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Tester
{
  public partial class PowerfulSample : Form
  {

    //styles
    //TextStyle BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
    //TextStyle BoldStyle = new TextStyle(null, null, FontStyle.Bold | FontStyle.Underline);
    //TextStyle GrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
    //TextStyle MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
    //TextStyle GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
    //TextStyle BrownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Italic);
    //TextStyle MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
    //MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));

    public PowerfulSample()
    {
      InitializeComponent();
      regExHighlighter1.FCTB = fctb;
      InitSyntaxHighlight();
    }

    private void InitStylesPriority()
    {
      //add this style explicitly for drawing under other styles
      //fctb.AddStyle(SameWordsStyle);
    }

    private void fctb_TextChanged(object sender, TextChangedEventArgs e)
    {
      //For sample, we will highlight the syntax of C# manually, although could use built-in highlighter
      regExHighlighter1.HighlightOnTextChanged(e);//custom highlighting

      if (fctb.Text.Trim().StartsWith("<?xml"))
      {
        fctb.Language = Language.XML;

        fctb.ClearStylesBuffer();
        fctb.Range.ClearStyle(StyleIndex.All);
        InitStylesPriority();
        fctb.AutoIndentNeeded -= fctb_AutoIndentNeeded;

        fctb.OnSyntaxHighlight(new TextChangedEventArgs(fctb.Range));
      }
    }

    private void findToolStripMenuItem_Click(object sender, EventArgs e)
    {
      fctb.ShowFindDialog();
    }

    private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
    {
      fctb.ShowReplaceDialog();
    }

    private void InitSyntaxHighlight()
    {
      fctb.ClearStylesBuffer();
      fctb.Range.ClearStyle(StyleIndex.All);
      InitStylesPriority();
      fctb.AutoIndentNeeded -= fctb_AutoIndentNeeded;
      fctb.Language = Language.Custom;
      fctb.CommentPrefix = null; // "//";
      //fctb.AutoIndentNeeded += fctb_AutoIndentNeeded;
      //call OnTextChanged for refresh syntax highlighting
      fctb.OnTextChanged();
      fctb.OnSyntaxHighlight(new TextChangedEventArgs(fctb.Range));
      miChangeColors.Enabled = true;
    }

    private void hTMLToolStripMenuItem1_Click(object sender, EventArgs e)
    {
      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Filter = "HTML with <PRE> tag|*.html|HTML without <PRE> tag|*.html";
      if (sfd.ShowDialog() == DialogResult.OK)
      {
        string html = "";

        if (sfd.FilterIndex == 1)
        {
          html = fctb.Html;
        }
        if (sfd.FilterIndex == 2)
        {

          ExportToHTML exporter = new ExportToHTML();
          exporter.UseBr = true;
          exporter.UseNbsp = false;
          exporter.UseForwardNbsp = true;
          exporter.UseStyleTag = true;
          html = exporter.GetHtml(fctb);
        }
        File.WriteAllText(sfd.FileName, html);
      }
    }

    private void fctb_SelectionChangedDelayed(object sender, EventArgs e)
    {
      regExHighlighter1.fctb_SelectionChangedDelayed(sender, e);
    }

    private void goForwardCtrlShiftToolStripMenuItem_Click(object sender, EventArgs e)
    {
      fctb.NavigateForward();
    }

    private void goBackwardCtrlToolStripMenuItem_Click(object sender, EventArgs e)
    {
      fctb.NavigateBackward();
    }

    const int maxBracketSearchIterations = 2000;

    void GoLeftBracket(FastColoredTextBox tb, char leftBracket, char rightBracket)
    {
      Range range = tb.Selection.Clone();//need to clone because we will move caret
      int counter = 0;
      int maxIterations = maxBracketSearchIterations;
      while (range.GoLeftThroughFolded())//move caret left
      {
        if (range.CharAfterStart == leftBracket) counter++;
        if (range.CharAfterStart == rightBracket) counter--;
        if (counter == 1)
        {
          //found
          tb.Selection.Start = range.Start;
          tb.DoSelectionVisible();
          break;
        }
        //
        maxIterations--;
        if (maxIterations <= 0) break;
      }
      tb.Invalidate();
    }

    void GoRightBracket(FastColoredTextBox tb, char leftBracket, char rightBracket)
    {
      var range = tb.Selection.Clone();//need clone because we will move caret
      int counter = 0;
      int maxIterations = maxBracketSearchIterations;
      do
      {
        if (range.CharAfterStart == leftBracket) counter++;
        if (range.CharAfterStart == rightBracket) counter--;
        if (counter == -1)
        {
          //found
          tb.Selection.Start = range.Start;
          tb.Selection.GoRightThroughFolded();
          tb.DoSelectionVisible();
          break;
        }
        //
        maxIterations--;
        if (maxIterations <= 0) break;
      } while (range.GoRightThroughFolded());//move caret right

      tb.Invalidate();
    }

    private void goLeftBracketToolStripMenuItem_Click(object sender, EventArgs e)
    {
      GoLeftBracket(fctb, '{', '}');
    }

    private void goRightBracketToolStripMenuItem_Click(object sender, EventArgs e)
    {
      GoRightBracket(fctb, '{', '}');
    }

    private void fctb_AutoIndentNeeded(object sender, AutoIndentEventArgs args)
    {
      //block {}
      if (Regex.IsMatch(args.LineText, @"^[^""']*\{.*\}[^""']*$"))
        return;
      //start of block {}
      if (Regex.IsMatch(args.LineText, @"^[^""']*\{"))
      {
        args.ShiftNextLines = args.TabLength;
        return;
      }
      //end of block {}
      if (Regex.IsMatch(args.LineText, @"}[^""']*$"))
      {
        args.Shift = -args.TabLength;
        args.ShiftNextLines = -args.TabLength;
        return;
      }
      //label
      if (Regex.IsMatch(args.LineText, @"^\s*\w+\s*:\s*($|//)") &&
          !Regex.IsMatch(args.LineText, @"^\s*default\s*:"))
      {
        args.Shift = -args.TabLength;
        return;
      }
      //some statements: case, default
      if (Regex.IsMatch(args.LineText, @"^\s*(case|default)\b.*:\s*($|//)"))
      {
        args.Shift = -args.TabLength / 2;
        return;
      }
      //is unclosed operator in previous line ?
      if (Regex.IsMatch(args.PrevLineText, @"^\s*(if|for|foreach|while|[\}\s]*else)\b[^{]*$"))
        if (!Regex.IsMatch(args.PrevLineText, @"(;\s*$)|(;\s*//)"))//operator is unclosed
        {
          args.Shift = args.TabLength;
          return;
        }
    }

    Random rnd = new Random();

    private void miChangeColors_Click(object sender, EventArgs e)
    {
      var styles = new Style[] { fctb.SyntaxHighlighter.BlueBoldStyle, fctb.SyntaxHighlighter.BlueStyle, fctb.SyntaxHighlighter.BoldStyle, fctb.SyntaxHighlighter.BrownStyle, fctb.SyntaxHighlighter.GrayStyle, fctb.SyntaxHighlighter.GreenStyle, fctb.SyntaxHighlighter.MagentaStyle, fctb.SyntaxHighlighter.MaroonStyle, fctb.SyntaxHighlighter.RedStyle };
      fctb.SyntaxHighlighter.AttributeStyle = styles[rnd.Next(styles.Length)];
      fctb.SyntaxHighlighter.ClassNameStyle = styles[rnd.Next(styles.Length)];
      fctb.SyntaxHighlighter.CommentStyle = styles[rnd.Next(styles.Length)];
      fctb.SyntaxHighlighter.CommentTagStyle = styles[rnd.Next(styles.Length)];
      fctb.SyntaxHighlighter.KeywordStyle = styles[rnd.Next(styles.Length)];
      fctb.SyntaxHighlighter.NumberStyle = styles[rnd.Next(styles.Length)];
      fctb.SyntaxHighlighter.StringStyle = styles[rnd.Next(styles.Length)];

      fctb.OnSyntaxHighlight(new TextChangedEventArgs(fctb.Range));
    }

    private void changeHotkeysToolStripMenuItem_Click(object sender, EventArgs e)
    {
      var form = new HotkeysEditorForm(fctb.HotkeysMapping);
      if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        fctb.HotkeysMapping = form.GetHotkeys();
    }

    private void rTFToolStripMenuItem_Click(object sender, EventArgs e)
    {
      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Filter = "RTF|*.rtf";
      if (sfd.ShowDialog() == DialogResult.OK)
      {
        string rtf = fctb.Rtf;
        File.WriteAllText(sfd.FileName, rtf);
      }
    }

    private void fctb_CustomAction(object sender, CustomActionEventArgs e)
    {
      MessageBox.Show(e.Action.ToString());
    }

  }
}
