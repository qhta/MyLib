using System.Collections.Generic;
using System.Linq;

namespace Qhta.RegularExpressions
{
  /// <summary>
  /// This class parses regex expression and produces the tree of regex elements.
  /// </summary>
  public class RegExTagger
  {
    const string OneCharEscapes = "rntavfe";
    const string SearchEscapes = "sSdDwW";
    const string AnchorEscapes = "AZzGbB";
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

    public RegExStatus TryParseText(string text)
    {
      Items = new RegExItems();
      int charNdx = 0;
      if (charNdx < text.Length)
        return TryParseText(text, ref charNdx);
      return RegExStatus.Unfinished;
    }

    private RegExStatus TryParseText(string text, ref int charNdx, char finalSep = '\0')
    {
      RegExStatus status = RegExStatus.Unfinished;
      if (Kind == SearchOrReplace.Search)
      {
        status = TryParseSearchText(text, ref charNdx, finalSep);
      }
      else
      {
        status = TryParseReplaceText(text, ref charNdx, finalSep);
      }
      JoinLiteralChars(Items);
      return status;
    }

    private RegExStatus TryParseSearchText(string text, ref int charNdx, char finalSep = '\0')
    {
      RegExStatus status = RegExStatus.Unfinished;
      for (; charNdx < text.Length; charNdx++)
      {
        RegExStatus status1 = RegExStatus.Unfinished;
        var thisChar = text[charNdx];
        switch (thisChar)
        {
          case '\\':
            status1 = TryParseEscapeSeq(text, ref charNdx);
            break;
          case '.':
            status1 = RegExStatus.OK;
            TagSeq(text, charNdx, 1, RegExTag.DomainChar, RegExStatus.OK);
            break;
          case '?':
          case '+':
          case '*':
            status1 = TryParseQuantifier(text, ref charNdx);
            break;
          case '{':
            status1 = TryParseQuantifier(text, ref charNdx);
            break;
          case '[':
            TryParseCharSet(text, ref charNdx);
            break;
          case '(':
            status1 = TryParseSubexpression(text, ref charNdx);
            break;
          case '^':
          case '$':
            status1 = RegExStatus.OK;
            TagSeq(text, charNdx, 1, RegExTag.AnchorControl, RegExStatus.OK);
            break;
          default:
            if (thisChar == finalSep)
              return status;
            status1 = RegExStatus.OK;
            TagSeq(text, charNdx, 1, RegExTag.LiteralChar, RegExStatus.OK);
            break;
        }
        if (status1 > status)
          status = status1;
        if (status1 == RegExStatus.Error)
          break;
      }
      return status;
    }

    private RegExStatus TryParseReplaceText(string text, ref int charNdx, char finalSep = '\0')
    {
      RegExStatus status = RegExStatus.Unfinished;
      for (; charNdx < text.Length; charNdx++)
      {
        RegExStatus status1 = RegExStatus.Unfinished;
        var thisChar = text[charNdx];
        switch (thisChar)
        {
          case '\\':
            status1 = TryParseEscapeSeq(text, ref charNdx);
            break;
          default:
            if (thisChar == finalSep)
              return status;
            status1 = RegExStatus.OK;
            TagSeq(text, charNdx, 1, RegExTag.LiteralChar, RegExStatus.OK);
            break;
        }
        if (status1 > status)
          status = status1;
        if (status1 == RegExStatus.Error)
          break;
      }
      return status;
    }

    private RegExStatus TryParseSubexpression(string text, ref int charNdx)
    {
      var seqStart = charNdx;
      charNdx++;
      var group = new RegExGroup { Tag = RegExTag.Subexpression, Start = seqStart, Str = GetSubstring(text, seqStart, 1) };
      Items.Add(group);
      RegExStack.Push(Items);
      Items = group.Items;
      RegExStatus status = RegExStatus.Unfinished;
      var thisChar = charNdx < text.Length ? text[charNdx] : '\0';
      if (thisChar == '?')
      {
        TagSeq(text, charNdx, 1, RegExTag.GroupControlChar, RegExStatus.OK);
        charNdx++;
        thisChar = charNdx < text.Length ? text[charNdx] : '\0';
        if (thisChar == '<')
          status = TryParseNamedGroup(text, ref charNdx, group, '>');
        else
        if (thisChar == '\'')
          status = TryParseNamedGroup(text, ref charNdx, group, '\'');
      }
      else
        status = TryParseText(text, ref charNdx, ')');
      bool isSeq = charNdx < text.Length && text[charNdx] == ')';
      if (isSeq)
      {
        group.Str = GetSubstring(text, seqStart, charNdx - seqStart + 1);
        group.Status = status;
      }
      Items = RegExStack.Pop();
      return status;
    }

