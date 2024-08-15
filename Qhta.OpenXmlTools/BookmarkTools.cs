using System;
using System.Collections.Generic;
using System.Text;
using DocumentFormat.OpenXml.Office.CoverPageProps;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with bookmarks
/// </summary>
public static class BookmarkTools
{
  /// <summary>
  /// Gets the bookmark start element that corresponds to the specified bookmark end element
  /// </summary>
  /// <param name="bookmarkEnd"></param>
  /// <param name="searchFromElement">searches from this element backward</param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"></exception>
  /// <exception cref="ArgumentException"></exception>
  public static DXW.BookmarkStart? GetBookmarkStart(this DXW.BookmarkEnd bookmarkEnd, DX.OpenXmlElement? searchFromElement = null)
  {
    if (bookmarkEnd == null)
      throw new ArgumentNullException(nameof(bookmarkEnd));
    string? Id = bookmarkEnd.Id?.Value;
    if (Id == null)
      return null;
    if (searchFromElement == null)
      searchFromElement = bookmarkEnd;
    var element = searchFromElement.PreviousSibling();
    while (element!=null)
    {
      if (element is DXW.BookmarkStart bookmarkStart && bookmarkStart.Id?.Value == Id)
        return bookmarkStart;
      element = element.PreviousSibling();
    }
    if (searchFromElement.Parent == null)
      return null;
    return GetBookmarkStart(bookmarkEnd, searchFromElement.Parent);

  }
}
