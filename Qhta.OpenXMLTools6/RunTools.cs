using System;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Office2013.Word;


namespace Qhta.OpenXMLTools
{
  public static class RunTools
  {
    //public static AnnotationReferenceMark GetAnnotationReferenceMark (this Run run) { return run.Elements<AnnotationReferenceMark>().FirstOrDefault();} 
    //public static FootnoteReferenceMark GetFootnoteReferenceMark (this Run run) { return run.Elements<FootnoteReferenceMark>().FirstOrDefault();} 
    //public static EndnoteReferenceMark GetEndnoteReferenceMark (this Run run) { return run.Elements<EndnoteReferenceMark>().FirstOrDefault();} 
    //public static LastRenderedPageBreak GetLastRenderedPageBreak (this Run run) { return run.Elements<LastRenderedPageBreak>().FirstOrDefault();} 
    //public static Break GetBreak (this Run run) { return run.Elements<Break>().FirstOrDefault();} 
    //public static Text GetText (this Run run) { return run.Elements<Text>().FirstOrDefault();} 
    //public static DeletedText GetDeletedText (this Run run) { return run.Elements<DeletedText>().FirstOrDefault();} 
    //public static FieldCode GetFieldCode (this Run run) { return run.Elements<FieldCode>().FirstOrDefault();} 
    //public static DeletedFieldCode GetDeletedFieldCode (this Run run) { return run.Elements<DeletedFieldCode>().FirstOrDefault();} 
    //public static NoBreakHyphen GetNoBreakHyphen (this Run run) { return run.Elements<NoBreakHyphen>().FirstOrDefault();} 
    //public static SoftHyphen GetSoftHyphen (this Run run) { return run.Elements<SoftHyphen>().FirstOrDefault();} 
    //public static DayShort GetDayShort (this Run run) { return run.Elements<DayShort>().FirstOrDefault();} 
    //public static MonthShort GetMonthShort (this Run run) { return run.Elements<MonthShort>().FirstOrDefault();} 
    //public static YearShort GetYearShort (this Run run) { return run.Elements<YearShort>().FirstOrDefault();} 
    //public static DayLong GetDayLong (this Run run) { return run.Elements<DayLong>().FirstOrDefault();} 
    //public static MonthLong GetMonthLong (this Run run) { return run.Elements<MonthLong>().FirstOrDefault();} 
    //public static YearLong GetYearLong (this Run run) { return run.Elements<YearLong>().FirstOrDefault();} 
    //public static EmbeddedObject GetEmbeddedObject (this Run run) { return run.Elements<EmbeddedObject>().FirstOrDefault();} 
    //public static PositionalTab GetPositionalTab (this Run run) { return run.Elements<PositionalTab>().FirstOrDefault();} 
    //public static SeparatorMark GetSeparatorMark (this Run run) { return run.Elements<SeparatorMark>().FirstOrDefault();} 
    //public static ContinuationSeparatorMark GetContinuationSeparatorMark (this Run run) { return run.Elements<ContinuationSeparatorMark>().FirstOrDefault();} 
    //public static SymbolChar GetSymbolChar (this Run run) { return run.Elements<SymbolChar>().FirstOrDefault();} 
    //public static PageNumber GetPageNumber (this Run run) { return run.Elements<PageNumber>().FirstOrDefault();} 
    //public static CarriageReturn GetCarriageReturn (this Run run) { return run.Elements<CarriageReturn>().FirstOrDefault();} 
    //public static TabChar GetTabChar (this Run run) { return run.Elements<TabChar>().FirstOrDefault();} 
    //public static RunProperties GetRunProperties (this Run run) { return run.Elements<RunProperties>().FirstOrDefault();} 
    //public static Picture GetPicture (this Run run) { return run.Elements<Picture>().FirstOrDefault();} 
    //public static FieldChar GetFieldChar (this Run run) { return run.Elements<FieldChar>().FirstOrDefault();} 
    //public static Ruby GetRuby (this Run run) { return run.Elements<Ruby>().FirstOrDefault();} 
    //public static FootnoteReference GetFootnoteReference (this Run run) { return run.Elements<FootnoteReference>().FirstOrDefault();} 
    //public static EndnoteReference GetEndnoteReference (this Run run) { return run.Elements<EndnoteReference>().FirstOrDefault();} 
    //public static CommentReference GetCommentReference (this Run run) { return run.Elements<CommentReference>().FirstOrDefault();} 
    //public static Drawing GetDrawing (this Run run) { return run.Elements<Drawing>().FirstOrDefault();} 

    public static string GetText(this Run run)
    {
      return String.Join("", run.Elements<TextType>().Select(item => item.Text));
    }
  }
}
