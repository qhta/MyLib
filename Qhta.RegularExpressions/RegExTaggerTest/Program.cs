using Qhta.RegularExpressions;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xaml;

namespace RegExTaggerTest
{
  class Program
  {
    [Flags]
    enum TestOptions
    {
      /// <summary>
      /// If input file is not protected && output file is not specified, 
      /// then input file can be overwritten by the corrected or added tests.
      /// </summary>
      IsInputFileProtected,
      IsOutputFileSpecified,
      TestOneCharPatterns,
      TestTwoCharsPatterns,
    }

    const string QuantifierChars = "?+*";
    const string AnchorChars = "^$";

    static List<string> testPatterns = new List<string>
    {
      //@"\b(\w+?)\s\1\b",
      //@"((\w+)[\s.])+.",
      //@"(?<name_012>subexpression[a-zA-Z0-9])",
    };

    static TestData TestInputData, testOutputData;
    static RegExTagger Tagger = new RegExTagger();
    static TextWriter TestOutput = Console.Out;

    static void Main(string[] args)
    {
      string testInputFileName = null;
      string testOutputFileName = null;
      TestOptions testOptions = 0;
      ReadArgs(args, out testInputFileName, out testOutputFileName, out testOptions);

      if (testInputFileName != null)
      {
        TestOutput.WriteLine($"reading test data from file \"{testInputFileName}\"");
        ReadTestData(testInputFileName);
        TestOutput.WriteLine($"read tests: {TestInputData.Count}");
        TestOutput.WriteLine($"");
      }
      else
        CreateTestData();
      testOutputData = new TestData();

      bool? wholeTestResult = null;

      if (testOptions.HasFlag(TestOptions.TestOneCharPatterns))
        wholeTestResult = TestOneCharPatterns();

      if (TestInputData.Count>0)
        wholeTestResult = RunTestInputData();

      bool? shouldSave = false;
      if (wholeTestResult == false)
      {
        if (testOptions.HasFlag(TestOptions.IsOutputFileSpecified))
          shouldSave = true;
        if (!testOptions.HasFlag(TestOptions.IsInputFileProtected))
          shouldSave = null;
      }
      TrySaveTestData(testOutputFileName, shouldSave, $"Test results differ from expected. Do you want to write test data to file \"{testOutputFileName}\"?");

      bool moreTestsAdded = TryAddMoreTests();

      if (moreTestsAdded)
        TrySaveTestData(testOutputFileName, null, $"You have added some test. Do you want to save all test data to file \"{testOutputFileName}\"?");
    }

    /// <summary>
    /// Reads filenames and options from args string collection.
    /// </summary>
    /// <param name="args"></param>
    static void ReadArgs(string[] args, out string inputFileName, out string outputFileName, out TestOptions options)
    {
      inputFileName = null;
      outputFileName = null;
      options = 0;
      if (args.Length == 0)
      {
        WriteUsage();
      }
      else
      {
        for (int i = 0; i < args.Length; i++)
        {
          //TestMonitor.WriteLine($"args[{i}]: {args[i]}");
          var arg = args[i];
          if (arg[0] == '/')
          {
            switch (arg[1])
            {
              case 'o':
                options |= TestOptions.IsOutputFileSpecified;
                break;
              case 'i':
                options |= TestOptions.IsInputFileProtected;
                break;
              case '1':
                options |= TestOptions.TestOneCharPatterns;
                break;
              case '2':
                options |= TestOptions.TestTwoCharsPatterns;
                break;
            }
          }
          else
          {
            if (options.HasFlag(TestOptions.IsOutputFileSpecified))
            {
              outputFileName = arg;
            }
            else
            {
              inputFileName = arg;
            }
          }
        }
        if (outputFileName == null && !options.HasFlag(TestOptions.IsInputFileProtected))
          outputFileName = inputFileName;
        TestOutput.WriteLine($"");
      }

    }

    /// <summary>
    /// Writes usage help messages to console
    /// </summary>
    static void WriteUsage()
    {
      TestOutput.WriteLine($"arguments: [testInputFileName [/o testOutputFileName]]");
      TestOutput.WriteLine($"options:");
      TestOutput.WriteLine($"\t/1 - test one-char patterns");
      TestOutput.WriteLine($"\t/2 - test two-chars patterns");
      TestOutput.WriteLine($"\t/i - intput file is input-only (is not rewritten)");
    }

