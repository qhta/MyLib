using System;
using System.Collections.Generic;
using System.Linq;

namespace Qhta.RegularExpressions.Descriptions
{
  public static class PatternItemsGenerator
  {
    static readonly Dictionary<string, string> CharNames = new Dictionary<string, string>
    {
      {@",", "comma" },
      {@".", "period"},
      {@";", "semicolon" },
      {@"-", "hyphen" },
    };

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
      {@"^", "the start of the input string" },
      {@"$", "the end of the input string" },
      {@"\b", "word boundary" },
      {@"\B", "not word boundary" },
      {@"\A", "the start of the string"},
      {@"\z", "the end of the string" },
      {@"\G", "where the last match ended" },
    };

    static readonly Dictionary<string, string> QuantifierLongDescriptions = new Dictionary<string, string>
    {
      {@"?", "zero or one occurrence of {0}" },
      {@"+", "one or more occurrence of {0}" },
      {@"*", "zero or more occurrences of {0}" },
      {@"+?", "one or more occurrence of {0}, but as few as possible" },
      {@"*?", "zero or more occurrences of {0}, but as few as possible" },
    };

    static readonly Dictionary<string, string> QuantifierShortDescriptions = new Dictionary<string, string>
    {
      {@"?", "zero or one {0}" },
      {@"+", "one or more {0}" },
      {@"*", "zero or more {0}" },
      {@"+?", "one or more {0}, but as few as possible" },
      {@"*?", "zero or more {0}, but as few as possible" },
    };

    static readonly Dictionary<string, string> QuantifierSuffixDescriptions = new Dictionary<string, string>
    {
      {@"?", "zero or one times" },
      {@"+", "one or more times" },
      {@"*", "zero or more times" },
      {@"+?", "one or more times, but as few as possible" },
      {@"*?", "zero or more times, but as few as possible" },
    };

    static readonly Dictionary<int, string> NumeralNames = new Dictionary<int, string>
    {
      {0, "zero" },
      {1, "one" },
      {2, "two" },
      {3, "three" },
      {4, "four" },
      {5, "five" },
      {6, "six" },
      {7, "seven" },
      {8, "eight" },
      {9, "nine" },
      {10, "ten" },
    };

    static readonly Dictionary<int, string> OrdinalNames = new Dictionary<int, string>
    {
      {1, "first" },
      {2, "second"},
      {3, "third" },
      {4, "fourth" },
      {5, "fifth"},
      {6, "sixth"},
      {7, "seventh"},
      {8, "eighth"},
      {9, "nineth"},
    };

    static readonly Dictionary<string, string> CharClassDescriptions = new Dictionary<string, string>
    {
      {@"\s", "white-space character" },
      {@"\S", "non-white-space character" },
      {@"\d", "decimal digit" },
      {@"\D", "any char except decimal digit" },
      {@"\w", "word character" },
    };

