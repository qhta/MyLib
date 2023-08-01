A package of classes implementing System.Windows.IValueConverter interface. 
Many of them are one-way converters. They are usefull to control the layout from MVVM ViewModels.

Converter classes:
* BoolToVisibilityConverter - One way value converter bool to Visibility (Visible, Collapsed).
* CamelStringConverter - String value converter between "camel string" and "CamelString".
* ColorConverter - Converts a color to string or brush.
* ColorToSolidBrushConverter - One-way color to brush converter.
* DashStyleConverter - Type Converter to convert between DashStyle and string.
* DecimalValueConverter - Converts a decimal value to string and back. 
* DoubleValueConverter - Converts a double value to string and back. 
* EnumValueConverter - Converts an enumerated value to a numeric value according to the given list of values.
* NullableIntConverter - Int value converter that converts null string to null int.
* IconEnabledConverter - One-way converter that changes Image. If value is false (not enabled) than Overlay is added to the Image.
* IndexingConverter - One-way multi-value converter that invokes an indexing property from the first value item.
* MultiObjectConverter - One-way multi-value converter that clones value objects.
* NumericValueConverter - Converts value of any type to numeric value.
* RowToIndexConverter - One-way converter from DataGridRow index to integer+1.
* SafeCollectionConverter - One-way converter that safely converts collection to an array.
* String2ObjectConverter - One-way converter that gets a value from a dictionary.
* StringFormatConverter - One-way converter that converts a value using parameter as format.
* StringsWidthConverter - One-way converter thar gets a pixel width of the string.
* TypeNameConverter -  One-way converter that gets a type name.
* ValidityBrushConverter -  One-way converter that gets a color from string-to-color dictionary and converts it to solid brush.

Helper converters:
* StringConverter - Abstract converter to operate on two string values. The second value is passed as a binding declaration or as Param dependency property.
* AppendingConverter - String converter to append a string parameter to the string value.
* ArithmeticConverter - Abstract converter to operate on two double values. The second value is passed as a binding declaration or as Param dependency property.
* AddingConverter - Arithmetic converter to add a double parameter to the double value.
* SubtractingConverter - Arithmetic converter to subtract a double parameter from the double value.
* MultiplyingConverter - Arithmetic converter to multiply the double value by the double parameter.
* DividingConverter -  Arithmetic converter to divide the double value by the double parameter.
* InverseValueConverter - Converts a double value to one over value.
* NegateValueConverter - Converts a numeric value to a negate numeric value.
* NegateBoolConverter - Converts a bool value to a negate bool value.
* BitTestConverter - One way converter to convert a specific bitset to bool. 
* EqualityComparingConverter - Compares a value to the parameter and returns a boolean value.
* IndirectPropertyConverter - Multi-value converter with two bindings. First binding returns an instance object. Second binding returns a property name. Converter gets a value from this property found in the instance object

Helper classes:
* ColorDictionary - Dictionary of Colors indexed by string names.Used in ValidityBrushConverter.
* DashStyleUtils - Helper methods to convert dash style to string. Used in DashStyleConverter.
* EnumValue - Used in EnumValueConverter.
* String2ObjectDictionary - A dictionary that maps strings to object. Used in String2ObjectConverter.