    /// <summary>
    /// Tests one-char patterns. No test input data needed. No test output data produced.
    /// </summary>
    /// <returns></returns>
    static bool? TestOneCharPatterns()
    {
      bool? wholeTestResult = null;

      for (char ch = ' '; ch < '\x7F'; ch++)
      {
        var pattern = new string(ch, 1);
        var testInputItem = new TestItem { Pattern = pattern, Result = RegExStatus.OK };
        RegExItem newItem;
        if (ch == '(')
          newItem = new RegExGroup { Start = 0, Str = new string(ch, 1), Tag = RegExTag.Subexpression, Status = RegExStatus.Unfinished };
        else if (ch == '.')
          newItem = new RegExItem { Start = 0, Str = new string(ch, 1), Tag = RegExTag.DomainChar, Status = RegExStatus.OK };
        else if (ch == '[')
          newItem = new RegExCharset { Start = 0, Str = new string(ch, 1), Tag = RegExTag.CharSet, Status = RegExStatus.Unfinished };
        else if (ch == '\\')
          newItem = new RegExItem { Start = 0, Str = new string(ch, 1), Tag = RegExTag.EscapedChar, Status = RegExStatus.Unfinished };
        else if (QuantifierChars.Contains(ch))
          newItem = new RegExItem { Start = 0, Str = new string(ch, 1), Tag = RegExTag.Quantifier, Status = RegExStatus.Warning };
        else if (AnchorChars.Contains(ch))
          newItem = new RegExItem { Start = 0, Str = new string(ch, 1), Tag = RegExTag.AnchorControl, Status = RegExStatus.OK };
        else if (ch == '{')
          newItem = new RegExItem { Start = 0, Str = new string(ch, 1), Tag = RegExTag.Quantifier, Status = RegExStatus.Unfinished };
        else
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.LiteralChar, Status = RegExStatus.OK };
        testInputItem.Items.Add(newItem);
        testInputItem.Result = newItem.Status;
        TestItem testOutputItem;
        var oneTestResult = RunOneTest(testInputItem, out testOutputItem);
        testOutputData.Add(testOutputItem);
        if (oneTestResult == false)
          wholeTestResult = false;
        else if (oneTestResult == true)
        {
          if (wholeTestResult != false)
            wholeTestResult = true;
        }
      }
      return wholeTestResult;
    }

    /// <summary>
    /// Runs a single test of one-char pattern
    /// </summary>
    /// <param name="testInputItem"></param>
    /// <param name="testOutputItem"></param>
    /// <returns></returns>
    static bool? RunOneTest(TestItem testInputItem, out TestItem testOutputItem)
    {
      var pattern = testInputItem.Pattern;
      TestOutput.WriteLine($"pattern = {pattern}");
      testOutputItem = new TestItem { Pattern = pattern };
      testOutputItem.Result = Tagger.TryParseText(pattern);
      testOutputItem.Items = Tagger.Items;
      bool? testResultOK = null;
      if (testInputItem.Result != null)
      {
        testResultOK = (testInputItem.Result == testOutputItem.Result);
        if (testResultOK == true)
          testResultOK = (testInputItem.Items == null) ||
          ItemsAreEqual(testInputItem.Items, testOutputItem.Items);
        if (testResultOK == true)
          TestOutput.WriteLine($"test PASSED");
        else if (testResultOK == false)
          TestOutput.WriteLine($"test FAILED");
      }
      TestOutput.WriteLine($"Result = {testOutputItem.Result}");
      Dump(TestOutput, testOutputItem.Items);
      TestOutput.WriteLine($"");
      return testResultOK;
    }

    /// <summary>
    /// Runs testing of data stored in testInputData collections. Fills testOutputData collection.
    /// </summary>
    /// <returns></returns>
    static bool? RunTestInputData()
    {
      bool? wholeTestResult = null;
      for (int i = 0; i < TestInputData.Count; i++)
      {
        if (TestOutput!=null)
          TestOutput.WriteLine($"Test {i + 1}:");
        var testInputItem = TestInputData[i];
        TestItem testOutputItem;
        var oneTestResult = RunOneTest(testInputItem, out testOutputItem);
        testOutputData.Add(testOutputItem);
        if (oneTestResult == false)
          wholeTestResult = false;
        else if (oneTestResult == true)
        {
          if (wholeTestResult != false)
            wholeTestResult = true;
        }
      }
      return wholeTestResult;
    }

