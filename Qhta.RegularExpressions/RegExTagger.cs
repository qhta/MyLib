using System.Collections.Generic;
using System.Linq;

namespace Qhta.RegularExpressions
{
  /// <summary>
  /// This class parses regex expression and produces the tree of regex elements.
  /// </summary>
  public class RegExTagger
  {
    const string OneCharEscapeChars = "rntavfe";
    const string CharClassEscapeChars = "sSdDwW";
    const string AnchorEscapes = "AZzGbB";
    const string RegExChars = @".$^{[(|)*+?\";
    const string OctalDigits = "01234567";
    const string DecimalDigits = "0123456789";
    const string HexadecimalDigits = "0123456789ABCDEFabcdef";

    private Stack<RegExItems> RegExStack = new Stack<RegExItems>();

    public RegExItems Items = new RegExItems();

    public int NumGroupsCount
    {
      get
      {
        return 0;
      }
    }

    public RegExTagger(SearchOrReplace kind = SearchOrReplace.Search)
    {
      Kind = kind;
    }

    public SearchOrReplace Kind { get; private set; }

    public RegExStatus TryParsePattern(string pattern)
    {
      Items = new RegExItems();
      int charNdx = 0;
      if (charNdx < pattern.Length)
        return TryParsePattern(pattern, ref charNdx);
      return RegExStatus.Unfinished;
    }

    private RegExStatus TryParsePattern(string pattern, ref int charNdx, char finalSep = '\0')
    {
      RegExStatus status;
      if (Kind == SearchOrReplace.Search)
      {
        status = TryParseSearchPattern(pattern, ref charNdx, finalSep);
      }
      else
      {
        status = TryParseReplacePattern(pattern, ref charNdx, finalSep);
      }
      NormalizeTags(Items);
      return status;
    }

    private RegExStatus TryParseSearchPattern(string pattern, ref int charNdx, char finalSep = '\0')
    {
      if (charNdx >= pattern.Length)
        return RegExStatus.Unfinished;
      RegExStatus status = RegExStatus.OK;
      int seqStart = charNdx;
      for (; charNdx < pattern.Length; charNdx++)
      {
        RegExStatus status1 = RegExStatus.Unfinished;
        var thisChar = pattern[charNdx];
        switch (thisChar)
        {
          case '\\':
            status1 = TryParseEscapeSeq(pattern, ref charNdx);
            break;
          case '.':
            status1 = RegExStatus.OK;
            TagSeq(pattern, charNdx, 1, RegExTag.DotChar, RegExStatus.OK);
            break;
          case '?':
          case '+':
          case '*':
            status1 = TryParseQuantifier(pattern, ref charNdx);
            break;
          case '{':
            status1 = TryParseNumQuantifier(pattern, ref charNdx);
            if (status1 == RegExStatus.Unfinished)
            {
              status1 = RegExStatus.OK;
              TagSeq(pattern, charNdx, 1, RegExTag.LiteralChar, RegExStatus.OK);
            }
            break;
          case '[':
            status1 = TryParseCharset(pattern, ref charNdx);
            break;
          case '(':
            status1 = TryParseSubexpression(pattern, ref charNdx);
            break;
          case '^':
          case '$':
            status1 = RegExStatus.OK;
            TagSeq(pattern, charNdx, 1, RegExTag.AnchorControl, RegExStatus.OK);
            break;
          default:
            if (thisChar == finalSep)
              return status;
            if (thisChar == ')')
            {
              status1 = RegExStatus.Error;
              TagSeq(pattern, charNdx, 1, RegExTag.Subexpression, status1);
            }
            else if (thisChar == '|')
            {
              status1 = (charNdx > seqStart && charNdx < pattern.Length - 1) ? RegExStatus.OK : RegExStatus.Warning;
              TagSeq(pattern, charNdx, 1, RegExTag.AltChar, status1);
              status = RegExTools.Max(status, status1);
              charNdx++;
              if (charNdx < pattern.Length)
              {
                status1 = TryParseSearchPattern(pattern, ref charNdx, finalSep);
                status = RegExTools.Max(status, status1);
              }
              return status;
            }
            else
            {
              status1 = RegExStatus.OK;
              TagSeq(pattern, charNdx, 1, RegExTag.LiteralChar, status1);
            }
            break;
        }
        status = RegExTools.Max(status, status1);
        if (status1 == RegExStatus.Error)
          break;
      }
      return status;
    }

