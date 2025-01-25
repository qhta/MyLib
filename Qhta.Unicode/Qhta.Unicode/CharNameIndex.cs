using System.Data;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

using Qhta.TextUtils;

namespace Qhta.Unicode;

/// <summary>
/// An index of codePoints with specific char name. Char name is a single-word string.
/// </summary>
public class CharNameIndex : Dictionary<string, CodePoint>
{
  private UnicodeData Ucd = null!;

  /// <summary>
  /// Initializes index.from a UnicodeData object.
  /// </summary>
  /// <param name="ucd"></param>
  public void Initialize(UnicodeData ucd)
  {
    Ucd = ucd;
    CreateShortNamesToFile("CharNames.txt");
  }

  /// <summary>
  /// Add a code point to the this.
  /// </summary>
  /// <param name="charName">Name is a single-word string</param>
  /// <param name="codePoint"></param>
  private void Add(string charName, int codePoint)
  {
    if (this.TryGetValue(charName, out var value))
    {
      throw new DuplicateNameException($"CharName \"{charName}\" already exists");
    }
    else
    {
      base.Add(charName, codePoint);
    }
  }

  private void CreateShortNamesToFile(string filename)
  {
    foreach (CharInfo charInfo in Ucd.Values.OrderBy(item => (int)item.CodePoint))
    {
      var charName = CreateShortName(charInfo);
      if (charName is not null)
      {
        if (this.TryGetValue(charName, out var existingCodePoint))
        {
          const int maxAlternative = 2;
          for (int alternative = 1; alternative <= maxAlternative; alternative++)
          {
            var altName2 = CreateShortName(charInfo, alternative);
            if (altName2 != null && altName2 != charName)
            {
              this.Add(altName2, charInfo.CodePoint);
              break;
            }
            else
            {
              var charInfo1 = Ucd[existingCodePoint];
              var altName1 = CreateShortName(charInfo1, alternative);
              if (altName1 != null && altName1 != charName)
              {
                this.Remove(charName);
                Add(altName1, existingCodePoint);
                Add(charName, charInfo.CodePoint);
                break;
              }
              else if (alternative == maxAlternative)
              {
                if (charName.StartsWith("hangjungseong"))
                {
                  charName = charName.Insert("hangjungseong".Length, "2");
                  Add(charName, charInfo.CodePoint);
                  break;
                }
                throw new DuplicateNameException($"CharName \"{charName}\" already exists");
                charName = charName + alternative.ToString();
                Add(charName, charInfo.CodePoint);
              }
            }
          }
        }
        else
          Add(charName, charInfo.CodePoint);
      }
    }
    using (var writer = File.CreateText(filename))
    {
      foreach (var item in this.OrderBy(item => (int)item.Value))
      {
        writer.WriteLine($"{item.Value};{item.Key}");
      }
    }
  }

  /// <summary>
  /// Create a short name for a character.
  /// </summary>
  /// <param name="charInfo"></param>
  /// <param name="alternative">Some names can have alternatives</param>
  /// <returns></returns>
  public string? CreateShortName(CharInfo charInfo, int alternative = 0)
  {
    if (charInfo.CodePoint == 0x16AC)
      Debug.Assert(true);

    string longName = charInfo.Name;
    string? charName = null;
    if (charInfo.CodePoint == 0x0021)
      Debug.Assert(true);
    if (charInfo.CodePoint == 0x007F)
      charName = "DEL";
    else if (charInfo.Category == UcdCategory.Cc || charInfo.CodePoint == 0x020 || longName.StartsWith("<"))
      charName = CreateAbbreviationCharName(charInfo);
    else if (charInfo.Category.ToString()[0] == 'Z')
      charName = CreateShortenName(charInfo, alternative);
    else if (longName.StartsWith("MODIFIER LETTER "))
      charName = CreateModifierLetterName(charInfo, alternative);
    else if (longName.StartsWith("COMBINING "))
      charName = CreateCombiningCharName(charInfo, alternative);
    else if (longName.StartsWith("VULGAR FRACTION "))
      charName = CreateVulgarFractionName(charInfo, alternative);
    else if (longName.StartsWith("PRESENTATION FORM "))
      charName = CreatePresentationFormName(charInfo, alternative);
    else if (longName.StartsWith("FULLWIDTH "))
      charName = CreateFullWidthName(charInfo, alternative);
    else if (charInfo.Category.ToString()[0] == 'L')
      charName = CreateLetterName(charInfo, alternative);
    else
      charName = CreateShortenName(charInfo, alternative);
    //Debug.WriteLine($"{charInfo.CodePoint};{charName}");
    if (charName!.Length == 1)
      Debug.Assert(true);
    return charName;
  }

