﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Definition of the Unicode ranges.
/// </summary>
public enum UnicodeRange
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  Unassigned,
  BasicLatin,
  Latin1Supplement,
  LatinExtendedA,
  LatinExtendedB,
  IPAExtensions,
  SpacingModifierLetters,
  CombiningDiacriticalMarks,
  GreekAndCoptic,
  Cyrillic,
  CyrillicSupplement,
  Armenian,
  Hebrew,
  Arabic,
  Syriac,
  ArabicSupplement,
  Thaana,
  NKo,
  Samaritan,
  MAndaic,
  SyriacSupplement,
  ArabicExtendedA,
  Devanagari,
  Bengali,
  Gurmukhi,
  Gujarati,
  Oriya,
  Tamil,
  Telugu,
  Kannada,
  Malayalam,
  Sinhala,
  Thai,
  Lao,
  Tibetan,
  Myanmar,
  Georgian,
  HangulJamo,
  Ethiopic,
  EthiopicSupplement,
  Cherokee,
  UnifiedCanadianAboriginalSyllabics,
  Ogham,
  Runic,
  Tagalog,
  Hanunoo,
  Buhid,
  Tagbanwa,
  Khmer,
  Mongolian,
  UnifiedCanadianAboriginalSyllabicsExtended,
  Limbu,
  TaiLe,
  NewTaiLue,
  KhmerSymbols,
  Buginese,
  TaiTham,
  CombiningDiacriticalMarksExtended,
  Balinese,
  Sundanese,
  Batak,
  Lepcha,
  OlChiki,
  CyrillicExtendedC,
  GeorgianExtended,
  SundaneseSupplement,
  VedicExtensions,
  PhoneticExtensions,
  PhoneticExtensionsSupplement,
  CombiningDiacriticalMarksSupplement,
  LatinExtendedAdditional,
  GreekExtended,
  GeneralPunctuation,
  SuperscriptsAndSubscripts,
  CurrencySymbols,
  CombiningDiacriticalMarksForSymbols,
  LetterlikeSymbols,
  NumberForms,
  Arrows,
  MathematicalOperators,
  MiscellaneousTechnical,
  ControlPictures,
  OpticalCharacterRecognition,
  EnclosedAlphanumerics,
  BoxDrawing,
  BlockElements,
  GeometricShapes,
  MiscellaneousSymbols,
  Dingbats,
  MiscellaneousMathematicalSymbolsA,
  SupplementalArrowsA,
  BraillePatterns,
  SupplementalArrowsB,
  MiscellaneousMathematicalSymbolsB,
  SupplementalMathematicalOperators,
  MiscellaneousSymbolsAndArrows,
  Glagolitic,
  LatinExtendedC,
  Coptic,
  GeorgianSupplement,
  Tifinagh,
  EthiopicExtended,
  CyrillicExtendedA,
  SupplementalPunctuation,
  CJKRadicalsSupplement,
  KangxiRadicals,
  IdeographicDescriptionCharacters,
  CJKSymbolsAndPunctuation,
  Hiragana,
  Katakana,
  Bopomofo,
  HangulCompatibilityJamo,
  Kanbun,
  BopomofoExtended,
  CJKStrokes,
  KatakanaPhoneticExtensions,
  EnclosedCJKLettersAndMonths,
  CJKCompatibility,
  CJKUnifiedIdeographsExtensionA,
  YijingHexagramSymbols,
  CJKUnifiedIdeographs,
  YiSyllables,
  YiRadicals,
  Lisu,
  Vai,
  CyrillicExtendedB,
  Bamum,
  ModifierToneLetters,
  LatinExtendedD,
  SylotiNagri,
  CommonIndicNumberForms,
  Phagspa,
  Saurashtra,
  DevanagariExtended,
  KayahLi,
  Rejang,
  HangulJamoExtendedA,
  Javanese,
  MyanmarExtendedB,
  Cham,
  MyanmarExtendedA,
  TaiViet,
  MeeteiMayekExtensions,
  EthiopicExtendedA,
  LatinExtendedE,
  CherokeeSupplement,
  MeeteiMayek,
  HangulSyllables,
  HangulJamoExtendedB,
  HighSurrogates,
  HighPrivateUseSurrogates,
  LowSurrogates,
  PrivateUseArea,
  CJKCompatibilityIdeographs,
  AlphabeticPresentationForms,
  ArabicPresentationFormsA,
  VariationSelectors,
  VerticalForms,
  CombiningHalfMarks,
  CJKCompatibilityForms,
  SmallFormVariants,
  ArabicPresentationFormsB,
  HalfwidthAndFullwidthForms,
  Specials,
  LinearBSyllabary,
  LinearBIdeograms,
  AegeanNumbers,
  AncientGreekNumbers,
  AncientSymbols,
  PhaistosDisc,
  Lycian,
  Carian,
  CopticEpactNumbers,
  OldItalic,
  Gothic,
  OldPermic,
  Ugaritic,
  OldPersian,
  Deseret,
  Shavian,
  Osmanya,
  Osage,
  Elbasan,
  CaucasianAlbanian,
  LinearA,
  CypriotSyllabary,
  ImperialAramaic,
  Palmyrene,
  Nabataean,
  Hatran,
  Phoenician,
  Lydian,
  MeroiticHieroglyphs,
  MeroiticCursive,
  Kharoshthi,
  OldSouthArabian,
  OldNorthArabian,
  Manichaean,
  Avestan,
  InscriptionalParthian,
  InscriptionalPahlavi,
  PsalterPahlavi,
  OldTurkic,
  OldHungarian,
  HanifiRohingya,
  RumiNumeralSymbols,
  Yezidi,
  OldSogdian,
  Sogdian,
  Chorasmian,
  Elymaic,
  Brahmi,
  Kaithi,
  SoraSompeng,
  Chakma,
  Mahajani,
  Sharada,
  SinhalaArchaicNumbers,
  Khojki,
  Multani,
  Khudawadi,
  Grantha,
  Newa,
  Tirhuta,
  Siddham,
  Modi,
  MongolianSupplement,
  Takri,
  Ahom,
  Dogra,
  WarangCiti,
  DivesAkuru,
  NAndinagari,
  ZanabazarSquare,
  Soyombo,
  PauCinHau,
  Bhaiksuki,
  Marchen,
  MasaramGondi,
  GunjalaGondi,
  Makasar,
  LisuSupplement,
  TamilSupplement,
  CuneiForm,
  CuneiFormNumbersAndPunctuation,
  EarlyDynasticCuneiForm,
  EgyptianHieroglyphs,
  EgyptianHieroglyphFormatControls,
  AnatolianHieroglyphs,
  BamumSupplement,
  Mro,
  BassaVah,
  PahawhHmong,
  Medefaidrin,
  Miao,
  IdeographicSymbolsAndPunctuation,
  Tangut,
  TangutComponents,
  KhitanSmallScript,
  TangutSupplement,
  KanaSupplement,
  KanaExtendedA,
  SmallKanaExtension,
  Nushu,
  Duployan,
  ShorthAndFormatControls,
  ByzantineMusicalSymbols,
  MusicalSymbols,
  AncientGreekMusicalNotation,
  MayanNumerals,
  TaiXuanJingSymbols,
  CountingRodNumerals,
  MathematicalAlphanumericSymbols,
  SuttonSignWriting,
  GlagoliticSupplement,
  NyiakengPuachueHmong,
  Wancho,
  MendeKikakui,
  Adlam,
  IndicSiyaqNumbers,
  OttomanSiyaqNumbers,
  ArabicMathematicalAlphabeticSymbols,
  MahjongTiles,
  DominoTiles,
  PlayingCards,
  EnclosedAlphanumericSupplement,
  EnclosedIdeographicSupplement,
  MiscellaneousSymbolsAndPictographs,
  Emoticons,
  OrnamentalDingbats,
  TransportAndMapSymbols,
  AlchemicalSymbols,
  GeometricShapesExtended,
  SupplementalArrowsC,
  SupplementalSymbolsAndPictographs,
  ChessSymbols,
  SymbolsAndPictographsExtendedA,
  SymbolsForLegacyComputing,
  CJKUnifiedIdeographsExtensionB,
  CJKUnifiedIdeographsExtensionC,
  CJKUnifiedIdeographsExtensionD,
  CJKUnifiedIdeographsExtensionE,
  CJKUnifiedIdeographsExtensionF,
  CJKCompatibilityIdeographsSupplement,
  CJKUnifiedIdeographsExtensionG,
  Tags,
  VariationSelectorsSupplement,
  SupplementaryPrivateUseAreaA,
  SupplementaryPrivateUseAreaB,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}

