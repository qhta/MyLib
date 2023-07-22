This package contains my implementation of XmlSerializer which is more flexible than system XmlSerializer.

The package contains the following classes:
* QXmlSerializer - flexible Xml serializer.
* QXmlReader - wrapper for system XmlReader used by QXmlSerializer.
* QXmlWriter - wrapper for system XmlWriter used by QXmlSerializer.
* XmlConverter - Xml equivalent of JsonConverter. Reads and writes object from/to XML.
* XmlSerializationInfoMapper - The purpose of this class is to build serialization info on types and properties and map xml element or attribute names to this info.
* XmlInternalException - class extending system Exception to hold LineNumber and LinePosition.
* SerializationOptions - class containing serialization options as separate boolean values.

Enum type:
* SerializationCase - specifies modes for name case change.
