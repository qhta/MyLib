This package contains type converters (derived from the TypeConverter class) that extend the set of standard type converters 
with flexible formatting capabilities.

Standard TypeConverter has no destination type parameter in ConvertFrom method. 
There is an assumption that the type of the result of the ConvertFrom method is determined by converter class.
However it causes a problem if one converter handles many value types. 
To handle this problem, a BaseTypeConverter class is defined with several properties common to other converter classes.
These common properties are:

* ExpectedType - Type expected in ConvertFrom method
* KnownTypes - Types known in ConvertFrom method
* KnownNamespaces - Known namespace prefixes
* XsdType - XsdSimpleType to use when converting to string in ConvertTo
* Format - Format to use when converting to/from string in ConvertTo/ConvertFrom
* Culture - CultureInfo to use when converting to/from string in ConvertTo/ConvertFrom

Basing on the BaseTypeConverters some new converters are defined:

* AnyUriTypeConverter - Standard UriTypeConverter converts Uri type to String.
 This converter just gives the original Uri string without any conversion.
 On its basis, the AnyUriTypeConverter converter was defined, which implements the ITypeConverter interface
 and returns null when reverse conversion is performed for the empty string.


* ArrayTypeConverter - ArrayTypeConverter operates on a one-dimensional array of elements
 of the type specified by the ExpectedType property (from BaseTypeConverter).
 By default, items are separated by spaces. If the specified type is a byte array,
 the conversion method can be changed to Base64Binary or HexBinary.

* Base64TypeConverter - converts array of bytes to Base64String and vice/versa.
 On backward conversion error it tries to convert array of bytes from HexString.

* BooleanTypeConverter - converts a Boolean value to/from string using true/false, 1/0, on/off pairs.
 The string pairs that represent boolean true and false values can be defined for a specific instance of this converter.
 Strings that are converted back are treated as case-insensitive by default.

* DateTimeTypeConverter - Converter supporting the following types: DateTime, DateTimeOffset, DateOnly and TimeOnly.

* DBNullTypeXmlConverter - Converter for System.DBNull type to string and backward.
 On ConvertTo, it converts null and DBNull values to null.
 On ConvertFrom, it gives DBNull value.
 This converter also implements Qhta.Xml.IXmlConverter interface.

* DoubleTypeConverter - Double to string converter that uses the specific Culture and Format string (as defined in BaseTypeConverter).
 If culture is not declared, then Invariant culture is used.* GDateTypeConverter - XSD types include gYear, gYearMonth, gMonth, gMonthDay, gDay. They represent, respectively: a period of the year, a specific month of the year, a specific month every year, a specific day of a specific month, a specific day of each month. Such values are represented by a specially defined GDate type, and their converter is the GDateTypeConverter class.

* EnumTypeConverter - Formattable converter for enum type. Uses standard EnumTypeConverter.
 Uses Format string in ConvertTo (as defined in Enum.ToString method).
 Converts empty string to null.

* GDateTypeConverter - Type converter for GDate structure. GDate is declared as a structure that specifies Year, Month, Day and Zone components.
 A 0 (zero) value for a specific component means it is unimportant.
 So we can have Year-only, Year-month, Year-Month-Day or Year-Month-Date-Zone values.

* GuidTypeConverter - Guid datatype converter that uses Format property (defined in BaseTypeConverter). Uses the standard GuidConverter.

* NumericTypeConverter - Universal converter for converting a numeric type into a string (forth and back).
 Supports types: Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Decimal, Single, Double.
 Specifies additional NumberStyle property to support hexadecimal format.
 Specifies min/max exclusive/inclusive values to validate the range.

* StringTypeConverter - converts a Unicode string to its serializable equivalent string (and vice versa). 
 When it meets invisible character, it can use EscapeSequences (like "\t" "\n" "\r"), Html entities or Hex entities. 
 EscapeSequences and HexEntities are predefined, but may be redefined by the programmer.
 It also supports Patterns and Enumerations on ConvertFrom (they can be case-insensitive).

* TimeSpanTypeConverter - converts TimeSpan values to a string and backward.
 Uses TimeSpanStyle as defined in System.Globalization.

* TypeNameConverter - converts type name to the specific type using known types, known namespaces and namespace prefixes.
 
* ValueTypeConverter - this class combines the above-mentioned converters.
 When creating a ValueTypeConverter class converter, the expected .NET data type must be provided, and an XSD simple type may be provided.

* XmlQualifiedNameTypeConverter - converts an XmlQualifiedName (defined in System.Xml) value to string and backward.

Helper ConversionOptions class defines options for conversion in ValueTypeConverter. These options are as follows:
* UseEscapeSequences -Specifies whether escape sequences should be used to convert strings.
* UseHtmlEntities - Specifies whether Html entities should be used to convert strings.
* DateTimeSeparator - Specifies the character to insert between the date and time when serializing a DateTime value.
* ShowFullTime - Specifies whether to display the fractional part of seconds when serializing a DateTime value.
* ShowTimeZone - Specifies whether to display the time zone when serializing a DateTime value.
* BooleanStrings - Specifies strings representation of the boolean value. First goes TrueString, second goes FalseString.
 First pair is used on serialization, all pairs are accepted on deserialization.

Two context classes can be used in conversion methods:
* TypeDescriptionContect - Class implementing ITypeDescriptorContext interface (defined in System.ComponentModel).
* PropertyInfoDescriptor - extends PropertyDescriptor class (declared in System.ComponentModel) with PropertyInfo data.
 
Several restriction interfaces are implemented in specific converters to be used in convert-back methods.
* ILengthRestrictions - defines string (or array) length restrictions.
* INumericRestrictions - defines numeric length restrictions and value restrictions.
* ITextRestrictions - defines Patterns and Enumerations and case insensitive option for string converter.
* IWhitespaceRestrictions - specifies how to treat whitespaces and whether they were fixed on convert-back,

An universal ITypeConverter interface is defined to be implemented in internal converters. It defines the following properties:
* ExpectedType - Expected type of output value in convert-back methods.
* KnownTypes - Known types array for convert-back methods.
* XsdType - to be used inconvert-forth methods.
* Format - used for some data types.
* Culture - used for some data types.