/// <summary>
/// 
/// </summary>
public static class UnicodeTools
{
  private static readonly Dictionary<uint, UnicodeRange> UnicodeRanges = new()
  {
    { 0x007F, UnicodeRange.BasicLatin },
    { 0x00FF, UnicodeRange.Latin1Supplement },
    { 0x017F, UnicodeRange.LatinExtendedA },
    { 0x024F, UnicodeRange.LatinExtendedB },
    { 0x02AF, UnicodeRange.IPAExtensions },
    { 0x02FF, UnicodeRange.SpacingModifierLetters },
    { 0x036F, UnicodeRange.CombiningDiacriticalMarks },
    { 0x03FF, UnicodeRange.GreekAndCoptic },
    { 0x04FF, UnicodeRange.Cyrillic },
    { 0x052F, UnicodeRange.CyrillicSupplement },
    { 0x058F, UnicodeRange.Armenian },
    { 0x05FF, UnicodeRange.Hebrew },
    { 0x06FF, UnicodeRange.Arabic },
    { 0x074F, UnicodeRange.Syriac },
    { 0x077F, UnicodeRange.ArabicSupplement },
    { 0x07BF, UnicodeRange.Thaana },
    { 0x07FF, UnicodeRange.NKo },
    { 0x083F, UnicodeRange.Samaritan },
    { 0x085F, UnicodeRange.MAndaic },
    { 0x086F, UnicodeRange.SyriacSupplement },
    { 0x089F, UnicodeRange.Unassigned },
    { 0x08FF, UnicodeRange.ArabicExtendedA },
    { 0x097F, UnicodeRange.Devanagari },
    { 0x09FF, UnicodeRange.Bengali },
    { 0x0A7F, UnicodeRange.Gurmukhi },
    { 0x0AFF, UnicodeRange.Gujarati },
    { 0x0B7F, UnicodeRange.Oriya },
    { 0x0BFF, UnicodeRange.Tamil },
    { 0x0C7F, UnicodeRange.Telugu },
    { 0x0CFF, UnicodeRange.Kannada },
    { 0x0D7F, UnicodeRange.Malayalam },
    { 0x0DFF, UnicodeRange.Sinhala },
    { 0x0E7F, UnicodeRange.Thai },
    { 0x0EFF, UnicodeRange.Lao },
    { 0x0FFF, UnicodeRange.Tibetan },
    { 0x109F, UnicodeRange.Myanmar },
    { 0x10FF, UnicodeRange.Georgian },
    { 0x11FF, UnicodeRange.HangulJamo },
    { 0x137F, UnicodeRange.Ethiopic },
    { 0x139F, UnicodeRange.EthiopicSupplement },
    { 0x13FF, UnicodeRange.Cherokee },
    { 0x167F, UnicodeRange.UnifiedCanadianAboriginalSyllabics },
    { 0x169F, UnicodeRange.Ogham },
    { 0x16FF, UnicodeRange.Runic },
    { 0x171F, UnicodeRange.Tagalog },
    { 0x173F, UnicodeRange.Hanunoo },
    { 0x175F, UnicodeRange.Buhid },
    { 0x177F, UnicodeRange.Tagbanwa },
    { 0x17FF, UnicodeRange.Khmer },
    { 0x18AF, UnicodeRange.Mongolian },
    { 0x18FF, UnicodeRange.UnifiedCanadianAboriginalSyllabicsExtended },
    { 0x194F, UnicodeRange.Limbu },
    { 0x197F, UnicodeRange.TaiLe },
    { 0x19DF, UnicodeRange.NewTaiLue },
    { 0x19FF, UnicodeRange.KhmerSymbols },
    { 0x1A1F, UnicodeRange.Buginese },
    { 0x1AAF, UnicodeRange.TaiTham },
    { 0x1AFF, UnicodeRange.CombiningDiacriticalMarksExtended },
    { 0x1B7F, UnicodeRange.Balinese },
    { 0x1BBF, UnicodeRange.Sundanese },
    { 0x1BFF, UnicodeRange.Batak },
    { 0x1C4F, UnicodeRange.Lepcha },
    { 0x1C7F, UnicodeRange.OlChiki },
    { 0x1C8F, UnicodeRange.CyrillicExtendedC },
    { 0x1CBF, UnicodeRange.GeorgianExtended },
    { 0x1CCF, UnicodeRange.SundaneseSupplement },
    { 0x1CFF, UnicodeRange.VedicExtensions },
    { 0x1D7F, UnicodeRange.PhoneticExtensions },
    { 0x1DBF, UnicodeRange.PhoneticExtensionsSupplement },
    { 0x1DFF, UnicodeRange.CombiningDiacriticalMarksSupplement },
    { 0x1EFF, UnicodeRange.LatinExtendedAdditional },
    { 0x1FFF, UnicodeRange.GreekExtended },
    { 0x206F, UnicodeRange.GeneralPunctuation },
    { 0x209F, UnicodeRange.SuperscriptsAndSubscripts },
    { 0x20CF, UnicodeRange.CurrencySymbols },
    { 0x20FF, UnicodeRange.CombiningDiacriticalMarksForSymbols },
    { 0x214F, UnicodeRange.LetterlikeSymbols },
    { 0x218F, UnicodeRange.NumberForms },
    { 0x21FF, UnicodeRange.Arrows },
    { 0x22FF, UnicodeRange.MathematicalOperators },
    { 0x23FF, UnicodeRange.MiscellaneousTechnical },
    { 0x243F, UnicodeRange.ControlPictures },
    { 0x245F, UnicodeRange.OpticalCharacterRecognition },
    { 0x24FF, UnicodeRange.EnclosedAlphanumerics },
    { 0x257F, UnicodeRange.BoxDrawing },
    { 0x259F, UnicodeRange.BlockElements },
    { 0x25FF, UnicodeRange.GeometricShapes },
    { 0x26FF, UnicodeRange.MiscellaneousSymbols },
    { 0x27BF, UnicodeRange.Dingbats },
    { 0x27EF, UnicodeRange.MiscellaneousMathematicalSymbolsA },
    { 0x27FF, UnicodeRange.SupplementalArrowsA },
    { 0x28FF, UnicodeRange.BraillePatterns },
    { 0x297F, UnicodeRange.SupplementalArrowsB },
    { 0x29FF, UnicodeRange.MiscellaneousMathematicalSymbolsB },
    { 0x2AFF, UnicodeRange.SupplementalMathematicalOperators },
    { 0x2BFF, UnicodeRange.MiscellaneousSymbolsAndArrows },
    { 0x2C5F, UnicodeRange.Glagolitic },
    { 0x2C7F, UnicodeRange.LatinExtendedC },
    { 0x2CFF, UnicodeRange.Coptic },
    { 0x2D2F, UnicodeRange.GeorgianSupplement },
    { 0x2D7F, UnicodeRange.Tifinagh },
    { 0x2DDF, UnicodeRange.EthiopicExtended },
    { 0x2DFF, UnicodeRange.CyrillicExtendedA },
    { 0x2E7F, UnicodeRange.SupplementalPunctuation },
    { 0x2EFF, UnicodeRange.CJKRadicalsSupplement },
    { 0x2FDF, UnicodeRange.KangxiRadicals },
    { 0x2FEF, UnicodeRange.Unassigned },
    { 0x2FFF, UnicodeRange.IdeographicDescriptionCharacters },
    { 0x303F, UnicodeRange.CJKSymbolsAndPunctuation },
    { 0x309F, UnicodeRange.Hiragana },
    { 0x30FF, UnicodeRange.Katakana },
    { 0x312F, UnicodeRange.Bopomofo },
    { 0x318F, UnicodeRange.HangulCompatibilityJamo },
    { 0x319F, UnicodeRange.Kanbun },
    { 0x31BF, UnicodeRange.BopomofoExtended },
    { 0x31EF, UnicodeRange.CJKStrokes },
    { 0x31FF, UnicodeRange.KatakanaPhoneticExtensions },
    { 0x32FF, UnicodeRange.EnclosedCJKLettersAndMonths },
    { 0x33FF, UnicodeRange.CJKCompatibility },
    { 0x4DBF, UnicodeRange.CJKUnifiedIdeographsExtensionA },
    { 0x4DFF, UnicodeRange.YijingHexagramSymbols },
    { 0x9FFF, UnicodeRange.CJKUnifiedIdeographs },
    { 0xA48F, UnicodeRange.YiSyllables },
    { 0xA4CF, UnicodeRange.YiRadicals },
    { 0xA4FF, UnicodeRange.Lisu },
    { 0xA63F, UnicodeRange.Vai },
    { 0xA69F, UnicodeRange.CyrillicExtendedB },
    { 0xA6FF, UnicodeRange.Bamum },
    { 0xA71F, UnicodeRange.ModifierToneLetters },
    { 0xA7FF, UnicodeRange.LatinExtendedD },
    { 0xA82F, UnicodeRange.SylotiNagri },
    { 0xA83F, UnicodeRange.CommonIndicNumberForms },
    { 0xA87F, UnicodeRange.Phagspa },
    { 0xA8DF, UnicodeRange.Saurashtra },
    { 0xA8FF, UnicodeRange.DevanagariExtended },
    { 0xA92F, UnicodeRange.KayahLi },
    { 0xA95F, UnicodeRange.Rejang },
    { 0xA97F, UnicodeRange.HangulJamoExtendedA },
    { 0xA9DF, UnicodeRange.Javanese },
    { 0xA9FF, UnicodeRange.MyanmarExtendedB },
    { 0xAA5F, UnicodeRange.Cham },
    { 0xAA7F, UnicodeRange.MyanmarExtendedA },
    { 0xAADF, UnicodeRange.TaiViet },
    { 0xAAFF, UnicodeRange.MeeteiMayekExtensions },
    { 0xAB2F, UnicodeRange.EthiopicExtendedA },
    { 0xAB6F, UnicodeRange.LatinExtendedE },
    { 0xABBF, UnicodeRange.CherokeeSupplement },
    { 0xABFF, UnicodeRange.MeeteiMayek },
    { 0xD7AF, UnicodeRange.HangulSyllables },
    { 0xD7FF, UnicodeRange.HangulJamoExtendedB },
    { 0xDB7F, UnicodeRange.HighSurrogates },
    { 0xDBFF, UnicodeRange.HighPrivateUseSurrogates },
    { 0xDFFF, UnicodeRange.LowSurrogates },
    { 0xF8FF, UnicodeRange.PrivateUseArea },
    { 0xFAFF, UnicodeRange.CJKCompatibilityIdeographs },
    { 0xFB4F, UnicodeRange.AlphabeticPresentationForms },
    { 0xFDFF, UnicodeRange.ArabicPresentationFormsA },
    { 0xFE0F, UnicodeRange.VariationSelectors },
    { 0xFE1F, UnicodeRange.VerticalForms },
    { 0xFE2F, UnicodeRange.CombiningHalfMarks },
    { 0xFE4F, UnicodeRange.CJKCompatibilityForms },
    { 0xFE6F, UnicodeRange.SmallFormVariants },
    { 0xFEFF, UnicodeRange.ArabicPresentationFormsB },
    { 0xFFEF, UnicodeRange.HalfwidthAndFullwidthForms },
    { 0xFFFF, UnicodeRange.Specials },
  };

  /// <summary>
  /// Gets the Unicode range of a character.
  /// </summary>
  /// <param name="c"></param>
  /// <returns></returns>
  public static UnicodeRange GetUnicodeRange(char c)
  {
    var codePoint = (uint)c;
    foreach (var range in UnicodeRanges)
    {
      if (codePoint <= range.Key)
        return range.Value;
    }
    return UnicodeRange.Unassigned;
  }
}