    private RegExStatus TryParseNamedGroup(string text, ref int charNdx, RegExGroup group, char nameEnd)
    {
      group.Tag = RegExTag.NamedGroup;
      var status = TryParseGroupName(text, ref charNdx, group, nameEnd);
      if (status == RegExStatus.OK)
      {
        charNdx++;
        status = TryParseText(text, ref charNdx, ')');
      }
      return status;
    }

    private RegExStatus TryParseGroupName(string text, ref int charNdx, RegExGroup group, char finalSep)
    {
      RegExStatus status = RegExStatus.Unfinished;
      string name = null;
      int seqStart = charNdx;
      RegExItem nameItem = new RegExItem { Tag = RegExTag.GroupName, Start = charNdx };
      group.Items.Add(nameItem);
      for (charNdx++; charNdx < text.Length; charNdx++)
      {
        var thisChar = text[charNdx];
        if (thisChar >= 'a' && thisChar <= 'z'
              || thisChar >= 'A' && thisChar <= 'A'
              || thisChar == '_'
              )
        {
          status = RegExStatus.Error;
          name += thisChar;
        }
        else if (thisChar >= '0' && thisChar <= '9')
        {
          if (name == null)
          {
            status = RegExStatus.Error;
            break;
          }
          name += thisChar;
        }
        else if (thisChar == finalSep)
        {
          status = RegExStatus.OK;
          break;
        }
        else
        {
          status = RegExStatus.Error;
          break;
        }
      }
      var len = charNdx - seqStart + 1;
      nameItem.Str = GetSubstring(text, seqStart, len);
      nameItem.Status = status;
      group.Name = name;
      return status;
    }

    private RegExStatus TryParseEscapeSeq(string text, ref int charNdx)
    {
      RegExStatus status = RegExStatus.Unfinished;
      var nextChar = (charNdx < text.Length - 1) ? text[charNdx + 1] : '\0';
      if (nextChar=='\0')
      {
        status = RegExStatus.Unfinished;
        TagSeq(text, charNdx, 1, RegExTag.EscapedChar, status);
      }
      else
      if (OneCharEscapes.Contains(nextChar)
        || Kind == SearchOrReplace.Search && RegExChars.Contains(nextChar))
      {
        status = RegExStatus.OK;
        TagSeq(text, charNdx, 2, RegExTag.EscapedChar, status);
        charNdx++;
      }
      else
      if (Kind == SearchOrReplace.Search && SearchEscapes.Contains(nextChar))
      {
        status = RegExStatus.OK;
        TagSeq(text, charNdx, 2, RegExTag.DomainChar, status);
        charNdx++;
      }
      else
      if (Kind == SearchOrReplace.Search && AnchorEscapes.Contains(nextChar))
      {
        status = RegExStatus.OK;
        TagSeq(text, charNdx, 2, RegExTag.AnchorControl, status);
        charNdx++;
      }
      else
      if (DecimalDigits.Contains(nextChar))
      {
        return TryParseOctalCodeOrBackref(text, ref charNdx);
      }
      else
      if (nextChar == 'c')
      {
        return TryParseControlChar(text, ref charNdx);
      }
      else
      if (nextChar == 'x')
      {
        return TryParseHexadecimalCode(text, ref charNdx);
      }
      else
      if (nextChar == 'u')
      {
        return TryParseUnicode(text, ref charNdx);
      }
      else
      if (nextChar == 'p' || nextChar == 'P')
      {
        return TryParseUnicodeGroup(text, ref charNdx);
      }
      return status;
    }