    static readonly Dictionary<string, string> UnicodeCategoryNames = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
    {
      {@"\p{Lu}", "uppercase letters" },
      {@"\p{Ll}", "lowercase letters" },
      {@"\p{Lt}", "titlecase letters" },
      {@"\p{Lm}", "modifier letters" },
      {@"\p{Lo}", "other letters" },
      {@"\p{L}", "letters" },
      {@"\p{Mn}", "nonspacing marks" },
      {@"\p{Mc}", "spacing combining marks" },
      {@"\p{Me}", "enclosing marks" },
      {@"\p{M}", "diacritic marks" },
      {@"\p{Nd}", "decimal digits" },
      {@"\p{Nl}", "letterlike numbers" },
      {@"\p{No}", "other numbers" },
      {@"\p{N}", "numbers" },
      {@"\p{Pc}", "connector punctuation" },
      {@"\p{Pd}", "dash punctuation" },
      {@"\p{Ps}", "open punctuation" },
      {@"\p{Pe}", "close punctuation" },
      {@"\p{Pi}", "initial quotes" },
      {@"\p{Pf}", "final quotes" },
      {@"\p{Po}", "other punctuation" },
      {@"\p{P}", "punctuation" },
      {@"\p{Sm}", "math symbols" },
      {@"\p{Sc}", "currency symbols" },
      {@"\p{Sk}", "modifier symbols" },
      {@"\p{So}", "other symbols" },
      {@"\p{S}", "symbols" },
      {@"\p{Zs}", "space separators" },
      {@"\p{Zl}", "line separators" },
      {@"\p{Zp}", "paragraph separators" },
      {@"\p{Z}", "separators" },
      {@"\p{Cc}", "other control characters" },
      {@"\p{Cf}", "format control characters" },
      {@"\p{Cs}", "surrogate characters" },
      {@"\p{Co}", "private use characters" },
      {@"\p{Cn}", "not assigned characters" },
      {@"\p{C}", "control characters" },
      {@"\p{IsAlphabeticPresentationForms}", "Alphabetic presentation forms characters" },
      {@"\p{IsArabic}", "Arabic characters" },
      {@"\p{IsArabicPresentationForms-A}", "Arabic presentation forms-A characters" },
      {@"\p{IsArabicPresentationForms-B}", "Arabic presentation forms-B characters" },
      {@"\p{IsArabicSupplement}", "Arabic supplement characters" },
      {@"\p{IsArmenian}", "Armenian characters" },
      {@"\p{IsArrows}", "arrows characters" },
      {@"\p{IsBalinese}", "Balinese characters" },
      {@"\p{IsBamum}", "Bamum characters" },
      {@"\p{IsBasicLatin}", "basic Latin characters" },
      {@"\p{IsBatak}", "Batak characters" },
      {@"\p{IsBengali}", "Bengali characters" },
      {@"\p{IsBlockElements}", "Block Elements characters" },
      {@"\p{IsBopomofo}", "Bopomofo characters" },
      {@"\p{IsBopomofoExtended}", "Bopomofo extended characters" },
      {@"\p{IsBoxDrawing}", "box drawing characters" },
      {@"\p{IsBraillePatterns}", "Braille patterns characters" },
      {@"\p{IsBuginese}", "Buginese characters" },
      {@"\p{IsBuhid}", "Buhid characters" },
      {@"\p{IsCham}", "Cham characters" },
      {@"\p{IsCherokee}", "Cherokee characters" },
      {@"\p{IsCJKCompatibility}", "CJK compatibility characters" },
      {@"\p{IsCJKCompatibilityForms}", "CJK compatibility forms characters" },
      {@"\p{IsCJKCompatibilityIdeographs}", "CJK compatibility ideographs characters" },
      {@"\p{IsCJKRadicalsSupplement}", "CJK radicals supplement characters" },
      {@"\p{IsCJKStrokes}", "CJK Strokes characters" },
      {@"\p{IsCJKSymbolsandPunctuation}", "CJK symbols and punctuation characters" },
      {@"\p{IsCJKUnifiedIdeographs}", "CJK Unified Ideographs characters" },
      {@"\p{IsCJKUnifiedIdeographsExtensionA}", "CJK Unified deographs Extension A characters" },
      {@"\p{IsCombiningDiacriticalMarks}", "Combining Diacritical marks characters" },
      {@"\p{IsCombiningDiacriticalMarksforSymbols}", "Combining Diacritical marks for symbols characters" },
      {@"\p{IsCombiningDiacriticalMarksSupplement}", "Combining Diacritical marks supplement characters" },
      {@"\p{IsCombiningHalfMarks}", "Combining Half marks characters" },
      {@"\p{IsCombiningMarksforSymbols}", "Combining marks for symbols characters" },
      {@"\p{IsCommonIndicNumberForms}", "Common Indic Number forms characters" },
      {@"\p{IsControlPictures}", "Control Pictures characters" },
      {@"\p{IsCoptic}", "Coptic characters" },
      {@"\p{IsCurrencySymbols}", "Currency symbols characters" },
      {@"\p{IsCyrillic}", "Cyrillic characters" },
      {@"\p{IsCyrillicExtended-A}", "Cyrillic extended-A characters" },
      {@"\p{IsCyrillicExtended-B}", "Cyrillic extended-B characters" },
      {@"\p{IsCyrillicSupplement}", "Cyrillic supplement characters" },
      {@"\p{IsCyrillicSupplementary}", "Cyrillic Supplementary characters" },
      {@"\p{IsDevanagari}", "Devanagari characters" },
      {@"\p{IsDevanagariExtended}", "Devanagari extended characters" },
      {@"\p{IsDingbats}", "Dingbats characters" },
      {@"\p{IsEnclosedAlphanumerics}", "Enclosed Alphanumerics characters" },
      {@"\p{IsEnclosedCJKLettersandMonths}", "Enclosed CJK letters and months characters" },
      {@"\p{IsEthiopic}", "Ethiopic characters" },
      {@"\p{IsEthiopicExtended}", "Ethiopic extended characters" },
      {@"\p{IsEthiopicExtended-A}", "Ethiopic extended-A characters" },
      {@"\p{IsEthiopicSupplement}", "Ethiopic supplement characters" },
      {@"\p{IsGeneralPunctuation}", "General punctuation characters" },
      {@"\p{IsGeometricShapes}", "Geometric Shapes characters" },
      {@"\p{IsGeorgian}", "Georgian characters" },
      {@"\p{IsGeorgianSupplement}", "Georgian supplement characters" },
      {@"\p{IsGlagolitic}", "Glagolitic characters" },
      {@"\p{IsGreek}", "Greek characters" },
      {@"\p{IsGreekandCoptic}", "Greek and Coptic characters" },
      {@"\p{IsGreekExtended}", "Greek extended characters" },
      {@"\p{IsGujarati}", "Gujarati characters" },
      {@"\p{IsGurmukhi}", "Gurmukhi characters" },
      {@"\p{IsHalfwidthandFullwidthForms}", "Half width and Full width forms characters" },
      {@"\p{IsHangulCompatibilityJamo}", "Hangul Compatibility Jamo characters" },
      {@"\p{IsHangulJamo}", "Hangul Jamo characters" },
      {@"\p{IsHangulJamoExtended-A}", "Hangul Jamo extended-A characters" },
      {@"\p{IsHangulJamoExtended-B}", "Hangul Jamo extended-B characters" },
      {@"\p{IsHangulSyllables}", "Hangul Syllables characters" },
      {@"\p{IsHanunoo}", "Hanunoo characters" },
      {@"\p{IsHebrew}", "Hebrew characters" },
      {@"\p{IsHighPrivateUseSurrogates}", "High Private Use Surrogates characters" },
      {@"\p{IsHighSurrogates}", "High Surrogates characters" },
      {@"\p{IsHiragana}", "Hiragana characters" },
      {@"\p{IsIdeographicDescriptionCharacters}", "Ideographic Description Characters characters" },
      {@"\p{IsIPAExtensions}", "IPA extensions characters" },
      {@"\p{IsJavanese}", "Javanese characters" },
      {@"\p{IsKanbun}", "Kanbun characters" },
      {@"\p{IsKangxiRadicals}", "Kangxi Radicals characters" },
      {@"\p{IsKannada}", "Kannada characters" },
      {@"\p{IsKatakana}", "Katakana characters" },
      {@"\p{IsKatakanaPhoneticExtensions}", "Katakana Phonetic extensions characters" },
      {@"\p{IsKayahLi}", "KayahLi characters" },
      {@"\p{IsKhmer}", "Khmer characters" },
      {@"\p{IsKhmerSymbols}", "Khmer symbols characters" },
      {@"\p{IsLao}", "Lao characters" },
      {@"\p{IsLatin-1Supplement}", "Latin-1 supplement characters" },
      {@"\p{IsLatinExtended-A}", "Latin extended-A characters" },
      {@"\p{IsLatinExtendedAdditional}", "Latin extended additional characters" },
      {@"\p{IsLatinExtended-B}", "Latin extended-B characters" },
      {@"\p{IsLatinExtended-C}", "Latin extended-C characters" },
      {@"\p{IsLatinExtended-D}", "Latin extended-D characters" },
      {@"\p{IsLepcha}", "Lepcha characters" },
      {@"\p{IsLetterlikeSymbols}", "Letterlike symbols characters" },
      {@"\p{IsLimbu}", "Limbu characters" },
      {@"\p{IsLisu}", "Lisu characters" },
      {@"\p{IsLowSurrogates}", "Low Surrogates characters" },
      {@"\p{IsMalayalam}", "Malayalam characters" },
      {@"\p{IsMandaic}", "Mandaic characters" },
      {@"\p{IsMathematicalOperators}", "mathematical operators characters" },
      {@"\p{IsMeeteiMayek}", "Meetei Mayek characters" },
      {@"\p{IsMiscellaneousMathematicalSymbols-A}", "miscellaneous mathematical symbols-A characters" },
      {@"\p{IsMiscellaneousMathematicalSymbols-B}", "miscellaneous mathematical symbols-B characters" },
      {@"\p{IsMiscellaneousSymbols}", "miscellaneous symbols characters" },
      {@"\p{IsMiscellaneousSymbolsandArrows}", "miscellaneous symbols and arrows characters" },
      {@"\p{IsMiscellaneousTechnical}", "miscellaneous Technical characters" },
      {@"\p{IsModifierToneLetters}", "modifier tone letters characters" },
      {@"\p{IsMongolian}", "Mongolian characters" },
      {@"\p{IsMyanmar}", "Myanmar characters" },
      {@"\p{IsMyanmarExtended-A}", "Myanmar extended-A characters" },
      {@"\p{IsNewTaiLue}", "New Ta iLue characters" },
      {@"\p{IsNKo}", "NKo characters" },
      {@"\p{IsNumberForms}", "number forms characters" },
      {@"\p{IsOgham}", "Ogham characters" },
      {@"\p{IsOlChiki}", "OlChiki characters" },
      {@"\p{IsOpticalCharacterRecognition}", "optical character recognition characters" },
      {@"\p{IsOriya}", "Oriya characters" },
      {@"\p{IsPhags-pa}", "Phags-pa characters" },
      {@"\p{IsPhoneticExtensions}", "phonetic extensions characters" },
      {@"\p{IsPhoneticExtensionsSupplement}", "phonetic extensions supplement characters" },
      {@"\p{IsPrivateUse}", "private use characters" },
      {@"\p{IsPrivateUseArea}", "private use Area characters" },
      {@"\p{IsRejang}", "Rejang characters" },
      {@"\p{IsRunic}", "runic characters" },
      {@"\p{IsSamaritan}", "Samaritan characters" },
      {@"\p{IsSaurashtra}", "Saurashtra characters" },
      {@"\p{IsSinhala}", "Sinhala characters" },
      {@"\p{IsSmallFormVariants}", "small form variants characters" },
      {@"\p{IsSpacingModifierLetters}", "spacing modifier letters characters" },
      {@"\p{IsSpecials}", "Specials characters" },
      {@"\p{IsSundanese}", "Sundanese characters" },
      {@"\p{IsSuperscriptsandSubscripts}", "superscripts and subscripts characters" },
      {@"\p{IsSupplementalArrows-A}", "supplemental arrows-A characters" },
      {@"\p{IsSupplementalArrows-B}", "supplemental arrows-B characters" },
      {@"\p{IsSupplementalMathematicalOperators}", "supplemental mathematical operators characters" },
      {@"\p{IsSupplementalPunctuation}", "supplemental punctuation characters" },
      {@"\p{IsSylotiNagri}", "Syloti Nagri characters" },
      {@"\p{IsSyriac}", "Syriac characters" },
      {@"\p{IsTagalog}", "Tagalog characters" },
      {@"\p{IsTagbanwa}", "Tagbanwa characters" },
      {@"\p{IsTaiLe}", "TaiLe characters" },
      {@"\p{IsTaiTham}", "TaiTham characters" },
      {@"\p{IsTaiViet}", "TaiViet characters" },
      {@"\p{IsTamil}", "Tamil characters" },
      {@"\p{IsTelugu}", "Telugu characters" },
      {@"\p{IsThaana}", "Thaana characters" },
      {@"\p{IsThai}", "Thai characters" },
      {@"\p{IsTibetan}", "Tibetan characters" },
      {@"\p{IsTifinagh}", "Tifinagh characters" },
      {@"\p{IsUnifiedCanadianAboriginalSyllabics}", "unified Canadian Aboriginal syllabics characters" },
      {@"\p{IsUnifiedCanadianAboriginalSyllabicsExtended}", "unified Canadian Aboriginal syllabics extended characters" },
      {@"\p{IsVai}", "Vai characters" },
      {@"\p{IsVariationSelectors}", "variation selectors characters" },
      {@"\p{IsVedicExtensions}", "Vedic extensions characters" },
      {@"\p{IsVerticalForms}", "Vertica forms characters" },
      {@"\p{IsYijingHexagramSymbols}", "Yijing hexagram symbols characters" },
      {@"\p{IsYiRadicals}", "Yi radicals characters" },
      {@"\p{IsYiSyllables}", "Yi syllables characters" },
    };

