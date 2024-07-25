using System.Runtime.CompilerServices;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with subdocuments in OpenXml documents.
/// </summary>
public static class SubdocumentTools
{
  /// <summary>
  /// Pair of a subdocument reference and its external relationship.
  /// </summary>
  public struct SubdocRefRelPair
  {
    /// <summary>
    /// Subdocument reference.
    /// </summary>
    public SubDocumentReference Ref;

    /// <summary>
    /// External relationship.
    /// </summary>
    public ExternalRelationship Rel;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="ref"></param>
    /// <param name="rel"></param>
    public SubdocRefRelPair(SubDocumentReference @ref, ExternalRelationship rel)
    {
      Ref = @ref;
      Rel = rel;
    }
  }

  /// <summary>
  /// Gets the external relationships that are subdocuments.
  /// </summary>
  /// <param name="mainDocumentPart"></param>
  /// <returns></returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IEnumerable<ExternalRelationship> GetSubDocumentRelationships(MainDocumentPart mainDocumentPart)
  {
    return mainDocumentPart.ExternalRelationships.Where(rel =>
      rel.IsExternal && (rel.RelationshipType?.EndsWith("/subDocument") ?? false));
  }

  /// <summary>
  /// Gets the subdocument references in the main document part.
  /// </summary>
  /// <param name="mainDocumentPart"></param>
  /// <returns></returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IEnumerable<SubDocumentReference> GetSubDocumentReferences(MainDocumentPart mainDocumentPart)
  {
    return mainDocumentPart.Document.Descendants<SubDocumentReference>();
  }

  /// <summary>
  /// Gets the subdocument references and their external relationships.
  /// </summary>
  /// <param name="mainDocumentPart"></param>
  /// <returns></returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IEnumerable<SubdocRefRelPair> GetSubDocuments(MainDocumentPart mainDocumentPart)
  {
    var rels = GetSubDocumentRelationships(mainDocumentPart);
    var refs = GetSubDocumentReferences(mainDocumentPart);
#pragma warning disable CS8603 // Possible null reference return.
    return refs.Join<SubDocumentReference, ExternalRelationship, string, SubdocRefRelPair>
      (rels, @ref => @ref.Id, rel => rel.Id, (@ref, rel) => new SubdocRefRelPair(@ref, rel));
#pragma warning restore CS8603 // Possible null reference return.
  }

}