  private string? CreateAbbreviationCharName(CharInfo charInfo)
  {
    string? charName = null;
    if (charInfo.Aliases != null)
    {
      var alias = charInfo.Aliases.FirstOrDefault(item => item.Type == NameAliasType.Abbreviation);
      charName = alias?.Name!;
      //Debug.WriteLine($"{charInfo.CodePoint};{charName}");
      if (alias != null)
        return alias!.Name;
    }
    var longName = charInfo.Name.ToString();
    if (longName.StartsWith("<") && longName.EndsWith(">"))
      return CreateShortenName(longName.Substring(1, longName.Length - 2).ToUpper());
    return charName;
  }

  /// <summary>
  /// Create a short name for a long character name.
  /// </summary>
  /// <param name="charInfo"></param>
  /// <param name="alternative">Some names can have alternatives</param>
  /// <returns></returns>
  private string CreateShortenName(CharInfo charInfo, int alternative = 0)
  {
    if (charInfo.CodePoint == 0xFE32)
      Debug.Assert(true);
    var longName = charInfo.Name.ToString();
    longName = ReplaceStrings(longName, alternative);
    var sb = new StringBuilder();
    if (charInfo.Category.ToString() == "Sk")
      longName = longName.Replace(" AND ", " ");
    var words = longName.Split([' ', ',', '-'], StringSplitOptions.RemoveEmptyEntries).ToList();
    for (int i = words.Count - 1; i >= 0; i--)
    {
      var word = words[i];
      if (TryFindWordToRemove(word, alternative))
        words.RemoveAt(i);
    }
    if (words.Count == 1)
    {
      if (WordsToReplace.TryGetValue(words[0], out var shortWord))
        return shortWord;
      return words[0].ToLower();
    }
    for (int i = 0; i < words.Count; i++)
    {
      var word = words[i];

      if (i == 0 && TryFindScriptName(word, alternative, out var scCode))
      {
        scCode = scCode.ToLower();
        sb.Append(scCode);
        continue;
      }

      if (charInfo.Category == UcdCategory.Sc && i == 0)
      {
        sb.Append(word.ToLower());
        continue;
      }

      if (word == "EN" || word == "EM")
      {
        if (i == 0)
          sb.Append(word.ToLower());
        else
          sb.Append(char.ToLower(word[1]));
      }
      else
      if (WordsToReplace.TryGetValue(word, out var shortWord))
        sb.Append(shortWord);
      else
        //if (i < words.Count - 1)
        //  sb.Append(char.ToLower(word[0]));
        //else
        sb.Append(word.ToLower());
    }
    return sb.ToString();
  }

  private string CreateLetterName(CharInfo charInfo, int alternative = 0)
  {
    var longName = charInfo.Name.ToString();
    return CreateShortenName(longName, alternative);
  }

  private string CreateModifierLetterName(CharInfo charInfo, int alternative = 0)
  {
    var longName = charInfo.Name.ToString();
    longName = longName.Replace("MODIFIER LETTER ", "");
    return CreateShortenName(longName, alternative) + "mod";
  }

  private string CreateCombiningCharName(CharInfo charInfo, int alternative = 0)
  {
    var longName = charInfo.Name.ToString();
    longName = longName.Substring("COMBINING ".Length);
    longName = longName.Replace(" AND ", " ");
    longName = longName.Replace('-', ' ');
    return "comb" + CreateShortenName(longName, alternative);
  }

  private string CreateVulgarFractionName(CharInfo charInfo, int alternative = 0)
  {
    var longName = charInfo.Name.ToString();
    longName = longName.Replace("VULGAR FRACTION ", "");
    return CreateShortenName(longName, alternative);
  }

