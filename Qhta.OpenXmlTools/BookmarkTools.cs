using System;

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
    var element = searchFromElement.PreviousSiblingMember();
    while (element!=null)
    {
      if (element is DXW.BookmarkStart bookmarkStart && bookmarkStart.Id?.Value == Id)
        return bookmarkStart;
      element = element.PreviousSiblingMember();
    }
    if (searchFromElement.Parent == null)
      return null;
    return GetBookmarkStart(bookmarkEnd, searchFromElement.Parent);
  }

  /// <summary>
  /// Gets the bookmark end element that corresponds to the specified bookmark start element
  /// </summary>
  /// <param name="bookmarkStart"></param>
  /// <param name="searchFromElement">searches from this element backward</param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"></exception>
  /// <exception cref="ArgumentException"></exception>
  public static DXW.BookmarkEnd? GetBookmarkEnd(this DXW.BookmarkStart bookmarkStart, DX.OpenXmlElement? searchFromElement = null)
  {
    if (bookmarkStart == null)
      throw new ArgumentNullException(nameof(bookmarkStart));
    string? Id = bookmarkStart.Id?.Value;
    if (Id == null)
      return null;
    if (searchFromElement == null)
      searchFromElement = bookmarkStart;
    var element = searchFromElement.PreviousSiblingMember();
    while (element != null)
    {
      if (element is DXW.BookmarkEnd bookmarkEnd && bookmarkStart.Id?.Value == Id)
        return bookmarkEnd;
      element = element.PreviousSiblingMember();
    }
    if (searchFromElement.Parent == null)
      return null;
    return GetBookmarkEnd(bookmarkStart, searchFromElement.Parent);
  }
}
