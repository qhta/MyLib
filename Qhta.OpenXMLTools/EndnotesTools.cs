using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXMLTools;

public static class EndnoteTools
{

  public static void Init(this Endnotes endnotes)
  {
    var item_1 = new DXW.Endnote();
    item_1.Id=-1;
    item_1.Type = FootnoteEndnoteValues.Separator;
    var p_1 = new DXW.Paragraph();
    item_1.AppendChild(p_1);
    var r_1 = new DXW.Run();
    p_1.AppendChild(r_1);
    var s_1 = new DXW.SeparatorMark();
    r_1.AppendChild(s_1);
    endnotes.AppendChild(item_1);

    var item0 = new DXW.Endnote();
    item0.Id=0;
    item0.Type = FootnoteEndnoteValues.ContinuationSeparator;
    var p0 = new DXW.Paragraph();
    item0.AppendChild(p0);
    var r0 = new DXW.Run();
    p0.AppendChild(r0);
    var s0 = new DXW.ContinuationSeparatorMark();
    r0.AppendChild(s0);
    endnotes.AppendChild(item0);
  }
}