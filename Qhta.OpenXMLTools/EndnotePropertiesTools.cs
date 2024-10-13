namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for EndnoteProperties element.
/// </summary>
public static class EndnotePropertiesTools
{
  /// <summary>
  /// Gets the EndnotePosition element value.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static DXW.EndnotePositionValues? GetEndnotePosition(this DXW.EndnoteProperties element)
  {
    return element.GetFirstElement<DXW.EndnotePosition>()?.Val?.Value;
  }

  /// <summary>
  /// Sets the EndnotePosition element value.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="value"></param>
  public static void SetEndnotePosition(this DXW.EndnoteProperties element, DXW.EndnotePositionValues? value)
  {
    if (value == null)
      element.RemoveAllChildren<DXW.EndnotePosition>();
    else
    {
      var EndnotePosition = element.GetFirstElement<DXW.EndnotePosition>();
      if (EndnotePosition == null)
      {
        EndnotePosition = new DXW.EndnotePosition();
        element.AppendChild(EndnotePosition);
      }
      EndnotePosition.Val = value;
    }
  }

  /// <summary>
  /// Gets the first NumberingFormat element value
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static DXW.NumberingFormat? GetNumberingFormat(this DXW.EndnoteProperties element)
  {
    return element.GetFirstElement<DXW.NumberingFormat>();
  }

  /// <summary>
  /// Sets the NumberingFormat element value.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="value"></param>
  public static void SetNumberingFormat(this DXW.EndnoteProperties element, DXW.NumberingFormat? value)
  {
    element.RemoveAllChildren<DXW.NumberingFormat>();
    if (value == null)
      element.AppendChild(value);
  }

  /// <summary>
  /// Gets the NumberingStart element value.
  /// </summary>
  public static int? GetNumberingStart(this DXW.EndnoteProperties element)
  {
    return element.GetFirstElement<DXW.NumberingStart>()?.Val?.Value;
  }

  /// <summary>
  /// Sets the NumberingStart element value.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="value"></param>
  public static void SetNumberingStart(this DXW.EndnoteProperties element, int? value)
  {
    if (value == null)
      element.RemoveAllChildren<DXW.NumberingStart>();
    else
    {
      var numberingStart = element.GetFirstElement<DXW.NumberingStart>();
      if (numberingStart == null)
      {
        numberingStart = new DXW.NumberingStart();
        element.AppendChild(numberingStart);
      }
      numberingStart.Val = new DX.UInt16Value((ushort)value);
    }
  }

  /// <summary>
  /// Gets the NumberingRestart element value.
  /// </summary>
  public static DXW.RestartNumberValues? GetNumberingRestart(this DXW.EndnoteProperties element)
  {
    return element.GetFirstElement<DXW.NumberingRestart>()?.Val?.Value;
  }

  /// <summary>
  /// Sets the NumberingRestart element value.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="value"></param>
  public static void SetNumberingRestart(this DXW.EndnoteProperties element, DXW.RestartNumberValues? value)
  {
    if (value == null)
      element.RemoveAllChildren<DXW.NumberingRestart>();
    else
    {
      var numberingRestart = element.GetFirstElement<DXW.NumberingRestart>();
      if (numberingRestart == null)
      {
        numberingRestart = new DXW.NumberingRestart();
        element.AppendChild(numberingRestart);
      }
      numberingRestart.Val = value;
    }
  }
}