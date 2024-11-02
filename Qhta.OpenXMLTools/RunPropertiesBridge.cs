namespace Qhta.OpenXmlTools;

/// <summary>
/// Run properties class that is a bridge for:
/// <list type="bullet">
/// <item><c>RunProperties</c> element</item>
/// <item><c>PreviousRunProperties</c> element</item>
/// <item><c>RunPropertiesBaseStyle</c> element</item>
/// <item><c>StyleRunProperties</c> element</item>
/// <item><c>ParagraphMarkRunProperties</c> element</item>
/// <item><c>PreviousParagraphMarkRunProperties</c> element</item>
/// <item><c>NumberingSymbolRunProperties</c> element</item>
/// <item><c>Math.RunProperties</c> element</item>
/// </list>
/// </summary>
public class RunPropertiesBridge
{
  /// <summary>
  /// Base element that is a bridge for a valid run properties element.
  /// </summary>
  public DX.OpenXmlCompositeElement BaseElement { get; private set; }

  /// <summary>
  /// Creates a new instance of the <c>RunPropertiesBridge</c> class from a <c>RunProperties</c> element.
  /// </summary>
  /// <param name="runProperties"></param>
  public RunPropertiesBridge(DXW.RunProperties runProperties)
  {
    BaseElement = runProperties;
  }

  /// <summary>
  /// Creates a new instance of the <c>RunPropertiesBridge</c> class from a <c>PreviousRunProperties</c> element.
  /// </summary>
  /// <param name="previousRunProperties"></param>
  public RunPropertiesBridge(DXW.PreviousRunProperties previousRunProperties)
  {
    BaseElement = previousRunProperties;
  }

  /// <summary>
  /// Creates a new instance of the <c>RunPropertiesBridge</c> class from a <c>RunPropertiesBaseStyle</c> element.
  /// </summary>
  /// <param name="RunPropertiesBaseStyle"></param>
  public RunPropertiesBridge(DXW.RunPropertiesBaseStyle RunPropertiesBaseStyle)
  {
    BaseElement = RunPropertiesBaseStyle;
  }

  /// <summary>
  /// Creates a new instance of the <c>RunPropertiesBridge</c> class from a <c>StyleRunProperties</c> element.
  /// </summary>
  /// <param name="styleRunProperties"></param>
  public RunPropertiesBridge(DXW.StyleRunProperties styleRunProperties)
  {
    BaseElement = styleRunProperties;
  }

  /// <summary>
  /// Creates a new instance of the <c>RunPropertiesBridge</c> class from a <c>ParagraphMarkRunProperties</c> element.
  /// </summary>
  /// <param name="paragraphMarkRunProperties"></param>
  public RunPropertiesBridge(DXW.ParagraphMarkRunProperties paragraphMarkRunProperties)
  {
    BaseElement = paragraphMarkRunProperties;
  }

  /// <summary>
  /// Creates a new instance of the <c>RunPropertiesBridge</c> class from a <c>PreviousParagraphMarkRunProperties</c> element.
  /// </summary>
  /// <param name="previousParagraphMarkRunProperties"></param>
  public RunPropertiesBridge(DXW.PreviousParagraphMarkRunProperties previousParagraphMarkRunProperties)
  {
    BaseElement = previousParagraphMarkRunProperties;
  }

  /// <summary>
  /// Creates a new instance of the <c>RunPropertiesBridge</c> class from a <c>NumberingSymbolRunProperties</c> element.
  /// </summary>
  /// <param name="numberingSymbolRunProperties"></param>
  public RunPropertiesBridge(DXW.NumberingSymbolRunProperties numberingSymbolRunProperties)
  {
    BaseElement = numberingSymbolRunProperties;
  }

  /// <summary>
  /// Creates a new instance of the <c>RunPropertiesBridge</c> class from a <c>RunProperties</c> element.
  /// </summary>
  /// <param name="mathRunProperties"></param>
  public RunPropertiesBridge(DXM.RunProperties mathRunProperties)
  {
    BaseElement = mathRunProperties;
  }


