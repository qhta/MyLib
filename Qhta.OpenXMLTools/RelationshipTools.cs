using System;
using System.Collections.Generic;
using System.Text;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Contains tools for working with OpenXml Relationship elements.
/// </summary>
public static class RelationshipTools
{
  /// <summary>
  /// Checks if the relationship is equal to another relationship.
  /// </summary>
  /// <param name="thisRelationship"></param>
  /// <param name="otherRelationship"></param>
  /// <returns></returns>
  public static bool IsEqual(this DXPack.ReferenceRelationship? thisRelationship, DXPack.ReferenceRelationship? otherRelationship)
  {
    if (thisRelationship == null && otherRelationship == null)
      return true;
    if (thisRelationship == null || otherRelationship == null)
      return false;
    if (thisRelationship.RelationshipType != otherRelationship.RelationshipType)
      return false;
    if (thisRelationship.Uri != otherRelationship.Uri)
      return false;
    if (thisRelationship.IsExternal != otherRelationship.IsExternal)
      return false;
    return true;
  }
}