    static readonly Dictionary<string, string> UnicodeSeqNames = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
    {
      {@"\u007C", "vertical bar" },
    };

    static readonly Dictionary<string, string> SpecialCases = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
    {
      {@"(.+)", "Match any character one or more times" },
      {@"\r?\n", "Match zero or one occurrence of a carriage return character followed by a new line character" },
      //{@"[ae]", "either an \"a\" or an \"e\""},
      {@"[aeiou]", "all vowels" },
      {@"[^aeiou]", "Match all characters except vowels" },
      {@"[.?!;:]", "one of five punctuation marks, including a period" },
    };

    static readonly List<RegExTag> FollowedTags = new List<RegExTag>
    {
      RegExTag.LiteralChar,
    };

    const string Vowels = "aeiouy";

    public static PatternItems GeneratePatternItems(this RegExItems itemList)
    {
      PatternItems result = new PatternItems();
      int itemIndex = 0;
      int groupNumber = 0;
      while (itemIndex < itemList.Count)
      {
        var patternItem = CreatePatternItemWithGroupName(itemList, ref itemIndex, false, false, ref groupNumber);
        result.Add(patternItem);
      }
      return result;
    }

    static PatternItem CreatePatternItemWithGroupName(IList<RegExItem> itemList, ref int itemIndex, bool inCharset, bool inGroup, ref int groupNumber)
    {
      var curItem = itemList[itemIndex];

      PatternItem result = CreatePatternItemSentence(itemList, ref itemIndex, inCharset, inGroup);
      if (curItem.Tag == RegExTag.Subexpression)
      {
        result.Description += GroupNumberAppendix(ref groupNumber);
        if (curItem.Items?.Where(item => item is RegExGroup).FirstOrDefault() != null)
        {
          result.Description += GroupNumberAdditionalAppendix(curItem.Items, ref groupNumber);
        }
      }
      return result;
    }

