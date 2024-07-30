namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with <c>Rsids</c> of a document.
/// </summary>
public static class RsidsTools
{

  /// <summary>
  /// Get all the <c>Rsids</c> as an array of <see cref="HexInt"/> values (including <c>RsidRoot</c>).
  /// </summary>
  /// <param name="rsids"></param>
  /// <returns></returns>
  public static HexInt[] ToArray(this DXW.Rsids rsids)
  {
    var sl = new List<HexInt>();
    foreach (var element in rsids.Elements())
    {
      if (element is DXW.RsidRoot rsidroot)
        sl.Add(new HexInt(rsidroot.Val?.Value!));
      else if (element is DXW.Rsid rsid)
        sl.Add(new HexInt(rsid.Val?.Value!));
    }
    return sl.ToArray();
  }
}
