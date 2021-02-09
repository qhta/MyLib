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
    const string OptionChars = "imnsx";

    static RegExTag[] CharRangeTags = new RegExTag[]
    {
      RegExTag.LiteralChar,
      RegExTag.EscapedChar,
      RegExTag.OctalSeq,
      RegExTag.HexadecimalSeq,
      RegExTag.UnicodeSeq,
      RegExTag.ControlCharSeq,
    };

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

    public SearchOrReplace Kind { get; set; }

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
      NormalizeItems(Items);
      NumberGroups(Items);
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
        var nextChar = (charNdx < pattern.Length - 1) ? pattern[charNdx + 1] : '\0';
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
            if (char.IsDigit(nextChar) || nextChar=='}')
              status1 = TryParseNumQuantifier(pattern, ref charNdx);
            else
            {
              status1 = RegExStatus.OK;
              TagSeq(pattern, charNdx, 1, RegExTag.LiteralChar, RegExStatus.OK);
            }
            break;
          case '[':
            status1 = TryParseCharset(pattern, ref charNdx);
            break;
          case '(':
            status1 = TryParseGroup(pattern, ref charNdx);
            break;
          case '^':
          case '$':
            status1 = RegExStatus.OK;
            TagSeq(pattern, charNdx, 1, RegExTag.AnchorControl, RegExStatus.OK);
            break;
          case '|':
            status1 = RegExStatus.OK;
            if (charNdx <= seqStart)
              status1 = RegExStatus.Warning;
            else if (nextChar == '\0')
              status1 = RegExStatus.Unfinished;
            else if (nextChar == ')')
              status1 = RegExStatus.Error;
            TagSeq(pattern, charNdx, 1, RegExTag.AltChar, status1);
            status = RegExTools.Max(status, status1);
            charNdx++;
            if (charNdx < pattern.Length)
            {
              status1 = TryParseSearchPattern(pattern, ref charNdx, finalSep);
              status = RegExTools.Max(status, status1);
            }
            return status;
          default:
            if (thisChar == finalSep)
              return status;
            if (thisChar == ')')
            {
              status1 = RegExStatus.Error;
              TagSeq(pattern, charNdx, 1, RegExTag.Subexpression, status1);
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
      RegExStatus status = RegExStatus.OK;
      for (; charNdx < pattern.Length; charNdx++)
      {
        RegExStatus status1 = RegExStatus.Unfinished;
        var thisChar = pattern[charNdx];
        var nextChar = (charNdx < pattern.Length - 1) ? pattern[charNdx + 1] : '\0';
        if (thisChar == '\\')
        {
          status1 = TryParseEscapeSeq(pattern, ref charNdx);
        }
        else if (thisChar == '$' && nextChar=='{')
        { 
          status1 = TryParseNamedGroupReplacement(pattern, ref charNdx);
        }
        else
        if (thisChar == finalSep)
          return status;
        else
        {
          status1 = RegExStatus.OK;
          TagSeq(pattern, charNdx, 1, RegExTag.LiteralChar, RegExStatus.OK);
        }
        if (status1 > status)
          status = status1;
        if (status1 == RegExStatus.Error)
          break;
      }
      return status;
    }

    private RegExStatus TryParseGroup(string pattern, ref int charNdx)
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
        var groupControlChar = TagSeq(pattern, charNdx, 1, RegExTag.GroupControlChar, RegExStatus.OK);
        charNdx++;
        var nextChar = charNdx < pattern.Length ? pattern[charNdx] : '\0';
        var nextNextChar = charNdx < pattern.Length-1 ? pattern[charNdx+1] : '\0';
        if (nextChar == '\0')
        {
          groupControlChar.Status = RegExStatus.OK;
          status = RegExStatus.Unfinished;
        }
        else
        if (nextChar == '<' && nextNextChar=='=')
        {
          group.Tag = RegExTag.BehindPositiveAssertion;
          TagSeq(pattern, charNdx, 1, RegExTag.GroupControlChar, RegExStatus.OK);
          charNdx++;
          TagSeq(pattern, charNdx, 1, RegExTag.GroupControlChar, RegExStatus.OK);
          charNdx++;
          var status1 = TryParsePattern(pattern, ref charNdx, ')');
          if (status1 == RegExStatus.Unfinished)
            status = RegExStatus.Unfinished;
          else
            status = RegExStatus.OK;
        }
        else
        if (nextChar == '<' && nextNextChar == '!')
        {
          group.Tag = RegExTag.BehindNegativeAssertion;
          TagSeq(pattern, charNdx, 1, RegExTag.GroupControlChar, RegExStatus.OK);
          charNdx++;
          TagSeq(pattern, charNdx, 1, RegExTag.GroupControlChar, RegExStatus.OK);
          charNdx++;
          var status1 = TryParsePattern(pattern, ref charNdx, ')');
          if (status1 == RegExStatus.Unfinished)
            status = RegExStatus.Unfinished;
          else
            status = RegExStatus.OK;
        }
        else
        if (nextChar == '<')
          status = TryParseNamedGroup(pattern, ref charNdx, group, '>');
        else
        if (nextChar == '\'')
          status = TryParseNamedGroup(pattern, ref charNdx, group, '\'');
        else
        if (nextChar == '(')
          status = TryParseBackRefNamedGroup(pattern, ref charNdx, group, ')');
        else
        if (nextChar == ':')
        {
          group.Tag = RegExTag.NonCapturingGroup;
          TagSeq(pattern, charNdx, 1, RegExTag.GroupControlChar, RegExStatus.OK);
          charNdx++;
          var status1 = TryParsePattern(pattern, ref charNdx, ')');
          if (status1 == RegExStatus.Unfinished)
            status = RegExStatus.Unfinished;
          else
            status = RegExStatus.OK;
        }
        else
        if (nextChar == '=')
        {
          group.Tag = RegExTag.AheadPositiveAssertion;
          TagSeq(pattern, charNdx, 1, RegExTag.GroupControlChar, RegExStatus.OK);
          charNdx++;
          var status1 = TryParsePattern(pattern, ref charNdx, ')');
          if (status1 == RegExStatus.Unfinished)
            status = RegExStatus.Unfinished;
          else
            status = RegExStatus.OK;
        }
        else
        if (nextChar == '!')
        {
          group.Tag = RegExTag.AheadNegativeAssertion;
          TagSeq(pattern, charNdx, 1, RegExTag.GroupControlChar, RegExStatus.OK);
          charNdx++;
          var status1 = TryParsePattern(pattern, ref charNdx, ')');
          if (status1 == RegExStatus.Unfinished)
            status = RegExStatus.Unfinished;
          else
            status = RegExStatus.OK;
        }
        else
        if (nextChar == '>')
        {
          group.Tag = RegExTag.NonBacktrackingGroup;
          TagSeq(pattern, charNdx, 1, RegExTag.GroupControlChar, RegExStatus.OK);
          charNdx++;
          var status1 = TryParsePattern(pattern, ref charNdx, ')');
          if (status1 == RegExStatus.Unfinished)
            status = RegExStatus.Unfinished;
          else
            status = RegExStatus.OK;
        }
        else
        if (OptionChars.Contains(nextChar) || nextChar=='-' || nextChar == ':')
        {
          group.Tag = RegExTag.LocalOptionsGroup;
          status = TryParseLocalOptionsGroup(pattern, ref charNdx, group);
        }
        else
        {
          var status1 = TryParsePattern(pattern, ref charNdx, ')');
          if (group.Items.Count > 1 && group.Items[1].Tag == RegExTag.Quantifier && group.Items[1].Status == RegExStatus.OK)
            group.Items[1].Status = RegExStatus.Warning;
          groupControlChar.Status = RegExStatus.Warning;
          status = RegExStatus.Warning;
        }
      }
      else
      {
        var status1 = TryParsePattern(pattern, ref charNdx, ')');
        if (status1 == RegExStatus.Unfinished)
          status = RegExStatus.Unfinished;
        else
          status = RegExStatus.OK;
      }
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
      if (status != RegExStatus.Unfinished)
      {
        var status1 = TryParsePattern(pattern, ref charNdx, ')');
        if (status1 > status)
          status = status1;
      }
      return status;
    }

    private RegExStatus TryParseNamedGroupReplacement(string pattern, ref int charNdx)
    {
      var seqStart = charNdx;
      var backref = new RegExBackRef { Tag = RegExTag.Replacement, Start = charNdx, Str = GetSubstring(pattern, charNdx, 1) };
      Items.Add(backref);
      charNdx++;
      RegExStack.Push(Items);
      Items = backref.Items;
      var nextChar = charNdx < pattern.Length ? pattern[charNdx] : '\0';
      if (nextChar == '\0')
      {
        backref.Status = RegExStatus.Unfinished;
      }
      else
        backref.Status = TryParseNamedGroupBackref(pattern, ref charNdx, backref, '}');
      backref.Str = GetSubstring(pattern, seqStart, charNdx - seqStart + 1);
      charNdx++;
      Items = RegExStack.Pop();
      var status = backref.Status;
      if (status != RegExStatus.Unfinished)
      {
        var status1 = TryParsePattern(pattern, ref charNdx, ')');
        if (status1 > status)
          status = status1;
      }
      return status;
    }

    private RegExStatus TryParseBackRefNamedGroup(string pattern, ref int charNdx, RegExGroup group, char nameEnd)
    {
      group.Tag = RegExTag.BackRefNamedGroup;
      var seqStart = charNdx;
      var backref = new RegExBackRef { Tag = RegExTag.BackRef, Start = charNdx, Str = GetSubstring(pattern, charNdx, 1) };
      group.Items.Add(backref);
      RegExStack.Push(Items);
      Items = backref.Items;
      var nextChar = charNdx < pattern.Length ? pattern[charNdx] : '\0';
      if (nextChar == '\0')
      {
        backref.Status = RegExStatus.Unfinished;
      }
      else
        backref.Status = TryParseNamedGroupBackref(pattern, ref charNdx, backref, nameEnd);
      backref.Str = GetSubstring(pattern, seqStart, charNdx - seqStart + 1);
      charNdx++;
      Items = RegExStack.Pop();
      var status = backref.Status;
      if (status != RegExStatus.Unfinished)
      {
        var status1 = TryParsePattern(pattern, ref charNdx, ')');
        if (status1 > status)
          status = status1;
      }
      return status;
    }

    private RegExStatus TryParseNamedGroupBackref(string pattern, ref int charNdx)
    {
      var seqStart = charNdx;
      var backref = new RegExBackRef { Tag = RegExTag.BackRef, Start = charNdx, Str = GetSubstring(pattern, charNdx, 2) };
      Items.Add(backref);
      RegExStack.Push(Items);
      Items = backref.Items;
      charNdx += 2;
      var nextChar = charNdx < pattern.Length ? pattern[charNdx] : '\0';
      if (nextChar == '\0')
      {
        backref.Status = RegExStatus.Unfinished;
      }
      else
      if (nextChar == '<')
        backref.Status = TryParseNamedGroupBackref(pattern, ref charNdx, backref, '>');
      else
      if (nextChar == '\'')
        backref.Status = TryParseNamedGroupBackref(pattern, ref charNdx, backref, '\'');
      else
        backref.Status = RegExStatus.Error;
      backref.Str = GetSubstring(pattern, seqStart, charNdx - seqStart + 1);
      Items = RegExStack.Pop();
      return backref.Status;
    }

    private RegExStatus TryParseNamedGroupBackref(string pattern, ref int charNdx, RegExBackRef group, char nameEnd)
    {
      var status = TryParseGroupName(pattern, ref charNdx, group, nameEnd);
      if (status != RegExStatus.Unfinished)
        charNdx--;
      return status;
    }

    private RegExStatus TryParseGroupName(string pattern, ref int charNdx, RegExNamedItem group, char finalSep)
    {
      var nameQuote = TagSeq(pattern, charNdx, 1, RegExTag.NameQuote, RegExStatus.Unfinished);
      charNdx++;
      RegExStack.Push(Items);
      Items = nameQuote.Items;
      RegExStatus status = RegExStatus.Unfinished;
      string name = null;
      int seqStart = charNdx;
      RegExItem nameItem = new RegExGroupName { Tag = RegExTag.GroupName, Start = charNdx };
      nameQuote.Items.Add(nameItem);
      char thisChar = '\0';
      string name1 = null;
      for (; charNdx <= pattern.Length; charNdx++)
      {
        thisChar = charNdx < pattern.Length ? pattern[charNdx] : '\0';
        if (thisChar == '\0')
        {
          status = nameItem.Status = RegExStatus.Unfinished;
          break;
        }
        else
        if (thisChar == ' ')
        {
          status = nameItem.Status = RegExStatus.Warning;
          name += thisChar;
        }
        else
        if (char.IsLetter(thisChar) || thisChar == '_')
        {
          if (status == RegExStatus.Unfinished)
            status = nameItem.Status = RegExStatus.OK;
          name += thisChar;
        }
        else
        if (char.IsDigit(thisChar))
        {
          if (name == null)
            status = nameItem.Status = RegExStatus.Warning;
          else
          if (status == RegExStatus.Unfinished)
            status = nameItem.Status = RegExStatus.OK;
          name += thisChar;
        }
        else
        if (thisChar == '-')
        {
          group.Tag = RegExTag.BalancingGroup;
          var nameStartStr = nameItem.Str = GetSubstring(pattern, seqStart, charNdx - seqStart);
          nameItem.Status = status;
          var dashItem = TagSeq(pattern, charNdx, 1, RegExTag.GroupControlChar, RegExStatus.OK);
          nameItem = new RegExGroupName { Tag = RegExTag.GroupName, Start = charNdx + 1 };
          nameQuote.Items.Add(nameItem);
          if (name1 == null)
            name1 = nameStartStr;
          else
          {
            status = dashItem.Status = RegExStatus.Error;
            nameItem.Status = RegExStatus.Warning;
          }
          name = null;
          seqStart = charNdx + 1;
        }
        else
        if (thisChar == finalSep)
        {
          break;
        }
        else
        {
          status = RegExStatus.Error;
          break;
        }
      }
      nameItem.Str = GetSubstring(pattern, seqStart, charNdx - seqStart);
      if (thisChar == finalSep)
      {
        nameQuote.Status = RegExStatus.OK;
        nameQuote.Str = pattern.Substring(nameQuote.Start, charNdx - nameQuote.Start + 1);
        charNdx++;
      }
      else //if (thisChar != '\0')
      {
        nameQuote.Status = RegExStatus.Unfinished;
        nameQuote.Str = pattern.Substring(nameQuote.Start, charNdx - nameQuote.Start);
        if (thisChar != '\0')
        {
          var invalidChar = new RegExItem { Start = charNdx, Str = pattern.Substring(charNdx, 1), Tag = RegExTag.GroupControlChar, Status = RegExStatus.Error };
          group.Items.Add(invalidChar);
          charNdx++;
        }
        if (nameItem.Status == RegExStatus.OK)
          nameItem.Status = RegExStatus.Unfinished;
      }
      if (name1 != null)
        group.Name = name1;
      else
        group.Name = name;
      Items = RegExStack.Pop();
      return status;
    }

    private RegExStatus TryParseLocalOptionsGroup(string pattern, ref int charNdx, RegExNamedItem group)
    {
      RegExStatus status = TryParseLocalOptions(pattern, ref charNdx, group);
      if (status != RegExStatus.Unfinished)
      {
        var status1 = TryParsePattern(pattern, ref charNdx, ')');
        if (status1 > status)
          status = status1;
      }
      group.Status = status;
      return status;
    }

    private RegExStatus TryParseLocalOptions(string pattern, ref int charNdx, RegExNamedItem group)
    {
      RegExStatus status = RegExStatus.OK;
      RegExOptions options;
      var curChar = (charNdx < pattern.Length) ? pattern[charNdx] : '\0';
      if (OptionChars.Contains(curChar))
      {
        options = new RegExOptions { Tag = RegExTag.OptionSet, Start = charNdx };
        group.Items.Add(options);
        status = TryParseLocalOptions(pattern, ref charNdx, options);
        curChar = (charNdx < pattern.Length) ? pattern[charNdx] : '\0';
      }
      if (curChar=='-')
      {
        group.Items.Add(new RegExItem { Tag = RegExTag.GroupControlChar, Str = pattern.Substring(charNdx,1), Start = charNdx, Status = RegExStatus.OK });
        charNdx++;
        options = new RegExOptions { Tag = RegExTag.OptionSet, Start = charNdx };
        group.Items.Add(options);
        RegExStatus status1 = TryParseLocalOptions(pattern, ref charNdx, options);
        curChar = (charNdx < pattern.Length) ? pattern[charNdx] : '\0';
        if (status1 > status)
          status = status1;
      }
      if (curChar == ':')
      {
        group.Items.Add(new RegExItem { Tag = RegExTag.GroupControlChar, Str = pattern.Substring(charNdx, 1), Start = charNdx, Status = RegExStatus.OK });
        charNdx++;
      }
      else
      if (curChar == ')')
      {
        // accept empty subexpression
      }
      else
      if (curChar != '\0')
      {
        status = RegExStatus.Error;
        group.Items.Add(new RegExItem { Tag = RegExTag.GroupControlChar, Str = pattern.Substring(charNdx, 1), Start = charNdx, Status = RegExStatus.Error });
        charNdx++;
      }
      else
        status = RegExStatus.Unfinished;
      return status;
    }

    private RegExStatus TryParseLocalOptions(string pattern, ref int charNdx, RegExOptions options)
    {
      RegExStatus status = RegExStatus.OK;
      var seqStart = charNdx;
      char curChar;
      while (char.IsLetter(curChar = (charNdx < pattern.Length) ? pattern[charNdx] : '\0'))
      {
        bool isValid = OptionChars.Contains(curChar);
        var optionStr = new string(curChar, 1);
        bool duplicated = options.Items.Where(item => item.Str == optionStr).FirstOrDefault() != null;
        options.Items.Add(new RegExItem { Tag = RegExTag.OptionChar, Str = optionStr, Start = charNdx, 
          Status = isValid ? (duplicated ? RegExStatus.Warning : RegExStatus.OK) : RegExStatus.Error });
        charNdx++;
      }
      if (curChar == '\0')
      {
        status = RegExStatus.Unfinished;
      }
      else
        options.Str = pattern.Substring(seqStart, charNdx - seqStart);
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
      if (nextChar == 'k')
      {
        return TryParseNamedGroupBackref(pattern, ref charNdx);
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
        TagSeq(pattern, seqStart, charNdx - seqStart + 1, RegExTag.OctalSeq, status);
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
      RegExCharSet charset = new RegExCharSet { Tag = RegExTag.CharSet, Start = seqStart, Str = GetSubstring(pattern, seqStart, 1) };
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
            if (lastItem != null)
            {
              if (lastItem.Tag == RegExTag.AnchorControl)
                lastItem.Tag = RegExTag.EscapedChar;
              else
              if (lastItem.Tag == RegExTag.BackRef)
              {
                lastItem.Status = RegExStatus.OK;
                var digit = lastItem.Str[1];
                if (digit >= '0' && digit <= '7')
                  lastItem.Tag = RegExTag.OctalSeq;
                else
                  lastItem.Tag = RegExTag.EscapedChar;
                if (lastItem.Str.Length > 2)
                {
                  for (int i = 2; i < lastItem.Str.Length; i++)
                    Items.Add(new RegExItem { Tag = RegExTag.LiteralChar, Start = lastItem.Start + i, Str = new string(lastItem.Str[i], 1), Status = RegExStatus.OK });
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
            var status1 = TryParseCharset(pattern, ref charNdx, level + 1);
            isOK = isOK && status1 != RegExStatus.Error;
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
              var rangeStr = priorChar + "-" + nextChar;
              if (priorChar > nextChar)
              {
                isRangeError = true;
              }
              var rangeItem = new RegExCharRange { Tag = RegExTag.CharRange, Start = item.Start, Str = rangeStr, Status = isRangeError ? RegExStatus.Error : item.Status };
              rangeItem.Items.Add(item);
              rangeItem.Items.Add(new RegExItem { Tag = RegExTag.CharSetControlChar, Start = item.Start + 1, Str = "-", Status = RegExStatus.OK });
              rangeItem.Items.Add(new RegExItem { Tag = RegExTag.LiteralChar, Start = item.Start + 2, Str = new string(nextChar, 1), Status = RegExStatus.OK });
              Items[Items.Count - 1] = rangeItem;
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
      charset.Str = GetSubstring(pattern, seqStart, charNdx - seqStart + (isSeq ? 1 : 0));
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
      RegExQuantifier quantifier = new RegExQuantifier { Tag = RegExTag.Quantifier, Start = charNdx };
      Items.Add(quantifier);
      bool isSeq = false;
      var seqStart = charNdx;
      charNdx++;
      var numStart = charNdx;
      int numNdx = 0;
      var status = RegExStatus.Unfinished;
      var status1 = RegExStatus.OK;
      for (; charNdx <= pattern.Length; charNdx++)
      {
        var curChar = charNdx < pattern.Length ? pattern[charNdx] : '\0';
        if (curChar == '\0')
        {
          quantifier.Str = GetSubstring(pattern, seqStart, charNdx - seqStart);
          break;
        }
        else
        if (curChar == ',')
        {
          if (numNdx > 0 || charNdx == numStart)
            status1 = RegExStatus.Error;
          if (charNdx > numStart)
            quantifier.Items.Add(new RegExItem { Tag = RegExTag.Number, Start = numStart, Str = GetSubstring(pattern, numStart, charNdx - numStart), Status = status1 });
          quantifier.Items.Add(new RegExItem { Tag = RegExTag.Separator, Start = charNdx, Str = GetSubstring(pattern, charNdx, 1), Status = status1 });
          numNdx++;
          numStart = charNdx + 1;
        }
        else
        if (curChar == '}')
        {
          if (charNdx > numStart)
            quantifier.Items.Add(new RegExItem { Tag = RegExTag.Number, Start = numStart, Str = GetSubstring(pattern, numStart, charNdx - numStart), Status = status1 });
          if (status == RegExStatus.Unfinished)
          {
            status = RegExStatus.OK;
          }
          quantifier.Str = GetSubstring(pattern, seqStart, charNdx - seqStart + 1);
          isSeq = true;
          break;
        }
        else
        if (!DecimalDigits.Contains(curChar))
        {
          status = RegExStatus.Error;
          quantifier.Str = GetSubstring(pattern, seqStart, charNdx - seqStart);
          break;
        }
      }
      if (quantifier.Items.Count > 0)
      {
        List<int> nums = new List<int>();
        foreach (var item in quantifier.Items)
        {
          if (item.Tag == RegExTag.Number)
          {
            if (int.TryParse(item.Str, out int n))
              nums.Add(n);
            else
              item.Status = RegExStatus.Error;
          }
        }
        if (nums.Count >= 2)
        {
          bool isOK = (nums[1] == 0 || nums[1] >= nums[0]);
          if (!isOK && status == RegExStatus.OK)
            status = RegExStatus.Warning;
        }
        else if (nums.Count == 0)
          status = RegExStatus.Error;
      }
      else
        status = RegExStatus.Error;
      quantifier.Status = status;
      if (!isSeq)
        return RegExStatus.Unfinished;
      else
      if (charNdx < pattern.Length - 1 && pattern[charNdx + 1] == '?')
      {
        quantifier.Str += '?';
        charNdx++;
      }
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
        tag = new RegExCharSet { Tag = regExTag, Start = seqStart, Status = status, Str = str };
      else if (regExTag == RegExTag.CharRange)
        tag = new RegExCharRange { Tag = regExTag, Start = seqStart, Status = status, Str = str };
      else if (regExTag == RegExTag.Quantifier)
        tag = new RegExQuantifier { Tag = regExTag, Start = seqStart, Status = status, Str = str };
      else if (regExTag == RegExTag.NameQuote)
        tag = new RegExNameQuote { Tag = regExTag, Start = seqStart, Status = status, Str = str };
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
    /// and produces normalized RegExCharRange items
    /// </summary>
    /// <param name="items"></param>
    private void NormalizeItems(RegExItems items)
    {
      for (int i = items.Count - 1; i >= 0; i--)
      {
        var item = items[i];
        if (item.Tag == RegExTag.Quantifier)
        {
          i--;
        }
        else if (item.Tag == RegExTag.LiteralChar || item.Tag == RegExTag.LiteralString)
        {
          if (i > 0)
          {
            var priorItem = items[i - 1];
            if (priorItem.Tag == RegExTag.LiteralChar || priorItem.Tag == RegExTag.LiteralString)
            {
              priorItem.Tag = RegExTag.LiteralString;
              priorItem.Str += item.Str;
              items.RemoveAt(i);
            }
          }
        }
        else if (item is RegExGroup)
          NormalizeItems(item.Items);
        else if (item is RegExCharSet)
          NormalizeCharsetItems(item.Items);
      }
    }

    private void NormalizeCharsetItems(RegExItems items)
    {
      for (int i = items.Count - 1; i >= 0; i--)
      {
        var item = items[i];
        if (item.Str == "-")
        {
          if (i < items.Count - 1 && i > 0)
          {
            var priorItem = items[i - 1];
            var nextItem = items[i + 1];
            if (CharRangeTags.Contains(priorItem.Tag) && CharRangeTags.Contains(nextItem.Tag))
            {
              var str = priorItem.Str + item.Str + nextItem.Str;
              var isOK = priorItem.CharValue <= nextItem.CharValue;
              var rangeItem = new RegExCharRange { Tag = RegExTag.CharRange, Start = priorItem.Start, Str = str, Status = isOK ? RegExStatus.OK : RegExStatus.Error };
              rangeItem.Items.Add(priorItem);
              item.Tag = RegExTag.CharSetControlChar;
              rangeItem.Items.Add(item);
              rangeItem.Items.Add(nextItem);
              items.RemoveAt(i - 1);
              items.RemoveAt(i - 1);
              items.RemoveAt(i - 1);
              items.Insert(i - 1, rangeItem);
              i--;
            }
          }
        }
        else
        if (item.Tag == RegExTag.CharRange && item.Items.Count == 0)
        {
          var k = item.Str.IndexOf('-');
          if (k > 0 && k < item.Str.Length - 1)
          {
            var s1 = item.Str.Substring(0, k);
            var s2 = item.Str.Substring(k + 1);
            RegExItem priorItem = new RegExItem { Tag = RegExTag.LiteralChar, Start = item.Start, Str = s1, Status = RegExStatus.OK };
            RegExItem midItem = new RegExItem { Tag = RegExTag.CharSetControlChar, Start = priorItem.Start + priorItem.Length, Str = "-", Status = RegExStatus.OK };
            RegExItem nextItem = new RegExItem { Tag = RegExTag.LiteralChar, Start = midItem.Start + midItem.Length, Str = s2, Status = RegExStatus.OK };
            item.Items.Add(priorItem);
            item.Items.Add(midItem);
            item.Items.Add(nextItem);
          }
        }
        else
        if (item is RegExCharSet)
          NormalizeCharsetItems(item.Items);
      }
    }

    public int NumberGroups(RegExItems itemList)
    {
      int groupNumber = 0;
      NumberGroups(itemList, ref groupNumber);
      return groupNumber;
    }

    public void NumberGroups(RegExItems itemList, ref int groupNumber)
    {
      foreach (var item in itemList)
      {
        if (item is RegExGroup group)
        {
          if (group.Name == null && group.Tag != RegExTag.NonCapturingGroup)
            group.GroupNumber = ++groupNumber;
          NumberGroups(group.Items, ref groupNumber);

        }
      }      
    }

  }
}
