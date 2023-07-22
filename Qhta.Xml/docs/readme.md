This package contains interfaces definint XmlWriter & XmlWriter and attributes used to annotate types and properties for Qhta.Xml.Serializer package.

Interfaces:
* IXmlReader - defines methods that must be implemented in an xml reader. Mostly taken from System.Xml.XmlReader.
* IXmlWriter - defines methods that must be implemented in an xml writer. Mostly taken from System.Xml.XmlWriter.
* IXmlSerializer - defines methods that must be implemented in an xml serializer. Mostly taken from System.Xml.Serialization.Serializer.
* IXmlConverter - interface for a converter to read/write Xml.
* IRealTypeConverter - interface for a type converter with unit property.


Attribute classes:
* SerializationOrderAttribute - defines an attribute which can specify missing order number.
* XmlCollectionAttribute - defines an attribute which can specify data of collection property, field or class that are needed for serialization/deserialization.
* XmlContentElementAttribute - defines an attribute which can specify that a property or field is serialized without preceding xml tag.
* XmlContentPropertyAttribute - this is a replacement for "System.Windows.Markup.ContentPropertyAttribute" which is allowed only for .NET Framework, but not for .NET Core.
* XmlConverterAttribute - defines an attribute which can specify a type converter for a class, property or field.
* XmlDataFormatAttribute - defines an attribute which can specify "Format" and "Culture" for a property or field.
* XmlDictionaryAttribute - defines an attribute which can specify data of dictionary property, field or class that are needed for serialization/deserialization. Extends XmlCollectionAttribute.
* XmlDictionaryItemAttribute - defines an attribute which can specify item data of dictionary property, field or class that are needed for serialization/deserialization. There may be multiple such attributes declared for a single dictionary.
* XmlItemElementAttribute - defines an attribute which can specify data needed to serialize/deserialize class/interface/struct items. There may be multiple such attributes declared for a single container.
* XmlNameAttribute - defines an attribute which can specify a name recognized in a class/interface/struct. There may be multiple such attributes declared for a single type.
* XmlObjectAttribute - defines an attribute which specifies that the class instance must be serialized as full object - not as a simple collection.
* XmlReferenceAttribute - defines an attribute which specifies than a property (or field) represents a reference.
* XmlReferencesAttribute - defines an attribute which specifies than a property (or class) represents collection of references.

Helper class:
* XmlQualifiedTagName - declares an Xml Qualified Tag Name. Replaces System.Xml.XmlQualifiedName.

Helper enum type:
* XsdSimpleType - defines Xsd simple types (with their XML tags).