  private string CreatePresentationFormName(CharInfo charInfo, int alternative = 0)
  {
    var longName = charInfo.Name.ToString();
    longName = longName.Replace("PRESENTATION FORM FOR VERTICAL ", "VERTICAL");
    return CreateShortenName(longName, alternative);
  }

  private string CreateFullWidthName(CharInfo charInfo, int alternative = 0)
  {
    var longName = charInfo.Name.ToString();
    longName = longName.Replace("FULLWIDTH ", "");
    return "wide" + CreateShortenName(longName, alternative);
  }

  private string CreateShortenName(string longName, int alternative = 0)
  {
    longName = ReplaceStrings(longName, alternative);
    var sb = new StringBuilder();
    var ss = longName.Split([' ', ',', '-']);
    var wasCapital = false;
    var wasSmall = false;
    var wasWith = false;
    for (int i = 0; i < ss.Length; i++)
    {
      var word = ss[i];//.Replace('-', '_');
      if (word == "LATIN")
      {
        continue;
      }
      if (i == 0 && TryFindScriptName(word, alternative, out var scCode))
      {
        scCode = scCode.ToLower();
        sb.Append(scCode);
        continue;
      }
      if (word == "WITH")
      {
        wasWith = true;
        continue;
      }
      if (wasWith && word == "AND")
        continue;
      if (TryFindWordToRemove(word, alternative))
        continue;

      if (word == "CAPITAL_LETTER" || word == "CAPITAL_LIGATURE")
      {
        wasCapital = true;
        continue;
      }

      if (word == "SMALL_LETTER" || word == "SMALL_LIGATURE")
      {
        wasSmall = true;
        continue;
      }

      if (word == "SMALL_CAPITAL_LETTER")
      {
        wasCapital = true;
        wasSmall = true;
        continue;
      }

      if (WordsToReplace.TryGetValue(word, out var replacement) && replacement.Length > 2)
        sb.Append(replacement);
      else
      if (wasCapital)
      {
        if (wasSmall)
          sb.Append("smcap");
        if (word.Length == 2)
          sb.Append(word.ToUpper());
        else
        {
          if (alternative > 0)
            sb.Append("cap");
          sb.Append(word.TitleCase());
        }
        wasCapital = false;
      }
      else
      if (wasSmall)
      {
        if (alternative > 0)
          sb.Append("small");
        sb.Append(word.ToLower());
      }
      else
        sb.Append(word.ToLower());
      wasSmall = false;
    }
    return sb.ToString();
  }

  private static bool TryFindWordToRemove(string word, int alternative)
  {
    if (WordsToRemove.TryGetValue(word, out int val))
      if (alternative <= val)
        return true;
    return false;
  }

  private static string ReplaceStrings(string longName, int alternative)
  {
    foreach (var entry in StringsToReplace)
      longName = longName.Replace(entry.Key, entry.Value);

    return longName;
  }

  private static bool TryFindScriptName(string word, int alternative, out string scCode)
  {
    word = word.Replace('_', ' ').TitleCase(true);
    if (ScriptCodes.UcdScriptNames.TryGetValue1(word, out scCode))
    {
      if (alternative == 0)
      {
        if (scCode is "Latn" or "Grek" or "Hebr")
          scCode = "";
      }
      return true;
    }
    return false;
  }

