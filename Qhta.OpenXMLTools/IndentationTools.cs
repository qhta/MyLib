namespace Qhta.OpenXmlTools;

/// <summary>
/// Contains tools for working with indentation elements.
/// </summary>
public static class IndentationTools
{
  /// <summary>
  /// Get the Left indentation value.
  /// </summary>
  /// <param name="indentation"></param>
  /// <returns></returns>
  public static int? GetLeft(this DXW.Indentation indentation)
  {
    if (indentation.Left != null)
      if (int.TryParse(indentation.Left.Value, out var result))
        return result;
    return null;
  }

  /// <summary>
  /// Set the Left indentation value.
  /// </summary>
  /// <param name="indentation"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetLeft(this DXW.Indentation indentation, int? value)
  {
    indentation.Left = value?.ToString();
  }

  /// <summary>
  /// Get the Start indentation value.
  /// </summary>
  /// <param name="indentation"></param>
  /// <returns></returns>
  public static int? GetStart(this DXW.Indentation indentation)
  {
    if (indentation.Start != null)
      if (int.TryParse(indentation.Start.Value, out var result))
        return result;
    return null;
  }

  /// <summary>
  /// Set the Start indentation value.
  /// </summary>
  /// <param name="indentation"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetStart(this DXW.Indentation indentation, int? value)
  {
    indentation.Start = value?.ToString();
  }

  /// <summary>
  /// Get the Right indentation value.
  /// </summary>
  /// <param name="indentation"></param>
  /// <returns></returns>
  public static int? GetRight(this DXW.Indentation indentation)
  {
    if (indentation.Right != null)
      if (int.TryParse(indentation.Right.Value, out var result))
        return result;
    return null;
  }

  /// <summary>
  /// Set the Right indentation value.
  /// </summary>
  /// <param name="indentation"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetRight(this DXW.Indentation indentation, int? value)
  {
    indentation.Right = value?.ToString();
  }


  /// <summary>
  /// Get the End indentation value.
  /// </summary>
  /// <param name="indentation"></param>
  /// <returns></returns>
  public static int? GetEnd(this DXW.Indentation indentation)
  {
    if (indentation.End != null)
      if (int.TryParse(indentation.End.Value, out var result))
        return result;
    return null;
  }

  /// <summary>
  /// Set the End indentation value.
  /// </summary>
  /// <param name="indentation"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetEnd(this DXW.Indentation indentation, int? value)
  {
    indentation.End = value?.ToString();
  }

  /// <summary>
  /// Get the FirstLine indentation value.
  /// </summary>
  /// <param name="indentation"></param>
  /// <returns></returns>
  public static int? GetFirstLine(this DXW.Indentation indentation)
  {
    if (indentation.FirstLine != null)
      if (int.TryParse(indentation.FirstLine.Value, out var result))
        return result;
    return null;
  }

  /// <summary>
  /// Set the FirstLine indentation value.
  /// </summary>
  /// <param name="indentation"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetFirstLine(this DXW.Indentation indentation, int? value)
  {
    indentation.FirstLine = value?.ToString();
  }

  /// <summary>
  /// Get the Hanging indentation value.
  /// </summary>
  /// <param name="indentation"></param>
  /// <returns></returns>
  public static int? GetHanging(this DXW.Indentation indentation)
  {
    if (indentation.Hanging != null)
      if (int.TryParse(indentation.Hanging.Value, out var result))
        return result;
    return null;
  }

  /// <summary>
  /// Set the Hanging indentation value.
  /// </summary>
  /// <param name="indentation"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetHanging(this DXW.Indentation indentation, int? value)
  {
    indentation.Hanging = value?.ToString();
  }
}