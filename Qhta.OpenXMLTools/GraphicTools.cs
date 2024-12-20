﻿namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with graphics
/// </summary>
public static partial class GraphicTools
{

  /// <summary>
  /// Get the anchor id of an inline
  /// </summary>
  /// <param name="inline"></param>
  /// <returns></returns>
  public static string? GetAnchorId (this DXDW.Inline inline)
    => inline.AnchorId?.Value;

  /// <summary>
  /// Set the anchor id of an inline
  /// </summary>
  /// <param name="inline"></param>
  /// <param name="value"></param>
  public static void SetAnchorId(this DXDW.Inline inline, string? value)
    => inline.AnchorId = value;

  /// <summary>
  /// Get the anchor id of an anchor
  /// </summary>
  /// <param name="anchor"></param>
  /// <returns></returns>
  public static string? GetAnchorId(this DXDW.Anchor anchor)
    => anchor.AnchorId?.Value;

  /// <summary>
  /// Set the anchor id of an anchor
  /// </summary>
  /// <param name="anchor"></param>
  /// <param name="value"></param>
  public static void SetAnchorId(this DXDW.Anchor anchor, string? value)
    => anchor.AnchorId = value;

  /// <summary>
  /// Convert an Anchor to an Inline
  /// </summary>
  /// <param name="anchor"></param>
  /// <returns></returns>
  public static DXDW.Inline ConvertAnchorToInline(this DXDW.Anchor anchor)
  {
    var inline = new DXDW.Inline();

    inline.AnchorId = anchor.AnchorId?.Value;
    var anchorDocProperties = anchor.Elements<DXDW.DocProperties>().FirstOrDefault();
    if (anchorDocProperties != null) 
      inline.DocProperties = anchorDocProperties.CloneNode(true) as DXDW.DocProperties;
    var extent = anchor.Elements<DXDW.Extent>().FirstOrDefault();
    if (extent != null)
      inline.Extent = extent.CloneNode(true) as DXDW.Extent;
    var effectExtent = anchor.Elements<DXDW.EffectExtent>().FirstOrDefault();
    if (effectExtent != null)
      inline.EffectExtent = effectExtent.CloneNode(true) as DXDW.EffectExtent;
    var frameProps = anchor.Elements<DXDW.NonVisualGraphicFrameDrawingProperties>().FirstOrDefault();
    if (frameProps != null)
      inline.NonVisualGraphicFrameDrawingProperties = frameProps.CloneNode(true) as DXDW.NonVisualGraphicFrameDrawingProperties;

    var graphic = anchor.Elements<DXD.Graphic>().FirstOrDefault();
    if (graphic != null)
      inline.Graphic = graphic.CloneNode(true) as DXD.Graphic;

    return inline;
  }

  /// <summary>
  /// Convert a floating picture to an inline picture
  /// </summary>
  /// <param name="picture"></param>
  /// <returns>Returns converted picture (or null if not converted)</returns>
  public static DXW.Picture? ConvertFloatingPictureToInlinePicture(this DXW.Picture picture)
  {
    var newPicture = (DXW.Picture)picture.CloneNode(true);
    var firstItem = newPicture.Elements().FirstOrDefault();
    if (firstItem != null)
    {
      if (firstItem is DXV.Group group)
      {
        string? style = group.Style;
        if (style != null && style.Contains(":absolute"))
        {
          var ss = style.Split(';').ToList();
          foreach (var s in ss.ToArray())
          {
            if (s.EndsWith(":absolute"))
            {
              ss.Remove(s);
            }
            else 
            if (s.StartsWith("margin-"))
            {
              ss.Remove(s);
              break;
            }
            else
            if (s.StartsWith("z-index:"))
            {
              ss.Remove(s);
              break;
            }
          }
          ss.Add("mso-position-horizontal-relative:char;mso-position-vertical-relative:line");
          group.Style = string.Join(";", ss);
          return newPicture;
        }
      }
    }
    return null;
  }

}