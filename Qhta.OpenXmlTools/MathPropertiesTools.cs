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
      .Elements<DXM.MathProperties>().FirstOrDefault()!=null;
  }

  /// <summary>
  /// Gets the math properties from the document. If the document does not have math properties, the properties are created.
  /// </summary>
  /// <param name="wordDoc">The WordprocessingDocument</param>
  /// <returns>True if the document has math properties</returns>
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
  /// Get the count of all the properties.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="all">specifies if all property names should be counted or non-empty ones</param>
  /// <returns></returns>
  public static int Count(this DXM.MathProperties properties, bool all = false)
  {
    if (all)
      return PropTypes.Count;
    return PropTypes.Count(item => properties.GetValue(item.Key) != null);
  }

  /// <summary>
  /// Get the names of all the properties.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="all">specifies if all property names should be listed or non-empty ones</param>
  /// <returns></returns>
  public static string[] GetNames(this DXM.MathProperties properties, bool all = false)
  {
    if (all)
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
      case "IntegralLimitLocations": return properties.GetIntegralLimitLocations();
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

  #region get settings
  /// <summary>
  /// Get the <c>BreakBinary</c> setting value.
  /// </summary>
  /// <param name="properties"></param>
  /// <result>result value</result>
  public static DXM.BreakBinaryOperatorValues? GetBreakBinary(this DXM.MathProperties properties) => properties.GetFirstEnumTypeElementVal<DXM.BreakBinary, DXM.BreakBinaryOperatorValues>();

  /// <summary>
  /// Get the <c>BreakBinarySubtraction</c> setting value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static DXM.BreakBinarySubtractionValues? GetBreakBinarySubtraction(this DXM.MathProperties properties) => properties.GetFirstEnumTypeElementVal<DXM.BreakBinarySubtraction, DXM.BreakBinarySubtractionValues>();

  /// <summary>
  ///  Get the <c>DefaultJustification</c> setting value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static DXM.JustificationValues? GetDefaultJustification(this DXM.MathProperties properties) => properties.GetFirstEnumTypeElementVal<DXM.DefaultJustification, DXM.JustificationValues>();

  /// <summary>
  /// Get the <c>DisplayDefaults</c> setting value.
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
  /// Get the <c>IntegralLimitLocations</c> setting value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static LimitLocationValues? GetIntegralLimitLocations(this MathProperties properties) => properties.GetFirstEnumTypeElementVal<DXM.IntegralLimitLocation, DXM.LimitLocationValues>();

  /// <summary>
  /// Get the <c>IntraSpacing</c> setting value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static Twips? GetIntraSpacing(this MathProperties properties) => properties.GetFirstTwipsMeasureMathTypeElementVal<IntraSpacing>();

  /// <summary>
  /// Get the <c>LeftMargin</c> setting value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static Twips? GetLeftMargin(this MathProperties properties) => properties.GetFirstTwipsMeasureMathTypeElementVal<LeftMargin>();

  /// <summary>
  /// Get the <c>MathFont</c> setting value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static string? GetMathFont(this MathProperties properties) => properties.GetFirstElementStringVal<MathFont>();

  /// <summary>
  /// Get the <c>NaryLimitLocation</c> setting value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static LimitLocationValues? GetNaryLimitLocation(this MathProperties properties) => properties.GetFirstEnumTypeElementVal<NaryLimitLocation, LimitLocationValues>();

  /// <summary>
  /// Get the <c>PostSpacing</c> setting value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static Twips? GetPostSpacing(this MathProperties properties) => properties.GetFirstTwipsMeasureMathTypeElementVal<PostSpacing>();

  /// <summary>
  /// Get the <c>PreSpacing</c> setting value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static Twips? GetPreSpacing(this MathProperties properties) => properties.GetFirstTwipsMeasureMathTypeElementVal<PreSpacing>();

  /// <summary>
  /// Get the <c>RightMargin</c> setting value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static Twips? GetRightMargin(this MathProperties properties) => properties.GetFirstTwipsMeasureMathTypeElementVal<RightMargin>();

  /// <summary>
  /// Get the <c>SmallFraction</c> setting value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static DXM.BooleanValues? GetSmallFraction(this MathProperties properties) => properties.GetFirstOnOffMathTypeElementVal<SmallFraction>();

  /// <summary>
  /// Get the <c>WrapIndent</c> setting value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static Twips? GetWrapIndent(this MathProperties properties) => properties.GetFirstTwipsMeasureMathTypeElementVal<WrapIndent>();

  /// <summary>
  /// Get the <c>WrapRight</c> setting value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static DXM.BooleanValues? GetWrapRight(this MathProperties properties) => properties.GetFirstOnOffMathTypeElementVal<WrapRight>();






  #endregion get settings

  private static readonly Dictionary<string, Type> PropTypes = new()
  {
    { "BreakBinary", typeof(DXM.BreakBinaryOperatorValues) },
    { "BreakBinarySubtraction", typeof(DXM.BreakBinarySubtractionValues) },
    { "DefaultJustification", typeof(DXM.JustificationValues) },
    { "DisplayDefaults", typeof(DXM.BooleanValues) },
    { "InterSpacing", typeof(Twips) },
    { "IntegralLimitLocations", typeof(DXM.LimitLocationValues) },
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