    private RegExStatus TryParseControlChar(string text, ref int charNdx)
    {
      bool isOK = false;
      RegExStatus status = RegExStatus.Unfinished;
      var seqStart = charNdx;
      charNdx++;
      isOK = charNdx < text.Length - 1;
      if (isOK)
      {
        var controlChar = text[charNdx + 1];
        isOK = (controlChar >= '@' && controlChar <= '[');
        if (isOK)
        {
          charNdx++;
        }
        status = isOK ? RegExStatus.OK : RegExStatus.Error;
      }
      TagSeq(text, seqStart, charNdx - seqStart + 1, RegExTag.ControlChar, status);
      return status;
    }

    private RegExStatus TryParseOctalCodeOrBackref(string text, ref int charNdx)
    {
      bool isOK = false;
      RegExStatus status = RegExStatus.Unfinished;
      var seqStart = charNdx;
      charNdx++;
      isOK = charNdx < text.Length - 2;
      if (isOK)
        isOK = OctalDigits.Contains(text[charNdx]) && OctalDigits.Contains(text[charNdx + 1]) && OctalDigits.Contains(text[charNdx + 2]);
      if (isOK)
      {
        charNdx += 2;
        status = RegExStatus.OK;
        TagSeq(text, seqStart, charNdx - seqStart + 1, RegExTag.OctalString, status);
      }
      else
      {
        isOK = CheckNumRef(text, charNdx);
        status = isOK ? RegExStatus.OK : RegExStatus.Warning;
        TagSeq(text, seqStart, charNdx - seqStart + 1, RegExTag.BackRef, status);
      }
      return status;
    }

