using System;
using System.Collections.Generic;
using System.Text;
using Qhta.Collections;
using Qhta.Unicode;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Definitions of character names used in RichText and XmlTaggedText
/// 
/// </summary>
public static class CharNames
{
  /// <summary>
  /// Try to get the name for a character code point.
  /// </summary>
  /// <param name="cp"></param>
  /// <param name="name"></param>
  /// <returns></returns>
  public static bool TryGetName(int cp, out string name)
  {
    return CharMapping.TryGetValue(cp, out name);
  }

  /// <summary>
  /// Try to get a character code point for the name.
  /// </summary>
  /// <param name="cp"></param>
  /// <param name="name"></param>
  /// <returns></returns>
  public static bool TryFindName(string name, out int cp)
  {
    if (name.Length == 1)
    {
      return EscapeMapping.TryGetValue1(name, out cp);
    }
    return CharMapping.TryGetValue1(name, out cp);
  }

  /// <summary>
  /// Try to get the function for a character code point.
  /// </summary>
  /// <param name="cp"></param>
  /// <param name="function"></param>
  /// <returns></returns>
  public static bool TryGetFunction(int cp, out string function)
  {
    return CharFunctions.TryGetValue(cp, out function);
  }

  /// <summary>
  /// Try to get a character code point for the function.
  /// </summary>
  /// <param name="cp"></param>
  /// <param name="function"></param>
  /// <returns></returns>
  public static bool TryFindFunction(string function, out int cp)
  {
    return CharFunctions.TryGetValue1(function, out cp);
  }


  /// <summary>
  /// Registered escape sequences for characters
  /// </summary>
  public static readonly BiDiDictionary<int, string> EscapeMapping = new BiDiDictionary<int, string>()
  {
    { 0x0009, @"t" },
    { 0x000A, @"n" },
    { 0x000B, @"v" },
    { 0x000C, @"f" },
    { 0x000D, @"r" },
    { 0x0022, @"""" },
    { 0x0027, @"'" },
    { 0x005C, @"\" },
    { 0x2028, @"l" },
    { 0x2029, @"p" },
  };

  /// <summary>
  /// Registered character names
  /// </summary>
  public static BiDiDictionary<int, string> CharMapping => _CharMapping ??= CreateCharMapping();

  private static BiDiDictionary<int, string>? _CharMapping;

  private static BiDiDictionary<int, string> CreateCharMapping()
  {
    BiDiDictionary<int, string> charMapping = new BiDiDictionary<int, string>();
    ;
    foreach (var entry in UnicodeData.Instance.CharNameIndex)
    {
      var charInfo = UnicodeData.Instance[entry.Value];
      var charName = Unicode.CharNameIndex.CreateShortName(charInfo);
      if (charName is not null)
      {
        charMapping.Add(charInfo.CodePoint, charName);
      }
    }
    return charMapping;
  }


  /// <summary>
  /// Registered character functions
  /// </summary>
  public static readonly BiDiDictionary<int, string> CharFunctions = new BiDiDictionary<int, string>()
  {
    { 0x2070, "sup{0}" },                             // Superscript zero
    { 0x00B9, "sup{1}" },                             // Superscript one
    { 0x00B2, "sup{2}" },                             // Superscript two
    { 0x00B3, "sup{3}" },                             // Superscript three
    { 0x2074, "sup{4}" },                             // Superscript four
    { 0x2075, "sup{5}" },                             // Superscript five
    { 0x2076, "sup{6}" },                             // Superscript six
    { 0x2077, "sup{7}" },                             // Superscript seven
    { 0x2078, "sup{8}" },                             // Superscript eight
    { 0x2079, "sup{9}" },                             // Superscript nine
    { 0x207A, "sup{+}" },                             // Superscript plus sign
    { 0x207B, "sup{-}" },                             // Superscript minus
    { 0x207C, "sup{=}" },                             // Superscript equals sign
    { 0x207D, "sup{(}" },                             // Superscript left parenthesis
    { 0x207E, "sup{)}" },                             // Superscript right parenthesis
    { 0x2071, "sup{i}" },                             // Superscript latin small letter I
    { 0x207F, "sup{n}" },                             // Superscript latin small letter N
    { 0x2080, "sub{0}" },                             // Subscript zero
    { 0x2081, "sub{1}" },                             // Subscript one
    { 0x2082, "sub{2}" },                             // Subscript two
    { 0x2083, "sub{3}" },                             // Subscript three
    { 0x2084, "sub{4}" },                             // Subscript four
    { 0x2085, "sub{5}" },                             // Subscript five
    { 0x2086, "sub{6}" },                             // Subscript six
    { 0x2087, "sub{7}" },                             // Subscript seven
    { 0x2088, "sub{8}" },                             // Subscript eight
    { 0x2089, "sub{9}" },                             // Subscript nine
    { 0x208A, "sub{+}" },                             // Subscript plus sign
    { 0x208B, "sub{-}" },                             // Subscript minus
    { 0x208C, "sub{=}" },                             // Subscript equals sign
    { 0x208D, "sub{(}" },                             // Subscript left parenthesis
    { 0x208E, "sub{)}" },                             // Subscript right parenthesis
    { 0x2090, "sub{a}" },                             // Latin subscript small letter A
    { 0x2091, "sub{e}" },                             // Latin subscript small letter E
    { 0x2092, "sub{o}" },                             // Latin subscript small letter O
    { 0x2093, "sub{x}" },                             // Latin subscript small letter X
    { 0x2094, "sub{Ə}" },                             // Latin subscript small letter schwa
    { 0x2095, "sub{h}" },                             // Latin subscript small letter H
    { 0x2096, "sub{k}" },                             // Latin subscript small letter K
    { 0x2097, "sub{l}" },                             // Latin subscript small letter L
    { 0x2098, "sub{m}" },                             // Latin subscript small letter M
    { 0x2099, "sub{n}" },                             // Latin subscript small letter N
    { 0x209A, "sub{p}" },                             // Latin subscript small letter P
    { 0x209B, "sub{s}" },                             // Latin subscript small letter S
    { 0x209C, "sub{t}" },                             // Latin subscript small letter T
    { 0x2C7C, "sub{j}" },                             // Latin subscript small letter J

  };

}
