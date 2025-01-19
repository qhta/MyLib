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
  /// Try to get the name for a character
  /// </summary>
  /// <param name="ch"></param>
  /// <param name="name"></param>
  /// <returns></returns>
  public static bool TryGetValue(char ch, out string name)
  {
    return CharMapping.TryGetValue(ch, out name);
  }

  /// <summary>
  /// Try to get the name for a character
  /// </summary>
  /// <param name="ch"></param>
  /// <param name="name"></param>
  /// <returns></returns>
  public static bool TryGetValue(string name, out char ch)
  {
    if (name.Length == 1)
    {
      return EscapeMapping.TryGetValue1(name, out ch);
    }
    return CharMapping.TryGetValue1(name, out ch);
  }

  /// <summary>
  /// Registered escape sequences for characters
  /// </summary>
  public static readonly BiDiDictionary<char, string> EscapeMapping = new BiDiDictionary<char, string>()
  {
    { '\u0009', @"t" },
    { '\u000A', @"n" },
    { '\u000B', @"v" },
    { '\u000C', @"f" },
    { '\u000D', @"r" },
    { '\u0022', @"""" },
    { '\u0027', @"'" },
    { '\u005C', @"\" },
    { '\u2028', @"l" },
    { '\u2029', @"p" },
  };

  /// <summary>
  /// Registered character names
  /// </summary>
  public static readonly BiDiDictionary<char, string> CharMapping = new BiDiDictionary<char, string>()
  {
    { '\u0000', "NUL" },
    { '\u0001', "STX" },
    { '\u0002', "SOT" },
    { '\u0003', "ETX" },
    { '\u0004', "EOT" },
    { '\u0005', "ENQ" },
    { '\u0006', "ACK" },
    { '\u0007', "BEL" },
    { '\u0008', "BS" },
    { '\u0009', "HT" },                           // Horizontal tab
    { '\u000A', "LF" },                           // Line feed
    { '\u000B', "VT" },                           // Vertical tab
    { '\u000C', "FF" },                           // Form feed
    { '\u000D', "CR" },                           // Carriage return
    { '\u000E', "SO" },
    { '\u000F', "SI" },
    { '\u0010', "DLE" },
    { '\u0011', "DC1" },
    { '\u0012', "DC2" },
    { '\u0013', "DC3" },
    { '\u0014', "DC4" },
    { '\u0015', "NAK" },
    { '\u0016', "SYN" },
    { '\u0017', "ETB" },
    { '\u0018', "CAN" },
    { '\u0019', "EM" },
    { '\u001A', "SUB" },
    { '\u001B', "ESC" },
    { '\u001C', "FS" },
    { '\u001D', "GS" },
    { '\u001E', "RS" },
    { '\u001F', "US" },
    { '\u007F', "DEL" },
    { '\u0080', "PAD" },
    { '\u0081', "HOP" },
    { '\u0082', "BPH" },
    { '\u0083', "NBH" },
    { '\u0084', "IND" },
    { '\u0085', "NEL" },
    { '\u0086', "SSA" },
    { '\u0087', "ESA" },
    { '\u0088', "HTS" },
    { '\u0089', "HTJ" },
    { '\u008A', "VTS" },
    { '\u008B', "PLD" },
    { '\u008C', "PLU" },
    { '\u008D', "RI" },
    { '\u008E', "SS2" },
    { '\u008F', "SS3" },
    { '\u0090', "DCS" },
    { '\u0091', "PU1" },
    { '\u0092', "PU2" },
    { '\u0093', "STS" },
    { '\u0094', "CCH" },
    { '\u0095', "MW" },
    { '\u0096', "SPA" },
    { '\u0097', "EPA" },
    { '\u0098', "SOS" },
    { '\u0099', "SGC" },
    { '\u009A', "SCI" },
    { '\u009B', "CSI" },
    { '\u009C', "ST" },
    { '\u009D', "OSC" },
    { '\u009E', "PM" },
    { '\u009F', "APC" },
    { '\u00A0', "nbsp" },                         // No-break space
    { '\u2000', "enquad" },                       // En quad
    { '\u2001', "emquad" },                       // Em quad
    { '\u2002', "ensp" },                         // En space
    { '\u2003', "emsp" },                         // Em space
    { '\u2004', "tpmsp" },                        // Three-per-em space
    { '\u2005', "fpmsp" },                        // Four-per-em space
    { '\u2006', "spmsp" },                        // Six-per-em space
    { '\u2007', "numsp" },                        // Figure space
    { '\u2008', "pctsp" },                        // Punctuation space
    { '\u2009', "thinsp" },                       // Thin space
    { '\u200A', "hairsp" },                       // Hair space
    { '\u2028', "line" },                         // Line separator
    { '\u2029', "par" },                          // Paragraph separator
    { '\u202F', "nnbsp" },                        // Narrow no-break space
    { '\u205F', "mmsp" },                         // Medium mathematical space
    { '\u3000', "idsp" },                         // Ideographic space
    { '\u2010', "hp" },                           // Hyphen
    { '\u2011', "nbhp" },                         // Non-breaking hyphen
    { '\u2012', "fdash" },                        // Figure dash
    { '\u2013', "endash" },                       // En dash
    { '\u2014', "emdash" },                       // Em dash
    { '\u2015', "longdash" },                     // Horizontal bar
    { '\u2027', "hpp" },                          // Hyphenation point
    { '\u2043', "hpb" },                          // Hyphen bullet
    { '\u2E17', "hpdo" },                         // Double oblique hyphen
    { '\u2E1A', "hpdi" },                         // Hyphen with dieresis
    { '\u2E40', "hpd" },                          // Double hyphen
    { '\u301C', "wdash" },                        // Wave dash
    { '\u2030', "wwdash" },                       // Wavy dash
    { '\u00AD', "sh" },                           // Soft hyphen
    { '\u200B', "zwsp" },                         // Zero width space
    { '\u200C', "zwnj" },                         // Zero width non-joiner
    { '\u200D', "zwj" },                          // Zero width joiner
    { '\u200E', "ltr" },                          // Left-to-right mark
    { '\u200F', "rtl" },                          // Right-to-left mark
    { '\u202A', "ltre" },                         // Left-to-right embedding
    { '\u202B', "rtle" },                         // Right-to-left embedding
    { '\u202C', "pdf" },                          // Pop directional formatting
    { '\u202D', "ltro" },                         // Left-to-right override
    { '\u202E', "rtlo" },                         // Right-to-left override
    { '\u2060', "wj" },                           // Word joiner
    { '\u2061', "fa" },                           // Function application
    { '\u2062', "nvtimes" },                      // Invisible times
    { '\u2063', "nvsep" },                        // Invisible separator
    { '\u2064', "nvplus" },                       // Invisible plus
    { '\u2066', "ltri" },                         // Left-to-right isolate
    { '\u2067', "rtli" },                         // Right-to-left isolate
    { '\u2068', "fsi" },                          // First strong isolate
    { '\u2069', "pdi" },                          // Pop directional isolate
    { '\u206A', "ssi" },                          // Inhibit symmetric swapping
    { '\u206B', "ssa" },                          // Activate symmetric swapping
    { '\u206C', "afsi" },                         // Inhibit Arabic form shaping
    { '\u206D', "afsa" },                         // Activate Arabic form shaping
    { '\u206E', "nds" },                          // National digit shapes
    { '\u206F', "mds" },                          // Nominal digit shapes
    { '\uFEFF', "zwnbsp" },                       // Zero width no-break space
    { '\uFFF9', "iaa" },                          // Interlinear annotation anchor
    { '\uFFFA', "ias" },                          // Interlinear annotation separator
    { '\uFFFB', "iat" },                          // Interlinear annotation terminator
    { '\u005E', "circumflex" },                   // Circumflex accent
    { '\u0060', "grave" },                        // Grave accent
    { '\u00A8', "dieresis" },                     // Diaeresis
    { '\u00AF', "macron" },                       // Macron
    { '\u00B4', "acute" },                        // Acute accent
    { '\u00B8', "cedilla" },                      // Cedilla
    { '\u02C2', "arrowheadleft" },                // Modifier letter left arrowhead
    { '\u02C3', "arrowheadright" },               // Modifier letter right arrowhead
    { '\u02C4', "arrowheadup" },                  // Modifier letter up arrowhead
    { '\u02C5', "arrowheaddown" },                // Modifier letter down arrowhead
    { '\u02D2', "ringrighthalfcenter" },          // Modifier letter centred right half ring
    { '\u02D3', "ringlefthalfsup" },              // Modifier letter centred left half ring
    { '\u02D4', "tackupmid" },                    // Modifier letter up tack
    { '\u02D5', "tackdownmid" },                  // Modifier letter down tack
    { '\u02D6', "plusmod" },                      // Modifier letter plus sign
    { '\u02D7', "minusmod" },                     // Modifier letter minus sign
    { '\u02D8', "breve" },                        // Breve
    { '\u02D9', "dotaccent" },                    // Dot above
    { '\u02DA', "ring" },                         // Ring above
    { '\u02DB', "ogonek" },                       // Ogonek
    { '\u02DC', "tilde" },                        // Small tilde
    { '\u02DD', "hungarumlaut" },                 // Double acute accent
    { '\u02DE', "rhotichook" },                   // Modifier letter rhotic hook
    { '\u02DF', "qofdagesh" },                    // Modifier letter cross accent
    { '\u02E5', "toneextrahigh" },                // Modifier letter extra-high tone bar
    { '\u02E6', "tonehigh" },                     // Modifier letter high tone bar
    { '\u02E7', "tonemid" },                      // Modifier letter mid tone bar
    { '\u02E8', "tonelow" },                      // Modifier letter low tone bar
    { '\u02E9', "toneextralow" },                 // Modifier letter extra-low tone bar
    { '\u02EA', "ltrmark" },                      // Modifier letter yin departing tone mark
    { '\u02EB', "rtlmark" },                      // Modifier letter yang departing tone mark
    { '\u02ED', "unaspirated" },                  // Modifier letter unaspirated
    { '\u02EF', "arrowheadlowdown" },             // Modifier letter low down arrowhead
    { '\u02F0', "arrowheadlowup" },               // Modifier letter low up arrowhead
    { '\u02F1', "arrowheadlowleft" },             // Modifier letter low left arrowhead
    { '\u02F2', "arrowheadlowright" },            // Modifier letter low right arrowhead
    { '\u02F3', "lowring" },                      // Modifier letter low ring
    { '\u02F4', "midgrave" },                     // Modifier letter middle grave accent
    { '\u02F5', "middgrave" },                    // Modifier letter middle double grave accent
    { '\u02F6', "middacute" },                    // Modifier letter middle double acute accent
    { '\u02F7', "lowtilde" },                     // Modifier letter low tilde
    { '\u02F8', "raisecolon" },                   // Modifier letter raised colon
    { '\u02F9', "hightonestart" },                // Modifier letter begin high tone
    { '\u02FA', "hightoneend" },                  // Modifier letter end high tone
    { '\u02FB', "lowtonestart" },                 // Modifier letter begin low tone
    { '\u02FC', "lowtoneend" },                   // Modifier letter end low tone
    { '\u02FD', "shelf" },                        // Modifier letter shelf
    { '\u02FE', "openshelf" },                    // Modifier letter open shelf
    { '\u02FF', "lowarrowleft" },                 // Modifier letter low left arrow
    { '\u0300', "gravenosp" },                    // Combining grave accent
    { '\u0301', "acutenosp" },                    // Combining acute accent
    { '\u0302', "circumflexnosp" },               // Combining circumflex accent
    { '\u0303', "tildenosp" },                    // Combining tilde
    { '\u0304', "macronnosp" },                   // Combining macron
    { '\u0305', "overscorenosp" },                // Combining overline
    { '\u0306', "brevenosp" },                    // Combining breve
    { '\u0307', "dotnosp" },                      // Combining dot above
    { '\u0308', "dieresisnosp" },                 // Combining diaeresis
    { '\u0309', "hooksupnosp" },                  // Combining hook above
    { '\u030A', "ringnosp" },                     // Combining ring above
    { '\u030B', "acutedblnosp" },                 // Combining double acute accent
    { '\u030C', "haceknosp" },                    // Combining caron
    { '\u030D', "linevertnosp" },                 // Combining vertical line above
    { '\u030E', "linevertdblnosp" },              // Combining double vertical line above
    { '\u030F', "gravedblnosp" },                 // Combining double grave accent
    { '\u0310', "candrabindunosp" },              // Combining candrabindu
    { '\u0311', "breveinvnosp" },                 // Combining inverted breve
    { '\u0312', "commaturnsupnosp" },             // Combining turned comma above
    { '\u0313', "apostrophesupnosp" },            // Combining comma above
    { '\u0314', "commasuprevnosp" },              // Combining reversed comma above
    { '\u0315', "commasuprightnosp" },            // Combining comma above right
    { '\u0316', "gravesubnosp" },                 // Combining grave accent below
    { '\u0317', "acutesubnosp" },                 // Combining acute accent below
    { '\u0318', "tackleftsubnosp" },              // Combining left tack below
    { '\u0319', "tackrightsubnosp" },             // Combining right tack below
    { '\u031A', "anglesupnosp" },                 // Combining left angle above
    { '\u031B', "hornnosp" },                     // Combining horn
    { '\u031C', "ringlefthalfsubnosp" },          // Combining left half ring below
    { '\u031D', "tackupsubnosp" },                // Combining up tack below
    { '\u031E', "tackdownsubnosp" },              // Combining down tack below
    { '\u031F', "plussubnosp" },                  // Combining plus sign below
    { '\u0320', "minussubnosp" },                 // Combining minus sign below
    { '\u0321', "hooksubpalatnosp" },             // Combining palatalized hook below
    { '\u0322', "hooksubretronosp" },             // Combining retroflex hook below
    { '\u0323', "dotsubnosp" },                   // Combining dot below
    { '\u0324', "dotdblsubnosp" },                // Combining diaeresis below
    { '\u0325', "ringsubnosp" },                  // Combining ring below
    { '\u0326', "commasubnosp" },                 // Combining comma below
    { '\u0327', "cedillanosp" },                  // Combining cedilla
    { '\u0328', "ogoneknosp" },                   // Combining ogonek
    { '\u0329', "linevertsubnosp" },              // Combining vertical line below
    { '\u032A', "bridgesubnosp" },                // Combining bridge below
    { '\u032B', "archdblsubnosp" },               // Combining inverted double arch below
    { '\u032C', "haceksubnosp" },                 // Combining caron below
    { '\u032D', "circumflexsubnosp" },            // Combining circumflex accent below
    { '\u032E', "brevesubnosp" },                 // Combining breve below
    { '\u032F', "breveinvsubnosp" },              // Combining inverted breve below
    { '\u0330', "tildesubnosp" },                 // Combining tilde below
    { '\u0331', "macronsubnosp" },                // Combining macron below
    { '\u0332', "underscorenosp" },               // Combining low line
    { '\u0333', "underscoredblnosp" },            // Combining double low line
    { '\u0334', "tildemidnosp" },                 // Combining tilde overlay
    { '\u0335', "barmidshortnosp" },              // Combining short stroke overlay
    { '\u0336', "barmidlongnosp" },               // Combining long stroke overlay
    { '\u0337', "slashshortnosp" },               // Combining short solidus overlay
    { '\u0338', "slashlongnosp" },                // Combining long solidus overlay
    { '\u0339', "ringrighthalfsubnosp" },         // Combining right half ring below
    { '\u033A', "bridgeinvsubnosp" },             // Combining inverted bridge below
    { '\u033B', "squaresubnosp" },                // Combining square below
    { '\u033C', "seagullsubnosp" },               // Combining seagull below
    { '\u033D', "xsupnosp" },                     // Combining x above
    { '\u033E', "tildevertsupnosp" },             // Combining vertical tilde
    { '\u033F', "overscoredblnosp" },             // Combining double overline
    { '\u0340', "graveleftnosp" },                // Combining grave tone mark
    { '\u0341', "acuterightnosp" },               // Combining acute tone mark
    { '\u0342', "perispomenigreekcmb" },          // Combining greek perispomeni
    { '\u0343', "koroniscmb" },                   // Combining greek koronis
    { '\u0344', "diaeresistonosnosp" },           // Combining greek dialytika tonos
    { '\u0345', "iotasubnosp" },                  // Combining greek ypogegrammeni
    { '\u2070', "sup{0}" },                       // Superscript zero
    { '\u00B9', "sup{1}" },                       // Superscript one
    { '\u00B2', "sup{2}" },                       // Superscript two
    { '\u00B3', "sup{3}" },                       // Superscript three
    { '\u2074', "sup{4}" },                       // Superscript four
    { '\u2075', "sup{5}" },                       // Superscript five
    { '\u2076', "sup{6}" },                       // Superscript six
    { '\u2077', "sup{7}" },                       // Superscript seven
    { '\u2078', "sup{8}" },                       // Superscript eight
    { '\u2079', "sup{9}" },                       // Superscript nine
    { '\u207A', "sup{+}" },                       // Superscript plus sign
    { '\u207B', "sup{-}" },                       // Superscript minus
    { '\u207C', "sup{=}" },                       // Superscript equals sign
    { '\u207D', "sup{(}" },                       // Superscript left parenthesis
    { '\u207E', "sup{)}" },                       // Superscript right parenthesis
    { '\u2071', "sup{i}" },                       // Superscript latin small letter I
    { '\u207F', "sup{n}" },                       // Superscript latin small letter N
    { '\u2080', "sub{0}" },                       // Subscript zero
    { '\u2081', "sub{1}" },                       // Subscript one
    { '\u2082', "sub{2}" },                       // Subscript two
    { '\u2083', "sub{3}" },                       // Subscript three
    { '\u2084', "sub{4}" },                       // Subscript four
    { '\u2085', "sub{5}" },                       // Subscript five
    { '\u2086', "sub{6}" },                       // Subscript six
    { '\u2087', "sub{7}" },                       // Subscript seven
    { '\u2088', "sub{8}" },                       // Subscript eight
    { '\u2089', "sub{9}" },                       // Subscript nine
    { '\u208A', "sub{+}" },                       // Subscript plus sign
    { '\u208B', "sub{-}" },                       // Subscript minus
    { '\u208C', "sub{=}" },                       // Subscript equals sign
    { '\u208D', "sub{(}" },                       // Subscript left parenthesis
    { '\u208E', "sub{)}" },                       // Subscript right parenthesis
    { '\u2090', "sub{a}" },                       // Latin subscript small letter A
    { '\u2091', "sub{e}" },                       // Latin subscript small letter E
    { '\u2092', "sub{o}" },                       // Latin subscript small letter O
    { '\u2093', "sub{x}" },                       // Latin subscript small letter X
    { '\u2094', "sub{Ə}" },                       // Latin subscript small letter schwa
    { '\u2095', "sub{h}" },                       // Latin subscript small letter H
    { '\u2096', "sub{k}" },                       // Latin subscript small letter K
    { '\u2097', "sub{l}" },                       // Latin subscript small letter L
    { '\u2098', "sub{m}" },                       // Latin subscript small letter M
    { '\u2099', "sub{n}" },                       // Latin subscript small letter N
    { '\u209A', "sub{p}" },                       // Latin subscript small letter P
    { '\u209B', "sub{s}" },                       // Latin subscript small letter S
    { '\u209C', "sub{t}" },                       // Latin subscript small letter T
    { '\u2C7C', "sub{j}" },                       // Latin subscript small letter J
    { '\u2160', "romanOne" },                     // Roman numeral one
    { '\u2161', "romanTwo" },                     // Roman numeral two
    { '\u2162', "romanThree" },                   // Roman numeral three
    { '\u2163', "romanFour" },                    // Roman numeral four
    { '\u2164', "romanFive" },                    // Roman numeral five
    { '\u2165', "romanSix" },                     // Roman numeral six
    { '\u2166', "romanSeven" },                   // Roman numeral seven
    { '\u2167', "romanEight" },                   // Roman numeral eight
    { '\u2168', "romanNine" },                    // Roman numeral nine
    { '\u2169', "romanTen" },                     // Roman numeral ten
    { '\u216A', "romanEleven" },                  // Roman numeral eleven
    { '\u216B', "romanTwelve" },                  // Roman numeral twelve
    { '\u216C', "romanFifty" },                   // Roman numeral fifty
    { '\u216D', "romanHundred" },                 // Roman numeral one hundred
    { '\u216E', "romanFivehundred" },             // Roman numeral five hundred
    { '\u216F', "romanThousand" },                // Roman numeral one thousand
    { '\u2170', "romanone" },                     // Small roman numeral one
    { '\u2171', "romantwo" },                     // Small roman numeral two
    { '\u2172', "romanthree" },                   // Small roman numeral three
    { '\u2173', "romanfour" },                    // Small roman numeral four
    { '\u2174', "romanfive" },                    // Small roman numeral five
    { '\u2175', "romansix" },                     // Small roman numeral six
    { '\u2176', "romanseven" },                   // Small roman numeral seven
    { '\u2177', "romaneight" },                   // Small roman numeral eight
    { '\u2178', "romannine" },                    // Small roman numeral nine
    { '\u2179', "romanten" },                     // Small roman numeral ten
    { '\u217A', "romaneleven" },                  // Small roman numeral eleven
    { '\u217B', "romantwelve" },                  // Small roman numeral twelve
    { '\u217C', "romanfifty" },                   // Small roman numeral fifty
    { '\u217D', "romanhundred" },                 // Small roman numeral one hundred
    { '\u217E', "romanfivehundred" },             // Small roman numeral five hundred
    { '\u217F', "romanthousand" },                // Small roman numeral one thousand
  };

}