    private RegExStatus TryParseReplacePattern(string pattern, ref int charNdx, char finalSep = '\0')
    {
      RegExStatus status = RegExStatus.Unfinished;
      for (; charNdx < pattern.Length; charNdx++)
      {
        RegExStatus status1 = RegExStatus.Unfinished;
        var thisChar = pattern[charNdx];
        switch (thisChar)
        {
          case '\\':
            status1 = TryParseEscapeSeq(pattern, ref charNdx);
            break;
          default:
            if (thisChar == finalSep)
              return status;
            status1 = RegExStatus.OK;
            TagSeq(pattern, charNdx, 1, RegExTag.LiteralChar, RegExStatus.OK);
            break;
        }
        if (status1 > status)
          status = status1;
        if (status1 == RegExStatus.Error)
          break;
      }
      return status;
    }

    private RegExStatus TryParseSubexpression(string pattern, ref int charNdx)
    {
      var seqStart = charNdx;
      charNdx++;
      var group = new RegExGroup { Tag = RegExTag.Subexpression, Start = seqStart, Str = GetSubstring(pattern, seqStart, 1) };
      Items.Add(group);
      RegExStack.Push(Items);
      Items = group.Items;
      RegExStatus status = RegExStatus.Unfinished;
      var thisChar = charNdx < pattern.Length ? pattern[charNdx] : '\0';
      if (thisChar == '?')
      {
        TagSeq(pattern, charNdx, 1, RegExTag.GroupControlChar, RegExStatus.OK);
        charNdx++;
        thisChar = charNdx < pattern.Length ? pattern[charNdx] : '\0';
        if (thisChar == '<')
          status = TryParseNamedGroup(pattern, ref charNdx, group, '>');
        else
        if (thisChar == '\'')
          status = TryParseNamedGroup(pattern, ref charNdx, group, '\'');
      }
      else
        status = TryParsePattern(pattern, ref charNdx, ')');
      bool isSeq = charNdx < pattern.Length && pattern[charNdx] == ')';
      if (isSeq)
      {
        group.Status = status;
      }
      else
      {
        status = RegExStatus.Unfinished;
        group.Status = status;
      }
      group.Str = GetSubstring(pattern, seqStart, charNdx - seqStart + 1);
      Items = RegExStack.Pop();
      return status;
    }

    private RegExStatus TryParseNamedGroup(string pattern, ref int charNdx, RegExGroup group, char nameEnd)
    {
      group.Tag = RegExTag.NamedGroup;
      var status = TryParseGroupName(pattern, ref charNdx, group, nameEnd);
      if (status == RegExStatus.OK)
      {
        charNdx++;
        status = TryParsePattern(pattern, ref charNdx, ')');
      }
      return status;
    }