  /// <summary>
  /// Gets or sets the font name.
  /// Gets the first available attribute from: <c>HighAnsiFont</c>, <c>AsciiFont</c>, <c>ComplexScriptFont</c>, <c>EastAsiaFont</c>.
  /// Sets the font name for all attributes.
  /// </summary>
  public string? FontName
  {
    get => HighAnsiFont ?? AsciiFont ?? ComplexScriptFont ?? EastAsiaFont;
    set
    {
      HighAnsiFont = value;
      AsciiFont = value;
      ComplexScriptFont = value;
      EastAsiaFont = value;
    }
  }

  /// <summary>
  /// Gets or sets the font name for ASCII characters.
  /// </summary>
  public string? AsciiFont
  {
    get => BaseElement.GetFirstChild<DXW.RunFonts>()?.Ascii;
    set
    {
      var runFonts = BaseElement.GetFirstChild<DXW.RunFonts>();
      if (runFonts == null)
      {
        runFonts = new DXW.RunFonts();
        BaseElement.AppendChild(runFonts);
      }
      runFonts.Ascii = value;
    }
  }

  /// <summary>
  /// Gets or sets the font name for high ANSI characters.
  /// </summary>
  public string? HighAnsiFont
  {
    get
    {
      var runFonts = BaseElement.GetFirstChild<DXW.RunFonts>();
      if (runFonts!=null)
      {
        if (runFonts.HighAnsi != null)
          return runFonts.HighAnsi;
        if (runFonts.HighAnsiTheme != null)
        {
          var themeFontValues = runFonts.HighAnsiTheme.Value;
          var fontScheme = BaseElement.GetMainDocumentPart()?.ThemePart?.Theme?.ThemeElements?.FontScheme;
          if (fontScheme != null)
          {
            var themeFont = fontScheme.MinorFont?.LatinFont ?? fontScheme.MajorFont?.LatinFont;
            if (themeFont != null)
            {
              var themeFontName = themeFont.Typeface;
              if (themeFontName != null)
              {
                return themeFontName;
              }
            }
          }
        }
      }
      return null;
    }
    set
    {
      var runFonts = BaseElement.GetFirstChild<DXW.RunFonts>();
      if (runFonts == null)
      {
        runFonts = new DXW.RunFonts();
        BaseElement.AppendChild(runFonts);
      }
      runFonts.HighAnsi = value;
    }
  }

  /// <summary>
  /// Gets or sets the font name for complex script characters.
  /// </summary>
  public string? ComplexScriptFont
  {
    get
    {
      var runFonts = BaseElement.GetFirstChild<DXW.RunFonts>();
      if (runFonts != null)
      {
        if (runFonts.ComplexScript != null)
          return runFonts.ComplexScript;
        if (runFonts.ComplexScriptTheme != null)
        {
          var themeFontValues = runFonts.ComplexScriptTheme.Value;
          var fontScheme = BaseElement.GetMainDocumentPart()?.ThemePart?.Theme?.ThemeElements?.FontScheme;
          if (fontScheme != null)
          {
            var themeFont = fontScheme.MinorFont?.ComplexScriptFont ?? fontScheme.MajorFont?.ComplexScriptFont;
            if (themeFont != null)
            {
              var themeFontName = themeFont.Typeface;
              if (themeFontName != null)
              {
                return themeFontName;
              }
            }
          }
        }
      }
      return null;
    }
    set
    {
      var runFonts = BaseElement.GetFirstChild<DXW.RunFonts>();
      if (runFonts == null)
      {
        runFonts = new DXW.RunFonts();
        BaseElement.AppendChild(runFonts);
      }
      runFonts.ComplexScript = value;
    }
  }

