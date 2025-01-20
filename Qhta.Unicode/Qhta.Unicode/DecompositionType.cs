namespace Qhta.Unicode;

public enum DecompositionType
{
  Unknown,                // No decomposition type
  Font,                   // Font variant (for example, a blackletter form)
  Nobreak,                // No-break version of a space or hyphen
  Initial,                // Initial presentation form (Arabic)
  Medial,                 // Medial presentation form (Arabic)
  Final,                  // Final presentation form (Arabic)
  Isolated,               // Isolated presentation form (Arabic)
  Circle,                 // Encircled form
  Super,                  // Superscript form
  Sub,                    // Subscript form
  Vertical,               // Vertical layout presentation form
  Wide,                   // Wide (or zenkaku) compatibility character
  Narrow,                 // Narrow (or hankaku) compatibility character
  Small,                  // Small variant form (CNS compatibility)
  Square,                 // CJK squared font variant
  Fraction,               // Vulgar fraction form
  Compat,                 // Otherwise unspecified compatibility character
}