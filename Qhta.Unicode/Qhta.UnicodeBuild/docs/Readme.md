UnicodeBuild is a research application for creating Unicode data files by transforming the Unicode Character Database (UCD) 
and other related sources.

The application runs on a Unicode database in Microsoft Access format. 
Data provided by the Unicode Consortium is previously imported into MS Access.

The application helps researchers prepare data file for encoding/decoding Unicode text in text files (such as XML or JSON) 
in an unambiguous and human-readable format. This format facilitates reader understanding of Unicode text strings 
and facilitates minor corrections without the use of a specialized Unicode text editor.

The output data file is a CSV file that contains a list of Unicode point codes with their character names.
A character name is a short name that uniquely identifies a character,
or an expression that makes it possible to identify a character unambiguously.
Character names are generated in the UnicodeBuild application based on a set of rules that take into account the character properties.
They can be used in a separate encoding/decoding library.

Application is build using MVVM pattern in C# and WPF.
It consists of several layers:
- [Database layer](#unicode-database) - is based on MS Access database that contains tables imported from Unicode Character Database (UCD) 
provided by Unicode Consortium and supplemented from other sources (mainly Wikipedia).
- [Data Model layer](#data-model) - contains Model classes for accessing the database and retrieving data. This layer is implemented using Entity Framework Core.
- [ViewModels layer](#viewmodels) - contains ViewModel classes that provide data for displaying and processing in the higher layers.
- [Business layer](#business-layer) - contains classes that implement logic for generating character names.
- [Commands layer](#commands) - contains Command classes that implement actions triggered by user interface events.
- [Presentatation layer](#presentation-layer) - contains View (and Window) classes that implement the user interface using WPF. 
It widely uses Syncfusion WPF controls supplemented with Qhta.SF.WPF.Tools library.

# Unicode Database

The Unicode database is stored in the Unicode.accdb file.
It is modeled after the UCD structure, but with some differences.

The database contains of many tables, but only a few of them are used in the application:
- UcdCodePoints - contains code point definitions from the UCD file UnicodeData.txt.
- UcdBlocks - contains block definitions from the UCD file Blocks.txt.
- WritingSystems - contains writing system definitions from the UCD file Scripts.txt, 
  as well as additional writing systems that are not scripts.
- WritingSystemTypes - contains definitions of writing system types.
- WritingSystemKinds - contains definitions of writing system kinds.
- Categories - contains definitions of Unicode categories.
- Aliases - contains alias names for Unicode characters.
- AliasTypes - defines types of aliases.
- NameGenMethods - contains definitions of name generation methods.

## UcdCodePoints table

The UcdCodePoints table contains code point definitions from the UCD file UnicodeData.txt. 
Several modifications were made to the original data:
- The CodePoint identifier is stored as a number (Long Integer) field named ID. 
It is easier to present code points in the code-value ascending order then using hexadecimal code.
- The Glyph field was added to store the character itself. It is useful for visualization of the character. 
Combined characters are expressed as a sequence of dashed circle and this combining character.
- The CharName field was added contains the generated character name.
- Block field was added as a foreign key to the UcdBlocks table.
- Area, Script, Language, Notation, SymbolSet, and Subset fields were added as foreign keys to the WritingSystems table.

## UcdBlocks table

The UcdBlocks table contains block definitions from the UCD file Blocks.txt with the following modifications:
- Autonumbered field ID was added as a primary key.
- Start and End fields were added to store the range boundaries as numbers (Long Integer).
- WritingSystemID was added as a foreign key to the WritingSystems table. It identifies the default writing system associated with the block.
- Comment field was added to store additional comment about the writing system.
- Scripts field was added to store a list of scripts associated with the block (extracted from Wikipedia).
- MajorAlphabets field was added to store a list of major alphabets associated with the block (extracted from Wikipedia).
- SymbolSets field was added to store a list of symbol sets associated with the block (extracted from Wikipedia).
- Size field was added to store the number of code points in the block.
- Assigned field was added to store the number of assigned code points in the block.
- Plane field was added to store the plane number (0 to 16) where the block is located.

## WritingSystems table

The WritingSystems table contains writing system definitions from the UCD file Scripts.txt, 
supplemented with additional writing systems that are not scripts. The WritingSystems table contains the following fields:
- ID - Autonumbered field as a primary key.
- Name - the name of the writing system.
- Type - a foreign key to the WritingSystemTypes table.
- Kind - a foreign key to the WritingSystemKinds table.
- ParentID - a foreign key to the WritingSystems table itself, used to create a hierarchy of writing systems.
- KeyPhrase - a short phrase that can be used to recognize the writing system by comparison with character descriptions.
- Ctg - a Unicode category that is typically used in the writing system.
- ISO - an ISO name associated with the writing system.
- Abbr - an abbreviation of the writing system name which will be used in character names.
- Ext - an extra code to be added to the abbreviation to make it unique.
- RTL - a Yes/No field indicating if the writing system is written from right to left.
- Bicameral - a Yes/No field indicating if the writing system has both uppercase and lowercase letters.
- NameGenMethod - a foreign key to the NameGenMethods table, indicating the method used to generate character names for this writing system.
- NameGetFiled - a name of the file, which can be used by the NameGenMethod.

## WritingSystemTypes table

The WritingSystemType table contains definitions of writing system types. They are as follows:
- Script - a set of characters used to write one or more languages (e.g., Latin, Cyrillic, Han).
- Language - a set of characters used to write a specific language (e.g., Ukrainian, Byellorusian). 
Only a small number of languages are defined, as usually a script is sufficient to identify a character.
- Notation - a set of characters used in a specific notation system, which is not a natural language, 
but has its rules  (e.g., Phonetic, Musical).
- SymbolSet - a set of characters used in a specific symbol system (e.g., Emoji, Math).
- Subset - a subset of characters from a script or symbol set (e.g., syllables, vowels, consonants).
- CodeArea - a set of characters that are grouped together in a specific code area (e.g., Private Use Area, Surrogates).
- Family - an organizational grouping of scripts (e.g., Brahmic, CJK).

## WritingSystemKinds table

The WritingSystemKinds table contains definitions of writing system kinds. They are as follows:
- Abjad - a writing system that primarily represents consonants, with vowels being optional or indicated by diacritics (e.g., Arabic, Hebrew).
- Abugida - a writing system where each character represents a consonant-vowel combination, with the vowel being inherent or indicated by diacritics 
(e.g., Devanagari, Ethiopic).
- Alphabet - a writing system where each character represents a single phoneme, including both consonants and vowels (e.g., Latin, Cyrillic).
- Constucted - a writing system that is artificially created for specific purposes, such as fictional languages or specialized notations.
- Control - a set of non-printable characters that control text formatting and layout (e.g., Line Feed, Carriage Return).
- Cuneiform - an ancient writing system that uses wedge-shaped marks on clay tablets (e.g., Sumerian, Akkadian).
- Encoding - a set of characters used for encoding other characters.
- Form - a set of characters in specific forms (e.g. bold, wide)
- Graphical - a set of characters used for graphical representation (e.g., Box Drawing, Geometric Shapes).
- Hieroglyphic - an ancient writing system that uses pictorial symbols to represent words or sounds (e.g., Egyptian Hieroglyphs).
- Ideographic - a writing system where each character represents an idea or concept rather than a specific sound (e.g., Kanxi Radicals).
- Invisible - a set of non-printable characters that are invisible in text (e.g., Zero Width Space, Non-Joiner).
- Logographic - a writing system where each character represents a word or morpheme (e.g., Anatolian Hieroglyphs).
- Mixed - a writing system that combines elements of different types (e.g., Japanese, which uses Kanji, Hiragana, and Katakana).
- Musical - a set of characters used in musical notation (e.g., notes, clefs, rests).
- Numerical - a set of characters used to represent numbers (e.g., Arabic numerals, Roman numerals).
- Phonetic - a set of characters used to represent the sounds of speech (e.g., International Phonetic Alphabet).
- Pictographic - a writing system that uses pictorial symbols to represent words or concepts (e.g.,  emoticons).
- Semi-syllabary - a writing system where some characters represent syllables while others represent individual sounds (e.g., Bopomofo).
- Stenography - a set of characters used in shorthand writing systems (e.g., Duployan Shorthand).
- Syllabary - a writing system where each character represents a syllable (e.g., Hiragana, Katakana).
- Symbolic - a set of characters used to represent symbols or icons (e.g., alchemical, astrological).
- Pictophonetic - a writing system that combines pictorial and phonetic elements (e.g., Egyptian Hieratic).
- Metrical - a set of characters used in metrical notation (e.g., poetic meter symbols).
- Mathematical - a set of characters used in mathematical notation (e.g., plus sign, integral symbol).
- Gestural - a set of characters used to represent gestures or sign languages (e.g., SignWriting).

## Categories table

The Categories table contains definitions of Unicode Character categories. The definitions are extracted from Wikipedia.
The table contains the following fields:
- ID - byte field as a primary key.
- Ctg - a two-letter Unicode category code.
- Name - full name of the Unicode category.
- Cat1 - first part of category name.
- Cat2 - second part of category name.
- info - additional information about the category (4-digit hexadecimal).
- Comment - additional comment about the category.

## Aliases, AliasTypes tables

Alias table stores aliases for Unicode characters. 
Each alias has a foreign key to the Ord primary key of UcdCodePoints table and a type defined in AliasTypes table.

Data was imported from the UCD file NameAliases.txt.

## NameGenMethods table

The NameGenMethods table contains definitions of name generation methods. They are as follows:
- None - No name generation method is defined.
- Predefined - Character names are predefined in a NameGenFile.
- Ordinal - Character names are generated using an ordinal method, where each name is assigned a unique ordinal number.
- Abbreviation - Character names are generated using an abbreviation method, where each name is formed by abbreviating a longer name.
- Procedural - Character names are generated using a procedural method, where names are formed based on a set of rules and procedures.
- Numeric - Character names are generated using a numeric method, where numeric words are converted to their corresponding numbers.

# Data model

The Data Access layer contains Model classes for accessing the database and retrieving data. 
The classes were produced using Entity Framework Code in Code First paradigm.

Main class is _DbContext class that joins all DbSet(s) of entity classes as well as OnModelCreating method 
used to specify mappings of entities properties to tables columns.

We don't use migrations. If there is a need to change the schema of the Unicode database 
we use MS Access application to do so, and we separately modify classes and/or mappings definitions.

Those table columns which hold foreign keys to tables defining types, kinds, and categories, 
we mapped to enum types based on byte type (according to column size). Enum types are the following:
- AliasType,
- NameGenMethos,
- UcdBidir,
- UcdCategory,
- UcdCombination,
- WritingSystemKind,
- WritingSystemType.

For these enum types, which occur in UI comboboxes to select, we defined also entity classes 
which let user to see the enum description (stored in Unicode database). These are:
- NameGenMethodEntity,
- UcdCategoryEntity,
- WritingSystemKindEntity,
- WritingSystemTypeEntity.

Primary entity classes are the following:
- UcdCodePoint - represents a single code point entity with all its properties, 
- UcdBlock - represents a single block entity with all its properties,
- WritingSystem - represents a single writing system entity with all its properties.

Supporting entity classes are:
- Alias - represents a single alias entity joined to UcdCodePoint,
- ScriptMapping - represents a mapping between a block and a script (many-to-many relationship). 
This class is not mapped to a table, but is used in the application logic.

Entity classes properties are declared as simple get/set properties with automatic accessors. 
Entity framework implements these accessors in the background.

Some properties are named differently than the corresponding table columns.

# ViewModels

The ViewModel classes provide access data for displaying and processing entities.
Each ViewModel class wraps an entity class and provides additional properties and methods for data manipulation.
Each of them is based on the EntityViewModel<T> generic base class that 
defines a Model property of type T (the entity class) and provides methods for changing 
ViewModel properties or Model properties (with Undo/Redo support and and property change notification).
The base class of EntityViewModel<T> is ViewModel<T> generic class defined in Qhta.MVVM library.

ViewModel classes of the same type are grouped in EntityCollection<T> generic class 
which is based on ObservableList<T> class defined in Qhta.ObservableObject library. 
The ObservableList<T> class is multithread version of List<T> with CollectionChanged notification.
It should be used instead of ObservableCollection<T> in MVVM architecture model when background threads are used.

A singleton class _ViewModels is defined to provide access to _DbCollection singleton and all ViewModel collections. 
It also provides properties to get selectable collections of view models for comboboxes in UI.

Primary ViewModel classes are the following:
- UcdCodePointViewModel - represents a single code point entity with all its properties, 
  including navigation properties to related entities (like Block, Area, Script, Language, Notation, SymbolSet, Subset).
- UcdBlockViewModel - represents a single block entity with all its properties,
- WritingSystemViewModel - represents a single writing system entity with all its properties.

Supporting ViewModel classes are:
- AliasViewModel - represents a single alias entity with all its properties,
- NameGenMethodViewModel - represents a single name generation method entity with all its properties,
- WritingSystemTypeViewModel - represents a single writing system type entity with all its properties,
- WritingSystemKindViewModel - represents a single writing system kind entity with all its properties.

These ViewModel classes that has fields with enum types may have additional navigation properties of corresponding entity ViewModel types.
Changing the enum property will change the corresponding entity property, and vice versa.

## UcdCodePointViewModel

UcdCodePointViewModel class represents a single code point ViewModel with all its properties:
- Id - entity identifier as a primary key.
- CP - code point in hexadecimal format (string).
- Glyph - a string that visually represents the character.
- GlyphSize - font size for displaying the glyph. It can be changed in to enlarge the Glyph.
- CharName - generated character name.
- Description - character description from UCD.
- Ctg - Unicode category as enum type.
- Category - Unicode category as entity ViewModel type.
- Comb - Unicode combination type as enum type.
- Bidir - Unicode bidirectional type as enum type.
- Decompositon - decomposition mapping from UCD.
- DecDigitValue - decimal digit value from UCD (as nullable byte).
- DigitVal - digit value from UCD (as nullable byte).
- NumVal - numeric value from UCD (as nullable string).
- Mirr - mirrored property from UCD (boolean).
- OldDescription - old character description from UCD.
- Comment - additional comment from UCD.
- Upper - uppercase mapping from UCD (as nullable CodePoint).
- Lower - lowercase mapping from UCD (as nullable CodePoint).
- Title - titlecase mapping from UCD (as nullable CodePoint).
- Aliases - collection of aliases associated with the code point.
- BlockId - foreign key to the block entity.
- Block - navigation property to the block ViewModel.
- AreaId - foreign key to the area writing system entity.
- Area - navigation property to the area writing system ViewModel.
- ScriptId - foreign key to the script writing system entity.
- Script - navigation property to the script writing system ViewModel.
- LanguageId - foreign key to the language writing system entity.
- Language - navigation property to the language writing system ViewModel.
- NotationId - foreign key to the notation writing system entity.
- Notation - navigation property to the notation writing system ViewModel.
- SymbolSetId - foreign key to the symbol set writing system entity.
- SymbolSet - navigation property to the symbol set writing system ViewModel.
- SubsetId - foreign key to the subset writing system entity.
- Subset - navigation property to the subset writing system ViewModel.

## UcdBlockViewModel

UcdBlockViewModel class represents a single block ViewModel with all its properties:
- Id - entity identifier as a primary key.
- Range - block range in format "XXXX..YYYY" (CodeRange type).
- Name - block name from UCD.
- Description - block description from Wikipedia.
- WritingSystemId - foreign key to the writing system entity.
- WritingSystem - navigation property to the writing system ViewModel.

## WritingSystemViewModel

WritingSystemViewModel class represents a single writing system ViewModel with all its properties:
- Id - entity identifier as a primary key.
- Name - writing system name.
- FullName - writing system name and type.
- Type - writing system type as enum type.
- Kind - writing system kind as enum type.
- NameGenMethod - name generation method as enum type.
- NameGenFile - name generation file name.
- ParentId - foreign key to the parent writing system entity.
- KeyPhrase - key phrase used for writing system recognition.
- Ctg - Unicode category used for writing system recognition. It is not an enum type, 
as it can express multiple categories by using wildcards (e.g. "L\*").
- Iso - ISO code associated with the writing system.
- Abbr - abbreviation of the writing system name.
- Ext - extra code to be added to abbreviation to make it unique.
- Description - writing system description from Wikipedia.
- Tooltip - description or full name used as a tooltip in UI.
- Parent - navigation property to the parent writing system ViewModel.
- IsUsed - indicates if the writing system is used in any code point or has child writing systems.
- Children - collection of child writing systems.
- IsExpanded - indicates if the writing system node is expanded in the tree view in UI.
- IsMarked - indicates if the writing system is marked in UI (e.g. for deletion).

# Business layer

Business layer contains classes that implement logic for generating character names. 
It includes the following classes:
- NameGenerator - main class that implements the logic for generating character names.
- NameComparer - class that implements logic for comparing character names. It prevents duplicate names.
- NameGenOptions - class that implements options for name generation.
- NameGenOptionsDialog - dialog window for setting name generation options.
- SpecialFunctions - enumeration of special functions which can be used in generated names (in Procedural method).

# Commands

Command classes implement actions triggered by user interface events. 
They can be also considered as part of the Business layer, but their logic is simplier and they are more closely related to UI.
As this application uses Qhta.SF.WPF.Tools library, all command classes defined in this library are also available.
Commands are invoked from the UI using Command bindings which refer to the commands defined in _Commander singleton class.

The _Commander class provides access to all command instances used in the application.
They are the followind:
- ApplyBlockMappingCommand - maps code points to Unicode blocks using rules. 
A code point is mapped to a block if its code value is within the block range, this command fills the Block property of code points.
- ApplyCharNamesGenerationCommand - applies character name generation to selected code points using the NameGenerator class. 
The CharName property of code points is filled.
- ApplyWritingSystemMappingCommand - maps code points to writing systems using a file provided by the user.
File must be in CSV format with two columns: first column contains CodePoint range (two hexadecimal separated by "...") 
or a single CodePoint (hexadecimal);
second column identifies a WritingSystem (name or abbreviation). 
A type if the WritingSystem (Area, Script, Language, Notation, SymbolSet, Subset) is recognized, 
and the appropriate property of the code point is filled.
- ApplyWritingSystemRecognitionCommand - recognizes writing systems for code points based on their descriptions and properties.
- BrowseNameGenFileCommand - opens a file dialog to select a name generation file.
- DeleteWritinsSystemCommand - deletes writing system from the database.
- EditWritingSystemCommand - opens a dialog to edit writing system properties.
- MarkUnusedWritingSystemsCommand - marks writing systems that are not used in any code point.
- NewWritingSystemCommand - opens a dialog to create a new writing system.

Many commands execution are implemented in async mode using Task.Run method. 
The base command class for this case is TimeConsumingCommand.
This command is associated with BreakTimeConsumingCommand which can be used to cancel the execution.

_Commander class also provides access to standard commands defined in Qhta.SF.WPF.Tools library:
- ApplicationCommands.Save
- ApplicationCommands.Copy
- ApplicationCommands.Cut
- ApplicationCommands.Paste
- ApplicationCommands.Delete
- ApplicationCommands.Undo
- ApplicationCommands.Redo
- ApplicationCommands.Find
- FindNextCommand

# Presentation layer

Presentation layer contains views (and windows) classes that implement the user interface using WPF. 
It contains MainWindow class and View controls for main EntityCollection(s). 
Is also contains a dialog window to edit a writing systems.

This EntityCollection(s) are displayed using Syncfusion SfDataGrid control. Columns are defined in XAML (not auto-generated).

## MainWindow

Main window is MainWindow class that contains the TabControl with three tabs:
- UcdCodePointsTab - contains an UcdCodePointView to display code points and their properties.
- UcdBlocksTab - contains an UcdBlockView to display blocks and their properties.
- WritingSystemsTab - contains a WritingSystemView to display writing systems and their properties.

MainWindow initializes a BackgroundTimer that periodically checks invokes CommandManager.InvalidateRequerySuggested method 
and _Commander.NotifyCanExecuteChanged method to update the state of commands.

MainWindow handles KeyDown event to process Ctrl-Z and Ctrl-Y key for global Undo/Redo commands. 
It also handles Shift-Return key to persuade focused element to process it as Return key 
(this is useful in Syncfusion LongTextColumn to enter new line to edited text).

Each tab item handles its MouseMove event to implement drag-and-drop operation. 
Dragging a tab item out of the tab control creates a new window with the same content.
NewWindow class is defined for this purpose.

## UcdCodePointsView

UcdCodePointsView class is defined to display code points in the SfDataGrid. Columns are the following:
- CP - displays code point in hexadecimal format in a TemplateColumn.
- CharName - displays generated character name in a TextColumn.
- Glyph - displays the character itself in a TextColumn.
- Description - displays character description int a TextColumn.
- Category - displays Unicode category in a ComboBoxColumn.
- Block - displays block name in a ComboBoxColumn.
- Area - displays area writing system name in a ComboBoxColumn.
- Script - displays script writing system name in a ComboBoxColumn.
- Language - displays language writing system name in a ComboBoxColumn.
- Notation - displays notation writing system name in a ComboBoxColumn.
- SymbolSet - displays symbol set writing system name in a ComboBoxColumn.
- Subset - displays subset writing system name in a ComboBoxColumn.

## UcdBlocksView

UcdBlocksView class is defined to display blocks in the SfDataGrid. Columns are the following:
- Range - displays block range in a TemplateColumn.
- Name - displays block name in a TextColumn.
- WritingSystem - displays writing system name in a ComboBoxColumn.
- Description - displays block description in a LongTextColumn.

## WritingSystemsView

WritingSystemsView class is defined to display writing systems in a TreeGrid control (Syncfusion SfTreeGrid)
and in a SfDataGrid control. Columns are the following:
- IsMarked - displays a checkbox to mark writing system in a CheckBoxColumn.
- Name - displays writing system name in a TextColumn.
- Type - displays writing system type in a ComboBoxColumn.
- Kind - displays writing system kind in a ComboBoxColumn.
- Parent - displays parent writing system name in a ComboBoxColumn.
- KeyPhrase - displays key phrase in a TextColumn.
- Ctg - displays Unicode category in a TextColumn.
- Iso - displays ISO code in a TextColumn.
- Abbr - displays abbreviation in a TextColumn.
- Ext - displays extra code in a TextColumn.
- Description - displays writing system description in a LongTextColumn.
- NameGenMethod - displays name generation method in a ComboBoxColumn.
- NameGenFile - displays name generation file name in a TextColumn.
