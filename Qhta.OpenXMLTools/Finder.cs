namespace Qhta.OpenXmlTools;

/// <summary>
/// Class to find (and replace) text in a document.
/// </summary>
public class Finder
{

  /// <summary>
  /// Construct a finder.
  /// </summary>
  /// <param name="context"></param>
  public Finder(Range context)
  {
    Context = context;
  }

  /// <summary>
  /// Context for the search.
  /// </summary>
  public Range Context { get; set; }

  /// <summary>
  /// Text to search for.
  /// </summary>
  public string? Text { get; set; }

  /// <summary>
  /// Determines if the search must consider formatting.
  /// </summary>
  public bool Format { get; set; }

  /// <summary>
  /// Text to replace.
  /// </summary>
  public string? Replacement { get; set; }

  /// <summary>
  /// Realize the search and replace.
  /// </summary>
  /// <returns>last found range (or null if not found)</returns>
  public Range? Execute()
  {
    if (Format)
    {
      var text = Context.GetText(TextOptions.PlainText);
    }
    else
    {
      var text = Context.GetText(TextOptions.PlainText);
    }
    return null;
  }
}