using System.Collections;
using System.Collections.Generic;

namespace Qhta.RegularExpressions
{
  public class UnicodeNames : ICollection<string>
  {
    private UnicodeNames() { }

    public static UnicodeNames Instance
    {
      get
      {
        if (_Instance == null)
        {
          _Instance = new UnicodeNames();
          _Instance.InitNames();
        }
        return _Instance;
      }
    }
    private static UnicodeNames _Instance;

    private SortedSet<string> Names = new SortedSet<string>();

    private static string[] _names = new string[]
    {
			"C",
			"Cc",
			"Cf",
			"Cn",
			"Co",
			"Cs",
			"IsAlphabeticPresentationForms",
			"IsArabic",
			"IsArabicPresentationForms-A",
			"IsArabicPresentationForms-B",
			"IsArabicSupplement",
			"IsArmenian",
			"IsArrows",
			"IsBalinese",
			"IsBamum",
			"IsBasicLatin",
			"IsBatak",
			"IsBengali",
			"IsBlockElements",
			"IsBopomofo",
			"IsBopomofoExtended",
			"IsBoxDrawing",
			"IsBraillePatterns",
			"IsBuginese",
			"IsBuhid",
			"IsCham",
			"IsCherokee",
			"IsCJKCompatibility",
			"IsCJKCompatibilityForms",
			"IsCJKCompatibilityIdeographs",
			"IsCJKRadicalsSupplement",
			"IsCJKStrokes",
			"IsCJKSymbolsandPunctuation",
			"IsCJKUnifiedIdeographs",
			"IsCJKUnifiedIdeographsExtensionA",
			"IsCombiningDiacriticalMarks",
			"IsCombiningDiacriticalMarksforSymbols",
			"IsCombiningDiacriticalMarksSupplement",
			"IsCombiningHalfMarks",
			"IsCombiningMarksforSymbols",
			"IsCommonIndicNumberForms",
			"IsControlPictures",
			"IsCoptic",
			"IsCurrencySymbols",
			"IsCyrillic",
			"IsCyrillicExtended-A",
			"IsCyrillicExtended-B",
			"IsCyrillicSupplement",
			"IsCyrillicSupplementary",
			"IsDevanagari",
			"IsDevanagariExtended",
			"IsDingbats",
			"IsEnclosedAlphanumerics",
			"IsEnclosedCJKLettersandMonths",
			"IsEthiopic",
			"IsEthiopicExtended",
			"IsEthiopicExtended-A",
			"IsEthiopicSupplement",
			"IsGeneralPunctuation",
			"IsGeometricShapes",
			"IsGeorgian",
			"IsGeorgianSupplement",
			"IsGlagolitic",
			"IsGreek",
			"IsGreekandCoptic",
			"IsGreekExtended",
			"IsGujarati",
			"IsGurmukhi",
			"IsHalfwidthandFullwidthForms",
			"IsHangulCompatibilityJamo",
			"IsHangulJamo",
			"IsHangulJamoExtended-A",
			"IsHangulJamoExtended-B",
			"IsHangulSyllables",
			"IsHanunoo",
			"IsHebrew",
			"IsHighPrivateUseSurrogates",
			"IsHighSurrogates",
			"IsHiragana",
			"IsIdeographicDescriptionCharacters",
			"IsIPAExtensions",
			"IsJavanese",
			"IsKanbun",
			"IsKangxiRadicals",
			"IsKannada",
			"IsKatakana",
			"IsKatakanaPhoneticExtensions",
			"IsKayahLi",
			"IsKhmer",
			"IsKhmerSymbols",
			"IsLao",
			"IsLatin-1Supplement",
			"IsLatinExtended-A",
			"IsLatinExtendedAdditional",
			"IsLatinExtended-B",
			"IsLatinExtended-C",
			"IsLatinExtended-D",
			"IsLepcha",
			"IsLetterlikeSymbols",
			"IsLimbu",
			"IsLisu",
			"IsLowSurrogates",
			"IsMalayalam",
			"IsMandaic",
			"IsMathematicalOperators",
			"IsMeeteiMayek",
			"IsMiscellaneousMathematicalSymbols-A",
			"IsMiscellaneousMathematicalSymbols-B",
			"IsMiscellaneousSymbols",
			"IsMiscellaneousSymbolsandArrows",
			"IsMiscellaneousTechnical",
			"IsModifierToneLetters",
			"IsMongolian",
			"IsMyanmar",
			"IsMyanmarExtended-A",
			"IsNewTaiLue",
			"IsNKo",
			"IsNumberForms",
			"IsOgham",
			"IsOlChiki",
			"IsOpticalCharacterRecognition",
			"IsOriya",
			"IsPhags-pa",
			"IsPhoneticExtensions",
			"IsPhoneticExtensionsSupplement",
			"IsPrivateUse",
			"IsPrivateUseArea",
			"IsRejang",
			"IsRunic",
			"IsSamaritan",
			"IsSaurashtra",
			"IsSinhala",
			"IsSmallFormVariants",
			"IsSpacingModifierLetters",
			"IsSpecials",
			"IsSundanese",
			"IsSuperscriptsandSubscripts",
			"IsSupplementalArrows-A",
			"IsSupplementalArrows-B",
			"IsSupplementalMathematicalOperators",
			"IsSupplementalPunctuation",
			"IsSylotiNagri",
			"IsSyriac",
			"IsTagalog",
			"IsTagbanwa",
			"IsTaiLe",
			"IsTaiTham",
			"IsTaiViet",
			"IsTamil",
			"IsTelugu",
			"IsThaana",
			"IsThai",
			"IsTibetan",
			"IsTifinagh",
			"IsUnifiedCanadianAboriginalSyllabics",
			"IsUnifiedCanadianAboriginalSyllabicsExtended",
			"IsVai",
			"IsVariationSelectors",
			"IsVedicExtensions",
			"IsVerticalForms",
			"IsYijingHexagramSymbols",
			"IsYiRadicals",
			"IsYiSyllables",
			"L",
			"Ll",
			"Lm",
			"Lo",
			"Lt",
			"Lu",
			"M",
			"Mc",
			"Me",
			"Mn",
			"N",
			"Nd",
			"Nl",
			"No",
			"P",
			"Pc",
			"Pd",
			"Pe",
			"Pf",
			"Pi",
			"Po",
			"Ps",
			"S",
			"Sc",
			"Sk",
			"Sm",
			"So",
			"Z",
			"Zl",
			"Zp",
			"Zs",

	};
    private void InitNames()
    {
			foreach (var s in _names)
				Add(s);
    }
    public void Add(string item)
    {
      if (!Names.Contains(item))
        ((ICollection<string>)Names).Add(item);
    }

    public void Clear()
    {
      ((ICollection<string>)Names).Clear();
    }

    public bool Contains(string item)
    {
      return ((ICollection<string>)Names).Contains(item);
    }

    public void CopyTo(string[] array, int arrayIndex)
    {
      ((ICollection<string>)Names).CopyTo(array, arrayIndex);
    }

    public bool Remove(string item)
    {
      return ((ICollection<string>)Names).Remove(item);
    }

    public int Count => ((ICollection<string>)Names).Count;

    public bool IsReadOnly => ((ICollection<string>)Names).IsReadOnly;

    public IEnumerator<string> GetEnumerator()
    {
      return ((IEnumerable<string>)Names).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable)Names).GetEnumerator();
    }
  }
}
