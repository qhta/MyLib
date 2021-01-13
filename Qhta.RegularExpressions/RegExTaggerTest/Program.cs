using Qhta.RegularExpressions;

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
      IsInputFileProtected = 1,
      IsOutputFileSpecified = 2,
      TestOneCharPatterns = 4,
      TestTwoCharsPatterns = 8,
      BreakAfterFirstFailure = 16,
      DontShowPassedTestResults = 32,
    }

    const string QuantifierChars = "?+*";
    const string AnchorChars = "^$";
    const string OpeningChars = "([{";
    const string Anchor2ndChars = "AZzGbB";
    const string Domain2ndChars = "DdWwSs";


    static List<string> testPatterns = new List<string>
    {
      //@"\b(\w+?)\s\1\b",
      //@"((\w+)[\s.])+.",
      //@"(?<name_012>subexpression[a-zA-Z0-9])",
    };

    static TestData TestInputData, testOutputData;
    static RegExTagger Tagger = new RegExTagger();
    static TextWriter TestOutput = Console.Out;
    static string TestInputFileName;
    static string TestOutputFileName;
    static TestOptions TestRunOptions = 0;

    static void Main(string[] args)
    {
      ReadArgs(args, out TestInputFileName, out TestOutputFileName, out TestRunOptions);
      var wholeTestResult = RunTests();
      if (wholeTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
      {
        Console.Write("Press any key to close test");
        Console.ReadKey();
      }
    }

    static bool? RunTests()
    {
      if (TestInputFileName != null)
      {
        TestOutput.WriteLine($"reading test data from file \"{TestInputFileName}\"");
        ReadTestData(TestInputFileName);
        TestOutput.WriteLine($"read tests: {TestInputData.Count}");
        TestOutput.WriteLine($"");
      }
      else
        CreateTestData();
      testOutputData = new TestData();

      bool? wholeTestResult = null;
      bool? partTestResult = null;

      if (TestRunOptions.HasFlag(TestOptions.TestOneCharPatterns))
      {
        partTestResult = TestOneCharPatterns();
        SetTestResult(ref wholeTestResult, partTestResult);
        if (wholeTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return wholeTestResult;
        ShowTestResult("One-char patterns test result is {0}", partTestResult);
      }

      if (TestRunOptions.HasFlag(TestOptions.TestTwoCharsPatterns))
      {
        partTestResult = TestTwoCharsPatterns();
        SetTestResult(ref wholeTestResult, partTestResult);
        if (wholeTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return wholeTestResult;
        ShowTestResult("Two-chars patterns test result is {0}", partTestResult);
      }

      if (TestInputData.Count > 0)
      {
        partTestResult = RunTestInputData();
        SetTestResult(ref wholeTestResult, partTestResult);
        if (wholeTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return wholeTestResult;
        ShowTestResult("Input data test result is {0}", partTestResult);
      }

      bool? shouldSave = false;
      if (wholeTestResult == false)
      {
        if (TestRunOptions.HasFlag(TestOptions.IsOutputFileSpecified))
          shouldSave = true;
        if (!TestRunOptions.HasFlag(TestOptions.IsInputFileProtected))
          shouldSave = null;
      }
      else if (wholeTestResult == true)
      {
        if (TestRunOptions.HasFlag(TestOptions.IsOutputFileSpecified))
          shouldSave = true;
      }
      TrySaveTestData(TestOutputFileName, shouldSave, $"Test results differ from expected. Do you want to write test data to file \"{TestOutputFileName}\"?");

      bool moreTestsAdded = TryAddMoreTests();

      if (moreTestsAdded)
        TrySaveTestData(TestOutputFileName, null, $"You have added some test. Do you want to save all test data to file \"{TestOutputFileName}\"?");
      return wholeTestResult;
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
        ShowUsage();
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
              case 'b':
                options |= TestOptions.BreakAfterFirstFailure;
                break;
              case 'd':
                options |= TestOptions.DontShowPassedTestResults;
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
    static void ShowUsage()
    {
      TestOutput.WriteLine($"arguments: [testInputFileName [/o testOutputFileName]]");
      TestOutput.WriteLine($"options:");
      TestOutput.WriteLine($"\t/1 - test one-char patterns");
      TestOutput.WriteLine($"\t/2 - test two-chars patterns");
      TestOutput.WriteLine($"\t/i - intput file is input-only (is not rewritten)");
      TestOutput.WriteLine($"\t/b - break on first failure");
      TestOutput.WriteLine($"\t/d - don't show passed results");
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
          newItem = new RegExGroup { Start = 0, Str = pattern, Tag = RegExTag.Subexpression, Status = RegExStatus.Unfinished };
        else
        if (ch == ')')
          newItem = new RegExGroup { Start = 0, Str = pattern, Tag = RegExTag.Subexpression, Status = RegExStatus.Error };
        else if (ch == '.')
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.DotChar, Status = RegExStatus.OK };
        else if (ch == '[')
          newItem = new RegExCharset { Start = 0, Str = pattern, Tag = RegExTag.CharSet, Status = RegExStatus.Unfinished };
        else if (ch == '\\')
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.EscapedChar, Status = RegExStatus.Unfinished };
        else if (QuantifierChars.Contains(ch))
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.Quantifier, Status = RegExStatus.Warning };
        else if (AnchorChars.Contains(ch))
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.AnchorControl, Status = RegExStatus.OK };
        else if (ch == '|')
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.AltChar, Status = RegExStatus.Warning };
        else
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.LiteralChar, Status = RegExStatus.OK };
        testInputItem.Items.Add(newItem);
        testInputItem.Result = newItem.Status;
        TestItem testOutputItem;
        var singleTestResult = RunSingleTest(testInputItem, out testOutputItem);
        testOutputData.Add(testOutputItem);
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
      }
      return wholeTestResult;
    }

    /// <summary>
    /// Tests two-chars patterns. No test input data needed. No test output data produced.
    /// </summary>
    /// <returns></returns>
    static bool? TestTwoCharsPatterns()
    {
      bool? wholeTestResult = null;
      for (char firstChar = '('; firstChar <= '\x7E'; firstChar++)
      {
        bool? singleTestResult;
        if (firstChar == '\\')
          singleTestResult = TestEscapedChars();
        else if (firstChar == '(')
          singleTestResult = Test2CharSubexpression();
        else if (firstChar == ')')
          singleTestResult = TestInvalidClosingParen();
        else if (firstChar == '[')
          singleTestResult = Test2CharCharset();
        else if (firstChar == '|')
          singleTestResult = TestAlt2Char();
        else if (QuantifierChars.Contains(firstChar))
          singleTestResult = Test1stCharQuantifiers(firstChar);
        else if (AnchorChars.Contains(firstChar))
          singleTestResult = Test1stCharAnchors(firstChar);
        else
          singleTestResult = TestAnyTwoChars(firstChar);

        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
      }
      return wholeTestResult;
    }

    /// <summary>
    /// Tests two-chars patterns where the first char is '\'.
    /// </summary>
    /// <returns></returns>
    static bool? TestEscapedChars()
    {
      bool? wholeTestResult = null;
      for (char nextChar = ' '; nextChar <= '\x7E'; nextChar++)
      {
        var pattern = new string(new char[] { '\\', nextChar });
        var testInputItem = new TestItem { Pattern = pattern, Result = RegExStatus.OK };
        RegExItem newItem;
        if (nextChar >= '0' && nextChar <= '9')
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.BackRef, Status = RegExStatus.Warning };
        else if (Anchor2ndChars.Contains(nextChar))
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.AnchorControl, Status = RegExStatus.OK };
        else if (Domain2ndChars.Contains(nextChar))
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.DomainSeq, Status = RegExStatus.OK };
        else if (nextChar == 'P' || nextChar == 'p')
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.UnicodeCategorySeq, Status = RegExStatus.Unfinished };
        else if (nextChar == 'c')
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.ControlCharSeq, Status = RegExStatus.Unfinished };
        else if (nextChar == 'u')
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.UnicodeSeq, Status = RegExStatus.Unfinished };
        else if (nextChar == 'x')
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.HexadecimalSeq, Status = RegExStatus.Unfinished };
        else
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.EscapedChar, Status = RegExStatus.OK };
        testInputItem.Items.Add(newItem);
        testInputItem.Result = newItem.Status;
        TestItem testOutputItem;
        var singleTestResult = RunSingleTest(testInputItem, out testOutputItem);
        testOutputData.Add(testOutputItem);
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
      }
      return wholeTestResult;
    }

    /// <summary>
    /// Tests two-chars patterns where the first char is '('.
    /// </summary>
    /// <returns></returns>
    static bool? Test2CharSubexpression()
    {
      bool? wholeTestResult = null;
      for (char nextChar = ' '; nextChar <= '\x7E'; nextChar++)
      {
        var pattern = new string(new char[] { '(', nextChar });
        var testInputItem = new TestItem { Pattern = pattern, Result = RegExStatus.OK };
        RegExGroup group = new RegExGroup { Start = 0, Str = pattern, Tag = RegExTag.Subexpression, Status = nextChar==')' ? RegExStatus.OK : RegExStatus.Unfinished };
        testInputItem.Items.Add(group);
        testInputItem.Result = group.Status;
        if (nextChar == '?')
        {
          pattern = new string(nextChar, 1);
          var subItem = new RegExItem { Start = 1, Str = pattern, Tag = RegExTag.GroupControlChar, Status = RegExStatus.OK };
            group.Items.Add(subItem);
        }
        else if (nextChar != ')')
        {
          pattern = new string(nextChar, 1);
          var subResult = Tagger.TryParsePattern(pattern);
          foreach (var subItem in Tagger.Items)
          {
            subItem.Start += 1;
            group.Items.Add(subItem);
          }
        }
        TestItem testOutputItem;
        var singleTestResult = RunSingleTest(testInputItem, out testOutputItem);
        testOutputData.Add(testOutputItem);
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
      }
      return wholeTestResult;
    }

    /// <summary>
    /// Tests two-chars patterns where the first char is ')'.
    /// </summary>
    /// <returns></returns>
    static bool? TestInvalidClosingParen()
    {
      bool? wholeTestResult = null;
      for (char nextChar = ' '; nextChar <= '\x7E'; nextChar++)
      {
        var pattern = new string(new char[] { ')', nextChar });
        var testInputItem = new TestItem { Pattern = pattern, Result = RegExStatus.OK };
        RegExItem newItem = new RegExGroup { Start = 0, Str = new string(')',1), Tag = RegExTag.Subexpression, Status = RegExStatus.Error };
        testInputItem.Items.Add(newItem);
        testInputItem.Result = newItem.Status;
        TestItem testOutputItem;
        var singleTestResult = RunSingleTest(testInputItem, out testOutputItem);
        testOutputData.Add(testOutputItem);
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
      }
      return wholeTestResult;
    }

    /// <summary>
    /// Tests two-chars patterns where the first char is '['.
    /// </summary>
    /// <returns></returns>
    static bool? Test2CharCharset()
    {
      bool? wholeTestResult = null;
      for (char nextChar = ' '; nextChar <= '\x7E'; nextChar++)
      {
        var pattern = new string(new char[] { '[', nextChar });
        var testInputItem = new TestItem { Pattern = pattern, Result = RegExStatus.OK };
        RegExCharset charset = new RegExCharset { Start = 0, Str = pattern, Tag = RegExTag.CharSet, Status = nextChar == ']' ? RegExStatus.Error : RegExStatus.Unfinished };
        testInputItem.Items.Add(charset);
        testInputItem.Result = charset.Status;
        if (nextChar == '^')
        {
          pattern = new string(nextChar, 1);
          var subItem = new RegExItem { Start = 1, Str = pattern, Tag = RegExTag.CharSetControlChar, Status = RegExStatus.OK };
          charset.Items.Add(subItem);
        }
        else if (nextChar == '[')
        {
          pattern = new string(nextChar, 1);
          var subItem = new RegExCharset { Start = 1, Str = pattern, Tag = RegExTag.CharSet, Status = RegExStatus.Unfinished };
          charset.Items.Add(subItem);
        }
        else if (nextChar == '\\')
        {
          pattern = new string(nextChar, 1);
          var subItem = new RegExItem { Start = 1, Str = pattern, Tag = RegExTag.EscapedChar, Status = RegExStatus.Unfinished };
          charset.Items.Add(subItem);
        }
        else if (nextChar != ']')
        {
          pattern = new string(nextChar, 1);
          var subItem = new RegExItem { Start = 1, Str = pattern, Tag = RegExTag.LiteralChar, Status = RegExStatus.OK };
          charset.Items.Add(subItem);
        }
        TestItem testOutputItem;
        var singleTestResult = RunSingleTest(testInputItem, out testOutputItem);
        testOutputData.Add(testOutputItem);
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
      }
      return wholeTestResult;
    }

    /// <summary>
    /// Tests two-chars patterns where the first char is '|'.
    /// </summary>
    /// <returns></returns>
    static bool? TestAlt2Char()
    {
      bool? wholeTestResult = null;
      for (char nextChar = ' '; nextChar <= '\x7E'; nextChar++)
      {
        var pattern = new string(new char[] { '|', nextChar });
        var testInputItem = new TestItem { Pattern = pattern, Result = RegExStatus.OK };
        RegExItem firstItem = new RegExItem { Start = 0, Str = new string('|', 1), Tag = RegExTag.AltChar, Status = RegExStatus.Warning };
        testInputItem.Items.Add(firstItem);
        var nextResult = Tagger.TryParsePattern(new string(nextChar, 1));
        testInputItem.Result = RegExTools.Max(firstItem.Status, nextResult);
        foreach (var nextItem in Tagger.Items)
        {
          nextItem.Start += 1;
          testInputItem.Items.Add(nextItem);
        }

        TestItem testOutputItem;
        var singleTestResult = RunSingleTest(testInputItem, out testOutputItem);
        testOutputData.Add(testOutputItem);
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
      }
      return wholeTestResult;
    }

    /// <summary>
    /// Tests two-chars patterns where the first char is '?', '+' or '*'.
    /// </summary>
    /// <returns></returns>
    static bool? Test1stCharQuantifiers(char firstChar)
    {
      bool? wholeTestResult = null;
      for (char nextChar = ' '; nextChar <= '\x7E'; nextChar++)
      {
        var pattern = new string(new char[] { firstChar, nextChar });
        var testInputItem = new TestItem { Pattern = pattern, Result = RegExStatus.OK };
        if (nextChar == '?')
        {
          RegExItem newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.Quantifier, Status = RegExStatus.Warning };
          testInputItem.Items.Add(newItem);
          testInputItem.Result = newItem.Status;
        }
        else
        {
          RegExItem firstItem = new RegExItem { Start = 0, Str = new string(firstChar,1), Tag = RegExTag.Quantifier, Status = RegExStatus.Warning };
          testInputItem.Items.Add(firstItem);
          var nextResult = Tagger.TryParsePattern(new string(nextChar, 1));
          testInputItem.Result = RegExTools.Max(firstItem.Status, nextResult);
          foreach (var nextItem in Tagger.Items)
          {
            nextItem.Start += 1;
            testInputItem.Items.Add(nextItem);
          }
        }
        TestItem testOutputItem;
        var singleTestResult = RunSingleTest(testInputItem, out testOutputItem);
        testOutputData.Add(testOutputItem);
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
      }
      return wholeTestResult;
    }

    /// <summary>
    /// Tests two-chars patterns where the first char is '^' or '$'.
    /// </summary>
    /// <returns></returns>
    static bool? Test1stCharAnchors(char firstChar)
    {
      bool? wholeTestResult = null;
      for (char nextChar = ' '; nextChar <= '\x7E'; nextChar++)
      {
        var pattern = new string(new char[] { firstChar, nextChar });
        var testInputItem = new TestItem { Pattern = pattern, Result = RegExStatus.OK };
        if (QuantifierChars.Contains(nextChar))
        {
          RegExItem firstItem = new RegExItem { Start = 0, Str = new string(firstChar,1), Tag = RegExTag.AnchorControl, Status = RegExStatus.OK };
          testInputItem.Items.Add(firstItem);
          testInputItem.Result = firstItem.Status;
          RegExItem nextItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.Quantifier, Status = RegExStatus.OK };
          testInputItem.Items.Add(nextItem);
        }
        else
        {
          RegExItem firstItem = new RegExItem { Start = 0, Str = new string(firstChar, 1), Tag = RegExTag.AnchorControl, Status = RegExStatus.OK };
          testInputItem.Items.Add(firstItem);
          var nextResult = Tagger.TryParsePattern(new string(nextChar, 1));
          testInputItem.Result = RegExTools.Max(firstItem.Status, nextResult);
          foreach (var nextItem in Tagger.Items)
          {
            nextItem.Start += 1;
            testInputItem.Items.Add(nextItem);
          }
        }
        TestItem testOutputItem;
        var singleTestResult = RunSingleTest(testInputItem, out testOutputItem);
        testOutputData.Add(testOutputItem);
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
      }
      return wholeTestResult;
    }

    /// <summary>
    /// Tests two-chars patterns where with first char predefined
    /// </summary>
    /// <returns></returns>
    static bool? TestAnyTwoChars(char firstChar)
    {
      bool? wholeTestResult = null;
      for (char nextChar = ' '; nextChar <= '\x7E'; nextChar++)
      {
        var pattern = new string(new char[] { firstChar, nextChar });
        var testInputItem = new TestItem { Pattern = pattern, Result = RegExStatus.OK };
        RegExItem newItem;
        if (OpeningChars.Contains(nextChar) || QuantifierChars.Contains(nextChar) || AnchorChars.Contains(nextChar) 
          || nextChar == ')' || nextChar == '\\')
        {
          var tag = (firstChar == '.') ? RegExTag.DotChar : RegExTag.LiteralChar;
          newItem = new RegExItem { Start = 0, Str = new string(firstChar,1), Tag = tag, Status = RegExStatus.OK };
          testInputItem.Items.Add(newItem);
          var nextResult = Tagger.TryParsePattern(new string(nextChar, 1));
          testInputItem.Result = RegExTools.Max(newItem.Status, nextResult);
          foreach (var nextItem in Tagger.Items)
          {
            nextItem.Start += 1;
            if (QuantifierChars.Contains(nextChar))
            {
              nextItem.Status = RegExStatus.OK;
              testInputItem.Result = RegExStatus.OK;
            }
            testInputItem.Items.Add(nextItem);
          }
        }
        else if (nextChar == '|')
        {
          var tag = (firstChar == '.') ? RegExTag.DotChar : RegExTag.LiteralChar; 
          newItem = new RegExItem { Start = 0, Str = new string(firstChar, 1), Tag = tag, Status = RegExStatus.OK };
          testInputItem.Items.Add(newItem);
          newItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.AltChar, Status = RegExStatus.Warning };
          testInputItem.Items.Add(newItem);
          testInputItem.Result = newItem.Status;
        }
        else
        {
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.LiteralString, Status = RegExStatus.OK };
          testInputItem.Items.Add(newItem);
          testInputItem.Result = newItem.Status;
        }
        TestItem testOutputItem;
        var singleTestResult = RunSingleTest(testInputItem, out testOutputItem);
        testOutputData.Add(testOutputItem);
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
      }
      return wholeTestResult;
    }

    /// <summary>
    /// Runs a single test of one-char pattern
    /// </summary>
    /// <param name="testInputItem"></param>
    /// <param name="testOutputItem"></param>
    /// <returns></returns>
    static bool? RunSingleTest(TestItem testInputItem, out TestItem testOutputItem)
    {
      var pattern = testInputItem.Pattern;
      TestOutput.WriteLine($"pattern = \"{pattern}\"");
      testOutputItem = new TestItem { Pattern = pattern };
      testOutputItem.Result = Tagger.TryParsePattern(pattern);
      testOutputItem.Items = Tagger.Items;
      bool? singleTestResult = null;
      if (testInputItem.Result != null)
      {
        singleTestResult = (testInputItem.Result == testOutputItem.Result);
        if (singleTestResult == true)
          singleTestResult = (testInputItem.Items == null) ||
          ItemsAreEqual(testInputItem.Items, testOutputItem.Items);
        if (singleTestResult == true)
          TestOutput.WriteLine($"test PASSED");
        else if (singleTestResult == false)
          TestOutput.WriteLine($"test FAILED");
      }
      if (singleTestResult == false)
      {
        TestOutput.WriteLine($"Expected:");
        ShowTestItem(testInputItem);
        TestOutput.WriteLine($"Obtained:");
        ShowTestItem(testOutputItem);
      }
      else if (singleTestResult != true || !TestRunOptions.HasFlag(TestOptions.DontShowPassedTestResults))
        ShowTestItem(testOutputItem);
      TestOutput.WriteLine($"");
      TestOutput.Flush();
      return singleTestResult;
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
        if (TestOutput != null)
          TestOutput.WriteLine($"Test {i + 1}:");
        var testInputItem = TestInputData[i];
        TestItem testOutputItem;
        var singleTestResult = RunSingleTest(testInputItem, out testOutputItem);
        testOutputData.Add(testOutputItem);
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
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
          Console.Write(message + " (Y/N)");
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
          var singleTestResult = RunSingleTest(testInputItem, out testOutputItem);
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

    static void ShowTestItem(TestItem item)
    {
      TestOutput.WriteLine($"  Result = {item.Result}");
      Dump(TestOutput, item.Items, 1);
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

    static void SetTestResult(ref bool? wholeTestResult, bool? partTestResult)
    {
      if (wholeTestResult != false && partTestResult != null)
      {
        if (wholeTestResult == null || partTestResult == false)
          wholeTestResult = partTestResult;
      }
    }

    static void ShowTestResult(string message, bool? testResult)
    {
      TestOutput.WriteLine();
      if (testResult == true)
        TestOutput.WriteLine(String.Format(message, "PASSED"));
      else
      if (testResult == false)
        TestOutput.WriteLine(String.Format(message, "FAILED"));
      else
        TestOutput.WriteLine(String.Format(message, "UNDECIDED"));
      TestOutput.WriteLine();
    }
  }
}
