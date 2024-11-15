using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// A collection of tools for working with <c>Numbering</c> element.
/// </summary>
public static class NumberingTools
{
  /// <summary>
  /// Get the new abstract number ID unique of all the abstract numbering elements.
  /// </summary>
  /// <param name="numbering">Element to examine</param>
  /// <returns></returns>
  public static int GetNewAbstractNumId(this DXW.Numbering numbering)
  {
    return numbering.Elements<AbstractNum>().Max(e => e.GetAbstractNumId()) ?? -1 + 1;
  }

  /// <summary>
  /// Get or create bulleted abstract numbering.
  /// </summary>
  /// <param name="numbering">Element to process</param>
  /// <returns></returns>
  public static DXW.AbstractNum GetDefaultBulletedAbstractNumbering(this DXW.Numbering numbering)
  {
    var abstractNum = numbering.Elements<AbstractNum>().FirstOrDefault(a => a.IsBulleted());
    if (abstractNum == null)
    {
      var newNumId = numbering.GetNewAbstractNumId(); 
      abstractNum = new AbstractNum { AbstractNumberId = newNumId };
      abstractNum.SetMultiLevelType(DXW.MultiLevelValues.HybridMultilevel);
      foreach (var symbol in new[] { "•", "◦", "-", "·", "▪", "▫", "▪", "▫", "▪", "▫" })
      {
        var level = new Level
        {
          LevelIndex = abstractNum.Elements<Level>().Count(),
          NumberingFormat = new NumberingFormat { Val = NumberFormatValues.Bullet },
          LevelText = new LevelText { Val = symbol }
        };
        abstractNum.Append(level);
      }
      numbering.Append(abstractNum);
    }
    return abstractNum;
  }

  /// <summary>
  /// Get the new numbering instance ID unique of all the numbering instance elements.
  /// </summary>
  /// <param name="numbering">Element to examine</param>
  /// <returns></returns>
  public static int GetNewNumberingInstanceId(this DXW.Numbering numbering)
  {
    return numbering.Elements<NumberingInstance>().Max(e => e.GetNumberingId()) ?? -1 + 1;
  }

  /// <summary>
  /// Get or create numbering instance for an abstract numbering.
  /// </summary>
  /// <param name="numbering">Element to process</param>
  /// <param name="abstractNumId">Abstract numbering id</param>
  /// <returns></returns>
  public static DXW.NumberingInstance GetNumberingInstance(this DXW.Numbering numbering, int abstractNumId)
  {
    var numberingInstance = numbering.Elements<NumberingInstance>().FirstOrDefault(e => e.GetAbstractNumId() == abstractNumId);
    if (numberingInstance == null)
    {
      var newNumId = numbering.GetNewNumberingInstanceId();
      numberingInstance = new NumberingInstance
      {
        NumberID = newNumId,
      };
      numberingInstance.SetAbstractNumId(abstractNumId);
      numbering.Append(numberingInstance);
    }
    return numberingInstance;
  }
}