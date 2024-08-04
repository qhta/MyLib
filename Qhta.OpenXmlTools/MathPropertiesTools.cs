using System;

using DocumentFormat.OpenXml.Math;

namespace Qhta.OpenXmlTools;
/// <summary>
/// Tools for working with math properties
/// </summary>
public static class MathPropertiesTools
{
  /// <summary>
  /// Check if the document has math properties
  /// </summary>
  /// <param name="wordDoc">The WordprocessingDocument</param>
  /// <returns>True if the document has math properties</returns>
  public static bool HasMathProperties(this DXPack.WordprocessingDocument wordDoc)
  {
    return wordDoc.MainDocumentPart?.DocumentSettingsPart?.Settings?
      .Elements<DXM.MathProperties>().FirstOrDefault() != null;
  }

  /// <summary>
  /// Gets the math properties from the document. If the document does not have math properties, the properties are created.
  /// </summary>
  /// <param name="wordDoc">The WordprocessingDocument</param>
  /// <returns>The instance of the math properties</returns>
  public static DXM.MathProperties GetMathProperties(this DXPack.WordprocessingDocument wordDoc)
  {
    var settings = wordDoc.GetSettings();
    var mathProperties = settings.GetFirstElement<DXM.MathProperties>();
    if (mathProperties == null)
    {
      mathProperties = new DXM.MathProperties();
      settings.Append(mathProperties);
    }
    return mathProperties;
  }


  /// <summary>
  /// Get the count of the properties.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="filter">specifies if all property names should be counted or non-empty ones</param>
  /// <returns></returns>
  public static int Count(this DXM.MathProperties properties, ItemFilter filter = ItemFilter.Defined)
  {
    if (filter == ItemFilter.All)
      return PropTypes.Count;
    return PropTypes.Count(item => properties.GetValue(item.Key) != null);
  }

  /// <summary>
  /// Get the names of the properties.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="filter">specifies if all property names should be listed or non-empty ones</param>
  /// <returns></returns>
  public static string[] GetNames(this DXM.MathProperties properties, ItemFilter filter = ItemFilter.Defined)
  {
    if (filter == ItemFilter.All)
      return PropTypes.Keys.ToArray();
    return PropTypes.Where(item => properties.GetValue(item.Key) != null).Select(item => item.Key).ToArray();
  }

  /// <summary>
  /// Get the type of property with its name.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="propertyName"></param>
  /// <returns></returns>
  public static Type GetType(this DXM.MathProperties properties, string propertyName)
  {
    if (PropTypes.TryGetValue(propertyName, out var type))
      return type;
    throw new ArgumentException($"Property {propertyName} not found");
  }

  /// <summary>
  /// Gets the <c>WordprocessingDocument</c> from the <c>MathProperties</c> object.
  /// </summary>
  /// <param name="properties"></param>
  /// <result>wordprocessing document</result>
  public static DXPack.WordprocessingDocument? GetDocument(this DXM.MathProperties properties)
  {
    return (properties.Parent as DXW.Settings)?.DocumentSettingsPart?.OpenXmlPackage as DXPack.WordprocessingDocument;
  }

  /// <summary>
  /// Gets the value of a <c>MathProperties</c> property.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="propertyName"></param>
  /// <returns></returns>
  public static object? GetValue(this DXM.MathProperties properties, string propertyName)
  {
    switch (propertyName)
    {
      case "BreakBinary": return properties.GetBreakBinary();
      case "BreakBinarySubtraction": return properties.GetBreakBinarySubtraction();
      case "DefaultJustification": return properties.GetDefaultJustification();
      case "DisplayDefaults": return properties.GetDisplayDefaults();
      case "InterSpacing": return properties.GetInterSpacing();
      case "IntegralLimitLocation": return properties.GetIntegralLimitLocation();
      case "IntraSpacing": return properties.GetIntraSpacing();
      case "LeftMargin": return properties.GetLeftMargin();
      case "MathFont": return properties.GetMathFont();
      case "NaryLimitLocation": return properties.GetNaryLimitLocation();
      case "PostSpacing": return properties.GetPostSpacing();
      case "PreSpacing": return properties.GetPreSpacing();
      case "RightMargin": return properties.GetRightMargin();
      case "SmallFraction": return properties.GetSmallFraction();
      case "WrapIndent": return properties.GetWrapIndent();
      case "WrapRight": return properties.GetWrapRight();
    }
    throw new ArgumentException($"Property {propertyName} not found");
  }