    static string GroupNumberAppendix(ref int groupNumber)
    {
      groupNumber++;
      if (!OrdinalNames.TryGetValue(groupNumber, out string ordinalName))
        throw new NotImplementedException($"CreatePatternItem error: Ordinal name for a \"{groupNumber}\" group number not found");
      var description = $" This is the {ordinalName} capturing group.";
      return description;
    }

    static string GroupNumberAdditionalAppendix(IList<RegExItem> itemList, ref int groupNumber)
    {
      List<string> strings = GroupNumberstring(itemList, ref groupNumber);
      var description = String.Join(" and ", strings);
      description = $" This expression also defines a {description} capturing group.";
      return description;
    }

    static List<string> GroupNumberstring(IList<RegExItem> itemList, ref int groupNumber)
    {
      List<string> strings = new List<string>();
      foreach (var item in itemList)
      {
        if (item.Tag == RegExTag.Subexpression)
        {
          groupNumber++;
          if (!OrdinalNames.TryGetValue(groupNumber, out string ordinalName))
            throw new NotImplementedException($"CreatePatternItem error: Ordinal name for a \"{groupNumber}\" group number not found");
          strings.Add(ordinalName);
          strings.AddRange(GroupNumberstring(item.Items, ref groupNumber));
        }
      }
      return strings;
    }

