namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for FootnoteProperties element.
/// </summary>
public static class FootnotePropertiesTools
{
  /// <param name="element"></param>
  extension(DXW.FootnoteProperties element)
  {
    /// <summary>
    /// Gets the FootnotePosition element value.
    /// </summary>
    /// <returns></returns>
    public DXW.FootnotePositionValues? GetFootnotePosition()
    {
      return element.GetFirstElement<DXW.FootnotePosition>()?.Val?.Value;
    }

    /// <summary>
    /// Sets the FootnotePosition element value.
    /// </summary>
    /// <param name="value"></param>
    public void SetFootnotePosition(DXW.FootnotePositionValues? value)
    {
      if (value == null)
        element.RemoveAllChildren<DXW.FootnotePosition>();
      else
      {
        var footnotePosition = element.GetFirstElement<DXW.FootnotePosition>();
        if (footnotePosition == null)
        {
          footnotePosition = new DXW.FootnotePosition();
          element.AppendChild(footnotePosition);
        }
        footnotePosition.Val = value;
      }
    }

    /// <summary>
    /// Gets the first NumberingFormat element value
    /// </summary>
    /// <returns></returns>
    public DXW.NumberingFormat? GetNumberingFormat()
    {
      return element.GetFirstElement<DXW.NumberingFormat>();
    }

    /// <summary>
    /// Sets the NumberingFormat element value.
    /// </summary>
    /// <param name="value"></param>
    public void SetNumberingFormat(DXW.NumberingFormat? value)
    {
      element.RemoveAllChildren<DXW.NumberingFormat>();
      if (value == null)
        element.AppendChild(value);
    }

    /// <summary>
    /// Gets the NumberingStart element value.
    /// </summary>
    public int? GetNumberingStart()
    {
      return element.GetFirstElement<DXW.NumberingStart>()?.Val?.Value;
    }

    /// <summary>
    /// Sets the NumberingStart element value.
    /// </summary>
    /// <param name="value"></param>
    public void SetNumberingStart(int? value)
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
    public DXW.RestartNumberValues? GetNumberingRestart()
    {
      return element.GetFirstElement<DXW.NumberingRestart>()?.Val?.Value;
    }

    /// <summary>
    /// Sets the NumberingRestart element value.
    /// </summary>
    /// <param name="value"></param>
    public void SetNumberingRestart(DXW.RestartNumberValues? value)
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
}