    private RegExStatus TryParseHexadecimalCode(string text, ref int charNdx)
    {
      bool isSeq = false;
      bool isOK = false;
      RegExStatus status = RegExStatus.Unfinished;
      var seqStart = charNdx;
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
              isSeq = true;
            }
          }
        }
      }
      if (isSeq)
        status = isOK ? RegExStatus.OK : RegExStatus.Error;
      TagSeq(text, seqStart, charNdx - seqStart + 1, RegExTag.HexadecimalString, status);
      return status;
    }

    private RegExStatus TryParseUnicode(string text, ref int charNdx)
    {
      bool isSeq = false;
      bool isOK = false;
      RegExStatus status = RegExStatus.Unfinished;
      var seqStart = charNdx;
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
                  isOK = charNdx < text.Length - 1;
                  if (isOK)
                  {
                    isOK = HexadecimalDigits.Contains(text[charNdx + 1]);
                    if (isOK)
                    {
                      charNdx++;
                      isSeq = true;
                    }
                  }
                }
              }
            }
          }
        }
      }
      if (isSeq)
        status = isOK ? RegExStatus.OK : RegExStatus.Error;
      TagSeq(text, seqStart, charNdx - seqStart + 1, RegExTag.EscapedChar, status);
      return status;
    }

    private RegExStatus TryParseUnicodeGroup(string text, ref int charNdx)
    {
      bool isSeq = false;
      bool isOK = false;
      RegExStatus status = RegExStatus.Unfinished;
      var seqStart = charNdx;
      charNdx += 2;
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
                var name = GetSubstring(text, nameStart, charNdx - nameStart);
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
        status = isOK ? RegExStatus.OK : RegExStatus.Error;
      TagSeq(text, seqStart, charNdx - seqStart + 1, RegExTag.UnicodeGroup, status);
      return status;
    }

    private RegExStatus TryParseCharSet(string text, ref int charNdx)
    {
      bool isSeq = false;
      bool isOK = true;
      RegExStatus status = RegExStatus.Unfinished;
      var seqStart = charNdx;
      var newItem = new RegExCharset { Tag = RegExTag.CharSet, Start = seqStart, Str = GetSubstring(text, seqStart, 1) };
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
            TagSeq(text, charNdx, 2, RegExTag.EscapedChar, RegExStatus.OK);
            charNdx++;
          }
          else
            TryParseEscapeSeq(text, ref charNdx);
        }
        else
        if (thisChar == ']')
        {
          isSeq = true;
          break;
        }
        else
        if ((thisChar == '-') && (nextChar != ']') && (charNdx != seqStart + 1))
        {
          var lastItem = Items.Last();
          if (lastItem.Length == 1)
          {
            var priorChar = lastItem.Str[0];
            lastItem.Str += "-" + nextChar;
            lastItem.Tag = RegExTag.CharRange;
            if (priorChar >= nextChar)
            {
              lastItem.Status = RegExStatus.Error;
              isOK = false;
            }
            charNdx++;
          }
          else
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
          status = TryParseCharSet(text, ref charNdx);
          isOK = isOK && status != RegExStatus.Error;
        }
        else
        {
          TagSeq(text, charNdx, 1, RegExTag.LiteralChar, RegExStatus.OK);
        }
      }
      if (isSeq)
        status = isOK ? RegExStatus.OK : RegExStatus.Error;
      newItem.Str = GetSubstring(text, seqStart, charNdx - seqStart + 1);
      newItem.Status = status;
      Items = RegExStack.Pop();
      return status;
    }

    private RegExStatus TryParseQuantifier(string text, ref int charNdx)
    {
      bool isSeq = false;
      bool isOK = false;
      RegExStatus status = RegExStatus.OK;
      var seqStart = charNdx;
      var thisChar = text[charNdx];
      if (thisChar == '+' || thisChar == '?' || thisChar == '*')
      {
        isOK = true;
      }
      else
      {
        isSeq = true;
        isOK = true;
        status = RegExStatus.Unfinished;
        for (int i = charNdx + 1; i < text.Length; i++)
        {
          charNdx = i;
          thisChar = text[i];
          if (thisChar == '}')
          {
            status = RegExStatus.OK;
            break;
          }
          else
          if (DecimalDigits.Contains(thisChar) || thisChar == ',')
            isOK = true;
          else
          {
            isOK = false;
            break;
          }
        }
      }

      if (isOK)
      {
        if (thisChar == '?')
        {
          isOK = Items.Count > 0;
          if (isOK)
          {
            var lastItem = Items.Last();
            if (lastItem.Tag == RegExTag.Quantifier)
            {
              if (lastItem.Str?.Length == 1)
              {
                lastItem.Str += "?";
                return RegExStatus.OK;
              }
              else
                isOK = false;
            }
          }
        }
        else
        {
          isOK = Items.Count > 0 && Items.Last().Tag != RegExTag.Quantifier;
        }
      }
      if (status != RegExStatus.Unfinished)
      if (isSeq)
      {
        status = isOK ? RegExStatus.OK : RegExStatus.Error;
      }
      else
      {
        status = isOK ? RegExStatus.OK : RegExStatus.Warning;
      }
      TagSeq(text, seqStart, charNdx - seqStart + 1, RegExTag.Quantifier, status);
      return status;
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

    private RegExItem TagSeq(string text, int seqStart, int length, RegExTag regExTag, RegExStatus status)
    {
      var str = GetSubstring(text, seqStart, length);
      var tag = new RegExItem { Tag = regExTag, Start = seqStart, Status = status, Str = str };
      Items.Add(tag);
      return tag;
    }

    private string GetSubstring(string text, int seqStart, int length)
    {
      if (seqStart + length > text.Length)
        length = text.Length - seqStart;
      if (length > 0)
        return text.Substring(seqStart, length);
      return "";
    }

    private void JoinLiteralChars(RegExItems items)
    {
      for (int i = items.Count - 1; i > 0; i--)
      {
        var item = items[i];
        if (item.Tag == RegExTag.Quantifier)
        {
          i--;
        }
        else if (item.Tag == RegExTag.LiteralChar || item.Tag == RegExTag.LiteralString)
        {
          var priorItem = items[i - 1];
          if (priorItem.Tag == RegExTag.LiteralChar || priorItem.Tag == RegExTag.LiteralString)
          {
            priorItem.Tag = RegExTag.LiteralString;
            priorItem.Str += item.Str;
            items.RemoveAt(i);
          }
        }
        else if (item is RegExGroup group)
          JoinLiteralChars(group.Items);
      }
    }
  }
}