  private static readonly Dictionary<string, string> StringsToReplace = new()
  {
    {" -"," ALT "},
    {"SQUARE BRACKET", "BRACKET"},
    {"CURLY BRACKET", "BRACE"},
    {"SMALL CAPITAL LETTER", "SMALL_CAPITAL_LETTER"},
    {"CAPITAL LETTER", "CAPITAL_LETTER"},
    {"SMALL LETTER", "SMALL_LETTER"},
    {"CAPITAL LIGATURE", "CAPITAL_LIGATURE"},
    {"SMALL LIGATURE", "SMALL_LIGATURE"},
    {"NO-BREAK", "NOBREAK"},
    {"NON-BREAKING", "NOBREAK"},
    {"THREE-PER-EM", "tpm"},
    {"FOUR-PER-EM", "fpm"},
    {"SIX-PER-EM", "spm"},
    {"TWO-EM DASH","twoemdash"},
    {"THREE-EM DASH", "triemdash"},
    {"EM DASH", "emdash"},
    {"EN DASH", "endash"},
    {"NUMBER SIGN", "hash"},
    {"REVERSE SOLIDUS", "BACKSLASH"},
    {"SOLIDUS", "SLASH"},
    {"-THAN", ""},
    {"HYPHEN-MINUS", "dash"},
    {"PRECEDED BY", "AFTER"},
    {"COMMERCIAL AT", "at"},
    {"SOFT HYPHEN", "sh"},
    {"MULTIPLICATION", "times"},
    {"RETROFLEX HOOK", "rhook"},
    {"PALATAL HOOK", "phook"},
    {"PALATALIZED HOOK", "phook"},
    {"CANADIAN SYLLABICS", "CANADIAN_ABORIGINAL"},
    {"KATAKANA-HIRAGANA", "JAPANESE"},
    {"ZERO WIDTH", "ZEROWIDTH"},
    {"NON-JOINER", "NONJOINER"},
    {"LEFT-TO-RIGHT EMBEDDING", "lre"},
    {"LEFT-TO-RIGHT OVERRIDE", "lro"},
    {"LEFT-TO-RIGHT ISOLATE", "lri"},
    {"LEFT TO RIGHT", "ltr"},
    {"RIGHT-TO-LEFT EMBEDDING", "rle"},
    {"RIGHT-TO-LEFT OVERRIDE", "rlo"},
    {"RIGHT-TO-LEFT ISOLATE", "rli"},
    {"RIGHT TO LEFT", "rtl"},
    {"FIRST STRONG ISOLATE", "fsi"},
    {"POP DIRECTIONAL FORMATTING", "pdf"},
    {"POP DIRECTIONAL ISOLATE", "pdi"},
    {"WORD JOINER", "wjn"},
    {"INHIBIT SYMMETRIC SWAPPING","iss"},
    {"ACTIVATE SYMMETRIC SWAPPING","ass"},
    {"INHIBIT ARABIC FORM SHAPING","iafs"},
    {"ACTIVATE ARABIC FORM SHAPING","aafs"},
    {"NATIONAL DIGIT SHAPES", "natds"},
    {"NOMINAL DIGIT SHAPES", "nomds"},
    {"INTERLINEAR ANNOTATION ANCHOR","iaa"},
    {"INTERLINEAR ANNOTATION SEPARATOR","ias"},
    {"INTERLINEAR ANNOTATION TERMINATOR","iat"},
    {"HEBREW LETTER", "HEBREW"},
    {"HEBREW LIGATURE", "HEBREW"},
    {"HEBREW PUNCTUATION", "HEBREW"},
    {"HEBREW POINT", "HEBREW"},
    {"VARIATION SELECTOR", "varsel"},
    {"CJK COMPATIBILITY IDEOGRAPH","cjk"},
    {"ISOLATED FORM", "ISOLATED"},
    {"FINAL FORM", "FINAL"},
    {"ARABIC LETTER","ARABIC"},
    {"CYPRIOT SYLLABLE", "CYPRIOT"},
  };

  private static readonly Dictionary<string, int> WordsToRemove = new()
  {
    {"SIGN", 1},
    {"MARK", 1},
    {"DIGIT", 1},
    {"WITH", 0 },
    {"COMMERCIAL", 0},
    {"THAN", 0},
  };