  /// <summary>
  /// Sets the value of a <c>MathProperties</c> property.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="propertyName"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetValue(this DXM.MathProperties properties, string propertyName, object? value)
  {
    switch (propertyName)
    {
      case "BreakBinary": properties.SetBreakBinary((BreakBinaryOperatorValues?)value); return;
      case "BreakBinarySubtraction": properties.SetBreakBinarySubtraction((BreakBinarySubtractionValues?)value); return;
      case "DefaultJustification": properties.SetDefaultJustification((DXM.JustificationValues?)value); return;
      case "DisplayDefaults": properties.SetDisplayDefaults((DXM.BooleanValues?)value); return;
      case "InterSpacing": properties.SetInterSpacing((Twips?)value); return;
      case "IntegralLimitLocation": properties.SetIntegralLimitLocation((LimitLocationValues?)value); return;
      case "IntraSpacing": properties.SetIntraSpacing((Twips?)value); return;
      case "LeftMargin": properties.SetLeftMargin((Twips?)value); return;
      case "MathFont": properties.SetMathFont((string?)value); return;
      case "NaryLimitLocation": properties.SetNaryLimitLocation((LimitLocationValues?)value); return;
      case "PostSpacing": properties.SetPostSpacing((Twips?)value); return;
      case "PreSpacing": properties.SetPreSpacing((Twips?)value); return;
      case "RightMargin": properties.SetRightMargin((Twips?)value); return;
      case "SmallFraction": properties.SetSmallFraction((DXM.BooleanValues?)value); return;
      case "WrapIndent": properties.SetWrapIndent((Twips?)value); return;
      case "WrapRight": properties.SetWrapRight((DXM.BooleanValues?)value); return;
    }
    throw new ArgumentException($"Property {propertyName} not found");
  }

  #region get settings
  /// <summary>
  /// Get the <c>BreakBinary</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <result>result value</result>
  public static DXM.BreakBinaryOperatorValues? GetBreakBinary(this DXM.MathProperties properties) => properties.GetFirstEnumTypeElementVal<DXM.BreakBinary, DXM.BreakBinaryOperatorValues>();

  /// <summary>
  /// Get the <c>BreakBinarySubtraction</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static DXM.BreakBinarySubtractionValues? GetBreakBinarySubtraction(this DXM.MathProperties properties) => properties.GetFirstEnumTypeElementVal<DXM.BreakBinarySubtraction, DXM.BreakBinarySubtractionValues>();

  /// <summary>
  ///  Get the <c>DefaultJustification</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static DXM.JustificationValues? GetDefaultJustification(this DXM.MathProperties properties) => properties.GetFirstEnumTypeElementVal<DXM.DefaultJustification, DXM.JustificationValues>();

  /// <summary>
  /// Get the <c>DisplayDefaults</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static DXM.BooleanValues? GetDisplayDefaults(this DXM.MathProperties properties) => properties.GetFirstOnOffMathTypeElementVal<DXM.DisplayDefaults>();

  /// <summary>
  /// Get 
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static Twips? GetInterSpacing(this MathProperties properties) => properties.GetFirstTwipsMeasureMathTypeElementVal<InterSpacing>();

  /// <summary>
  /// Get the <c>IntegralLimitLocations</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static LimitLocationValues? GetIntegralLimitLocation(this MathProperties properties) => properties.GetFirstEnumTypeElementVal<IntegralLimitLocation, DXM.LimitLocationValues>();

  /// <summary>
  /// Get the <c>IntraSpacing</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static Twips? GetIntraSpacing(this MathProperties properties) => properties.GetFirstTwipsMeasureMathTypeElementVal<IntraSpacing>();

  /// <summary>
  /// Get the <c>LeftMargin</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static Twips? GetLeftMargin(this MathProperties properties) => properties.GetFirstTwipsMeasureMathTypeElementVal<LeftMargin>();

  /// <summary>
  /// Get the <c>MathFont</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static string? GetMathFont(this MathProperties properties) => properties.GetFirstElementStringVal<MathFont>();

  /// <summary>
  /// Get the <c>NaryLimitLocation</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static LimitLocationValues? GetNaryLimitLocation(this MathProperties properties) => properties.GetFirstEnumTypeElementVal<NaryLimitLocation, LimitLocationValues>();

  /// <summary>
  /// Get the <c>PostSpacing</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static Twips? GetPostSpacing(this MathProperties properties) => properties.GetFirstTwipsMeasureMathTypeElementVal<PostSpacing>();

  /// <summary>
  /// Get the <c>PreSpacing</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static Twips? GetPreSpacing(this MathProperties properties) => properties.GetFirstTwipsMeasureMathTypeElementVal<PreSpacing>();

  /// <summary>
  /// Get the <c>RightMargin</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static Twips? GetRightMargin(this MathProperties properties) => properties.GetFirstTwipsMeasureMathTypeElementVal<RightMargin>();

  /// <summary>
  /// Get the <c>SmallFraction</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static DXM.BooleanValues? GetSmallFraction(this MathProperties properties) => properties.GetFirstOnOffMathTypeElementVal<SmallFraction>();

  /// <summary>
  /// Get the <c>WrapIndent</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static Twips? GetWrapIndent(this MathProperties properties) => properties.GetFirstTwipsMeasureMathTypeElementVal<WrapIndent>();

  /// <summary>
  /// Get the <c>WrapRight</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static DXM.BooleanValues? GetWrapRight(this MathProperties properties) => properties.GetFirstOnOffMathTypeElementVal<WrapRight>();
  #endregion get settings

  #region set settings

  /// <summary>
  /// Set <c>BreakBinary</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="value"></param>
  public static void SetBreakBinary(this MathProperties properties, BreakBinaryOperatorValues? value) => properties.SetFirstEnumTypeElementVal<BreakBinary, DXM.BreakBinaryOperatorValues>(value);

