using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.HtmlUtils
{
  public static class HtmlTextUtils
  {
    public static string DecodeHtmlEntities(this string text)
    {
      text = text.RemoveHtmlComments();
      text = text.ReplaceCodedEntities();
      text = text.Replace("&nbsp;", "\u00A0")
        .Replace("&quot;", "\"")
        .Replace("&amp;", "&")
        .Replace("&lt;", "<")
        .Replace("&gt;", ">")
        .Replace("&rarr;", "\u2192")
        ;
      return text;
    }

    public static string RemoveHtmlComments(this string text)
    {
      int k = text.IndexOf("<!--");
      while (k>=0)
      {
        int l = text.IndexOf("-->", k);
        if (l < 0) break;
        text = text.Remove(k, l - k + 3);
        k = text.IndexOf("<!--");
      }
      return text;
    }

    private static string ReplaceCodedEntities(this string text)
    {
      int k = text.IndexOf('%');
      List<byte> lastCodes = null;
      int lastIndex = -2;
      while (k >= 0 && k < text.Length - 2)
      {
        string s = text.Substring(k + 1, 2);
        byte b;
        if (!s.Contains(' ') && byte.TryParse(s, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out b))
        {
          if (k == lastIndex + 1 && lastCodes!=null && lastCodes.Last()>=0x80)
          {
            lastCodes.Add(b);
            char[] chars = UTF8Encoding.UTF8.GetChars(lastCodes.ToArray());
            text = text.Substring(0, k-1) + new string(chars) + text.Substring(k + 3);
          }
          else
          {
            lastCodes = new List<byte> { b };
            char ch = (char)b;
            text = text.Substring(0, k) + new string(ch, 1) + text.Substring(k + 3);
          }
        }
        lastIndex = k;
        if (k > text.Length - 2)
          break;
        k = text.IndexOf('%', k + 1);
      }

      k = text.IndexOf("&#");
      while (k >= 0 && k < text.Length - 2)
      {
        int l = k;
        int code = 0;
        for (int i = k+2; i<=text.Length; i++)
        {
          if (i==text.Length)
          {
            l = i;
            break;
          }
          char ch = text[i];
          if (ch >= '0' && ch <= '9')
            code = code * 10 + (ch - '0');
          else if (ch==';')
          {
            l = i + 1;
            break;
          }
          else
          {
            l = i;
            break;
          }
        }
        if (l > k)
        {
          text = text.Remove(k, l - k);
          text = text.Insert(k, Char.ConvertFromUtf32(code));
        }
        else
          break;
        k = text.IndexOf("&#");
      }
      return text;
    }

    public static string RemoveNumbering(this string text)
    {
      if (!TryRemoveNumber(ref text))
        TryRemoveLetter(ref text);
      return text;
    }

    private static bool TryRemoveNumber(ref string text)
    {
      for (int j = 0; j < text.Length; j++)
      {
        char ch = text[j];
        if (!(Char.IsDigit(ch) || ch == '.' || ch == ' '))
        {
          if (j > 0)
          {
            text = text.Substring(j).TrimStart();
            return true;
          }
          break;
        }
      }
      return false;
    }

    private static bool TryRemoveLetter(ref string text)
    {
      if (text.Length >= 2)
      {
        if (text[1]==')' && Char.IsLetter(text[0]))
        {
          text = text.Substring(2).TrimStart();
          return true;
        }
      }
      return false;
    }

    public static string HtmlToSimpleText(this string html)
    {
      HtmlDocument document = new HtmlDocument();
      document.LoadHtml(html);
      return document.DocumentNode.InnerText;
    }

    public static string RemoveHtmlTags(this string str)
    {
      int k = str.IndexOf('<');
      while (k >= 0)
      {
        int l = str.IndexOf('>', k + 1);
        if (l > k)
        {
          str = str.Remove(k, l - k + 1);
        }
        if (k < str.Length - 1)
          k = str.IndexOf('<', k + 1);
        else
          break;
      }
      return str.Trim();
    }
  }
}
