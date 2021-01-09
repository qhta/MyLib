using System.Collections.Generic;
using System.Linq;

namespace Qhta.RegularExpressions
{
  /// <summary>
  /// This class parses regex expression and produces the tree of regex elements.
  /// </summary>
  public class RegExTagger
  {
    const string OneCharEscapes = "rntabvfe";
    const string OneCharDomains = "sSdDwW";
    const string RegExChars = @".$^{[(|)*+?\";
    const string OctalDigits = "01234567";
    const string DecimalDigits = "0123456789";
    const string HexadecimalDigits = "0123456789ABCDEFabcdef";

    private Stack<RegExItems> RegExStack = new Stack<RegExItems>();

    public RegExItems Items = new RegExItems();

    public RegExTagger(SearchOrReplace kind = SearchOrReplace.Search)
    {
      Kind = kind;
    }

    public SearchOrReplace Kind { get; private set; }

    public bool TryParseText(string text)
    {
      int charNdx = 0;
      if (charNdx < text.Length)
        return TryParseText(text, ref charNdx);
      return false;
    }

    private bool TryParseText(string text, ref int charNdx, char finalSep = '\0')
    {
      if (Kind == SearchOrReplace.Search)
      {
        return TryParseSearchText(text, ref charNdx, finalSep);
      }
      else
      {
        return TryParseReplaceText(text, ref charNdx, finalSep);
      }
    }

    private bool TryParseSearchText(string text, ref int charNdx, char finalSep = '\0')
    {
      for (; charNdx < text.Length; charNdx++)
      {
        var thisChar = text[charNdx];
        switch (thisChar)
        {
          case '\0':
            return true;
          case '\\':
            TryParseEscapeSeq(text, ref charNdx);
            break;
          case '.':
            TagSeq(text, charNdx, 1, RegExTag.DotChar, RegExStatus.OK);
            break;
          case '?':
          case '+':
          case '*':
            TryParseQuantifier(text, ref charNdx);
            break;
          case '{':
            TryParseQuantifier(text, ref charNdx);
            break;
          case '[':
            TryParseCharSet(text, ref charNdx);
            break;
          case '(':
            if (!TryParseSubexpression(text, ref charNdx))
              return false;
            break;
          default:
            if (thisChar == finalSep)
              return true;
            TagSeq(text, charNdx, 1, RegExTag.LiteralChar, RegExStatus.OK);
            break;
        }
      }
      return true;
    }

    private bool TryParseReplaceText(string text, ref int charNdx, char finalSep = '\0')
    {
      for (; charNdx < text.Length; charNdx++)
      {
        var thisChar = text[charNdx];
        switch (thisChar)
        {
          case '\0':
            return true;
          case '\\':
            TryParseEscapeSeq(text, ref charNdx);
            break;
          default:
            if (thisChar == finalSep)
            {
              return true;
            }
            break;
        }
      }
      return true;
    }

    private bool TryParseSubexpression(string text, ref int charNdx)
    {
      var seqStart = charNdx;
      charNdx++;
      var newItem = new RegExGroup { Tag = RegExTag.Subexpression, Start = seqStart, Length = 1 };
      Items.Add(newItem);
      RegExStack.Push(Items);
      Items = newItem.Items;
      bool isOK = TryParseText(text, ref charNdx, ')');
      bool isSeq = charNdx < text.Length && text[charNdx] == ')';
      if (isSeq)
      {
        newItem.Length = charNdx - seqStart + 1;
        newItem.Str = text.Substring(seqStart, charNdx - seqStart + 1);
        newItem.Status = isOK ? RegExStatus.OK : RegExStatus.Error;
      }
      Items = RegExStack.Pop();
      return isOK;
    }


    private void TryParseEscapeSeq(string text, ref int charNdx)
    {
      var nextChar = (charNdx < text.Length - 1) ? text[charNdx + 1] : '\0';
      if (OneCharEscapes.Contains(nextChar)
        || Kind == SearchOrReplace.Search && (OneCharDomains.Contains(nextChar) || RegExChars.Contains(nextChar)))
      {
        TagSeq(text, charNdx, 2, RegExTag.EscapeChar, RegExStatus.OK);
        charNdx++;
      }
      else
      if (DecimalDigits.Contains(nextChar))
      {
        TryParseOctalCodeOrBackref(text, ref charNdx);
      }
      else
      if (nextChar == 'c')
      {
        TryParseControlChar(text, ref charNdx);
      }
      else
      if (nextChar == 'x')
      {
        TryParseHexadecimalCode(text, ref charNdx);
      }
      else
      if (nextChar == 'u')
      {
        TryParseUnicode(text, ref charNdx);
      }
      else
      if (nextChar == 'p' || nextChar == 'P')
      {
        TryParseUnicodeGroup(text, ref charNdx);
      }
    }

