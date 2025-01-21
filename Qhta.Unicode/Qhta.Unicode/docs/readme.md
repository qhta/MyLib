This package provides a UnicodeData instance. It is a dictionary-like object that maps Unicode code points to their corresponding UnicodeData objects.
UnicodeData is parsed from the UnicodeData.txt and NameAliases.txt files, which should be located in the default directory.
If the files are not found, the package will attempt to download them from the Unicode Consortium's website.
UnicodeData is a singleton, so only one instance will ever exist. It is created when with the static Instance property.
# 