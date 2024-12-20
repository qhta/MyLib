﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qhta.RegularExpressions.Descriptions
{
  public class PatternItemsGenerator
  {
    static readonly Dictionary<string, string> CharNames = new Dictionary<string, string>
    {
      {@" ", "space" },
      {@"!", "exclamation mark" },
      {"\"", "double quote" },
      {@"#", "hash sign" },
      {@"$", "dollar sign" },
      {@"%", "percent sign" },
      {@"&", "ampersand" },
      {@"'", "single quote" },
      {@"(", "opening parenthesis" },
      {@")", "closing parenthesis" },
      {@"*", "asterisk" },
      {@"+", "plus sign" },
      {@",", "comma" },
      {@"-", "hyphen" },
      {@".", "dot"},
      {@"/", "slash" },
      {@":", "colon" },
      {@";", "semicolon" },
      {@"<", "left angle bracket" },
      {@"=", "equal sign" },
      {@">", "right angle bracket" },
      {@"?", "question mark" },
      {@"@", "\"at\" sign" },
      {@"[", "opening bracket" },
      {@"\", "backslash" },
      {@"]", "closing bracket" },
      {@"^", "caret sign" },
      {@"_", "underscore" },
      {@"`", "grave" },
      {@"{", "opening brace" },
      {@"|", "vertical bar" },
      {@"}", "closing brace" },
      {@"~", "tilde" },
    };

    static readonly Dictionary<string, string> UnicodeSeqNames = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
    {
      {@"\u007C", "vertical bar" },
    };

    static readonly Dictionary<string, string> OptionsDescriptions = new Dictionary<string, string>
    {
      {@"i", "case-insensitive" },
      {@"m", "multiline"},
      {@"s", "single line" },
      {@"n", "explicit capture" },
      {@"x", "ignoring white space in pattern" },
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
      {@"^", "the beginning of the input string" },
      {@"$", "the end of the input string" },
      {@"\b", "word boundary" },
      {@"\B", "Do not start at a word boundary" },
      {@"\A", "the beginning of the string"},
      {@"\z", "the end of the string" },
      {@"\G", "where the last match ended" },
    };

    static readonly Dictionary<string, string> QuantifierLongDescriptions = new Dictionary<string, string>
    {
      {@"?", "zero or one occurrence of {0}" },
      {@"+", "one or more occurrences of {0}" },
      {@"*", "zero or more occurrences of {0}" },
      {@"??", "zero or one occurrence of {0}" },
      {@"+?", "one or more occurrences of {0}, but as few as possible" },
      {@"*?", "zero or more occurrences of {0}, but as few as possible" },
    };

    static readonly Dictionary<string, string> QuantifierShortDescriptions = new Dictionary<string, string>
    {
      {@"?", "zero or one {0}" },
      {@"+", "one or more {0}" },
      {@"*", "zero or more {0}" },
      {@"??", "zero or one {0}" },
      {@"+?", "one or more {0}, but as few as possible" },
      {@"*?", "zero or more {0}, but as few as possible" },
    };

    static readonly Dictionary<string, string> QuantifierSuffixDescriptions = new Dictionary<string, string>
    {
      {@"?", "zero or one time" },
      {@"+", "one or more times" },
      {@"*", "zero or more times" },
      {@"??", "zero or one time" },
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
      {@"\D", "non-decimal digit" /*"any char except decimal digit"*/ },
      {@"\w", "word character" },
      {@"\W", "non-word character" },
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

    public PatternItemsGenerator(SearchOrReplace kind = SearchOrReplace.Search)
    {
      Kind = kind;
    }

    public SearchOrReplace Kind { get; set; }

    public PatternItems GeneratePatternItems(RegExItems itemList)
    {
      PatternItems result = new PatternItems();
      int itemIndex = 0;
      while (itemIndex < itemList.Count)
      {
        var patternItem = CreatePatternItemWithGroupNumber(itemList, ref itemIndex, false, false);
        result.Add(patternItem);
      }
      return result;
    }

    private PatternItem CreatePatternItemWithGroupNumber(IList<RegExItem> itemList, ref int itemIndex, bool inCharset, bool inGroup)
    {
      var curItem = itemList[itemIndex];

      PatternItem result = CreatePatternItemSentence(itemList, ref itemIndex, inCharset, inGroup);
      if (curItem is RegExGroup groupItem)
      {
        if (curItem.Tag == RegExTag.Subexpression)
        {
          result.Description += GroupNumberAppendix(groupItem);
          if (curItem.Items?.Where(item => item is RegExGroup).FirstOrDefault() != null)
          {
            result.Description += GroupNumberAdditionalAppendix(curItem.Items);
          }
        }
      }
      return result;
    }


    private string GroupNumberAppendix(RegExGroup groupItem)
    {
      var groupNumber=groupItem.GroupNumber;
      if (groupNumber != null)
      {
        if (!OrdinalNames.TryGetValue((int)groupNumber, out string ordinalName))
        {
          ordinalName = ((int)groupNumber).ToString();
          //throw new NotImplementedException($"CreatePatternItem error: Ordinal name for a \"{groupNumber}\" group number not found");
        }
        var description = $" This is the {ordinalName} capturing group.";
        return description;
      }
      return null;
    }

    private string GroupNumberAdditionalAppendix(IList<RegExItem> itemList)
    {
      List<string> strings = GroupNumberString(itemList);
      var description = String.Join(" and ", strings);
      description = AddAnArticle(description, true);
      description = $" This expression also defines {description} capturing group.";
      return description;
    }

    private List<string> GroupNumberString(IList<RegExItem> itemList)
    {
      List<string> strings = new List<string>();
      foreach (var item in itemList)
      {
        if (item is RegExGroup groupItem)
        {
          if (item.Tag == RegExTag.Subexpression)
          {
            var groupNumber = groupItem.GroupNumber;
            if (groupNumber != null)
            {
              if (!OrdinalNames.TryGetValue((int)groupNumber, out string ordinalName))
              {
                ordinalName = ((int)groupNumber).ToString();
                //throw new NotImplementedException($"CreatePatternItem error: Ordinal name for a \"{groupNumber}\" group number not found");
              }
              strings.Add(ordinalName);
              strings.AddRange(GroupNumberString(item.Items));
            }
          }
          if (item.Tag == RegExTag.NamedGroup || item.Tag == RegExTag.BalancingGroup)
          {
            strings.Add($"\"{groupItem.Name}\"");
          }
        }
      }
      return strings;
    }

    private PatternItem CreatePatternItemSentence(IList<RegExItem> itemList, ref int itemIndex, bool inCharset, bool inGroup)
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
            var followingItem = CreatePatternItem(itemList, ref itemIndex, inCharset, inGroup);
            result.Description += " followed by " + followingItem.Description;
            result.Str += followingItem.Str;
            curItem = nextItem;
          }
        }
      }
      if (!Char.IsUpper(result.Description.FirstOrDefault()))
      {
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
        {
          if (Kind == SearchOrReplace.Replace)
            result.Description = "Add " + result.Description;
          else
            result.Description = "Match " + result.Description;
        }
      }
      if (result.Description.LastOrDefault()!='.')
        result.Description += ".";
      return result;
    }

    private PatternItem CreatePatternItemSpecialCase(IList<RegExItem> itemList, ref int itemIndex, bool inCharset, bool inGroup)
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

    private PatternItem CreatePatternItem(IList<RegExItem> itemList, ref int itemIndex, bool inCharset, bool inGroup, bool underQuantifier = false)
    {
      var curItem = itemList[itemIndex];
      var result = CreateSpecificPatternItem(itemList, ref itemIndex, inCharset, inGroup);
      var nextItem = (itemIndex < itemList.Count) ? itemList[itemIndex] : null;
      if (nextItem?.Tag == RegExTag.Quantifier)
      {
        var quantifier = nextItem as RegExQuantifier;
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
          if (quantifier.IsMultiplying && ! itemDescription.Contains("captured"))
              itemDescription = Pluralize(itemDescription);
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

    private string Pluralize(string str)
    {
      if (Char.IsLetter(str.LastOrDefault()) && !str.EndsWith("s") && !str.EndsWith("es") && !str.StartsWith("any ") && !str.EndsWith("\"") 
        && !str.StartsWith("the pattern"))
        str = str + "s";
      return str;
    }

    private PatternItem CreateQuantifierPatternItem(IList<RegExItem> itemList, ref int itemIndex, bool inCharset, bool inGroup, bool underQuantifier)
    {
      var quantifier = itemList[itemIndex] as RegExQuantifier;
      PatternItem result = new PatternItem { Str = quantifier.Str, Description = "" };
      if (itemIndex > 0)
      {
        var priorItem = itemList[itemIndex - 1];
        if (priorItem.Str == "." && quantifier.Str == "*")
        {
          result.Description += "until";
          itemIndex++;
          var nextResult = CreatePatternItem(itemList, ref itemIndex, inCharset, inGroup);
          result.Str += nextResult.Str;
          result.Description += " " + nextResult.Description;
          itemIndex--;
        }
        else
        if (priorItem.Tag == RegExTag.Subexpression && !underQuantifier)
        {
          string quantifierDescription;
          if (priorItem.Items?.Where(item => item is RegExGroup).FirstOrDefault() != null)
          {
            if (!QuantifierLongDescriptions.TryGetValue(quantifier.Str, out quantifierDescription))
              if (!GetNumeralQuantifierSuffix(quantifier, out quantifierDescription, false, "times"))
              {
                quantifierDescription = quantifier.Str;
                //throw new NotImplementedException($"CreatePatternItem error: quantifier description for a \"{quantifier.Str}\" not found");
              }
          }
          else
          if (!QuantifierSuffixDescriptions.TryGetValue(quantifier.Str, out quantifierDescription))
            if (!GetNumeralQuantifierSuffix(quantifier, out quantifierDescription, false, "times"))
            {
              quantifierDescription = quantifier.Str;
              //throw new NotImplementedException($"CreatePatternItem error: quantifier description for a \"{quantifier.Str}\" not found");
            }
          result.Description = quantifierDescription;
        }
        else
        {
          if (priorItem.Tag == RegExTag.CharClass || priorItem.Tag == RegExTag.UnicodeCategorySeq
            || (priorItem.Tag == RegExTag.Subexpression && priorItem.Items.Count == 1 && (priorItem.Items[0].Tag == RegExTag.CharClass || priorItem.Items[0].Tag == RegExTag.UnicodeCategorySeq)))
          {
            if (!QuantifierShortDescriptions.TryGetValue(quantifier.Str, out string quantifierDescription))
              if (!GetNumeralQuantifierSuffix(quantifier, out quantifierDescription, true, "characters"))
              {
                quantifierDescription = quantifier.Str;
                //throw new NotImplementedException($"CreatePatternItem error: quantifier description for a \"{quantifier.Str}\" not found");
              }
            result.Description = quantifierDescription;
          }
          else
          {
            if (!QuantifierLongDescriptions.TryGetValue(quantifier.Str, out string quantifierDescription))
              if (!GetNumeralQuantifierSuffix(quantifier, out quantifierDescription, false, "times"))
              {
                quantifierDescription = quantifier.Str;
                //throw new NotImplementedException($"CreatePatternItem error: quantifier description for a \"{quantifier.Str}\" not found");
              }
            result.Description = quantifierDescription;
          }
        }
      }
      itemIndex++;
      return result;
    }

    private bool GetNumeralQuantifierSuffix(RegExQuantifier quantifier, out string description, bool shorten, string asFewItems)
    {
      description = null;
      if (quantifier.Str.StartsWith("{"))
      {
        if (quantifier.LowLimitItem == quantifier.HighLimitItem)
        {
          if (quantifier.LowLimitItem != null)
          {
            if (shorten)
            {
              description = GetNumeral(quantifier.LowLimitItem.Str)+" {0}";
              if (quantifier.IsMultiplying)
                description = Pluralize(description);
              if (quantifier.Str.LastOrDefault() == '?')
                description += $", but as few {asFewItems} as possible";
            }
            else
              if (quantifier.Str.LastOrDefault() == '?')
              description += $"{{0}}, {GetNumeral(quantifier.LowLimitItem.Str)} times, but as few {asFewItems} as possible";
            else
              description = $"exactly {GetNumeral(quantifier.LowLimitItem.Str)} times";
            return true;
          }
        }
        else
        if (quantifier.HighLimitItem==null)
        {
          if (quantifier.LowLimitItem != null)
          {
            if (shorten)
            {
              description = "at least " + GetNumeral(quantifier.LowLimitItem.Str) + " {0}";
              if (quantifier.Str.LastOrDefault() == '?')
                description += $", but as few {asFewItems} as possible";
            }
            else
              description = $"at least {GetNumeral(quantifier.LowLimitItem.Str)} times";
            return true;
          }
        }
        else
        {
          description = $"{{0}}, between {quantifier.LowLimitItem.Str} and {quantifier.HighLimitItem.Str} times";
          if (quantifier.Str.LastOrDefault() == '?')
            description += ", but as few times as possible";
          return true;
        }
      }
      return false;

    }

    private string GetNumeral(string str)
    {
      if (int.TryParse(str, out var n))
        if (NumeralNames.TryGetValue(n, out var numeral))
          return numeral;
      return str;
    }

    private PatternItem CreateSpecificPatternItem(IList<RegExItem> itemList, ref int itemIndex, bool inCharset, bool inGroup)
    {
      var curItem = itemList[itemIndex];
      if (curItem is RegExCharSet)
      {
        return CreateCharsetPatternItem(itemList, ref itemIndex, inCharset, inGroup);
      }
      else
      if (curItem is RegExCharRange)
      {
        //if (!inCharset)
        //  throw new InvalidOperationException($"CreatePatternItem error: CharRange must occur in charset");
        return CreateCharRangePatternItem(itemList, ref itemIndex, inGroup);
      }
      if (curItem is RegExGroup)
      {
        //if (inCharset)
        //  throw new InvalidOperationException($"CreatePatternItem error: Subexpression must not occur in charset");
        return CreateGroupPatternItem(itemList, ref itemIndex, inGroup);
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
          result.Description = $"a literal string \"{curItem.Str}\"";
          break;

        case RegExTag.EscapedChar:
          if (EscapeSeqNames.TryGetValue(curItem.Str, out string escName))
            result.Description = AddAnArticle(escName) + " character";
          else
          if (curItem.Str.Length == 2)
          {
            if (CharNames.TryGetValue(curItem.Str.Substring(1, 1), out string charName))
              result.Description = AddAnArticle(charName);
            else
              result.Description = $"\"{curItem.Str.Substring(1, 1)}\" character";
          }
          else
            throw new NotImplementedException($"CreatePatternItem error: Escape sequence name for a \"{curItem.Str}\" not found");
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

        case RegExTag.CharSetControlChar:
          if (curItem.Str == "^")
            result.Description += "all characters except";
          else
          if (curItem.Str == "-")
            result.Description += "except for";
          else
            throw new InvalidOperationException($"CreatePatternItem error: CharSetControlChar \"{curItem.Str}\" must not be textualized here");
          break;

        case RegExTag.BackRef:
          if (curItem is RegExBackRef backrefItem)
            result.Description = $"the string captured in the \"{backrefItem.Name}\" capturing group";
          else
          {
            string ordinalStr = curItem.Str;
            if (curItem.Str.Length > 1)
              if (int.TryParse(curItem.Str.Substring(1), out var ordinalNum))
                if (OrdinalNames.TryGetValue(ordinalNum, out var ordinalNumName))
                  ordinalStr = ordinalNumName;
            result.Description = $"the string captured in the {ordinalStr} capturing group";
          }
          break;

        case RegExTag.Quantifier:
          throw new InvalidOperationException($"CreatePatternItem error: Quantifier must not be textualized here");

        case RegExTag.CharSet:
          throw new InvalidOperationException($"CreatePatternItem error: Charset must not be textualized here");

        case RegExTag.CharRange:
          throw new InvalidOperationException($"CreatePatternItem error: CharRange must not be textualized here");

        case RegExTag.Subexpression:
          throw new InvalidOperationException($"CreatePatternItem error: Subexpression must not be textualized here");

        case RegExTag.AltChar:
          throw new InvalidOperationException($"CreatePatternItem error: AltChar must not be textualized here");

        case RegExTag.Number:
          throw new InvalidOperationException($"CreatePatternItem error: Number must not be textualized here");

        case RegExTag.Replacement:
          if (curItem is RegExBackRef backrefItem1)
            result.Description = $"the string captured by the \"{backrefItem1.Name}\" capturing group";
          else
          {
            string ordinalStr = curItem.Str;
            if (curItem.Str.Length > 1)
              if (int.TryParse(curItem.Str.Substring(1), out var ordinalNum))
                if (OrdinalNames.TryGetValue(ordinalNum, out var ordinalNumName))
                  ordinalStr = ordinalNumName;
            result.Description = $"the string captured by the {ordinalStr} capturing group";
          }
          break;
        default:
          throw new NotImplementedException($"CreatePatternItem for a {curItem.Tag} not implemented");
      }
      return result;
    }


    private PatternItem CreateCharsetPatternItem(IList<RegExItem> itemList, ref int itemIndex, bool inCharset, bool inGroup)
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

    private PatternItem CreateCharRangePatternItem(IList<RegExItem> itemList, ref int itemIndex, bool inGroup)
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

    private PatternItem CreateGroupPatternItem(IList<RegExItem> itemList, ref int itemIndex, bool inGroup)
    {
      var groupItem = itemList[itemIndex] as RegExGroup;
      itemIndex++;
      PatternItem result = new PatternItem { Str = groupItem.Str, Description = "" };
      int subIndex;
      if (SpecialCases.TryGetValue(groupItem.Str, out var specialCaseDescription))
      {
        result.Description = specialCaseDescription;
      }
      else
      {
        var nextItem = (itemIndex < itemList.Count) ? itemList[itemIndex] : null;
        var underQuantifier = (nextItem?.Tag == RegExTag.Quantifier);
        var subStrings = new List<string>();
        subIndex = 0;
        if (subIndex < groupItem.Items.Count && groupItem.Items[subIndex].Tag == RegExTag.GroupControlChar)
        {
          if (groupItem.Tag == RegExTag.NamedGroup || groupItem.Tag == RegExTag.BalancingGroup)
            subIndex += 2;
          else
          if (groupItem.Tag == RegExTag.BackRefNamedGroup)
          {
            subIndex += 2;
            inGroup = true;
          }
          else
          if (groupItem.Tag == RegExTag.AheadPositiveAssertion || groupItem.Tag == RegExTag.AheadNegativeAssertion)
            subIndex += 2;
          else
          if (groupItem.Tag == RegExTag.BehindPositiveAssertion || groupItem.Tag == RegExTag.BehindNegativeAssertion)
            subIndex += 3;
          else
          if (groupItem.Tag == RegExTag.NonBacktrackingGroup)
            subIndex += 2;
          else
          if (groupItem.Tag == RegExTag.LocalOptionsGroup)
          {
            var skipTags = new RegExTag[] { RegExTag.GroupControlChar, RegExTag.OptionSet };
            while (subIndex< groupItem.Items.Count && skipTags.Contains(groupItem.Items[subIndex].Tag))
              subIndex++;
          }
        }
        bool isCompound = false;
        while (subIndex < groupItem.Items.Count)
        {
          if (groupItem.Items[subIndex] is RegExGroup)
            isCompound = true;
          subStrings.Add(CreatePatternItem(groupItem.Items, ref subIndex, false, true, underQuantifier).Description);
        }
        var str = "";
        for (int i = 0; i < subStrings.Count; i++)
        {
          if (i>0)
          {
            if (str.Contains(',') || str.Contains("followed"))
              str += ",";
            if (str.Contains(", assign"))
              str += " and follow the match by ";
            else
              str += " followed by ";
            str += subStrings[i];
          }
          else
            str = subStrings[i];
        }
        result.Description = str;
        if (!inGroup)
        {
          if (isCompound
            || itemIndex < itemList.Count && itemList[itemIndex].Tag == RegExTag.Quantifier && subStrings.Count > 1)
              result.Description = "the pattern of " + result.Description;
        }

        if (groupItem.Tag == RegExTag.NamedGroup)
        {
          if (!inGroup)
            result.Description += $" and name this group \"{groupItem.Name}\"";
        }
        else
        if (groupItem.Tag == RegExTag.BalancingGroup)
        {
          string firstName = null;
          var nameQuote = groupItem.Items.Where(item => item is RegExNameQuote).FirstOrDefault();
          if (nameQuote != null)
          {
            var lastNameItem = nameQuote.Items.Where(item => item.Tag == RegExTag.GroupName).LastOrDefault();
            if (lastNameItem!=null)
              firstName = lastNameItem.Str;
          }
          var secondName = groupItem.Name;
          result.Description += $", assign the substring between the \"{firstName}\" group and the current group to the \"{secondName}\" group, and delete the definition of the \"Open\" group";
        }
        else
        if (groupItem.Tag == RegExTag.BackRefNamedGroup)
        {
          string groupName = null;
          var backRef = groupItem.Items.Where(item => item is RegExBackRef).FirstOrDefault();
          if (backRef != null)
          {
            var nameQuote = backRef.Items.Where(item => item is RegExNameQuote).FirstOrDefault();
            if (nameQuote != null)
            {
              var nameItem = nameQuote.Items.Where(item => item.Tag == RegExTag.GroupName).LastOrDefault();
              if (nameItem != null)
                groupName = nameItem.Str;
            }
          }
          var firstChar = result.Description.FirstOrDefault();
          if (Char.IsUpper(firstChar))
            result.Description = Char.ToLower(firstChar) + result.Description.Substring(1);
          result.Description = $"If the \"{groupName}\" group exists, " + result.Description;
        }
        else
        if (groupItem.Tag == RegExTag.AheadPositiveAssertion || groupItem.Tag == RegExTag.AheadNegativeAssertion)
        {
          bool isEmptyString = result.Description == "";
          if (isEmptyString)
            result.Description = "empty string";
          result.Description = $"Determine whether the previous match is followed by {result.Description}.";
          if (groupItem.Tag == RegExTag.AheadPositiveAssertion)
          {
            result.Description += " If so, the match was successful.";
            if (isEmptyString)
              result.Description += " Because an empty string is always implicitly present in an input string, this assertion is always true.";
          }
          else
          {
            result.Description += " If it is not, the match was successful.";
            if (isEmptyString)
              result.Description += " Because an empty string is always implicitly present in an input string, this assertion is always false.";
          }
        }
        else
        if (groupItem.Tag == RegExTag.BehindPositiveAssertion || groupItem.Tag == RegExTag.BehindNegativeAssertion)
        {
          bool isEmptyString = result.Description == "";
          if (isEmptyString)
            result.Description = "empty string";
          result.Description = $"Determine whether the next match is preceded by {result.Description}.";
          if (groupItem.Tag == RegExTag.BehindPositiveAssertion)
          {
            result.Description += " If so, the match is possible.";
            if (isEmptyString)
              result.Description += " Because an empty string is always implicitly present in an input string, this assertion is always true.";
          }
          else
          {
            result.Description += " If it is not, the match is possible.";
            if (isEmptyString)
              result.Description += " Because an empty string is always implicitly present in an input string, this assertion is always false.";
          }
        }
        else
        if (groupItem.Tag == RegExTag.NonBacktrackingGroup)
        {
          result.Description += $", but do not backtrack to the following match";
        }
        else
        if (groupItem.Tag == RegExTag.LocalOptionsGroup)
        {
          RegExOptions usingOptions = groupItem.UsingOptions;
          RegExOptions cancelOptions = groupItem.CancelOptions;
          string usingOptionsStr = (usingOptions!=null) ? CreateLocalOptionsDescription(usingOptions) : null;
          string cancelOptionsStr = (cancelOptions != null) ? CreateLocalOptionsDescription(cancelOptions) : null;
          var optionsStr = "";
          if (usingOptionsStr != null)
            optionsStr = "Using " + usingOptionsStr;
          if (cancelOptionsStr != null)
          {
            if (optionsStr != "")
              optionsStr += ", and canceling " + cancelOptionsStr;
            else
              optionsStr = "Canceling " + cancelOptionsStr;
          }
          if (result.Description!="")
            result.Description = optionsStr + ", match " + result.Description;
          else
            result.Description = optionsStr + ", perform the following matches";
        }
      }
      return result;
    }

    private string CreateLocalOptionsDescription(RegExOptions options)
    {
      var str = "";
      var lastStr = "";
      for (int i = 0; i < options.Items.Count; i++)
      {
        var optionItem = options.Items[i];
        if (optionItem.Status == RegExStatus.OK)
        {
          if (!OptionsDescriptions.TryGetValue(optionItem.Str, out var s1))
            throw new InvalidOperationException($"CreatePatternItem error: local option \"{optionItem.Str}\" description not found");
          if (optionItem.Str == "x")
            lastStr = s1;
          else
          {
            if (i >= 2)
              str += ",";
            if (i >= 1)
              str += " and ";
            str += s1;
          }
        }
      }
      if (str!="")
        str += " matching";
      if (lastStr!="")
      {
        if (str != "")
        {
          if (str.Contains("and"))
            str += ", and ";
          else
            str += " and ";
        }
        str += lastStr;
      }
      return str;
    }

    private string AddAnArticle(string description, bool ignoreCase=false)
    {
      string str;
      var firstChar = description.Where(ch => Char.IsLetter(ch)).FirstOrDefault();
      if (ignoreCase)
        firstChar = Char.ToLower(firstChar);
      if (Char.IsUpper(firstChar) || description.StartsWith("the"))
        str = description;
      else
      {
        if (!description.StartsWith("where"))
          str = $"{(Vowels.Contains(firstChar) ? "an" : "a")} {description}";
        else
          str = description;
      }
      return str;
    }

    private string ChangeAdArticleToThe(string description)
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