    private bool TryParseControlChar(string text, ref int charNdx)
    {
      var seqStart = charNdx;
      charNdx++;
      var isOK = charNdx < text.Length - 1;
      if (isOK)
      {
        var controlChar = text[charNdx + 1];
        isOK = (controlChar >= '@' && controlChar <= '[');
        if (isOK)
        {
          charNdx++;
        }
      }
      TagSeq(text, seqStart, charNdx - seqStart + 1, RegExTag.ControlChar, isOK ? RegExStatus.OK : RegExStatus.Error);
      return isOK;
    }

    private bool TryParseOctalCodeOrBackref(string text, ref int charNdx)
    {
      var seqStart = charNdx;
      charNdx++;
      var isOK = charNdx < text.Length - 2;
      if (isOK)
        isOK = OctalDigits.Contains(text[charNdx]) && OctalDigits.Contains(text[charNdx + 1]) && OctalDigits.Contains(text[charNdx + 2]);
      if (isOK)
      {
        charNdx += 2;
        TagSeq(text, seqStart, charNdx - seqStart + 1, RegExTag.OctalString, RegExStatus.OK);
      }
      else
      {
        isOK = CheckNumRef(text, charNdx);
        TagSeq(text, seqStart, charNdx - seqStart + 1, RegExTag.OctalString, isOK ? RegExStatus.OK : RegExStatus.Warning);
      }
      return isOK;
    }

    private bool TryParseHexadecimalCode(string text, ref int charNdx)
    {
      var seqStart = charNdx;
      charNdx++;
      var isOK = charNdx < text.Length - 1;
      if (isOK)
      {
        isOK = HexadecimalDigits.Contains(text[charNdx + 1]);
        if (isOK)
        {
          charNdx++;
          isOK = charNdx < text.Length - 1;
          if (isOK)
          {
            isOK = HexadecimalDigits.Contains(text[charNdx + 1]);
            if (isOK)
            {
              charNdx++;
            }
          }
        }
      }
      TagSeq(text, seqStart, charNdx - seqStart + 1, RegExTag.HexadecimalString, isOK ? RegExStatus.OK : RegExStatus.Error);
      return isOK;
    }

    private bool TryParseUnicode(string text, ref int charNdx)
    {
      var seqStart = charNdx;
      charNdx++;
      var isOK = charNdx < text.Length - 1;
      if (isOK)
      {
        isOK = HexadecimalDigits.Contains(text[charNdx + 1]);
        if (isOK)
        {
          charNdx++;
          isOK = charNdx < text.Length - 1;
          if (isOK)
          {
            isOK = HexadecimalDigits.Contains(text[charNdx + 1]);
            if (isOK)
            {
              charNdx++;
              isOK = charNdx < text.Length - 1;
              if (isOK)
              {
                isOK = HexadecimalDigits.Contains(text[charNdx + 1]);
                if (isOK)
                {
                  charNdx++;
                  isOK = charNdx < text.Length - 1;
                  if (isOK)
                  {
                    isOK = HexadecimalDigits.Contains(text[charNdx + 1]);
                    if (isOK)
                    {
                      charNdx++;
                    }
                  }
                }
              }
            }
          }
        }
      }
      TagSeq(text, seqStart, charNdx - seqStart + 1, RegExTag.EscapeChar, isOK ? RegExStatus.OK : RegExStatus.Error);
      return isOK;
    }

