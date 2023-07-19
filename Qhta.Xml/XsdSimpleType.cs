namespace Qhta.Xml;

/// <summary>
/// Defines Xsd simple types (with their XML tags).
/// </summary>
public enum XsdSimpleType
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  [XmlEnum("anyURI")] AnyUri = 1,
  [XmlEnum("base64Binary")] Base64Binary,
  [XmlEnum("boolean")] Boolean,
  [XmlEnum("byte")] Byte,
  [XmlEnum("date")] Date,
  [XmlEnum("dateTime")] DateTime,
  [XmlEnum("decimal")] Decimal,
  [XmlEnum("double")] Double,
  [XmlEnum("duration")] Duration,
  [XmlEnum("ENTITIES")] Entities,
  [XmlEnum("ENTITY")] Entity,
  [XmlEnum("float")] Float,
  [XmlEnum("gDay")] GDay,
  [XmlEnum("gMonth")] GMonth,
  [XmlEnum("gMonthDay")] GMonthDay,
  [XmlEnum("gYear")] GYear,
  [XmlEnum("gYearMonth")] GYearMonth,
  [XmlEnum("hexBinary")] HexBinary,
  [XmlEnum("ID")] Id,
  [XmlEnum("IDREF")] IdRef,
  [XmlEnum("IDREFS")] IdRefs,
  [XmlEnum("int")] Int,
  [XmlEnum("integer")] Integer,
  [XmlEnum("language")] Language,
  [XmlEnum("long")] Long,
  [XmlEnum("Name")] Name,
  [XmlEnum("NCName")] NcName,
  [XmlEnum("negativeInteger")] NegativeInteger,
  [XmlEnum("NMTOKEN")] NmToken,
  [XmlEnum("NMTOKENS")] NmTokens,
  [XmlEnum("nonNegativeInteger")] NonNegativeInteger,
  [XmlEnum("nonPositiveInteger")] NonPositiveInteger,
  [XmlEnum("normalizedString")] NormalizedString,
  [XmlEnum("NOTATION")] Notation,
  [XmlEnum("positiveInteger")] PositiveInteger,
  [XmlEnum("QName")] QName,
  [XmlEnum("short")] Short,
  [XmlEnum("string")] String,
  [XmlEnum("time")] Time,
  [XmlEnum("token")] Token,
  [XmlEnum("unsignedByte")] UnsignedByte,
  [XmlEnum("unsignedInt")] UnsignedInt,
  [XmlEnum("unsignedLong")] UnsignedLong,
  [XmlEnum("unsignedShort")] UnsignedShort
}