  /// <summary>
  /// Gets or sets the font name for east asian characters.
  /// </summary>
  public string? EastAsiaFont
  {
    get
    {
      var runFonts = BaseElement.GetFirstChild<DXW.RunFonts>();
      if (runFonts != null)
      {
        if (runFonts.EastAsia != null)
          return runFonts.EastAsia;
        if (runFonts.EastAsiaTheme != null)
        {
          var themeFontValues = runFonts.EastAsiaTheme.Value;
          var fontScheme = BaseElement.GetMainDocumentPart()?.ThemePart?.Theme?.ThemeElements?.FontScheme;
          if (fontScheme != null)
          {
            var themeFont = fontScheme.MinorFont?.EastAsianFont ?? fontScheme.MajorFont?.EastAsianFont;
            if (themeFont != null)
            {
              var themeFontName = themeFont.Typeface;
              if (themeFontName != null)
              {
                return themeFontName;
              }
            }
          }
        }
      }
      return null;
    }
    set
    {
      var runFonts = BaseElement.GetFirstChild<DXW.RunFonts>();
      if (runFonts == null)
      {
        runFonts = new DXW.RunFonts();
        BaseElement.AppendChild(runFonts);
      }
      runFonts.EastAsia = value;
    }
  }

  /// <summary>
  /// Gets or sets the language.
  /// </summary>
  public string? Language
  {
    get
    {
      var languages = BaseElement.GetFirstChild<DXW.Languages>();
      if (languages != null)
      {
        if (languages.Val != null)
          return languages.Val;
      }
      var themeFontLanguages = BaseElement.GetFirstChild<DXW.ThemeFontLanguages>();
      if (themeFontLanguages != null)
      {
        if (themeFontLanguages.Val != null)
          return themeFontLanguages.Val;
      }
      return null;
    }
    set
    {
      var languages = BaseElement.GetFirstChild<DXW.Languages>();
      if (languages == null)
      {
        languages = new DXW.Languages();
        BaseElement.AppendChild(languages);
      }
      languages.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the bidirectional language.
  /// </summary>
  public string? BiDiLanguage
  {
    get
    {
      var languages = BaseElement.GetFirstChild<DXW.Languages>();
      if (languages != null)
      {
        if (languages.Bidi != null)
          return languages.Bidi;
      }
      var themeFontLanguages = BaseElement.GetFirstChild<DXW.ThemeFontLanguages>();
      if (themeFontLanguages != null)
      {
        if (themeFontLanguages.Bidi != null)
          return themeFontLanguages.Bidi;
      }
      return null;
    }
    set
    {
      var languages = BaseElement.GetFirstChild<DXW.Languages>();
      if (languages == null)
      {
        languages = new DXW.Languages();
        BaseElement.AppendChild(languages);
      }
      languages.Bidi = value;
    }
  }

  /// <summary>
  /// Gets or sets the east asia language.
  /// </summary>
  public string? EastAsiaLanguage
  {
    get
    {
      var languages = BaseElement.GetFirstChild<DXW.Languages>();
      if (languages != null)
      {
        if (languages.EastAsia != null)
          return languages.EastAsia;
      }
      var themeFontLanguages = BaseElement.GetFirstChild<DXW.ThemeFontLanguages>();
      if (themeFontLanguages != null)
      {
        if (themeFontLanguages.EastAsia != null)
          return themeFontLanguages.EastAsia;
      }
      return null;
    }
    set
    {
      var languages = BaseElement.GetFirstChild<DXW.Languages>();
      if (languages == null)
      {
        languages = new DXW.Languages();
        BaseElement.AppendChild(languages);
      }
      languages.EastAsia = value;
    }
  }
  /// <summary>
  /// Gets or sets the <c>Bold</c> attribute.
  /// </summary>
  public bool? Bold
  {
    get => BaseElement.GetFirstChild<DXW.Bold>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.Bold>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.Bold();
        if (value == false)
          element.Val = false;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>BoldComplexScript</c> attribute.
  /// </summary>
  public bool? BoldComplexScript
  {
    get => BaseElement.GetFirstChild<DXW.BoldComplexScript>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.BoldComplexScript>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.BoldComplexScript();
        if (value == false)
          element.Val = false;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>Italic</c> attribute.
  /// </summary>
  public bool? Italic
  {
    get => BaseElement.GetFirstChild<DXW.Italic>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.Italic>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.Italic();
        if (value == false)
          element.Val = false;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>ItalicComplexScript</c> attribute.
  /// </summary>
  public bool? ItalicComplexScript
  {
    get => BaseElement.GetFirstChild<DXW.ItalicComplexScript>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.ItalicComplexScript>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.ItalicComplexScript();
        if (value == false)
          element.Val = false;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>Caps</c> attribute.
  /// </summary>
  public bool? Caps
  {
    get => BaseElement.GetFirstChild<DXW.Caps>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.Caps>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.Caps();
        if (value == false)
          element.Val = false;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>SmallCaps</c> attribute.
  /// </summary>
  public bool? SmallCaps
  {
    get => BaseElement.GetFirstChild<DXW.SmallCaps>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.SmallCaps>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.SmallCaps();
        if (value == false)
          element.Val = false;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>Strike</c> attribute.
  /// </summary>
  public bool? Strike
  {
    get => BaseElement.GetFirstChild<DXW.Strike>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.Strike>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.Strike();
        if (value == false)
          element.Val = false;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>DoubleStrike</c> attribute.
  /// </summary>
  public bool? DoubleStrike
  {
    get => BaseElement.GetFirstChild<DXW.DoubleStrike>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.DoubleStrike>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.DoubleStrike();
        if (value == false)
          element.Val = false;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>Outline</c> attribute.
  /// </summary>
  public bool? Outline
  {
    get => BaseElement.GetFirstChild<DXW.Outline>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.Outline>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.Outline();
        if (value == false)
          element.Val = false;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>Shadow</c> attribute.
  /// </summary>
  public bool? Shadow
  {
    get => BaseElement.GetFirstChild<DXW.Shadow>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.Shadow>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.Shadow();
        if (value == false)
          element.Val = false;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>Emboss</c> attribute.
  /// </summary>
  public bool? Emboss
  {
    get => BaseElement.GetFirstChild<DXW.Emboss>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.Emboss>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.Emboss();
        if (value == false)
          element.Val = false;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>Imprint</c> attribute.
  /// </summary>
  public bool? Imprint
  {
    get => BaseElement.GetFirstChild<DXW.Imprint>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.Imprint>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.Imprint();
        if (value == false)
          element.Val = false;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>NoProof</c> attribute.
  /// </summary>
  public bool? NoProof
  {
    get => BaseElement.GetFirstChild<DXW.NoProof>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.NoProof>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.NoProof();
        if (value == false)
          element.Val = false;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>SnapToGrid</c> attribute.
  /// </summary>
  public bool? SnapToGrid
  {
    get => BaseElement.GetFirstChild<DXW.SnapToGrid>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.SnapToGrid>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.SnapToGrid();
        if (value == false)
          element.Val = false;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>Vanish</c> attribute.
  /// </summary>
  public bool? Vanish
  {
    get => BaseElement.GetFirstChild<DXW.Vanish>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.Vanish>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.Vanish();
        if (value == false)
          element.Val = false;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>SpecVanish</c> attribute.
  /// </summary>
  public bool? SpecVanish
  {
    get => BaseElement.GetFirstChild<DXW.SpecVanish>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.SpecVanish>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.SpecVanish();
        if (value == false)
          element.Val = false;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }


  /// <summary>
  /// Gets or sets the <c>WebHidden</c> attribute.
  /// </summary>
  public bool? WebHidden
  {
    get => BaseElement.GetFirstChild<DXW.WebHidden>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.WebHidden>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.WebHidden();
        if (value == false)
          element.Val = false;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>Color</c> attribute.
  /// </summary>
  public string? Color
  {
    get => BaseElement.GetFirstChild<DXW.Color>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.Color>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.Color();
        element.Val = value;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>Spacing</c> attribute.
  /// </summary>
  public int? Spacing
  {
    get => BaseElement.GetFirstChild<DXW.Spacing>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.Spacing>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.Spacing();
        element.Val = value;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>CharacterScale</c> attribute.
  /// </summary>
  public long? CharacterScale
  {
    get => BaseElement.GetFirstChild<DXW.CharacterScale>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.CharacterScale>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.CharacterScale();
        element.Val = value;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }


  /// <summary>
  /// Gets or sets the <c>Kern</c> attribute.
  /// </summary>
  public uint? Kern
  {
    get => BaseElement.GetFirstChild<DXW.Kern>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.Kern>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.Kern();
        element.Val = value;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>FitText</c> attribute.
  /// </summary>
  public uint? FitText
  {
    get => BaseElement.GetFirstChild<DXW.FitText>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.FitText>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.FitText();
        element.Val = value;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>Position</c> attribute.
  /// </summary>
  public string? Position
  {
    get => BaseElement.GetFirstChild<DXW.Position>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.Position>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.Position();
        element.Val = value;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>FontSize</c> attribute.
  /// </summary>
  public string? FontSize
  {
    get => BaseElement.GetFirstChild<DXW.FontSize>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.FontSize>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.FontSize();
        element.Val = value;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>FontSizeComplexScript</c> attribute.
  /// </summary>
  public string? FontSizeComplexScript
  {
    get => BaseElement.GetFirstChild<DXW.FontSizeComplexScript>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.FontSizeComplexScript>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.FontSizeComplexScript();
        element.Val = value;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>Underline</c> attribute.
  /// </summary>
  public DXW.UnderlineValues? Underline
  {
    get => BaseElement.GetFirstChild<DXW.Underline>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.Underline>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.Underline();
        element.Val = value;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }


  /// <summary>
  /// Gets or sets the <c>TextEffect</c> attribute.
  /// </summary>
  public DXW.TextEffectValues? TextEffect
  {
    get => BaseElement.GetFirstChild<DXW.TextEffect>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.TextEffect>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.TextEffect();
        element.Val = value;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>Border</c> attribute.
  /// </summary>
  public DXW.BorderValues? Border
  {
    get => BaseElement.GetFirstChild<DXW.Border>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.Border>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.Border();
        element.Val = value;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>Shading</c> attribute.
  /// </summary>
  public DXW.ShadingPatternValues? Shading
  {
    get => BaseElement.GetFirstChild<DXW.Shading>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.Shading>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.Shading();
        element.Val = value;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>VerticalTextAlignment</c> attribute.
  /// </summary>
  public DXW.VerticalPositionValues? VerticalTextAlignment
  {
    get => BaseElement.GetFirstChild<DXW.VerticalTextAlignment>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.VerticalTextAlignment>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.VerticalTextAlignment();
        element.Val = value;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>Emphasis</c> attribute.
  /// </summary>
  public DXW.EmphasisMarkValues? Emphasis
  {
    get => BaseElement.GetFirstChild<DXW.Emphasis>()?.Val!;
    set
    {
      var element = BaseElement.GetFirstChild<DXW.Emphasis>();
      if (element == null)
      {
        if (value == null)
          return;
        element = new DXW.Emphasis();
        element.Val = value;
        BaseElement.AppendChild(element);
      }
      else if (value == null)
        element.Remove();
      else
        element.Val = value;
    }
  }

  /// <summary>
  /// Gets or sets the <c>EastAsianLayout</c> child element.
  /// </summary>
  public DXW.EastAsianLayout? EastAsianLayout
  {
    get => BaseElement.GetFirstChild<DXW.EastAsianLayout>();
    set
    {
      var element = BaseElement.GetFirstChild<DXW.EastAsianLayout>();
      if (element == null)
      {
        if (value == null)
          return;
        element = value;
        BaseElement.AppendChild(element);
      }
      else
      {
        if (element != value)
        {
          element.Remove();
          element = value;
          BaseElement.AppendChild(element);
        }
      }
    }
  }

}