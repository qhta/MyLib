using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;

using Microsoft.Office.Core;
using Qhta.TextUtils;
using Word = Microsoft.Office.Interop.Word;

namespace Qhta.WordInteropOpenXmlConverter;

public class ThemeTools
{
  public ThemeTools(Word.Document activeDocument)
  {
    Word.Application wordApp = activeDocument.Application;
    themeFontNames = new Dictionary<string, ThemeFontValues>
    {
      {"+" + Strings.Body, ThemeFontValues.MinorHighAnsi},
      {"+" + Strings.Body_CS, ThemeFontValues.MinorBidi},
      {"+" + Strings.Body_EA, ThemeFontValues.MinorEastAsia},
      {"+" + Strings.Headings, ThemeFontValues.MajorHighAnsi},
      {"+" + Strings.Headings_CS, ThemeFontValues.MajorBidi},
      {"+" + Strings.Headings_EA, ThemeFontValues.MajorEastAsia},
    };
  }

  public CultureInfo  GetWordApplicationCulture(Word.Application wordApp)
  {
    // Get the language ID of the Word application
    int languageId = (int)wordApp.LanguageSettings.LanguageID[MsoAppLanguageID.msoLanguageIDUI];

    // Convert the language ID to a CultureInfo object
    CultureInfo cultureInfo = new System.Globalization.CultureInfo(languageId);
    return cultureInfo;
  }

  private readonly Dictionary<string, ThemeFontValues> themeFontNames;
  
  public bool TryGetThemeFont(string fontName, out ThemeFontValues themeFont)
  {
    if (fontName.StartsWith("+"))
    {
      if (themeFontNames.TryGetValue(fontName, out var themeFont1))
      {
        themeFont = themeFont1;
        return true;
      }
    }
    themeFont = ThemeFontValues.MinorHighAnsi;
    return false;
  }
}
