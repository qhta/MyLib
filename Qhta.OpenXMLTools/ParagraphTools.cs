using System;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Office2013.Word;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2010.Word;
using ContentPart = DocumentFormat.OpenXml.Wordprocessing.ContentPart;

namespace OpenXMLTools
{
  [ChildElementInfo(typeof(ParagraphProperties))]
  [ChildElementInfo(typeof(ContentPart), FileFormatVersions.Office2010)]
  [ChildElementInfo(typeof(SubDocumentReference))]
  [ChildElementInfo(typeof(Run))]
  [ChildElementInfo(typeof(CustomXmlRun))]
  [ChildElementInfo(typeof(SimpleField))]
  [ChildElementInfo(typeof(Hyperlink))]
  [ChildElementInfo(typeof(SdtRun))]
  [ChildElementInfo(typeof(ProofError))]
  [ChildElementInfo(typeof(PermStart))]
  [ChildElementInfo(typeof(PermEnd))]
  [ChildElementInfo(typeof(BookmarkStart))]
  [ChildElementInfo(typeof(BookmarkEnd))]
  [ChildElementInfo(typeof(CommentRangeStart))]
  [ChildElementInfo(typeof(CommentRangeEnd))]
  [ChildElementInfo(typeof(MoveFromRangeStart))]
  [ChildElementInfo(typeof(MoveFromRangeEnd))]
  [ChildElementInfo(typeof(MoveToRangeStart))]
  [ChildElementInfo(typeof(MoveToRangeEnd))]
  [ChildElementInfo(typeof(InsertedRun))]
  [ChildElementInfo(typeof(DeletedRun))]
  [ChildElementInfo(typeof(MoveFromRun))]
  [ChildElementInfo(typeof(MoveToRun))]
  [ChildElementInfo(typeof(RunConflictInsertion), FileFormatVersions.Office2010 | FileFormatVersions.Office2013)]
  [ChildElementInfo(typeof(RunConflictDeletion), FileFormatVersions.Office2010 | FileFormatVersions.Office2013)]
  [ChildElementInfo(typeof(BidirectionalOverride), FileFormatVersions.Office2010)]
  [ChildElementInfo(typeof(BidirectionalEmbedding), FileFormatVersions.Office2010)]
  [ChildElementInfo(typeof(CustomXmlInsRangeStart))]
  [ChildElementInfo(typeof(CustomXmlInsRangeEnd))]
  [ChildElementInfo(typeof(CustomXmlDelRangeStart))]
  [ChildElementInfo(typeof(CustomXmlDelRangeEnd))]
  [ChildElementInfo(typeof(CustomXmlMoveFromRangeStart))]
  [ChildElementInfo(typeof(CustomXmlMoveFromRangeEnd))]
  [ChildElementInfo(typeof(CustomXmlMoveToRangeStart))]
  [ChildElementInfo(typeof(CustomXmlMoveToRangeEnd))]
  [ChildElementInfo(typeof(CustomXmlConflictInsertionRangeStart), FileFormatVersions.Office2010 | FileFormatVersions.Office2013)]
  [ChildElementInfo(typeof(CustomXmlConflictInsertionRangeEnd), FileFormatVersions.Office2010 | FileFormatVersions.Office2013)]
  [ChildElementInfo(typeof(CustomXmlConflictDeletionRangeStart), FileFormatVersions.Office2010 | FileFormatVersions.Office2013)]
  [ChildElementInfo(typeof(CustomXmlConflictDeletionRangeEnd), FileFormatVersions.Office2010 | FileFormatVersions.Office2013)]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.Paragraph))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.OfficeMath))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.Accent))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.Bar))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.Box))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.BorderBox))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.Delimiter))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.EquationArray))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.Fraction))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.MathFunction))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.GroupChar))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.LimitUpper))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.LimitLower))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.Matrix))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.Nary))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.Phantom))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.Radical))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.PreSubSuper))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.Subscript))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.SubSuperscript))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.Superscript))]
  [ChildElementInfo(typeof(DocumentFormat.OpenXml.Math.Run))]
  public static class ParagraphTools
  {



    public static string GetText(this Paragraph paragraph)
    {
      return String.Join("", paragraph.Elements<Run>().Select(item => item.GetText()));
    }
  }
}