  private static readonly Dictionary<string, string> WordsToReplace = new()
  {
    {"ABBREVIATION", "abbr"},
    {"ACCENT", "acc"},
    {"ACUTE", "acute"},
    {"AMPERSAND", "amp"},
    {"ANGLE", "angle"},
    {"APOSTROPHE", "apos"},
    {"APPLICATION","app"},
    {"ARROWHEAD","ahead"},
    {"ASTERISK", "ast"},
    {"AT", "at"},
    {"BACKSLASH", "bslash"},
    {"BAR", "bar"},
    {"BRACE", "brace"},
    {"BRACKET", "bracket"},
    {"BREVE", "breve"},
    {"BULLET", "bullet"},
    {"CARET", "caret"},
    {"CARON", "caron"},
    {"CEDILLA", "cedilla"},
    {"CENTRED","cnt"},
    {"CIRCLE", "circle"},
    {"CIRCUMFLEX", "cflex"},
    {"CLOSE", "close"},
    {"COLON", "colon"},
    {"COMMA", "comma"},
    {"COPYRIGHT", "cpright"},
    {"CURLY", "curly"},
    {"DASH", "dash"},
    {"DESCENDER", "desc"},
    {"DIAERESIS", "die"},
    {"DOLLAR", "dollar"},
    {"DOT", "dot"},
    {"DOTTED","dot"},
    {"DOUBLE", "dbl"},
    {"ELLIPSIS", "ellipsis"},
    {"EM", "em"},
    {"EN", "en"},
    {"EQUALS", "eq"},
    {"EXCLAMATION", "excl"},
    {"FEMININE", "fem"},
    {"FIGURE", "fig"},
    {"FOOTNOTE", "ftn"},
    {"FRACTION", "frac"},
    {"FULL", "f"},
    {"FUNCTION","funct"},
    {"GLOTTAL", "glot"},
    {"GREATER", "gt"},
    {"GRAVE", "grave"},
    {"HAIR", "hair"},
    {"HASH", "hash"},
    {"HORIZONTAL", "horz"},
    {"HYPHEN", "hyphen"},
    {"IDEOGRAPHIC", "id"},
    {"INDICATOR", "ind"},
    {"INVERTED", "inv"},
    {"INVISIBLE","nv"},
    {"JOINER","jn"},
    {"LEFT", "l"},
    {"LESS", "lt"},
    {"LETTER", "letter"},
    {"LIGATURE", "lig"},
    {"LONG", "long"},
    {"LOW", "low"},
    {"MACRON", "macron"},
    {"MARK", "mark"},
    {"MARKER", "mark"},
    {"MASCULINE", "masc"},
    {"MATHEMATICAL", "mat"},
    {"MEDIUM", "med"},
    {"MIDDLE", "mid"},
    {"MINUS", "minus"},
    {"MODIFIED","mod"},
    {"NARROW", "nar"},
    {"NOBREAK", "nb"},
    {"NONBREAKING", "nb"},
    {"NONJOINER","njn"},
    {"NOT", "not"},
    {"NUMBER", "num"},
    {"OBLIQUE", "obl"},
    {"OGONEK", "ogonek"},
    {"OPEN", "open"},
    {"ORDINAL", "ord"},
    {"PARAGRAPH", "para"},
    {"PARENTHESIS", "paren"},
    {"PERCENT", "percent"},
    {"PLUS", "plus"},
    {"POINTING", "pt"},
    {"PRIVATE", "priv"},
    {"PUNCTUATION", "punct"},
    {"QUADRUPLE", "quad"},
    {"QUESTION", "quest"},
    {"QUINTUPLE", "quint"},
    {"QUOTATION", "quote"},
    {"QUOTE", "quote"},
    {"REGISTERED", "regrd"},
    {"REVERSED", "rev"},
    {"RIGHT", "r"},
    {"RING", "ring"},
    {"ROUND", "round"},
    {"SECTION", "sect"},
    {"SEMICOLON", "scolon"},
    {"SEPARATOR", "sep"},
    {"SHORT", "short"},
    {"SIGN", "sign"},
    {"SIX-PER-EM", "spm"},
    {"SLASH", "slash"},
    {"SPACE", "sp"},
    {"SQUARE", "sq"},
    {"STAR", "star"},
    {"STROKE", "stroke"},
    {"SUBSCRIPT", "sub"},
    {"SUPERSCRIPT", "sup"},
    {"SURROGATE", "sur"},
    {"THIN", "thin"},
    {"TILDE", "tilde"},
    {"TRIPLE", "tple"},
    {"TURNED", "turn"},
    {"VERTICAL", "vert"},
    {"VERTICALLY","vert"},
    {"WAVE", "wave"},
    {"WORD_JOINER","wjn"},
    {"UNDERSCORE", "uline"},
    {"ZEROWIDTH", "zw"},
  };

}