  /// <summary>
  /// Set <c>BreakBinarySubtraction</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="value"></param>
  public static void SetBreakBinarySubtraction(this MathProperties properties, BreakBinarySubtractionValues? value) => properties.SetFirstEnumTypeElementVal<BreakBinarySubtraction, DXM.BreakBinarySubtractionValues>(value);

  /// <summary>
  /// Set <c>DefaultJustification</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="value"></param>
  public static void SetDefaultJustification(this MathProperties properties, DXM.JustificationValues? value) => properties.SetFirstEnumTypeElementVal<DefaultJustification, DXM.JustificationValues>(value);

  /// <summary>
  /// Set <c>DisplayDefaults</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="value"></param>
  public static void SetDisplayDefaults(this MathProperties properties, DXM.BooleanValues? value) => properties.SetFirstOnOffMathTypeElementVal<DisplayDefaults>(value);

  /// <summary>
  /// Set
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="value"></param>
  public static void SetInterSpacing(this MathProperties properties, Twips? value) => properties.SetFirstTwipsMeasureMathTypeElementVal<InterSpacing>(value);

  /// <summary>
  /// Set <c>IntegralLimitLocation</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="value"></param>
  public static void SetIntegralLimitLocation(this MathProperties properties, LimitLocationValues? value) => properties.SetFirstEnumTypeElementVal<DXM.IntegralLimitLocation, DXM.LimitLocationValues>(value);

  /// <summary>
  /// Set <c>IntraSpacing</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="value"></param>
  public static void SetIntraSpacing(this MathProperties properties, Twips? value) => properties.SetFirstTwipsMeasureMathTypeElementVal<IntraSpacing>(value);

  /// <summary>
  /// Set <c>LeftMargin</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="value"></param>
  public static void SetLeftMargin(this MathProperties properties, Twips? value) => properties.SetFirstTwipsMeasureMathTypeElementVal<LeftMargin>(value);

  /// <summary>
  /// Set <c>MathFont</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="value"></param>
  public static void SetMathFont(this MathProperties properties, string? value) => properties.SetFirstElementStringVal<MathFont>(value);

  /// <summary>
  /// Set <c>NaryLimitLocation</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="value"></param>
  public static void SetNaryLimitLocation(this MathProperties properties, LimitLocationValues? value) => properties.SetFirstEnumTypeElementVal<NaryLimitLocation, LimitLocationValues>(value);

  /// <summary>
  /// Set <c>PostSpacing</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="value"></param>
  public static void SetPostSpacing(this MathProperties properties, Twips? value) => properties.SetFirstTwipsMeasureMathTypeElementVal<PostSpacing>(value);

  /// <summary>
  /// Set <c>PreSpacing</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="value"></param>
  public static void SetPreSpacing(this MathProperties properties, Twips? value) => properties.SetFirstTwipsMeasureMathTypeElementVal<PreSpacing>(value);

  /// <summary>
  /// Set <c>RightMargin</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="value"></param>
  public static void SetRightMargin(this MathProperties properties, Twips? value) => properties.SetFirstTwipsMeasureMathTypeElementVal<RightMargin>(value);

  /// <summary>
  /// Set <c>SmallFraction</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="value"></param>
  public static void SetSmallFraction(this MathProperties properties, DXM.BooleanValues? value) => properties.SetFirstOnOffMathTypeElementVal<SmallFraction>(value);

  /// <summary>
  /// Set <c>WrapIndent</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="value"></param>
  public static void SetWrapIndent(this MathProperties properties, Twips? value) => properties.SetFirstTwipsMeasureMathTypeElementVal<WrapIndent>(value);

  /// <summary>
  /// Set <c>WrapRight</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="value"></param>
  public static void SetWrapRight(this MathProperties properties, DXM.BooleanValues? value) => properties.SetFirstOnOffMathTypeElementVal<WrapRight>(value);

  #endregion set settings

  private static readonly Dictionary<string, Type> PropTypes = new()
  {
    { "BreakBinary", typeof(DXM.BreakBinaryOperatorValues) },
    { "BreakBinarySubtraction", typeof(DXM.BreakBinarySubtractionValues) },
    { "DefaultJustification", typeof(DXM.JustificationValues) },
    { "DisplayDefaults", typeof(DXM.BooleanValues) },
    { "InterSpacing", typeof(Twips) },
    { "IntegralLimitLocation", typeof(DXM.LimitLocationValues) },
    { "IntraSpacing", typeof(Twips) },
    { "LeftMargin", typeof(Twips) },
    { "MathFont", typeof(string) },
    { "NaryLimitLocation", typeof(DXM.LimitLocationValues) },
    { "PostSpacing", typeof(Twips) },
    { "PreSpacing", typeof(Twips) },
    { "RightMargin", typeof(Twips) },
    { "SmallFraction", typeof(DXM.BooleanValues) },
    { "WrapIndent", typeof(Twips) },
    { "WrapRight", typeof(DXM.BooleanValues) },

  };
}
