using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// A composite tool for cleaning a Wordprocessing document.
/// </summary>
public partial class DocumentCleaner
{

  /// <summary>
  /// Detect paragraphs that contain a bullet and enter a new paragraph with bullet numbering.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void FixParagraphNumbering(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nRepairing paragraphs numbering");
    var bulleted = wordDoc.GetBody().FixParagraphsWithBullets();
    var numbered = wordDoc.GetBody().FixParagraphsWithNumbers();

    if (VerboseLevel > 0)
    {
      Console.WriteLine($"  {bulleted} paragraphs with bullet symbol fixed");
      Console.WriteLine($"  {numbered} paragraphs with numbering fixed");

    }
  }

}
