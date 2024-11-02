using System;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

using Qhta.TextUtils;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with styles in OpenXml documents.
/// </summary>
public static partial class StylesTools
{

  /// <summary>
  /// Gets the default style settings for the <c>Styles</c> element.
  /// If the <c>Styles</c> element does not have a <c>DocDefaults</c> element, one is created.
  /// </summary>
  /// <param name="styles"></param>
  /// <returns></returns>
  public static DXW.DocDefaults GetDocDefaults(this DXW.Styles styles)
  {
    var docDefaults = styles.DocDefaults;
    if (docDefaults == null)
    {
      docDefaults = new DXW.DocDefaults();
      styles.DocDefaults = docDefaults;
    }
    return docDefaults;
  }


  /// <summary>
  /// Gets the default run properties for the <c>DocDefaults</c> element.
  /// If the <c>DocDefaults</c> element does not have a <c>RunPropertiesDefault</c> element, one is created.
  /// </summary>
  /// <param name="docDefaults"></param>
  /// <returns></returns>
  public static DXW.RunPropertiesDefault GetRunPropertiesDefault(this DXW.DocDefaults docDefaults)
  {
    var runPropertiesDefault = docDefaults.RunPropertiesDefault;
    if (runPropertiesDefault == null)
    {
      runPropertiesDefault = new DXW.RunPropertiesDefault();
      docDefaults.RunPropertiesDefault = runPropertiesDefault;
    }
    return runPropertiesDefault;
  }

  /// <summary>
  /// Gets the run properties base style for the <c>RunPropertiesDefault</c> element.
  /// If the <c>RunPropertiesDefault</c> element does not have a <c>RunProperties</c> element, one is created.
  /// </summary>
  /// <param name="runPropertiesDefault"/>
  /// <returns></returns>
  public static RunPropertiesBridge GetRunProperties(this DXW.RunPropertiesDefault runPropertiesDefault)
  {
    var runPropertiesBaseStyle = runPropertiesDefault.RunPropertiesBaseStyle;
    if (runPropertiesBaseStyle == null)
    {
      runPropertiesBaseStyle = new DXW.RunPropertiesBaseStyle();
      runPropertiesDefault.RunPropertiesBaseStyle = runPropertiesBaseStyle;
    }
    return new RunPropertiesBridge(runPropertiesBaseStyle);
    ;
  }
}

