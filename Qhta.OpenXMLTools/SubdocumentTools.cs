using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXMLTools
{
  public static class SubdocumentTools
  {
    public struct SubdocRefRelPair
    {
      public SubDocumentReference Ref;
      public ExternalRelationship Rel;

      public SubdocRefRelPair(SubDocumentReference @ref, ExternalRelationship rel)
      {
        Ref = @ref;
        Rel = rel;
      }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<ExternalRelationship> GetSubDocumentRelationships(WordprocessingDocument docx)
    {
      return docx.MainDocumentPart.ExternalRelationships.Where(rel =>
      rel.IsExternal && (rel.RelationshipType?.EndsWith("/subDocument") ?? false));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<SubDocumentReference> GetSubDocumentReferences(WordprocessingDocument docx)
    {
      return docx.MainDocumentPart.Document.Descendants<SubDocumentReference>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<SubdocRefRelPair> GetSubDocuments(WordprocessingDocument docx)
    {
      var rels = GetSubDocumentRelationships(docx);
      var refs = GetSubDocumentReferences(docx);
      return Enumerable.Join<SubDocumentReference, ExternalRelationship, string, SubdocRefRelPair>
        (refs, rels, @ref => @ref.Id, rel => rel.Id, (@ref, rel) => new SubdocRefRelPair(@ref, rel));
    }

  }
}