    private bool TryParseUnicodeGroup(string text, ref int charNdx)
    {
      bool isSeq = false;
      var seqStart = charNdx;
      charNdx += 2;
      var isOK = false;
      if (charNdx < text.Length)
      {
        if (text[charNdx] == '{')
        {
          isSeq = true;
          charNdx++;
          var nameStart = charNdx;
          for (; charNdx < text.Length; charNdx++)
          {
            var ch = text[charNdx];
            if (ch == '}')
            {
              if (charNdx > nameStart)
              {
                var name = text.Substring(nameStart, charNdx - nameStart);
                isOK = UnicodeNames.Instance.Contains(name);
              }
              break;
            }
            if (!(ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z'))
              break;
          }
        }
      }
      if (isSeq)
        TagSeq(text, seqStart, charNdx - seqStart + 1, RegExTag.UnicodeString, isOK ? RegExStatus.OK : RegExStatus.Error);
      return isOK;
    }

    private bool TryParseCharSet(string text, ref int charNdx)
    {
      bool isSeq = false;
      bool isOK = false;
      var seqStart = charNdx;
      var newItem = new RegExCharset { Tag = RegExTag.CharSet, Start = seqStart, Length = 1 };
      Items.Add(newItem);
      RegExStack.Push(Items);
      Items = newItem.Items;

      for (charNdx = seqStart + 1; charNdx < text.Length; charNdx++)
      {
        var thisChar = text[charNdx];
        var nextChar = (charNdx < text.Length - 1) ? text[charNdx + 1] : '\0';
        if (thisChar == '\0')
        {
          isSeq = false;
          break;
        }
        else
        if (thisChar == '\\')
        {
          if (nextChar == '\\' || nextChar == ']')
          {
            TagSeq(text, charNdx, 2, RegExTag.EscapeChar, RegExStatus.OK);
            charNdx++;
          }
          else
            TryParseEscapeSeq(text, ref charNdx);
        }
        else
        if (thisChar == ']')
        {
          isSeq = true;
          isOK = true;
          break;
        }
        else
        if ((thisChar == '-') && (nextChar != ']') && (charNdx != seqStart + 1))
        {
          TagSeq(text, charNdx, 1, RegExTag.CharSetControlChar, RegExStatus.OK);
        }
        else
        if (thisChar == '^' && charNdx == seqStart + 1)
        {
          TagSeq(text, charNdx, 1, RegExTag.CharSetControlChar, RegExStatus.OK);
        }
        else
        if (thisChar == '[')
        {
          isOK = TryParseCharSet(text, ref charNdx);
        }
        else
          TagSeq(text, charNdx, 1, RegExTag.LiteralChar, RegExStatus.OK);
      }
      if (isSeq)
      {
        newItem.Length = charNdx - seqStart + 1;
        newItem.Str = text.Substring(seqStart, charNdx - seqStart + 1);
        newItem.Status = isOK ? RegExStatus.OK : RegExStatus.Error;
      }
      Items = RegExStack.Pop();
      return isSeq && isOK;
    }

    private void TryParseQuantifier(string text, ref int charNdx)
    {
      bool isSeq = false;
      bool isOK = false;
      var seqStart = charNdx;
      var thisChar = text[charNdx];
      if (thisChar == '+' || thisChar == '?' || thisChar == '*')
      {
        isSeq = true;
        isOK = true;

      }
      else
      {
        for (int i = charNdx + 1; i < text.Length; i++)
        {
          charNdx = i;
          thisChar = text[i];
          if (thisChar == '\0')
          {
            isSeq = false;
            break;
          }
          else
          if (thisChar == '}')
          {
            isSeq = true;
            break;
          }
          else
          if (DecimalDigits.Contains(thisChar) || thisChar == ',')
            isOK = true;
          else
          {
            isOK = false;
            isSeq = true;
            break;
          }
        }
      }
      if (isSeq)
      {
        if (!isOK)
          TagSeq(text, seqStart, charNdx - seqStart + 1, RegExTag.Quantifier, RegExStatus.Error);
        else
        {
          if (thisChar == '?')
          {
            if (Items.Count == 0)
              isOK = false;
            else if (Items.Count > 1)
              isOK = Items[Items.Count - 2].Tag != RegExTag.Quantifier;
          }
          else
          {
            isOK = Items.Count > 0 && Items.Last().Tag != RegExTag.Quantifier;
          }
          TagSeq(text, seqStart, charNdx - seqStart + 1, RegExTag.Quantifier, isOK ? RegExStatus.OK : RegExStatus.Warning);
        }

      };
    }

    private bool CheckNumRef(string text, int charNdx)
    {
      var thisChar = (charNdx < text.Length) ? text[charNdx] : '\0';
      var isOK = DecimalDigits.Contains(thisChar);
      if (isOK)
      {
        int num = (int)(thisChar - '0');
        isOK = num > 0 && num <= Items.Count;
      }
      return isOK;
    }

    private void TagSeq(string text, int seqStart, int length, RegExTag regExTag, RegExStatus status)
    {
      var tag = new RegExItem { Tag = regExTag, Start = seqStart, Length = length, Status = status, Str = text.Substring(seqStart, length) };
      Items.Add(tag);
    }

  }
}
