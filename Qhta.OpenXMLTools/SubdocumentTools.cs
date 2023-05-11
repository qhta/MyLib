using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXMLTools;

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
  public static IEnumerable<ExternalRelationship> GetSubDocumentRelationships(MainDocumentPart mainDocumentPart)
  {
    return mainDocumentPart.ExternalRelationships.Where(rel =>
      rel.IsExternal && (rel.RelationshipType?.EndsWith("/subDocument") ?? false));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IEnumerable<SubDocumentReference> GetSubDocumentReferences(MainDocumentPart mainDocumentPart)
  {
    return mainDocumentPart.Document.Descendants<SubDocumentReference>();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IEnumerable<SubdocRefRelPair> GetSubDocuments(MainDocumentPart mainDocumentPart)
  {
    var rels = GetSubDocumentRelationships(mainDocumentPart);
    var refs = GetSubDocumentReferences(mainDocumentPart);
#pragma warning disable CS8603 // Possible null reference return.
    return Enumerable.Join<SubDocumentReference, ExternalRelationship, string, SubdocRefRelPair>
      (refs, rels, @ref => @ref.Id, rel => rel.Id, (@ref, rel) => new SubdocRefRelPair(@ref, rel));
#pragma warning restore CS8603 // Possible null reference return.
  }

}