using System;
using System.Collections.Generic;
using System.Text;
using Qhta.Collections;

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
  public static bool TryGetValue(int cp, out string name)
  {
    return CharMapping.TryGetValue(cp, out name);
  }

  /// <summary>
  /// Try to get the name for a character code point  
  /// </summary>
  /// <param name="cp"></param>
  /// <param name="name"></param>
  /// <returns></returns>
  public static bool TryGetValue(string name, out int cp)
  {
    if (name.Length == 1)
    {
      return EscapeMapping.TryGetValue1(name, out cp);
    }
    return CharMapping.TryGetValue1(name, out cp);
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
  public static readonly BiDiDictionary<int, string> CharMapping = new BiDiDictionary<int, string>()
  {
    { 0x0000, "NUL" },
    { 0x0001, "STX" },
    { 0x0002, "SOT" },
    { 0x0003, "ETX" },
    { 0x0004, "EOT" },
    { 0x0005, "ENQ" },
    { 0x0006, "ACK" },
    { 0x0007, "BEL" },
    { 0x0008, "BS" },
    { 0x0009, "HT" },                                 // Horizontal tab
    { 0x000A, "LF" },                                 // Line feed
    { 0x000B, "VT" },                                 // Vertical tab
    { 0x000C, "FF" },                                 // Form feed
    { 0x000D, "CR" },                                 // Carriage return
    { 0x000E, "SO" },
    { 0x000F, "SI" },
    { 0x0010, "DLE" },
    { 0x0011, "DC1" },
    { 0x0012, "DC2" },
    { 0x0013, "DC3" },
    { 0x0014, "DC4" },
    { 0x0015, "NAK" },
    { 0x0016, "SYN" },
    { 0x0017, "ETB" },
    { 0x0018, "CAN" },
    { 0x0019, "EM" },
    { 0x001A, "SUB" },
    { 0x001B, "ESC" },
    { 0x001C, "FS" },
    { 0x001D, "GS" },
    { 0x001E, "RS" },
    { 0x001F, "US" },
    { 0x007F, "DEL" },
    { 0x0080, "PAD" },
    { 0x0081, "HOP" },
    { 0x0082, "BPH" },
    { 0x0083, "NBH" },
    { 0x0084, "IND" },
    { 0x0085, "NEL" },
    { 0x0086, "SSA" },
    { 0x0087, "ESA" },
    { 0x0088, "HTS" },
    { 0x0089, "HTJ" },
    { 0x008A, "VTS" },
    { 0x008B, "PLD" },
    { 0x008C, "PLU" },
    { 0x008D, "RI" },
    { 0x008E, "SS2" },
    { 0x008F, "SS3" },
    { 0x0090, "DCS" },
    { 0x0091, "PU1" },
    { 0x0092, "PU2" },
    { 0x0093, "STS" },
    { 0x0094, "CCH" },
    { 0x0095, "MW" },
    { 0x0096, "SPA" },
    { 0x0097, "EPA" },
    { 0x0098, "SOS" },
    { 0x0099, "SGC" },
    { 0x009A, "SCI" },
    { 0x009B, "CSI" },
    { 0x009C, "ST" },
    { 0x009D, "OSC" },
    { 0x009E, "PM" },
    { 0x009F, "APC" },
    { 0x00A0, "nbsp" },                               // No-break space
    { 0x2000, "enquad" },                             // En quad
    { 0x2001, "emquad" },                             // Em quad
    { 0x2002, "ensp" },                               // En space
    { 0x2003, "emsp" },                               // Em space
    { 0x2004, "tpmsp" },                              // Three-per-em space
    { 0x2005, "fpmsp" },                              // Four-per-em space
    { 0x2006, "spmsp" },                              // Six-per-em space
    { 0x2007, "numsp" },                              // Figure space
    { 0x2008, "pctsp" },                              // Punctuation space
    { 0x2009, "thinsp" },                             // Thin space
    { 0x200A, "hairsp" },                             // Hair space
    { 0x2028, "line" },                               // Line separator
    { 0x2029, "par" },                                // Paragraph separator
    { 0x202F, "nnbsp" },                              // Narrow no-break space
    { 0x205F, "mmsp" },                               // Medium mathematical space
    { 0x3000, "idsp" },                               // Ideographic space
    { 0x2010, "hp" },                                 // Hyphen
    { 0x2011, "nbhp" },                               // Non-breaking hyphen
    { 0x2012, "fdash" },                              // Figure dash
    { 0x2013, "endash" },                             // En dash
    { 0x2014, "emdash" },                             // Em dash
    { 0x2015, "longdash" },                           // Horizontal bar
    { 0x2027, "hpp" },                                // Hyphenation point
    { 0x2043, "hpb" },                                // Hyphen bullet
    { 0x2E17, "hpdo" },                               // Double oblique hyphen
    { 0x2E1A, "hpdi" },                               // Hyphen with dieresis
    { 0x2E40, "hpd" },                                // Double hyphen
    { 0x301C, "wdash" },                              // Wave dash
    { 0x2030, "wwdash" },                             // Wavy dash
    { 0x00AD, "sh" },                                 // Soft hyphen
    { 0x200B, "zwsp" },                               // Zero width space
    { 0x200C, "zwnj" },                               // Zero width non-joiner
    { 0x200D, "zwj" },                                // Zero width joiner
    { 0x200E, "ltr" },                                // Left-to-right mark
    { 0x200F, "rtl" },                                // Right-to-left mark
    { 0x202A, "ltre" },                               // Left-to-right embedding
    { 0x202B, "rtle" },                               // Right-to-left embedding
    { 0x202C, "pdf" },                                // Pop directional formatting
    { 0x202D, "ltro" },                               // Left-to-right override
    { 0x202E, "rtlo" },                               // Right-to-left override
    { 0x2060, "wj" },                                 // Word joiner
    { 0x2061, "fa" },                                 // Function application
    { 0x2062, "nvtimes" },                            // Invisible times
    { 0x2063, "nvsep" },                              // Invisible separator
    { 0x2064, "nvplus" },                             // Invisible plus
    { 0x2066, "ltri" },                               // Left-to-right isolate
    { 0x2067, "rtli" },                               // Right-to-left isolate
    { 0x2068, "fsi" },                                // First strong isolate
    { 0x2069, "pdi" },                                // Pop directional isolate
    { 0x206A, "ssi" },                                // Inhibit symmetric swapping
    { 0x206B, "ssa" },                                // Activate symmetric swapping
    { 0x206C, "afsi" },                               // Inhibit Arabic form shaping
    { 0x206D, "afsa" },                               // Activate Arabic form shaping
    { 0x206E, "nds" },                                // National digit shapes
    { 0x206F, "mds" },                                // Nominal digit shapes
    { 0xFEFF, "zwnbsp" },                             // Zero width no-break space
    { 0xFFF9, "iaa" },                                // Interlinear annotation anchor
    { 0xFFFA, "ias" },                                // Interlinear annotation separator
    { 0xFFFB, "iat" },                                // Interlinear annotation terminator
    { 0x005E, "circumflex" },                         // Circumflex accent
    { 0x0060, "grave" },                              // Grave accent
    { 0x00A8, "dieresis" },                           // Diaeresis
    { 0x00AF, "macron" },                             // Macron
    { 0x00B4, "acute" },                              // Acute accent
    { 0x00B8, "cedilla" },                            // Cedilla
    { 0x02C2, "arrowheadleft" },                      // Modifier letter left arrowhead
    { 0x02C3, "arrowheadright" },                     // Modifier letter right arrowhead
    { 0x02C4, "arrowheadup" },                        // Modifier letter up arrowhead
    { 0x02C5, "arrowheaddown" },                      // Modifier letter down arrowhead
    { 0x02D2, "ringrighthalfcenter" },                // Modifier letter centred right half ring
    { 0x02D3, "ringlefthalfsup" },                    // Modifier letter centred left half ring
    { 0x02D4, "tackupmid" },                          // Modifier letter up tack
    { 0x02D5, "tackdownmid" },                        // Modifier letter down tack
    { 0x02D6, "plusmod" },                            // Modifier letter plus sign
    { 0x02D7, "minusmod" },                           // Modifier letter minus sign
    { 0x02D8, "breve" },                              // Breve
    { 0x02D9, "dotaccent" },                          // Dot above
    { 0x02DA, "ring" },                               // Ring above
    { 0x02DB, "ogonek" },                             // Ogonek
    { 0x02DC, "tilde" },                              // Small tilde
    { 0x02DD, "hungarumlaut" },                       // Double acute accent
    { 0x02DE, "rhotichook" },                         // Modifier letter rhotic hook
    { 0x02DF, "qofdagesh" },                          // Modifier letter cross accent
    { 0x02E5, "toneextrahigh" },                      // Modifier letter extra-high tone bar
    { 0x02E6, "tonehigh" },                           // Modifier letter high tone bar
    { 0x02E7, "tonemid" },                            // Modifier letter mid tone bar
    { 0x02E8, "tonelow" },                            // Modifier letter low tone bar
    { 0x02E9, "toneextralow" },                       // Modifier letter extra-low tone bar
    { 0x02EA, "ltrmark" },                            // Modifier letter yin departing tone mark
    { 0x02EB, "rtlmark" },                            // Modifier letter yang departing tone mark
    { 0x02ED, "unaspirated" },                        // Modifier letter unaspirated
    { 0x02EF, "arrowheadlowdown" },                   // Modifier letter low down arrowhead
    { 0x02F0, "arrowheadlowup" },                     // Modifier letter low up arrowhead
    { 0x02F1, "arrowheadlowleft" },                   // Modifier letter low left arrowhead
    { 0x02F2, "arrowheadlowright" },                  // Modifier letter low right arrowhead
    { 0x02F3, "lowring" },                            // Modifier letter low ring
    { 0x02F4, "midgrave" },                           // Modifier letter middle grave accent
    { 0x02F5, "middgrave" },                          // Modifier letter middle double grave accent
    { 0x02F6, "middacute" },                          // Modifier letter middle double acute accent
    { 0x02F7, "lowtilde" },                           // Modifier letter low tilde
    { 0x02F8, "raisecolon" },                         // Modifier letter raised colon
    { 0x02F9, "hightonestart" },                      // Modifier letter begin high tone
    { 0x02FA, "hightoneend" },                        // Modifier letter end high tone
    { 0x02FB, "lowtonestart" },                       // Modifier letter begin low tone
    { 0x02FC, "lowtoneend" },                         // Modifier letter end low tone
    { 0x02FD, "shelf" },                              // Modifier letter shelf
    { 0x02FE, "openshelf" },                          // Modifier letter open shelf
    { 0x02FF, "lowarrowleft" },                       // Modifier letter low left arrow
    { 0x0300, "gravenosp" },                          // Combining grave accent
    { 0x0301, "acutenosp" },                          // Combining acute accent
    { 0x0302, "circumflexnosp" },                     // Combining circumflex accent
    { 0x0303, "tildenosp" },                          // Combining tilde
    { 0x0304, "macronnosp" },                         // Combining macron
    { 0x0305, "overscorenosp" },                      // Combining overline
    { 0x0306, "brevenosp" },                          // Combining breve
    { 0x0307, "dotnosp" },                            // Combining dot above
    { 0x0308, "dieresisnosp" },                       // Combining diaeresis
    { 0x0309, "hooksupnosp" },                        // Combining hook above
    { 0x030A, "ringnosp" },                           // Combining ring above
    { 0x030B, "acutedblnosp" },                       // Combining double acute accent
    { 0x030C, "haceknosp" },                          // Combining caron
    { 0x030D, "linevertnosp" },                       // Combining vertical line above
    { 0x030E, "linevertdblnosp" },                    // Combining double vertical line above
    { 0x030F, "gravedblnosp" },                       // Combining double grave accent
    { 0x0310, "candrabindunosp" },                    // Combining candrabindu
    { 0x0311, "breveinvnosp" },                       // Combining inverted breve
    { 0x0312, "commaturnsupnosp" },                   // Combining turned comma above
    { 0x0313, "apostrophesupnosp" },                  // Combining comma above
    { 0x0314, "commasuprevnosp" },                    // Combining reversed comma above
    { 0x0315, "commasuprightnosp" },                  // Combining comma above right
    { 0x0316, "gravesubnosp" },                       // Combining grave accent below
    { 0x0317, "acutesubnosp" },                       // Combining acute accent below
    { 0x0318, "tackleftsubnosp" },                    // Combining left tack below
    { 0x0319, "tackrightsubnosp" },                   // Combining right tack below
    { 0x031A, "anglesupnosp" },                       // Combining left angle above
    { 0x031B, "hornnosp" },                           // Combining horn
    { 0x031C, "ringlefthalfsubnosp" },                // Combining left half ring below
    { 0x031D, "tackupsubnosp" },                      // Combining up tack below
    { 0x031E, "tackdownsubnosp" },                    // Combining down tack below
    { 0x031F, "plussubnosp" },                        // Combining plus sign below
    { 0x0320, "minussubnosp" },                       // Combining minus sign below
    { 0x0321, "hooksubpalatnosp" },                   // Combining palatalized hook below
    { 0x0322, "hooksubretronosp" },                   // Combining retroflex hook below
    { 0x0323, "dotsubnosp" },                         // Combining dot below
    { 0x0324, "dotdblsubnosp" },                      // Combining diaeresis below
    { 0x0325, "ringsubnosp" },                        // Combining ring below
    { 0x0326, "commasubnosp" },                       // Combining comma below
    { 0x0327, "cedillanosp" },                        // Combining cedilla
    { 0x0328, "ogoneknosp" },                         // Combining ogonek
    { 0x0329, "linevertsubnosp" },                    // Combining vertical line below
    { 0x032A, "bridgesubnosp" },                      // Combining bridge below
    { 0x032B, "archdblsubnosp" },                     // Combining inverted double arch below
    { 0x032C, "haceksubnosp" },                       // Combining caron below
    { 0x032D, "circumflexsubnosp" },                  // Combining circumflex accent below
    { 0x032E, "brevesubnosp" },                       // Combining breve below
    { 0x032F, "breveinvsubnosp" },                    // Combining inverted breve below
    { 0x0330, "tildesubnosp" },                       // Combining tilde below
    { 0x0331, "macronsubnosp" },                      // Combining macron below
    { 0x0332, "underscorenosp" },                     // Combining low line
    { 0x0333, "underscoredblnosp" },                  // Combining double low line
    { 0x0334, "tildemidnosp" },                       // Combining tilde overlay
    { 0x0335, "barmidshortnosp" },                    // Combining short stroke overlay
    { 0x0336, "barmidlongnosp" },                     // Combining long stroke overlay
    { 0x0337, "slashshortnosp" },                     // Combining short solidus overlay
    { 0x0338, "slashlongnosp" },                      // Combining long solidus overlay
    { 0x0339, "ringrighthalfsubnosp" },               // Combining right half ring below
    { 0x033A, "bridgeinvsubnosp" },                   // Combining inverted bridge below
    { 0x033B, "squaresubnosp" },                      // Combining square below
    { 0x033C, "seagullsubnosp" },                     // Combining seagull below
    { 0x033D, "xsupnosp" },                           // Combining x above
    { 0x033E, "tildevertsupnosp" },                   // Combining vertical tilde
    { 0x033F, "overscoredblnosp" },                   // Combining double overline
    { 0x0340, "graveleftnosp" },                      // Combining grave tone mark
    { 0x0341, "acuterightnosp" },                     // Combining acute tone mark
    { 0x0342, "perispomenigreekcmb" },                // Combining greek perispomeni
    { 0x0343, "koroniscmb" },                         // Combining greek koronis
    { 0x0344, "diaeresistonosnosp" },                 // Combining greek dialytika tonos
    { 0x0345, "iotasubnosp" },                        // Combining greek ypogegrammeni
    { 0x2070, "zerosuperior" },                       // Superscript zero
    { 0x00B9, "onesuperior" },                        // Superscript one
    { 0x00B2, "twosuperior" },                        // Superscript two
    { 0x00B3, "threesuperior" },                      // Superscript three
    { 0x2074, "foursuperior" },                       // Superscript four
    { 0x2075, "fivesuperior" },                       // Superscript five
    { 0x2076, "sixsuperior" },                        // Superscript six
    { 0x2077, "sevensuperior" },                      // Superscript seven
    { 0x2078, "eightsuperior" },                      // Superscript eight
    { 0x2079, "ninesuperior" },                       // Superscript nine
    { 0x207A, "plussuperior" },                       // Superscript plus sign
    { 0x207B, "minussuperior" },                      // Superscript minus
    { 0x207C, "equalsuperior" },                      // Superscript equals sign
    { 0x207D, "parenleftsuperior" },                  // Superscript left parenthesis
    { 0x207E, "parenrightsuperior" },                 // Superscript right parenthesis
    { 0x2071, "isuperior" },                          // Superscript latin small letter I
    { 0x207F, "nsuperior" },                          // Superscript latin small letter N
    { 0x2080, "zerosub" },                            // Subscript zero
    { 0x2081, "onesub" },                             // Subscript one
    { 0x2082, "twosub" },                             // Subscript two
    { 0x2083, "threesub" },                           // Subscript three
    { 0x2084, "foursub" },                            // Subscript four
    { 0x2085, "fivesub" },                            // Subscript five
    { 0x2086, "sixsub" },                             // Subscript six
    { 0x2087, "sevensub" },                           // Subscript seven
    { 0x2088, "eightsub" },                           // Subscript eight
    { 0x2089, "ninesub" },                            // Subscript nine
    { 0x208A, "plussub" },                            // Subscript plus sign
    { 0x208B, "minussub" },                           // Subscript minus
    { 0x208C, "equalsub" },                           // Subscript equals sign
    { 0x208D, "parenleftsub" },                       // Subscript left parenthesis
    { 0x208E, "parenrightsub" },                      // Subscript right parenthesis
    { 0x2090, "asub" },                               // Latin subscript small letter A
    { 0x2091, "esub" },                               // Latin subscript small letter E
    { 0x2092, "osub" },                               // Latin subscript small letter O
    { 0x2093, "xsub" },                               // Latin subscript small letter X
    { 0x2094, "schwasub" },                           // Latin subscript small letter schwa
    { 0x2095, "hsub" },                               // Latin subscript small letter H
    { 0x2096, "ksub" },                               // Latin subscript small letter K
    { 0x2097, "lsub" },                               // Latin subscript small letter L
    { 0x2098, "msub" },                               // Latin subscript small letter M
    { 0x2099, "nsub" },                               // Latin subscript small letter N
    { 0x209A, "psub" },                               // Latin subscript small letter P
    { 0x209B, "ssub" },                               // Latin subscript small letter S
    { 0x209C, "tsub" },                               // Latin subscript small letter T
    { 0x2C7C, "jsub" },                               // Latin subscript small letter J
    { 0x2160, "romanOne" },                           // Roman numeral one
    { 0x2161, "romanTwo" },                           // Roman numeral two
    { 0x2162, "romanThree" },                         // Roman numeral three
    { 0x2163, "romanFour" },                          // Roman numeral four
    { 0x2164, "romanFive" },                          // Roman numeral five
    { 0x2165, "romanSix" },                           // Roman numeral six
    { 0x2166, "romanSeven" },                         // Roman numeral seven
    { 0x2167, "romanEight" },                         // Roman numeral eight
    { 0x2168, "romanNine" },                          // Roman numeral nine
    { 0x2169, "romanTen" },                           // Roman numeral ten
    { 0x216A, "romanEleven" },                        // Roman numeral eleven
    { 0x216B, "romanTwelve" },                        // Roman numeral twelve
    { 0x216C, "romanFifty" },                         // Roman numeral fifty
    { 0x216D, "romanHundred" },                       // Roman numeral one hundred
    { 0x216E, "romanFivehundred" },                   // Roman numeral five hundred
    { 0x216F, "romanThousand" },                      // Roman numeral one thousand
    { 0x2170, "romanone" },                           // Small roman numeral one
    { 0x2171, "romantwo" },                           // Small roman numeral two
    { 0x2172, "romanthree" },                         // Small roman numeral three
    { 0x2173, "romanfour" },                          // Small roman numeral four
    { 0x2174, "romanfive" },                          // Small roman numeral five
    { 0x2175, "romansix" },                           // Small roman numeral six
    { 0x2176, "romanseven" },                         // Small roman numeral seven
    { 0x2177, "romaneight" },                         // Small roman numeral eight
    { 0x2178, "romannine" },                          // Small roman numeral nine
    { 0x2179, "romanten" },                           // Small roman numeral ten
    { 0x217A, "romaneleven" },                        // Small roman numeral eleven
    { 0x217B, "romantwelve" },                        // Small roman numeral twelve
    { 0x217C, "romanfifty" },                         // Small roman numeral fifty
    { 0x217D, "romanhundred" },                       // Small roman numeral one hundred
    { 0x217E, "romanfivehundred" },                   // Small roman numeral five hundred
    { 0x217F, "romanthousand" },                      // Small roman numeral one thousand
    { 0x0020, "space" },                              // Space
    { 0x0021, "exclam" },                             // Exclamation mark
    { 0x0022, "quotedbl" },                           // Quotation mark
    { 0x0023, "numbersign" },                         // Number sign
    { 0x0024, "dollar" },                             // Dollar sign
    { 0x0025, "percent" },                            // Percent sign
    { 0x0026, "ampersand" },                          // Ampersand
    { 0x0027, "quotesingle" },                        // Apostrophe
    { 0x0028, "parenleft" },                          // Left parenthesis
    { 0x0029, "parenright" },                         // Right parenthesis
    { 0x002A, "asterisk" },                           // Asterisk
    { 0x002B, "plus" },                               // Plus sign
    { 0x002C, "comma" },                              // Comma
    { 0x002D, "hyphen" },                             // Hyphen-minus
    { 0x002E, "period" },                             // Full stop
    { 0x002F, "slash" },                              // Solidus
    { 0x0030, "zero" },                               // Digit zero
    { 0x0031, "one" },                                // Digit one
    { 0x0032, "two" },                                // Digit two
    { 0x0033, "three" },                              // Digit three
    { 0x0034, "four" },                               // Digit four
    { 0x0035, "five" },                               // Digit five
    { 0x0036, "six" },                                // Digit six
    { 0x0037, "seven" },                              // Digit seven
    { 0x0038, "eight" },                              // Digit eight
    { 0x0039, "nine" },                               // Digit nine
    { 0x003A, "colon" },                              // Colon
    { 0x003B, "semicolon" },                          // Semicolon
    { 0x003C, "less" },                               // Less-than sign
    { 0x003D, "equal" },                              // Equals sign
    { 0x003E, "greater" },                            // Greater-than sign
    { 0x003F, "questionmark" },                       // Question mark
    { 0x0040, "at" },                                 // Commercial at
    { 0x005B, "bracketleft" },                        // Left square bracket
    { 0x005C, "backslash" },                          // Reverse solidus
    { 0x005D, "bracketright" },                       // Right square bracket
    { 0x005E, "circumflex" },                         // Circumflex accent
    { 0x005F, "underscore" },                         // Low line
    { 0x0060, "grave" },                              // Grave accent
    { 0x007B, "braceleft" },                          // Left curly bracket
    { 0x007C, "verticalbar" },                        // Vertical line
    { 0x007D, "braceright" },                         // Right curly bracket
    { 0x007E, "tilde" },                              // Tilde
    { 0x00A0, "nbspace" },                            // No-break space
    { 0x00A1, "exclamdown" },                         // Inverted exclamation mark
    { 0x00A2, "cent" },                               // Cent sign
    { 0x00A3, "sterling" },                           // Pound sign
    { 0x00A4, "currency" },                           // Currency sign
    { 0x00A5, "yen" },                                // Yen sign
    { 0x00A6, "brokenbar" },                          // Broken bar
    { 0x00A7, "section" },                            // Section sign
    { 0x00A8, "dieresis" },                           // Diaeresis
    { 0x00A9, "copyright" },                          // Copyright sign
    { 0x00AA, "ordfeminine" },                        // Feminine ordinal indicator
    { 0x00AB, "guillemotleft" },                      // Left-pointing double angle quotation mark
    { 0x00AC, "logicalnot" },                         // Not sign
    { 0x00AD, "softhyphen" },                         // Soft hyphen
    { 0x00AE, "registered" },                         // Registered sign
    { 0x00AF, "macron" },                             // Macron
    { 0x00B0, "degree" },                             // Degree sign
    { 0x00B1, "plusminus" },                          // Plus-minus sign
    { 0x00B2, "twosuperior" },                        // Superscript two
    { 0x00B3, "threesuperior" },                      // Superscript three
    { 0x00B4, "acute" },                              // Acute accent
    { 0x00B5, "micro" },                              // Micro sign
    { 0x00B6, "paragraph" },                          // Pilcrow sign
    { 0x00B7, "mdot" },                               // Middle dot
    { 0x00B8, "cedilla" },                            // Cedilla
    { 0x00B9, "onesuperior" },                        // Superscript one
    { 0x00BA, "ordmasculine" },                       // Masculine ordinal indicator
    { 0x00BB, "guillemotright" },                     // Right-pointing double angle quotation mark
    { 0x00BC, "onequarter" },                         // Vulgar fraction one quarter
    { 0x00BD, "onehalf" },                            // Vulgar fraction one half
    { 0x00BE, "threequarters" },                      // Vulgar fraction three quarters
    { 0x00BF, "questiondown" },                       // Inverted question mark
    { 0x00C0, "Agrave" },                             // Latin Capital Letter A With Grave
    { 0x00C1, "Aacute" },                             // Latin Capital Letter A With Acute
    { 0x00C2, "Acircumflex" },                        // Latin Capital Letter A With Circumflex
    { 0x00C3, "Atilde" },                             // Latin Capital Letter A With Tilde
    { 0x00C4, "Adieresis" },                          // Latin Capital Letter A With Diaeresis
    { 0x00C5, "Aring" },                              // Latin Capital Letter A With Ring Above
    { 0x00C6, "AE" },                                 // Latin Capital Letter Ae
    { 0x00C7, "Ccedilla" },                           // Latin Capital Letter C With Cedilla
    { 0x00C8, "Egrave" },                             // Latin Capital Letter E With Grave
    { 0x00C9, "Eacute" },                             // Latin Capital Letter E With Acute
    { 0x00CA, "Ecircumflex" },                        // Latin Capital Letter E With Circumflex
    { 0x00CB, "Edieresis" },                          // Latin Capital Letter E With Diaeresis
    { 0x00CC, "Igrave" },                             // Latin Capital Letter I With Grave
    { 0x00CD, "Iacute" },                             // Latin Capital Letter I With Acute
    { 0x00CE, "Icircumflex" },                        // Latin Capital Letter I With Circumflex
    { 0x00CF, "Idieresis" },                          // Latin Capital Letter I With Diaeresis
    { 0x00D0, "Eth" },                                // Latin Capital Letter Eth
    { 0x00D1, "Ntilde" },                             // Latin Capital Letter N With Tilde
    { 0x00D2, "Ograve" },                             // Latin Capital Letter O With Grave
    { 0x00D3, "Oacute" },                             // Latin Capital Letter O With Acute
    { 0x00D4, "Ocircumflex" },                        // Latin Capital Letter O With Circumflex
    { 0x00D5, "Otilde" },                             // Latin Capital Letter O With Tilde
    { 0x00D6, "Odieresis" },                          // Latin Capital Letter O With Diaeresis
    { 0x00D7, "times" },                              // Multiplication sign
    { 0x00D8, "Oslash" },                             // Latin Capital Letter O With Stroke
    { 0x00D9, "Ugrave" },                             // Latin Capital Letter U With Grave
    { 0x00DA, "Uacute" },                             // Latin Capital Letter U With Acute
    { 0x00DB, "Ucircumflex" },                        // Latin Capital Letter U With Circumflex
    { 0x00DC, "Udieresis" },                          // Latin Capital Letter U With Diaeresis
    { 0x00DD, "Yacute" },                             // Latin Capital Letter Y With Acute
    { 0x00DE, "Thorn" },                              // Latin Capital Letter Thorn
    { 0x00DF, "germandbls" },                         // Latin small letter sharp s
    { 0x00E0, "agrave" },                             // Latin small letter a with grave
    { 0x00E1, "aacute" },                             // Latin small letter a with acute
    { 0x00E2, "acircumflex" },                        // Latin small letter a with circumflex
    { 0x00E3, "atilde" },                             // Latin small letter a with tilde
    { 0x00E4, "adieresis" },                          // Latin small letter a with diaeresis
    { 0x00E5, "aring" },                              // Latin small letter a with ring above
    { 0x00E6, "ae" },                                 // Latin small letter ae
    { 0x00E7, "ccedilla" },                           // Latin small letter c with cedilla
    { 0x00E8, "egrave" },                             // Latin small letter e with grave
    { 0x00E9, "eacute" },                             // Latin small letter e with acute
    { 0x00EA, "ecircumflex" },                        // Latin small letter e with circumflex
    { 0x00EB, "edieresis" },                          // Latin small letter e with diaeresis
    { 0x00EC, "igrave" },                             // Latin small letter i with grave
    { 0x00ED, "iacute" },                             // Latin small letter i with acute
    { 0x00EE, "icircumflex" },                        // Latin small letter i with circumflex
    { 0x00EF, "idieresis" },                          // Latin small letter i with diaeresis
    { 0x00F0, "eth" },                                // Latin small letter eth
    { 0x00F1, "ntilde" },                             // Latin small letter n with tilde
    { 0x00F2, "ograve" },                             // Latin small letter o with grave
    { 0x00F3, "oacute" },                             // Latin small letter o with acute
    { 0x00F4, "ocircumflex" },                        // Latin small letter o with circumflex
    { 0x00F5, "otilde" },                             // Latin small letter o with tilde
    { 0x00F6, "odieresis" },                          // Latin small letter o with diaeresis
    { 0x00F7, "divide" },                             // Division sign
    { 0x00F8, "oslash" },                             // Latin small letter o with stroke
    { 0x00F9, "ugrave" },                             // Latin small letter u with grave
    { 0x00FA, "uacute" },                             // Latin small letter u with acute
    { 0x00FB, "ucircumflex" },                        // Latin small letter u with circumflex
    { 0x00FC, "udieresis" },                          // Latin small letter u with diaeresis
    { 0x00FD, "yacute" },                             // Latin small letter y with acute
    { 0x00FE, "thorn" },                              // Latin small letter thorn
    { 0x00FF, "ydieresis" },                          // Latin small letter y with diaeresis
    { 0x0100, "Amacron" },                            // Latin Capital Letter A With Macron
    { 0x0101, "amacron" },                            // Latin small letter a with macron
    { 0x0102, "Abreve" },                             // Latin Capital Letter A With Breve
    { 0x0103, "abreve" },                             // Latin small letter a with breve
    { 0x0104, "Aogonek" },                            // Latin Capital Letter A With Ogonek
    { 0x0105, "aogonek" },                            // Latin small letter a with ogonek
    { 0x0106, "Cacute" },                             // Latin Capital Letter C With Acute
    { 0x0107, "cacute" },                             // Latin small letter c with acute
    { 0x0108, "Ccircumflex" },                        // Latin Capital Letter C With Circumflex
    { 0x0109, "ccircumflex" },                        // Latin small letter c with circumflex
    { 0x010A, "Cdotaccent" },                         // Latin Capital Letter C With Dot Above
    { 0x010B, "cdotaccent" },                         // Latin small letter c with dot above
    { 0x010C, "Ccaron" },                             // Latin Capital Letter C With Caron
    { 0x010D, "ccaron" },                             // Latin small letter c with caron
    { 0x010E, "Dcaron" },                             // Latin Capital Letter D With Caron
    { 0x010F, "dcaron" },                             // Latin small letter d with caron
    { 0x0110, "Dstroke" },                            // Latin Capital Letter D With Stroke
    { 0x0111, "dstroke" },                            // Latin small letter d with stroke
    { 0x0112, "Emacron" },                            // Latin Capital Letter E With Macron
    { 0x0113, "emacron" },                            // Latin small letter e with macron
    { 0x0114, "Ebreve" },                             // Latin Capital Letter E With Breve
    { 0x0115, "ebreve" },                             // Latin small letter e with breve
    { 0x0116, "Edotaccent" },                         // Latin Capital Letter E With Dot Above
    { 0x0117, "edotaccent" },                         // Latin small letter e with dot above
    { 0x0118, "Eogonek" },                            // Latin Capital Letter E With Ogonek
    { 0x0119, "eogonek" },                            // Latin small letter e with ogonek
    { 0x011A, "Ecaron" },                             // Latin Capital Letter E With Caron
    { 0x011B, "ecaron" },                             // Latin small letter e with caron
    { 0x011C, "Gcircumflex" },                        // Latin Capital Letter G With Circumflex
    { 0x011D, "gcircumflex" },                        // Latin small letter g with circumflex
    { 0x011E, "Gbreve" },                             // Latin Capital Letter G With Breve
    { 0x011F, "gbreve" },                             // Latin small letter g with breve
    { 0x0120, "Gdotaccent" },                         // Latin Capital Letter G With Dot Above
    { 0x0121, "gdotaccent" },                         // Latin small letter g with dot above
    { 0x0122, "Gcedilla" },                           // Latin Capital Letter G With Cedilla
    { 0x0123, "gcedilla" },                           // Latin small letter g with cedilla
    { 0x0124, "Hcircumflex" },                        // Latin Capital Letter H With Circumflex
    { 0x0125, "hcircumflex" },                        // Latin small letter h with circumflex
    { 0x0126, "Hbar" },                               // Latin Capital Letter H With Stroke
    { 0x0127, "hbar" },                               // Latin small letter h with stroke
    { 0x0128, "Itilde" },                             // Latin Capital Letter I With Tilde
    { 0x0129, "itilde" },                             // Latin small letter i with tilde
    { 0x012A, "Imacron" },                            // Latin Capital Letter I With Macron
    { 0x012B, "imacron" },                            // Latin small letter i with macron
    { 0x012C, "Ibreve" },                             // Latin Capital Letter I With Breve
    { 0x012D, "ibreve" },                             // Latin small letter i with breve
    { 0x012E, "Iogonek" },                            // Latin Capital Letter I With Ogonek
    { 0x012F, "iogonek" },                            // Latin small letter i with ogonek
    { 0x0130, "Idotaccent" },                         // Latin Capital Letter I With Dot Above
    { 0x0131, "dotlessi" },                           // Latin small letter dotless i
    { 0x0132, "IJ" },                                 // Latin Capital Ligature Ij
    { 0x0133, "ij" },                                 // Latin small ligature ij
    { 0x0134, "Jcircumflex" },                        // Latin Capital Letter J With Circumflex
    { 0x0135, "jcircumflex" },                        // Latin small letter j with circumflex
    { 0x0136, "Kcedilla" },                           // Latin Capital Letter K With Cedilla
    { 0x0137, "kcedilla" },                           // Latin small letter k with cedilla
    { 0x0138, "kgreenlandic" },                       // Latin small letter kra
    { 0x0139, "Lacute" },                             // Latin Capital Letter L With Acute
    { 0x013A, "lacute" },                             // Latin small letter l with acute
    { 0x013B, "Lcedilla" },                           // Latin Capital Letter L With Cedilla
    { 0x013C, "lcedilla" },                           // Latin small letter l with cedilla
    { 0x013D, "Lcaron" },                             // Latin Capital Letter L With Caron
    { 0x013E, "lcaron" },                             // Latin small letter l with caron
    { 0x013F, "Ldotaccent" },                         // Latin Capital Letter L With Middle Dot
    { 0x0140, "ldotaccent" },                         // Latin small letter l with middle dot
    { 0x0141, "Lslash" },                             // Latin Capital Letter L With Stroke
    { 0x0142, "lslash" },                             // Latin small letter l with stroke
    { 0x0143, "Nacute" },                             // Latin Capital Letter N With Acute
    { 0x0144, "nacute" },                             // Latin small letter n with acute
    { 0x0145, "Ncedilla" },                           // Latin Capital Letter N With Cedilla
    { 0x0146, "ncedilla" },                           // Latin small letter n with cedilla
    { 0x0147, "Ncaron" },                             // Latin Capital Letter N With Caron
    { 0x0148, "ncaron" },                             // Latin small letter n with caron
    { 0x0149, "quoten" },                             // Latin small letter n preceded by apostrophe
    { 0x014A, "Eng" },                                // Latin Capital Letter Eng
    { 0x014B, "eng" },                                // Latin small letter eng
    { 0x014C, "Omacron" },                            // Latin Capital Letter O With Macron
    { 0x014D, "omacron" },                            // Latin small letter o with macron
    { 0x014E, "Obreve" },                             // Latin Capital Letter O With Breve
    { 0x014F, "obreve" },                             // Latin small letter o with breve
    { 0x0150, "Ohungarumlaut" },                      // Latin Capital Letter O With Double Acute
    { 0x0151, "ohungarumlaut" },                      // Latin small letter o with double acute
    { 0x0152, "OE" },                                 // Latin Capital Ligature Oe
    { 0x0153, "oe" },                                 // Latin small ligature oe
    { 0x0154, "Racute" },                             // Latin Capital Letter R With Acute
    { 0x0155, "racute" },                             // Latin small letter r with acute
    { 0x0156, "Rcedilla" },                           // Latin Capital Letter R With Cedilla
    { 0x0157, "rcedilla" },                           // Latin small letter r with cedilla
    { 0x0158, "Rcaron" },                             // Latin Capital Letter R With Caron
    { 0x0159, "rcaron" },                             // Latin small letter r with caron
    { 0x015A, "Sacute" },                             // Latin Capital Letter S With Acute
    { 0x015B, "sacute" },                             // Latin small letter s with acute
    { 0x015C, "Scircumflex" },                        // Latin Capital Letter S With Circumflex
    { 0x015D, "scircumflex" },                        // Latin small letter s with circumflex
    { 0x015E, "Scedilla" },                           // Latin Capital Letter S With Cedilla
    { 0x015F, "scedilla" },                           // Latin small letter s with cedilla
    { 0x0160, "Scaron" },                             // Latin Capital Letter S With Caron
    { 0x0161, "scaron" },                             // Latin small letter s with caron
    { 0x0162, "Tcedilla" },                           // Latin Capital Letter T With Cedilla
    { 0x0163, "tcedilla" },                           // Latin small letter t with cedilla
    { 0x0164, "Tcaron" },                             // Latin Capital Letter T With Caron
    { 0x0165, "tcaron" },                             // Latin small letter t with caron
    { 0x0166, "Tbar" },                               // Latin Capital Letter T With Stroke
    { 0x0167, "tbar" },                               // Latin small letter t with stroke
    { 0x0168, "Utilde" },                             // Latin Capital Letter U With Tilde
    { 0x0169, "utilde" },                             // Latin small letter u with tilde
    { 0x016A, "Umacron" },                            // Latin Capital Letter U With Macron
    { 0x016B, "umacron" },                            // Latin small letter u with macron
    { 0x016C, "Ubreve" },                             // Latin Capital Letter U With Breve
    { 0x016D, "ubreve" },                             // Latin small letter u with breve
    { 0x016E, "Uring" },                              // Latin Capital Letter U With Ring Above
    { 0x016F, "uring" },                              // Latin small letter u with ring above
    { 0x0170, "Uhungarumlaut" },                      // Latin Capital Letter U With Double Acute
    { 0x0171, "uhungarumlaut" },                      // Latin small letter u with double acute
    { 0x0172, "Uogonek" },                            // Latin Capital Letter U With Ogonek
    { 0x0173, "uogonek" },                            // Latin small letter u with ogonek
    { 0x0174, "Wcircumflex" },                        // Latin Capital Letter W With Circumflex
    { 0x0175, "wcircumflex" },                        // Latin small letter w with circumflex
    { 0x0176, "Ycircumflex" },                        // Latin Capital Letter Y With Circumflex
    { 0x0177, "ycircumflex" },                        // Latin small letter y with circumflex
    { 0x0178, "Ydieresis" },                          // Latin Capital Letter Y With Diaeresis
    { 0x0179, "Zacute" },                             // Latin Capital Letter Z With Acute
    { 0x017A, "zacute" },                             // Latin small letter z with acute
    { 0x017B, "Zdotaccent" },                         // Latin Capital Letter Z With Dot Above
    { 0x017C, "zdotaccent" },                         // Latin small letter z with dot above
    { 0x017D, "Zcaron" },                             // Latin Capital Letter Z With Caron
    { 0x017E, "zcaron" },                             // Latin small letter z with caron
    { 0x017F, "slong" },                              // Latin small letter long s
    { 0x0180, "bstroke" },                            // Latin small letter b with stroke
    { 0x0181, "Bhook" },                              // Latin Capital Letter B With Hook
    { 0x0182, "Btopbar" },                            // Latin Capital Letter B With Topbar
    { 0x0183, "btopbar" },                            // Latin small letter b with topbar
    { 0x0184, "Tonesix" },                            // Latin Capital Letter Tone Six
    { 0x0185, "tonesix" },                            // Latin small letter tone six
    { 0x0186, "Oopen" },                              // Latin Capital Letter Open O
    { 0x0187, "Chook" },                              // Latin Capital Letter C With Hook
    { 0x0188, "chook" },                              // Latin small letter c with hook
    { 0x0189, "Dafrican" },                           // Latin Capital Letter African D
    { 0x018A, "Dhook" },                              // Latin Capital Letter D With Hook
    { 0x018B, "Dtopbar" },                            // Latin Capital Letter D With Topbar
    { 0x018C, "dtopbar" },                            // Latin small letter d with topbar
    { 0x018D, "deltaturn" },                          // Latin small letter turned delta
    { 0x018E, "Eturn" },                              // Latin Capital Letter Reversed E
    { 0x018F, "Schwa" },                              // Latin Capital Letter Schwa
    { 0x0190, "Eopen" },                              // Latin Capital Letter Open E
    { 0x0191, "Fhook" },                              // Latin Capital Letter F With Hook
    { 0x0192, "florin" },                             // Latin small letter f with hook
    { 0x0193, "Ghook" },                              // Latin Capital Letter G With Hook
    { 0x0194, "Gammaafrican" },                       // Latin Capital Letter Gamma
    { 0x0195, "hv" },                                 // Latin small letter hv
    { 0x0196, "Iotaafrican" },                        // Latin Capital Letter Iota
    { 0x0197, "Istroke" },                            // Latin Capital Letter I With Stroke
    { 0x0198, "Khook" },                              // Latin Capital Letter K With Hook
    { 0x0199, "khook" },                              // Latin small letter k with hook
    { 0x019A, "lbar" },                               // Latin small letter l with bar
    { 0x019B, "lambdastroke" },                       // Latin small letter lambda with stroke
    { 0x019C, "Mturned" },                            // Latin Capital Letter Turned M
    { 0x019D, "Nhook" },                              // Latin Capital Letter N With Left Hook
    { 0x019E, "nleg" },                               // Latin small letter n with long right leg
    { 0x019F, "Ocenteredtilde" },                     // Latin Capital Letter O With Middle Tilde
    { 0x01A0, "Ohorn" },                              // Latin Capital Letter O With Horn
    { 0x01A1, "ohorn" },                              // Latin small letter o with horn
    { 0x01A2, "OI" },                                 // Latin Capital Letter Oi
    { 0x01A3, "oi" },                                 // Latin small letter oi
    { 0x01A4, "Phook" },                              // Latin Capital Letter P With Hook
    { 0x01A5, "phook" },                              // Latin small letter p with hook
    { 0x01A6, "YR" },                                 // Latin letter yr
    { 0x01A7, "Tonetwo" },                            // Latin Capital Letter Tone Two
    { 0x01A8, "tonetwo" },                            // Latin small letter tone two
    { 0x01A9, "Esh" },                                // Latin Capital Letter Esh
    { 0x01AA, "eshlooprev" },                         // Latin letter reversed esh loop
    { 0x01AB, "tpalatalhook" },                       // Latin small letter t with palatal hook
    { 0x01AC, "Thook" },                              // Latin Capital Letter T With Hook
    { 0x01AD, "thook" },                              // Latin small letter t with hook
    { 0x01AE, "Trthook" },                            // Latin Capital Letter T With Retroflex Hook
    { 0x01AF, "Uhorn" },                              // Latin Capital Letter U With Horn
    { 0x01B0, "uhorn" },                              // Latin small letter u with horn
    { 0x01B1, "Upsilonafrican" },                     // Latin Capital Letter Upsilon
    { 0x01B2, "Vhook" },                              // Latin Capital Letter V With Hook
    { 0x01B3, "Yhook" },                              // Latin Capital Letter Y With Hook
    { 0x01B4, "yhook" },                              // Latin small letter y with hook
    { 0x01B5, "Zstroke" },                            // Latin Capital Letter Z With Stroke
    { 0x01B6, "zstroke" },                            // Latin small letter z with stroke
    { 0x01B7, "Ezh" },                                // Latin Capital Letter Ezh
    { 0x01B8, "Ezhreversed" },                        // Latin Capital Letter Ezh Reversed
    { 0x01B9, "ezhreversed" },                        // Latin small letter ezh reversed
    { 0x01BA, "ezhtail" },                            // Latin small letter ezh with tail
    { 0x01BB, "twostroke" },                          // Latin letter two with stroke
    { 0x01BC, "Tonefive" },                           // Latin Capital Letter Tone Five
    { 0x01BD, "tonefive" },                           // Latin small letter tone five
    { 0x01BE, "glottalinvertedstroke" },              // Latin letter inverted glottal stop with stroke
    { 0x01BF, "wynn" },                               // Latin letter wynn
    { 0x01C0, "clickdental" },                        // Latin letter dental click
    { 0x01C1, "clicklateral" },                       // Latin letter lateral click
    { 0x01C2, "clickalveolar" },                      // Latin letter alveolar click
    { 0x01C3, "clickretroflex" },                     // Latin letter retroflex click
    { 0x01C4, "DZcaron" },                            // Latin Capital Letter Dz With Caron
    { 0x01C5, "Dzcaron" },                            // Latin Capital Letter D With small letter z with caron
    { 0x01C6, "dzcaron" },                            // Latin small letter dz with caron
    { 0x01C7, "LJ" },                                 // Latin Capital Letter Lj
    { 0x01C8, "Lj" },                                 // Latin Capital Letter L With small letter j
    { 0x01C9, "lj" },                                 // Latin small letter lj
    { 0x01CA, "NJ" },                                 // Latin Capital Letter Nj
    { 0x01CB, "Nj" },                                 // Latin Capital Letter N With small letter j
    { 0x01CC, "nj" },                                 // Latin small letter nj
    { 0x01CD, "Acaron" },                             // Latin Capital Letter A With Caron
    { 0x01CE, "acaron" },                             // Latin small letter a with caron
    { 0x01CF, "Icaron" },                             // Latin Capital Letter I With Caron
    { 0x01D0, "icaron" },                             // Latin small letter i with caron
    { 0x01D1, "Ocaron" },                             // Latin Capital Letter O With Caron
    { 0x01D2, "ocaron" },                             // Latin small letter o with caron
    { 0x01D3, "Ucaron" },                             // Latin Capital Letter U With Caron
    { 0x01D4, "ucaron" },                             // Latin small letter u with caron
    { 0x01D5, "Udieresismacron" },                    // Latin Capital Letter U With Diaeresis And Macron
    { 0x01D6, "udieresismacron" },                    // Latin small letter u with diaeresis and macron
    { 0x01D7, "Udieresisacute" },                     // Latin Capital Letter U With Diaeresis And Acute
    { 0x01D8, "udieresisacute" },                     // Latin small letter u with diaeresis and acute
    { 0x01D9, "Udieresiscaron" },                     // Latin Capital Letter U With Diaeresis And Caron
    { 0x01DA, "udieresiscaron" },                     // Latin small letter u with diaeresis and caron
    { 0x01DB, "Udieresisgrave" },                     // Latin Capital Letter U With Diaeresis And Grave
    { 0x01DC, "udieresisgrave" },                     // Latin small letter u with diaeresis and grave
    { 0x01DD, "eturn" },                              // Latin small letter turned e
    { 0x01DE, "Adieresismacron" },                    // Latin Capital Letter A With Diaeresis And Macron
    { 0x01DF, "adieresismacron" },                    // Latin small letter a with diaeresis and macron
    { 0x01E0, "Adotmacron" },                         // Latin Capital Letter A With Dot Above And Macron
    { 0x01E1, "adotmacron" },                         // Latin small letter a with dot above and macron
    { 0x01E2, "AEmacron" },                           // Latin Capital Letter Ae With Macron
    { 0x01E3, "aemacron" },                           // Latin small letter ae with macron
    { 0x01E4, "Gstroke" },                            // Latin Capital Letter G With Stroke
    { 0x01E5, "gbar" },                               // Latin small letter g with stroke
    { 0x01E6, "Gcaron" },                             // Latin Capital Letter G With Caron
    { 0x01E7, "gcaron" },                             // Latin small letter g with caron
    { 0x01E8, "Kcaron" },                             // Latin Capital Letter K With Caron
    { 0x01E9, "kcaron" },                             // Latin small letter k with caron
    { 0x01EA, "Oogonek" },                            // Latin Capital Letter O With Ogonek
    { 0x01EB, "oogonek" },                            // Latin small letter o with ogonek
    { 0x01EC, "Oogonekmacron" },                      // Latin Capital Letter O With Ogonek And Macron
    { 0x01ED, "oogonekmacron" },                      // Latin small letter o with ogonek and macron
    { 0x01EE, "Ezhcaron" },                           // Latin Capital Letter Ezh With Caron
    { 0x01EF, "ezhcaron" },                           // Latin small letter ezh with caron
    { 0x01F0, "jcaron" },                             // Latin small letter j with caron
    { 0x01F1, "DZ" },                                 // Latin Capital Letter Dz
    { 0x01F2, "Dz" },                                 // Latin Capital Letter D With small letter z
    { 0x01F3, "dz" },                                 // Latin small letter dz
    { 0x01F4, "Gacute" },                             // Latin Capital Letter G With Acute
    { 0x01F5, "gacute" },                             // Latin small letter g with acute
    { 0x01F8, "Ngrave" },                             // Latin Capital Letter N With Grave
    { 0x01F9, "ngrave" },                             // Latin small letter n with grave
    { 0x01FA, "Aringacute" },                         // Latin Capital Letter A With Ring Above And Acute
    { 0x01FB, "aringacute" },                         // Latin small letter a with ring above and acute
    { 0x01FC, "AEacute" },                            // Latin Capital Letter Ae With Acute
    { 0x01FD, "aeacute" },                            // Latin small letter ae with acute
    { 0x01FE, "Ostrokeacute" },                       // Latin Capital Letter O With Stroke And Acute
    { 0x01FF, "ostrokeacute" },                       // Latin small letter o with stroke and acute
    { 0x0200, "Adblgrave" },                          // Latin Capital Letter A With Double Grave
    { 0x0201, "adblgrave" },                          // Latin small letter a with double grave
    { 0x0202, "Ainvbreve" },                          // Latin Capital Letter A With Inverted Breve
    { 0x0203, "ainvbreve" },                          // Latin small letter a with inverted breve
    { 0x0204, "Edblgrave" },                          // Latin Capital Letter E With Double Grave
    { 0x0205, "edblgrave" },                          // Latin small letter e with double grave
    { 0x0206, "Einvbreve" },                          // Latin Capital Letter E With Inverted Breve
    { 0x0207, "einvbreve" },                          // Latin small letter e with inverted breve
    { 0x0208, "Idblgrave" },                          // Latin Capital Letter I With Double Grave
    { 0x0209, "idblgrave" },                          // Latin small letter i with double grave
    { 0x020A, "Iinvbreve" },                          // Latin Capital Letter I With Inverted Breve
    { 0x020B, "iinvbreve" },                          // Latin small letter i with inverted breve
    { 0x020C, "Odblgrave" },                          // Latin Capital Letter O With Double Grave
    { 0x020D, "odblgrave" },                          // Latin small letter o with double grave
    { 0x020E, "Oinvbreve" },                          // Latin Capital Letter O With Inverted Breve
    { 0x020F, "oinvbreve" },                          // Latin small letter o with inverted breve
    { 0x0210, "Rdblgrave" },                          // Latin Capital Letter R With Double Grave
    { 0x0211, "rdblgrave" },                          // Latin small letter r with double grave
    { 0x0212, "Rinvbreve" },                          // Latin Capital Letter R With Inverted Breve
    { 0x0213, "rinvbreve" },                          // Latin small letter r with inverted breve
    { 0x0214, "Udblgrave" },                          // Latin Capital Letter U With Double Grave
    { 0x0215, "udblgrave" },                          // Latin small letter u with double grave
    { 0x0216, "Uinvbreve" },                          // Latin Capital Letter U With Inverted Breve
    { 0x0217, "uinvbreve" },                          // Latin small letter u with inverted breve
    { 0x0218, "Scommasub" },                          // Latin Capital Letter S With Comma Below
    { 0x0219, "scommasub" },                          // Latin small letter s with comma below
    { 0x0228, "Ecedilla" },                           // Latin Capital Letter E With Cedilla
    { 0x0229, "ecedilla" },                           // Latin small letter e with cedilla
    { 0x022A, "Odieresismacron" },                    // Latin Capital Letter O With Diaeresis And Macron
    { 0x022B, "odieresismacron" },                    // Latin small letter o with diaeresis and macron
    { 0x022C, "Otildemacron" },                       // Latin Capital Letter O With Tilde And Macron
    { 0x022D, "otildemacron" },                       // Latin small letter o with tilde and macron
    { 0x0230, "Odotmacron" },                         // Latin Capital Letter O With Dot Above And Macron
    { 0x0231, "odotmacron" },                         // Latin small letter o with dot above and macron
    { 0x0232, "Ymacron" },                            // Latin Capital Letter Y With Macron
    { 0x0233, "ymacron" },                            // Latin small letter y with macron
    { 0x0250, "aturn" },                              // Latin small letter turned a
    { 0x0251, "ascript" },                            // Latin small letter alpha
    { 0x0252, "ascriptturn" },                        // Latin small letter turned alpha
    { 0x0253, "bhook" },                              // Latin small letter b with hook
    { 0x0254, "cturn" },                              // Latin small letter open o
    { 0x0255, "ccurl" },                              // Latin small letter c with curl
    { 0x0256, "dtail" },                              // Latin small letter d with tail
    { 0x0257, "dhook" },                              // Latin small letter d with hook
    { 0x0258, "erev" },                               // Latin small letter reversed e
    { 0x0259, "schwa" },                              // Latin small letter schwa
    { 0x025A, "schwahook" },                          // Latin small letter schwa with hook
    { 0x025B, "eopen" },                              // Latin small letter open e
    { 0x025C, "eopenreversed" },                      // Latin small letter reversed open e
    { 0x025D, "eopenreversedhook" },                  // Latin small letter reversed open e with hook
    { 0x025E, "eopenreversedclosed" },                // Latin small letter closed reversed open e
    { 0x025F, "jdotlessstroke" },                     // Latin small letter dotless j with stroke
    { 0x0260, "ghook" },                              // Latin small letter g with hook
    { 0x0261, "gcursive" },                           // Latin small letter script g
    { 0x0262, "Gsmallcap" },                          // Latin letter small Capital G
    { 0x0263, "gammalatinsmall" },                    // Latin small letter gamma
    { 0x0264, "ramshorn" },                           // Latin small letter rams horn
    { 0x0265, "hturn" },                              // Latin small letter turned h
    { 0x0266, "hhook" },                              // Latin small letter h with hook
    { 0x0267, "henghook" },                           // Latin small letter heng with hook
    { 0x0268, "istroke" },                            // Latin small letter i with stroke
    { 0x0269, "iotalatin" },                          // Latin small letter iota
    { 0x026A, "Ismallcap" },                          // Latin letter small Capital I
    { 0x026B, "lmidtilde" },                          // Latin small letter l with middle tilde
    { 0x026C, "lbelt" },                              // Latin small letter l with belt
    { 0x026D, "lrthook" },                            // Latin small letter l with retroflex hook
    { 0x026E, "lezh" },                               // Latin small letter lezh
    { 0x026F, "mturn" },                              // Latin small letter turned m
    { 0x0270, "mlonglegturned" },                     // Latin small letter turned m with long leg
    { 0x0271, "mhook" },                              // Latin small letter m with hook
    { 0x0272, "nlftlfthook" },                        // Latin small letter n with left hook
    { 0x0273, "nrthook" },                            // Latin small letter n with retroflex hook
    { 0x0274, "Nsmallcap" },                          // Latin letter small Capital N
    { 0x0275, "obar" },                               // Latin small letter barred o
    { 0x0276, "OEsmallcap" },                         // Latin letter small Capital Oe
    { 0x0277, "omegaclosed" },                        // Latin small letter closed omega
    { 0x0278, "philatin" },                           // Latin small letter phi
    { 0x0279, "rturn" },                              // Latin small letter turned r
    { 0x027A, "rlonglegturned" },                     // Latin small letter turned r with long leg
    { 0x027B, "rturnrthook" },                        // Latin small letter turned r with hook
    { 0x027C, "rlongleg" },                           // Latin small letter r with long leg
    { 0x027D, "rhook" },                              // Latin small letter r with tail
    { 0x027E, "rfishhook" },                          // Latin small letter r with fishhook
    { 0x027F, "rfishhookrev" },                       // Latin small letter reversed r with fishhook
    { 0x0280, "Rsmallcap" },                          // Latin letter small Capital R
    { 0x0281, "Rsmallinverted" },                     // Latin letter small Capital Inverted R
    { 0x0282, "shook" },                              // Latin small letter s with hook
    { 0x0283, "esh" },                                // Latin small letter esh
    { 0x0284, "jhookdblbar" },                        // Latin small letter dotless j with stroke and hook
    { 0x0285, "eshshortrev" },                        // Latin small letter squat reversed esh
    { 0x0286, "eshcurl" },                            // Latin small letter esh with curl
    { 0x0287, "tturn" },                              // Latin small letter turned t
    { 0x0288, "trthook" },                            // Latin small letter t with retroflex hook
    { 0x0289, "ubar" },                               // Latin small letter u bar
    { 0x028A, "upsilonlatin" },                       // Latin small letter upsilon
    { 0x028B, "vhook" },                              // Latin small letter v with hook
    { 0x028C, "vturn" },                              // Latin small letter turned v
    { 0x028D, "wturn" },                              // Latin small letter turned w
    { 0x028E, "yturn" },                              // Latin small letter turned y
    { 0x028F, "Ysmallcap" },                          // Latin letter small Capital Y
    { 0x0290, "zrthook" },                            // Latin small letter z with retroflex hook
    { 0x0291, "zcurl" },                              // Latin small letter z with curl
    { 0x0292, "ezh" },                                // Latin small letter ezh
    { 0x0293, "ezhcurl" },                            // Latin small letter ezh with curl
    { 0x0294, "glottalstop" },                        // Latin letter glottal stop
    { 0x0295, "glottalstoprevinv" },                  // Latin letter pharyngeal voiced fricative
    { 0x0296, "glottalstopinv" },                     // Latin letter inverted glottal stop
    { 0x0297, "cstretch" },                           // Latin letter stretched c
    { 0x0298, "bullseye" },                           // Latin letter bilabial click
    { 0x0299, "Bsmallcap" },                          // Latin letter small Capital B
    { 0x029A, "eopenclosed" },                        // Latin small letter closed open e
    { 0x029B, "Gsmallhook" },                         // Latin letter small Capital G With Hook
    { 0x029C, "Hsmallcap" },                          // Latin letter small Capital H
    { 0x029D, "jcrosstail" },                         // Latin small letter j with crossed-tail
    { 0x029E, "kturn" },                              // Latin small letter turned k
    { 0x029F, "Lsmallcap" },                          // Latin letter small Capital L
    { 0x02A0, "qhook" },                              // Latin small letter q with hook
    { 0x02A1, "glottalstopbar" },                     // Latin letter glottal stop with stroke
    { 0x02A2, "glottalstopbarrev" },                  // Latin letter reversed glottal stop with stroke
    { 0x02A3, "dzaltone" },                           // Latin small letter dz digraph
    { 0x02A4, "dezh" },                               // Latin small letter dezh digraph
    { 0x02A5, "dzcurl" },                             // Latin small letter dz digraph with curl
    { 0x02A6, "ts" },                                 // Latin small letter ts digraph
    { 0x02A7, "tesh" },                               // Latin small letter tesh digraph
    { 0x02A8, "tccurl" },                             // Latin small letter tc digraph with curl
    { 0x02AA, "finalkaf" },                           // Latin small letter ls digraph
    { 0x02AD, "finalmem" },                           // Latin letter bidental percussive
    { 0x02AF, "finalnun" },                           // Latin small letter turned h with fishhook and tail
    { 0x02B0, "hsuper" },                             // Modifier letter small h
    { 0x02B1, "hhooksuper" },                         // Modifier letter small h with hook
    { 0x02B2, "jsuper" },                             // Modifier letter small j
    { 0x02B3, "rsuper" },                             // Modifier letter small r
    { 0x02B4, "rturnsuper" },                         // Modifier letter small turned r
    { 0x02B5, "rturnrthooksuper" },                   // Modifier letter small turned r with hook
    { 0x02B6, "Rturnsuper" },                         // Modifier letter small Capital Inverted R
    { 0x02B7, "wsuper" },                             // Modifier letter small w
    { 0x02B8, "ysuper" },                             // Modifier letter small y
    { 0x02B9, "primemod" },                           // Modifier letter prime
    { 0x02BA, "dblprimemod" },                        // Modifier letter double prime
    { 0x02BB, "quoteleftmod" },                       // Modifier letter turned comma
    { 0x02BC, "apostrophe" },                         // Modifier letter apostrophe
    { 0x02BD, "commareversedmod" },                   // Modifier letter reversed comma
    { 0x02BE, "ringrighthalfsuper" },                 // Modifier letter right half ring
    { 0x02BF, "ringlefthalfsuper" },                  // Modifier letter left half ring
    { 0x02C0, "glottal" },                            // Modifier letter glottal stop
    { 0x02C1, "glottalrev" },                         // Modifier letter reversed glottal stop
    { 0x02C2, "arrowheadleftmod" },                   // Modifier letter left arrowhead
    { 0x02C3, "arrowheadrightmod" },                  // Modifier letter right arrowhead
    { 0x02C4, "arrowheadupmod" },                     // Modifier letter up arrowhead
    { 0x02C5, "arrowheaddownmod" },                   // Modifier letter down arrowhead
    { 0x02C6, "circumflex" },                         // Modifier letter circumflex accent
    { 0x02C7, "caron" },                              // Caron
    { 0x02C8, "linevert" },                           // Modifier letter vertical line
    { 0x02C9, "macronmod" },                          // Modifier letter macron
    { 0x02CA, "acuteaccent" },                        // Modifier letter acute accent
    { 0x02CB, "grave1" },                             // Modifier letter grave accent
    { 0x02CC, "linevertsub" },                        // Modifier letter low vertical line
    { 0x02CD, "macronsubmod" },                       // Modifier letter low macron
    { 0x02CE, "gravesub" },                           // Modifier letter low grave accent
    { 0x02CF, "acutesub" },                           // Modifier letter low acute accent
    { 0x02D0, "colontriangularmod" },                 // Modifier letter triangular colon
    { 0x02D1, "colontriangularhalfmod" },             // Modifier letter half triangular colon
    { 0x02D2, "ringrighthalfcenter" },                // Modifier letter centred right half ring
    { 0x02D3, "ringlefthalfsup" },                    // Modifier letter centred left half ring
    { 0x02D4, "tackupmid" },                          // Modifier letter up tack
    { 0x02D5, "tackdownmid" },                        // Modifier letter down tack
    { 0x02D6, "plusmod" },                            // Modifier letter plus sign
    { 0x02D7, "minusmod" },                           // Modifier letter minus sign
    { 0x02D8, "breve" },                              // Breve
    { 0x02D9, "dotaccent" },                          // Dot above
    { 0x02DA, "ring" },                               // Ring above
    { 0x02DB, "ogonek" },                             // Ogonek
    { 0x02DC, "tilde" },                              // Small tilde
    { 0x02DD, "hungarumlaut" },                       // Double acute accent
    { 0x02DE, "rhotichook" },                         // Modifier letter rhotic hook
    { 0x02DF, "qofdagesh" },                          // Modifier letter cross accent
    { 0x02E0, "gammasuper" },                         // Modifier letter small gamma
    { 0x02E1, "lsuper" },                             // Modifier letter small l
    { 0x02E2, "ssuper" },                             // Modifier letter small s
    { 0x02E3, "xsuper" },                             // Modifier letter small x
    { 0x02E4, "glottalrevsuper" },                    // Modifier letter small reversed glottal stop
    { 0x02E5, "toneextrahigh" },                      // Modifier letter extra-high tone bar
    { 0x02E6, "tonehigh" },                           // Modifier letter high tone bar
    { 0x02E7, "tonemid" },                            // Modifier letter mid tone bar
    { 0x02E8, "tonelow" },                            // Modifier letter low tone bar
    { 0x02E9, "toneextralow" },                       // Modifier letter extra-low tone bar
    { 0x02EA, "lefttorightmark" },                    // Modifier letter yin departing tone mark
    { 0x02EB, "righttoleftmark" },                    // Modifier letter yang departing tone mark
    { 0x0370, "Heta" },                               // Greek Capital Letter Heta
    { 0x0371, "heta" },                               // Greek small letter heta
    { 0x0372, "Sampi" },                              // Greek Capital Letter Archaic Sampi
    { 0x0373, "sampi" },                              // Greek small letter archaic sampi
    { 0x0374, "numeralgreek" },                       // Greek numeral sign
    { 0x0375, "numeralgreeksub" },                    // Greek lower numeral sign
    { 0x0376, "Pamphylian" },                         // Greek Capital Letter Pamphylian Digamma
    { 0x0377, "pamphylian" },                         // Greek small letter pamphylian digamma
    { 0x037A, "iotasub" },                            // Greek ypogegrammeni
    { 0x037E, "questiongreek" },                      // Greek question mark
    { 0x0384, "tonos" },                              // Greek tonos
    { 0x0385, "dialytikatonos" },                     // Greek dialytika tonos
    { 0x0386, "Alphaacute" },                         // Greek Capital Letter Alpha With Tonos
    { 0x0387, "anoteleia" },                          // Greek ano teleia
    { 0x0388, "Epsilonacute" },                       // Greek Capital Letter Epsilon With Tonos
    { 0x0389, "Etaacute" },                           // Greek Capital Letter Eta With Tonos
    { 0x038A, "Iotaacute" },                          // Greek Capital Letter Iota With Tonos
    { 0x038C, "Omicronacute" },                       // Greek Capital Letter Omicron With Tonos
    { 0x038E, "Upsilonacute" },                       // Greek Capital Letter Upsilon With Tonos
    { 0x038F, "Omegaacute" },                         // Greek Capital Letter Omega With Tonos
    { 0x0390, "iotaacutedieresis" },                  // Greek small letter iota with dialytika and tonos
    { 0x0391, "Alpha" },                              // Greek Capital Letter Alpha
    { 0x0392, "Beta" },                               // Greek Capital Letter Beta
    { 0x0393, "Gamma" },                              // Greek Capital Letter Gamma
    { 0x0394, "Delta" },                              // Greek Capital Letter Delta
    { 0x0395, "Epsilon" },                            // Greek Capital Letter Epsilon
    { 0x0396, "Zeta" },                               // Greek Capital Letter Zeta
    { 0x0397, "Eta" },                                // Greek Capital Letter Eta
    { 0x0398, "Theta" },                              // Greek Capital Letter Theta
    { 0x0399, "Iota" },                               // Greek Capital Letter Iota
    { 0x039A, "Kappa" },                              // Greek Capital Letter Kappa
    { 0x039B, "Lambda" },                             // Greek Capital Letter Lamda
    { 0x039C, "Mu" },                                 // Greek Capital Letter Mu
    { 0x039D, "Nu" },                                 // Greek Capital Letter Nu
    { 0x039E, "Xi" },                                 // Greek Capital Letter Xi
    { 0x039F, "Omicron" },                            // Greek Capital Letter Omicron
    { 0x03A0, "Pi" },                                 // Greek Capital Letter Pi
    { 0x03A1, "Rho" },                                // Greek Capital Letter Rho
    { 0x03A3, "Sigma" },                              // Greek Capital Letter Sigma
    { 0x03A4, "Tau" },                                // Greek Capital Letter Tau
    { 0x03A5, "Upsilon" },                            // Greek Capital Letter Upsilon
    { 0x03A6, "Phi" },                                // Greek Capital Letter Phi
    { 0x03A7, "Chi" },                                // Greek Capital Letter Chi
    { 0x03A8, "Psi" },                                // Greek Capital Letter Psi
    { 0x03A9, "Omega" },                              // Greek Capital Letter Omega
    { 0x03AA, "Iotadieresis" },                       // Greek Capital Letter Iota With Dialytika
    { 0x03AB, "Upsilondieresis" },                    // Greek Capital Letter Upsilon With Dialytika
    { 0x03AC, "alphaacute" },                         // Greek small letter alpha with tonos
    { 0x03AD, "epsilonacute" },                       // Greek small letter epsilon with tonos
    { 0x03AE, "etaacute" },                           // Greek small letter eta with tonos
    { 0x03AF, "iotaacute" },                          // Greek small letter iota with tonos
    { 0x03B0, "upsilonacutedieresis" },               // Greek small letter upsilon with dialytika and tonos
    { 0x03B1, "alpha" },                              // Greek small letter alpha
    { 0x03B2, "beta" },                               // Greek small letter beta
    { 0x03B3, "gamma" },                              // Greek small letter gamma
    { 0x03B4, "delta" },                              // Greek small letter delta
    { 0x03B5, "epsilon" },                            // Greek small letter epsilon
    { 0x03B6, "zeta" },                               // Greek small letter zeta
    { 0x03B7, "eta" },                                // Greek small letter eta
    { 0x03B8, "theta" },                              // Greek small letter theta
    { 0x03B9, "iota" },                               // Greek small letter iota
    { 0x03BA, "kappa" },                              // Greek small letter kappa
    { 0x03BB, "lambda" },                             // Greek small letter lamda
    { 0x03BC, "mugreek" },                            // Greek small letter mu
    { 0x03BD, "nu" },                                 // Greek small letter nu
    { 0x03BE, "xi" },                                 // Greek small letter xi
    { 0x03BF, "omicron" },                            // Greek small letter omicron
    { 0x03C0, "pi" },                                 // Greek small letter pi
    { 0x03C1, "rho" },                                // Greek small letter rho
    { 0x03C2, "sigma1" },                             // Greek small letter final sigma
    { 0x03C3, "sigma" },                              // Greek small letter sigma
    { 0x03C4, "tau" },                                // Greek small letter tau
    { 0x03C5, "upsilon" },                            // Greek small letter upsilon
    { 0x03C6, "phi" },                                // Greek small letter phi
    { 0x03C7, "chi" },                                // Greek small letter chi
    { 0x03C8, "psi" },                                // Greek small letter psi
    { 0x03C9, "omega" },                              // Greek small letter omega
    { 0x03CA, "iotadieresis" },                       // Greek small letter iota with dialytika
    { 0x03CB, "upsilondieresis" },                    // Greek small letter upsilon with dialytika
    { 0x03CC, "omicronacute" },                       // Greek small letter omicron with tonos
    { 0x03CD, "upsilonacute" },                       // Greek small letter upsilon with tonos
    { 0x03CE, "omegaacute" },                         // Greek small letter omega with tonos
    { 0x03CF, "Kai" },                                // Greek Capital Kai Symbol
    { 0x03D0, "beta1" },                              // Greek beta symbol
    { 0x03D1, "theta1" },                             // Greek theta symbol
    { 0x03D2, "Upsilon1" },                           // Greek upsilon with hook symbol
    { 0x03D3, "Upsilon1tonos" },                      // Greek upsilon with acute and hook symbol
    { 0x03D4, "Upsilon1dieresis" },                   // Greek upsilon with diaeresis and hook symbol
    { 0x03D5, "phi1" },                               // Greek phi symbol
    { 0x03D6, "pi1" },                                // Greek pi symbol
    { 0x03DA, "Stigma" },                             // Greek letter stigma
    { 0x03DC, "Digamma" },                            // Greek letter digamma
    { 0x03DE, "Koppa" },                              // Greek letter koppa
    { 0x03E0, "Sampigreek" },                         // Greek letter sampi
    { 0x03E2, "Sheicoptic" },                         // Coptic Capital Letter Shei
    { 0x03E3, "sheicoptic" },                         // Coptic small letter shei
    { 0x03E4, "Feicoptic" },                          // Coptic Capital Letter Fei
    { 0x03E5, "feicoptic" },                          // Coptic small letter fei
    { 0x03E6, "Kheicoptic" },                         // Coptic Capital Letter Khei
    { 0x03E7, "kheicoptic" },                         // Coptic small letter khei
    { 0x03E8, "Horicoptic" },                         // Coptic Capital Letter Hori
    { 0x03E9, "horicoptic" },                         // Coptic small letter hori
    { 0x03EA, "Gangiacoptic" },                       // Coptic Capital Letter Gangia
    { 0x03EB, "gangiacoptic" },                       // Coptic small letter gangia
    { 0x03EC, "Shimacoptic" },                        // Coptic Capital Letter Shima
    { 0x03ED, "shimacoptic" },                        // Coptic small letter shima
    { 0x03EE, "Dei" },                                // Coptic Capital Letter Dei
    { 0x03EF, "dei" },                                // Coptic small letter dei
    { 0x03F0, "kappa1" },                             // Greek kappa symbol
    { 0x03F1, "rho1" },                               // Greek rho symbol
    { 0x03F2, "sigmalunate" },                        // Greek lunate sigma symbol
    { 0x03F3, "yotgreek" },                           // Greek letter yot
    { 0x0401, "Io" },                                 // Cyrillic Capital Letter Io
    { 0x0402, "Dje" },                                // Cyrillic Capital Letter Dje
    { 0x0403, "Gje" },                                // Cyrillic Capital Letter Gje
    { 0x0404, "Ecyril" },                             // Cyrillic Capital Letter Ukrainian Ie
    { 0x0405, "Dze" },                                // Cyrillic Capital Letter Dze
    { 0x0406, "Icyril" },                             // Cyrillic Capital Letter Byelorussian-ukrainian I
    { 0x0407, "Yi" },                                 // Cyrillic Capital Letter Yi
    { 0x0408, "Je" },                                 // Cyrillic Capital Letter Je
    { 0x0409, "Lje" },                                // Cyrillic Capital Letter Lje
    { 0x040A, "Nje" },                                // Cyrillic Capital Letter Nje
    { 0x040B, "Tshe" },                               // Cyrillic Capital Letter Tshe
    { 0x040C, "Kje" },                                // Cyrillic Capital Letter Kje
    { 0x040E, "Ucyrilbreve" },                        // Cyrillic Capital Letter Short U
    { 0x040F, "Dzhe" },                               // Cyrillic Capital Letter Dzhe
    { 0x0410, "Acyril" },                             // Cyrillic Capital Letter A
    { 0x0411, "Be" },                                 // Cyrillic Capital Letter Be
    { 0x0412, "Ve" },                                 // Cyrillic Capital Letter Ve
    { 0x0413, "Ge" },                                 // Cyrillic Capital Letter Ghe
    { 0x0414, "De" },                                 // Cyrillic Capital Letter De
    { 0x0415, "Ie" },                                 // Cyrillic Capital Letter Ie
    { 0x0416, "Zhe" },                                // Cyrillic Capital Letter Zhe
    { 0x0417, "Ze" },                                 // Cyrillic Capital Letter Ze
    { 0x0418, "Ii" },                                 // Cyrillic Capital Letter I
    { 0x0419, "Iibreve" },                            // Cyrillic Capital Letter Short I
    { 0x041A, "Ka" },                                 // Cyrillic Capital Letter Ka
    { 0x041B, "El" },                                 // Cyrillic Capital Letter El
    { 0x041C, "Em" },                                 // Cyrillic Capital Letter Em
    { 0x041D, "En" },                                 // Cyrillic Capital Letter En
    { 0x041E, "Ocyril" },                             // Cyrillic Capital Letter O
    { 0x041F, "Pecyril" },                            // Cyrillic Capital Letter Pe
    { 0x0420, "Er" },                                 // Cyrillic Capital Letter Er
    { 0x0421, "Es" },                                 // Cyrillic Capital Letter Es
    { 0x0422, "Te" },                                 // Cyrillic Capital Letter Te
    { 0x0423, "Ucyril" },                             // Cyrillic Capital Letter U
    { 0x0424, "Ef" },                                 // Cyrillic Capital Letter Ef
    { 0x0425, "Kha" },                                // Cyrillic Capital Letter Ha
    { 0x0426, "Tse" },                                // Cyrillic Capital Letter Tse
    { 0x0427, "Che" },                                // Cyrillic Capital Letter Che
    { 0x0428, "Sha" },                                // Cyrillic Capital Letter Sha
    { 0x0429, "Shcha" },                              // Cyrillic Capital Letter Shcha
    { 0x042A, "Hard" },                               // Cyrillic Capital Letter Hard Sign
    { 0x042B, "Yeri" },                               // Cyrillic Capital Letter Yeru
    { 0x042C, "Soft" },                               // Cyrillic Capital Letter Soft Sign
    { 0x042D, "Ecyrilrev" },                          // Cyrillic Capital Letter E
    { 0x042E, "Iu" },                                 // Cyrillic Capital Letter Yu
    { 0x042F, "Ia" },                                 // Cyrillic Capital Letter Ya
    { 0x0430, "acyril" },                             // Cyrillic small letter a
    { 0x0431, "be" },                                 // Cyrillic small letter be
    { 0x0432, "ve" },                                 // Cyrillic small letter ve
    { 0x0433, "ge" },                                 // Cyrillic small letter ghe
    { 0x0434, "de" },                                 // Cyrillic small letter de
    { 0x0435, "ie" },                                 // Cyrillic small letter ie
    { 0x0436, "zhe" },                                // Cyrillic small letter zhe
    { 0x0437, "ze" },                                 // Cyrillic small letter ze
    { 0x0438, "ii" },                                 // Cyrillic small letter i
    { 0x0439, "iibreve" },                            // Cyrillic small letter short i
    { 0x043A, "ka" },                                 // Cyrillic small letter ka
    { 0x043B, "el" },                                 // Cyrillic small letter el
    { 0x043C, "em" },                                 // Cyrillic small letter em
    { 0x043D, "en" },                                 // Cyrillic small letter en
    { 0x043E, "ocyril" },                             // Cyrillic small letter o
    { 0x043F, "pecyril" },                            // Cyrillic small letter pe
    { 0x0440, "er" },                                 // Cyrillic small letter er
    { 0x0441, "es" },                                 // Cyrillic small letter es
    { 0x0442, "te" },                                 // Cyrillic small letter te
    { 0x0443, "ucyril" },                             // Cyrillic small letter u
    { 0x0444, "ef" },                                 // Cyrillic small letter ef
    { 0x0445, "kha" },                                // Cyrillic small letter ha
    { 0x0446, "tse" },                                // Cyrillic small letter tse
    { 0x0447, "che" },                                // Cyrillic small letter che
    { 0x0448, "sha" },                                // Cyrillic small letter sha
    { 0x0449, "shcha" },                              // Cyrillic small letter shcha
    { 0x044A, "hard" },                               // Cyrillic small letter hard sign
    { 0x044B, "yeri" },                               // Cyrillic small letter yeru
    { 0x044C, "soft" },                               // Cyrillic small letter soft sign
    { 0x044D, "ecyrilrev" },                          // Cyrillic small letter e
    { 0x044E, "iu" },                                 // Cyrillic small letter yu
    { 0x044F, "ia" },                                 // Cyrillic small letter ya
    { 0x0451, "io" },                                 // Cyrillic small letter io
    { 0x0452, "dje" },                                // Cyrillic small letter dje
    { 0x0453, "gje" },                                // Cyrillic small letter gje
    { 0x0454, "ecyril" },                             // Cyrillic small letter ukrainian ie
    { 0x0455, "dze" },                                // Cyrillic small letter dze
    { 0x0456, "icyril" },                             // Cyrillic small letter byelorussian-ukrainian i
    { 0x0457, "yi" },                                 // Cyrillic small letter yi
    { 0x0458, "je" },                                 // Cyrillic small letter je
    { 0x0459, "lje" },                                // Cyrillic small letter lje
    { 0x045A, "nje" },                                // Cyrillic small letter nje
    { 0x045B, "tshe" },                               // Cyrillic small letter tshe
    { 0x045C, "kje" },                                // Cyrillic small letter kje
    { 0x045E, "ucyrilbreve" },                        // Cyrillic small letter short u
    { 0x045F, "dzhe" },                               // Cyrillic small letter dzhe
    { 0x0460, "Omegacyrillic" },                      // Cyrillic Capital Letter Omega
    { 0x0461, "omegacyrillic" },                      // Cyrillic small letter omega
    { 0x0462, "Yat" },                                // Cyrillic Capital Letter Yat
    { 0x0463, "yat" },                                // Cyrillic small letter yat
    { 0x0464, "Eiotifiedcyrillic" },                  // Cyrillic Capital Letter Iotified E
    { 0x0465, "eiotifiedcyrillic" },                  // Cyrillic small letter iotified e
    { 0x0466, "Yuslittlecyrillic" },                  // Cyrillic Capital Letter Little Yus
    { 0x0467, "yuslittlecyrillic" },                  // Cyrillic small letter little yus
    { 0x0468, "Yuslittleiotifiedcyrillic" },          // Cyrillic Capital Letter Iotified Little Yus
    { 0x0469, "yuslittleiotifiedcyrillic" },          // Cyrillic small letter iotified little yus
    { 0x046A, "Yusbig" },                             // Cyrillic Capital Letter Big Yus
    { 0x046B, "yusbig" },                             // Cyrillic small letter big yus
    { 0x046C, "Yusbigiotifiedcyrillic" },             // Cyrillic Capital Letter Iotified Big Yus
    { 0x046D, "yusbigiotifiedcyrillic" },             // Cyrillic small letter iotified big yus
    { 0x046E, "Ksicyrillic" },                        // Cyrillic Capital Letter Ksi
    { 0x046F, "ksicyrillic" },                        // Cyrillic small letter ksi
    { 0x0470, "Psicyrillic" },                        // Cyrillic Capital Letter Psi
    { 0x0471, "psicyrillic" },                        // Cyrillic small letter psi
    { 0x0472, "Fitacyrillic" },                       // Cyrillic Capital Letter Fita
    { 0x0473, "fitacyrillic" },                       // Cyrillic small letter fita
    { 0x0474, "Izhitsacyrillic" },                    // Cyrillic Capital Letter Izhitsa
    { 0x0475, "izhitsacyrillic" },                    // Cyrillic small letter izhitsa
    { 0x0476, "Izhitsadblgravecyrillic" },            // Cyrillic Capital Letter Izhitsa With Double Grave Accent
    { 0x0477, "izhitsadblgravecyrillic" },            // Cyrillic small letter izhitsa with double grave accent
    { 0x0478, "Ukcyrillic" },                         // Cyrillic Capital Letter Uk
    { 0x0479, "ukcyrillic" },                         // Cyrillic small letter uk
    { 0x047A, "Omegaroundcyrillic" },                 // Cyrillic Capital Letter Round Omega
    { 0x047B, "omegaroundcyrillic" },                 // Cyrillic small letter round omega
    { 0x047C, "Omegatitlocyrillic" },                 // Cyrillic Capital Letter Omega With Titlo
    { 0x047D, "omegatitlocyrillic" },                 // Cyrillic small letter omega with titlo
    { 0x047E, "Otcyrillic" },                         // Cyrillic Capital Letter Ot
    { 0x047F, "otcyrillic" },                         // Cyrillic small letter ot
    { 0x0480, "Koppacyrillic" },                      // Cyrillic Capital Letter Koppa
    { 0x0481, "koppacyrillic" },                      // Cyrillic small letter koppa
    { 0x0482, "thousandcyrillic" },                   // Cyrillic thousands sign
    { 0x0490, "Geupturn" },                           // Cyrillic Capital Letter Ghe With Upturn
    { 0x0491, "geupturn" },                           // Cyrillic small letter ghe with upturn
    { 0x0492, "Gebar" },                              // Cyrillic Capital Letter Ghe With Stroke
    { 0x0493, "gebar" },                              // Cyrillic small letter ghe with stroke
    { 0x0494, "Gehook" },                             // Cyrillic Capital Letter Ghe With Middle Hook
    { 0x0495, "gehook" },                             // Cyrillic small letter ghe with middle hook
    { 0x0496, "Zhertdes" },                           // Cyrillic Capital Letter Zhe With Descender
    { 0x0497, "zhertdes" },                           // Cyrillic small letter zhe with descender
    { 0x0498, "Zecedilla" },                          // Cyrillic Capital Letter Ze With Descender
    { 0x0499, "zecedilla" },                          // Cyrillic small letter ze with descender
    { 0x049A, "Kartdes" },                            // Cyrillic Capital Letter Ka With Descender
    { 0x049B, "kartdes" },                            // Cyrillic small letter ka with descender
    { 0x049C, "Kavertbar" },                          // Cyrillic Capital Letter Ka With Vertical Stroke
    { 0x049D, "kavertbar" },                          // Cyrillic small letter ka with vertical stroke
    { 0x049E, "Kabar" },                              // Cyrillic Capital Letter Ka With Stroke
    { 0x049F, "kabar" },                              // Cyrillic small letter ka with stroke
    { 0x04A0, "GeKarev" },                            // Cyrillic Capital Letter Bashkir Ka
    { 0x04A1, "gekarev" },                            // Cyrillic small letter bashkir ka
    { 0x04A2, "Enrtdes" },                            // Cyrillic Capital Letter En With Descender
    { 0x04A3, "enrtdes" },                            // Cyrillic small letter en with descender
    { 0x04A4, "EnGe" },                               // Cyrillic Capital Ligature En Ghe
    { 0x04A5, "enge" },                               // Cyrillic small ligature en ghe
    { 0x04A6, "Pehook" },                             // Cyrillic Capital Letter Pe With Middle Hook
    { 0x04A7, "pehook" },                             // Cyrillic small letter pe with middle hook
    { 0x04A8, "Ohook" },                              // Cyrillic Capital Letter Abkhasian Ha
    { 0x04A9, "ohook" },                              // Cyrillic small letter abkhasian ha
    { 0x04AA, "Escedilla" },                          // Cyrillic Capital Letter Es With Descender
    { 0x04AB, "escedilla" },                          // Cyrillic small letter es with descender
    { 0x04AC, "Tertdes" },                            // Cyrillic Capital Letter Te With Descender
    { 0x04AD, "tertdes" },                            // Cyrillic small letter te with descender
    { 0x04AE, "Ustrt" },                              // Cyrillic Capital Letter Straight U
    { 0x04AF, "ustrt" },                              // Cyrillic small letter straight u
    { 0x04B0, "Ustrtbar" },                           // Cyrillic Capital Letter Straight U With Stroke
    { 0x04B1, "ustrtbar" },                           // Cyrillic small letter straight u with stroke
    { 0x04B2, "Khartdes" },                           // Cyrillic Capital Letter Ha With Descender
    { 0x04B3, "khartdes" },                           // Cyrillic small letter ha with descender
    { 0x04B4, "TeTse" },                              // Cyrillic Capital Ligature Te Tse
    { 0x04B5, "tetse" },                              // Cyrillic small ligature te tse
    { 0x04B6, "Chertdes" },                           // Cyrillic Capital Letter Che With Descender
    { 0x04B7, "chertdes" },                           // Cyrillic small letter che with descender
    { 0x04B8, "Chevertbar" },                         // Cyrillic Capital Letter Che With Vertical Stroke
    { 0x04B9, "chevertbar" },                         // Cyrillic small letter che with vertical stroke
    { 0x04BA, "Hcyril" },                             // Cyrillic Capital Letter Shha
    { 0x04BB, "hcyril" },                             // Cyrillic small letter shha
    { 0x04BC, "Iehook" },                             // Cyrillic Capital Letter Abkhasian Che
    { 0x04BD, "iehook" },                             // Cyrillic small letter abkhasian che
    { 0x04BE, "Iehookogonek" },                       // Cyrillic Capital Letter Abkhasian Che With Descender
    { 0x04BF, "iehookogonek" },                       // Cyrillic small letter abkhasian che with descender
    { 0x04C0, "Icyril1" },                            // Cyrillic letter palochka
    { 0x04C1, "Zhebreve" },                           // Cyrillic Capital Letter Zhe With Breve
    { 0x04C2, "zhebreve" },                           // Cyrillic small letter zhe with breve
    { 0x04C3, "Kahook" },                             // Cyrillic Capital Letter Ka With Hook
    { 0x04C4, "kahook" },                             // Cyrillic small letter ka with hook
    { 0x04C7, "Enhook" },                             // Cyrillic Capital Letter En With Hook
    { 0x04C8, "enhook" },                             // Cyrillic small letter en with hook
    { 0x04CB, "Cheleftdes" },                         // Cyrillic Capital Letter Khakassian Che
    { 0x04CC, "cheleftdes" },                         // Cyrillic small letter khakassian che
    { 0x04D0, "Abrevecyrillic" },                     // Cyrillic Capital Letter A With Breve
    { 0x04D1, "abrevecyrillic" },                     // Cyrillic small letter a with breve
    { 0x04D2, "Adieresiscyrillic" },                  // Cyrillic Capital Letter A With Diaeresis
    { 0x04D3, "adieresiscyrillic" },                  // Cyrillic small letter a with diaeresis
    { 0x04D4, "Aiecyrillic" },                        // Cyrillic Capital Ligature A Ie
    { 0x04D5, "aiecyrillic" },                        // Cyrillic small ligature a ie
    { 0x04D6, "Iebrevecyrillic" },                    // Cyrillic Capital Letter Ie With Breve
    { 0x04D7, "iebrevecyrillic" },                    // Cyrillic small letter ie with breve
    { 0x04D8, "Schwacyrillic" },                      // Cyrillic Capital Letter Schwa
    { 0x04D9, "schwacyrillic" },                      // Cyrillic small letter schwa
    { 0x04DA, "Schwadieresiscyrillic" },              // Cyrillic Capital Letter Schwa With Diaeresis
    { 0x04DB, "schwadieresiscyrillic" },              // Cyrillic small letter schwa with diaeresis
    { 0x04DC, "Zhedieresiscyrillic" },                // Cyrillic Capital Letter Zhe With Diaeresis
    { 0x04DD, "zhedieresiscyrillic" },                // Cyrillic small letter zhe with diaeresis
    { 0x04DE, "Zedieresiscyrillic" },                 // Cyrillic Capital Letter Ze With Diaeresis
    { 0x04DF, "zedieresiscyrillic" },                 // Cyrillic small letter ze with diaeresis
    { 0x04E0, "Dzeabkhasiancyrillic" },               // Cyrillic Capital Letter Abkhasian Dze
    { 0x04E1, "dzeabkhasiancyrillic" },               // Cyrillic small letter abkhasian dze
    { 0x04E2, "Imacroncyrillic" },                    // Cyrillic Capital Letter I With Macron
    { 0x04E3, "imacroncyrillic" },                    // Cyrillic small letter i with macron
    { 0x04E4, "Idieresiscyrillic" },                  // Cyrillic Capital Letter I With Diaeresis
    { 0x04E5, "idieresiscyrillic" },                  // Cyrillic small letter i with diaeresis
    { 0x04E6, "Odieresiscyrillic" },                  // Cyrillic Capital Letter O With Diaeresis
    { 0x04E7, "odieresiscyrillic" },                  // Cyrillic small letter o with diaeresis
    { 0x04E8, "Obarredcyrillic" },                    // Cyrillic Capital Letter Barred O
    { 0x04E9, "obarredcyrillic" },                    // Cyrillic small letter barred o
    { 0x04EA, "Obarreddieresiscyrillic" },            // Cyrillic Capital Letter Barred O With Diaeresis
    { 0x04EB, "obarreddieresiscyrillic" },            // Cyrillic small letter barred o with diaeresis
    { 0x04EE, "Umacroncyrillic" },                    // Cyrillic Capital Letter U With Macron
    { 0x04EF, "umacroncyrillic" },                    // Cyrillic small letter u with macron
    { 0x04F0, "Udieresiscyrillic" },                  // Cyrillic Capital Letter U With Diaeresis
    { 0x04F1, "udieresiscyrillic" },                  // Cyrillic small letter u with diaeresis
    { 0x04F2, "Uhungarumlautcyrillic" },              // Cyrillic Capital Letter U With Double Acute
    { 0x04F3, "uhungarumlautcyrillic" },              // Cyrillic small letter u with double acute
    { 0x04F4, "Chedieresiscyrillic" },                // Cyrillic Capital Letter Che With Diaeresis
    { 0x04F5, "chedieresiscyrillic" },                // Cyrillic small letter che with diaeresis
    { 0x04F8, "Yerudieresiscyrillic" },               // Cyrillic Capital Letter Yeru With Diaeresis
    { 0x04F9, "yerudieresiscyrillic" },               // Cyrillic small letter yeru with diaeresis
    { 0x0514, "Lha" },                                // Cyrillic Capital Letter Lha
    { 0x0515, "lha" },                                // Cyrillic small letter lha
    { 0x0516, "Rha" },                                // Cyrillic Capital Letter Rha
    { 0x0517, "rha" },                                // Cyrillic small letter rha
    { 0x0518, "Yae" },                                // Cyrillic Capital Letter Yae
    { 0x0519, "yae" },                                // Cyrillic small letter yae
    { 0x051A, "Qa" },                                 // Cyrillic Capital Letter Qa
    { 0x051B, "qa" },                                 // Cyrillic small letter qa
    { 0x051C, "We" },                                 // Cyrillic Capital Letter We
    { 0x051D, "we" },                                 // Cyrillic small letter we
    { 0x051E, "Kaaleut" },                            // Cyrillic Capital Letter Aleut Ka
    { 0x051F, "kaaleut" },                            // Cyrillic small letter aleut ka
    { 0x0520, "Elmidhook" },                          // Cyrillic Capital Letter El With Middle Hook
    { 0x0521, "elmidhook" },                          // Cyrillic small letter el with middle hook
    { 0x0522, "Enmidhook" },                          // Cyrillic Capital Letter En With Middle Hook
    { 0x0523, "enmidhook" },                          // Cyrillic small letter en with middle hook
    { 0x0531, "Aybarmenian" },                        // Armenian Capital Letter Ayb
    { 0x0532, "Benarmenian" },                        // Armenian Capital Letter Ben
    { 0x0533, "Gimarmenian" },                        // Armenian Capital Letter Gim
    { 0x0534, "Daarmenian" },                         // Armenian Capital Letter Da
    { 0x0535, "Echarmenian" },                        // Armenian Capital Letter Ech
    { 0x0536, "Zaarmenian" },                         // Armenian Capital Letter Za
    { 0x0537, "Eharmenian" },                         // Armenian Capital Letter Eh
    { 0x0538, "Etarmenian" },                         // Armenian Capital Letter Et
    { 0x0539, "Toarmenian" },                         // Armenian Capital Letter To
    { 0x053A, "Zhearmenian" },                        // Armenian Capital Letter Zhe
    { 0x053B, "Iniarmenian" },                        // Armenian Capital Letter Ini
    { 0x053C, "Liwnarmenian" },                       // Armenian Capital Letter Liwn
    { 0x053D, "Xeharmenian" },                        // Armenian Capital Letter Xeh
    { 0x053E, "Caarmenian" },                         // Armenian Capital Letter Ca
    { 0x053F, "Kenarmenian" },                        // Armenian Capital Letter Ken
    { 0x0540, "Hoarmenian" },                         // Armenian Capital Letter Ho
    { 0x0541, "Jaarmenian" },                         // Armenian Capital Letter Ja
    { 0x0542, "Ghadarmenian" },                       // Armenian Capital Letter Ghad
    { 0x0543, "Cheharmenian" },                       // Armenian Capital Letter Cheh
    { 0x0544, "Menarmenian" },                        // Armenian Capital Letter Men
    { 0x0545, "Yiarmenian" },                         // Armenian Capital Letter Yi
    { 0x0546, "Nowarmenian" },                        // Armenian Capital Letter Now
    { 0x0547, "Shaarmenian" },                        // Armenian Capital Letter Sha
    { 0x0548, "Voarmenian" },                         // Armenian Capital Letter Vo
    { 0x0549, "Chaarmenian" },                        // Armenian Capital Letter Cha
    { 0x054A, "Peharmenian" },                        // Armenian Capital Letter Peh
    { 0x054B, "Jheharmenian" },                       // Armenian Capital Letter Jheh
    { 0x054C, "Raarmenian" },                         // Armenian Capital Letter Ra
    { 0x054D, "Seharmenian" },                        // Armenian Capital Letter Seh
    { 0x054E, "Vewarmenian" },                        // Armenian Capital Letter Vew
    { 0x054F, "Tiwnarmenian" },                       // Armenian Capital Letter Tiwn
    { 0x0550, "Reharmenian" },                        // Armenian Capital Letter Reh
    { 0x0551, "Coarmenian" },                         // Armenian Capital Letter Co
    { 0x0552, "Yiwnarmenian" },                       // Armenian Capital Letter Yiwn
    { 0x0553, "Piwrarmenian" },                       // Armenian Capital Letter Piwr
    { 0x0554, "Keharmenian" },                        // Armenian Capital Letter Keh
    { 0x0555, "Oharmenian" },                         // Armenian Capital Letter Oh
    { 0x0556, "Feharmenian" },                        // Armenian Capital Letter Feh
    { 0x0559, "ringhalfleftarmenian" },               // Armenian modifier letter left half ring
    { 0x055A, "apostrophearmenian" },                 // Armenian apostrophe
    { 0x055B, "emphasismarkarmenian" },               // Armenian emphasis mark
    { 0x055C, "exclamarmenian" },                     // Armenian exclamation mark
    { 0x055D, "commaarmenian" },                      // Armenian comma
    { 0x055E, "questionarmenian" },                   // Armenian question mark
    { 0x055F, "abbreviationmarkarmenian" },           // Armenian abbreviation mark
    { 0x0561, "aybarmenian" },                        // Armenian small letter ayb
    { 0x0562, "benarmenian" },                        // Armenian small letter ben
    { 0x0563, "gimarmenian" },                        // Armenian small letter gim
    { 0x0564, "daarmenian" },                         // Armenian small letter da
    { 0x0565, "echarmenian" },                        // Armenian small letter ech
    { 0x0566, "zaarmenian" },                         // Armenian small letter za
    { 0x0567, "eharmenian" },                         // Armenian small letter eh
    { 0x0568, "etarmenian" },                         // Armenian small letter et
    { 0x0569, "toarmenian" },                         // Armenian small letter to
    { 0x056A, "zhearmenian" },                        // Armenian small letter zhe
    { 0x056B, "iniarmenian" },                        // Armenian small letter ini
    { 0x056C, "liwnarmenian" },                       // Armenian small letter liwn
    { 0x056D, "xeharmenian" },                        // Armenian small letter xeh
    { 0x056E, "caarmenian" },                         // Armenian small letter ca
    { 0x056F, "kenarmenian" },                        // Armenian small letter ken
    { 0x0570, "hoarmenian" },                         // Armenian small letter ho
    { 0x0571, "jaarmenian" },                         // Armenian small letter ja
    { 0x0572, "ghadarmenian" },                       // Armenian small letter ghad
    { 0x0573, "cheharmenian" },                       // Armenian small letter cheh
    { 0x0574, "menarmenian" },                        // Armenian small letter men
    { 0x0575, "yiarmenian" },                         // Armenian small letter yi
    { 0x0576, "nowarmenian" },                        // Armenian small letter now
    { 0x0577, "shaarmenian" },                        // Armenian small letter sha
    { 0x0578, "voarmenian" },                         // Armenian small letter vo
    { 0x0579, "chaarmenian" },                        // Armenian small letter cha
    { 0x057A, "peharmenian" },                        // Armenian small letter peh
    { 0x057B, "jheharmenian" },                       // Armenian small letter jheh
    { 0x057C, "raarmenian" },                         // Armenian small letter ra
    { 0x057D, "seharmenian" },                        // Armenian small letter seh
    { 0x057E, "vewarmenian" },                        // Armenian small letter vew
    { 0x057F, "tiwnarmenian" },                       // Armenian small letter tiwn
    { 0x0580, "reharmenian" },                        // Armenian small letter reh
    { 0x0581, "coarmenian" },                         // Armenian small letter co
    { 0x0582, "yiwnarmenian" },                       // Armenian small letter yiwn
    { 0x0583, "piwrarmenian" },                       // Armenian small letter piwr
    { 0x0584, "keharmenian" },                        // Armenian small letter keh
    { 0x0585, "oharmenian" },                         // Armenian small letter oh
    { 0x0586, "feharmenian" },                        // Armenian small letter feh
    { 0x0587, "echyiwnarmenian" },                    // Armenian small ligature ech yiwn
    { 0x0589, "periodarmenian" },                     // Armenian full stop
    { 0x0591, "etnahtahebrew" },                      // Hebrew accent etnahta
    { 0x0592, "segoltahebrew" },                      // Hebrew accent segol
    { 0x0593, "shalshelethebrew" },                   // Hebrew accent shalshelet
    { 0x0594, "zaqefqatanhebrew" },                   // Hebrew accent zaqef qatan
    { 0x0595, "zaqefgadolhebrew" },                   // Hebrew accent zaqef gadol
    { 0x0596, "tipehahebrew" },                       // Hebrew accent tipeha
    { 0x0597, "reviahebrew" },                        // Hebrew accent revia
    { 0x0598, "zarqahebrew" },                        // Hebrew accent zarqa
    { 0x0599, "pashtahebrew" },                       // Hebrew accent pashta
    { 0x059A, "yetivhebrew" },                        // Hebrew accent yetiv
    { 0x059B, "tevirhebrew" },                        // Hebrew accent tevir
    { 0x059C, "gereshaccenthebrew" },                 // Hebrew accent geresh
    { 0x059D, "gereshmuqdamhebrew" },                 // Hebrew accent geresh muqdam
    { 0x059E, "gershayimaccenthebrew" },              // Hebrew accent gershayim
    { 0x059F, "qarneyparahebrew" },                   // Hebrew accent qarney para
    { 0x05A0, "telishagedolahebrew" },                // Hebrew accent telisha gedola
    { 0x05A1, "pazerhebrew" },                        // Hebrew accent pazer
    { 0x05A3, "munahhebrew" },                        // Hebrew accent munah
    { 0x05A4, "mahapakhhebrew" },                     // Hebrew accent mahapakh
    { 0x05A5, "merkhahebrew" },                       // Hebrew accent merkha
    { 0x05A6, "merkhakefulahebrew" },                 // Hebrew accent merkha kefula
    { 0x05A7, "dargahebrew" },                        // Hebrew accent darga
    { 0x05A8, "qadmahebrew" },                        // Hebrew accent qadma
    { 0x05A9, "telishaqetanahebrew" },                // Hebrew accent telisha qetana
    { 0x05AA, "yerahbenyomohebrew" },                 // Hebrew accent yerah ben yomo
    { 0x05AB, "olehebrew" },                          // Hebrew accent ole
    { 0x05AC, "iluyhebrew" },                         // Hebrew accent iluy
    { 0x05AD, "dehihebrew" },                         // Hebrew accent dehi
    { 0x05AE, "zinorhebrew" },                        // Hebrew accent zinor
    { 0x05AF, "masoracirclehebrew" },                 // Hebrew mark masora circle
    { 0x05B0, "shevawidehebrew" },                    // Hebrew point sheva
    { 0x05B1, "hatafsegolwidehebrew" },               // Hebrew point hataf segol
    { 0x05B2, "hatafpatahwidehebrew" },               // Hebrew point hataf patah
    { 0x05B3, "hatafqamatswidehebrew" },              // Hebrew point hataf qamats
    { 0x05B4, "hiriqwidehebrew" },                    // Hebrew point hiriq
    { 0x05B5, "tserewidehebrew" },                    // Hebrew point tsere
    { 0x05B6, "segolwidehebrew" },                    // Hebrew point segol
    { 0x05B7, "patahwidehebrew" },                    // Hebrew point patah
    { 0x05B8, "qamats10" },                           // Hebrew point qamats
    { 0x05B9, "holamwidehebrew" },                    // Hebrew point holam
    { 0x05BB, "qubutswidehebrew" },                   // Hebrew point qubuts
    { 0x05BC, "dagesh" },                             // Hebrew point dagesh or mapiq
    { 0x05BD, "meteg" },                              // Hebrew point meteg
    { 0x05BE, "maqaf" },                              // Hebrew punctuation maqaf
    { 0x05BF, "rafe" },                               // Hebrew point rafe
    { 0x05C0, "paseq" },                              // Hebrew punctuation paseq
    { 0x05C1, "shindot" },                            // Hebrew point shin dot
    { 0x05C2, "sindot" },                             // Hebrew point sin dot
    { 0x05C3, "sofpasuq" },                           // Hebrew punctuation sof pasuq
    { 0x05C4, "upperdothebrew" },                     // Hebrew mark upper dot
    { 0x05D0, "alef" },                               // Hebrew letter alef
    { 0x05D1, "bet" },                                // Hebrew letter bet
    { 0x05D2, "gimel" },                              // Hebrew letter gimel
    { 0x05D3, "daletqamats" },                        // Hebrew letter dalet
    { 0x05D4, "he" },                                 // Hebrew letter he
    { 0x05D5, "vav" },                                // Hebrew letter vav
    { 0x05D6, "zayin" },                              // Hebrew letter zayin
    { 0x05D7, "het" },                                // Hebrew letter het
    { 0x05D8, "tet" },                                // Hebrew letter tet
    { 0x05D9, "yod" },                                // Hebrew letter yod
    { 0x05DA, "finalkafshevahebrew" },                // Hebrew letter final kaf
    { 0x05DB, "kaf" },                                // Hebrew letter kaf
    { 0x05DC, "lamedholamhebrew" },                   // Hebrew letter lamed
    { 0x05DD, "memfinal" },                           // Hebrew letter final mem
    { 0x05DE, "mem" },                                // Hebrew letter mem
    { 0x05DF, "nunfinal" },                           // Hebrew letter final nun
    { 0x05E0, "nun" },                                // Hebrew letter nun
    { 0x05E1, "samekh" },                             // Hebrew letter samekh
    { 0x05E2, "ayin" },                               // Hebrew letter ayin
    { 0x05E3, "pefinal" },                            // Hebrew letter final pe
    { 0x05E4, "pe" },                                 // Hebrew letter pe
    { 0x05E5, "tsadifinal" },                         // Hebrew letter final tsadi
    { 0x05E6, "tsadi" },                              // Hebrew letter tsadi
    { 0x05E7, "qofqamats" },                          // Hebrew letter qof
    { 0x05E8, "reshqamats" },                         // Hebrew letter resh
    { 0x05E9, "shin" },                               // Hebrew letter shin
    { 0x05EA, "tav" },                                // Hebrew letter tav
    { 0x05F0, "vavdbl" },                             // Hebrew ligature yiddish double vav
    { 0x05F1, "vavyod" },                             // Hebrew ligature yiddish vav yod
    { 0x05F2, "yoddbl" },                             // Hebrew ligature yiddish double yod
    { 0x05F3, "geresh" },                             // Hebrew punctuation geresh
    { 0x05F4, "gershayim" },                          // Hebrew punctuation gershayim
    { 0x060C, "commaarabic" },                        // Arabic comma
    { 0x061B, "semicolonarabic" },                    // Arabic semicolon
    { 0x061F, "questionarabic" },                     // Arabic question mark
    { 0x0621, "hamzasukunarabic" },                   // Arabic letter hamza
    { 0x0622, "alefmaddaabovearabic" },               // Arabic letter alef with madda above
    { 0x0623, "alefhamzaabovearabic" },               // Arabic letter alef with hamza above
    { 0x0624, "wawhamzaabovearabic" },                // Arabic letter waw with hamza above
    { 0x0625, "alefhamzabelowarabic" },               // Arabic letter alef with hamza below
    { 0x0626, "yehhamzaabovearabic" },                // Arabic letter yeh with hamza above
    { 0x0627, "alefarabic" },                         // Arabic letter alef
    { 0x0628, "beharabic" },                          // Arabic letter beh
    { 0x0629, "tehmarbutaarabic" },                   // Arabic letter teh marbuta
    { 0x062A, "teharabic" },                          // Arabic letter teh
    { 0x062B, "theharabic" },                         // Arabic letter theh
    { 0x062C, "jeemarabic" },                         // Arabic letter jeem
    { 0x062D, "haharabic" },                          // Arabic letter hah
    { 0x062E, "khaharabic" },                         // Arabic letter khah
    { 0x062F, "dalarabic" },                          // Arabic letter dal
    { 0x0630, "thalarabic" },                         // Arabic letter thal
    { 0x0631, "reharabic" },                          // Arabic letter reh
    { 0x0632, "zainarabic" },                         // Arabic letter zain
    { 0x0633, "seenarabic" },                         // Arabic letter seen
    { 0x0634, "sheenarabic" },                        // Arabic letter sheen
    { 0x0635, "sadarabic" },                          // Arabic letter sad
    { 0x0636, "dadarabic" },                          // Arabic letter dad
    { 0x0637, "taharabic" },                          // Arabic letter tah
    { 0x0638, "zaharabic" },                          // Arabic letter zah
    { 0x0639, "ainarabic" },                          // Arabic letter ain
    { 0x063A, "ghainarabic" },                        // Arabic letter ghain
    { 0x0640, "tatweelarabic" },                      // Arabic tatweel
    { 0x0641, "feharabic" },                          // Arabic letter feh
    { 0x0642, "qafarabic" },                          // Arabic letter qaf
    { 0x0643, "kafarabic" },                          // Arabic letter kaf
    { 0x0644, "lamarabic" },                          // Arabic letter lam
    { 0x0645, "meemarabic" },                         // Arabic letter meem
    { 0x0646, "noonarabic" },                         // Arabic letter noon
    { 0x0647, "heharabic" },                          // Arabic letter heh
    { 0x0648, "wawarabic" },                          // Arabic letter waw
    { 0x0649, "alefmaksuraarabic" },                  // Arabic letter alef maksura
    { 0x064A, "yeharabic" },                          // Arabic letter yeh
    { 0x064B, "fathatanarabic" },                     // Arabic fathatan
    { 0x064C, "dammatanarabic" },                     // Arabic dammatan
    { 0x064D, "kasratanarabic" },                     // Arabic kasratan
    { 0x064E, "fathaarabic" },                        // Arabic fatha
    { 0x064F, "dammaarabic" },                        // Arabic damma
    { 0x0650, "kasraarabic" },                        // Arabic kasra
    { 0x0651, "shaddaarabic" },                       // Arabic shadda
    { 0x0652, "sukunarabic" },                        // Arabic sukun
    { 0x0660, "zeroarabic" },                         // Arabic-indic digit zero
    { 0x0661, "onearabic" },                          // Arabic-indic digit one
    { 0x0662, "twoarabic" },                          // Arabic-indic digit two
    { 0x0663, "threearabic" },                        // Arabic-indic digit three
    { 0x0664, "fourarabic" },                         // Arabic-indic digit four
    { 0x0665, "fivearabic" },                         // Arabic-indic digit five
    { 0x0666, "sixarabic" },                          // Arabic-indic digit six
    { 0x0667, "sevenarabic" },                        // Arabic-indic digit seven
    { 0x0668, "eightarabic" },                        // Arabic-indic digit eight
    { 0x0669, "ninearabic" },                         // Arabic-indic digit nine
    { 0x066A, "percentarabic" },                      // Arabic percent sign
    { 0x066B, "decimalseparatorarabic" },             // Arabic decimal separator
    { 0x066C, "thousandsseparatorarabic" },           // Arabic thousands separator
    { 0x066D, "asteriskarabic" },                     // Arabic five pointed star
    { 0x0679, "tteharabic" },                         // Arabic letter tteh
    { 0x067E, "peharabic" },                          // Arabic letter peh
    { 0x0686, "tcheharabic" },                        // Arabic letter tcheh
    { 0x0688, "ddalarabic" },                         // Arabic letter ddal
    { 0x0691, "rreharabic" },                         // Arabic letter rreh
    { 0x0698, "jeharabic" },                          // Arabic letter jeh
    { 0x06A4, "veharabic" },                          // Arabic letter veh
    { 0x06AF, "gafarabic" },                          // Arabic letter gaf
    { 0x06BA, "noonghunnaarabic" },                   // Arabic letter noon ghunna
    { 0x06C1, "haaltonearabic" },                     // Arabic letter heh goal
    { 0x06D1, "yehthreedotsbelowarabic" },            // Arabic letter yeh with three dots below
    { 0x06D2, "yehbarreearabic" },                    // Arabic letter yeh barree
    { 0x06F0, "zeropersian" },                        // Extended arabic-indic digit zero
    { 0x06F1, "onepersian" },                         // Extended arabic-indic digit one
    { 0x06F2, "twopersian" },                         // Extended arabic-indic digit two
    { 0x06F3, "threepersian" },                       // Extended arabic-indic digit three
    { 0x06F4, "fourpersian" },                        // Extended arabic-indic digit four
    { 0x06F5, "fivepersian" },                        // Extended arabic-indic digit five
    { 0x06F6, "sixpersian" },                         // Extended arabic-indic digit six
    { 0x06F7, "sevenpersian" },                       // Extended arabic-indic digit seven
    { 0x06F8, "eightpersian" },                       // Extended arabic-indic digit eight
    { 0x06F9, "ninepersian" },                        // Extended arabic-indic digit nine
    { 0x0901, "candrabindudeva" },                    // Devanagari sign candrabindu
    { 0x0902, "anusvaradeva" },                       // Devanagari sign anusvara
    { 0x0903, "visargadeva" },                        // Devanagari sign visarga
    { 0x0905, "adeva" },                              // Devanagari letter a
    { 0x0906, "aadeva" },                             // Devanagari letter aa
    { 0x0907, "ideva" },                              // Devanagari letter i
    { 0x0908, "iideva" },                             // Devanagari letter ii
    { 0x0909, "udeva" },                              // Devanagari letter u
    { 0x090A, "uudeva" },                             // Devanagari letter uu
    { 0x090B, "rvocalicdeva" },                       // Devanagari letter vocalic r
    { 0x090C, "lvocalicdeva" },                       // Devanagari letter vocalic l
    { 0x090D, "ecandradeva" },                        // Devanagari letter candra e
    { 0x090E, "eshortdeva" },                         // Devanagari letter short e
    { 0x090F, "edeva" },                              // Devanagari letter e
    { 0x0910, "aideva" },                             // Devanagari letter ai
    { 0x0911, "ocandradeva" },                        // Devanagari letter candra o
    { 0x0912, "oshortdeva" },                         // Devanagari letter short o
    { 0x0913, "odeva" },                              // Devanagari letter o
    { 0x0914, "audeva" },                             // Devanagari letter au
    { 0x0915, "kadeva" },                             // Devanagari letter ka
    { 0x0916, "khadeva" },                            // Devanagari letter kha
    { 0x0917, "gadeva" },                             // Devanagari letter ga
    { 0x0918, "ghadeva" },                            // Devanagari letter gha
    { 0x0919, "ngadeva" },                            // Devanagari letter nga
    { 0x091A, "cadeva" },                             // Devanagari letter ca
    { 0x091B, "chadeva" },                            // Devanagari letter cha
    { 0x091C, "jadeva" },                             // Devanagari letter ja
    { 0x091D, "jhadeva" },                            // Devanagari letter jha
    { 0x091E, "nyadeva" },                            // Devanagari letter nya
    { 0x091F, "ttadeva" },                            // Devanagari letter tta
    { 0x0920, "tthadeva" },                           // Devanagari letter ttha
    { 0x0921, "ddadeva" },                            // Devanagari letter dda
    { 0x0922, "ddhadeva" },                           // Devanagari letter ddha
    { 0x0923, "nnadeva" },                            // Devanagari letter nna
    { 0x0924, "tadeva" },                             // Devanagari letter ta
    { 0x0925, "thadeva" },                            // Devanagari letter tha
    { 0x0926, "dadeva" },                             // Devanagari letter da
    { 0x0927, "dhadeva" },                            // Devanagari letter dha
    { 0x0928, "nadeva" },                             // Devanagari letter na
    { 0x0929, "nnnadeva" },                           // Devanagari letter nnna
    { 0x092A, "padeva" },                             // Devanagari letter pa
    { 0x092B, "phadeva" },                            // Devanagari letter pha
    { 0x092C, "badeva" },                             // Devanagari letter ba
    { 0x092D, "bhadeva" },                            // Devanagari letter bha
    { 0x092E, "madeva" },                             // Devanagari letter ma
    { 0x092F, "yadeva" },                             // Devanagari letter ya
    { 0x0930, "radeva" },                             // Devanagari letter ra
    { 0x0931, "rradeva" },                            // Devanagari letter rra
    { 0x0932, "ladeva" },                             // Devanagari letter la
    { 0x0933, "lladeva" },                            // Devanagari letter lla
    { 0x0934, "llladeva" },                           // Devanagari letter llla
    { 0x0935, "vadeva" },                             // Devanagari letter va
    { 0x0936, "shadeva" },                            // Devanagari letter sha
    { 0x0937, "ssadeva" },                            // Devanagari letter ssa
    { 0x0938, "sadeva" },                             // Devanagari letter sa
    { 0x0939, "hadeva" },                             // Devanagari letter ha
    { 0x093C, "nuktadeva" },                          // Devanagari sign nukta
    { 0x093D, "avagrahadeva" },                       // Devanagari sign avagraha
    { 0x093E, "aavowelsigndeva" },                    // Devanagari vowel sign aa
    { 0x093F, "ivowelsigndeva" },                     // Devanagari vowel sign i
    { 0x0940, "iivowelsigndeva" },                    // Devanagari vowel sign ii
    { 0x0941, "uvowelsigndeva" },                     // Devanagari vowel sign u
    { 0x0942, "uuvowelsigndeva" },                    // Devanagari vowel sign uu
    { 0x0943, "rvocalicvowelsigndeva" },              // Devanagari vowel sign vocalic r
    { 0x0944, "rrvocalicvowelsigndeva" },             // Devanagari vowel sign vocalic rr
    { 0x0945, "ecandravowelsigndeva" },               // Devanagari vowel sign candra e
    { 0x0946, "eshortvowelsigndeva" },                // Devanagari vowel sign short e
    { 0x0947, "evowelsigndeva" },                     // Devanagari vowel sign e
    { 0x0948, "aivowelsigndeva" },                    // Devanagari vowel sign ai
    { 0x0949, "ocandravowelsigndeva" },               // Devanagari vowel sign candra o
    { 0x094A, "oshortvowelsigndeva" },                // Devanagari vowel sign short o
    { 0x094B, "ovowelsigndeva" },                     // Devanagari vowel sign o
    { 0x094C, "auvowelsigndeva" },                    // Devanagari vowel sign au
    { 0x094D, "viramadeva" },                         // Devanagari sign virama
    { 0x0950, "omdeva" },                             // Devanagari om
    { 0x0951, "udattadeva" },                         // Devanagari stress sign udatta
    { 0x0952, "anudattadeva" },                       // Devanagari stress sign anudatta
    { 0x0953, "gravedeva" },                          // Devanagari grave accent
    { 0x0954, "acutedeva" },                          // Devanagari acute accent
    { 0x0958, "qadeva" },                             // Devanagari letter qa
    { 0x0959, "khhadeva" },                           // Devanagari letter khha
    { 0x095A, "ghhadeva" },                           // Devanagari letter ghha
    { 0x095B, "zadeva" },                             // Devanagari letter za
    { 0x095C, "dddhadeva" },                          // Devanagari letter dddha
    { 0x095D, "rhadeva" },                            // Devanagari letter rha
    { 0x095E, "fadeva" },                             // Devanagari letter fa
    { 0x095F, "yyadeva" },                            // Devanagari letter yya
    { 0x0960, "rrvocalicdeva" },                      // Devanagari letter vocalic rr
    { 0x0961, "llvocalicdeva" },                      // Devanagari letter vocalic ll
    { 0x0962, "lvocalicvowelsigndeva" },              // Devanagari vowel sign vocalic l
    { 0x0963, "llvocalicvowelsigndeva" },             // Devanagari vowel sign vocalic ll
    { 0x0964, "danda" },                              // Devanagari danda
    { 0x0965, "dbldanda" },                           // Devanagari double danda
    { 0x0966, "zerodeva" },                           // Devanagari digit zero
    { 0x0967, "onedeva" },                            // Devanagari digit one
    { 0x0968, "twodeva" },                            // Devanagari digit two
    { 0x0969, "threedeva" },                          // Devanagari digit three
    { 0x096A, "fourdeva" },                           // Devanagari digit four
    { 0x096B, "fivedeva" },                           // Devanagari digit five
    { 0x096C, "sixdeva" },                            // Devanagari digit six
    { 0x096D, "sevendeva" },                          // Devanagari digit seven
    { 0x096E, "eightdeva" },                          // Devanagari digit eight
    { 0x096F, "ninedeva" },                           // Devanagari digit nine
    { 0x0970, "abbreviationsigndeva" },               // Devanagari abbreviation sign
    { 0x0981, "candrabindubengali" },                 // Bengali sign candrabindu
    { 0x0982, "anusvarabengali" },                    // Bengali sign anusvara
    { 0x0983, "visargabengali" },                     // Bengali sign visarga
    { 0x0985, "abengali" },                           // Bengali letter a
    { 0x0986, "aabengali" },                          // Bengali letter aa
    { 0x0987, "ibengali" },                           // Bengali letter i
    { 0x0988, "iibengali" },                          // Bengali letter ii
    { 0x0989, "ubengali" },                           // Bengali letter u
    { 0x098A, "uubengali" },                          // Bengali letter uu
    { 0x098B, "rvocalicbengali" },                    // Bengali letter vocalic r
    { 0x098C, "lvocalicbengali" },                    // Bengali letter vocalic l
    { 0x098F, "ebengali" },                           // Bengali letter e
    { 0x0990, "aibengali" },                          // Bengali letter ai
    { 0x0993, "obengali" },                           // Bengali letter o
    { 0x0994, "aubengali" },                          // Bengali letter au
    { 0x0995, "kabengali" },                          // Bengali letter ka
    { 0x0996, "khabengali" },                         // Bengali letter kha
    { 0x0997, "gabengali" },                          // Bengali letter ga
    { 0x0998, "ghabengali" },                         // Bengali letter gha
    { 0x0999, "ngabengali" },                         // Bengali letter nga
    { 0x099A, "cabengali" },                          // Bengali letter ca
    { 0x099B, "chabengali" },                         // Bengali letter cha
    { 0x099C, "jabengali" },                          // Bengali letter ja
    { 0x099D, "jhabengali" },                         // Bengali letter jha
    { 0x099E, "nyabengali" },                         // Bengali letter nya
    { 0x099F, "ttabengali" },                         // Bengali letter tta
    { 0x09A0, "tthabengali" },                        // Bengali letter ttha
    { 0x09A1, "ddabengali" },                         // Bengali letter dda
    { 0x09A2, "ddhabengali" },                        // Bengali letter ddha
    { 0x09A3, "nnabengali" },                         // Bengali letter nna
    { 0x09A4, "tabengali" },                          // Bengali letter ta
    { 0x09A5, "thabengali" },                         // Bengali letter tha
    { 0x09A6, "dabengali" },                          // Bengali letter da
    { 0x09A7, "dhabengali" },                         // Bengali letter dha
    { 0x09A8, "nabengali" },                          // Bengali letter na
    { 0x09AA, "pabengali" },                          // Bengali letter pa
    { 0x09AB, "phabengali" },                         // Bengali letter pha
    { 0x09AC, "babengali" },                          // Bengali letter ba
    { 0x09AD, "bhabengali" },                         // Bengali letter bha
    { 0x09AE, "mabengali" },                          // Bengali letter ma
    { 0x09AF, "yabengali" },                          // Bengali letter ya
    { 0x09B0, "rabengali" },                          // Bengali letter ra
    { 0x09B2, "labengali" },                          // Bengali letter la
    { 0x09B6, "shabengali" },                         // Bengali letter sha
    { 0x09B7, "ssabengali" },                         // Bengali letter ssa
    { 0x09B8, "sabengali" },                          // Bengali letter sa
    { 0x09B9, "habengali" },                          // Bengali letter ha
    { 0x09BC, "nuktabengali" },                       // Bengali sign nukta
    { 0x09BE, "aavowelsignbengali" },                 // Bengali vowel sign aa
    { 0x09BF, "ivowelsignbengali" },                  // Bengali vowel sign i
    { 0x09C0, "iivowelsignbengali" },                 // Bengali vowel sign ii
    { 0x09C1, "uvowelsignbengali" },                  // Bengali vowel sign u
    { 0x09C2, "uuvowelsignbengali" },                 // Bengali vowel sign uu
    { 0x09C3, "rvocalicvowelsignbengali" },           // Bengali vowel sign vocalic r
    { 0x09C4, "rrvocalicvowelsignbengali" },          // Bengali vowel sign vocalic rr
    { 0x09C7, "evowelsignbengali" },                  // Bengali vowel sign e
    { 0x09C8, "aivowelsignbengali" },                 // Bengali vowel sign ai
    { 0x09CB, "ovowelsignbengali" },                  // Bengali vowel sign o
    { 0x09CC, "auvowelsignbengali" },                 // Bengali vowel sign au
    { 0x09CD, "viramabengali" },                      // Bengali sign virama
    { 0x09D7, "aulengthmarkbengali" },                // Bengali au length mark
    { 0x09DC, "rrabengali" },                         // Bengali letter rra
    { 0x09DD, "rhabengali" },                         // Bengali letter rha
    { 0x09DF, "yyabengali" },                         // Bengali letter yya
    { 0x09E0, "rrvocalicbengali" },                   // Bengali letter vocalic rr
    { 0x09E1, "llvocalicbengali" },                   // Bengali letter vocalic ll
    { 0x09E2, "lvocalicvowelsignbengali" },           // Bengali vowel sign vocalic l
    { 0x09E3, "llvocalicvowelsignbengali" },          // Bengali vowel sign vocalic ll
    { 0x09E6, "zerobengali" },                        // Bengali digit zero
    { 0x09E7, "onebengali" },                         // Bengali digit one
    { 0x09E8, "twobengali" },                         // Bengali digit two
    { 0x09E9, "threebengali" },                       // Bengali digit three
    { 0x09EA, "fourbengali" },                        // Bengali digit four
    { 0x09EB, "fivebengali" },                        // Bengali digit five
    { 0x09EC, "sixbengali" },                         // Bengali digit six
    { 0x09ED, "sevenbengali" },                       // Bengali digit seven
    { 0x09EE, "eightbengali" },                       // Bengali digit eight
    { 0x09EF, "ninebengali" },                        // Bengali digit nine
    { 0x09F0, "ramiddlediagonalbengali" },            // Bengali letter ra with middle diagonal
    { 0x09F1, "ralowerdiagonalbengali" },             // Bengali letter ra with lower diagonal
    { 0x09F2, "rupeemarkbengali" },                   // Bengali rupee mark
    { 0x09F3, "rupeesignbengali" },                   // Bengali rupee sign
    { 0x09F4, "onenumeratorbengali" },                // Bengali currency numerator one
    { 0x09F5, "twonumeratorbengali" },                // Bengali currency numerator two
    { 0x09F6, "threenumeratorbengali" },              // Bengali currency numerator three
    { 0x09F7, "fournumeratorbengali" },               // Bengali currency numerator four
    { 0x09F8, "denominatorminusonenumeratorbengali" },// Bengali currency numerator one less than the denominator
    { 0x09F9, "sixteencurrencydenominatorbengali" },  // Bengali currency denominator sixteen
    { 0x09FA, "issharbengali" },                      // Bengali isshar
    { 0x0A02, "bindigurmukhi" },                      // Gurmukhi sign bindi
    { 0x0A05, "agurmukhi" },                          // Gurmukhi letter a
    { 0x0A06, "aagurmukhi" },                         // Gurmukhi letter aa
    { 0x0A07, "igurmukhi" },                          // Gurmukhi letter i
    { 0x0A08, "iigurmukhi" },                         // Gurmukhi letter ii
    { 0x0A09, "ugurmukhi" },                          // Gurmukhi letter u
    { 0x0A0A, "uugurmukhi" },                         // Gurmukhi letter uu
    { 0x0A0F, "eegurmukhi" },                         // Gurmukhi letter ee
    { 0x0A10, "aigurmukhi" },                         // Gurmukhi letter ai
    { 0x0A13, "oogurmukhi" },                         // Gurmukhi letter oo
    { 0x0A14, "augurmukhi" },                         // Gurmukhi letter au
    { 0x0A15, "kagurmukhi" },                         // Gurmukhi letter ka
    { 0x0A16, "khagurmukhi" },                        // Gurmukhi letter kha
    { 0x0A17, "gagurmukhi" },                         // Gurmukhi letter ga
    { 0x0A18, "ghagurmukhi" },                        // Gurmukhi letter gha
    { 0x0A19, "ngagurmukhi" },                        // Gurmukhi letter nga
    { 0x0A1A, "cagurmukhi" },                         // Gurmukhi letter ca
    { 0x0A1B, "chagurmukhi" },                        // Gurmukhi letter cha
    { 0x0A1C, "jagurmukhi" },                         // Gurmukhi letter ja
    { 0x0A1D, "jhagurmukhi" },                        // Gurmukhi letter jha
    { 0x0A1E, "nyagurmukhi" },                        // Gurmukhi letter nya
    { 0x0A1F, "ttagurmukhi" },                        // Gurmukhi letter tta
    { 0x0A20, "tthagurmukhi" },                       // Gurmukhi letter ttha
    { 0x0A21, "ddagurmukhi" },                        // Gurmukhi letter dda
    { 0x0A22, "ddhagurmukhi" },                       // Gurmukhi letter ddha
    { 0x0A23, "nnagurmukhi" },                        // Gurmukhi letter nna
    { 0x0A24, "tagurmukhi" },                         // Gurmukhi letter ta
    { 0x0A25, "thagurmukhi" },                        // Gurmukhi letter tha
    { 0x0A26, "dagurmukhi" },                         // Gurmukhi letter da
    { 0x0A27, "dhagurmukhi" },                        // Gurmukhi letter dha
    { 0x0A28, "nagurmukhi" },                         // Gurmukhi letter na
    { 0x0A2A, "pagurmukhi" },                         // Gurmukhi letter pa
    { 0x0A2B, "phagurmukhi" },                        // Gurmukhi letter pha
    { 0x0A2C, "bagurmukhi" },                         // Gurmukhi letter ba
    { 0x0A2D, "bhagurmukhi" },                        // Gurmukhi letter bha
    { 0x0A2E, "magurmukhi" },                         // Gurmukhi letter ma
    { 0x0A2F, "yagurmukhi" },                         // Gurmukhi letter ya
    { 0x0A30, "ragurmukhi" },                         // Gurmukhi letter ra
    { 0x0A32, "lagurmukhi" },                         // Gurmukhi letter la
    { 0x0A35, "vagurmukhi" },                         // Gurmukhi letter va
    { 0x0A36, "shagurmukhi" },                        // Gurmukhi letter sha
    { 0x0A38, "sagurmukhi" },                         // Gurmukhi letter sa
    { 0x0A39, "hagurmukhi" },                         // Gurmukhi letter ha
    { 0x0A3C, "nuktagurmukhi" },                      // Gurmukhi sign nukta
    { 0x0A3E, "aamatragurmukhi" },                    // Gurmukhi vowel sign aa
    { 0x0A3F, "imatragurmukhi" },                     // Gurmukhi vowel sign i
    { 0x0A40, "iimatragurmukhi" },                    // Gurmukhi vowel sign ii
    { 0x0A41, "umatragurmukhi" },                     // Gurmukhi vowel sign u
    { 0x0A42, "uumatragurmukhi" },                    // Gurmukhi vowel sign uu
    { 0x0A47, "eematragurmukhi" },                    // Gurmukhi vowel sign ee
    { 0x0A48, "aimatragurmukhi" },                    // Gurmukhi vowel sign ai
    { 0x0A4B, "oomatragurmukhi" },                    // Gurmukhi vowel sign oo
    { 0x0A4C, "aumatragurmukhi" },                    // Gurmukhi vowel sign au
    { 0x0A4D, "halantgurmukhi" },                     // Gurmukhi sign virama
    { 0x0A59, "khhagurmukhi" },                       // Gurmukhi letter khha
    { 0x0A5A, "ghhagurmukhi" },                       // Gurmukhi letter ghha
    { 0x0A5B, "zagurmukhi" },                         // Gurmukhi letter za
    { 0x0A5C, "rragurmukhi" },                        // Gurmukhi letter rra
    { 0x0A5E, "fagurmukhi" },                         // Gurmukhi letter fa
    { 0x0A66, "zerogurmukhi" },                       // Gurmukhi digit zero
    { 0x0A67, "onegurmukhi" },                        // Gurmukhi digit one
    { 0x0A68, "twogurmukhi" },                        // Gurmukhi digit two
    { 0x0A69, "threegurmukhi" },                      // Gurmukhi digit three
    { 0x0A6A, "fourgurmukhi" },                       // Gurmukhi digit four
    { 0x0A6B, "fivegurmukhi" },                       // Gurmukhi digit five
    { 0x0A6C, "sixgurmukhi" },                        // Gurmukhi digit six
    { 0x0A6D, "sevengurmukhi" },                      // Gurmukhi digit seven
    { 0x0A6E, "eightgurmukhi" },                      // Gurmukhi digit eight
    { 0x0A6F, "ninegurmukhi" },                       // Gurmukhi digit nine
    { 0x0A70, "tippigurmukhi" },                      // Gurmukhi tippi
    { 0x0A71, "addakgurmukhi" },                      // Gurmukhi addak
    { 0x0A72, "irigurmukhi" },                        // Gurmukhi iri
    { 0x0A73, "uragurmukhi" },                        // Gurmukhi ura
    { 0x0A74, "ekonkargurmukhi" },                    // Gurmukhi ek onkar
    { 0x0A81, "candrabindugujarati" },                // Gujarati sign candrabindu
    { 0x0A82, "anusvaragujarati" },                   // Gujarati sign anusvara
    { 0x0A83, "visargagujarati" },                    // Gujarati sign visarga
    { 0x0A85, "agujarati" },                          // Gujarati letter a
    { 0x0A86, "aagujarati" },                         // Gujarati letter aa
    { 0x0A87, "igujarati" },                          // Gujarati letter i
    { 0x0A88, "iigujarati" },                         // Gujarati letter ii
    { 0x0A89, "ugujarati" },                          // Gujarati letter u
    { 0x0A8A, "uugujarati" },                         // Gujarati letter uu
    { 0x0A8B, "rvocalicgujarati" },                   // Gujarati letter vocalic r
    { 0x0A8D, "ecandragujarati" },                    // Gujarati vowel candra e
    { 0x0A8F, "egujarati" },                          // Gujarati letter e
    { 0x0A90, "aigujarati" },                         // Gujarati letter ai
    { 0x0A91, "ocandragujarati" },                    // Gujarati vowel candra o
    { 0x0A93, "ogujarati" },                          // Gujarati letter o
    { 0x0A94, "augujarati" },                         // Gujarati letter au
    { 0x0A95, "kagujarati" },                         // Gujarati letter ka
    { 0x0A96, "khagujarati" },                        // Gujarati letter kha
    { 0x0A97, "gagujarati" },                         // Gujarati letter ga
    { 0x0A98, "ghagujarati" },                        // Gujarati letter gha
    { 0x0A99, "ngagujarati" },                        // Gujarati letter nga
    { 0x0A9A, "cagujarati" },                         // Gujarati letter ca
    { 0x0A9B, "chagujarati" },                        // Gujarati letter cha
    { 0x0A9C, "jagujarati" },                         // Gujarati letter ja
    { 0x0A9D, "jhagujarati" },                        // Gujarati letter jha
    { 0x0A9E, "nyagujarati" },                        // Gujarati letter nya
    { 0x0A9F, "ttagujarati" },                        // Gujarati letter tta
    { 0x0AA0, "tthagujarati" },                       // Gujarati letter ttha
    { 0x0AA1, "ddagujarati" },                        // Gujarati letter dda
    { 0x0AA2, "ddhagujarati" },                       // Gujarati letter ddha
    { 0x0AA3, "nnagujarati" },                        // Gujarati letter nna
    { 0x0AA4, "tagujarati" },                         // Gujarati letter ta
    { 0x0AA5, "thagujarati" },                        // Gujarati letter tha
    { 0x0AA6, "dagujarati" },                         // Gujarati letter da
    { 0x0AA7, "dhagujarati" },                        // Gujarati letter dha
    { 0x0AA8, "nagujarati" },                         // Gujarati letter na
    { 0x0AAA, "pagujarati" },                         // Gujarati letter pa
    { 0x0AAB, "phagujarati" },                        // Gujarati letter pha
    { 0x0AAC, "bagujarati" },                         // Gujarati letter ba
    { 0x0AAD, "bhagujarati" },                        // Gujarati letter bha
    { 0x0AAE, "magujarati" },                         // Gujarati letter ma
    { 0x0AAF, "yagujarati" },                         // Gujarati letter ya
    { 0x0AB0, "ragujarati" },                         // Gujarati letter ra
    { 0x0AB2, "lagujarati" },                         // Gujarati letter la
    { 0x0AB3, "llagujarati" },                        // Gujarati letter lla
    { 0x0AB5, "vagujarati" },                         // Gujarati letter va
    { 0x0AB6, "shagujarati" },                        // Gujarati letter sha
    { 0x0AB7, "ssagujarati" },                        // Gujarati letter ssa
    { 0x0AB8, "sagujarati" },                         // Gujarati letter sa
    { 0x0AB9, "hagujarati" },                         // Gujarati letter ha
    { 0x0ABC, "nuktagujarati" },                      // Gujarati sign nukta
    { 0x0ABE, "aavowelsigngujarati" },                // Gujarati vowel sign aa
    { 0x0ABF, "ivowelsigngujarati" },                 // Gujarati vowel sign i
    { 0x0AC0, "iivowelsigngujarati" },                // Gujarati vowel sign ii
    { 0x0AC1, "uvowelsigngujarati" },                 // Gujarati vowel sign u
    { 0x0AC2, "uuvowelsigngujarati" },                // Gujarati vowel sign uu
    { 0x0AC3, "rvocalicvowelsigngujarati" },          // Gujarati vowel sign vocalic r
    { 0x0AC4, "rrvocalicvowelsigngujarati" },         // Gujarati vowel sign vocalic rr
    { 0x0AC5, "ecandravowelsigngujarati" },           // Gujarati vowel sign candra e
    { 0x0AC7, "evowelsigngujarati" },                 // Gujarati vowel sign e
    { 0x0AC8, "aivowelsigngujarati" },                // Gujarati vowel sign ai
    { 0x0AC9, "ocandravowelsigngujarati" },           // Gujarati vowel sign candra o
    { 0x0ACB, "ovowelsigngujarati" },                 // Gujarati vowel sign o
    { 0x0ACC, "auvowelsigngujarati" },                // Gujarati vowel sign au
    { 0x0ACD, "viramagujarati" },                     // Gujarati sign virama
    { 0x0AD0, "omgujarati" },                         // Gujarati om
    { 0x0AE0, "rrvocalicgujarati" },                  // Gujarati letter vocalic rr
    { 0x0AE6, "zerogujarati" },                       // Gujarati digit zero
    { 0x0AE7, "onegujarati" },                        // Gujarati digit one
    { 0x0AE8, "twogujarati" },                        // Gujarati digit two
    { 0x0AE9, "threegujarati" },                      // Gujarati digit three
    { 0x0AEA, "fourgujarati" },                       // Gujarati digit four
    { 0x0AEB, "fivegujarati" },                       // Gujarati digit five
    { 0x0AEC, "sixgujarati" },                        // Gujarati digit six
    { 0x0AED, "sevengujarati" },                      // Gujarati digit seven
    { 0x0AEE, "eightgujarati" },                      // Gujarati digit eight
    { 0x0AEF, "ninegujarati" },                       // Gujarati digit nine
    { 0x0E01, "kokaithai" },                          // Thai character ko kai
    { 0x0E02, "khokhaithai" },                        // Thai character kho khai
    { 0x0E03, "khokhuatthai" },                       // Thai character kho khuat
    { 0x0E04, "khokhwaithai" },                       // Thai character kho khwai
    { 0x0E05, "khokhonthai" },                        // Thai character kho khon
    { 0x0E06, "khorakhangthai" },                     // Thai character kho rakhang
    { 0x0E07, "ngonguthai" },                         // Thai character ngo ngu
    { 0x0E08, "chochanthai" },                        // Thai character cho chan
    { 0x0E09, "chochingthai" },                       // Thai character cho ching
    { 0x0E0A, "chochangthai" },                       // Thai character cho chang
    { 0x0E0B, "sosothai" },                           // Thai character so so
    { 0x0E0C, "chochoethai" },                        // Thai character cho choe
    { 0x0E0D, "yoyingthai" },                         // Thai character yo ying
    { 0x0E0E, "dochadathai" },                        // Thai character do chada
    { 0x0E0F, "topatakthai" },                        // Thai character to patak
    { 0x0E10, "thothanthai" },                        // Thai character tho than
    { 0x0E11, "thonangmonthothai" },                  // Thai character tho nangmontho
    { 0x0E12, "thophuthaothai" },                     // Thai character tho phuthao
    { 0x0E13, "nonenthai" },                          // Thai character no nen
    { 0x0E14, "dodekthai" },                          // Thai character do dek
    { 0x0E15, "totaothai" },                          // Thai character to tao
    { 0x0E16, "thothungthai" },                       // Thai character tho thung
    { 0x0E17, "thothahanthai" },                      // Thai character tho thahan
    { 0x0E18, "thothongthai" },                       // Thai character tho thong
    { 0x0E19, "nonuthai" },                           // Thai character no nu
    { 0x0E1A, "bobaimaithai" },                       // Thai character bo baimai
    { 0x0E1B, "poplathai" },                          // Thai character po pla
    { 0x0E1C, "phophungthai" },                       // Thai character pho phung
    { 0x0E1D, "fofathai" },                           // Thai character fo fa
    { 0x0E1E, "phophanthai" },                        // Thai character pho phan
    { 0x0E1F, "fofanthai" },                          // Thai character fo fan
    { 0x0E20, "phosamphaothai" },                     // Thai character pho samphao
    { 0x0E21, "momathai" },                           // Thai character mo ma
    { 0x0E22, "yoyakthai" },                          // Thai character yo yak
    { 0x0E23, "roruathai" },                          // Thai character ro rua
    { 0x0E24, "ruthai" },                             // Thai character ru
    { 0x0E25, "lolingthai" },                         // Thai character lo ling
    { 0x0E26, "luthai" },                             // Thai character lu
    { 0x0E27, "wowaenthai" },                         // Thai character wo waen
    { 0x0E28, "sosalathai" },                         // Thai character so sala
    { 0x0E29, "sorusithai" },                         // Thai character so rusi
    { 0x0E2A, "sosuathai" },                          // Thai character so sua
    { 0x0E2B, "hohipthai" },                          // Thai character ho hip
    { 0x0E2C, "lochulathai" },                        // Thai character lo chula
    { 0x0E2D, "oangthai" },                           // Thai character o ang
    { 0x0E2E, "honokhukthai" },                       // Thai character ho nokhuk
    { 0x0E2F, "paiyannoithai" },                      // Thai character paiyannoi
    { 0x0E30, "saraathai" },                          // Thai character sara a
    { 0x0E31, "maihanakatthai" },                     // Thai character mai han-akat
    { 0x0E32, "saraaathai" },                         // Thai character sara aa
    { 0x0E33, "saraamthai" },                         // Thai character sara am
    { 0x0E34, "saraithai" },                          // Thai character sara i
    { 0x0E35, "saraiithai" },                         // Thai character sara ii
    { 0x0E36, "sarauethai" },                         // Thai character sara ue
    { 0x0E37, "saraueethai" },                        // Thai character sara uee
    { 0x0E38, "sarauthai" },                          // Thai character sara u
    { 0x0E39, "sarauuthai" },                         // Thai character sara uu
    { 0x0E3A, "phinthuthai" },                        // Thai character phinthu
    { 0x0E3F, "bahtthai" },                           // Thai currency symbol baht
    { 0x0E40, "saraethai" },                          // Thai character sara e
    { 0x0E41, "saraaethai" },                         // Thai character sara ae
    { 0x0E42, "saraothai" },                          // Thai character sara o
    { 0x0E43, "saraaimaimuanthai" },                  // Thai character sara ai maimuan
    { 0x0E44, "saraaimaimalaithai" },                 // Thai character sara ai maimalai
    { 0x0E45, "lakkhangyaothai" },                    // Thai character lakkhangyao
    { 0x0E46, "maiyamokthai" },                       // Thai character maiyamok
    { 0x0E47, "maitaikhuthai" },                      // Thai character maitaikhu
    { 0x0E48, "maiekthai" },                          // Thai character mai ek
    { 0x0E49, "maithothai" },                         // Thai character mai tho
    { 0x0E4A, "maitrithai" },                         // Thai character mai tri
    { 0x0E4B, "maichattawathai" },                    // Thai character mai chattawa
    { 0x0E4C, "thanthakhatthai" },                    // Thai character thanthakhat
    { 0x0E4D, "nikhahitthai" },                       // Thai character nikhahit
    { 0x0E4E, "yamakkanthai" },                       // Thai character yamakkan
    { 0x0E4F, "fongmanthai" },                        // Thai character fongman
    { 0x0E50, "zerothai" },                           // Thai digit zero
    { 0x0E51, "onethai" },                            // Thai digit one
    { 0x0E52, "twothai" },                            // Thai digit two
    { 0x0E53, "threethai" },                          // Thai digit three
    { 0x0E54, "fourthai" },                           // Thai digit four
    { 0x0E55, "fivethai" },                           // Thai digit five
    { 0x0E56, "sixthai" },                            // Thai digit six
    { 0x0E57, "seventhai" },                          // Thai digit seven
    { 0x0E58, "eightthai" },                          // Thai digit eight
    { 0x0E59, "ninethai" },                           // Thai digit nine
    { 0x0E5A, "angkhankhuthai" },                     // Thai character angkhankhu
    { 0x0E5B, "khomutthai" },                         // Thai character khomut
    { 0x1E00, "Aringbelow" },                         // Latin Capital Letter A With Ring Below
    { 0x1E01, "aringbelow" },                         // Latin small letter a with ring below
    { 0x1E02, "Bdotaccent" },                         // Latin Capital Letter B With Dot Above
    { 0x1E03, "bdotaccent" },                         // Latin small letter b with dot above
    { 0x1E04, "Bdotbelow" },                          // Latin Capital Letter B With Dot Below
    { 0x1E05, "bdotbelow" },                          // Latin small letter b with dot below
    { 0x1E06, "Blinebelow" },                         // Latin Capital Letter B With Line Below
    { 0x1E07, "blinebelow" },                         // Latin small letter b with line below
    { 0x1E08, "Ccedillaacute" },                      // Latin Capital Letter C With Cedilla And Acute
    { 0x1E09, "ccedillaacute" },                      // Latin small letter c with cedilla and acute
    { 0x1E0A, "Ddotaccent" },                         // Latin Capital Letter D With Dot Above
    { 0x1E0B, "ddotaccent" },                         // Latin small letter d with dot above
    { 0x1E0C, "Ddotbelow" },                          // Latin Capital Letter D With Dot Below
    { 0x1E0D, "ddotbelow" },                          // Latin small letter d with dot below
    { 0x1E0E, "Dlinebelow" },                         // Latin Capital Letter D With Line Below
    { 0x1E0F, "dlinebelow" },                         // Latin small letter d with line below
    { 0x1E10, "Dcedilla" },                           // Latin Capital Letter D With Cedilla
    { 0x1E11, "dcedilla" },                           // Latin small letter d with cedilla
    { 0x1E12, "Dcircumflexbelow" },                   // Latin Capital Letter D With Circumflex Below
    { 0x1E13, "dcircumflexbelow" },                   // Latin small letter d with circumflex below
    { 0x1E14, "Emacrongrave" },                       // Latin Capital Letter E With Macron And Grave
    { 0x1E15, "emacrongrave" },                       // Latin small letter e with macron and grave
    { 0x1E16, "Emacronacute" },                       // Latin Capital Letter E With Macron And Acute
    { 0x1E17, "emacronacute" },                       // Latin small letter e with macron and acute
    { 0x1E18, "Ecircumflexbelow" },                   // Latin Capital Letter E With Circumflex Below
    { 0x1E19, "ecircumflexbelow" },                   // Latin small letter e with circumflex below
    { 0x1E1A, "Etildebelow" },                        // Latin Capital Letter E With Tilde Below
    { 0x1E1B, "etildebelow" },                        // Latin small letter e with tilde below
    { 0x1E1C, "Ecedillabreve" },                      // Latin Capital Letter E With Cedilla And Breve
    { 0x1E1D, "ecedillabreve" },                      // Latin small letter e with cedilla and breve
    { 0x1E1E, "Fdotaccent" },                         // Latin Capital Letter F With Dot Above
    { 0x1E1F, "fdotaccent" },                         // Latin small letter f with dot above
    { 0x1E20, "Gmacron" },                            // Latin Capital Letter G With Macron
    { 0x1E21, "gmacron" },                            // Latin small letter g with macron
    { 0x1E22, "Hdotaccent" },                         // Latin Capital Letter H With Dot Above
    { 0x1E23, "hdotaccent" },                         // Latin small letter h with dot above
    { 0x1E24, "Hdotbelow" },                          // Latin Capital Letter H With Dot Below
    { 0x1E25, "hdotbelow" },                          // Latin small letter h with dot below
    { 0x1E26, "Hdieresis" },                          // Latin Capital Letter H With Diaeresis
    { 0x1E27, "hdieresis" },                          // Latin small letter h with diaeresis
    { 0x1E28, "Hcedilla" },                           // Latin Capital Letter H With Cedilla
    { 0x1E29, "hcedilla" },                           // Latin small letter h with cedilla
    { 0x1E2A, "Hbrevebelow" },                        // Latin Capital Letter H With Breve Below
    { 0x1E2B, "hbrevebelow" },                        // Latin small letter h with breve below
    { 0x1E2C, "Itildebelow" },                        // Latin Capital Letter I With Tilde Below
    { 0x1E2D, "itildebelow" },                        // Latin small letter i with tilde below
    { 0x1E2E, "Idieresisacute" },                     // Latin Capital Letter I With Diaeresis And Acute
    { 0x1E2F, "idieresisacute" },                     // Latin small letter i with diaeresis and acute
    { 0x1E30, "Kacute" },                             // Latin Capital Letter K With Acute
    { 0x1E31, "kacute" },                             // Latin small letter k with acute
    { 0x1E32, "Kdotbelow" },                          // Latin Capital Letter K With Dot Below
    { 0x1E33, "kdotbelow" },                          // Latin small letter k with dot below
    { 0x1E34, "Klinebelow" },                         // Latin Capital Letter K With Line Below
    { 0x1E35, "klinebelow" },                         // Latin small letter k with line below
    { 0x1E36, "Ldotbelow" },                          // Latin Capital Letter L With Dot Below
    { 0x1E37, "ldotbelow" },                          // Latin small letter l with dot below
    { 0x1E38, "Ldotbelowmacron" },                    // Latin Capital Letter L With Dot Below And Macron
    { 0x1E39, "ldotbelowmacron" },                    // Latin small letter l with dot below and macron
    { 0x1E3A, "Llinebelow" },                         // Latin Capital Letter L With Line Below
    { 0x1E3B, "llinebelow" },                         // Latin small letter l with line below
    { 0x1E3C, "Lcircumflexbelow" },                   // Latin Capital Letter L With Circumflex Below
    { 0x1E3D, "lcircumflexbelow" },                   // Latin small letter l with circumflex below
    { 0x1E3E, "Macute" },                             // Latin Capital Letter M With Acute
    { 0x1E3F, "macute" },                             // Latin small letter m with acute
    { 0x1E40, "Mdotaccent" },                         // Latin Capital Letter M With Dot Above
    { 0x1E41, "mdotaccent" },                         // Latin small letter m with dot above
    { 0x1E42, "Mdotbelow" },                          // Latin Capital Letter M With Dot Below
    { 0x1E43, "mdotbelow" },                          // Latin small letter m with dot below
    { 0x1E44, "Ndotaccent" },                         // Latin Capital Letter N With Dot Above
    { 0x1E45, "ndotaccent" },                         // Latin small letter n with dot above
    { 0x1E46, "Ndotbelow" },                          // Latin Capital Letter N With Dot Below
    { 0x1E47, "ndotbelow" },                          // Latin small letter n with dot below
    { 0x1E48, "Nlinebelow" },                         // Latin Capital Letter N With Line Below
    { 0x1E49, "nlinebelow" },                         // Latin small letter n with line below
    { 0x1E4A, "Ncircumflexbelow" },                   // Latin Capital Letter N With Circumflex Below
    { 0x1E4B, "ncircumflexbelow" },                   // Latin small letter n with circumflex below
    { 0x1E4C, "Otildeacute" },                        // Latin Capital Letter O With Tilde And Acute
    { 0x1E4D, "otildeacute" },                        // Latin small letter o with tilde and acute
    { 0x1E4E, "Otildedieresis" },                     // Latin Capital Letter O With Tilde And Diaeresis
    { 0x1E4F, "otildedieresis" },                     // Latin small letter o with tilde and diaeresis
    { 0x1E50, "Omacrongrave" },                       // Latin Capital Letter O With Macron And Grave
    { 0x1E51, "omacrongrave" },                       // Latin small letter o with macron and grave
    { 0x1E52, "Omacronacute" },                       // Latin Capital Letter O With Macron And Acute
    { 0x1E53, "omacronacute" },                       // Latin small letter o with macron and acute
    { 0x1E54, "Pacute" },                             // Latin Capital Letter P With Acute
    { 0x1E55, "pacute" },                             // Latin small letter p with acute
    { 0x1E56, "Pdotaccent" },                         // Latin Capital Letter P With Dot Above
    { 0x1E57, "pdotaccent" },                         // Latin small letter p with dot above
    { 0x1E58, "Rdotaccent" },                         // Latin Capital Letter R With Dot Above
    { 0x1E59, "rdotaccent" },                         // Latin small letter r with dot above
    { 0x1E5A, "Rdotbelow" },                          // Latin Capital Letter R With Dot Below
    { 0x1E5B, "rdotbelow" },                          // Latin small letter r with dot below
    { 0x1E5C, "Rdotbelowmacron" },                    // Latin Capital Letter R With Dot Below And Macron
    { 0x1E5D, "rdotbelowmacron" },                    // Latin small letter r with dot below and macron
    { 0x1E5E, "Rlinebelow" },                         // Latin Capital Letter R With Line Below
    { 0x1E5F, "rlinebelow" },                         // Latin small letter r with line below
    { 0x1E60, "Sdotaccent" },                         // Latin Capital Letter S With Dot Above
    { 0x1E61, "sdotaccent" },                         // Latin small letter s with dot above
    { 0x1E62, "Sdotbelow" },                          // Latin Capital Letter S With Dot Below
    { 0x1E63, "sdotbelow" },                          // Latin small letter s with dot below
    { 0x1E64, "Sacutedotaccent" },                    // Latin Capital Letter S With Acute And Dot Above
    { 0x1E65, "sacutedotaccent" },                    // Latin small letter s with acute and dot above
    { 0x1E66, "Scarondotaccent" },                    // Latin Capital Letter S With Caron And Dot Above
    { 0x1E67, "scarondotaccent" },                    // Latin small letter s with caron and dot above
    { 0x1E68, "Sdotbelowdotaccent" },                 // Latin Capital Letter S With Dot Below And Dot Above
    { 0x1E69, "sdotbelowdotaccent" },                 // Latin small letter s with dot below and dot above
    { 0x1E6A, "Tdotaccent" },                         // Latin Capital Letter T With Dot Above
    { 0x1E6B, "tdotaccent" },                         // Latin small letter t with dot above
    { 0x1E6C, "Tdotbelow" },                          // Latin Capital Letter T With Dot Below
    { 0x1E6D, "tdotbelow" },                          // Latin small letter t with dot below
    { 0x1E6E, "Tlinebelow" },                         // Latin Capital Letter T With Line Below
    { 0x1E6F, "tlinebelow" },                         // Latin small letter t with line below
    { 0x1E70, "Tcircumflexbelow" },                   // Latin Capital Letter T With Circumflex Below
    { 0x1E71, "tcircumflexbelow" },                   // Latin small letter t with circumflex below
    { 0x1E72, "Udieresisbelow" },                     // Latin Capital Letter U With Diaeresis Below
    { 0x1E73, "udieresisbelow" },                     // Latin small letter u with diaeresis below
    { 0x1E74, "Utildebelow" },                        // Latin Capital Letter U With Tilde Below
    { 0x1E75, "utildebelow" },                        // Latin small letter u with tilde below
    { 0x1E76, "Ucircumflexbelow" },                   // Latin Capital Letter U With Circumflex Below
    { 0x1E77, "ucircumflexbelow" },                   // Latin small letter u with circumflex below
    { 0x1E78, "Utildeacute" },                        // Latin Capital Letter U With Tilde And Acute
    { 0x1E79, "utildeacute" },                        // Latin small letter u with tilde and acute
    { 0x1E7A, "Umacrondieresis" },                    // Latin Capital Letter U With Macron And Diaeresis
    { 0x1E7B, "umacrondieresis" },                    // Latin small letter u with macron and diaeresis
    { 0x1E7C, "Vtilde" },                             // Latin Capital Letter V With Tilde
    { 0x1E7D, "vtilde" },                             // Latin small letter v with tilde
    { 0x1E7E, "Vdotbelow" },                          // Latin Capital Letter V With Dot Below
    { 0x1E7F, "vdotbelow" },                          // Latin small letter v with dot below
    { 0x1E80, "Wgrave" },                             // Latin Capital Letter W With Grave
    { 0x1E81, "wgrave" },                             // Latin small letter w with grave
    { 0x1E82, "Wacute" },                             // Latin Capital Letter W With Acute
    { 0x1E83, "wacute" },                             // Latin small letter w with acute
    { 0x1E84, "Wdieresis" },                          // Latin Capital Letter W With Diaeresis
    { 0x1E85, "wdieresis" },                          // Latin small letter w with diaeresis
    { 0x1E86, "Wdotaccent" },                         // Latin Capital Letter W With Dot Above
    { 0x1E87, "wdotaccent" },                         // Latin small letter w with dot above
    { 0x1E88, "Wdotbelow" },                          // Latin Capital Letter W With Dot Below
    { 0x1E89, "wdotbelow" },                          // Latin small letter w with dot below
    { 0x1E8A, "Xdotaccent" },                         // Latin Capital Letter X With Dot Above
    { 0x1E8B, "xdotaccent" },                         // Latin small letter x with dot above
    { 0x1E8C, "Xdieresis" },                          // Latin Capital Letter X With Diaeresis
    { 0x1E8D, "xdieresis" },                          // Latin small letter x with diaeresis
    { 0x1E8E, "Ydotaccent" },                         // Latin Capital Letter Y With Dot Above
    { 0x1E8F, "ydotaccent" },                         // Latin small letter y with dot above
    { 0x1E90, "Zcircumflex" },                        // Latin Capital Letter Z With Circumflex
    { 0x1E91, "zcircumflex" },                        // Latin small letter z with circumflex
    { 0x1E92, "Zdotbelow" },                          // Latin Capital Letter Z With Dot Below
    { 0x1E93, "zdotbelow" },                          // Latin small letter z with dot below
    { 0x1E94, "Zlinebelow" },                         // Latin Capital Letter Z With Line Below
    { 0x1E95, "zlinebelow" },                         // Latin small letter z with line below
    { 0x1E96, "hlinebelow" },                         // Latin small letter h with line below
    { 0x1E97, "tdieresis" },                          // Latin small letter t with diaeresis
    { 0x1E98, "wring" },                              // Latin small letter w with ring above
    { 0x1E99, "yring" },                              // Latin small letter y with ring above
    { 0x1E9A, "arighthalf" },                         // Latin small letter a with right half ring
    { 0x1E9B, "slongdotaccent" },                     // Latin small letter long s with dot above
    { 0x1EA0, "Adotbelow" },                          // Latin Capital Letter A With Dot Below
    { 0x1EA1, "adotbelow" },                          // Latin small letter a with dot below
    { 0x1EA2, "Ahook" },                              // Latin Capital Letter A With Hook Above
    { 0x1EA3, "ahook" },                              // Latin small letter a with hook above
    { 0x1EA4, "Acircumflexacute" },                   // Latin Capital Letter A With Circumflex And Acute
    { 0x1EA5, "acircumflexacute" },                   // Latin small letter a with circumflex and acute
    { 0x1EA6, "Acircumflexgrave" },                   // Latin Capital Letter A With Circumflex And Grave
    { 0x1EA7, "acircumflexgrave" },                   // Latin small letter a with circumflex and grave
    { 0x1EA8, "Acircumflexhook" },                    // Latin Capital Letter A With Circumflex And Hook Above
    { 0x1EA9, "acircumflexhook" },                    // Latin small letter a with circumflex and hook above
    { 0x1EAA, "Acircumflextilde" },                   // Latin Capital Letter A With Circumflex And Tilde
    { 0x1EAB, "acircumflextilde" },                   // Latin small letter a with circumflex and tilde
    { 0x1EAC, "Acircumflexdotbelow" },                // Latin Capital Letter A With Circumflex And Dot Below
    { 0x1EAD, "acircumflexdotbelow" },                // Latin small letter a with circumflex and dot below
    { 0x1EAE, "Abreveacute" },                        // Latin Capital Letter A With Breve And Acute
    { 0x1EAF, "abreveacute" },                        // Latin small letter a with breve and acute
    { 0x1EB0, "Abrevegrave" },                        // Latin Capital Letter A With Breve And Grave
    { 0x1EB1, "abrevegrave" },                        // Latin small letter a with breve and grave
    { 0x1EB2, "Abrevehook" },                         // Latin Capital Letter A With Breve And Hook Above
    { 0x1EB3, "abrevehook" },                         // Latin small letter a with breve and hook above
    { 0x1EB4, "Abrevetilde" },                        // Latin Capital Letter A With Breve And Tilde
    { 0x1EB5, "abrevetilde" },                        // Latin small letter a with breve and tilde
    { 0x1EB6, "Abrevedotbelow" },                     // Latin Capital Letter A With Breve And Dot Below
    { 0x1EB7, "abrevedotbelow" },                     // Latin small letter a with breve and dot below
    { 0x1EB8, "Edotbelow" },                          // Latin Capital Letter E With Dot Below
    { 0x1EB9, "edotbelow" },                          // Latin small letter e with dot below
    { 0x1EBA, "Ehook" },                              // Latin Capital Letter E With Hook Above
    { 0x1EBB, "ehook" },                              // Latin small letter e with hook above
    { 0x1EBC, "Etilde" },                             // Latin Capital Letter E With Tilde
    { 0x1EBD, "etilde" },                             // Latin small letter e with tilde
    { 0x1EBE, "Ecircumflexacute" },                   // Latin Capital Letter E With Circumflex And Acute
    { 0x1EBF, "ecircumflexacute" },                   // Latin small letter e with circumflex and acute
    { 0x1EC0, "Ecircumflexgrave" },                   // Latin Capital Letter E With Circumflex And Grave
    { 0x1EC1, "ecircumflexgrave" },                   // Latin small letter e with circumflex and grave
    { 0x1EC2, "Ecircumflexhook" },                    // Latin Capital Letter E With Circumflex And Hook Above
    { 0x1EC3, "ecircumflexhook" },                    // Latin small letter e with circumflex and hook above
    { 0x1EC4, "Ecircumflextilde" },                   // Latin Capital Letter E With Circumflex And Tilde
    { 0x1EC5, "ecircumflextilde" },                   // Latin small letter e with circumflex and tilde
    { 0x1EC6, "Ecircumflexdotbelow" },                // Latin Capital Letter E With Circumflex And Dot Below
    { 0x1EC7, "ecircumflexdotbelow" },                // Latin small letter e with circumflex and dot below
    { 0x1EC8, "Ihook" },                              // Latin Capital Letter I With Hook Above
    { 0x1EC9, "ihook" },                              // Latin small letter i with hook above
    { 0x1ECA, "Idotbelow" },                          // Latin Capital Letter I With Dot Below
    { 0x1ECB, "idotbelow" },                          // Latin small letter i with dot below
    { 0x1ECC, "Odotbelow" },                          // Latin Capital Letter O With Dot Below
    { 0x1ECD, "odotbelow" },                          // Latin small letter o with dot below
    { 0x1ECE, "Ohookabove" },                         // Latin Capital Letter O With Hook Above
    { 0x1ECF, "ohookabove" },                         // Latin small letter o with hook above
    { 0x1ED0, "Ocircumflexacute" },                   // Latin Capital Letter O With Circumflex And Acute
    { 0x1ED1, "ocircumflexacute" },                   // Latin small letter o with circumflex and acute
    { 0x1ED2, "Ocircumflexgrave" },                   // Latin Capital Letter O With Circumflex And Grave
    { 0x1ED3, "ocircumflexgrave" },                   // Latin small letter o with circumflex and grave
    { 0x1ED4, "Ocircumflexhook" },                    // Latin Capital Letter O With Circumflex And Hook Above
    { 0x1ED5, "ocircumflexhook" },                    // Latin small letter o with circumflex and hook above
    { 0x1ED6, "Ocircumflextilde" },                   // Latin Capital Letter O With Circumflex And Tilde
    { 0x1ED7, "ocircumflextilde" },                   // Latin small letter o with circumflex and tilde
    { 0x1ED8, "Ocircumflexdotbelow" },                // Latin Capital Letter O With Circumflex And Dot Below
    { 0x1ED9, "ocircumflexdotbelow" },                // Latin small letter o with circumflex and dot below
    { 0x1EDA, "Ohornacute" },                         // Latin Capital Letter O With Horn And Acute
    { 0x1EDB, "ohornacute" },                         // Latin small letter o with horn and acute
    { 0x1EDC, "Ohorngrave" },                         // Latin Capital Letter O With Horn And Grave
    { 0x1EDD, "ohorngrave" },                         // Latin small letter o with horn and grave
    { 0x1EDE, "Ohornhook" },                          // Latin Capital Letter O With Horn And Hook Above
    { 0x1EDF, "ohornhook" },                          // Latin small letter o with horn and hook above
    { 0x1EE0, "Ohorntilde" },                         // Latin Capital Letter O With Horn And Tilde
    { 0x1EE1, "ohorntilde" },                         // Latin small letter o with horn and tilde
    { 0x1EE2, "Ohorndotbelow" },                      // Latin Capital Letter O With Horn And Dot Below
    { 0x1EE3, "ohorndotbelow" },                      // Latin small letter o with horn and dot below
    { 0x1EE4, "Udotbelow" },                          // Latin Capital Letter U With Dot Below
    { 0x1EE5, "udotbelow" },                          // Latin small letter u with dot below
    { 0x1EE6, "Uhook" },                              // Latin Capital Letter U With Hook Above
    { 0x1EE7, "uhook" },                              // Latin small letter u with hook above
    { 0x1EE8, "Uhornacute" },                         // Latin Capital Letter U With Horn And Acute
    { 0x1EE9, "uhornacute" },                         // Latin small letter u with horn and acute
    { 0x1EEA, "Uhorngrave" },                         // Latin Capital Letter U With Horn And Grave
    { 0x1EEB, "uhorngrave" },                         // Latin small letter u with horn and grave
    { 0x1EEC, "Uhornhook" },                          // Latin Capital Letter U With Horn And Hook Above
    { 0x1EED, "uhornhook" },                          // Latin small letter u with horn and hook above
    { 0x1EEE, "Uhorntilde" },                         // Latin Capital Letter U With Horn And Tilde
    { 0x1EEF, "uhorntilde" },                         // Latin small letter u with horn and tilde
    { 0x1EF0, "Uhorndotbelow" },                      // Latin Capital Letter U With Horn And Dot Below
    { 0x1EF1, "uhorndotbelow" },                      // Latin small letter u with horn and dot below
    { 0x1EF2, "Ygrave" },                             // Latin Capital Letter Y With Grave
    { 0x1EF3, "ygrave" },                             // Latin small letter y with grave
    { 0x1EF4, "Ydotbelow" },                          // Latin Capital Letter Y With Dot Below
    { 0x1EF5, "ydotbelow" },                          // Latin small letter y with dot below
    { 0x1EF6, "Yhookabove" },                         // Latin Capital Letter Y With Hook Above
    { 0x1EF7, "yhookabove" },                         // Latin small letter y with hook above
    { 0x1EF8, "Ytilde" },                             // Latin Capital Letter Y With Tilde
    { 0x1EF9, "ytilde" },                             // Latin small letter y with tilde
    { 0x2000, "enquad" },                             // En quad
    { 0x2001, "emquad" },                             // Em quad
    { 0x2002, "enspace" },                            // En space
    { 0x2003, "emspace" },                            // Em space
    { 0x2004, "threeperemspace" },                    // Three-per-em space
    { 0x2005, "fourperemspace" },                     // Four-per-em space
    { 0x2006, "sixperemspace" },                      // Six-per-em space
    { 0x2007, "figurespace" },                        // Figure space
    { 0x2008, "punctuationspace" },                   // Punctuation space
    { 0x2009, "thinspace" },                          // Thin space
    { 0x200A, "hairspace" },                          // Hair space
    { 0x200B, "zerospace" },                          // Zero width space
    { 0x200C, "zeronojoin" },                         // Zero width non-joiner
    { 0x200D, "zerojoin" },                           // Zero width joiner
    { 0x200E, "lefttoright" },                        // Left-to-right mark
    { 0x200F, "righttoleft" },                        // Right-to-left mark
    { 0x2010, "hyphentwo" },                          // Hyphen
    { 0x2011, "hyphennobreak" },                      // Non-breaking hyphen
    { 0x2012, "figuredash" },                         // Figure dash
    { 0x2013, "endash" },                             // En dash
    { 0x2014, "emdash" },                             // Em dash
    { 0x2015, "longdash" },                           // Horizontal bar
    { 0x2016, "dblverticalbar" },                     // Double vertical line
    { 0x2017, "dbllowline" },                         // Double low line
    { 0x2018, "quoteleft" },                          // Left single quotation mark
    { 0x2019, "quoteright" },                         // Right single quotation mark
    { 0x201A, "quotesinglbase" },                     // Single low-9 quotation mark
    { 0x201B, "quoteleftreversed" },                  // Single high-reversed-9 quotation mark
    { 0x201C, "quotedblleft" },                       // Left double quotation mark
    { 0x201D, "quotedblright" },                      // Right double quotation mark
    { 0x201E, "quotedblbase" },                       // Double low-9 quotation mark
    { 0x201F, "quotedblrev" },                        // Double high-reversed-9 quotation mark
    { 0x2020, "dagger" },                             // Dagger
    { 0x2021, "daggerdbl" },                          // Double dagger
    { 0x2022, "bullet" },                             // Bullet
    { 0x2023, "trianglebullet" },                     // Triangular bullet
    { 0x2024, "onedotlead" },                         // One dot leader
    { 0x2025, "twodotlead" },                         // Two dot leader
    { 0x2026, "ellipsis" },                           // Horizontal ellipsis
    { 0x2027, "hyphendot" },                          // Hyphenation point
    { 0x2028, "lineseparator" },                      // Line separator
    { 0x2029, "paragraphseparator" },                 // Paragraph separator
    { 0x202A, "lre" },                                // Left-to-right embedding
    { 0x202B, "rle" },                                // Right-to-left embedding
    { 0x202C, "pdf" },                                // Pop directional formatting
    { 0x202D, "lro" },                                // Left-to-right override
    { 0x202E, "rlo" },                                // Right-to-left override
    { 0x2030, "perthousand" },                        // Per mille sign
    { 0x2031, "pertenthousand" },                     // Per ten thousand sign
    { 0x2032, "prime1" },                             // Prime
    { 0x2033, "second" },                             // Double prime
    { 0x2034, "primetripl1" },                        // Triple prime
    { 0x2035, "primerev1" },                          // Reversed prime
    { 0x2036, "primedblrev1" },                       // Reversed double prime
    { 0x2037, "primetriplerev1" },                    // Reversed triple prime
    { 0x2038, "caret" },                              // Caret
    { 0x2039, "guilsinglleft" },                      // Single left-pointing angle quotation mark
    { 0x203A, "guilsinglright" },                     // Single right-pointing angle quotation mark
    { 0x203B, "refmark" },                            // Reference mark
    { 0x203C, "exclamdbl" },                          // Double exclamation mark
    { 0x203D, "interrobang" },                        // Interrobang
    { 0x203E, "radicalex" },                          // Overline
    { 0x203F, "undertie" },                           // Undertie
    { 0x2040, "tie1" },                               // Character tie
    { 0x2041, "caretinsert" },                        // Caret insertion point
    { 0x2042, "asterism" },                           // Asterism
    { 0x2043, "hyphenbullet" },                       // Hyphen bullet
    { 0x2044, "fraction1" },                          // Fraction slash
    { 0x2045, "lquilbrace" },                         // Left square bracket with quill
    { 0x2046, "rquilbrace" },                         // Right square bracket with quill
    { 0x2070, "zerosuperior" },                       // Superscript zero
    { 0x2074, "foursuperior" },                       // Superscript four
    { 0x2075, "fivesuperior" },                       // Superscript five
    { 0x2076, "sixsuperior" },                        // Superscript six
    { 0x2077, "sevensuperior" },                      // Superscript seven
    { 0x2078, "eightsuperior" },                      // Superscript eight
    { 0x2079, "ninesuperior" },                       // Superscript nine
    { 0x207A, "plussuperior" },                       // Superscript plus sign
    { 0x207B, "minussuperior" },                      // Superscript minus
    { 0x207C, "equalsuperior" },                      // Superscript equals sign
    { 0x207D, "parenleftsuperior" },                  // Superscript left parenthesis
    { 0x207E, "parenrightsuperior" },                 // Superscript right parenthesis
    { 0x207F, "nsuperior" },                          // Superscript latin small letter n
    { 0x2080, "zerosub" },                            // Subscript zero
    { 0x2081, "onesub" },                             // Subscript one
    { 0x2082, "twosub" },                             // Subscript two
    { 0x2083, "threesub" },                           // Subscript three
    { 0x2084, "foursub" },                            // Subscript four
    { 0x2085, "fivesub" },                            // Subscript five
    { 0x2086, "sixsub" },                             // Subscript six
    { 0x2087, "sevensub" },                           // Subscript seven
    { 0x2088, "eightsub" },                           // Subscript eight
    { 0x2089, "ninesub" },                            // Subscript nine
    { 0x208A, "plussub" },                            // Subscript plus sign
    { 0x208B, "minussub" },                           // Subscript minus
    { 0x208C, "equalsub" },                           // Subscript equals sign
    { 0x208D, "parenleftsub" },                       // Subscript left parenthesis
    { 0x208E, "parenrightsub" },                      // Subscript right parenthesis
    { 0x20A0, "Euro" },                               // Euro-currency sign
    { 0x20A1, "colonsign" },                          // Colon sign
    { 0x20A2, "cruzeiro" },                           // Cruzeiro sign
    { 0x20A3, "franc" },                              // French franc sign
    { 0x20A4, "lira" },                               // Lira sign
    { 0x20A5, "mill" },                               // Mill sign
    { 0x20A6, "naira" },                              // Naira sign
    { 0x20A7, "Pts" },                                // Peseta sign
    { 0x20A8, "rupee" },                              // Rupee sign
    { 0x20A9, "won" },                                // Won sign
    { 0x20AA, "sheqelhebrew" },                       // New sheqel sign
    { 0x20AB, "dong" },                               // Dong sign
    { 0x2100, "accountof" },                          // Account of
    { 0x2101, "addresssubject" },                     // Addressed to the subject
    { 0x2102, "Cbb" },                                // Double-struck Capital C
    { 0x2103, "centigrade" },                         // Degree celsius
    { 0x2104, "CL" },                                 // Centre line symbol
    { 0x2105, "careof" },                             // Care of
    { 0x2106, "cadauna" },                            // Cada una
    { 0x2107, "Euler" },                              // Euler constant
    { 0x2108, "scruple" },                            // Scruple
    { 0x2109, "fahrenheit" },                         // Degree fahrenheit
    { 0x210A, "gscript" },                            // Script small g
    { 0x210B, "Hscript" },                            // Script Capital H
    { 0x210C, "Hfraktur" },                           // Black-letter Capital H
    { 0x210D, "Hbb" },                                // Double-struck Capital H
    { 0x210E, "planck" },                             // Planck constant
    { 0x210F, "planckover2pi" },                      // Planck constant over two pi
    { 0x2110, "Iscript" },                            // Script Capital I
    { 0x2111, "Ifraktur" },                           // Black-letter Capital I
    { 0x2112, "Lscript" },                            // Script Capital L
    { 0x2113, "lsquare" },                            // Script small l
    { 0x2114, "lbbar" },                              // L b bar symbol
    { 0x2115, "Nbb" },                                // Double-struck Capital N
    { 0x2116, "numero" },                             // Numero sign
    { 0x2117, "recordright" },                        // Sound recording copyright
    { 0x2118, "weierstrass" },                        // Script Capital P
    { 0x2119, "Pbb" },                                // Double-struck Capital P
    { 0x211A, "Qbb" },                                // Double-struck Capital Q
    { 0x211B, "Rscript" },                            // Script Capital R
    { 0x211C, "Rfraktur" },                           // Black-letter Capital R
    { 0x211D, "Rbb" },                                // Double-struck Capital R
    { 0x211E, "Rx" },                                 // Prescription take
    { 0x211F, "response" },                           // Response
    { 0x2120, "servicemark" },                        // Service mark
    { 0x2121, "tel" },                                // Telephone sign
    { 0x2122, "trademark" },                          // Trade mark sign
    { 0x2123, "versicle" },                           // Versicle
    { 0x2124, "Zbb" },                                // Double-struck Capital Z
    { 0x2125, "ounce" },                              // Ounce sign
    { 0x2126, "ohm" },                                // Ohm sign
    { 0x2127, "mho" },                                // Inverted ohm sign
    { 0x2128, "Zfraktur" },                           // Black-letter Capital Z
    { 0x2129, "iotaturn" },                           // Turned greek small letter iota
    { 0x212A, "degreekelvin" },                       // Kelvin sign
    { 0x212B, "angstrom" },                           // Angstrom sign
    { 0x212C, "Bscript" },                            // Script Capital B
    { 0x212D, "Cfraktur" },                           // Black-letter Capital C
    { 0x212E, "estimated" },                          // Estimated symbol
    { 0x212F, "escript" },                            // Script small e
    { 0x2130, "Escript" },                            // Script Capital E
    { 0x2131, "Fscript" },                            // Script Capital F
    { 0x2132, "Fturn" },                              // Turned Capital F
    { 0x2133, "Mscript" },                            // Script Capital M
    { 0x2134, "oscript" },                            // Script small o
    { 0x2135, "aleph" },                              // Alef symbol
    { 0x2136, "bethmath" },                           // Bet symbol
    { 0x2137, "gimelmath" },                          // Gimel symbol
    { 0x2138, "dalethmath" },                         // Dalet symbol
    { 0x2153, "onethird" },                           // Vulgar fraction one third
    { 0x2154, "twothirds" },                          // Vulgar fraction two thirds
    { 0x2155, "onefifth" },                           // Vulgar fraction one fifth
    { 0x2156, "twofifth" },                           // Vulgar fraction two fifths
    { 0x2157, "threefifth" },                         // Vulgar fraction three fifths
    { 0x2158, "fourfifth" },                          // Vulgar fraction four fifths
    { 0x2159, "onesixth" },                           // Vulgar fraction one sixth
    { 0x215A, "fivesixth" },                          // Vulgar fraction five sixths
    { 0x215B, "oneeighth" },                          // Vulgar fraction one eighth
    { 0x215C, "threeeighths" },                       // Vulgar fraction three eighths
    { 0x215D, "fiveeighths" },                        // Vulgar fraction five eighths
    { 0x215E, "seveneighths" },                       // Vulgar fraction seven eighths
    { 0x215F, "oneover" },                            // Fraction numerator one
    { 0x2160, "romanOne" },                           // Roman numeral one
    { 0x2161, "romanTwo" },                           // Roman numeral two
    { 0x2162, "romanThree" },                         // Roman numeral three
    { 0x2163, "romanFour" },                          // Roman numeral four
    { 0x2164, "romanFive" },                          // Roman numeral five
    { 0x2165, "romanSix" },                           // Roman numeral six
    { 0x2166, "romanSeven" },                         // Roman numeral seven
    { 0x2167, "romanEight" },                         // Roman numeral eight
    { 0x2168, "romanNine" },                          // Roman numeral nine
    { 0x2169, "romanTen" },                           // Roman numeral ten
    { 0x216A, "romanEleven" },                        // Roman numeral eleven
    { 0x216B, "romanTwelve" },                        // Roman numeral twelve
    { 0x216C, "romanFifty" },                         // Roman numeral fifty
    { 0x216D, "romanHundred" },                       // Roman numeral one hundred
    { 0x216E, "romanFivehundred" },                   // Roman numeral five hundred
    { 0x216F, "romanThousand" },                      // Roman numeral one thousand
    { 0x2170, "romanone" },                           // Small roman numeral one
    { 0x2171, "romantwo" },                           // Small roman numeral two
    { 0x2172, "romanthree" },                         // Small roman numeral three
    { 0x2173, "romanfour" },                          // Small roman numeral four
    { 0x2174, "romanfive" },                          // Small roman numeral five
    { 0x2175, "romansix" },                           // Small roman numeral six
    { 0x2176, "romanseven" },                         // Small roman numeral seven
    { 0x2177, "romaneight" },                         // Small roman numeral eight
    { 0x2178, "romannine" },                          // Small roman numeral nine
    { 0x2179, "romanten" },                           // Small roman numeral ten
    { 0x217A, "romaneleven" },                        // Small roman numeral eleven
    { 0x217B, "romantwelve" },                        // Small roman numeral twelve
    { 0x217C, "romanfifty" },                         // Small roman numeral fifty
    { 0x217D, "romanhundred" },                       // Small roman numeral one hundred
    { 0x217E, "romanfivehundred" },                   // Small roman numeral five hundred
    { 0x217F, "romanthousand" },                      // Small roman numeral one thousand
    { 0x2190, "arrowleft" },                          // Leftwards arrow
    { 0x2191, "arrowup" },                            // Upwards arrow
    { 0x2192, "arrowright" },                         // Rightwards arrow
    { 0x2193, "arrowdown" },                          // Downwards arrow
    { 0x2194, "arrowlongboth" },                      // Left right arrow
    { 0x2195, "arrowlongbothv" },                     // Up down arrow
    { 0x2196, "arrownorthwest" },                     // North west arrow
    { 0x2197, "arrownortheast" },                     // North east arrow
    { 0x2198, "arrowsoutheast" },                     // South east arrow
    { 0x2199, "arrowsouthwest" },                     // South west arrow
    { 0x219A, "arrowleftnot" },                       // Leftwards arrow with stroke
    { 0x219B, "arrowrightnot" },                      // Rightwards arrow with stroke
    { 0x219C, "arrowwaveleft" },                      // Leftwards wave arrow
    { 0x219D, "arrowwaveright" },                     // Rightwards wave arrow
    { 0x219E, "dblarrowheadleft" },                   // Leftwards two headed arrow
    { 0x219F, "dblarrowheadup" },                     // Upwards two headed arrow
    { 0x21A0, "dblarrowheadright" },                  // Rightwards two headed arrow
    { 0x21A1, "dblarrowheaddown" },                   // Downwards two headed arrow
    { 0x21A2, "arrowtailleft" },                      // Leftwards arrow with tail
    { 0x21A3, "arrowtailright" },                     // Rightwards arrow with tail
    { 0x21A4, "arrowbarleft" },                       // Leftwards arrow from bar
    { 0x21A5, "arrowbarup" },                         // Upwards arrow from bar
    { 0x21A6, "arrowbarright" },                      // Rightwards arrow from bar
    { 0x21A7, "arrowbardown" },                       // Downwards arrow from bar
    { 0x21A8, "arrowupdnbse" },                       // Up down arrow with base
    { 0x21A9, "arrowhookleft" },                      // Leftwards arrow with hook
    { 0x21AA, "arrowhookright" },                     // Rightwards arrow with hook
    { 0x21AB, "arrowloopleft" },                      // Leftwards arrow with loop
    { 0x21AC, "arrowloopright" },                     // Rightwards arrow with loop
    { 0x21AD, "arrowwaveboth" },                      // Left right wave arrow
    { 0x21AE, "arrowlongbothnot" },                   // Left right arrow with stroke
    { 0x21AF, "arrowzigzag" },                        // Downwards zigzag arrow
    { 0x21B0, "arrowupleft" },                        // Upwards arrow with tip leftwards
    { 0x21B1, "arrowupright" },                       // Upwards arrow with tip rightwards
    { 0x21B2, "arrowdownleft" },                      // Downwards arrow with tip leftwards
    { 0x21B3, "arrowdownright" },                     // Downwards arrow with tip rightwards
    { 0x21B4, "arrowrightdown" },                     // Rightwards arrow with corner downwards
    { 0x21B5, "carriagerreturn" },                    // Downwards arrow with corner leftwards
    { 0x21B6, "arrowsemanticlockw" },                 // Anticlockwise top semicircle arrow
    { 0x21B7, "arrowsemclockw" },                     // Clockwise top semicircle arrow
    { 0x21B8, "home" },                               // North west arrow to long bar
    { 0x21B9, "tableftright" },                       // Leftwards arrow to bar over rightwards arrow to bar
    { 0x21BA, "arrowanticlockw" },                    // Anticlockwise open circle arrow
    { 0x21BB, "arrowclockw" },                        // Clockwise open circle arrow
    { 0x21BC, "arrowlefttophalf" },                   // Leftwards harpoon with barb upwards
    { 0x21BD, "arrowleftbothalf" },                   // Leftwards harpoon with barb downwards
    { 0x21BE, "harpoonupright" },                     // Upwards harpoon with barb rightwards
    { 0x21BF, "harpoonupleft" },                      // Upwards harpoon with barb leftwards
    { 0x21C0, "arrowrighttophalf" },                  // Rightwards harpoon with barb upwards
    { 0x21C1, "arrowrightbothalf" },                  // Rightwards harpoon with barb downwards
    { 0x21C2, "harpoondownright" },                   // Downwards harpoon with barb rightwards
    { 0x21C3, "harpoondownleft" },                    // Downwards harpoon with barb leftwards
    { 0x21C4, "arrowrightoverleft" },                 // Rightwards arrow over leftwards arrow
    { 0x21C5, "dblarrowupdown" },                     // Upwards arrow leftwards of downwards arrow
    { 0x21C6, "arrowleftoverright" },                 // Leftwards arrow over rightwards arrow
    { 0x21C7, "dblarrowleft" },                       // Leftwards paired arrows
    { 0x21C8, "dblarrowup" },                         // Upwards paired arrows
    { 0x21C9, "dblarrowright" },                      // Rightwards paired arrows
    { 0x21CA, "dblarrowdown" },                       // Downwards paired arrows
    { 0x21CB, "harpoonleftright" },                   // Leftwards harpoon over rightwards harpoon
    { 0x21CC, "harpoonrightleft" },                   // Rightwards harpoon over leftwards harpoon
    { 0x21CD, "arrowdblleftnot" },                    // Leftwards double arrow with stroke
    { 0x21CE, "arrowdbllongbothnot" },                // Left right double arrow with stroke
    { 0x21CF, "arrowdblrightnot" },                   // Rightwards double arrow with stroke
    { 0x21D0, "arrowleftdbl" },                       // Leftwards double arrow
    { 0x21D1, "arrowdblup" },                         // Upwards double arrow
    { 0x21D2, "arrowdblright" },                      // Rightwards double arrow
    { 0x21D3, "arrowdbldown" },                       // Downwards double arrow
    { 0x21D4, "arrowdblboth" },                       // Left right double arrow
    { 0x21D5, "arrowdbllongbothv" },                  // Up down double arrow
    { 0x21D6, "arrowdblnw" },                         // North west double arrow
    { 0x21D7, "arrowdblne" },                         // North east double arrow
    { 0x21D8, "arrowdblse" },                         // South east double arrow
    { 0x21D9, "arrowdblsw" },                         // South west double arrow
    { 0x21DA, "arrowtripleleft" },                    // Leftwards triple arrow
    { 0x21DB, "arrowtripleright" },                   // Rightwards triple arrow
    { 0x21DC, "arrowsquiggleleft" },                  // Leftwards squiggle arrow
    { 0x21DD, "arrowsquiggleright" },                 // Rightwards squiggle arrow
    { 0x21DE, "pageup" },                             // Upwards arrow with double stroke
    { 0x21DF, "pagedown" },                           // Downwards arrow with double stroke
    { 0x21E0, "arrowdashleft" },                      // Leftwards dashed arrow
    { 0x21E1, "arrowdashup" },                        // Upwards dashed arrow
    { 0x21E2, "arrowdashright" },                     // Rightwards dashed arrow
    { 0x21E3, "arrowdashdown" },                      // Downwards dashed arrow
    { 0x21E4, "arrowtableft" },                       // Leftwards arrow to bar
    { 0x21E5, "arrowtabright" },                      // Rightwards arrow to bar
    { 0x21E6, "arrowopenleft" },                      // Leftwards white arrow
    { 0x21E7, "arrowopenup" },                        // Upwards white arrow
    { 0x21E8, "arrowopenright" },                     // Rightwards white arrow
    { 0x21E9, "arrowopendown" },                      // Downwards white arrow
    { 0x21EA, "capslock" },                           // Upwards white arrow from bar
    { 0x21F4, "arrowrightring" },                     // Right arrow with small circle
    { 0x21F5, "arrowdownleftup" },                    // Downwards arrow leftwards of upwards arrow
    { 0x21F6, "tplarrowright" },                      // Three rightwards arrows
    { 0x21F7, "arrowleftstroke" },                    // Leftwards arrow with vertical stroke
    { 0x21F8, "arrowrightstroke" },                   // Rightwards arrow with vertical stroke
    { 0x21F9, "arrowleftrightstroke" },               // Left right arrow with vertical stroke
    { 0x21FA, "arrowleftdblstroke" },                 // Leftwards arrow with double vertical stroke
    { 0x21FB, "arrowrightdblstroke" },                // Rightwards arrow with double vertical stroke
    { 0x21FC, "arrowleftrightdblstroke" },            // Left right arrow with double vertical stroke
    { 0x21FD, "arrowleftopenhead" },                  // Leftwards open-headed arrow
    { 0x21FE, "arrowrightopenhead" },                 // Rightwards open-headed arrow
    { 0x21FF, "arrowleftrightopenhead" },             // Left right open-headed arrow
    { 0x2200, "forall" },                             // For all
    { 0x2201, "complement" },                         // Complement
    { 0x2202, "partialdiff" },                        // Partial differential
    { 0x2203, "thereexists" },                        // There exists
    { 0x2204, "notexistential" },                     // There does not exist
    { 0x2205, "emptyset" },                           // Empty set
    { 0x2206, "increment" },                          // Increment
    { 0x2207, "nabla" },                              // Nabla
    { 0x2208, "element" },                            // Element of
    { 0x2209, "notelement" },                         // Not an element of
    { 0x220A, "elementsmall" },                       // Small element of
    { 0x220B, "owner" },                              // Contains as member
    { 0x220C, "notowner" },                           // Does not contain as member
    { 0x220D, "ownersmall" },                         // Small contains as member
    { 0x220E, "eop" },                                // End of proof
    { 0x220F, "product" },                            // N-ary product
    { 0x2210, "coproduct" },                          // N-ary coproduct
    { 0x2211, "summation" },                          // N-ary summation
    { 0x2212, "minus" },                              // Minus sign
    { 0x2213, "minusplus" },                          // Minus-or-plus sign
    { 0x2214, "dotplus" },                            // Dot plus
    { 0x2215, "fraction" },                           // Division slash
    { 0x2216, "backslashmath" },                      // Set minus
    { 0x2217, "asteriskmath" },                       // Asterisk operator
    { 0x2218, "mathcircle" },                         // Ring operator
    { 0x2219, "middot" },                             // Bullet operator
    { 0x221A, "radical" },                            // Square root
    { 0x221B, "cuberoot" },                           // Cube root
    { 0x221C, "fourthroot" },                         // Fourth root
    { 0x221D, "proportional" },                       // Proportional to
    { 0x221E, "infinity" },                           // Infinity
    { 0x221F, "rightangle" },                         // Right angle
    { 0x2220, "angle" },                              // Angle
    { 0x2221, "measuredangle" },                      // Measured angle
    { 0x2222, "sphericalangle" },                     // Spherical angle
    { 0x2223, "bar1" },                               // Divides
    { 0x2224, "notbar" },                             // Does not divide
    { 0x2225, "bardbl2" },                            // Parallel to
    { 0x2226, "notbardbl" },                          // Not parallel to
    { 0x2227, "logicaland" },                         // Logical and
    { 0x2228, "logicalor" },                          // Logical or
    { 0x2229, "intersection" },                       // Intersection
    { 0x222A, "union" },                              // Union
    { 0x222B, "integral" },                           // Integral
    { 0x222C, "dblintegral" },                        // Double integral
    { 0x222D, "integraltrpl" },                       // Triple integral
    { 0x222E, "contintegral" },                       // Contour integral
    { 0x222F, "surfintegral" },                       // Surface integral
    { 0x2230, "volintegral" },                        // Volume integral
    { 0x2231, "clwintegral" },                        // Clockwise integral
    { 0x2232, "clwcontintegral" },                    // Clockwise contour integral
    { 0x2233, "cclwcontintegral" },                   // Anticlockwise contour integral
    { 0x2234, "therefore" },                          // Therefore
    { 0x2235, "because" },                            // Because
    { 0x2236, "ratio" },                              // Ratio
    { 0x2237, "proportion" },                         // Proportion
    { 0x2238, "dotminus" },                           // Dot minus
    { 0x2239, "excess" },                             // Excess
    { 0x223A, "geomproportion" },                     // Geometric proportion
    { 0x223B, "homothetic" },                         // Homothetic
    { 0x223C, "similar" },                            // Tilde operator
    { 0x223D, "revsimilar" },                         // Reversed tilde
    { 0x223E, "lazysinv" },                           // Inverted lazy s
    { 0x223F, "sine" },                               // Sine wave
    { 0x2240, "wreathproduct" },                      // Wreath product
    { 0x2241, "notsimilar" },                         // Not tilde
    { 0x2242, "minustilde" },                         // Minus tilde
    { 0x2243, "asymptequal" },                        // Asymptotically equal to
    { 0x2244, "notasymptequal" },                     // Not asymptotically equal to
    { 0x2245, "congruent" },                          // Approximately equal to
    { 0x2246, "approxnotequal" },                     // Approximately but not actually equal to
    { 0x2247, "notapproxequal" },                     // Neither approximately nor actually equal to
    { 0x2248, "approxequal" },                        // Almost equal to
    { 0x2249, "notalmostequal" },                     // Not almost equal to
    { 0x224A, "almostorequal" },                      // Almost equal or equal to
    { 0x224B, "tildetrpl" },                          // Triple tilde
    { 0x224C, "allequal" },                           // All equal to
    { 0x224D, "equivasymptotic" },                    // Equivalent to
    { 0x224E, "geomequivalent" },                     // Geometrically equivalent to
    { 0x224F, "difference" },                         // Difference between
    { 0x2250, "approaches" },                         // Approaches the limit
    { 0x2251, "geomequal" },                          // Geometrically equal to
    { 0x2252, "approxequalorimage" },                 // Approximately equal to or the image of
    { 0x2253, "imageorapproxequal" },                 // Image of or approximately equal to
    { 0x2254, "colonequal" },                         // Colon equals
    { 0x2255, "equalcolon" },                         // Equals colon
    { 0x2256, "ringinequal" },                        // Ring in equal to
    { 0x2257, "ringequal" },                          // Ring equal to
    { 0x2258, "corresponds" },                        // Corresponds to
    { 0x2259, "estimates" },                          // Estimates
    { 0x225A, "equiangular" },                        // Equiangular to
    { 0x225B, "starequal" },                          // Star equals
    { 0x225C, "deltaequal" },                         // Delta equal to
    { 0x225D, "definequal" },                         // Equal to by definition
    { 0x225E, "measurequal" },                        // Measured by
    { 0x225F, "questionequal" },                      // Questioned equal to
    { 0x2260, "notequal" },                           // Not equal to
    { 0x2261, "equivalence" },                        // Identical to
    { 0x2262, "notidentical" },                       // Not identical to
    { 0x2263, "strictequivalence" },                  // Strictly equivalent to
    { 0x2264, "lessequal" },                          // Less-than or equal to
    { 0x2265, "greaterequal" },                       // Greater-than or equal to
    { 0x2266, "lessdblequal" },                       // Less-than over equal to
    { 0x2267, "greaterdblequal" },                    // Greater-than over equal to
    { 0x2268, "lessnotdblequal" },                    // Less-than but not equal to
    { 0x2269, "greaternotdblequal" },                 // Greater-than but not equal to
    { 0x226A, "muchless" },                           // Much less-than
    { 0x226B, "muchgreater" },                        // Much greater-than
    { 0x226C, "between" },                            // Between
    { 0x226D, "notequivasymptotic" },                 // Not equivalent to
    { 0x226E, "notless" },                            // Not less-than
    { 0x226F, "notgreater" },                         // Not greater-than
    { 0x2270, "notlessequal1" },                      // Neither less-than nor equal to
    { 0x2271, "notgreaterequal1" },                   // Neither greater-than nor equal to
    { 0x2272, "lessequivlnt" },                       // Less-than or equivalent to
    { 0x2273, "greaterequivlnt" },                    // Greater-than or equivalent to
    { 0x2274, "notlessequivlnt" },                    // Neither less-than nor equivalent to
    { 0x2275, "notgreaterequivlnt" },                 // Neither greater-than nor equivalent to
    { 0x2276, "lessorgreater" },                      // Less-than or greater-than
    { 0x2277, "greaterorless" },                      // Greater-than or less-than
    { 0x2278, "notlessgreater" },                     // Neither less-than nor greater-than
    { 0x2279, "notgreaterless" },                     // Neither greater-than nor less-than
    { 0x227A, "precedes" },                           // Precedes
    { 0x227B, "follows" },                            // Succeeds
    { 0x227C, "precedesequal1" },                     // Precedes or equal to
    { 0x227D, "followsequal1" },                      // Succeeds or equal to
    { 0x227E, "precedequivlnt" },                     // Precedes or equivalent to
    { 0x227F, "followsequivlnt" },                    // Succeeds or equivalent to
    { 0x2280, "notpreceeds" },                        // Does not precede
    { 0x2281, "notfollows" },                         // Does not succeed
    { 0x2282, "subset" },                             // Subset of
    { 0x2283, "superset" },                           // Superset of
    { 0x2284, "notsubset" },                          // Not a subset of
    { 0x2285, "notsuperset" },                        // Not a superset of
    { 0x2286, "reflexsubset" },                       // Subset of or equal to
    { 0x2287, "reflexsuperset" },                     // Superset of or equal to
    { 0x2288, "notreflexsubset" },                    // Neither a subset of nor equal to
    { 0x2289, "notreflexsuperset" },                  // Neither a superset of nor equal to
    { 0x228A, "subsetnotequal" },                     // Subset of with not equal to
    { 0x228B, "supersetnotequal" },                   // Superset of with not equal to
    { 0x228C, "multiset" },                           // Multiset
    { 0x228D, "multiplymultiset" },                   // Multiset multiplication
    { 0x228E, "unionmulti" },                         // Multiset union
    { 0x228F, "squareimage" },                        // Square image of
    { 0x2290, "squareoriginal" },                     // Square original of
    { 0x2291, "subsetsqequal" },                      // Square image of or equal to
    { 0x2292, "supersetsqequal" },                    // Square original of or equal to
    { 0x2293, "intersectionsq" },                     // Square cap
    { 0x2294, "unionsq" },                            // Square cup
    { 0x2295, "pluscircle" },                         // Circled plus
    { 0x2296, "minuscircle" },                        // Circled minus
    { 0x2297, "timescircle" },                        // Circled times
    { 0x2298, "circledivide" },                       // Circled division slash
    { 0x2299, "circleot" },                           // Circled dot operator
    { 0x229A, "circlering" },                         // Circled ring operator
    { 0x229B, "circleasterisk" },                     // Circled asterisk operator
    { 0x229C, "circleequal" },                        // Circled equals
    { 0x229D, "circleminus1" },                       // Circled dash
    { 0x229E, "squareplus" },                         // Squared plus
    { 0x229F, "squareminus" },                        // Squared minus
    { 0x22A0, "squaremultiply" },                     // Squared times
    { 0x22A1, "squaredot" },                          // Squared dot operator
    { 0x22A2, "turnstileleft" },                      // Right tack
    { 0x22A3, "tackleft" },                           // Left tack
    { 0x22A4, "tackdown" },                           // Down tack
    { 0x22A5, "perpendicular" },                      // Up tack
    { 0x22A6, "assertion" },                          // Assertion
    { 0x22A7, "truestate" },                          // Models
    { 0x22A8, "satisfy" },                            // True
    { 0x22A9, "force" },                              // Forces
    { 0x22AA, "tacktrpl" },                           // Triple vertical bar right turnstile
    { 0x22AB, "forceextr" },                          // Double vertical bar double right turnstile
    { 0x22AC, "notturnstileleft" },                   // Does not prove
    { 0x22AD, "notsatisfy" },                         // Not true
    { 0x22AE, "notforce" },                           // Does not force
    { 0x22AF, "notforceextr" },                       // Negated double vertical bar double right turnstile
    { 0x22B0, "lowerrank" },                          // Precedes under relation
    { 0x22B1, "higherrank" },                         // Succeeds under relation
    { 0x22B2, "triangleleft" },                       // Normal subgroup of
    { 0x22B3, "triangleright" },                      // Contains as normal subgroup
    { 0x22B4, "triangleftequal" },                    // Normal subgroup of or equal to
    { 0x22B5, "triangrightequal" },                   // Contains as normal subgroup or equal to
    { 0x22B6, "original" },                           // Original of
    { 0x22B7, "image" },                              // Image of
    { 0x22B8, "multimap" },                           // Multimap
    { 0x22B9, "hermitconjmatrix" },                   // Hermitian conjugate matrix
    { 0x22BA, "intercal" },                           // Intercalate
    { 0x22BB, "xor" },                                // Xor
    { 0x22BC, "nand" },                               // Nand
    { 0x22BD, "nor" },                                // Nor
    { 0x22BE, "rightanglearc" },                      // Right angle with arc
    { 0x22BF, "righttriangle" },                      // Right triangle
    { 0x22C0, "narylogicaland" },                     // N-ary logical and
    { 0x22C1, "narylogicalor" },                      // N-ary logical or
    { 0x22C2, "naryintersection" },                   // N-ary intersection
    { 0x22C3, "naryunion" },                          // N-ary union
    { 0x22C4, "diamondmath" },                        // Diamond operator
    { 0x22C5, "dotmath" },                            // Dot operator
    { 0x22C6, "star" },                               // Star operator
    { 0x22C7, "divideonmultiply" },                   // Division times
    { 0x22C8, "bowtie" },                             // Bowtie
    { 0x22C9, "multicloseleft" },                     // Left normal factor semidirect product
    { 0x22CA, "multicloseright" },                    // Right normal factor semidirect product
    { 0x22CB, "multiopenleft" },                      // Left semidirect product
    { 0x22CC, "multiopenright" },                     // Right semidirect product
    { 0x22CD, "revasymptequal" },                     // Reversed tilde equals
    { 0x22CE, "curlor" },                             // Curly logical or
    { 0x22CF, "curland" },                            // Curly logical and
    { 0x22D0, "subsetdbl" },                          // Double subset
    { 0x22D1, "supersetdbl" },                        // Double superset
    { 0x22D2, "uniondbl" },                           // Double intersection
    { 0x22D3, "intersectiondbl" },                    // Double union
    { 0x22D4, "fork" },                               // Pitchfork
    { 0x22D5, "equalparallel" },                      // Equal and parallel to
    { 0x22D6, "lessdot" },                            // Less-than with dot
    { 0x22D7, "greaterdot" },                         // Greater-than with dot
    { 0x22D8, "verymuchless" },                       // Very much less-than
    { 0x22D9, "verymuchgreater" },                    // Very much greater-than
    { 0x22DA, "lessequalgreater" },                   // Less-than equal to or greater-than
    { 0x22DB, "greaterequalless" },                   // Greater-than equal to or less-than
    { 0x22DC, "equalless" },                          // Equal to or less-than
    { 0x22DD, "equalgreater" },                       // Equal to or greater-than
    { 0x22DE, "equalprecedes1" },                     // Equal to or precedes
    { 0x22DF, "equalfollows1" },                      // Equal to or succeeds
    { 0x22E0, "preceedsnotequal" },                   // Does not precede or equal
    { 0x22E1, "followsnotequal" },                    // Does not succeed or equal
    { 0x22E2, "notsubsetsqequal" },                   // Not square image of or equal to
    { 0x22E3, "notsupersetsqequal" },                 // Not square original of or equal to
    { 0x22E4, "sqimageornotequal" },                  // Square image of or not equal to
    { 0x22E5, "sqoriginornotequal" },                 // Square original of or not equal to
    { 0x22E6, "lessnotequivlnt" },                    // Less-than but not equivalent to
    { 0x22E7, "greaternotequivlnt" },                 // Greater-than but not equivalent to
    { 0x22E8, "preceedsnotsimilar" },                 // Precedes but not equivalent to
    { 0x22E9, "followsnotequivlnt" },                 // Succeeds but not equivalent to
    { 0x22EA, "nottriangleleft" },                    // Not normal subgroup of
    { 0x22EB, "nottriangleright" },                   // Does not contain as normal subgroup
    { 0x22EC, "nottriangleftequal" },                 // Not normal subgroup of or equal to
    { 0x22ED, "nottriangrightequal" },                // Does not contain as normal subgroup or equal
    { 0x22EE, "vertellipsis" },                       // Vertical ellipsis
    { 0x22EF, "midhorizellipsis" },                   // Midline horizontal ellipsis
    { 0x22F0, "upslopeellipsis" },                    // Up right diagonal ellipsis
    { 0x22F1, "downslopeellipsis" },                  // Down right diagonal ellipsis
    { 0x2300, "diameter" },                           // Diameter sign
    { 0x2302, "house" },                              // House
    { 0x2303, "control" },                            // Up arrowhead
    { 0x2304, "arrowheaddown" },                      // Down arrowhead
    { 0x2305, "projection" },                         // Projective
    { 0x2306, "perspective" },                        // Perspective
    { 0x2307, "wavyline" },                           // Wavy line
    { 0x2308, "ceilingleft" },                        // Left ceiling
    { 0x2309, "ceilingright" },                       // Right ceiling
    { 0x230A, "floorleft" },                          // Left floor
    { 0x230B, "floorright" },                         // Right floor
    { 0x230C, "cropbottomright" },                    // Bottom right crop
    { 0x230D, "cropbottomleft" },                     // Bottom left crop
    { 0x230E, "croptopright" },                       // Top right crop
    { 0x230F, "croptopleft" },                        // Top left crop
    { 0x2310, "revlogicalnot" },                      // Reversed not sign
    { 0x2311, "squarelozenge" },                      // Square lozenge
    { 0x2312, "arc" },                                // Arc
    { 0x2313, "segment" },                            // Segment
    { 0x2314, "sector" },                             // Sector
    { 0x2315, "telrecorder" },                        // Telephone recorder
    { 0x2316, "positionindicator" },                  // Position indicator
    { 0x2317, "viewdatasquare" },                     // Viewdata square
    { 0x2318, "propellor" },                          // Place of interest sign
    { 0x2319, "turnednot" },                          // Turned not sign
    { 0x231A, "watch" },                              // Watch
    { 0x231B, "hourglass" },                          // Hourglass
    { 0x231C, "ulcorner" },                           // Top left corner
    { 0x231D, "urcorner" },                           // Top right corner
    { 0x231E, "dlcorner" },                           // Bottom left corner
    { 0x231F, "drcorner" },                           // Bottom right corner
    { 0x2320, "integraltp" },                         // Top half integral
    { 0x2321, "integralbt" },                         // Bottom half integral
    { 0x2322, "slurabove" },                          // Frown
    { 0x2323, "slurbelow" },                          // Smile
    { 0x2324, "insert" },                             // Up arrowhead between two horizontal bars
    { 0x2325, "option" },                             // Option key
    { 0x2326, "eraseleft" },                          // Erase to the right
    { 0x2327, "clear" },                              // X in a rectangle box
    { 0x2328, "keyboard" },                           // Keyboard
    { 0x2329, "angleleft" },                          // Left-pointing angle bracket
    { 0x232A, "angleright" },                         // Right-pointing angle bracket
    { 0x232B, "deleteleft" },                         // Erase to the left
    { 0x232C, "benzene" },                            // Benzene ring
    { 0x232D, "cylindicity" },                        // Cylindricity
    { 0x232E, "allaround" },                          // All around-profile
    { 0x232F, "symmetry" },                           // Symmetry
    { 0x2330, "totalrunnout" },                       // Total runout
    { 0x2331, "dimensionorigin" },                    // Dimension origin
    { 0x2332, "conicaltaper" },                       // Conical taper
    { 0x2333, "slope" },                              // Slope
    { 0x2334, "counterbore" },                        // Counterbore
    { 0x2335, "countersink" },                        // Countersink
    { 0x2336, "Ibeam" },                              // Apl functional symbol i-beam
    { 0x2337, "squishquad" },                         // Apl functional symbol squish quad
    { 0x2338, "squareequal" },                        // Apl functional symbol quad equal
    { 0x2339, "squaredivide" },                       // Apl functional symbol quad divide
    { 0x233A, "squarediamond" },                      // Apl functional symbol quad diamond
    { 0x233B, "squarejot" },                          // Apl functional symbol quad jot
    { 0x233C, "squarecircle" },                       // Apl functional symbol quad circle
    { 0x233D, "circlestile" },                        // Apl functional symbol circle stile
    { 0x233E, "circlejot" },                          // Apl functional symbol circle jot
    { 0x233F, "slashbar" },                           // Apl functional symbol slash bar
    { 0x2340, "backslashbar" },                       // Apl functional symbol backslash bar
    { 0x2341, "squareslash" },                        // Apl functional symbol quad slash
    { 0x2342, "squarebackslash" },                    // Apl functional symbol quad backslash
    { 0x2343, "squareless" },                         // Apl functional symbol quad less-than
    { 0x2344, "squaregreater" },                      // Apl functional symbol quad greater-than
    { 0x2345, "vaneleft" },                           // Apl functional symbol leftwards vane
    { 0x2346, "vaneright" },                          // Apl functional symbol rightwards vane
    { 0x2347, "squarearrowleft" },                    // Apl functional symbol quad leftwards arrow
    { 0x2348, "squarearrowright" },                   // Apl functional symbol quad rightwards arrow
    { 0x2349, "circlebackslash" },                    // Apl functional symbol circle backslash
    { 0x234A, "tackdownunderbar" },                   // Apl functional symbol down tack underbar
    { 0x234B, "deltastile" },                         // Apl functional symbol delta stile
    { 0x234C, "squarecaretdown" },                    // Apl functional symbol quad down caret
    { 0x234D, "squaredelta" },                        // Apl functional symbol quad delta
    { 0x234E, "tackdownjot" },                        // Apl functional symbol down tack jot
    { 0x234F, "vaneup" },                             // Apl functional symbol upwards vane
    { 0x2350, "squarearrowup" },                      // Apl functional symbol quad upwards arrow
    { 0x2351, "tackupoverbar" },                      // Apl functional symbol up tack overbar
    { 0x2352, "delstile" },                           // Apl functional symbol del stile
    { 0x2353, "squarecaretup" },                      // Apl functional symbol quad up caret
    { 0x2354, "squaredel" },                          // Apl functional symbol quad del
    { 0x2355, "tackupjot" },                          // Apl functional symbol up tack jot
    { 0x2356, "vanedown" },                           // Apl functional symbol downwards vane
    { 0x2357, "squarearrowdown" },                    // Apl functional symbol quad downwards arrow
    { 0x2358, "quoteunderbar" },                      // Apl functional symbol quote underbar
    { 0x2359, "deltaunderbar" },                      // Apl functional symbol delta underbar
    { 0x235A, "diamondunderbar" },                    // Apl functional symbol diamond underbar
    { 0x235B, "jotunderbar" },                        // Apl functional symbol jot underbar
    { 0x235C, "circleunderbar" },                     // Apl functional symbol circle underbar
    { 0x235D, "shoejotup" },                          // Apl functional symbol up shoe jot
    { 0x235E, "quotequad" },                          // Apl functional symbol quote quad
    { 0x235F, "circlestar" },                         // Apl functional symbol circle star
    { 0x2360, "squarecolon" },                        // Apl functional symbol quad colon
    { 0x2361, "tackupdiaeresis" },                    // Apl functional symbol up tack diaeresis
    { 0x2362, "deldiaeresis" },                       // Apl functional symbol del diaeresis
    { 0x2363, "stardiaeresis" },                      // Apl functional symbol star diaeresis
    { 0x2364, "jotdiaeresis" },                       // Apl functional symbol jot diaeresis
    { 0x2365, "circlediaeresis" },                    // Apl functional symbol circle diaeresis
    { 0x2366, "shoedownstile" },                      // Apl functional symbol down shoe stile
    { 0x2367, "shoeleftstile" },                      // Apl functional symbol left shoe stile
    { 0x2368, "tildediaeresis" },                     // Apl functional symbol tilde diaeresis
    { 0x2369, "greaterdiaeresis" },                   // Apl functional symbol greater-than diaeresis
    { 0x236A, "commabar" },                           // Apl functional symbol comma bar
    { 0x236B, "deltilde" },                           // Apl functional symbol del tilde
    { 0x236C, "zilde" },                              // Apl functional symbol zilde
    { 0x236D, "stiletilde" },                         // Apl functional symbol stile tilde
    { 0x236E, "semicolonunderbar" },                  // Apl functional symbol semicolon underbar
    { 0x236F, "squarenotequal" },                     // Apl functional symbol quad not equal
    { 0x2370, "squarequestion" },                     // Apl functional symbol quad question
    { 0x2371, "caretdowntilde" },                     // Apl functional symbol down caret tilde
    { 0x2372, "caretuptilde" },                       // Apl functional symbol up caret tilde
    { 0x2376, "alphaunderbar" },                      // Apl functional symbol alpha underbar
    { 0x2377, "epsilonunderbar" },                    // Apl functional symbol epsilon underbar
    { 0x2378, "iotaunderbar" },                       // Apl functional symbol iota underbar
    { 0x2379, "omegaunderbar" },                      // Apl functional symbol omega underbar
    { 0x237C, "anglerigthzigzagdown" },               // Right angle with downwards zigzag arrow
    { 0x2400, "null" },                               // Symbol for null
    { 0x2401, "startofhead" },                        // Symbol for start of heading
    { 0x2402, "starttext" },                          // Symbol for start of text
    { 0x2403, "endtext" },                            // Symbol for end of text
    { 0x2404, "endtrans" },                           // Symbol for end of transmission
    { 0x2405, "enquiry" },                            // Symbol for enquiry
    { 0x2406, "acknowledge" },                        // Symbol for acknowledge
    { 0x2407, "bell" },                               // Symbol for bell
    { 0x2408, "backspace" },                          // Symbol for backspace
    { 0x2409, "horiztab" },                           // Symbol for horizontal tabulation
    { 0x240A, "linefeed" },                           // Symbol for line feed
    { 0x240B, "verttab" },                            // Symbol for vertical tabulation
    { 0x240C, "formfeed" },                           // Symbol for form feed
    { 0x240D, "carriagereturn" },                     // Symbol for carriage return
    { 0x240E, "shiftout" },                           // Symbol for shift out
    { 0x240F, "shiftin" },                            // Symbol for shift in
    { 0x2410, "datalinkescape" },                     // Symbol for data link escape
    { 0x2411, "devcon1" },                            // Symbol for device control one
    { 0x2412, "devcon2" },                            // Symbol for device control two
    { 0x2413, "devcon3" },                            // Symbol for device control three
    { 0x2414, "devcon4" },                            // Symbol for device control four
    { 0x2415, "negacknowledge" },                     // Symbol for negative acknowledge
    { 0x2416, "synch" },                              // Symbol for synchronous idle
    { 0x2417, "endtransblock" },                      // Symbol for end of transmission block
    { 0x2418, "cancel" },                             // Symbol for cancel
    { 0x2419, "endmedium" },                          // Symbol for end of medium
    { 0x241A, "substitute" },                         // Symbol for substitute
    { 0x241B, "escape" },                             // Symbol for escape
    { 0x241C, "fileseparator" },                      // Symbol for file separator
    { 0x241D, "groupseparator" },                     // Symbol for group separator
    { 0x241E, "recordseparator" },                    // Symbol for record separator
    { 0x241F, "unitseparator" },                      // Symbol for unit separator
    { 0x2420, "spaceliteral" },                       // Symbol for space
    { 0x2421, "delete" },                             // Symbol for delete
    { 0x2422, "blankb" },                             // Blank symbol
    { 0x2423, "blank" },                              // Open box
    { 0x2424, "newline" },                            // Symbol for newline
    { 0x2440, "ocrhook" },                            // Ocr hook
    { 0x2441, "ocrchair" },                           // Ocr chair
    { 0x2442, "ocrfork" },                            // Ocr fork
    { 0x2443, "ocrinvertedfork" },                    // Ocr inverted fork
    { 0x2444, "ocrbeltbuckle" },                      // Ocr belt buckle
    { 0x2445, "ocrbowtie" },                          // Ocr bow tie
    { 0x2446, "ocrbranchbankidentification" },        // Ocr branch bank identification
    { 0x2447, "ocramountofcheck" },                   // Ocr amount of check
    { 0x2448, "ocrdash" },                            // Ocr dash
    { 0x2449, "ocrcustomeraccountnumber" },           // Ocr customer account number
    { 0x244A, "ocrdoublebackslash" },                 // Ocr double backslash
    { 0x2460, "onecircle" },                          // Circled digit one
    { 0x2461, "twocircle" },                          // Circled digit two
    { 0x2462, "threecircle" },                        // Circled digit three
    { 0x2463, "fourcircle" },                         // Circled digit four
    { 0x2464, "fivecircle" },                         // Circled digit five
    { 0x2465, "sixcircle" },                          // Circled digit six
    { 0x2466, "sevencircle" },                        // Circled digit seven
    { 0x2467, "eightcircle" },                        // Circled digit eight
    { 0x2468, "ninecircle" },                         // Circled digit nine
    { 0x2469, "tencircle" },                          // Circled number ten
    { 0x246A, "elevencircle" },                       // Circled number eleven
    { 0x246B, "twelvecircle" },                       // Circled number twelve
    { 0x246C, "thirteencircle" },                     // Circled number thirteen
    { 0x246D, "fourteencircle" },                     // Circled number fourteen
    { 0x246E, "fifteencircle" },                      // Circled number fifteen
    { 0x246F, "sixteencircle" },                      // Circled number sixteen
    { 0x2470, "seventeencircle" },                    // Circled number seventeen
    { 0x2471, "eighteencircle" },                     // Circled number eighteen
    { 0x2472, "nineteencircle" },                     // Circled number nineteen
    { 0x2473, "twentycircle" },                       // Circled number twenty
    { 0x2474, "oneparen" },                           // Parenthesized digit one
    { 0x2475, "twoparen" },                           // Parenthesized digit two
    { 0x2476, "threeparen" },                         // Parenthesized digit three
    { 0x2477, "fourparen" },                          // Parenthesized digit four
    { 0x2478, "fiveparen" },                          // Parenthesized digit five
    { 0x2479, "sixparen" },                           // Parenthesized digit six
    { 0x247A, "sevenparen" },                         // Parenthesized digit seven
    { 0x247B, "eightparen" },                         // Parenthesized digit eight
    { 0x247C, "nineparen" },                          // Parenthesized digit nine
    { 0x247D, "tenparen" },                           // Parenthesized number ten
    { 0x247E, "elevenparen" },                        // Parenthesized number eleven
    { 0x247F, "twelveparen" },                        // Parenthesized number twelve
    { 0x2480, "thirteenparen" },                      // Parenthesized number thirteen
    { 0x2481, "fourteenparen" },                      // Parenthesized number fourteen
    { 0x2482, "fifteenparen" },                       // Parenthesized number fifteen
    { 0x2483, "sixteenparen" },                       // Parenthesized number sixteen
    { 0x2484, "seventeenparen" },                     // Parenthesized number seventeen
    { 0x2485, "eighteenparen" },                      // Parenthesized number eighteen
    { 0x2486, "nineteenparen" },                      // Parenthesized number nineteen
    { 0x2487, "twentyparen" },                        // Parenthesized number twenty
    { 0x2488, "oneperiod" },                          // Digit one full stop
    { 0x2489, "twoperiod" },                          // Digit two full stop
    { 0x248A, "threeperiod" },                        // Digit three full stop
    { 0x248B, "fourperiod" },                         // Digit four full stop
    { 0x248C, "fiveperiod" },                         // Digit five full stop
    { 0x248D, "sixperiod" },                          // Digit six full stop
    { 0x248E, "sevenperiod" },                        // Digit seven full stop
    { 0x248F, "eightperiod" },                        // Digit eight full stop
    { 0x2490, "nineperiod" },                         // Digit nine full stop
    { 0x2491, "tenperiod" },                          // Number ten full stop
    { 0x2492, "elevenperiod" },                       // Number eleven full stop
    { 0x2493, "twelveperiod" },                       // Number twelve full stop
    { 0x2494, "thirteenperiod" },                     // Number thirteen full stop
    { 0x2495, "fourteenperiod" },                     // Number fourteen full stop
    { 0x2496, "fifteenperiod" },                      // Number fifteen full stop
    { 0x2497, "sixteenperiod" },                      // Number sixteen full stop
    { 0x2498, "seventeenperiod" },                    // Number seventeen full stop
    { 0x2499, "eighteenperiod" },                     // Number eighteen full stop
    { 0x249A, "nineteenperiod" },                     // Number nineteen full stop
    { 0x249B, "twentyperiod" },                       // Number twenty full stop
    { 0x249C, "aparen" },                             // Parenthesized latin small letter a
    { 0x249D, "bparen" },                             // Parenthesized latin small letter b
    { 0x249E, "cparen" },                             // Parenthesized latin small letter c
    { 0x249F, "dparen" },                             // Parenthesized latin small letter d
    { 0x24A0, "eparen" },                             // Parenthesized latin small letter e
    { 0x24A1, "fparen" },                             // Parenthesized latin small letter f
    { 0x24A2, "gparen" },                             // Parenthesized latin small letter g
    { 0x24A3, "hparen" },                             // Parenthesized latin small letter h
    { 0x24A4, "iparen" },                             // Parenthesized latin small letter i
    { 0x24A5, "jparen" },                             // Parenthesized latin small letter j
    { 0x24A6, "kparen" },                             // Parenthesized latin small letter k
    { 0x24A7, "lparen" },                             // Parenthesized latin small letter l
    { 0x24A8, "mparen" },                             // Parenthesized latin small letter m
    { 0x24A9, "nparen" },                             // Parenthesized latin small letter n
    { 0x24AA, "oparen" },                             // Parenthesized latin small letter o
    { 0x24AB, "pparen" },                             // Parenthesized latin small letter p
    { 0x24AC, "qparen" },                             // Parenthesized latin small letter q
    { 0x24AD, "rparen" },                             // Parenthesized latin small letter r
    { 0x24AE, "sparen" },                             // Parenthesized latin small letter s
    { 0x24AF, "tparen" },                             // Parenthesized latin small letter t
    { 0x24B0, "uparen" },                             // Parenthesized latin small letter u
    { 0x24B1, "vparen" },                             // Parenthesized latin small letter v
    { 0x24B2, "wparen" },                             // Parenthesized latin small letter w
    { 0x24B3, "xparen" },                             // Parenthesized latin small letter x
    { 0x24B4, "yparen" },                             // Parenthesized latin small letter y
    { 0x24B5, "zparen" },                             // Parenthesized latin small letter z
    { 0x24B6, "Acircle" },                            // Circled latin Capital Letter A
    { 0x24B7, "Bcircle" },                            // Circled latin Capital Letter B
    { 0x24B8, "Ccircle" },                            // Circled latin Capital Letter C
    { 0x24B9, "Dcircle" },                            // Circled latin Capital Letter D
    { 0x24BA, "Ecircle" },                            // Circled latin Capital Letter E
    { 0x24BB, "Fcircle" },                            // Circled latin Capital Letter F
    { 0x24BC, "Gcircle" },                            // Circled latin Capital Letter G
    { 0x24BD, "Hcircle" },                            // Circled latin Capital Letter H
    { 0x24BE, "Icircle" },                            // Circled latin Capital Letter I
    { 0x24BF, "Jcircle" },                            // Circled latin Capital Letter J
    { 0x24C0, "Kcircle" },                            // Circled latin Capital Letter K
    { 0x24C1, "Lcircle" },                            // Circled latin Capital Letter L
    { 0x24C2, "Mcircle" },                            // Circled latin Capital Letter M
    { 0x24C3, "Ncircle" },                            // Circled latin Capital Letter N
    { 0x24C4, "Ocircle" },                            // Circled latin Capital Letter O
    { 0x24C5, "Pcircle" },                            // Circled latin Capital Letter P
    { 0x24C6, "Qcircle" },                            // Circled latin Capital Letter Q
    { 0x24C7, "Rcircle" },                            // Circled latin Capital Letter R
    { 0x24C8, "Scircle" },                            // Circled latin Capital Letter S
    { 0x24C9, "Tcircle" },                            // Circled latin Capital Letter T
    { 0x24CA, "Ucircle" },                            // Circled latin Capital Letter U
    { 0x24CB, "Vcircle" },                            // Circled latin Capital Letter V
    { 0x24CC, "Wcircle" },                            // Circled latin Capital Letter W
    { 0x24CD, "Xcircle" },                            // Circled latin Capital Letter X
    { 0x24CE, "Ycircle" },                            // Circled latin Capital Letter Y
    { 0x24CF, "Zcircle" },                            // Circled latin Capital Letter Z
    { 0x24D0, "acircle" },                            // Circled latin small letter a
    { 0x24D1, "bcircle" },                            // Circled latin small letter b
    { 0x24D2, "ccircle" },                            // Circled latin small letter c
    { 0x24D3, "dcircle" },                            // Circled latin small letter d
    { 0x24D4, "ecircle" },                            // Circled latin small letter e
    { 0x24D5, "fcircle" },                            // Circled latin small letter f
    { 0x24D6, "gcircle" },                            // Circled latin small letter g
    { 0x24D7, "hcircle" },                            // Circled latin small letter h
    { 0x24D8, "icircle" },                            // Circled latin small letter i
    { 0x24D9, "jcircle" },                            // Circled latin small letter j
    { 0x24DA, "kcircle" },                            // Circled latin small letter k
    { 0x24DB, "lcircle" },                            // Circled latin small letter l
    { 0x24DC, "mcircle" },                            // Circled latin small letter m
    { 0x24DD, "ncircle" },                            // Circled latin small letter n
    { 0x24DE, "ocircle" },                            // Circled latin small letter o
    { 0x24DF, "pcircle" },                            // Circled latin small letter p
    { 0x24E0, "qcircle" },                            // Circled latin small letter q
    { 0x24E1, "rcircle" },                            // Circled latin small letter r
    { 0x24E2, "scircle" },                            // Circled latin small letter s
    { 0x24E3, "tcircle" },                            // Circled latin small letter t
    { 0x24E4, "ucircle" },                            // Circled latin small letter u
    { 0x24E5, "vcircle" },                            // Circled latin small letter v
    { 0x24E6, "wcircle" },                            // Circled latin small letter w
    { 0x24E7, "xcircle" },                            // Circled latin small letter x
    { 0x24E8, "ycircle" },                            // Circled latin small letter y
    { 0x24E9, "zcircle" },                            // Circled latin small letter z
    { 0x24EA, "zerocircled" },                        // Circled digit zero
    { 0x2500, "horizontallight" },                    // Box drawings light horizontal
    { 0x2501, "horizontalheavy" },                    // Box drawings heavy horizontal
    { 0x2502, "verticallight" },                      // Box drawings light vertical
    { 0x2503, "verticalheavy" },                      // Box drawings heavy vertical
    { 0x2504, "horizontaltripledashlight" },          // Box drawings light triple dash horizontal
    { 0x2505, "horizontaltripledashheavy" },          // Box drawings heavy triple dash horizontal
    { 0x2506, "verticaltripledashlight" },            // Box drawings light triple dash vertical
    { 0x2507, "verticaltripledashheavy" },            // Box drawings heavy triple dash vertical
    { 0x2508, "horizontalquadrupledashlight" },       // Box drawings light quadruple dash horizontal
    { 0x2509, "horizontalquadrupledashheavy" },       // Box drawings heavy quadruple dash horizontal
    { 0x250A, "verticalquadrupledashlight" },         // Box drawings light quadruple dash vertical
    { 0x250B, "verticalquadrupledashheavy" },         // Box drawings heavy quadruple dash vertical
    { 0x250C, "downrightlight" },                     // Box drawings light down and right
    { 0x250D, "downlightrightheavy" },                // Box drawings down light and right heavy
    { 0x250E, "downheavyrightlight" },                // Box drawings down heavy and right light
    { 0x250F, "downrightheavy" },                     // Box drawings heavy down and right
    { 0x2510, "downleftlight" },                      // Box drawings light down and left
    { 0x2511, "downlightleftheavy" },                 // Box drawings down light and left heavy
    { 0x2512, "downheavyleftlight" },                 // Box drawings down heavy and left light
    { 0x2513, "downleftheavy" },                      // Box drawings heavy down and left
    { 0x2514, "uprightlight" },                       // Box drawings light up and right
    { 0x2515, "uplightrightheavy" },                  // Box drawings up light and right heavy
    { 0x2516, "upheavyrightlight" },                  // Box drawings up heavy and right light
    { 0x2517, "uprightheavy" },                       // Box drawings heavy up and right
    { 0x2518, "upleftlight" },                        // Box drawings light up and left
    { 0x2519, "uplightleftheavy" },                   // Box drawings up light and left heavy
    { 0x251A, "upheavyleftlight" },                   // Box drawings up heavy and left light
    { 0x251B, "upleftheavy" },                        // Box drawings heavy up and left
    { 0x251C, "verticalrightlight" },                 // Box drawings light vertical and right
    { 0x251D, "verticallightrightheavy" },            // Box drawings vertical light and right heavy
    { 0x251E, "upheavyrightdownlight" },              // Box drawings up heavy and right down light
    { 0x251F, "downheavyrightuplight" },              // Box drawings down heavy and right up light
    { 0x2520, "verticalheavyrightlight" },            // Box drawings vertical heavy and right light
    { 0x2521, "downlightrightupheavy" },              // Box drawings down light and right up heavy
    { 0x2522, "uplightrightdownheavy" },              // Box drawings up light and right down heavy
    { 0x2523, "verticalrightheavy" },                 // Box drawings heavy vertical and right
    { 0x2524, "verticalleftlight" },                  // Box drawings light vertical and left
    { 0x2525, "verticallightleftheavy" },             // Box drawings vertical light and left heavy
    { 0x2526, "upheavyleftdownlight" },               // Box drawings up heavy and left down light
    { 0x2527, "downheavyleftuplight" },               // Box drawings down heavy and left up light
    { 0x2528, "verticalheavyleftlight" },             // Box drawings vertical heavy and left light
    { 0x2529, "downlightleftupheavy" },               // Box drawings down light and left up heavy
    { 0x252A, "uplightleftdownheavy" },               // Box drawings up light and left down heavy
    { 0x252B, "verticalleftheavy" },                  // Box drawings heavy vertical and left
    { 0x252C, "downhorizontallight" },                // Box drawings light down and horizontal
    { 0x252D, "leftheavyrightdownlight" },            // Box drawings left heavy and right down light
    { 0x252E, "rightheavyleftdownlight" },            // Box drawings right heavy and left down light
    { 0x252F, "downlighthorizontalheavy" },           // Box drawings down light and horizontal heavy
    { 0x2530, "downheavyhorizontallight" },           // Box drawings down heavy and horizontal light
    { 0x2531, "rightlightleftdownheavy" },            // Box drawings right light and left down heavy
    { 0x2532, "leftlightrightdownheavy" },            // Box drawings left light and right down heavy
    { 0x2533, "downhorizontalheavy" },                // Box drawings heavy down and horizontal
    { 0x2534, "uphorizontallight" },                  // Box drawings light up and horizontal
    { 0x2535, "leftheavyrightuplight" },              // Box drawings left heavy and right up light
    { 0x2536, "rightheavyleftuplight" },              // Box drawings right heavy and left up light
    { 0x2537, "uplighthorizontalheavy" },             // Box drawings up light and horizontal heavy
    { 0x2538, "upheavyhorizontallight" },             // Box drawings up heavy and horizontal light
    { 0x2539, "rightlightleftupheavy" },              // Box drawings right light and left up heavy
    { 0x253A, "leftlightrightupheavy" },              // Box drawings left light and right up heavy
    { 0x253B, "uphorizontalheavy" },                  // Box drawings heavy up and horizontal
    { 0x253C, "verticalhorizontallight" },            // Box drawings light vertical and horizontal
    { 0x253D, "leftheavyrightverticallight" },        // Box drawings left heavy and right vertical light
    { 0x253E, "rightheavyleftverticallight" },        // Box drawings right heavy and left vertical light
    { 0x253F, "verticallighthorizontalheavy" },       // Box drawings vertical light and horizontal heavy
    { 0x2540, "upheavydownhorizontallight" },         // Box drawings up heavy and down horizontal light
    { 0x2541, "downheavyuphorizontallight" },         // Box drawings down heavy and up horizontal light
    { 0x2542, "verticalheavyhorizontallight" },       // Box drawings vertical heavy and horizontal light
    { 0x2543, "leftupheavyrightdownlight" },          // Box drawings left up heavy and right down light
    { 0x2544, "rightupheavyleftdownlight" },          // Box drawings right up heavy and left down light
    { 0x2545, "leftdownheavyrightuplight" },          // Box drawings left down heavy and right up light
    { 0x2546, "rightdownheavyleftuplight" },          // Box drawings right down heavy and left up light
    { 0x2547, "downlightuphorizontalheavy" },         // Box drawings down light and up horizontal heavy
    { 0x2548, "uplightdownhorizontalheavy" },         // Box drawings up light and down horizontal heavy
    { 0x2549, "rightlightleftverticalheavy" },        // Box drawings right light and left vertical heavy
    { 0x254A, "leftlightrightverticalheavy" },        // Box drawings left light and right vertical heavy
    { 0x254B, "verticalhorizontalheavy" },            // Box drawings heavy vertical and horizontal
    { 0x254C, "horizontaldoubledashlight" },          // Box drawings light double dash horizontal
    { 0x254D, "horizontaldoubledashheavy" },          // Box drawings heavy double dash horizontal
    { 0x254E, "verticaldoubledashlight" },            // Box drawings light double dash vertical
    { 0x254F, "verticaldoubledashheavy" },            // Box drawings heavy double dash vertical
    { 0x2550, "horizontaldouble" },                   // Box drawings double horizontal
    { 0x2551, "verticaldouble" },                     // Box drawings double vertical
    { 0x2552, "downsinglerightdouble" },              // Box drawings down single and right double
    { 0x2553, "downdoublerightsingle" },              // Box drawings down double and right single
    { 0x2554, "downrightdouble" },                    // Box drawings double down and right
    { 0x2555, "downsingleleftdouble" },               // Box drawings down single and left double
    { 0x2556, "downdoubleleftsingle" },               // Box drawings down double and left single
    { 0x2557, "downleftdouble" },                     // Box drawings double down and left
    { 0x2558, "upsinglerightdouble" },                // Box drawings up single and right double
    { 0x2559, "updoublerightsingle" },                // Box drawings up double and right single
    { 0x255A, "uprightdouble" },                      // Box drawings double up and right
    { 0x255B, "upsingleleftdouble" },                 // Box drawings up single and left double
    { 0x255C, "updoubleleftsingle" },                 // Box drawings up double and left single
    { 0x255D, "upleftdouble" },                       // Box drawings double up and left
    { 0x255E, "verticalsinglerightdouble" },          // Box drawings vertical single and right double
    { 0x255F, "verticaldoublerightsingle" },          // Box drawings vertical double and right single
    { 0x2560, "verticalrightdouble" },                // Box drawings double vertical and right
    { 0x2561, "verticalsingleleftdouble" },           // Box drawings vertical single and left double
    { 0x2562, "verticaldoubleleftsingle" },           // Box drawings vertical double and left single
    { 0x2563, "verticalleftdouble" },                 // Box drawings double vertical and left
    { 0x2564, "downsinglehorizontaldouble" },         // Box drawings down single and horizontal double
    { 0x2565, "downdoublehorizontalsingle" },         // Box drawings down double and horizontal single
    { 0x2566, "downhorizontaldouble" },               // Box drawings double down and horizontal
    { 0x2567, "upsinglehorizontaldouble" },           // Box drawings up single and horizontal double
    { 0x2568, "updoublehorizontalsingle" },           // Box drawings up double and horizontal single
    { 0x2569, "uphorizontaldouble" },                 // Box drawings double up and horizontal
    { 0x256A, "verticalsinglehorizontaldouble" },     // Box drawings vertical single and horizontal double
    { 0x256B, "verticaldoublehorizontalsingle" },     // Box drawings vertical double and horizontal single
    { 0x256C, "verticalhorizontaldouble" },           // Box drawings double vertical and horizontal
    { 0x256D, "arcdownrightlight" },                  // Box drawings light arc down and right
    { 0x256E, "arcdownleftlight" },                   // Box drawings light arc down and left
    { 0x256F, "arcupleftlight" },                     // Box drawings light arc up and left
    { 0x2570, "arcuprightlight" },                    // Box drawings light arc up and right
    { 0x2571, "diagonaluprightdownleftlight" },       // Box drawings light diagonal upper right to lower left
    { 0x2572, "diagonalupleftdownrightlight" },       // Box drawings light diagonal upper left to lower right
    { 0x2573, "diagonalcrosslight" },                 // Box drawings light diagonal cross
    { 0x2574, "leftlight" },                          // Box drawings light left
    { 0x2575, "uplight" },                            // Box drawings light up
    { 0x2576, "rightlight" },                         // Box drawings light right
    { 0x2577, "downlight" },                          // Box drawings light down
    { 0x2578, "leftheavy" },                          // Box drawings heavy left
    { 0x2579, "upheavy" },                            // Box drawings heavy up
    { 0x257A, "rightheavy" },                         // Box drawings heavy right
    { 0x257B, "downheavy" },                          // Box drawings heavy down
    { 0x257C, "leftlightrightheavy" },                // Box drawings light left and heavy right
    { 0x257D, "uplightdownheavy" },                   // Box drawings light up and heavy down
    { 0x257E, "leftheavyrightlight" },                // Box drawings heavy left and light right
    { 0x257F, "upheavylightdownlight" },              // Box drawings heavy up and light down
    { 0x2580, "upblock" },                            // Upper half block
    { 0x2581, "dneighthblock" },                      // Lower one eighth block
    { 0x2582, "dnquarterblock" },                     // Lower one quarter block
    { 0x2583, "dnthreeeighthblock" },                 // Lower three eighths block
    { 0x2584, "dnblock" },                            // Lower half block
    { 0x2585, "dnfiveeighthblock" },                  // Lower five eighths block
    { 0x2586, "dnthreequarterblock" },                // Lower three quarters block
    { 0x2587, "dnseveneighthblock" },                 // Lower seven eighths block
    { 0x2588, "block" },                              // Full block
    { 0x2589, "lfseveneighthblock" },                 // Left seven eighths block
    { 0x258A, "lfthreequarterblock" },                // Left three quarters block
    { 0x258B, "lffiveeighthblock" },                  // Left five eighths block
    { 0x258C, "lfblock" },                            // Left half block
    { 0x258D, "lfthreeeighthblock" },                 // Left three eighths block
    { 0x258E, "lfquarterblock" },                     // Left one quarter block
    { 0x258F, "lfeighthblock" },                      // Left one eighth block
    { 0x2590, "rtblock" },                            // Right half block
    { 0x2591, "ltshade" },                            // Light shade
    { 0x2592, "shade" },                              // Medium shade
    { 0x2593, "dkshade" },                            // Dark shade
    { 0x2594, "upeighthblock" },                      // Upper one eighth block
    { 0x2595, "rteighthblock" },                      // Right one eighth block
    { 0x25A0, "blacksquare" },                        // Black square
    { 0x25A1, "box" },                                // White square
    { 0x25A2, "boxrounded" },                         // White square with rounded corners
    { 0x25A3, "boxnested" },                          // White square containing black small square
    { 0x25A4, "boxhorizhatch" },                      // Square with horizontal fill
    { 0x25A5, "boxverthatch" },                       // Square with vertical fill
    { 0x25A6, "boxcrosshatch" },                      // Square with orthogonal crosshatch fill
    { 0x25A7, "boxleftdiaghatch" },                   // Square with upper left to lower right fill
    { 0x25A8, "boxrtdiaghatch" },                     // Square with upper right to lower left fill
    { 0x25A9, "boxcrossdiaghatch" },                  // Square with diagonal crosshatch fill
    { 0x25AA, "U25aa" },                              // Black small square
    { 0x25AB, "U25ab" },                              // White small square
    { 0x25AC, "filledrect" },                         // Black rectangle
    { 0x25AD, "rectangle" },                          // White rectangle
    { 0x25AE, "filledvertrect" },                     // Black vertical rectangle
    { 0x25AF, "vertrectangle" },                      // White vertical rectangle
    { 0x25B0, "filledparallelogram" },                // Black parallelogram
    { 0x25B1, "parallelogram" },                      // White parallelogram
    { 0x25B2, "blackuppointingtriangle" },            // Black up-pointing triangle
    { 0x25B3, "triangle" },                           // White up-pointing triangle
    { 0x25B4, "smalltrianglesld" },                   // Black up-pointing small triangle
    { 0x25B5, "smalltriangle" },                      // White up-pointing small triangle
    { 0x25B6, "trianglerightsld1" },                  // Black right-pointing triangle
    { 0x25B7, "triangleright1" },                     // White right-pointing triangle
    { 0x25B8, "smalltrianglerightsld" },              // Black right-pointing small triangle
    { 0x25B9, "smalltriangleright" },                 // White right-pointing small triangle
    { 0x25BA, "triagrt" },                            // Black right-pointing pointer
    { 0x25BB, "triagrtopen" },                        // White right-pointing pointer
    { 0x25BC, "blackdownpointingtriangle" },          // Black down-pointing triangle
    { 0x25BD, "triangleinv" },                        // White down-pointing triangle
    { 0x25BE, "smalltriangleinvsld" },                // Black down-pointing small triangle
    { 0x25BF, "smalltriangleinv" },                   // White down-pointing small triangle
    { 0x25C0, "triangleleftsld1" },                   // Black left-pointing triangle
    { 0x25C1, "triangleleft1" },                      // White left-pointing triangle
    { 0x25C2, "smalltriangleleftsld" },               // Black left-pointing small triangle
    { 0x25C3, "smalltriangleleft" },                  // White left-pointing small triangle
    { 0x25C4, "triaglf" },                            // Black left-pointing pointer
    { 0x25C5, "triaglfopen" },                        // White left-pointing pointer
    { 0x25C6, "blackdiamond" },                       // Black diamond
    { 0x25C7, "whitediamond" },                       // White diamond
    { 0x25C8, "diamondrhombnested" },                 // White diamond containing black small diamond
    { 0x25C9, "fisheye" },                            // Fisheye
    { 0x25CA, "lozenge" },                            // Lozenge
    { 0x25CB, "circle" },                             // White circle
    { 0x25CC, "circledash" },                         // Dotted circle
    { 0x25CD, "circleverthatch" },                    // Circle with vertical fill
    { 0x25CE, "bullseye1" },                          // Bullseye
    { 0x25CF, "U25cf" },                              // Black circle
    { 0x25D0, "circleleftsld" },                      // Circle with left half black
    { 0x25D1, "circlerightsld" },                     // Circle with right half black
    { 0x25D2, "circlebottomsld" },                    // Circle with lower half black
    { 0x25D3, "circletopsld" },                       // Circle with upper half black
    { 0x25D4, "circlenesld" },                        // Circle with upper right quadrant black
    { 0x25D5, "circlenwopen" },                       // Circle with all but upper left quadrant black
    { 0x25D6, "semicircleleftsld" },                  // Left half black circle
    { 0x25D7, "semicirclelertsld" },                  // Right half black circle
    { 0x25D8, "invbullet" },                          // Inverse bullet
    { 0x25D9, "invcircle" },                          // Inverse white circle
    { 0x25DA, "invsemicircleup" },                    // Upper half inverse white circle
    { 0x25DB, "invsemicircledn" },                    // Lower half inverse white circle
    { 0x25DC, "nwquadarc" },                          // Upper left quadrant circular arc
    { 0x25DD, "nequadarc" },                          // Upper right quadrant circular arc
    { 0x25DE, "sequadarc" },                          // Lower right quadrant circular arc
    { 0x25DF, "swquadarc" },                          // Lower left quadrant circular arc
    { 0x25E0, "toparc" },                             // Upper half circle
    { 0x25E1, "bottomarc" },                          // Lower half circle
    { 0x25E2, "trianglesesld" },                      // Black lower right triangle
    { 0x25E3, "triangleswsld" },                      // Black lower left triangle
    { 0x25E4, "tranglenwsld" },                       // Black upper left triangle
    { 0x25E5, "trianglenesld" },                      // Black upper right triangle
    { 0x25E6, "whitebullet" },                        // White bullet
    { 0x25E7, "squareleftsld" },                      // Square with left half black
    { 0x25E8, "squarerightsld" },                     // Square with right half black
    { 0x25E9, "squarenwsld" },                        // Square with upper left diagonal half black
    { 0x25EA, "squaresesld" },                        // Square with lower right diagonal half black
    { 0x25EB, "squarevertbisect" },                   // White square with vertical bisecting line
    { 0x25EC, "triangledot" },                        // White up-pointing triangle with dot
    { 0x25ED, "triangleleftsld" },                    // Up-pointing triangle with left half black
    { 0x25EE, "trianglerightsld" },                   // Up-pointing triangle with right half black
    { 0x25EF, "largecircle" },                        // Large circle
    { 0x2605, "blackstar" },                          // Black star
    { 0x2606, "whitestar" },                          // White star
    { 0x260E, "telephoneblack" },                     // Black telephone
    { 0x260F, "whitetelephone" },                     // White telephone
    { 0x261B, "a11" },                                // Black right pointing index
    { 0x261C, "pointingindexleftwhite" },             // White left pointing index
    { 0x261D, "pointingindexupwhite" },               // White up pointing index
    { 0x261E, "pointingindexrightwhite" },            // White right pointing index
    { 0x261F, "pointingindexdownwhite" },             // White down pointing index
    { 0x262F, "yinyang" },                            // Yin yang
    { 0x263A, "smileface" },                          // White smiling face
    { 0x263B, "invsmileface" },                       // Black smiling face
    { 0x263C, "sun" },                                // White sun with rays
    { 0x2640, "venus" },                              // Female sign
    { 0x2641, "earth" },                              // Earth
    { 0x2642, "mars" },                               // Male sign
    { 0x2660, "spadesuitblack" },                     // Black spade suit
    { 0x2661, "heartsuit" },                          // White heart suit
    { 0x2662, "diamondsuit" },                        // White diamond suit
    { 0x2663, "clubsuitblack" },                      // Black club suit
    { 0x2664, "spadesuitwhite" },                     // White spade suit
    { 0x2665, "heartsuitblack" },                     // Black heart suit
    { 0x2666, "diamond" },                            // Black diamond suit
    { 0x2667, "clubsuitwhite" },                      // White club suit
    { 0x2668, "hotsprings" },                         // Hot springs
    { 0x2669, "quarternote" },                        // Quarter note
    { 0x266A, "musicalnote" },                        // Eighth note
    { 0x266B, "musicalnotedbl" },                     // Beamed eighth notes
    { 0x266C, "beamedsixteenthnotes" },               // Beamed sixteenth notes
    { 0x266D, "flat" },                               // Music flat sign
    { 0x266E, "natural" },                            // Music natural sign
    { 0x266F, "sharp" },                              // Music sharp sign
    { 0x2701, "a1" },                                 // Upper blade scissors
    { 0x2702, "a2" },                                 // Black scissors
    { 0x2703, "a202" },                               // Lower blade scissors
    { 0x2704, "a3" },                                 // White scissors
    { 0x2706, "a5" },                                 // Telephone location sign
    { 0x2707, "a119" },                               // Tape drive
    { 0x2708, "a118" },                               // Airplane
    { 0x2709, "a117" },                               // Envelope
    { 0x270C, "a13" },                                // Victory hand
    { 0x270D, "a14" },                                // Writing hand
    { 0x270E, "a15" },                                // Lower right pencil
    { 0x270F, "a16" },                                // Pencil
    { 0x2710, "a105" },                               // Upper right pencil
    { 0x2711, "a17" },                                // White nib
    { 0x2712, "a18" },                                // Black nib
    { 0x2713, "checkmark" },                          // Check mark
    { 0x2714, "a20" },                                // Heavy check mark
    { 0x2715, "a21" },                                // Multiplication x
    { 0x2716, "a22" },                                // Heavy multiplication x
    { 0x2717, "a23" },                                // Ballot x
    { 0x2718, "a24" },                                // Heavy ballot x
    { 0x2719, "a25" },                                // Outlined greek cross
    { 0x271A, "a26" },                                // Heavy greek cross
    { 0x271B, "a27" },                                // Open centre cross
    { 0x271C, "a28" },                                // Heavy open centre cross
    { 0x271D, "a6" },                                 // Latin cross
    { 0x271E, "a7" },                                 // Shadowed white latin cross
    { 0x271F, "a8" },                                 // Outlined latin cross
    { 0x2720, "a9" },                                 // Maltese cross
    { 0x2721, "a10" },                                // Star of david
    { 0x2722, "a29" },                                // Four teardrop-spoked asterisk
    { 0x2723, "a30" },                                // Four balloon-spoked asterisk
    { 0x2724, "a31" },                                // Heavy four balloon-spoked asterisk
    { 0x2725, "a32" },                                // Four club-spoked asterisk
    { 0x2726, "a33" },                                // Black four pointed star
    { 0x2727, "a34" },                                // White four pointed star
    { 0x2729, "a36" },                                // Stress outlined white star
    { 0x272A, "a37" },                                // Circled white star
    { 0x272B, "a38" },                                // Open centre black star
    { 0x272C, "a39" },                                // Black centre white star
    { 0x272D, "a40" },                                // Outlined black star
    { 0x272E, "a41" },                                // Heavy outlined black star
    { 0x272F, "a42" },                                // Pinwheel star
    { 0x2730, "a43" },                                // Shadowed white star
    { 0x2731, "a44" },                                // Heavy asterisk
    { 0x2732, "a45" },                                // Open centre asterisk
    { 0x2733, "a46" },                                // Eight spoked asterisk
    { 0x2734, "a47" },                                // Eight pointed black star
    { 0x2735, "a48" },                                // Eight pointed pinwheel star
    { 0x2736, "a49" },                                // Six pointed black star
    { 0x2737, "a50" },                                // Eight pointed rectilinear black star
    { 0x2738, "a51" },                                // Heavy eight pointed rectilinear black star
    { 0x2739, "a52" },                                // Twelve pointed black star
    { 0x273A, "a53" },                                // Sixteen pointed asterisk
    { 0x273B, "a54" },                                // Teardrop-spoked asterisk
    { 0x273C, "a55" },                                // Open centre teardrop-spoked asterisk
    { 0x273D, "a56" },                                // Heavy teardrop-spoked asterisk
    { 0x273E, "a57" },                                // Six petalled black and white florette
    { 0x273F, "a58" },                                // Black florette
    { 0x2740, "a59" },                                // White florette
    { 0x2741, "a60" },                                // Eight petalled outlined black florette
    { 0x2742, "a61" },                                // Circled open centre eight pointed star
    { 0x2743, "a62" },                                // Heavy teardrop-spoked pinwheel asterisk
    { 0x2744, "a63" },                                // Snowflake
    { 0x2745, "a64" },                                // Tight trifoliate snowflake
    { 0x2746, "a65" },                                // Heavy chevron snowflake
    { 0x2747, "a66" },                                // Sparkle
    { 0x2748, "a67" },                                // Heavy sparkle
    { 0x2749, "a68" },                                // Balloon-spoked asterisk
    { 0x274A, "a69" },                                // Eight teardrop-spoked propeller asterisk
    { 0x274B, "a70" },                                // Heavy eight teardrop-spoked propeller asterisk
    { 0x274D, "a72" },                                // Shadowed white circle
    { 0x274F, "a74" },                                // Lower right drop-shadowed white square
    { 0x2750, "a203" },                               // Upper right drop-shadowed white square
    { 0x2751, "a75" },                                // Lower right shadowed white square
    { 0x2752, "a204" },                               // Upper right shadowed white square
    { 0x2756, "a79" },                                // Black diamond minus white x
    { 0x2758, "a82" },                                // Light vertical bar
    { 0x2759, "a83" },                                // Medium vertical bar
    { 0x275A, "a84" },                                // Heavy vertical bar
    { 0x275B, "a97" },                                // Heavy single turned comma quotation mark ornament
    { 0x275C, "a98" },                                // Heavy single comma quotation mark ornament
    { 0x275D, "a99" },                                // Heavy double turned comma quotation mark ornament
    { 0x275E, "a100" },                               // Heavy double comma quotation mark ornament
    { 0x2761, "a101" },                               // Curved stem paragraph sign ornament
    { 0x2762, "a102" },                               // Heavy exclamation mark ornament
    { 0x2763, "a103" },                               // Heavy heart exclamation mark ornament
    { 0x2764, "a104" },                               // Heavy black heart
    { 0x2765, "a106" },                               // Rotated heavy black heart bullet
    { 0x2766, "a107" },                               // Floral heart
    { 0x2767, "a108" },                               // Rotated floral heart bullet
    { 0x2776, "a130" },                               // Dingbat negative circled digit one
    { 0x2777, "a131" },                               // Dingbat negative circled digit two
    { 0x2778, "a132" },                               // Dingbat negative circled digit three
    { 0x2779, "a133" },                               // Dingbat negative circled digit four
    { 0x277A, "a134" },                               // Dingbat negative circled digit five
    { 0x277B, "a135" },                               // Dingbat negative circled digit six
    { 0x277C, "a136" },                               // Dingbat negative circled digit seven
    { 0x277D, "a137" },                               // Dingbat negative circled digit eight
    { 0x277E, "a138" },                               // Dingbat negative circled digit nine
    { 0x277F, "a139" },                               // Dingbat negative circled number ten
    { 0x2780, "a140" },                               // Dingbat circled sans-serif digit one
    { 0x2781, "a141" },                               // Dingbat circled sans-serif digit two
    { 0x2782, "a142" },                               // Dingbat circled sans-serif digit three
    { 0x2783, "a143" },                               // Dingbat circled sans-serif digit four
    { 0x2784, "a144" },                               // Dingbat circled sans-serif digit five
    { 0x2785, "a145" },                               // Dingbat circled sans-serif digit six
    { 0x2786, "a146" },                               // Dingbat circled sans-serif digit seven
    { 0x2787, "a147" },                               // Dingbat circled sans-serif digit eight
    { 0x2788, "a148" },                               // Dingbat circled sans-serif digit nine
    { 0x2789, "a149" },                               // Dingbat circled sans-serif number ten
    { 0x278A, "onecircleinversesansserif" },          // Dingbat negative circled sans-serif digit one
    { 0x278B, "twocircleinversesansserif" },          // Dingbat negative circled sans-serif digit two
    { 0x278C, "threecircleinversesansserif" },        // Dingbat negative circled sans-serif digit three
    { 0x278D, "fourcircleinversesansserif" },         // Dingbat negative circled sans-serif digit four
    { 0x278E, "fivecircleinversesansserif" },         // Dingbat negative circled sans-serif digit five
    { 0x278F, "sixcircleinversesansserif" },          // Dingbat negative circled sans-serif digit six
    { 0x2790, "sevencircleinversesansserif" },        // Dingbat negative circled sans-serif digit seven
    { 0x2791, "eightcircleinversesansserif" },        // Dingbat negative circled sans-serif digit eight
    { 0x2792, "ninecircleinversesansserif" },         // Dingbat negative circled sans-serif digit nine
    { 0x2793, "a159" },                               // Dingbat negative circled sans-serif number ten
    { 0x2794, "a160" },                               // Heavy wide-headed rightwards arrow
    { 0x2798, "a196" },                               // Heavy south east arrow
    { 0x2799, "a165" },                               // Heavy rightwards arrow
    { 0x279A, "a192" },                               // Heavy north east arrow
    { 0x279B, "a166" },                               // Drafting point rightwards arrow
    { 0x279C, "a167" },                               // Heavy round-tipped rightwards arrow
    { 0x279D, "a168" },                               // Triangle-headed rightwards arrow
    { 0x279E, "arrowrightheavy" },                    // Heavy triangle-headed rightwards arrow
    { 0x279F, "a170" },                               // Dashed triangle-headed rightwards arrow
    { 0x27A0, "a171" },                               // Heavy dashed triangle-headed rightwards arrow
    { 0x27A1, "a172" },                               // Black rightwards arrow
    { 0x27A2, "a173" },                               // Three-d top-lighted rightwards arrowhead
    { 0x27A3, "a162" },                               // Three-d bottom-lighted rightwards arrowhead
    { 0x27A4, "a174" },                               // Black rightwards arrowhead
    { 0x27A5, "a175" },                               // Heavy black curved downwards and rightwards arrow
    { 0x27A6, "a176" },                               // Heavy black curved upwards and rightwards arrow
    { 0x27A7, "a177" },                               // Squat black rightwards arrow
    { 0x27A8, "a178" },                               // Heavy concave-pointed black rightwards arrow
    { 0x27A9, "a179" },                               // Right-shaded white rightwards arrow
    { 0x27AA, "a193" },                               // Left-shaded white rightwards arrow
    { 0x27AB, "a180" },                               // Back-tilted shadowed white rightwards arrow
    { 0x27AC, "a199" },                               // Front-tilted shadowed white rightwards arrow
    { 0x27AD, "a181" },                               // Heavy lower right-shadowed white rightwards arrow
    { 0x27AE, "a200" },                               // Heavy upper right-shadowed white rightwards arrow
    { 0x27AF, "a182" },                               // Notched lower right-shadowed white rightwards arrow
    { 0x27B1, "a201" },                               // Notched upper right-shadowed white rightwards arrow
    { 0x27B2, "a183" },                               // Circled heavy white rightwards arrow
    { 0x27B3, "a184" },                               // White-feathered rightwards arrow
    { 0x27B4, "a197" },                               // Black-feathered south east arrow
    { 0x27B5, "a185" },                               // Black-feathered rightwards arrow
    { 0x27B6, "a194" },                               // Black-feathered north east arrow
    { 0x27B7, "a198" },                               // Heavy black-feathered south east arrow
    { 0x27B8, "a186" },                               // Heavy black-feathered rightwards arrow
    { 0x27B9, "a195" },                               // Heavy black-feathered north east arrow
    { 0x27BA, "a187" },                               // Teardrop-barbed rightwards arrow
    { 0x27BB, "a188" },                               // Heavy teardrop-shanked rightwards arrow
    { 0x27BC, "a189" },                               // Wedge-tailed rightwards arrow
    { 0x27BD, "a190" },                               // Heavy wedge-tailed rightwards arrow
    { 0x27BE, "a191" },                               // Open-outlined rightwards arrow
    { 0x27D1, "anddot" },                             // And with dot
    { 0x27D8, "tackuplarge" },                        // Large up tack
    { 0x27D9, "tackdownlarge" },                      // Large down tack
    { 0x27DB, "tackleftright" },                      // Left and right tack
    { 0x27DC, "multimapleft" },                       // Left multimap
    { 0x27DD, "tackrightlong" },                      // Long right tack
    { 0x27DE, "tackleftlong" },                       // Long left tack
    { 0x27DF, "tackupring" },                         // Up tack with circle above
    { 0x27F0, "quadarrowup" },                        // Upwards quadruple arrow
    { 0x27F1, "quadarrowdown" },                      // Downwards quadruple arrow
    { 0x27F2, "ccwlarrowgap" },                       // Anticlockwise gapped circle arrow
    { 0x27F3, "clwarrowgap" },                        // Clockwise gapped circle arrow
    { 0x27F4, "arrowrightcircleplus" },               // Right arrow with circled plus
    { 0x27F5, "longarrowleft" },                      // Long leftwards arrow
    { 0x27F6, "longarrowright" },                     // Long rightwards arrow
    { 0x27F7, "longarrowleftright" },                 // Long left right arrow
    { 0x27F8, "longdblarrowleft" },                   // Long leftwards double arrow
    { 0x27F9, "longdblarrowright" },                  // Long rightwards double arrow
    { 0x27FA, "longdblarrowleftright" },              // Long left right double arrow
    { 0x2A16, "quaternionintegral" },                 // Quaternion integral operator
    { 0x3000, "ideographicspace" },                   // Ideographic space
    { 0x3001, "ideographiccomma" },                   // Ideographic comma
    { 0x3002, "ideographicperiod" },                  // Ideographic full stop
    { 0x3003, "dittomark" },                          // Ditto mark
    { 0x3004, "jis" },                                // Japanese industrial standard symbol
    { 0x3005, "ideographiciterationmark" },           // Ideographic iteration mark
    { 0x3006, "ideographicclose" },                   // Ideographic closing mark
    { 0x3007, "ideographiczero" },                    // Ideographic number zero
    { 0x3008, "anglebracketleft" },                   // Left angle bracket
    { 0x3009, "anglebracketright" },                  // Right angle bracket
    { 0x300A, "dblanglebracketleft" },                // Left double angle bracket
    { 0x300B, "dblanglebracketright" },               // Right double angle bracket
    { 0x300C, "cornerbracketleft" },                  // Left corner bracket
    { 0x300D, "cornerbracketright" },                 // Right corner bracket
    { 0x300E, "whitecornerbracketleft" },             // Left white corner bracket
    { 0x300F, "whitecornerbracketright" },            // Right white corner bracket
    { 0x3010, "blacklenticularbracketleft" },         // Left black lenticular bracket
    { 0x3011, "blacklenticularbracketright" },        // Right black lenticular bracket
    { 0x3012, "postalmark" },                         // Postal mark
    { 0x3013, "getamark" },                           // Geta mark
    { 0x3014, "tortoiseshellbracketleft" },           // Left tortoise shell bracket
    { 0x3015, "tortoiseshellbracketright" },          // Right tortoise shell bracket
    { 0x3016, "whitelenticularbracketleft" },         // Left white lenticular bracket
    { 0x3017, "whitelenticularbracketright" },        // Right white lenticular bracket
    { 0x3018, "whitetortoiseshellbracketleft" },      // Left white tortoise shell bracket
    { 0x3019, "whitetortoiseshellbracketright" },     // Right white tortoise shell bracket
    { 0x301C, "wavedash" },                           // Wave dash
    { 0x301D, "quotedblprimereversed" },              // Reversed double prime quotation mark
    { 0x301E, "quotedblprime" },                      // Double prime quotation mark
    { 0x3020, "postalmarkface" },                     // Postal mark face
    { 0x3021, "onehangzhou" },                        // Hangzhou numeral one
    { 0x3022, "twohangzhou" },                        // Hangzhou numeral two
    { 0x3023, "threehangzhou" },                      // Hangzhou numeral three
    { 0x3024, "fourhangzhou" },                       // Hangzhou numeral four
    { 0x3025, "fivehangzhou" },                       // Hangzhou numeral five
    { 0x3026, "sixhangzhou" },                        // Hangzhou numeral six
    { 0x3027, "sevenhangzhou" },                      // Hangzhou numeral seven
    { 0x3028, "eighthangzhou" },                      // Hangzhou numeral eight
    { 0x3029, "ninehangzhou" },                       // Hangzhou numeral nine
    { 0x3036, "circlepostalmark" },                   // Circled postal mark
    { 0x3041, "asmallhiragana" },                     // Hiragana letter small a
    { 0x3042, "ahiragana" },                          // Hiragana letter a
    { 0x3043, "ismallhiragana" },                     // Hiragana letter small i
    { 0x3044, "ihiragana" },                          // Hiragana letter i
    { 0x3045, "usmallhiragana" },                     // Hiragana letter small u
    { 0x3046, "uhiragana" },                          // Hiragana letter u
    { 0x3047, "esmallhiragana" },                     // Hiragana letter small e
    { 0x3048, "ehiragana" },                          // Hiragana letter e
    { 0x3049, "osmallhiragana" },                     // Hiragana letter small o
    { 0x304A, "ohiragana" },                          // Hiragana letter o
    { 0x304B, "kahiragana" },                         // Hiragana letter ka
    { 0x304C, "gahiragana" },                         // Hiragana letter ga
    { 0x304D, "kihiragana" },                         // Hiragana letter ki
    { 0x304E, "gihiragana" },                         // Hiragana letter gi
    { 0x304F, "kuhiragana" },                         // Hiragana letter ku
    { 0x3050, "guhiragana" },                         // Hiragana letter gu
    { 0x3051, "kehiragana" },                         // Hiragana letter ke
    { 0x3052, "gehiragana" },                         // Hiragana letter ge
    { 0x3053, "kohiragana" },                         // Hiragana letter ko
    { 0x3054, "gohiragana" },                         // Hiragana letter go
    { 0x3055, "sahiragana" },                         // Hiragana letter sa
    { 0x3056, "zahiragana" },                         // Hiragana letter za
    { 0x3057, "sihiragana" },                         // Hiragana letter si
    { 0x3058, "zihiragana" },                         // Hiragana letter zi
    { 0x3059, "suhiragana" },                         // Hiragana letter su
    { 0x305A, "zuhiragana" },                         // Hiragana letter zu
    { 0x305B, "sehiragana" },                         // Hiragana letter se
    { 0x305C, "zehiragana" },                         // Hiragana letter ze
    { 0x305D, "sohiragana" },                         // Hiragana letter so
    { 0x305E, "zohiragana" },                         // Hiragana letter zo
    { 0x305F, "tahiragana" },                         // Hiragana letter ta
    { 0x3060, "dahiragana" },                         // Hiragana letter da
    { 0x3061, "tihiragana" },                         // Hiragana letter ti
    { 0x3062, "dihiragana" },                         // Hiragana letter di
    { 0x3063, "tusmallhiragana" },                    // Hiragana letter small tu
    { 0x3064, "tuhiragana" },                         // Hiragana letter tu
    { 0x3065, "duhiragana" },                         // Hiragana letter du
    { 0x3066, "tehiragana" },                         // Hiragana letter te
    { 0x3067, "dehiragana" },                         // Hiragana letter de
    { 0x3068, "tohiragana" },                         // Hiragana letter to
    { 0x3069, "dohiragana" },                         // Hiragana letter do
    { 0x306A, "nahiragana" },                         // Hiragana letter na
    { 0x306B, "nihiragana" },                         // Hiragana letter ni
    { 0x306C, "nuhiragana" },                         // Hiragana letter nu
    { 0x306D, "nehiragana" },                         // Hiragana letter ne
    { 0x306E, "nohiragana" },                         // Hiragana letter no
    { 0x306F, "hahiragana" },                         // Hiragana letter ha
    { 0x3070, "bahiragana" },                         // Hiragana letter ba
    { 0x3071, "pahiragana" },                         // Hiragana letter pa
    { 0x3072, "hihiragana" },                         // Hiragana letter hi
    { 0x3073, "bihiragana" },                         // Hiragana letter bi
    { 0x3074, "pihiragana" },                         // Hiragana letter pi
    { 0x3075, "huhiragana" },                         // Hiragana letter hu
    { 0x3076, "buhiragana" },                         // Hiragana letter bu
    { 0x3077, "puhiragana" },                         // Hiragana letter pu
    { 0x3078, "hehiragana" },                         // Hiragana letter he
    { 0x3079, "behiragana" },                         // Hiragana letter be
    { 0x307A, "pehiragana" },                         // Hiragana letter pe
    { 0x307B, "hohiragana" },                         // Hiragana letter ho
    { 0x307C, "bohiragana" },                         // Hiragana letter bo
    { 0x307D, "pohiragana" },                         // Hiragana letter po
    { 0x307E, "mahiragana" },                         // Hiragana letter ma
    { 0x307F, "mihiragana" },                         // Hiragana letter mi
    { 0x3080, "muhiragana" },                         // Hiragana letter mu
    { 0x3081, "mehiragana" },                         // Hiragana letter me
    { 0x3082, "mohiragana" },                         // Hiragana letter mo
    { 0x3083, "yasmallhiragana" },                    // Hiragana letter small ya
    { 0x3084, "yahiragana" },                         // Hiragana letter ya
    { 0x3085, "yusmallhiragana" },                    // Hiragana letter small yu
    { 0x3086, "yuhiragana" },                         // Hiragana letter yu
    { 0x3087, "yosmallhiragana" },                    // Hiragana letter small yo
    { 0x3088, "yohiragana" },                         // Hiragana letter yo
    { 0x3089, "rahiragana" },                         // Hiragana letter ra
    { 0x308A, "rihiragana" },                         // Hiragana letter ri
    { 0x308B, "ruhiragana" },                         // Hiragana letter ru
    { 0x308C, "rehiragana" },                         // Hiragana letter re
    { 0x308D, "rohiragana" },                         // Hiragana letter ro
    { 0x308E, "wasmallhiragana" },                    // Hiragana letter small wa
    { 0x308F, "wahiragana" },                         // Hiragana letter wa
    { 0x3090, "wihiragana" },                         // Hiragana letter wi
    { 0x3091, "wehiragana" },                         // Hiragana letter we
    { 0x3092, "wohiragana" },                         // Hiragana letter wo
    { 0x3093, "nhiragana" },                          // Hiragana letter n
    { 0x3094, "vuhiragana" },                         // Hiragana letter vu
    { 0x309B, "voicedmarkkana" },                     // Katakana-hiragana voiced sound mark
    { 0x309C, "semivoicedmarkkana" },                 // Katakana-hiragana semi-voiced sound mark
    { 0x309D, "iterationhiragana" },                  // Hiragana iteration mark
    { 0x309E, "voicediterationhiragana" },            // Hiragana voiced iteration mark
    { 0x30A1, "asmallkatakana" },                     // Katakana letter small a
    { 0x30A2, "akatakana" },                          // Katakana letter a
    { 0x30A3, "ismallkatakana" },                     // Katakana letter small i
    { 0x30A4, "ikatakana" },                          // Katakana letter i
    { 0x30A5, "usmallkatakana" },                     // Katakana letter small u
    { 0x30A6, "ukatakana" },                          // Katakana letter u
    { 0x30A7, "esmallkatakana" },                     // Katakana letter small e
    { 0x30A8, "ekatakana" },                          // Katakana letter e
    { 0x30A9, "osmallkatakana" },                     // Katakana letter small o
    { 0x30AA, "okatakana" },                          // Katakana letter o
    { 0x30AB, "kakatakana" },                         // Katakana letter ka
    { 0x30AC, "gakatakana" },                         // Katakana letter ga
    { 0x30AD, "kikatakana" },                         // Katakana letter ki
    { 0x30AE, "gikatakana" },                         // Katakana letter gi
    { 0x30AF, "kukatakana" },                         // Katakana letter ku
    { 0x30B0, "gukatakana" },                         // Katakana letter gu
    { 0x30B1, "kekatakana" },                         // Katakana letter ke
    { 0x30B2, "gekatakana" },                         // Katakana letter ge
    { 0x30B3, "kokatakana" },                         // Katakana letter ko
    { 0x30B4, "gokatakana" },                         // Katakana letter go
    { 0x30B5, "sakatakana" },                         // Katakana letter sa
    { 0x30B6, "zakatakana" },                         // Katakana letter za
    { 0x30B7, "sikatakana" },                         // Katakana letter si
    { 0x30B8, "zikatakana" },                         // Katakana letter zi
    { 0x30B9, "sukatakana" },                         // Katakana letter su
    { 0x30BA, "zukatakana" },                         // Katakana letter zu
    { 0x30BB, "sekatakana" },                         // Katakana letter se
    { 0x30BC, "zekatakana" },                         // Katakana letter ze
    { 0x30BD, "sokatakana" },                         // Katakana letter so
    { 0x30BE, "zokatakana" },                         // Katakana letter zo
    { 0x30BF, "takatakana" },                         // Katakana letter ta
    { 0x30C0, "dakatakana" },                         // Katakana letter da
    { 0x30C1, "tikatakana" },                         // Katakana letter ti
    { 0x30C2, "dikatakana" },                         // Katakana letter di
    { 0x30C3, "tusmallkatakana" },                    // Katakana letter small tu
    { 0x30C4, "tukatakana" },                         // Katakana letter tu
    { 0x30C5, "dukatakana" },                         // Katakana letter du
    { 0x30C6, "tekatakana" },                         // Katakana letter te
    { 0x30C7, "dekatakana" },                         // Katakana letter de
    { 0x30C8, "tokatakana" },                         // Katakana letter to
    { 0x30C9, "dokatakana" },                         // Katakana letter do
    { 0x30CA, "nakatakana" },                         // Katakana letter na
    { 0x30CB, "nikatakana" },                         // Katakana letter ni
    { 0x30CC, "nukatakana" },                         // Katakana letter nu
    { 0x30CD, "nekatakana" },                         // Katakana letter ne
    { 0x30CE, "nokatakana" },                         // Katakana letter no
    { 0x30CF, "hakatakana" },                         // Katakana letter ha
    { 0x30D0, "bakatakana" },                         // Katakana letter ba
    { 0x30D1, "pakatakana" },                         // Katakana letter pa
    { 0x30D2, "hikatakana" },                         // Katakana letter hi
    { 0x30D3, "bikatakana" },                         // Katakana letter bi
    { 0x30D4, "pikatakana" },                         // Katakana letter pi
    { 0x30D5, "hukatakana" },                         // Katakana letter hu
    { 0x30D6, "bukatakana" },                         // Katakana letter bu
    { 0x30D7, "pukatakana" },                         // Katakana letter pu
    { 0x30D8, "hekatakana" },                         // Katakana letter he
    { 0x30D9, "bekatakana" },                         // Katakana letter be
    { 0x30DA, "pekatakana" },                         // Katakana letter pe
    { 0x30DB, "hokatakana" },                         // Katakana letter ho
    { 0x30DC, "bokatakana" },                         // Katakana letter bo
    { 0x30DD, "pokatakana" },                         // Katakana letter po
    { 0x30DE, "makatakana" },                         // Katakana letter ma
    { 0x30DF, "mikatakana" },                         // Katakana letter mi
    { 0x30E0, "mukatakana" },                         // Katakana letter mu
    { 0x30E1, "mekatakana" },                         // Katakana letter me
    { 0x30E2, "mokatakana" },                         // Katakana letter mo
    { 0x30E3, "yasmallkatakana" },                    // Katakana letter small ya
    { 0x30E4, "yakatakana" },                         // Katakana letter ya
    { 0x30E5, "yusmallkatakana" },                    // Katakana letter small yu
    { 0x30E6, "yukatakana" },                         // Katakana letter yu
    { 0x30E7, "yosmallkatakana" },                    // Katakana letter small yo
    { 0x30E8, "yokatakana" },                         // Katakana letter yo
    { 0x30E9, "rakatakana" },                         // Katakana letter ra
    { 0x30EA, "rikatakana" },                         // Katakana letter ri
    { 0x30EB, "rukatakana" },                         // Katakana letter ru
    { 0x30EC, "rekatakana" },                         // Katakana letter re
    { 0x30ED, "rokatakana" },                         // Katakana letter ro
    { 0x30EE, "wasmallkatakana" },                    // Katakana letter small wa
    { 0x30EF, "wakatakana" },                         // Katakana letter wa
    { 0x30F0, "wikatakana" },                         // Katakana letter wi
    { 0x30F1, "wekatakana" },                         // Katakana letter we
    { 0x30F2, "wokatakana" },                         // Katakana letter wo
    { 0x30F3, "nkatakana" },                          // Katakana letter n
    { 0x30F4, "vukatakana" },                         // Katakana letter vu
    { 0x30F5, "kasmallkatakana" },                    // Katakana letter small ka
    { 0x30F6, "kesmallkatakana" },                    // Katakana letter small ke
    { 0x30F7, "vakatakana" },                         // Katakana letter va
    { 0x30F8, "vikatakana" },                         // Katakana letter vi
    { 0x30F9, "vekatakana" },                         // Katakana letter ve
    { 0x30FA, "vokatakana" },                         // Katakana letter vo
    { 0x30FB, "dotkatakana" },                        // Katakana middle dot
    { 0x30FC, "prolongedkana" },                      // Katakana-hiragana prolonged sound mark
    { 0x30FD, "iterationkatakana" },                  // Katakana iteration mark
    { 0x30FE, "voicediterationkatakana" },            // Katakana voiced iteration mark
    { 0x3105, "bbopomofo" },                          // Bopomofo letter b
    { 0x3106, "pbopomofo" },                          // Bopomofo letter p
    { 0x3107, "mbopomofo" },                          // Bopomofo letter m
    { 0x3108, "fbopomofo" },                          // Bopomofo letter f
    { 0x3109, "dbopomofo" },                          // Bopomofo letter d
    { 0x310A, "tbopomofo" },                          // Bopomofo letter t
    { 0x310B, "nbopomofo" },                          // Bopomofo letter n
    { 0x310C, "lbopomofo" },                          // Bopomofo letter l
    { 0x310D, "gbopomofo" },                          // Bopomofo letter g
    { 0x310E, "kbopomofo" },                          // Bopomofo letter k
    { 0x310F, "hbopomofo" },                          // Bopomofo letter h
    { 0x3110, "jbopomofo" },                          // Bopomofo letter j
    { 0x3111, "qbopomofo" },                          // Bopomofo letter q
    { 0x3112, "xbopomofo" },                          // Bopomofo letter x
    { 0x3113, "zhbopomofo" },                         // Bopomofo letter zh
    { 0x3114, "chbopomofo" },                         // Bopomofo letter ch
    { 0x3115, "shbopomofo" },                         // Bopomofo letter sh
    { 0x3116, "rbopomofo" },                          // Bopomofo letter r
    { 0x3117, "zbopomofo" },                          // Bopomofo letter z
    { 0x3118, "cbopomofo" },                          // Bopomofo letter c
    { 0x3119, "sbopomofo" },                          // Bopomofo letter s
    { 0x311A, "abopomofo" },                          // Bopomofo letter a
    { 0x311B, "obopomofo" },                          // Bopomofo letter o
    { 0x311C, "ebopomofo" },                          // Bopomofo letter e
    { 0x311D, "ehbopomofo" },                         // Bopomofo letter eh
    { 0x311E, "aibopomofo" },                         // Bopomofo letter ai
    { 0x311F, "eibopomofo" },                         // Bopomofo letter ei
    { 0x3120, "aubopomofo" },                         // Bopomofo letter au
    { 0x3121, "oubopomofo" },                         // Bopomofo letter ou
    { 0x3122, "anbopomofo" },                         // Bopomofo letter an
    { 0x3123, "enbopomofo" },                         // Bopomofo letter en
    { 0x3124, "angbopomofo" },                        // Bopomofo letter ang
    { 0x3125, "engbopomofo" },                        // Bopomofo letter eng
    { 0x3126, "erbopomofo" },                         // Bopomofo letter er
    { 0x3127, "ibopomofo" },                          // Bopomofo letter i
    { 0x3128, "ubopomofo" },                          // Bopomofo letter u
    { 0x3129, "iubopomofo" },                         // Bopomofo letter iu
    { 0x3131, "kiyeokkorean" },                       // Hangul letter kiyeok
    { 0x3132, "ssangkiyeokkorean" },                  // Hangul letter ssangkiyeok
    { 0x3133, "kiyeoksioskorean" },                   // Hangul letter kiyeok-sios
    { 0x3134, "nieunkorean" },                        // Hangul letter nieun
    { 0x3135, "nieuncieuckorean" },                   // Hangul letter nieun-cieuc
    { 0x3136, "nieunhieuhkorean" },                   // Hangul letter nieun-hieuh
    { 0x3137, "tikeutkorean" },                       // Hangul letter tikeut
    { 0x3138, "ssangtikeutkorean" },                  // Hangul letter ssangtikeut
    { 0x3139, "rieulkorean" },                        // Hangul letter rieul
    { 0x313A, "rieulkiyeokkorean" },                  // Hangul letter rieul-kiyeok
    { 0x313B, "rieulmieumkorean" },                   // Hangul letter rieul-mieum
    { 0x313C, "rieulpieupkorean" },                   // Hangul letter rieul-pieup
    { 0x313D, "rieulsioskorean" },                    // Hangul letter rieul-sios
    { 0x313E, "rieulthieuthkorean" },                 // Hangul letter rieul-thieuth
    { 0x313F, "rieulphieuphkorean" },                 // Hangul letter rieul-phieuph
    { 0x3140, "rieulhieuhkorean" },                   // Hangul letter rieul-hieuh
    { 0x3141, "mieumkorean" },                        // Hangul letter mieum
    { 0x3142, "pieupkorean" },                        // Hangul letter pieup
    { 0x3143, "ssangpieupkorean" },                   // Hangul letter ssangpieup
    { 0x3144, "pieupsioskorean" },                    // Hangul letter pieup-sios
    { 0x3145, "sioskorean" },                         // Hangul letter sios
    { 0x3146, "ssangsioskorean" },                    // Hangul letter ssangsios
    { 0x3147, "ieungkorean" },                        // Hangul letter ieung
    { 0x3148, "cieuckorean" },                        // Hangul letter cieuc
    { 0x3149, "ssangcieuckorean" },                   // Hangul letter ssangcieuc
    { 0x314A, "chieuchkorean" },                      // Hangul letter chieuch
    { 0x314B, "khieukhkorean" },                      // Hangul letter khieukh
    { 0x314C, "thieuthkorean" },                      // Hangul letter thieuth
    { 0x314D, "phieuphkorean" },                      // Hangul letter phieuph
    { 0x314E, "hieuhkorean" },                        // Hangul letter hieuh
    { 0x314F, "akorean" },                            // Hangul letter a
    { 0x3150, "aekorean" },                           // Hangul letter ae
    { 0x3151, "yakorean" },                           // Hangul letter ya
    { 0x3152, "yaekorean" },                          // Hangul letter yae
    { 0x3153, "eokorean" },                           // Hangul letter eo
    { 0x3154, "ekorean" },                            // Hangul letter e
    { 0x3155, "yeokorean" },                          // Hangul letter yeo
    { 0x3156, "yekorean" },                           // Hangul letter ye
    { 0x3157, "okorean" },                            // Hangul letter o
    { 0x3158, "wakorean" },                           // Hangul letter wa
    { 0x3159, "waekorean" },                          // Hangul letter wae
    { 0x315A, "oekorean" },                           // Hangul letter oe
    { 0x315B, "yokorean" },                           // Hangul letter yo
    { 0x315C, "ukorean" },                            // Hangul letter u
    { 0x315D, "weokorean" },                          // Hangul letter weo
    { 0x315E, "wekorean" },                           // Hangul letter we
    { 0x315F, "wikorean" },                           // Hangul letter wi
    { 0x3160, "yukorean" },                           // Hangul letter yu
    { 0x3161, "eukorean" },                           // Hangul letter eu
    { 0x3162, "yikorean" },                           // Hangul letter yi
    { 0x3163, "ikorean" },                            // Hangul letter i
    { 0x3164, "hangulfiller" },                       // Hangul filler
    { 0x3165, "ssangnieunkorean" },                   // Hangul letter ssangnieun
    { 0x3166, "nieuntikeutkorean" },                  // Hangul letter nieun-tikeut
    { 0x3167, "nieunsioskorean" },                    // Hangul letter nieun-sios
    { 0x3168, "nieunpansioskorean" },                 // Hangul letter nieun-pansios
    { 0x3169, "rieulkiyeoksioskorean" },              // Hangul letter rieul-kiyeok-sios
    { 0x316A, "rieultikeutkorean" },                  // Hangul letter rieul-tikeut
    { 0x316B, "rieulpieupsioskorean" },               // Hangul letter rieul-pieup-sios
    { 0x316C, "rieulpansioskorean" },                 // Hangul letter rieul-pansios
    { 0x316D, "rieulyeorinhieuhkorean" },             // Hangul letter rieul-yeorinhieuh
    { 0x316E, "mieumpieupkorean" },                   // Hangul letter mieum-pieup
    { 0x316F, "mieumsioskorean" },                    // Hangul letter mieum-sios
    { 0x3170, "mieumpansioskorean" },                 // Hangul letter mieum-pansios
    { 0x3171, "kapyeounmieumkorean" },                // Hangul letter kapyeounmieum
    { 0x3172, "pieupkiyeokkorean" },                  // Hangul letter pieup-kiyeok
    { 0x3173, "pieuptikeutkorean" },                  // Hangul letter pieup-tikeut
    { 0x3174, "pieupsioskiyeokkorean" },              // Hangul letter pieup-sios-kiyeok
    { 0x3175, "pieupsiostikeutkorean" },              // Hangul letter pieup-sios-tikeut
    { 0x3176, "pieupcieuckorean" },                   // Hangul letter pieup-cieuc
    { 0x3177, "pieupthieuthkorean" },                 // Hangul letter pieup-thieuth
    { 0x3178, "kapyeounpieupkorean" },                // Hangul letter kapyeounpieup
    { 0x3179, "kapyeounssangpieupkorean" },           // Hangul letter kapyeounssangpieup
    { 0x317A, "sioskiyeokkorean" },                   // Hangul letter sios-kiyeok
    { 0x317B, "siosnieunkorean" },                    // Hangul letter sios-nieun
    { 0x317C, "siostikeutkorean" },                   // Hangul letter sios-tikeut
    { 0x317D, "siospieupkorean" },                    // Hangul letter sios-pieup
    { 0x317E, "sioscieuckorean" },                    // Hangul letter sios-cieuc
    { 0x317F, "pansioskorean" },                      // Hangul letter pansios
    { 0x3180, "ssangieungkorean" },                   // Hangul letter ssangieung
    { 0x3181, "yesieungkorean" },                     // Hangul letter yesieung
    { 0x3182, "yesieungsioskorean" },                 // Hangul letter yesieung-sios
    { 0x3183, "yesieungpansioskorean" },              // Hangul letter yesieung-pansios
    { 0x3184, "kapyeounphieuphkorean" },              // Hangul letter kapyeounphieuph
    { 0x3185, "ssanghieuhkorean" },                   // Hangul letter ssanghieuh
    { 0x3186, "yeorinhieuhkorean" },                  // Hangul letter yeorinhieuh
    { 0x3187, "yoyakorean" },                         // Hangul letter yo-ya
    { 0x3188, "yoyaekorean" },                        // Hangul letter yo-yae
    { 0x3189, "yoikorean" },                          // Hangul letter yo-i
    { 0x318A, "yuyeokorean" },                        // Hangul letter yu-yeo
    { 0x318B, "yuyekorean" },                         // Hangul letter yu-ye
    { 0x318C, "yuikorean" },                          // Hangul letter yu-i
    { 0x318D, "araeakorean" },                        // Hangul letter araea
    { 0x318E, "araeaekorean" },                       // Hangul letter araeae
    { 0x3200, "kiyeokparenkorean" },                  // Parenthesized hangul kiyeok
    { 0x3201, "nieunparenkorean" },                   // Parenthesized hangul nieun
    { 0x3202, "tikeutparenkorean" },                  // Parenthesized hangul tikeut
    { 0x3203, "rieulparenkorean" },                   // Parenthesized hangul rieul
    { 0x3204, "mieumparenkorean" },                   // Parenthesized hangul mieum
    { 0x3205, "pieupparenkorean" },                   // Parenthesized hangul pieup
    { 0x3206, "siosparenkorean" },                    // Parenthesized hangul sios
    { 0x3207, "ieungparenkorean" },                   // Parenthesized hangul ieung
    { 0x3208, "cieucparenkorean" },                   // Parenthesized hangul cieuc
    { 0x3209, "chieuchparenkorean" },                 // Parenthesized hangul chieuch
    { 0x320A, "khieukhparenkorean" },                 // Parenthesized hangul khieukh
    { 0x320B, "thieuthparenkorean" },                 // Parenthesized hangul thieuth
    { 0x320C, "phieuphparenkorean" },                 // Parenthesized hangul phieuph
    { 0x320D, "hieuhparenkorean" },                   // Parenthesized hangul hieuh
    { 0x320E, "kiyeokaparenkorean" },                 // Parenthesized hangul kiyeok a
    { 0x320F, "nieunaparenkorean" },                  // Parenthesized hangul nieun a
    { 0x3210, "tikeutaparenkorean" },                 // Parenthesized hangul tikeut a
    { 0x3211, "rieulaparenkorean" },                  // Parenthesized hangul rieul a
    { 0x3212, "mieumaparenkorean" },                  // Parenthesized hangul mieum a
    { 0x3213, "pieupaparenkorean" },                  // Parenthesized hangul pieup a
    { 0x3214, "siosaparenkorean" },                   // Parenthesized hangul sios a
    { 0x3215, "ieungaparenkorean" },                  // Parenthesized hangul ieung a
    { 0x3216, "cieucaparenkorean" },                  // Parenthesized hangul cieuc a
    { 0x3217, "chieuchaparenkorean" },                // Parenthesized hangul chieuch a
    { 0x3218, "khieukhaparenkorean" },                // Parenthesized hangul khieukh a
    { 0x3219, "thieuthaparenkorean" },                // Parenthesized hangul thieuth a
    { 0x321A, "phieuphaparenkorean" },                // Parenthesized hangul phieuph a
    { 0x321B, "hieuhaparenkorean" },                  // Parenthesized hangul hieuh a
    { 0x321C, "cieucuparenkorean" },                  // Parenthesized hangul cieuc u
    { 0x3220, "oneideographicparen" },                // Parenthesized ideograph one
    { 0x3221, "twoideographicparen" },                // Parenthesized ideograph two
    { 0x3222, "threeideographicparen" },              // Parenthesized ideograph three
    { 0x3223, "fourideographicparen" },               // Parenthesized ideograph four
    { 0x3224, "fiveideographicparen" },               // Parenthesized ideograph five
    { 0x3225, "sixideographicparen" },                // Parenthesized ideograph six
    { 0x3226, "sevenideographicparen" },              // Parenthesized ideograph seven
    { 0x3227, "eightideographicparen" },              // Parenthesized ideograph eight
    { 0x3228, "nineideographicparen" },               // Parenthesized ideograph nine
    { 0x3229, "tenideographicparen" },                // Parenthesized ideograph ten
    { 0x322A, "ideographicmoonparen" },               // Parenthesized ideograph moon
    { 0x322B, "ideographicfireparen" },               // Parenthesized ideograph fire
    { 0x322C, "ideographicwaterparen" },              // Parenthesized ideograph water
    { 0x322D, "ideographicwoodparen" },               // Parenthesized ideograph wood
    { 0x322E, "ideographicmetalparen" },              // Parenthesized ideograph metal
    { 0x322F, "ideographicearthparen" },              // Parenthesized ideograph earth
    { 0x3230, "ideographicsunparen" },                // Parenthesized ideograph sun
    { 0x3231, "ideographicstockparen" },              // Parenthesized ideograph stock
    { 0x3232, "ideographichaveparen" },               // Parenthesized ideograph have
    { 0x3233, "ideographicsocietyparen" },            // Parenthesized ideograph society
    { 0x3234, "ideographicnameparen" },               // Parenthesized ideograph name
    { 0x3235, "ideographicspecialparen" },            // Parenthesized ideograph special
    { 0x3236, "ideographicfinancialparen" },          // Parenthesized ideograph financial
    { 0x3237, "ideographiccongratulationparen" },     // Parenthesized ideograph congratulation
    { 0x3238, "ideographiclaborparen" },              // Parenthesized ideograph labor
    { 0x3239, "ideographicrepresentparen" },          // Parenthesized ideograph represent
    { 0x323A, "ideographiccallparen" },               // Parenthesized ideograph call
    { 0x323B, "ideographicstudyparen" },              // Parenthesized ideograph study
    { 0x323C, "ideographicsuperviseparen" },          // Parenthesized ideograph supervise
    { 0x323D, "ideographicenterpriseparen" },         // Parenthesized ideograph enterprise
    { 0x323E, "ideographicresourceparen" },           // Parenthesized ideograph resource
    { 0x323F, "ideographicallianceparen" },           // Parenthesized ideograph alliance
    { 0x3240, "ideographicfestivalparen" },           // Parenthesized ideograph festival
    { 0x3242, "ideographicselfparen" },               // Parenthesized ideograph self
    { 0x3243, "ideographicreachparen" },              // Parenthesized ideograph reach
    { 0x3260, "kiyeokcirclekorean" },                 // Circled hangul kiyeok
    { 0x3261, "nieuncirclekorean" },                  // Circled hangul nieun
    { 0x3262, "tikeutcirclekorean" },                 // Circled hangul tikeut
    { 0x3263, "rieulcirclekorean" },                  // Circled hangul rieul
    { 0x3264, "mieumcirclekorean" },                  // Circled hangul mieum
    { 0x3265, "pieupcirclekorean" },                  // Circled hangul pieup
    { 0x3266, "sioscirclekorean" },                   // Circled hangul sios
    { 0x3267, "ieungcirclekorean" },                  // Circled hangul ieung
    { 0x3268, "cieuccirclekorean" },                  // Circled hangul cieuc
    { 0x3269, "chieuchcirclekorean" },                // Circled hangul chieuch
    { 0x326A, "khieukhcirclekorean" },                // Circled hangul khieukh
    { 0x326B, "thieuthcirclekorean" },                // Circled hangul thieuth
    { 0x326C, "phieuphcirclekorean" },                // Circled hangul phieuph
    { 0x326D, "hieuhcirclekorean" },                  // Circled hangul hieuh
    { 0x326E, "kiyeokacirclekorean" },                // Circled hangul kiyeok a
    { 0x326F, "nieunacirclekorean" },                 // Circled hangul nieun a
    { 0x3270, "tikeutacirclekorean" },                // Circled hangul tikeut a
    { 0x3271, "rieulacirclekorean" },                 // Circled hangul rieul a
    { 0x3272, "mieumacirclekorean" },                 // Circled hangul mieum a
    { 0x3273, "pieupacirclekorean" },                 // Circled hangul pieup a
    { 0x3274, "siosacirclekorean" },                  // Circled hangul sios a
    { 0x3275, "ieungacirclekorean" },                 // Circled hangul ieung a
    { 0x3276, "cieucacirclekorean" },                 // Circled hangul cieuc a
    { 0x3277, "chieuchacirclekorean" },               // Circled hangul chieuch a
    { 0x3278, "khieukhacirclekorean" },               // Circled hangul khieukh a
    { 0x3279, "thieuthacirclekorean" },               // Circled hangul thieuth a
    { 0x327A, "phieuphacirclekorean" },               // Circled hangul phieuph a
    { 0x327B, "hieuhacirclekorean" },                 // Circled hangul hieuh a
    { 0x327F, "koreanstandardsymbol" },               // Korean standard symbol
    { 0x328A, "ideographmooncircle" },                // Circled ideograph moon
    { 0x328B, "ideographfirecircle" },                // Circled ideograph fire
    { 0x328C, "ideographwatercircle" },               // Circled ideograph water
    { 0x328D, "ideographwoodcircle" },                // Circled ideograph wood
    { 0x328E, "ideographmetalcircle" },               // Circled ideograph metal
    { 0x328F, "ideographearthcircle" },               // Circled ideograph earth
    { 0x3290, "ideographsuncircle" },                 // Circled ideograph sun
    { 0x3294, "ideographnamecircle" },                // Circled ideograph name
    { 0x3296, "ideographicfinancialcircle" },         // Circled ideograph financial
    { 0x3298, "ideographiclaborcircle" },             // Circled ideograph labor
    { 0x3299, "ideographicsecretcircle" },            // Circled ideograph secret
    { 0x329D, "ideographicexcellentcircle" },         // Circled ideograph excellent
    { 0x329E, "ideographicprintcircle" },             // Circled ideograph print
    { 0x32A3, "ideographiccorrectcircle" },           // Circled ideograph correct
    { 0x32A4, "ideographichighcircle" },              // Circled ideograph high
    { 0x32A5, "ideographiccentrecircle" },            // Circled ideograph centre
    { 0x32A6, "ideographiclowcircle" },               // Circled ideograph low
    { 0x32A7, "ideographicleftcircle" },              // Circled ideograph left
    { 0x32A8, "ideographicrightcircle" },             // Circled ideograph right
    { 0x32A9, "ideographicmedicinecircle" },          // Circled ideograph medicine
    { 0x3300, "apaatosquare" },                       // Square apaato
    { 0x3303, "aarusquare" },                         // Square aaru
    { 0x3305, "intisquare" },                         // Square inti
    { 0x330D, "karoriisquare" },                      // Square karorii
    { 0x3314, "kirosquare" },                         // Square kiro
    { 0x3315, "kiroguramusquare" },                   // Square kiroguramu
    { 0x3316, "kiromeetorusquare" },                  // Square kiromeetoru
    { 0x3318, "guramusquare" },                       // Square guramu
    { 0x331E, "kooposquare" },                        // Square koopo
    { 0x3322, "sentisquare" },                        // Square senti
    { 0x3323, "sentosquare" },                        // Square sento
    { 0x3326, "dorusquare" },                         // Square doru
    { 0x3327, "tonsquare" },                          // Square ton
    { 0x332A, "haitusquare" },                        // Square haitu
    { 0x332B, "paasentosquare" },                     // Square paasento
    { 0x3331, "birusquare" },                         // Square biru
    { 0x3333, "huiitosquare" },                       // Square huiito
    { 0x3336, "hekutaarusquare" },                    // Square hekutaaru
    { 0x3339, "herutusquare" },                       // Square herutu
    { 0x333B, "peezisquare" },                        // Square peezi
    { 0x3342, "hoonsquare" },                         // Square hoon
    { 0x3347, "mansyonsquare" },                      // Square mansyon
    { 0x3349, "mirisquare" },                         // Square miri
    { 0x334A, "miribaarusquare" },                    // Square miribaaru
    { 0x334D, "meetorusquare" },                      // Square meetoru
    { 0x334E, "yaadosquare" },                        // Square yaado
    { 0x3351, "rittorusquare" },                      // Square rittoru
    { 0x3357, "wattosquare" },                        // Square watto
    { 0x337B, "heiseierasquare" },                    // Square era name heisei
    { 0x337C, "syouwaerasquare" },                    // Square era name syouwa
    { 0x337D, "taisyouerasquare" },                   // Square era name taisyou
    { 0x337E, "meizierasquare" },                     // Square era name meizi
    { 0x337F, "corporationsquare" },                  // Square corporation
    { 0x3380, "paampssquare" },                       // Square pa amps
    { 0x3381, "nasquare" },                           // Square na
    { 0x3382, "muasquare" },                          // Square mu a
    { 0x3383, "masquare" },                           // Square ma
    { 0x3384, "kasquare" },                           // Square ka
    { 0x3385, "KBsquare" },                           // Square kb
    { 0x3386, "MBsquare" },                           // Square mb
    { 0x3387, "GBsquare" },                           // Square gb
    { 0x3388, "calsquare" },                          // Square cal
    { 0x3389, "kcalsquare" },                         // Square kcal
    { 0x338A, "pfsquare" },                           // Square pf
    { 0x338B, "nfsquare" },                           // Square nf
    { 0x338C, "mufsquare" },                          // Square mu f
    { 0x338D, "mugsquare" },                          // Square mu g
    { 0x338E, "squaremg" },                           // Square mg
    { 0x338F, "squarekg" },                           // Square kg
    { 0x3390, "Hzsquare" },                           // Square hz
    { 0x3391, "khzsquare" },                          // Square khz
    { 0x3392, "mhzsquare" },                          // Square mhz
    { 0x3393, "ghzsquare" },                          // Square ghz
    { 0x3394, "thzsquare" },                          // Square thz
    { 0x3395, "mulsquare" },                          // Square mu l
    { 0x3396, "mlsquare" },                           // Square ml
    { 0x3397, "dlsquare" },                           // Square dl
    { 0x3398, "klsquare" },                           // Square kl
    { 0x3399, "fmsquare" },                           // Square fm
    { 0x339A, "nmsquare" },                           // Square nm
    { 0x339B, "mumsquare" },                          // Square mu m
    { 0x339C, "squaremm" },                           // Square mm
    { 0x339D, "squarecm" },                           // Square cm
    { 0x339E, "squarekm" },                           // Square km
    { 0x339F, "mmsquaredsquare" },                    // Square mm squared
    { 0x33A0, "cmsquaredsquare" },                    // Square cm squared
    { 0x33A1, "squaremsquared" },                     // Square m squared
    { 0x33A2, "kmsquaredsquare" },                    // Square km squared
    { 0x33A3, "mmcubedsquare" },                      // Square mm cubed
    { 0x33A4, "cmcubedsquare" },                      // Square cm cubed
    { 0x33A5, "mcubedsquare" },                       // Square m cubed
    { 0x33A6, "kmcubedsquare" },                      // Square km cubed
    { 0x33A7, "moverssquare" },                       // Square m over s
    { 0x33A8, "moverssquaredsquare" },                // Square m over s squared
    { 0x33A9, "pasquare" },                           // Square pa
    { 0x33AA, "kpasquare" },                          // Square kpa
    { 0x33AB, "mpasquare" },                          // Square mpa
    { 0x33AC, "gpasquare" },                          // Square gpa
    { 0x33AD, "radsquare" },                          // Square rad
    { 0x33AE, "radoverssquare" },                     // Square rad over s
    { 0x33AF, "radoverssquaredsquare" },              // Square rad over s squared
    { 0x33B0, "pssquare" },                           // Square ps
    { 0x33B1, "nssquare" },                           // Square ns
    { 0x33B2, "mussquare" },                          // Square mu s
    { 0x33B3, "mssquare" },                           // Square ms
    { 0x33B4, "pvsquare" },                           // Square pv
    { 0x33B5, "nvsquare" },                           // Square nv
    { 0x33B6, "muvsquare" },                          // Square mu v
    { 0x33B7, "mvsquare" },                           // Square mv
    { 0x33B8, "kvsquare" },                           // Square kv
    { 0x33B9, "mvmegasquare" },                       // Square mv mega
    { 0x33BA, "pwsquare" },                           // Square pw
    { 0x33BB, "nwsquare" },                           // Square nw
    { 0x33BC, "muwsquare" },                          // Square mu w
    { 0x33BD, "mwsquare" },                           // Square mw
    { 0x33BE, "kwsquare" },                           // Square kw
    { 0x33BF, "mwmegasquare" },                       // Square mw mega
    { 0x33C0, "kohmsquare" },                         // Square k ohm
    { 0x33C1, "mohmsquare" },                         // Square m ohm
    { 0x33C2, "amsquare" },                           // Square am
    { 0x33C3, "bqsquare" },                           // Square bq
    { 0x33C4, "squarecc" },                           // Square cc
    { 0x33C5, "cdsquare" },                           // Square cd
    { 0x33C6, "coverkgsquare" },                      // Square c over kg
    { 0x33C7, "cosquare" },                           // Square co
    { 0x33C8, "dbsquare" },                           // Square db
    { 0x33C9, "gysquare" },                           // Square gy
    { 0x33CA, "hasquare" },                           // Square ha
    { 0x33CB, "HPsquare" },                           // Square hp
    { 0x33CD, "KKsquare" },                           // Square kk
    { 0x33CE, "squarekmcapital" },                    // Square km Capital
    { 0x33CF, "ktsquare" },                           // Square kt
    { 0x33D0, "lmsquare" },                           // Square lm
    { 0x33D1, "squareln" },                           // Square ln
    { 0x33D2, "squarelog" },                          // Square log
    { 0x33D3, "lxsquare" },                           // Square lx
    { 0x33D4, "mbsquare" },                           // Square mb small
    { 0x33D5, "squaremil" },                          // Square mil
    { 0x33D6, "molsquare" },                          // Square mol
    { 0x33D8, "pmsquare" },                           // Square pm
    { 0x33DB, "srsquare" },                           // Square sr
    { 0x33DC, "svsquare" },                           // Square sv
    { 0x33DD, "wbsquare" },                           // Square wb
    { 0xFB00, "ff" },                                 // Latin small ligature ff
    { 0xFB01, "fi" },                                 // Latin small ligature fi
    { 0xFB02, "fl" },                                 // Latin small ligature fl
    { 0xFB03, "ffi" },                                // Latin small ligature ffi
    { 0xFB04, "ffl" },                                // Latin small ligature ffl
    { 0xFB05, "longst" },                             // Latin small ligature long s t
    { 0xFB06, "st" },                                 // Latin small ligature st
    { 0xFB1E, "varika" },                             // Hebrew point judeo-spanish varika
    { 0xFB1F, "yodyodpatahhebrew" },                  // Hebrew ligature yiddish yod yod patah
    { 0xFB20, "ayinaltonehebrew" },                   // Hebrew letter alternative ayin
    { 0xFB2A, "shinshindothebrew" },                  // Hebrew letter shin with shin dot
    { 0xFB2B, "shinsindothebrew" },                   // Hebrew letter shin with sin dot
    { 0xFB2C, "shindageshshindothebrew" },            // Hebrew letter shin with dagesh and shin dot
    { 0xFB2D, "shindageshsindothebrew" },             // Hebrew letter shin with dagesh and sin dot
    { 0xFB2E, "alefpatahhebrew" },                    // Hebrew letter alef with patah
    { 0xFB2F, "alefqamatshebrew" },                   // Hebrew letter alef with qamats
    { 0xFB30, "alefdageshhebrew" },                   // Hebrew letter alef with mapiq
    { 0xFB31, "betdageshhebrew" },                    // Hebrew letter bet with dagesh
    { 0xFB32, "gimeldageshhebrew" },                  // Hebrew letter gimel with dagesh
    { 0xFB33, "daletdageshhebrew" },                  // Hebrew letter dalet with dagesh
    { 0xFB34, "hedageshhebrew" },                     // Hebrew letter he with mapiq
    { 0xFB35, "vavdageshhebrew" },                    // Hebrew letter vav with dagesh
    { 0xFB36, "zayindageshhebrew" },                  // Hebrew letter zayin with dagesh
    { 0xFB38, "tetdageshhebrew" },                    // Hebrew letter tet with dagesh
    { 0xFB39, "yoddageshhebrew" },                    // Hebrew letter yod with dagesh
    { 0xFB3A, "finalkafdageshhebrew" },               // Hebrew letter final kaf with dagesh
    { 0xFB3B, "kafdageshhebrew" },                    // Hebrew letter kaf with dagesh
    { 0xFB3C, "lameddageshhebrew" },                  // Hebrew letter lamed with dagesh
    { 0xFB3E, "memdageshhebrew" },                    // Hebrew letter mem with dagesh
    { 0xFB40, "nundageshhebrew" },                    // Hebrew letter nun with dagesh
    { 0xFB41, "samekhdageshhebrew" },                 // Hebrew letter samekh with dagesh
    { 0xFB43, "pefinaldageshhebrew" },                // Hebrew letter final pe with dagesh
    { 0xFB44, "pedageshhebrew" },                     // Hebrew letter pe with dagesh
    { 0xFB46, "tsadidageshhebrew" },                  // Hebrew letter tsadi with dagesh
    { 0xFB47, "qofdageshhebrew" },                    // Hebrew letter qof with dagesh
    { 0xFB48, "reshdageshhebrew" },                   // Hebrew letter resh with dagesh
    { 0xFB49, "shindageshhebrew" },                   // Hebrew letter shin with dagesh
    { 0xFB4A, "tavdageshhebrew" },                    // Hebrew letter tav with dagesh
    { 0xFB4B, "vavholamhebrew" },                     // Hebrew letter vav with holam
    { 0xFB4C, "betrafehebrew" },                      // Hebrew letter bet with rafe
    { 0xFB4D, "kafrafehebrew" },                      // Hebrew letter kaf with rafe
    { 0xFB4E, "perafehebrew" },                       // Hebrew letter pe with rafe
    { 0xFB4F, "aleflamedhebrew" },                    // Hebrew ligature alef lamed
    { 0xFB57, "pehfinalarabic" },                     // Arabic letter peh final form
    { 0xFB58, "pehinitialarabic" },                   // Arabic letter peh initial form
    { 0xFB59, "pehmedialarabic" },                    // Arabic letter peh medial form
    { 0xFB67, "ttehfinalarabic" },                    // Arabic letter tteh final form
    { 0xFB68, "ttehinitialarabic" },                  // Arabic letter tteh initial form
    { 0xFB69, "ttehmedialarabic" },                   // Arabic letter tteh medial form
    { 0xFB6B, "vehfinalarabic" },                     // Arabic letter veh final form
    { 0xFB6C, "vehinitialarabic" },                   // Arabic letter veh initial form
    { 0xFB6D, "vehmedialarabic" },                    // Arabic letter veh medial form
    { 0xFB7B, "tchehfinalarabic" },                   // Arabic letter tcheh final form
    { 0xFB7C, "tchehinitialarabic" },                 // Arabic letter tcheh initial form
    { 0xFB7D, "tchehmedialarabic" },                  // Arabic letter tcheh medial form
    { 0xFB89, "ddalfinalarabic" },                    // Arabic letter ddal final form
    { 0xFB8B, "jehfinalarabic" },                     // Arabic letter jeh final form
    { 0xFB8D, "rrehfinalarabic" },                    // Arabic letter rreh final form
    { 0xFB90, "c19424" },                             // Arabic letter keheh initial form
    { 0xFB93, "gaffinalarabic" },                     // Arabic letter gaf final form
    { 0xFB94, "gafinitialarabic" },                   // Arabic letter gaf initial form
    { 0xFB95, "gafmedialarabic" },                    // Arabic letter gaf medial form
    { 0xFB9F, "noonghunnafinalarabic" },              // Arabic letter noon ghunna final form
    { 0xFBA4, "hehhamzaaboveisolatedarabic" },        // Arabic letter heh with yeh above isolated form
    { 0xFBA5, "hehhamzaabovefinalarabic" },           // Arabic letter heh with yeh above final form
    { 0xFBA7, "hehfinalaltonearabic" },               // Arabic letter heh goal final form
    { 0xFBA8, "hehinitialaltonearabic" },             // Arabic letter heh goal initial form
    { 0xFBA9, "hehmedialaltonearabic" },              // Arabic letter heh goal medial form
    { 0xFBAF, "yehbarreefinalarabic" },               // Arabic letter yeh barree final form
    { 0xFBFD, "c19440" },                             // Arabic letter farsi yeh final form
    { 0xFC08, "behmeemisolatedarabic" },              // Arabic ligature beh with meem isolated form
    { 0xFC0B, "tehjeemisolatedarabic" },              // Arabic ligature teh with jeem isolated form
    { 0xFC0C, "tehhahisolatedarabic" },               // Arabic ligature teh with hah isolated form
    { 0xFC0E, "tehmeemisolatedarabic" },              // Arabic ligature teh with meem isolated form
    { 0xFC32, "c20220" },                             // Arabic ligature feh with yeh isolated form
    { 0xFC3F, "c24069" },                             // Arabic ligature lam with jeem isolated form
    { 0xFC40, "c24070" },                             // Arabic ligature lam with hah isolated form
    { 0xFC41, "c24071" },                             // Arabic ligature lam with khah isolated form
    { 0xFC42, "c20631" },                             // Arabic ligature lam with meem isolated form
    { 0xFC43, "c20224" },                             // Arabic ligature lam with alef maksura isolated form
    { 0xFC44, "c20222" },                             // Arabic ligature lam with yeh isolated form
    { 0xFC48, "meemmeemisolatedarabic" },             // Arabic ligature meem with meem isolated form
    { 0xFC4B, "noonjeemisolatedarabic" },             // Arabic ligature noon with jeem isolated form
    { 0xFC4E, "noonmeemisolatedarabic" },             // Arabic ligature noon with meem isolated form
    { 0xFC58, "yehmeemisolatedarabic" },              // Arabic ligature yeh with meem isolated form
    { 0xFC5E, "shaddadammatanarabic" },               // Arabic ligature shadda with dammatan isolated form
    { 0xFC5F, "shaddakasratanarabic" },               // Arabic ligature shadda with kasratan isolated form
    { 0xFC60, "shaddafathaarabic" },                  // Arabic ligature shadda with fatha isolated form
    { 0xFC61, "shaddadammaarabic" },                  // Arabic ligature shadda with damma isolated form
    { 0xFC62, "shaddakasraarabic" },                  // Arabic ligature shadda with kasra isolated form
    { 0xFC6A, "c20622" },                             // Arabic ligature beh with reh final form
    { 0xFC6D, "behnoonfinalarabic" },                 // Arabic ligature beh with noon final form
    { 0xFC6F, "c24074" },                             // Arabic ligature beh with yeh final form
    { 0xFC70, "c20650" },                             // Arabic ligature teh with reh final form
    { 0xFC73, "tehnoonfinalarabic" },                 // Arabic ligature teh with noon final form
    { 0xFC75, "c20652" },                             // Arabic ligature teh with yeh final form
    { 0xFC8D, "noonnoonfinalarabic" },                // Arabic ligature noon with noon final form
    { 0xFC8F, "c20641" },                             // Arabic ligature noon with yeh final form
    { 0xFC91, "c20254" },                             // Arabic ligature yeh with reh final form
    { 0xFC94, "yehnoonfinalarabic" },                 // Arabic ligature yeh with noon final form
    { 0xFC9C, "c24075" },                             // Arabic ligature beh with jeem initial form
    { 0xFC9D, "c24076" },                             // Arabic ligature beh with hah initial form
    { 0xFC9E, "c24077" },                             // Arabic ligature beh with khah initial form
    { 0xFC9F, "behmeeminitialarabic" },               // Arabic ligature beh with meem initial form
    { 0xFCA1, "tehjeeminitialarabic" },               // Arabic ligature teh with jeem initial form
    { 0xFCA2, "tehhahinitialarabic" },                // Arabic ligature teh with hah initial form
    { 0xFCA3, "c20647" },                             // Arabic ligature teh with khah initial form
    { 0xFCA4, "tehmeeminitialarabic" },               // Arabic ligature teh with meem initial form
    { 0xFCA6, "c24067" },                             // Arabic ligature theh with meem initial form
    { 0xFCA8, "c20626" },                             // Arabic ligature jeem with meem initial form
    { 0xFCAA, "c20624" },                             // Arabic ligature hah with meem initial form
    { 0xFCAC, "c20627" },                             // Arabic ligature khah with meem initial form
    { 0xFCB0, "c24072" },                             // Arabic ligature seen with meem initial form
    { 0xFCC9, "lamjeeminitialarabic" },               // Arabic ligature lam with jeem initial form
    { 0xFCCA, "lamhahinitialarabic" },                // Arabic ligature lam with hah initial form
    { 0xFCCB, "lamkhahinitialarabic" },               // Arabic ligature lam with khah initial form
    { 0xFCCC, "lammeeminitialarabic" },               // Arabic ligature lam with meem initial form
    { 0xFCCD, "c20630" },                             // Arabic ligature lam with heh initial form
    { 0xFCCE, "c20633" },                             // Arabic ligature meem with jeem initial form
    { 0xFCCF, "c20632" },                             // Arabic ligature meem with hah initial form
    { 0xFCD0, "c20634" },                             // Arabic ligature meem with khah initial form
    { 0xFCD1, "meemmeeminitialarabic" },              // Arabic ligature meem with meem initial form
    { 0xFCD2, "noonjeeminitialarabic" },              // Arabic ligature noon with jeem initial form
    { 0xFCD3, "c24079" },                             // Arabic ligature noon with hah initial form
    { 0xFCD4, "c24080" },                             // Arabic ligature noon with khah initial form
    { 0xFCD5, "noonmeeminitialarabic" },              // Arabic ligature noon with meem initial form
    { 0xFCDA, "c20656" },                             // Arabic ligature yeh with jeem initial form
    { 0xFCDB, "c20654" },                             // Arabic ligature yeh with hah initial form
    { 0xFCDC, "c20657" },                             // Arabic ligature yeh with khah initial form
    { 0xFCDD, "yehmeeminitialarabic" },               // Arabic ligature yeh with meem initial form
    { 0xFD30, "c24073" },                             // Arabic ligature sheen with meem initial form
    { 0xFD3E, "parenleftaltonearabic" },              // Ornate left parenthesis
    { 0xFD3F, "parenrightaltonearabic" },             // Ornate right parenthesis
    { 0xFD88, "lammeemhahinitialarabic" },            // Arabic ligature lam with meem with hah initial form
    { 0xFDF2, "lamlamhehisolatedarabic" },            // Arabic ligature allah isolated form
    { 0xFDFA, "sallallahoualayhewasallamarabic" },    // Arabic ligature sallallahou alayhe wasallam
    { 0xFE30, "twodotleadervertical" },               // Presentation form for vertical two dot leader
    { 0xFE31, "emdashvertical" },                     // Presentation form for vertical em dash
    { 0xFE32, "endashvertical" },                     // Presentation form for vertical en dash
    { 0xFE33, "underscorevertical" },                 // Presentation form for vertical low line
    { 0xFE34, "wavyunderscorevertical" },             // Presentation form for vertical wavy low line
    { 0xFE35, "parenleftvertical" },                  // Presentation form for vertical left parenthesis
    { 0xFE36, "parenrightvertical" },                 // Presentation form for vertical right parenthesis
    { 0xFE37, "braceleftvertical" },                  // Presentation form for vertical left curly bracket
    { 0xFE38, "bracerightvertical" },                 // Presentation form for vertical right curly bracket
    { 0xFE39, "tortoiseshellbracketleftvertical" },   // Presentation form for vertical left tortoise shell bracket
    { 0xFE3A, "tortoiseshellbracketrightvertical" },  // Presentation form for vertical right tortoise shell bracket
    { 0xFE3B, "blacklenticularbracketleftvertical" }, // Presentation form for vertical left black lenticular bracket
    { 0xFE3C, "blacklenticularbracketrightvertical" },// Presentation form for vertical right black lenticular bracket
    { 0xFE3D, "dblanglebracketleftvertical" },        // Presentation form for vertical left double angle bracket
    { 0xFE3E, "dblanglebracketrightvertical" },       // Presentation form for vertical right double angle bracket
    { 0xFE3F, "anglebracketleftvertical" },           // Presentation form for vertical left angle bracket
    { 0xFE40, "anglebracketrightvertical" },          // Presentation form for vertical right angle bracket
    { 0xFE41, "cornerbracketleftvertical" },          // Presentation form for vertical left corner bracket
    { 0xFE42, "cornerbracketrightvertical" },         // Presentation form for vertical right corner bracket
    { 0xFE43, "whitecornerbracketleftvertical" },     // Presentation form for vertical left white corner bracket
    { 0xFE44, "whitecornerbracketrightvertical" },    // Presentation form for vertical right white corner bracket
    { 0xFE49, "overlinedashed" },                     // Dashed overline
    { 0xFE4A, "overlinecenterline" },                 // Centreline overline
    { 0xFE4B, "overlinewavy" },                       // Wavy overline
    { 0xFE4C, "overlinedblwavy" },                    // Double wavy overline
    { 0xFE4D, "lowlinedashed" },                      // Dashed low line
    { 0xFE4E, "lowlinecenterline" },                  // Centreline low line
    { 0xFE4F, "underscorewavy" },                     // Wavy low line
    { 0xFE50, "commasmall" },                         // Small comma
    { 0xFE52, "periodsmall" },                        // Small full stop
    { 0xFE54, "semicolonsmall" },                     // Small semicolon
    { 0xFE55, "colonsmall" },                         // Small colon
    { 0xFE59, "parenleftsmall" },                     // Small left parenthesis
    { 0xFE5A, "parenrightsmall" },                    // Small right parenthesis
    { 0xFE5B, "braceleftsmall" },                     // Small left curly bracket
    { 0xFE5C, "bracerightsmall" },                    // Small right curly bracket
    { 0xFE5D, "tortoiseshellbracketleftsmall" },      // Small left tortoise shell bracket
    { 0xFE5E, "tortoiseshellbracketrightsmall" },     // Small right tortoise shell bracket
    { 0xFE5F, "numbersignsmall" },                    // Small number sign
    { 0xFE61, "asterisksmall" },                      // Small asterisk
    { 0xFE62, "plussmall" },                          // Small plus sign
    { 0xFE63, "hyphensmall" },                        // Small hyphen-minus
    { 0xFE64, "lesssmall" },                          // Small less-than sign
    { 0xFE65, "greatersmall" },                       // Small greater-than sign
    { 0xFE66, "equalsmall" },                         // Small equals sign
    { 0xFE69, "dollarsmall" },                        // Small dollar sign
    { 0xFE6A, "percentsmall" },                       // Small percent sign
    { 0xFE6B, "atsmall" },                            // Small commercial at
    { 0xFE70, "c19482" },                             // Arabic fathatan isolated form
    { 0xFE72, "c19484" },                             // Arabic dammatan isolated form
    { 0xFE74, "c24054" },                             // Arabic kasratan isolated form
    { 0xFE76, "c19479" },                             // Arabic fatha isolated form
    { 0xFE78, "c19481" },                             // Arabic damma isolated form
    { 0xFE7A, "c19999" },                             // Arabic kasra isolated form
    { 0xFE7C, "c19492" },                             // Arabic shadda isolated form
    { 0xFE7E, "c19491" },                             // Arabic sukun isolated form
    { 0xFE80, "c24189" },                             // Arabic letter hamza isolated form
    { 0xFE81, "c20174" },                             // Arabic letter alef with madda above isolated form
    { 0xFE82, "alefmaddaabovefinalarabic" },          // Arabic letter alef with madda above final form
    { 0xFE83, "c24198" },                             // Arabic letter alef with hamza above isolated form
    { 0xFE84, "alefhamzaabovefinalarabic" },          // Arabic letter alef with hamza above final form
    { 0xFE85, "c24194" },                             // Arabic letter waw with hamza above isolated form
    { 0xFE86, "wawhamzaabovefinalarabic" },           // Arabic letter waw with hamza above final form
    { 0xFE87, "c24200" },                             // Arabic letter alef with hamza below isolated form
    { 0xFE88, "alefhamzabelowfinalarabic" },          // Arabic letter alef with hamza below final form
    { 0xFE89, "c24190" },                             // Arabic letter yeh with hamza above isolated form
    { 0xFE8A, "yehhamzaabovefinalarabic" },           // Arabic letter yeh with hamza above final form
    { 0xFE8B, "yehhamzaaboveinitialarabic" },         // Arabic letter yeh with hamza above initial form
    { 0xFE8C, "yehhamzaabovemedialarabic" },          // Arabic letter yeh with hamza above medial form
    { 0xFE8D, "c20051" },                             // Arabic letter alef isolated form
    { 0xFE8E, "aleffinalarabic" },                    // Arabic letter alef final form
    { 0xFE8F, "c20053" },                             // Arabic letter beh isolated form
    { 0xFE90, "behfinalarabic" },                     // Arabic letter beh final form
    { 0xFE91, "behinitialarabic" },                   // Arabic letter beh initial form
    { 0xFE92, "behmedialarabic" },                    // Arabic letter beh medial form
    { 0xFE93, "c20153" },                             // Arabic letter teh marbuta isolated form
    { 0xFE94, "tehmarbutafinalarabic" },              // Arabic letter teh marbuta final form
    { 0xFE95, "c20057" },                             // Arabic letter teh isolated form
    { 0xFE96, "tehfinalarabic" },                     // Arabic letter teh final form
    { 0xFE97, "tehinitialarabic" },                   // Arabic letter teh initial form
    { 0xFE98, "tehmedialarabic" },                    // Arabic letter teh medial form
    { 0xFE99, "c20061" },                             // Arabic letter theh isolated form
    { 0xFE9A, "thehfinalarabic" },                    // Arabic letter theh final form
    { 0xFE9B, "thehinitialarabic" },                  // Arabic letter theh initial form
    { 0xFE9C, "thehmedialarabic" },                   // Arabic letter theh medial form
    { 0xFE9D, "c20065" },                             // Arabic letter jeem isolated form
    { 0xFE9E, "jeemfinalarabic" },                    // Arabic letter jeem final form
    { 0xFE9F, "jeeminitialarabic" },                  // Arabic letter jeem initial form
    { 0xFEA0, "jeemmedialarabic" },                   // Arabic letter jeem medial form
    { 0xFEA1, "c20069" },                             // Arabic letter hah isolated form
    { 0xFEA2, "hahfinalarabic" },                     // Arabic letter hah final form
    { 0xFEA3, "hahinitialarabic" },                   // Arabic letter hah initial form
    { 0xFEA4, "hahmedialarabic" },                    // Arabic letter hah medial form
    { 0xFEA5, "c20073" },                             // Arabic letter khah isolated form
    { 0xFEA6, "khahfinalarabic" },                    // Arabic letter khah final form
    { 0xFEA7, "khahinitialarabic" },                  // Arabic letter khah initial form
    { 0xFEA8, "khahmedialarabic" },                   // Arabic letter khah medial form
    { 0xFEA9, "c20077" },                             // Arabic letter dal isolated form
    { 0xFEAA, "dalfinalarabic" },                     // Arabic letter dal final form
    { 0xFEAB, "c20079" },                             // Arabic letter thal isolated form
    { 0xFEAC, "thalfinalarabic" },                    // Arabic letter thal final form
    { 0xFEAD, "c20081" },                             // Arabic letter reh isolated form
    { 0xFEAE, "rehfinalarabic" },                     // Arabic letter reh final form
    { 0xFEAF, "c20083" },                             // Arabic letter zain isolated form
    { 0xFEB0, "zainfinalarabic" },                    // Arabic letter zain final form
    { 0xFEB1, "c20085" },                             // Arabic letter seen isolated form
    { 0xFEB2, "seenfinalarabic" },                    // Arabic letter seen final form
    { 0xFEB3, "seeninitialarabic" },                  // Arabic letter seen initial form
    { 0xFEB4, "seenmedialarabic" },                   // Arabic letter seen medial form
    { 0xFEB5, "c20089" },                             // Arabic letter sheen isolated form
    { 0xFEB6, "sheenfinalarabic" },                   // Arabic letter sheen final form
    { 0xFEB7, "sheeninitialarabic" },                 // Arabic letter sheen initial form
    { 0xFEB8, "sheenmedialarabic" },                  // Arabic letter sheen medial form
    { 0xFEB9, "c20093" },                             // Arabic letter sad isolated form
    { 0xFEBA, "sadfinalarabic" },                     // Arabic letter sad final form
    { 0xFEBB, "sadinitialarabic" },                   // Arabic letter sad initial form
    { 0xFEBC, "sadmedialarabic" },                    // Arabic letter sad medial form
    { 0xFEBD, "c20097" },                             // Arabic letter dad isolated form
    { 0xFEBE, "dadfinalarabic" },                     // Arabic letter dad final form
    { 0xFEBF, "dadinitialarabic" },                   // Arabic letter dad initial form
    { 0xFEC0, "dadmedialarabic" },                    // Arabic letter dad medial form
    { 0xFEC1, "c20101" },                             // Arabic letter tah isolated form
    { 0xFEC2, "tahfinalarabic" },                     // Arabic letter tah final form
    { 0xFEC3, "tahinitialarabic" },                   // Arabic letter tah initial form
    { 0xFEC4, "tahmedialarabic" },                    // Arabic letter tah medial form
    { 0xFEC5, "c20105" },                             // Arabic letter zah isolated form
    { 0xFEC6, "zahfinalarabic" },                     // Arabic letter zah final form
    { 0xFEC7, "zahinitialarabic" },                   // Arabic letter zah initial form
    { 0xFEC8, "zahmedialarabic" },                    // Arabic letter zah medial form
    { 0xFEC9, "c20109" },                             // Arabic letter ain isolated form
    { 0xFECA, "ainfinalarabic" },                     // Arabic letter ain final form
    { 0xFECB, "aininitialarabic" },                   // Arabic letter ain initial form
    { 0xFECC, "ainmedialarabic" },                    // Arabic letter ain medial form
    { 0xFECD, "c20113" },                             // Arabic letter ghain isolated form
    { 0xFECE, "ghainfinalarabic" },                   // Arabic letter ghain final form
    { 0xFECF, "ghaininitialarabic" },                 // Arabic letter ghain initial form
    { 0xFED0, "ghainmedialarabic" },                  // Arabic letter ghain medial form
    { 0xFED1, "c20117" },                             // Arabic letter feh isolated form
    { 0xFED2, "fehfinalarabic" },                     // Arabic letter feh final form
    { 0xFED3, "fehinitialarabic" },                   // Arabic letter feh initial form
    { 0xFED4, "fehmedialarabic" },                    // Arabic letter feh medial form
    { 0xFED5, "c20121" },                             // Arabic letter qaf isolated form
    { 0xFED6, "qaffinalarabic" },                     // Arabic letter qaf final form
    { 0xFED7, "qafinitialarabic" },                   // Arabic letter qaf initial form
    { 0xFED8, "qafmedialarabic" },                    // Arabic letter qaf medial form
    { 0xFED9, "c20125" },                             // Arabic letter kaf isolated form
    { 0xFEDA, "kaffinalarabic" },                     // Arabic letter kaf final form
    { 0xFEDB, "kafinitialarabic" },                   // Arabic letter kaf initial form
    { 0xFEDC, "kafmedialarabic" },                    // Arabic letter kaf medial form
    { 0xFEDD, "c20129" },                             // Arabic letter lam isolated form
    { 0xFEDE, "lamfinalarabic" },                     // Arabic letter lam final form
    { 0xFEDF, "laminitialarabic" },                   // Arabic letter lam initial form
    { 0xFEE0, "lammedialarabic" },                    // Arabic letter lam medial form
    { 0xFEE1, "c20133" },                             // Arabic letter meem isolated form
    { 0xFEE2, "meemfinalarabic" },                    // Arabic letter meem final form
    { 0xFEE3, "meeminitialarabic" },                  // Arabic letter meem initial form
    { 0xFEE4, "meemmedialarabic" },                   // Arabic letter meem medial form
    { 0xFEE5, "c20137" },                             // Arabic letter noon isolated form
    { 0xFEE6, "noonfinalarabic" },                    // Arabic letter noon final form
    { 0xFEE7, "nooninitialarabic" },                  // Arabic letter noon initial form
    { 0xFEE8, "noonmedialarabic" },                   // Arabic letter noon medial form
    { 0xFEE9, "c20141" },                             // Arabic letter heh isolated form
    { 0xFEEA, "hehfinalarabic" },                     // Arabic letter heh final form
    { 0xFEEB, "hehinitialarabic" },                   // Arabic letter heh initial form
    { 0xFEEC, "hehmedialarabic" },                    // Arabic letter heh medial form
    { 0xFEED, "c20145" },                             // Arabic letter waw isolated form
    { 0xFEEE, "wawfinalarabic" },                     // Arabic letter waw final form
    { 0xFEEF, "c20256" },                             // Arabic letter alef maksura isolated form
    { 0xFEF0, "alefmaksurafinalarabic" },             // Arabic letter alef maksura final form
    { 0xFEF1, "c20148" },                             // Arabic letter yeh isolated form
    { 0xFEF2, "yehfinalarabic" },                     // Arabic letter yeh final form
    { 0xFEF3, "alefmaksurainitialarabic" },           // Arabic letter yeh initial form
    { 0xFEF4, "c20150" },                             // Arabic letter yeh medial form
    { 0xFEF5, "lamalefmaddaaboveisolatedarabic" },    // Arabic ligature lam with alef with madda above isolated form
    { 0xFEF6, "lamalefmaddaabovefinalarabic" },       // Arabic ligature lam with alef with madda above final form
    { 0xFEF7, "lamalefhamzaaboveisolatedarabic" },    // Arabic ligature lam with alef with hamza above isolated form
    { 0xFEF8, "lamalefhamzaabovefinalarabic" },       // Arabic ligature lam with alef with hamza above final form
    { 0xFEF9, "lamalefhamzabelowisolatedarabic" },    // Arabic ligature lam with alef with hamza below isolated form
    { 0xFEFA, "lamalefhamzabelowfinalarabic" },       // Arabic ligature lam with alef with hamza below final form
    { 0xFEFB, "lamalefisolatedarabic" },              // Arabic ligature lam with alef isolated form
    { 0xFEFC, "lamaleffinalarabic" },                 // Arabic ligature lam with alef final form
    { 0xFEFF, "bom" },                                // Zero width no-break space
    { 0xFF01, "exclammonospace" },                    // Fullwidth exclamation mark
    { 0xFF02, "quotedblmonospace" },                  // Fullwidth quotation mark
    { 0xFF03, "numbersignmonospace" },                // Fullwidth number sign
    { 0xFF04, "dollarmonospace" },                    // Fullwidth dollar sign
    { 0xFF05, "percentmonospace" },                   // Fullwidth percent sign
    { 0xFF06, "ampersandmonospace" },                 // Fullwidth ampersand
    { 0xFF07, "quotesinglemonospace" },               // Fullwidth apostrophe
    { 0xFF08, "parenleftmonospace" },                 // Fullwidth left parenthesis
    { 0xFF09, "parenrightmonospace" },                // Fullwidth right parenthesis
    { 0xFF0A, "asteriskmonospace" },                  // Fullwidth asterisk
    { 0xFF0B, "plusmonospace" },                      // Fullwidth plus sign
    { 0xFF0C, "commamonospace" },                     // Fullwidth comma
    { 0xFF0D, "hyphenmonospace" },                    // Fullwidth hyphen-minus
    { 0xFF0E, "periodmonospace" },                    // Fullwidth full stop
    { 0xFF0F, "slashmonospace" },                     // Fullwidth solidus
    { 0xFF10, "zeromonospace" },                      // Fullwidth digit zero
    { 0xFF11, "onemonospace" },                       // Fullwidth digit one
    { 0xFF12, "twomonospace" },                       // Fullwidth digit two
    { 0xFF13, "threemonospace" },                     // Fullwidth digit three
    { 0xFF14, "fourmonospace" },                      // Fullwidth digit four
    { 0xFF15, "fivemonospace" },                      // Fullwidth digit five
    { 0xFF16, "sixmonospace" },                       // Fullwidth digit six
    { 0xFF17, "sevenmonospace" },                     // Fullwidth digit seven
    { 0xFF18, "eightmonospace" },                     // Fullwidth digit eight
    { 0xFF19, "ninemonospace" },                      // Fullwidth digit nine
    { 0xFF1A, "colonmonospace" },                     // Fullwidth colon
    { 0xFF1B, "semicolonmonospace" },                 // Fullwidth semicolon
    { 0xFF1C, "lessmonospace" },                      // Fullwidth less-than sign
    { 0xFF1D, "equalmonospace" },                     // Fullwidth equals sign
    { 0xFF1E, "greatermonospace" },                   // Fullwidth greater-than sign
    { 0xFF1F, "questionmonospace" },                  // Fullwidth question mark
    { 0xFF20, "atmonospace" },                        // Fullwidth commercial at
    { 0xFF21, "Amonospace" },                         // Fullwidth latin Capital Letter A
    { 0xFF22, "Bmonospace" },                         // Fullwidth latin Capital Letter B
    { 0xFF23, "Cmonospace" },                         // Fullwidth latin Capital Letter C
    { 0xFF24, "Dmonospace" },                         // Fullwidth latin Capital Letter D
    { 0xFF25, "Emonospace" },                         // Fullwidth latin Capital Letter E
    { 0xFF26, "Fmonospace" },                         // Fullwidth latin Capital Letter F
    { 0xFF27, "Gmonospace" },                         // Fullwidth latin Capital Letter G
    { 0xFF28, "Hmonospace" },                         // Fullwidth latin Capital Letter H
    { 0xFF29, "Imonospace" },                         // Fullwidth latin Capital Letter I
    { 0xFF2A, "Jmonospace" },                         // Fullwidth latin Capital Letter J
    { 0xFF2B, "Kmonospace" },                         // Fullwidth latin Capital Letter K
    { 0xFF2C, "Lmonospace" },                         // Fullwidth latin Capital Letter L
    { 0xFF2D, "Mmonospace" },                         // Fullwidth latin Capital Letter M
    { 0xFF2E, "Nmonospace" },                         // Fullwidth latin Capital Letter N
    { 0xFF2F, "Omonospace" },                         // Fullwidth latin Capital Letter O
    { 0xFF30, "Pmonospace" },                         // Fullwidth latin Capital Letter P
    { 0xFF31, "Qmonospace" },                         // Fullwidth latin Capital Letter Q
    { 0xFF32, "Rmonospace" },                         // Fullwidth latin Capital Letter R
    { 0xFF33, "Smonospace" },                         // Fullwidth latin Capital Letter S
    { 0xFF34, "Tmonospace" },                         // Fullwidth latin Capital Letter T
    { 0xFF35, "Umonospace" },                         // Fullwidth latin Capital Letter U
    { 0xFF36, "Vmonospace" },                         // Fullwidth latin Capital Letter V
    { 0xFF37, "Wmonospace" },                         // Fullwidth latin Capital Letter W
    { 0xFF38, "Xmonospace" },                         // Fullwidth latin Capital Letter X
    { 0xFF39, "Ymonospace" },                         // Fullwidth latin Capital Letter Y
    { 0xFF3A, "Zmonospace" },                         // Fullwidth latin Capital Letter Z
    { 0xFF3B, "bracketleftmonospace" },               // Fullwidth left square bracket
    { 0xFF3C, "backslashmonospace" },                 // Fullwidth reverse solidus
    { 0xFF3D, "bracketrightmonospace" },              // Fullwidth right square bracket
    { 0xFF3E, "asciicircummonospace" },               // Fullwidth circumflex accent
    { 0xFF3F, "underscoremonospace" },                // Fullwidth low line
    { 0xFF40, "gravemonospace" },                     // Fullwidth grave accent
    { 0xFF41, "amonospace" },                         // Fullwidth latin small letter a
    { 0xFF42, "bmonospace" },                         // Fullwidth latin small letter b
    { 0xFF43, "cmonospace" },                         // Fullwidth latin small letter c
    { 0xFF44, "dmonospace" },                         // Fullwidth latin small letter d
    { 0xFF45, "emonospace" },                         // Fullwidth latin small letter e
    { 0xFF46, "fmonospace" },                         // Fullwidth latin small letter f
    { 0xFF47, "gmonospace" },                         // Fullwidth latin small letter g
    { 0xFF48, "hmonospace" },                         // Fullwidth latin small letter h
    { 0xFF49, "imonospace" },                         // Fullwidth latin small letter i
    { 0xFF4A, "jmonospace" },                         // Fullwidth latin small letter j
    { 0xFF4B, "kmonospace" },                         // Fullwidth latin small letter k
    { 0xFF4C, "lmonospace" },                         // Fullwidth latin small letter l
    { 0xFF4D, "mmonospace" },                         // Fullwidth latin small letter m
    { 0xFF4E, "nmonospace" },                         // Fullwidth latin small letter n
    { 0xFF4F, "omonospace" },                         // Fullwidth latin small letter o
    { 0xFF50, "pmonospace" },                         // Fullwidth latin small letter p
    { 0xFF51, "qmonospace" },                         // Fullwidth latin small letter q
    { 0xFF52, "rmonospace" },                         // Fullwidth latin small letter r
    { 0xFF53, "smonospace" },                         // Fullwidth latin small letter s
    { 0xFF54, "tmonospace" },                         // Fullwidth latin small letter t
    { 0xFF55, "umonospace" },                         // Fullwidth latin small letter u
    { 0xFF56, "vmonospace" },                         // Fullwidth latin small letter v
    { 0xFF57, "wmonospace" },                         // Fullwidth latin small letter w
    { 0xFF58, "xmonospace" },                         // Fullwidth latin small letter x
    { 0xFF59, "ymonospace" },                         // Fullwidth latin small letter y
    { 0xFF5A, "zmonospace" },                         // Fullwidth latin small letter z
    { 0xFF5B, "braceleftmonospace" },                 // Fullwidth left curly bracket
    { 0xFF5C, "barmonospace" },                       // Fullwidth vertical line
    { 0xFF5D, "bracerightmonospace" },                // Fullwidth right curly bracket
    { 0xFF5E, "asciitildemonospace" },                // Fullwidth tilde
    { 0xFF61, "periodhalfwidth" },                    // Halfwidth ideographic full stop
    { 0xFF62, "cornerbracketlefthalfwidth" },         // Halfwidth left corner bracket
    { 0xFF63, "cornerbracketrighthalfwidth" },        // Halfwidth right corner bracket
    { 0xFF64, "ideographiccommaleft" },               // Halfwidth ideographic comma
    { 0xFF65, "middledotkatakanahalfwidth" },         // Halfwidth katakana middle dot
    { 0xFF66, "wokatakanahalfwidth" },                // Halfwidth katakana letter wo
    { 0xFF67, "asmallkatakanahalfwidth" },            // Halfwidth katakana letter small a
    { 0xFF68, "ismallkatakanahalfwidth" },            // Halfwidth katakana letter small i
    { 0xFF69, "usmallkatakanahalfwidth" },            // Halfwidth katakana letter small u
    { 0xFF6A, "esmallkatakanahalfwidth" },            // Halfwidth katakana letter small e
    { 0xFF6B, "osmallkatakanahalfwidth" },            // Halfwidth katakana letter small o
    { 0xFF6C, "yasmallkatakanahalfwidth" },           // Halfwidth katakana letter small ya
    { 0xFF6D, "yusmallkatakanahalfwidth" },           // Halfwidth katakana letter small yu
    { 0xFF6E, "yosmallkatakanahalfwidth" },           // Halfwidth katakana letter small yo
    { 0xFF6F, "tusmallkatakanahalfwidth" },           // Halfwidth katakana letter small tu
    { 0xFF70, "katahiraprolongmarkhalfwidth" },       // Halfwidth katakana-hiragana prolonged sound mark
    { 0xFF72, "ikatakanahalfwidth" },                 // Halfwidth katakana letter i
    { 0xFF73, "ukatakanahalfwidth" },                 // Halfwidth katakana letter u
    { 0xFF74, "ekatakanahalfwidth" },                 // Halfwidth katakana letter e
    { 0xFF75, "okatakanahalfwidth" },                 // Halfwidth katakana letter o
    { 0xFF76, "kakatakanahalfwidth" },                // Halfwidth katakana letter ka
    { 0xFF77, "kikatakanahalfwidth" },                // Halfwidth katakana letter ki
    { 0xFF78, "kukatakanahalfwidth" },                // Halfwidth katakana letter ku
    { 0xFF79, "kekatakanahalfwidth" },                // Halfwidth katakana letter ke
    { 0xFF7A, "kokatakanahalfwidth" },                // Halfwidth katakana letter ko
    { 0xFF7B, "sakatakanahalfwidth" },                // Halfwidth katakana letter sa
    { 0xFF7C, "sikatakanahalfwidth" },                // Halfwidth katakana letter si
    { 0xFF7D, "sukatakanahalfwidth" },                // Halfwidth katakana letter su
    { 0xFF7E, "sekatakanahalfwidth" },                // Halfwidth katakana letter se
    { 0xFF7F, "sokatakanahalfwidth" },                // Halfwidth katakana letter so
    { 0xFF80, "takatakanahalfwidth" },                // Halfwidth katakana letter ta
    { 0xFF81, "tikatakanahalfwidth" },                // Halfwidth katakana letter ti
    { 0xFF82, "tukatakanahalfwidth" },                // Halfwidth katakana letter tu
    { 0xFF83, "tekatakanahalfwidth" },                // Halfwidth katakana letter te
    { 0xFF84, "tokatakanahalfwidth" },                // Halfwidth katakana letter to
    { 0xFF85, "nakatakanahalfwidth" },                // Halfwidth katakana letter na
    { 0xFF86, "nikatakanahalfwidth" },                // Halfwidth katakana letter ni
    { 0xFF87, "nukatakanahalfwidth" },                // Halfwidth katakana letter nu
    { 0xFF88, "nekatakanahalfwidth" },                // Halfwidth katakana letter ne
    { 0xFF89, "nokatakanahalfwidth" },                // Halfwidth katakana letter no
    { 0xFF8A, "hakatakanahalfwidth" },                // Halfwidth katakana letter ha
    { 0xFF8B, "hikatakanahalfwidth" },                // Halfwidth katakana letter hi
    { 0xFF8C, "hukatakanahalfwidth" },                // Halfwidth katakana letter hu
    { 0xFF8D, "hekatakanahalfwidth" },                // Halfwidth katakana letter he
    { 0xFF8E, "hokatakanahalfwidth" },                // Halfwidth katakana letter ho
    { 0xFF8F, "makatakanahalfwidth" },                // Halfwidth katakana letter ma
    { 0xFF90, "mikatakanahalfwidth" },                // Halfwidth katakana letter mi
    { 0xFF91, "mukatakanahalfwidth" },                // Halfwidth katakana letter mu
    { 0xFF92, "mekatakanahalfwidth" },                // Halfwidth katakana letter me
    { 0xFF93, "mokatakanahalfwidth" },                // Halfwidth katakana letter mo
    { 0xFF94, "yakatakanahalfwidth" },                // Halfwidth katakana letter ya
    { 0xFF95, "yukatakanahalfwidth" },                // Halfwidth katakana letter yu
    { 0xFF96, "yokatakanahalfwidth" },                // Halfwidth katakana letter yo
    { 0xFF97, "rakatakanahalfwidth" },                // Halfwidth katakana letter ra
    { 0xFF98, "rikatakanahalfwidth" },                // Halfwidth katakana letter ri
    { 0xFF99, "rukatakanahalfwidth" },                // Halfwidth katakana letter ru
    { 0xFF9A, "rekatakanahalfwidth" },                // Halfwidth katakana letter re
    { 0xFF9B, "rokatakanahalfwidth" },                // Halfwidth katakana letter ro
    { 0xFF9C, "wakatakanahalfwidth" },                // Halfwidth katakana letter wa
    { 0xFF9D, "nkatakanahalfwidth" },                 // Halfwidth katakana letter n
    { 0xFF9E, "voicedmarkkanahalfwidth" },            // Halfwidth katakana voiced sound mark
    { 0xFF9F, "semivoicedmarkkanahalfwidth" },        // Halfwidth katakana semi-voiced sound mark
    { 0xFFE0, "centmonospace" },                      // Fullwidth cent sign
    { 0xFFE1, "sterlingmonospace" },                  // Fullwidth pound sign
    { 0xFFE3, "macronmonospace" },                    // Fullwidth macron
    { 0xFFE5, "yenmonospace" },                       // Fullwidth yen sign
    { 0xFFE6, "wonmonospace" },                       // Fullwidth won sign
    { 0x1D49C, "Ascript" },                           // Mathematical script Capital A
    { 0x1D49E, "Cscript" },                           // Mathematical script Capital C
    { 0x1D49F, "Dscript" },                           // Mathematical script Capital D
    { 0x1D4A2, "Gscript" },                           // Mathematical script Capital G
    { 0x1D4A5, "Jscript" },                           // Mathematical script Capital J
    { 0x1D4A6, "Kscript" },                           // Mathematical script Capital K
    { 0x1D4A9, "Nscript" },                           // Mathematical script Capital N
    { 0x1D4AA, "Oscript" },                           // Mathematical script Capital O
    { 0x1D4AB, "Pscript" },                           // Mathematical script Capital P
    { 0x1D4AC, "Qscript" },                           // Mathematical script Capital Q
    { 0x1D4AE, "Sscript" },                           // Mathematical script Capital S
    { 0x1D4AF, "Tscript" },                           // Mathematical script Capital T
    { 0x1D4B0, "UScript" },                           // Mathematical script Capital U
    { 0x1D4B1, "VScript" },                           // Mathematical script Capital V
    { 0x1D4B2, "Wscript" },                           // Mathematical script Capital W
    { 0x1D4B3, "Xscript" },                           // Mathematical script Capital X
    { 0x1D4B4, "Yscript" },                           // Mathematical script Capital Y
    { 0x1D4B5, "Zscript" },                           // Mathematical script Capital Z
    { 0x1D4D0, "Aboldscript" },                       // Mathematical bold script Capital A
    { 0x1D4D1, "Bboldscript" },                       // Mathematical bold script Capital B
    { 0x1D4D2, "Cboldscript" },                       // Mathematical bold script Capital C
    { 0x1D4D3, "Dboldscript" },                       // Mathematical bold script Capital D
    { 0x1D4D4, "Eboldscript" },                       // Mathematical bold script Capital E
    { 0x1D4D5, "Fboldscript" },                       // Mathematical bold script Capital F
    { 0x1D4D6, "Gboldscript" },                       // Mathematical bold script Capital G
    { 0x1D4D7, "Hboldscript" },                       // Mathematical bold script Capital H
    { 0x1D4D8, "Iboldscript" },                       // Mathematical bold script Capital I
    { 0x1D4D9, "Jboldscript" },                       // Mathematical bold script Capital J
    { 0x1D4DA, "Kboldscript" },                       // Mathematical bold script Capital K
    { 0x1D4DB, "Lboldscript" },                       // Mathematical bold script Capital L
    { 0x1D4DC, "Mboldscript" },                       // Mathematical bold script Capital M
    { 0x1D4DD, "Nboldscript" },                       // Mathematical bold script Capital N
    { 0x1D4DE, "Oboldscript" },                       // Mathematical bold script Capital O
    { 0x1D4DF, "Pboldscript" },                       // Mathematical bold script Capital P
    { 0x1D4E0, "Qboldscript" },                       // Mathematical bold script Capital Q
    { 0x1D4E1, "Rboldscript" },                       // Mathematical bold script Capital R
    { 0x1D4E2, "Sboldscript" },                       // Mathematical bold script Capital S
    { 0x1D4E3, "Tboldscript" },                       // Mathematical bold script Capital T
    { 0x1D4E4, "Uboldscript" },                       // Mathematical bold script Capital U
    { 0x1D4E5, "Vboldscript" },                       // Mathematical bold script Capital V
    { 0x1D4E6, "Wboldscript" },                       // Mathematical bold script Capital W
    { 0x1D4E7, "Xboldscript" },                       // Mathematical bold script Capital X
    { 0x1D4E8, "Yboldscript" },                       // Mathematical bold script Capital Y
    { 0x1D4E9, "Zboldscript" },                       // Mathematical bold script Capital Z
    { 0x1D504, "Afraktur" },                          // Mathematical fraktur Capital A
    { 0x1D505, "Bfraktur" },                          // Mathematical fraktur Capital B
    { 0x1D507, "Dfraktur" },                          // Mathematical fraktur Capital D
    { 0x1D508, "Efraktur" },                          // Mathematical fraktur Capital E
    { 0x1D509, "Ffraktur" },                          // Mathematical fraktur Capital F
    { 0x1D50A, "Gfraktur" },                          // Mathematical fraktur Capital G
    { 0x1D50D, "Jfraktur" },                          // Mathematical fraktur Capital J
    { 0x1D50E, "Kfraktur" },                          // Mathematical fraktur Capital K
    { 0x1D50F, "Lfraktur" },                          // Mathematical fraktur Capital L
    { 0x1D510, "Mfraktur" },                          // Mathematical fraktur Capital M
    { 0x1D511, "Nfraktur" },                          // Mathematical fraktur Capital N
    { 0x1D512, "Ofraktur" },                          // Mathematical fraktur Capital O
    { 0x1D513, "Pfraktur" },                          // Mathematical fraktur Capital P
    { 0x1D514, "Qfraktur" },                          // Mathematical fraktur Capital Q
    { 0x1D516, "Sfraktur" },                          // Mathematical fraktur Capital S
    { 0x1D517, "Tfraktur" },                          // Mathematical fraktur Capital T
    { 0x1D518, "Ufraktur" },                          // Mathematical fraktur Capital U
    { 0x1D519, "Vfraktur" },                          // Mathematical fraktur Capital V
    { 0x1D51A, "Wfraktur" },                          // Mathematical fraktur Capital W
    { 0x1D51B, "Xfraktur" },                          // Mathematical fraktur Capital X
    { 0x1D51C, "Yfraktur" },                          // Mathematical fraktur Capital Y
    { 0x1D51E, "afraktur" },                          // Mathematical fraktur small a
    { 0x1D51F, "bfraktur" },                          // Mathematical fraktur small b
    { 0x1D520, "cfraktur" },                          // Mathematical fraktur small c
    { 0x1D521, "dfraktur" },                          // Mathematical fraktur small d
    { 0x1D522, "efraktur" },                          // Mathematical fraktur small e
    { 0x1D523, "ffraktur" },                          // Mathematical fraktur small f
    { 0x1D524, "gfraktur" },                          // Mathematical fraktur small g
    { 0x1D525, "hfraktur" },                          // Mathematical fraktur small h
    { 0x1D526, "ifraktur" },                          // Mathematical fraktur small i
    { 0x1D527, "jfraktur" },                          // Mathematical fraktur small j
    { 0x1D528, "kfraktur" },                          // Mathematical fraktur small k
    { 0x1D529, "lfraktur" },                          // Mathematical fraktur small l
    { 0x1D52A, "mfraktur" },                          // Mathematical fraktur small m
    { 0x1D52B, "nfraktur" },                          // Mathematical fraktur small n
    { 0x1D52C, "ofraktur" },                          // Mathematical fraktur small o
    { 0x1D52D, "pfraktur" },                          // Mathematical fraktur small p
    { 0x1D52E, "qfraktur" },                          // Mathematical fraktur small q
    { 0x1D52F, "rfraktur" },                          // Mathematical fraktur small r
    { 0x1D530, "sfraktur" },                          // Mathematical fraktur small s
    { 0x1D531, "tfraktur" },                          // Mathematical fraktur small t
    { 0x1D532, "ufraktur" },                          // Mathematical fraktur small u
    { 0x1D533, "vfraktur" },                          // Mathematical fraktur small v
    { 0x1D534, "wfraktur" },                          // Mathematical fraktur small w
    { 0x1D535, "xfraktur" },                          // Mathematical fraktur small x
    { 0x1D536, "yfraktur" },                          // Mathematical fraktur small y
    { 0x1D537, "zfraktur" },                          // Mathematical fraktur small z
  };
}
