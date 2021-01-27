using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qhta.RegularExpressions.Descriptions
{
  public static class PaternItemsGenerator
  {
    static readonly Dictionary<string, string> EscapeSeqNames = new Dictionary<string, string>
    {
      {@"\a", "bell" },
      {@"\b", "backspace"},
      {@"\t", "tab" },
      {@"\r", "carriage return" },
      {@"\v", "vertical tab" },
      {@"\f", "form feed"},
      {@"\n", "new line" },
      {@"\e", "escape" },
    };

    static readonly Dictionary<string, string> AnchorDescriptions = new Dictionary<string, string>
    {
      {@"^", "start of line" },
      {@"$", "end of line" },
      {@"\b", "word boundary" },
      {@"\B", "not word boundary" },
      {@"\A", "start of string"},
      {@"\Z", "end of string" },
      {@"\G", "Begin the match where the last match ended" },
    };

    static readonly Dictionary<string, string> CharClassDescriptions = new Dictionary<string, string>
    {
      {@"\s", "white-space character" },
      {@"\S", "non-white-space character" },
    };

    static readonly Dictionary<string, string> UnicodeCategoryNames = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
    {
      {@"\p{P}", "punctuation mark" },
    };

    static readonly Dictionary<string, string> QuantifierSeqDescriptions = new Dictionary<string, string>
    {
      {@"?", "zero or one occurrence occurence of {0}" },
      {@"+", "one or more occurrence of {0}" },
      {@"*", "zero or more occurrences of {0}" },
      {@"+?", "one or more occurrence of {0}, but as few as possible" },
      {@"*?", "zero or more occurrences of {0}, but as few as possible" },
    };

    static readonly Dictionary<string, string> UnicodeSeqNames = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
    {
      {@"\u007C", "vertical bar" },
    };

    static readonly Dictionary<string, string> SpecialCases = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
    {
      {@"(.+)", "Match any character one or more times" },
      {@"\r?\n", "Match zero or one occurrence of a carriage return character followed by a new line character" },
      {@"[ae]", "either an \"a\" or an \"e\""},
    };

    static readonly Dictionary<int, string> OrdinalNames = new Dictionary<int, string>
    {
      {1, "first" },
      {2, "second"},
      {3, "third" },
      {5, "fifth"},
      {6, "sixth"},
      {7, "seventh"},
      {8, "eighth"},
      {9, "nineth"},
    };

    const string Vowels = "aeiouy";

    public static PatternItems GeneratePatternItems(this RegExItems itemList)
    {
      PatternItems result = new PatternItems();
      int itemIndex = 0;
      int groupNumber = 0;
      while (itemIndex < itemList.Count)
      {
        var patternItem = CreatePatternItem(itemList, ref itemIndex, ref groupNumber); ;
        result.Add(patternItem);
      }
      return result;
    }

    static PatternItem CreatePatternItem(IList<RegExItem> itemList, ref int itemIndex, ref int groupNumber)
    {
      var curItem = itemList[itemIndex];

      PatternItem result = CreatePatternItemSpecialCase(itemList, ref itemIndex, ref groupNumber);
      if (result != null)
        return result;
      result = new PatternItem { Str = curItem.Str };
      switch (curItem.Tag)
      {
        case RegExTag.Subexpression:
          result = CreatePatternItem(itemList, ref itemIndex);
          result.Description += GroupNumberAppendix(ref groupNumber);
          break;
        default:
          result = CreatePatternItem(itemList, ref itemIndex);
          if (itemIndex< itemList.Count)
          {
            curItem = itemList[itemIndex];
            if (curItem.Tag==RegExTag.CharClass)
            {
              if (result.Description.StartsWith("a "))
                result.Description = "the "+ result.Description.Substring(2);
              else
              if (result.Description.StartsWith("an "))
                result.Description = "the " + result.Description.Substring(3);
              result.Description += " followed by " + CreatePatternItem(itemList, ref itemIndex).Description;
              result.Str += curItem.Str;
            }
          }
          break;
      }
      if (!Char.IsUpper(result.Description.FirstOrDefault()))
        result.Description = "Match " + result.Description;
      result.Description += ".";
      return result;
    }

    static PatternItem CreatePatternItemSpecialCase(IList<RegExItem> itemList, ref int itemIndex, ref int groupNumber)
    {
      var curItem = itemList[itemIndex];
      var str = "";
      for (int i = 0; i < 3; i++)
      {
        int j = itemIndex + i;
        if (j < itemList.Count)
        {
          curItem = itemList[j];
          str += curItem.Str;
          if (SpecialCases.TryGetValue(str, out string description))
          {
            if (curItem.Tag == RegExTag.Subexpression)
              description += GroupNumberAppendix(ref groupNumber);
            if (!Char.IsUpper(description.FirstOrDefault()))
              description = "Match " + description;
            description += ".";
            PatternItem result = new PatternItem { Str = str, Description = description };
            itemIndex = j + 1;
            return result;
          }
        }
      }
      return null;
    }

    static string GroupNumberAppendix(ref int groupNumber)
    {
      groupNumber++;
      if (!OrdinalNames.TryGetValue(groupNumber, out string ordinalName))
        throw new NotImplementedException($"CreatePatternItem error: Ordinal name for a \"{groupNumber}\" group number not found");
      var description = $". This is the {ordinalName} capturing group";
      return description;
    }

    static PatternItem CreatePatternItem(IList<RegExItem> itemList, ref int itemIndex)
    {
      var result = CreateSpecificPatternItem(itemList, ref itemIndex);
      var nextItem = (itemIndex < itemList.Count) ? itemList[itemIndex] : null;
      if (nextItem != null && nextItem.Tag == RegExTag.Quantifier)
      {
        if (!QuantifierSeqDescriptions.TryGetValue(nextItem.Str, out string quantifierDescription))
          throw new NotImplementedException($"CreatePatternItem error: quantifier description for a \"{nextItem.Str}\" not found");
        result.Str += nextItem.Str;
        result.Description = String.Format(quantifierDescription, result.Description);
        itemIndex++;
      }
      return result;
    }

    static PatternItem CreateSpecificPatternItem(IList<RegExItem> itemList, ref int itemIndex)
    {
      var curItem = itemList[itemIndex];
      itemIndex++;
      PatternItem result = new PatternItem { Str = curItem.Str };
      var startIndex = itemIndex;
      var nextItem = (itemIndex < itemList.Count) ? itemList[itemIndex] : null;
      List<String> subStrings;
      int subIndex;
      switch (curItem.Tag)
      {
        case RegExTag.LiteralChar:
          result.Description = $"a literal character \"{curItem.Str}\"";
          break;
        case RegExTag.LiteralString:
          result.Description = $"the literal characters \"{curItem.Str}\"";
          break;
        case RegExTag.EscapedChar:
          if (!EscapeSeqNames.TryGetValue(curItem.Str, out string charName))
            throw new NotImplementedException($"CreatePatternItem error: Char name for a \"{curItem.Str}\" not found");
          result.Description = $"{(Vowels.Contains(charName.FirstOrDefault()) ? "an" : "a")} {charName} character";
          break;
        case RegExTag.AnchorControl:
          if (!AnchorDescriptions.TryGetValue(curItem.Str, out string anchorDescription))
            throw new NotImplementedException($"CreatePatternItem error: Anchor description for a \"{curItem.Str}\" not found");
          if (Char.IsUpper(anchorDescription.FirstOrDefault()))
            result.Description = anchorDescription;
          else
            result.Description = $"{(Vowels.Contains(anchorDescription.FirstOrDefault()) ? "an" : "a")} {anchorDescription}";
          break;
        case RegExTag.CharClass:
          if (!CharClassDescriptions.TryGetValue(curItem.Str, out string className))
            throw new NotImplementedException($"CreatePatternItem error: Char class name for a \"{curItem.Str}\" not found");
          result.Description = $"{(Vowels.Contains(className.FirstOrDefault()) ? "an" : "a")} {className}";
          break;
        case RegExTag.DotChar:
          result.Description = $"any character";
          break;
        case RegExTag.UnicodeSeq:
          if (!UnicodeSeqNames.TryGetValue(curItem.Str, out string unicodeName))
            throw new NotImplementedException($"CreatePatternItem error: Unicode char name for a \"{curItem.Str}\" not found");
          result.Description = $"{(Vowels.Contains(unicodeName.FirstOrDefault()) ? "an" : "a")} {unicodeName} character";
          break;
        case RegExTag.UnicodeCategorySeq:
          if (!UnicodeCategoryNames.TryGetValue(curItem.Str, out string categoryName))
            throw new NotImplementedException($"CreatePatternItem error: Unicode category name for a \"{curItem.Str}\" not found");
          result.Description = $"{(Vowels.Contains(categoryName.FirstOrDefault()) ? "an" : "a")} {categoryName}";
          break;
        case RegExTag.Quantifier:
          throw new InvalidOperationException($"CreatePatternItem error: Quantifier must not be textualized here");
        case RegExTag.CharSet:
          subStrings = new List<string>();
          subIndex = 0;
          while (subIndex < curItem.SubItems.Count)
            subStrings.Add(CreatePatternItem(curItem.SubItems, ref subIndex).Description);
          result.Description = String.Join(" or ", subStrings);
          if (curItem.SubItems.Count == 2)
            result.Description = "either " + result.Description;
          break;
        case RegExTag.Subexpression:
          subStrings = new List<string>();
          subIndex = 0;
          while (subIndex < curItem.SubItems.Count)
            subStrings.Add(CreatePatternItem(curItem.SubItems, ref subIndex).Description);
          result.Description = String.Join(" ", subStrings);
          break;
        default:
          throw new NotImplementedException($"CreatePatternItem for a {curItem.Tag} not implemented");
      }
      return result;
    }

  }
}