    static PatternItem CreatePatternItemSentence(IList<RegExItem> itemList, ref int itemIndex, bool inCharset, bool inGroup)
    {
      var curItem = itemList[itemIndex];

      PatternItem result = CreatePatternItemSpecialCase(itemList, ref itemIndex, inCharset, inGroup);
      if (result == null)
      {
        result = CreatePatternItem(itemList, ref itemIndex, inCharset, inGroup);
        if (itemIndex < itemList.Count && FollowedTags.Contains(curItem.Tag))
        {
          var nextItem = itemList[itemIndex];
          if (nextItem.Tag == RegExTag.CharClass)
          {
            if (curItem.Tag != RegExTag.LiteralChar)
              result.Description = ChangeAdArticleToThe(result.Description);
            result.Description += " followed by " + CreatePatternItem(itemList, ref itemIndex, inCharset, inGroup).Description;
            result.Str += nextItem.Str;
            curItem = nextItem;
          }
        }
      }
      if (curItem.Tag == RegExTag.AnchorControl && itemIndex == 1)
      {
        if (result.Description.StartsWith("where"))
          result.Description = "Start " + result.Description;
        else
          result.Description = "Start at " + result.Description;
      }
      else
      if (curItem.Tag == RegExTag.AnchorControl && itemIndex == itemList.Count)
        result.Description = "End at " + result.Description;
      else
      if (!Char.IsUpper(result.Description.FirstOrDefault()))
        result.Description = "Match " + result.Description;
      result.Description += ".";
      return result;
    }