    private RegExStatus TryParseGroupName(string pattern, ref int charNdx, RegExGroup group, char finalSep)
    {
      RegExStatus status = RegExStatus.Unfinished;
      string name = null;
      int seqStart = charNdx;
      RegExItem nameItem = new RegExItem { Tag = RegExTag.GroupName, Start = charNdx };
      group.Items.Add(nameItem);
      for (charNdx++; charNdx < pattern.Length; charNdx++)
      {
        var thisChar = pattern[charNdx];
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
      nameItem.Str = GetSubstring(pattern, seqStart, len);
      nameItem.Status = status;
      group.Name = name;
      return status;
    }

    private RegExStatus TryParseEscapeSeq(string pattern, ref int charNdx)
    {
      RegExStatus status = RegExStatus.Unfinished;
      var nextChar = (charNdx < pattern.Length - 1) ? pattern[charNdx + 1] : '\0';
      if (nextChar == '\0')
      {
        status = RegExStatus.Unfinished;
        TagSeq(pattern, charNdx, 1, RegExTag.EscapedChar, status);
      }
      else
      if (OneCharEscapeChars.Contains(nextChar)
        || Kind == SearchOrReplace.Search && RegExChars.Contains(nextChar))
      {
        status = RegExStatus.OK;
        TagSeq(pattern, charNdx, 2, RegExTag.EscapedChar, status);
        charNdx++;
      }
      else
      if (Kind == SearchOrReplace.Search && CharClassEscapeChars.Contains(nextChar))
      {
        status = RegExStatus.OK;
        TagSeq(pattern, charNdx, 2, RegExTag.CharClass, status);
        charNdx++;
      }
      else
      if (Kind == SearchOrReplace.Search && AnchorEscapes.Contains(nextChar))
      {
        status = RegExStatus.OK;
        TagSeq(pattern, charNdx, 2, RegExTag.AnchorControl, status);
        charNdx++;
      }
      else
      if (DecimalDigits.Contains(nextChar))
      {
        return TryParseOctalCodeOrBackref(pattern, ref charNdx);
      }
      else
      if (nextChar == 'c')
      {
        return TryParseControlCharSeq(pattern, ref charNdx);
      }
      else
      if (nextChar == 'x')
      {
        return TryParseHexadecimalCode(pattern, ref charNdx);
      }
      else
      if (nextChar == 'u')
      {
        return TryParseUnicode(pattern, ref charNdx);
      }
      else
      if (nextChar == 'p' || nextChar == 'P')
      {
        return TryParseUnicodeCategory(pattern, ref charNdx);
      }
      else
      {
        status = RegExStatus.OK;
        TagSeq(pattern, charNdx, 2, RegExTag.EscapedChar, status);
        charNdx++;
      }
      return status;
    }

    private RegExStatus TryParseControlCharSeq(string pattern, ref int charNdx)
    {
      bool isOK = false;
      bool isSeq = false;
      RegExStatus status = RegExStatus.Unfinished;
      var seqStart = charNdx;
      charNdx++;
      if (charNdx < pattern.Length - 1)
      {
        charNdx++;
        isSeq = true;
        var nextChar = pattern[charNdx];
        nextChar = char.ToUpperInvariant(nextChar);
        isOK = (nextChar >= '@' && nextChar <= '_');
      }
      if (isSeq)
        status = isOK ? RegExStatus.OK : RegExStatus.Error;
      else
        status = RegExStatus.Unfinished;
      TagSeq(pattern, seqStart, charNdx - seqStart + 1, RegExTag.ControlCharSeq, status);
      return status;
    }

    private RegExStatus TryParseOctalCodeOrBackref(string pattern, ref int charNdx)
    {
      bool isOK = false;
      RegExStatus status = RegExStatus.Unfinished;
      var seqStart = charNdx;
      charNdx++;
      isOK = charNdx < pattern.Length - 2;
      if (isOK)
        isOK = OctalDigits.Contains(pattern[charNdx]) && OctalDigits.Contains(pattern[charNdx + 1]) && OctalDigits.Contains(pattern[charNdx + 2]);
      if (isOK)
      {
        charNdx += 2;
        status = RegExStatus.OK;
        TagSeq(pattern, seqStart, charNdx - seqStart + 1, RegExTag.OctalString, status);
      }
      else
      {
        isOK = CheckNumRef(pattern, charNdx);
        status = isOK ? RegExStatus.OK : RegExStatus.Warning;
        TagSeq(pattern, seqStart, charNdx - seqStart + 1, RegExTag.BackRef, status);
      }
      return status;
    }

    private RegExStatus TryParseHexadecimalCode(string pattern, ref int charNdx)
    {
      bool isSeq = false;
      bool isOK = true;
      RegExStatus status = RegExStatus.Unfinished;
      var seqStart = charNdx;
      charNdx++;
      if (charNdx < pattern.Length - 1)
      {
        charNdx++;
        isOK = HexadecimalDigits.Contains(pattern[charNdx]);
        if (isOK)
        {
          if (charNdx < pattern.Length - 1)
          {
            charNdx++;
            isOK = HexadecimalDigits.Contains(pattern[charNdx]);
            isSeq = true;
          }
        }
      }
      if (isSeq)
        status = isOK ? RegExStatus.OK : RegExStatus.Error;
      else
        status = isOK ? RegExStatus.Unfinished : RegExStatus.Error;
      TagSeq(pattern, seqStart, charNdx - seqStart + 1, RegExTag.HexadecimalSeq, status);
      return status;
    }

    private RegExStatus TryParseUnicode(string pattern, ref int charNdx)
    {
      bool isSeq = false;
      bool isOK = true;
      RegExStatus status = RegExStatus.Unfinished;
      var seqStart = charNdx;
      charNdx++;
      if (charNdx < pattern.Length - 1)
      {
        charNdx++;
        isOK = HexadecimalDigits.Contains(pattern[charNdx]);
        if (isOK)
        {
          if (charNdx < pattern.Length - 1)
          {
            charNdx++;
            isOK = HexadecimalDigits.Contains(pattern[charNdx]);
            if (isOK)
            {
              if (charNdx < pattern.Length - 1)
              {
                charNdx++;
                isOK = HexadecimalDigits.Contains(pattern[charNdx]);
                if (isOK)
                {
                  if (charNdx < pattern.Length - 1)
                  {
                    charNdx++;
                    isOK = HexadecimalDigits.Contains(pattern[charNdx]);
                    isSeq = true;
                  }
                }
              }
            }
          }
        }
      }
      if (isSeq)
        status = isOK ? RegExStatus.OK : RegExStatus.Error;
      else
        status = isOK ? RegExStatus.Unfinished : RegExStatus.Error;
      TagSeq(pattern, seqStart, charNdx - seqStart + 1, RegExTag.UnicodeSeq, status);
      return status;
    }

    private RegExStatus TryParseUnicodeCategory(string pattern, ref int charNdx)
    {
      bool isOK = true;
      bool isBraceOpened = false;
      bool isBraceClosed = false;
      RegExStatus status = RegExStatus.Unfinished;
      var seqStart = charNdx;
      charNdx += 2;
      if (charNdx < pattern.Length)
      {
        if (pattern[charNdx] == '{')
        {
          isBraceOpened = true;
          isOK = true;
          charNdx++;
          var nameStart = charNdx;
          for (; charNdx <= pattern.Length; charNdx++)
          {
            var ch = (charNdx < pattern.Length) ? pattern[charNdx] : '\0';
            if (ch == '}' || ch == '\0')
            {
              isBraceClosed = ch == '}';
              if (charNdx > nameStart)
              {
                var name = GetSubstring(pattern, nameStart, charNdx - nameStart);
                isOK = UnicodeNames.Instance.Contains(name);
                if (!isOK && !isBraceClosed)
                {
                  isOK = UnicodeNames.Instance.Where(item => item.StartsWith(name)).FirstOrDefault() != null;
                }
              }
              else if (isBraceClosed)
                isOK = false;
              break;
            }
            if (!(ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z'))
            {
              if (charNdx - nameStart < 2)
                isOK = false;
              else
              if (!(ch >= '0' && ch <= '9' || ch == '-'))
                isOK = false;
              if (!isOK)
                break;
            }
          }
        }
        else
          isOK = false;
      }
      if (isBraceOpened)
      {
        if (isBraceClosed)
          status = isOK ? RegExStatus.OK : RegExStatus.Error;
        else
          status = isOK ? RegExStatus.Unfinished : RegExStatus.Error;
      }
      else
        status = isOK ? RegExStatus.Unfinished : RegExStatus.Error;
      TagSeq(pattern, seqStart, charNdx - seqStart + 1, RegExTag.UnicodeCategorySeq, status);
      return status;
    }

    private RegExStatus TryParseCharset(string pattern, ref int charNdx, int level = 0)
    {
      bool isSeq = false;
      bool isOK = false;
      bool isRangeError = false;
      RegExStatus status = RegExStatus.Unfinished;
      var seqStart = charNdx;
      RegExCharset charset = new RegExCharset { Tag = RegExTag.CharSet, Start = seqStart, Str = GetSubstring(pattern, seqStart, 1) };
      Items.Add(charset);
      RegExStack.Push(Items);
      Items = charset.Items;

      for (charNdx = seqStart + 1; charNdx < pattern.Length; charNdx++)
      {
        var thisChar = pattern[charNdx];
        var nextChar = (charNdx < pattern.Length - 1) ? pattern[charNdx + 1] : '\0';
        if (thisChar == '\0')
        {
          isSeq = false;
          break;
        }
        else
        if (thisChar == '\\')
        {
          isOK = true;
          var status1 = TryParseEscapeSeq(pattern, ref charNdx);
          if (status == RegExStatus.Error)
            isOK = false;
          else
          {
            var lastItem = Items.LastOrDefault();
            if (lastItem!=null)
            {
              if (lastItem.Tag == RegExTag.AnchorControl)
                lastItem.Tag = RegExTag.EscapedChar;
              else
              if (lastItem.Tag == RegExTag.BackRef)
              {
                lastItem.Status = RegExStatus.OK;
                var digit = lastItem.Str[1];
                if (digit >= '0' && digit <= '7')
                  lastItem.Tag = RegExTag.OctalString;
                else
                  lastItem.Tag = RegExTag.EscapedChar;
                if (lastItem.Str.Length > 2)
                {
                  for (int i = 2; i < lastItem.Str.Length; i++)
                    Items.Add(new RegExItem { Tag = RegExTag.LiteralChar, Start = lastItem.Start+i, Str = new string(lastItem.Str[i], 1), Status=RegExStatus.OK });
                  lastItem.Str = lastItem.Str.Substring(0, 2);
                }
              }
            }
          }
        }
        else
        if (thisChar == ']')          
        {
          isSeq = true;
          break;
        }
        else
        if ((thisChar == '-') && (charNdx != seqStart + 1))
        {
          if (nextChar == '[' /*&& !(charNdx < pattern.Length - 2 && pattern[charNdx + 2] == ']')*/)
          {
            TagSeq(pattern, charNdx, 1, RegExTag.CharSetControlChar, RegExStatus.OK);
            //charset.IsNegative = true;
            charNdx++;
            status = TryParseCharset(pattern, ref charNdx, level + 1);
            isOK = isOK && status != RegExStatus.Error;
          }
          else if (nextChar == ']' /*&& !(charNdx < pattern.Length - 2 && pattern[charNdx + 2] == ']')*/)
          {
            TagSeq(pattern, charNdx, 1, RegExTag.LiteralChar, RegExStatus.OK);
            isOK = true;
          }
          else
          {
            var item = Items.Last();
            if (item.Tag == RegExTag.LiteralChar || item.Tag == RegExTag.EscapedChar)
            {
              var priorChar = item.Str[0];
              item.Str += "-" + nextChar;
              item.Tag = RegExTag.CharRange;
              if (priorChar > nextChar)
              {
                item.Status = RegExStatus.Error;
                isRangeError = true;
              }
              Items[Items.Count - 1] = new RegExCharRange { Tag = item.Tag, Start = item.Start, Str = item.Str, Status = item.Status };
              charNdx++;
            }
            else
            {
              TagSeq(pattern, charNdx, 1, RegExTag.LiteralChar, RegExStatus.OK);
              isOK = true;
            }
          }
        }
        else
        if (thisChar == '^' && charNdx == seqStart + 1)
        {
          TagSeq(pattern, charNdx, 1, RegExTag.CharSetControlChar, RegExStatus.OK);
          //charset.IsNegative = true;
        }
        else
        {
          isOK = true;
          TagSeq(pattern, charNdx, 1, RegExTag.LiteralChar, RegExStatus.OK);
        }
      }
      if (isSeq)
      {
        status = isRangeError ? RegExStatus.Error : (isOK ? RegExStatus.OK : RegExStatus.Error);
      }
      charset.Str = GetSubstring(pattern, seqStart, charNdx - seqStart + 1);
      charset.Status = status;
      Items = RegExStack.Pop();
      return status;
    }

    private RegExStatus TryParseQuantifier(string pattern, ref int charNdx)
    {
      bool isSeq = false;
      bool isOK = false;
      RegExStatus status = RegExStatus.OK;
      var seqStart = charNdx;
      var thisChar = pattern[charNdx];
      if (thisChar == '+' || thisChar == '?' || thisChar == '*')
      {
        isOK = true;
      }
      if (isOK)
      {
        if (thisChar == '?')
        {
          isOK = Items.Count > 0;
          if (isOK)
          {
            var lastItem = Items.Last();
            if (lastItem.Tag == RegExTag.AltChar)
              isOK = false;
            else
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
          isOK = Items.Count > 0 && Items.Last().Tag != RegExTag.Quantifier && Items.Last().Tag != RegExTag.AltChar;
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
      TagSeq(pattern, seqStart, charNdx - seqStart + 1, RegExTag.Quantifier, status);
      return status;
    }

    private RegExStatus TryParseNumQuantifier(string pattern, ref int charNdx)
    {
      bool isSeq = false;
      var seqStart = charNdx;
      int seqLength = 1;
      var thisChar = pattern[charNdx];
      int[] nums = new int[2];
      int numNdx = 0;
      for (int i = charNdx + 1; i < pattern.Length; i++)
      {
        thisChar = pattern[i];
        if (thisChar == '}')
        {
          if (i >= charNdx + 1)
          {
            isSeq = true;
            seqLength = i - seqStart;
            break;
          }
          else
            break;
        }
        else //~|
        if (DecimalDigits.Contains(thisChar))
          nums[numNdx] = nums[numNdx] * 10 + (thisChar - '0');
        else if (thisChar == ',' && numNdx == 0)
          numNdx++;
        else
          break;
      }
      if (!isSeq || seqLength <= 2)
        return RegExStatus.Unfinished;

      bool isOK = nums[0] > 0 && (nums[1] == 0 || nums[1] > nums[0]);
      if (isOK)
        isOK = Items.Count > 0 && Items.Last().Tag != RegExTag.Quantifier;
      var status = isOK ? RegExStatus.OK : RegExStatus.Warning;
      charNdx = seqStart + seqLength - 1;
      TagSeq(pattern, seqStart, charNdx - seqStart + 1, RegExTag.Quantifier, status);
      return status;
    }

    private bool CheckNumRef(string pattern, int charNdx)
    {
      var thisChar = (charNdx < pattern.Length) ? pattern[charNdx] : '\0';
      var isOK = DecimalDigits.Contains(thisChar);
      if (isOK)
      {
        int num = (int)(thisChar - '0');
        isOK = num > 0 && num <= NumGroupsCount;
      }
      return isOK;
    }

    private RegExItem TagSeq(string pattern, int seqStart, int length, RegExTag regExTag, RegExStatus status)
    {
      var str = GetSubstring(pattern, seqStart, length);
      RegExItem tag;
      if (regExTag == RegExTag.Subexpression)
        tag = new RegExGroup { Tag = regExTag, Start = seqStart, Status = status, Str = str };
      else if (regExTag == RegExTag.CharSet)
        tag = new RegExCharset { Tag = regExTag, Start = seqStart, Status = status, Str = str };
      else
        tag = new RegExItem { Tag = regExTag, Start = seqStart, Status = status, Str = str };
      Items.Add(tag);
      return tag;
    }

    private string GetSubstring(string pattern, int seqStart, int length)
    {
      if (seqStart + length > pattern.Length)
        length = pattern.Length - seqStart;
      if (length > 0)
        return pattern.Substring(seqStart, length);
      return "";
    }

    /// <summary>
    /// This method joins literal chars to literalstring
    /// </summary>
    /// <param name="items"></param>
    private void NormalizeTags(RegExItems items)
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
          NormalizeTags(group.Items);
      }
    }
  }
}