    /// <summary>
    /// Creates testInputData collection from testPatterns.
    /// </summary>
    static void CreateTestData()
    {
      TestInputData = new TestData();
      foreach (var pattern in testPatterns)
      {
        var item = new TestItem { Pattern = pattern };
        TestInputData.Add(item);
      }
    }

    /// <summary>
    /// Compares two regex items for equality. Used in tests.
    /// </summary>
    /// <param name="items1"></param>
    /// <param name="items2"></param>
    /// <returns></returns>
    static bool ItemsAreEqual(RegExItems items1, RegExItems items2)
    {
      if (items1.Count == items2.Count)
      {
        for (int i = 0; i < items1.Count; i++)
        {
          var item1 = items1[i];
          var item2 = items2[i];
          if (item1.GetType() != item2.GetType())
            return false;
          if (!item1.Equals(item2))
            return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Saves testOutputData in file depending on suggestion and user decision
    /// </summary>
    /// <param name="filename">name of output file</param>
    /// <param name="shouldSave">if is null then save depends on user decision</param>
    /// <param name="message">message to ask user for decision</param>
    /// <returns></returns>
    static bool TrySaveTestData(string filename, bool? shouldSave, string message)
    {
      bool testDataWritten = false;
      if (filename != null)
      {
        if (shouldSave == null)
        {
          Console.Write(message+" (Y/N)");
          var key1 = Console.ReadKey();
          Console.WriteLine();
          if (key1.Key == ConsoleKey.Y)
          {
            WriteTestData(filename);
            TestOutput.WriteLine($"written tests: {testOutputData.Count}");
            TestOutput.WriteLine("");
            testDataWritten = true;
          }
        }
        else if (shouldSave == true)
        {
          TestOutput.WriteLine($"Writting test data to file \"{filename}\\");
          WriteTestData(filename);
          TestOutput.WriteLine($"written tests: {testOutputData.Count}");
          TestOutput.WriteLine("");
          testDataWritten = true;
        }
      }
      return testDataWritten;
    }

    /// <summary>
    /// Depending on user decision tries to run and add more tests to testOutputData
    /// </summary>
    /// <returns></returns>
    static bool TryAddMoreTests()
    {
      bool added = false;
      bool shouldRepeat = true;
      while (shouldRepeat)
      {
        Console.Write("Do you want do run new test? (Y/N) ");
        var key2 = Console.ReadKey();
        TestOutput.WriteLine();
        if (key2.Key == ConsoleKey.Y)
        {
          Console.Write("enter test pattern:");
          var pattern = Console.ReadLine();
          if (pattern == "")
            break;
          var testInputItem = new TestItem { Pattern = pattern };
          TestItem testOutputItem;
          var oneTestResult = RunOneTest(testInputItem, out testOutputItem);
          TestOutput.WriteLine("");
          Console.Write("Do you accept test result? (Y/N) ");
          var key1 = Console.ReadKey();
          Console.WriteLine();
          if (key1.Key == ConsoleKey.Y)
          {
            testOutputData.Add(testOutputItem);
            added = true;
          }
        }
        else
          shouldRepeat = false;
      }
      return added;
    }

    static void ReadTestData(string filename)
    {
      using (var reader = File.OpenText(filename))
      {
        TestInputData = (TestData)XamlServices.Load(reader);
      }
    }

    static void WriteTestData(string filename)
    {
      using (var writer = File.CreateText(filename))
      {
        XamlServices.Save(writer, testOutputData);
      }
    }


    static void Dump(TextWriter writer, RegExItems items, int level = 0)
    {
      foreach (var item in items)
      {
        var s = $"{item.GetType().Name}: {item}";
        writer.WriteLine(new string(' ', level * 2) + s);
        if (item is RegExGroup group)
        {
          if (group.Name != null)
            TestOutput.WriteLine(new string(' ', (level + 1) * 2) + $"Name=\"{group.Name}\"");
          Dump(writer, group.Items, level + 1);
        }
        else
        if (item is RegExCharset charset)
          Dump(writer, charset.Items, level + 1);
      }
    }
  }
}
