#TestHelper

TestHelper is a helper library for development testing. Unlike unit testing frameworks like MS Test, Nunit, XUnit.net, etc., it's not just for passing/failed testing, it's more for keeping track of what's going on during test execution. Thanks to this, the developer can more easily find out how to use external components that he cannot track directly, as well as in the operation of complex algorithms in which he can hide mechanisms for previewing intermediate and partial results.

In general, the infrastructure of the TestHelper component can be divided into four areas:

- TraceWriting - tracking by saving data, results and events,
- FileComparing - comparing files (especially with the results),
- TestBuilding - creating and running tests,
- Others - other.

## TraceWriting

The TraceWriting infrastructure is defined based on the following interfaces:

- ITextWriter - defines text writing operations including text indentation.
- IConsoleWriter - extends the ITextWriter interface with the ability to write text in color on the system console,
- ITraceTextWriter - extends the IConsoleTextWriter interface with the ability to simultaneously write text to the console, to the Trace class and to a text file (stream),
- ITraceXmlWriter - extends the ITraceTextWriter interface with the ability to create an Xml structure,
- ITraceMonitor - allows you to collect notifications from the monitored application and process them collectively

and the classes that implement them:

- BufferedTextWriter – an abstract class implementing the ITextWriter interface based on TextWriter using a buffer.
- ConsoleWriter - implements the IConsoleWriter interface, allows you to write to the console using colors.
- TraceWriter - extends the ConsoleWriter class, implements the ITextWriter interface, allows you to write text to the console and to the Trace class at the same time, color is interpreted when output to the console,
- TraceTextWriter - extends the TraceWriter class. implements the ITraceTextWriter interface, allows you to write text to the console, to the Trace class and to the file/stream at the same time, the color is interpreted when output to the console,
- TraceXmlWriter - extends the TraceTextWriter class, implements the ITraceXmlWriter interface, allows you to create an Xml structure,
- MessageCounting Monitor – implements the ITraceMonitor interface by counting messages sent from a specific method, Creates an implementation of the TraceTextWriter class in the Flush method.

This solution makes it possible to bind other components to the TraceWriting module through interfaces and possibly implement these interfaces in other components.

## FileComparing

The TestHelper module contains four classes for comparing files. These are:

- FileCompareOptions – represents the comparison options. Allows you to trim spaces at line ends, treat multiple spaces as single, ignore case, etc.
- AbstractFileComparer – abstract file comparison class. It contains the basic operations of comparing and displaying lines. The underlying CompareFiles method must be implemented in the child class. Uses the ITraceStreamWriter interface to display non-matching lines.
- TextFileComparer - a specific class that compares text files. It implements a number of comparison options, including ignoring blank lines. When it detects inconsistent line areas, it tries to synchronize the comparison of further areas.
- XmlFileComparer – a specific class that compares Xml files. Unlike text comparison, it interprets the internal structure of Xml files.

## TestBuilding

The TestHelper module contains the basic infrastructure for creating tests. These are the classes:

- AbstractTest - an abstract base class for all tests defining the Run method, the virtual Init method and the abstract Prepare, Execute and Finalize methods,
- AbstractFileGenTest - abstract base class for file generation tests, includes results comparison.
- TestCase - an abstract class representing a test case, it has an abstract Execute method,
- TestCase\<DataType\> – a class representing a test case that stores data and performs a test function on this data.