    static PatternItem CreatePatternItemSpecialCase(IList<RegExItem> itemList, ref int itemIndex, bool inCharset, bool inGroup)
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
            PatternItem result = new PatternItem { Str = str, Description = description };
            itemIndex = j + 1;
            return result;
          }
        }
      }
      return null;
    }

    static PatternItem CreatePatternItem(IList<RegExItem> itemList, ref int itemIndex, bool inCharset, bool inGroup, bool underQuantifier = false)
    {
      var curItem = itemList[itemIndex];
      var result = CreateSpecificPatternItem(itemList, ref itemIndex, inCharset, inGroup);
      var nextItem = (itemIndex < itemList.Count) ? itemList[itemIndex] : null;
      if (nextItem?.Tag == RegExTag.Quantifier)
      {
        var quantifierItem = CreateQuantifierPatternItem(itemList, ref itemIndex, inCharset, inGroup, underQuantifier);
        result.Str += quantifierItem.Str;
        var quantifierDescription = quantifierItem.Description;
        var itemDescription = result.Description;
        if (quantifierDescription.Contains("{0}"))
        {

          if (itemDescription.StartsWith("a "))
            itemDescription = itemDescription.Substring(2);
          else
          if (itemDescription.StartsWith("an "))
            itemDescription = itemDescription.Substring(3);
          if (quantifierDescription.Contains("more"))
            if (!itemDescription.EndsWith("s") && !itemDescription.EndsWith("es"))
              itemDescription = itemDescription + "s";
          result.Description = String.Format(quantifierDescription, itemDescription);
        }
        else
          result.Description = itemDescription + " " + quantifierDescription;
        nextItem = (itemIndex < itemList.Count) ? itemList[itemIndex] : null;
      }
      if (nextItem?.Tag == RegExTag.AltChar)
      {
        result.Str += nextItem.Str;
        itemIndex++;
        if (itemIndex < itemList.Count)
        {
          //var altItem = itemList[itemIndex];
          var result2 = CreatePatternItem(itemList, ref itemIndex, inCharset, inGroup);
          result.Str += result2.Str;
          result.Description = $"either {result.Description} or {result2.Description}";
        }
      }
      return result;
    }


    static PatternItem CreateQuantifierPatternItem(IList<RegExItem> itemList, ref int itemIndex, bool inCharset, bool inGroup, bool underQuantifier)
    {
      var quantifier = itemList[itemIndex] as RegExQuantifier;
      if (itemIndex == 0)
        throw new InvalidOperationException($"CreatePatternItem error: quantifier {quantifier.Str} must not be the first item");
      var priorItem = itemList[itemIndex - 1];
      PatternItem result = new PatternItem { Str = quantifier.Str, Description = "" };
      if (priorItem.Str == "." && quantifier.Str == "*")
      {
        result.Description += " until";
        itemIndex++;
        var nextResult = CreatePatternItem(itemList, ref itemIndex, inCharset, inGroup);
        result.Str += nextResult.Str;
        result.Description += " " + nextResult.Description;
        itemIndex--;
      }
      else
      if (priorItem.Tag == RegExTag.Subexpression && !underQuantifier)
      {
        if (!QuantifierSuffixDescriptions.TryGetValue(quantifier.Str, out string quantifierDescription))
          if (!GetNumeralQuantifierSuffix(quantifier, out quantifierDescription, false))
            throw new NotImplementedException($"CreatePatternItem error: quantifier description for a \"{quantifier.Str}\" not found");
        result.Description = quantifierDescription;
      }
      else
      {
        if (priorItem.Tag == RegExTag.CharClass || priorItem.Tag == RegExTag.UnicodeCategorySeq || priorItem.Tag == RegExTag.Subexpression)
        {
          if (!QuantifierShortDescriptions.TryGetValue(quantifier.Str, out string quantifierDescription))
            if (!GetNumeralQuantifierSuffix(quantifier, out quantifierDescription, true))
              throw new NotImplementedException($"CreatePatternItem error: quantifier description for a \"{quantifier.Str}\" not found");
          result.Description = quantifierDescription;
        }
        else
        {
          if (!QuantifierLongDescriptions.TryGetValue(quantifier.Str, out string quantifierDescription))
            if (!GetNumeralQuantifierSuffix(quantifier, out quantifierDescription, false))
              throw new NotImplementedException($"CreatePatternItem error: quantifier description for a \"{quantifier.Str}\" not found");
          result.Description = quantifierDescription;
        }
      }
      itemIndex++;
      return result;
    }

    static bool GetNumeralQuantifierSuffix(RegExQuantifier quantifier, out string description, bool shorten)
    {
      description = null;
      if (quantifier.Str.StartsWith("{"))
      {
        List<string> strings = new List<string>();
        if (quantifier.LowLimit == quantifier.HighLimit)
        {
          if (quantifier.LowLimit != null)
          {
            if (shorten)
            {
              description = GetNumeral(quantifier.LowLimit.Str)+" {0}";
              if (int.TryParse(quantifier.LowLimit.Str, out var n) && n > 1)
                description += "s";
              return true;
            }
            else
              strings.Add($"exactly {GetNumeral(quantifier.LowLimit.Str)}");
          }
        }
        else
        {
          if (quantifier.LowLimit != null)
            strings.Add($"at least {GetNumeral(quantifier.LowLimit.Str)}");
          if (quantifier.HighLimit != null)
            strings.Add($"at most {GetNumeral(quantifier.HighLimit.Str)}");
        }
        description = String.Join(" and ", strings) + " times";
        return true;
      }
      return false;

    }

    static string GetNumeral(string str)
    {
      if (int.TryParse(str, out var n))
        if (NumeralNames.TryGetValue(n, out var numeral))
          return numeral;
      return str;
    }

    static PatternItem CreateSpecificPatternItem(IList<RegExItem> itemList, ref int itemIndex, bool inCharset, bool inGroup)
    {
      var curItem = itemList[itemIndex];
      if (curItem.Tag == RegExTag.CharSet)
      {
        return CreateCharsetPatternItem(itemList, ref itemIndex, inCharset, inGroup);
      }
      else
      if (curItem.Tag == RegExTag.CharRange)
      {
        if (!inCharset)
          throw new InvalidOperationException($"CreatePatternItem error: CharRange must occur in charset");
        return CreateCharRangePatternItem(itemList, ref itemIndex, inGroup);
      }
      if (curItem.Tag == RegExTag.Subexpression)
      {
        if (inCharset)
          throw new InvalidOperationException($"CreatePatternItem error: Subexpression must not occur in charset");
        return CreateSubexpressionPatternItem(itemList, ref itemIndex, inGroup);
      }

      itemIndex++;
      PatternItem result = new PatternItem { Str = curItem.Str, Description = "" };
      switch (curItem.Tag)
      {
        case RegExTag.LiteralChar:
          if (inCharset)
            result.Description = AddAnArticle($"\"{curItem.Str}\"");
          else
          {
            if (CharNames.TryGetValue(curItem.Str, out var char1Name))
              result.Description = AddAnArticle(char1Name);
            else
              result.Description = $"a literal character \"{curItem.Str}\"";
          }
          break;

        case RegExTag.LiteralString:
          result.Description = $"the literal characters \"{curItem.Str}\"";
          break;

        case RegExTag.EscapedChar:
          if (!EscapeSeqNames.TryGetValue(curItem.Str, out string charName))
            throw new NotImplementedException($"CreatePatternItem error: Char name for a \"{curItem.Str}\" not found");
          result.Description = AddAnArticle(charName) + " character";
          break;

        case RegExTag.AnchorControl:
          if (!AnchorDescriptions.TryGetValue(curItem.Str, out string anchorDescription))
            throw new NotImplementedException($"CreatePatternItem error: Anchor description for a \"{curItem.Str}\" not found");
          result.Description = AddAnArticle(anchorDescription);
          break;

        case RegExTag.DotChar:
          result.Description = $"any character";
          break;

        case RegExTag.UnicodeSeq:
          if (!UnicodeSeqNames.TryGetValue(curItem.Str, out string unicodeName))
            throw new NotImplementedException($"CreatePatternItem error: Unicode char name for a \"{curItem.Str}\" not found");
          result.Description = AddAnArticle(unicodeName) + " character";
          break;

        case RegExTag.CharClass:
          if (!CharClassDescriptions.TryGetValue(curItem.Str, out string className))
            throw new NotImplementedException($"CreatePatternItem error: Char class name for a \"{curItem.Str}\" not found");
          if (inCharset)
            result.Description = $"all {className}s";
          else
            result.Description = AddAnArticle(className);
          break;

        case RegExTag.UnicodeCategorySeq:
          if (!UnicodeCategoryNames.TryGetValue(curItem.Str, out string categoryName))
            throw new NotImplementedException($"CreatePatternItem error: Unicode category name for a \"{curItem.Str}\" not found");
          if (inCharset)
            result.Description = $"all {categoryName}";
          else
          {
            if (categoryName.EndsWith("s"))
              categoryName = categoryName.Substring(0, categoryName.Length - 1);
            result.Description = AddAnArticle(categoryName);
          }
          break;

        case RegExTag.Quantifier:
          throw new InvalidOperationException($"CreatePatternItem error: Quantifier must not be textualized here");

        case RegExTag.CharSet:
          throw new InvalidOperationException($"CreatePatternItem error: Charset must not be textualized here");

        case RegExTag.CharRange:
          throw new InvalidOperationException($"CreatePatternItem error: CharRange must not be textualized here");

        case RegExTag.CharSetControlChar:
          if (curItem.Str == "^")
            result.Description += "all characters except";
          else
          if (curItem.Str == "-")
            result.Description += "except for";
          else
            throw new InvalidOperationException($"CreatePatternItem error: CharSetControlChar \"{curItem.Str}\" must not be textualized here");
          break;

        case RegExTag.Subexpression:
          throw new InvalidOperationException($"CreatePatternItem error: Subexpression must not be textualized here");

        case RegExTag.AltChar:
          throw new InvalidOperationException($"CreatePatternItem error: AltChar must not be textualized here");

        case RegExTag.Number:
          throw new InvalidOperationException($"CreatePatternItem error: Number must not be textualized here");

        default:
          throw new NotImplementedException($"CreatePatternItem for a {curItem.Tag} not implemented");
      }
      return result;
    }


    static PatternItem CreateCharsetPatternItem(IList<RegExItem> itemList, ref int itemIndex, bool inCharset, bool inGroup)
    {
      var curItem = itemList[itemIndex];
      itemIndex++;
      PatternItem result = new PatternItem { Str = curItem.Str, Description = "" };
      if (SpecialCases.TryGetValue(curItem.Str, out var specialCaseDescription))
      {
        result.Description = specialCaseDescription;
      }
      else
      {
        var subStrings = new List<string>();
        int subIndex = 0;
        bool hasAll = false;
        while (subIndex < curItem.Items.Count)
        {
          var subItem = CreateSpecificPatternItem(curItem.Items, ref subIndex, true, inGroup);
          if (subItem.Description.StartsWith("all "))
          {
            if (hasAll)
              subItem.Description = subItem.Description.Substring(4);
            hasAll = true;
          }
          var thisSubstring = subItem.Description;
          var lastSubstring = subStrings.LastOrDefault() ?? "";
          if (thisSubstring.StartsWith("except") || lastSubstring.EndsWith("except") || lastSubstring.EndsWith("except for"))
          {
            lastSubstring += " " + subItem.Description;
            subStrings[subStrings.Count - 1] = lastSubstring;
          }
          else
            subStrings.Add(subItem.Description);
        }
        if (hasAll)
          result.Description = String.Join(" and ", subStrings);
        else
        {
          result.Description = String.Join(" or ", subStrings);
          if (curItem.Items.Count == 2/* && curItem.SubItems[0].Tag == RegExTag.LiteralChar && curItem.SubItems[2].Tag == RegExTag.LiteralChar*/)
            result.Description = "either " + result.Description;
        }
      }
      return result;
    }

    static PatternItem CreateCharRangePatternItem(IList<RegExItem> itemList, ref int itemIndex, bool inGroup)
    {
      var curItem = itemList[itemIndex] as RegExCharRange;
      itemIndex++;
      PatternItem result = new PatternItem { Str = curItem.Str, Description = "" };
      if (SpecialCases.TryGetValue(curItem.Str, out var specialCaseDescription))
      {
        result.Description = specialCaseDescription;
      }
      else
      {
        var subStrings = new List<string>();
        var ch1 = (curItem.Items.Count > 0) ? curItem.Items[0].Str.FirstOrDefault() : '\0';
        var ch2 = (curItem.Items.Count > 2) ? curItem.Items[2].Str.FirstOrDefault() : '\0';
        if (Char.IsUpper(ch1) && Char.IsUpper(ch2))
          subStrings.Add("uppercase");
        else
        if (Char.IsLower(ch1) && Char.IsLower(ch2))
          subStrings.Add("lowercase");
        subStrings.Add("character");
        subStrings.Add("from");
        if (Char.IsUpper(ch1) && !Char.IsUpper(ch2))
          subStrings.Add("uppercase");
        else
        if (Char.IsLower(ch1) && Char.IsUpper(ch2))
          subStrings.Add("lowercase");
        subStrings.Add($"\"{ch1}\"");
        subStrings.Add("to");
        if (Char.IsUpper(ch2) && !Char.IsUpper(ch1))
          subStrings.Add("uppercase");
        else
        if (Char.IsLower(ch2) && Char.IsUpper(ch1))
          subStrings.Add("lowercase");
        subStrings.Add($"\"{ch2}\"");
        result.Description = String.Join(" ", subStrings);
        result.Description = "any " + result.Description;
      }
      return result;
    }

    static PatternItem CreateSubexpressionPatternItem(IList<RegExItem> itemList, ref int itemIndex, bool inGroup)
    {
      var curItem = itemList[itemIndex];
      itemIndex++;
      PatternItem result = new PatternItem { Str = curItem.Str, Description = "" };
      int subIndex;
      if (SpecialCases.TryGetValue(curItem.Str, out var specialCaseDescription))
      {
        result.Description = specialCaseDescription;
      }
      else
      {
        var nextItem = (itemIndex < itemList.Count) ? itemList[itemIndex] : null;
        var underQuantifier = (nextItem?.Tag == RegExTag.Quantifier);
        var subStrings = new List<string>();
        subIndex = 0;
        bool isCompound = false;
        while (subIndex < curItem.Items.Count)
        {
          if (curItem.Items[subIndex] is RegExGroup)
            isCompound = true;
          subStrings.Add(CreatePatternItem(curItem.Items, ref subIndex, false, true, underQuantifier).Description);
        }
        if (subStrings.Count<=2)
          result.Description = String.Join(" followed by ", subStrings);
        else
        {
          result.Description = String.Join(", followed by ", subStrings);
          var commaIndex = result.Description.IndexOf(',');
          if (commaIndex > 0)
            result.Description = result.Description.Remove(commaIndex, 1);
        }
        if (isCompound && !inGroup)
          result.Description = "the pattern of " + result.Description;
      }
      return result;
    }

    static string AddAnArticle(string description)
    {
      string str;
      var firstChar = description.Where(ch => Char.IsLetter(ch)).FirstOrDefault();
      if (Char.IsUpper(firstChar) || description.StartsWith("the"))
        str = description;
      else
      {
        if (!description.StartsWith("where"))
          str = $"{(Vowels.Contains(description.FirstOrDefault()) ? "an" : "a")} {description}";
        else
          str = description;
      }
      return str;
    }

    static string ChangeAdArticleToThe(string description)
    {
      if (description.StartsWith("a "))
        description = "the " + description.Substring(2);
      else
      if (description.StartsWith("an "))
        description = "the " + description.Substring(3);
      return description;
    }
  }
}
