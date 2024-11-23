namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for drawing elements.
/// </summary>
public static class DrawingTools
{
  /// <summary>
  /// Get the text for the drawing element.
  /// </summary>
  /// <param name="drawing"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this DXW.Drawing drawing, TextOptions options)
  {
    var sl = new List<string>();
    if (options.IncludeDrawings)
    {
      if (options.IgnoreDrawingContents)
      {
        sl.Add(options.DrawingSubstituteTag);
      }
      else
      {
        if (options.UseIndenting)
        {
          sl.Add(options.NewLine);
          sl.Add(options.GetIndent());
        }
        sl.Add(options.DrawingStartTag);
        options.IndentLevel++;
        if (options.UseIndenting)
        {
          sl.Add(options.NewLine);
        }
        sl.Add((drawing as DX.OpenXmlCompositeElement).GetText(options));
        options.IndentLevel--;
        if (options.UseIndenting)
        {
          sl.Add(options.NewLine);
          sl.Add(options.GetIndent());
        }
        sl.Add(options.DrawingEndTag);
      }
    }
    return string.Join("", sl);
  }
}