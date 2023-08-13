This package contains type converters (derived from the TypeConverter class) that extend the set of standard type converters 
with flexible formatting capabilities.

New converters:
* StringTypeConverter - converts a Unicode string to its serializable equivalent string (and vice versa). It can operate in several modes (depending on the set of properties).
* BooleanTypeConverter - converts a Boolean value to/from string using true/false, 1/0, on/off pairs.
* NumericTypeConverter - universal converter for converting a numeric type into a string (back and forth). Supports types: Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Decimal, Single, Double.
* DateTimeTypeConverter - converter supporting the following types: DateTime, DateTimeOffset, DateOnly and TimeOnly.
* TimeSpanTypeConverter - converts TimeSpan values to a string and back.
* GDateTypeConverter - XSD types include gYear, gYearMonth, gMonth, gMonthDay, gDay. They represent, respectively: a period of the year, a specific month of the year, a specific month every year, a specific day of a specific month, a specific day of each month. Such values are represented by a specially defined GDate type, and their converter is the GDateTypeConverter class.
* AnyUriTypeConverter - The Visual Studio community provides a UriTypeConverter to convert Uri type to String. This converter just gives the original Uri string without any conversion. On its basis, the AnyUriTypeConverter converter was defined, which implements the ITypeConverter interface and returns null when reverse conversion is performed for the empty string.
* GuidTypeConverter - The Guid type converter respects the standard formats for this type.
* ArrayTypeConverter - ArrayTypeConverter operates on a one-dimensional array of elements of the type specified by the ExpectedType property. By default, items are separated by spaces. If the specified type is a byte array, the conversion method can be changed to Base64Binary or HexBinary.
* ValueTypeConverter - this class combines the above-mentioned converters. When creating a ValueTypeConverter class converter, the expected .NET data type must be provided, and an XSD simple type may be provided.
* FloatDoubleFormatter - One side double value converter in specific culture.