using Qhta.RegularExpressions;
using Qhta.RegularExpressions.Descriptions;

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
      BreakAfterFirstFailure = 1,
      SuppressPassedTestResults = 2,
      IsInputFileProtected = 4,
      IsOutputFileSpecified = 8,
      TestOneCharPatterns = 16,
      TestTwoCharsPatterns = 32,
      TestOctalCodePatterns = 64,
      TestHexadecimalPatterns = 128,
      TestUnicodePatterns = 0x100,
      TestControlCharPatterns = 0x200,
      TestUnicodeCategories = 0x400,
      TestCharsetPatterns = 0x800,
      TestGroupingPatterns = 0x1000,
    }

    const string QuantifierChars = "?+*";
    const string AnchorChars = "^$";
    const string OpeningChars = "([{";
    const string Anchor2ndChars = "AZzGbB";
    const string Domain2ndChars = "DdWwSs";
    const string UnicodeCategoryFirstLetters = "LNMPSZC";
    const string HexadecimalTestChars = "09AFafg";
    static string[] UnicodeCategories = new string[]
    {
      "Cc", "Cf", "Cn", "Co", "Cs", "Ll", "Lm", "Lo", "Lt", "Lu", "Mc", "Me", "Mn", "Nd", "Nl", "No",
      "Pc", "Pd", "Pe", "Pf", "Pi", "Po", "Ps", "Sc", "Sk", "Sm", "So", "Zl", "Zp", "Zs",
    };

    static string[] UnicodeBlockNames = new string[]
    {
      "IsBasicLatin", "IsLatin-1Supplement", "IsLatinExtended-A", "IsLatinExtended-B", "IsIPAExtensions", "IsSpacingModifierLetters", "IsCombiningDiacriticalMarks",
      "IsGreek", /* or */ "IsGreekandCoptic",
      "IsCyrillic", "IsCyrillicSupplement", "IsArmenian", "IsHebrew", "IsArabic", "IsSyriac", "IsThaana", "IsDevanagari", "IsBengali", "IsGurmukhi", "IsGujarati", "IsOriya",
      "IsTamil", "IsTelugu", "IsKannada", "IsMalayalam", "IsSinhala", "IsThai", "IsLao", "IsTibetan", "IsMyanmar", "IsGeorgian", "IsHangulJamo", "IsEthiopic",
      "IsCherokee", "IsUnifiedCanadianAboriginalSyllabics", "IsOgham", "IsRunic", "IsTagalog", "IsHanunoo", "IsBuhid", "IsTagbanwa", "IsKhmer", "IsMongolian", "IsLimbu",
      "IsTaiLe", "IsKhmerSymbols", "IsPhoneticExtensions", "IsLatinExtendedAdditional", "IsGreekExtended",
      "IsGeneralPunctuation", "IsSuperscriptsandSubscripts", "IsCurrencySymbols",
      "IsCombiningDiacriticalMarksforSymbols", /* or */ "IsCombiningMarksforSymbols",
      "IsLetterlikeSymbols", "IsNumberForms", "IsArrows", "IsMathematicalOperators", "IsMiscellaneousTechnical", "IsControlPictures", "IsOpticalCharacterRecognition",
      "IsEnclosedAlphanumerics", "IsBoxDrawing", "IsBlockElements", "IsGeometricShapes", "IsMiscellaneousSymbols", "IsDingbats", "IsMiscellaneousMathematicalSymbols-A",
      "IsSupplementalArrows-A", "IsBraillePatterns", "IsSupplementalArrows-B", "IsMiscellaneousMathematicalSymbols-B", "IsSupplementalMathematicalOperators",
      "IsMiscellaneousSymbolsandArrows", "IsCJKRadicalsSupplement", "IsKangxiRadicals", "IsIdeographicDescriptionCharacters", "IsCJKSymbolsandPunctuation",
      "IsHiragana", "IsKatakana", "IsBopomofo", "IsHangulCompatibilityJamo", "IsKanbun", "IsBopomofoExtended", "IsKatakanaPhoneticExtensions",
      "IsEnclosedCJKLettersandMonths", "IsCJKCompatibility", "IsCJKUnifiedIdeographsExtensionA", "IsYijingHexagramSymbols", "IsCJKUnifiedIdeographs",
      "IsYiSyllables", "IsYiRadicals", "IsHangulSyllables", "IsHighSurrogates", "IsHighPrivateUseSurrogates", "IsLowSurrogates",
      "IsPrivateUse", /* or */ "IsPrivateUseArea",
      "IsCJKCompatibilityIdeographs", "IsAlphabeticPresentationForms", "IsArabicPresentationForms-A", "IsVariationSelectors", "IsCombiningHalfMarks",
      "IsCJKCompatibilityForms", "IsSmallFormVariants", "IsArabicPresentationForms-B", "IsHalfwidthandFullwidthForms", "IsSpecials",
    };
    static string ValidChars;

    static TestData TestInputData, testOutputData;
    static RegExTagger Tagger = new RegExTagger();
    static TextWriter TestOutput = Console.Out;
    static string TestInputFileName;
    static string TestOutputFileName;
    static TestOptions TestRunOptions = 0;

    static void Main(string[] args)
    {
      for (char ch = ' '; ch <= '\x7E'; ch++)
      {
        if (!(ch > 'a' && ch < 'z' || ch > '0' && ch < '9' || ch > 'A' && ch < 'Z'))
          ValidChars += ch;
      }

      bool? wholeTestResult = null;
      if (args.Length == 0)
      {
        ShowUsage();
      }
      else
      {
        if (!ReadArgs(args, out TestInputFileName, out TestOutputFileName, out TestRunOptions))
        {
          ShowUsage();
          wholeTestResult = false;
        }
      }
      if (wholeTestResult != false)
        wholeTestResult = RunTests();
      if (wholeTestResult == false)
      {
        Console.Write("\nPress any key to close test");
        Console.ReadKey();
      }
    }


    /// <summary>
    /// Writes usage help messages to console
    /// </summary>
    static void ShowUsage()
    {
      TestOutput.WriteLine($"\narguments: [testInputFileName [/o testOutputFileName]]");
      TestOutput.WriteLine($"options:");
      TestOutput.WriteLine($"\t/b - break on first failure");
      TestOutput.WriteLine($"\t/s - suppress passed test results");
      TestOutput.WriteLine($"\t/i - intput file is input-only (is not rewritten)");
      TestOutput.WriteLine($"\t/o - output file name is specified");
      TestOutput.WriteLine($"\t/1 - test one-char patterns");
      TestOutput.WriteLine($"\t/2 - test two-chars patterns");
      TestOutput.WriteLine($"\t/8 - test octal code patterns");
      TestOutput.WriteLine($"\t/x - test hexadecimal code patterns");
      TestOutput.WriteLine($"\t/u - test Unicode patterns");
      TestOutput.WriteLine($"\t/c - test control char patterns");
      TestOutput.WriteLine($"\t/p - test Unicode categories");
      TestOutput.WriteLine($"\t/[ - test charset patterns");
      TestOutput.WriteLine($"\t/( - test grouping patterns");
      TestOutput.WriteLine($"");
      TestOutput.WriteLine($"TestInputFile can be of the type:");
      TestOutput.WriteLine($"\txml - the whole test data is read with test results");
      TestOutput.WriteLine($"\ttxt - only patterns are read (each in a separate line)");
      TestOutput.WriteLine($"TestOutputFile is always of type xml. Is is written if the overall test result is passed.");
      TestOutput.WriteLine($"If the input test file is specified and is not preceded by the \\i option, and the output file ");
      TestOutput.WriteLine($"is not specified with the \\o option, then the input file may be replaced with new test results.");
      TestOutput.WriteLine();
    }

    /// <summary>
    /// Reads filenames and options from args string collection.
    /// </summary>
    /// <param name="args"></param>
    static bool ReadArgs(string[] args, out string inputFileName, out string outputFileName, out TestOptions options)
    {
      inputFileName = null;
      outputFileName = null;
      options = 0;
      if (args.Length > 0)
      {
        for (int i = 0; i < args.Length; i++)
        {
          //TestMonitor.WriteLine($"args[{i}]: {args[i]}");
          var arg = args[i];
          if (arg[0] == '/')
          {
            switch (arg[1])
            {
              case 'b':
                options |= TestOptions.BreakAfterFirstFailure;
                break;
              case 's':
                options |= TestOptions.SuppressPassedTestResults;
                break;
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
              case '8':
                options |= TestOptions.TestOctalCodePatterns;
                break;
              case 'x':
                options |= TestOptions.TestHexadecimalPatterns;
                break;
              case 'u':
                options |= TestOptions.TestUnicodePatterns;
                break;
              case 'c':
                options |= TestOptions.TestControlCharPatterns;
                break;
              case 'p':
                options |= TestOptions.TestUnicodeCategories;
                break;
              case '[':
                options |= TestOptions.TestCharsetPatterns;
                break;
              case '(':
                options |= TestOptions.TestGroupingPatterns;
                break;
              default:
                Console.WriteLine($"Unrecognized option \"{arg}\"");
                return false;
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
        if (inputFileName != null)
          if (outputFileName == null && !options.HasFlag(TestOptions.IsInputFileProtected) && Path.GetExtension(inputFileName)?.ToLower() == ".xml")
            outputFileName = inputFileName;
        TestOutput.WriteLine($"");
        return true;
      }
      return false;
    }

    static bool? RunTests()
    {
      if (TestInputFileName != null)
      {
        TestOutput.WriteLine($"reading test data from file \"{TestInputFileName}\"");
        TestInputData = ReadTestData(TestInputFileName);
        if (TestInputData==null || TestInputData.Count==0)
        {
          return null;
        }
        TestOutput.WriteLine($"read tests: {TestInputData.Count}");
        TestOutput.WriteLine($"");
      }
      testOutputData = new TestData();

      bool? wholeTestResult = null;
      bool? partTestResult;

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

      if (TestRunOptions.HasFlag(TestOptions.TestOctalCodePatterns))
      {
        partTestResult = TestOctalCodePatterns();
        SetTestResult(ref wholeTestResult, partTestResult);
        if (wholeTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return wholeTestResult;
        ShowTestResult("Octal code patterns test result is {0}", partTestResult);
      }

      if (TestRunOptions.HasFlag(TestOptions.TestHexadecimalPatterns))
      {
        partTestResult = TestHexadecimalPatterns();
        SetTestResult(ref wholeTestResult, partTestResult);
        if (wholeTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return wholeTestResult;
        ShowTestResult("Hexadecimal code patterns test result is {0}", partTestResult);
      }

      if (TestRunOptions.HasFlag(TestOptions.TestUnicodePatterns))
      {
        partTestResult = TestUnicodePatterns();
        SetTestResult(ref wholeTestResult, partTestResult);
        if (wholeTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return wholeTestResult;
        ShowTestResult("Unicode code patterns test result is {0}", partTestResult);
      }

      if (TestRunOptions.HasFlag(TestOptions.TestControlCharPatterns))
      {
        partTestResult = TestControlCharPatterns();
        SetTestResult(ref wholeTestResult, partTestResult);
        if (wholeTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return wholeTestResult;
        ShowTestResult("Control char patterns test result is {0}", partTestResult);
      }

      if (TestRunOptions.HasFlag(TestOptions.TestUnicodeCategories))
      {
        partTestResult = TestUnicodeCategories();
        SetTestResult(ref wholeTestResult, partTestResult);
        if (wholeTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return wholeTestResult;
        ShowTestResult("Unicode categories test result is {0}", partTestResult);
      }

      if (TestRunOptions.HasFlag(TestOptions.TestCharsetPatterns))
      {
        partTestResult = TestCharsetPatterns();
        SetTestResult(ref wholeTestResult, partTestResult);
        if (wholeTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return wholeTestResult;
        ShowTestResult("Charset patterns test result is {0}", partTestResult);
      }


      if (TestRunOptions.HasFlag(TestOptions.TestGroupingPatterns))
      {
        partTestResult = TestGroupingPatterns();
        SetTestResult(ref wholeTestResult, partTestResult);
        if (wholeTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return wholeTestResult;
        ShowTestResult("Grouping patterns test result is {0}", partTestResult);
      }

      if (TestInputData != null && TestInputData.Count > 0)
      {
        partTestResult = RunTestInputData();
        SetTestResult(ref wholeTestResult, partTestResult);
        if (wholeTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return wholeTestResult;
        ShowTestResult("Input data test result is {0}", partTestResult);
      }

      if (TestOutputFileName != null)
      {
        //if (wholeTestResult != true)
        {
          Console.Write($"Do you want to write test data to file \"{TestOutputFileName}\"? (Y/N)");
          var key1 = Console.ReadKey();
          Console.WriteLine();
          if (key1.Key == ConsoleKey.Y)
          {
            TrySaveTestData(TestOutputFileName);
          }
        }
      }


      bool moreTestsAdded = TryAddMoreTests();

      if (TestOutputFileName != null && moreTestsAdded)
      {
        Console.Write($"You have added some test. Do you want to save all test data to file \"{TestOutputFileName}\"?");
        var key1 = Console.ReadKey();
        Console.WriteLine();
        if (key1.Key == ConsoleKey.Y)
        {
          TrySaveTestData(TestOutputFileName);
        }
      }
      return wholeTestResult;
    }

    #region Test One Char Patterns
    /// <summary>
    /// Tests one-char patterns. No test input data needed. No test output data produced.
    /// </summary>
    /// <returns></returns>
    static bool? TestOneCharPatterns()
    {
      bool? wholeTestResult = null;

      foreach (char ch in ValidChars)
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
          newItem = new RegExCharSet { Start = 0, Str = pattern, Tag = RegExTag.CharSet, Status = RegExStatus.Unfinished };
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
    #endregion

    #region Test Two Chars Patterns
    /// <summary>
    /// Tests two-chars patterns. No test input data needed. No test output data produced.
    /// </summary>
    /// <returns></returns>
    static bool? TestTwoCharsPatterns()
    {
      bool? wholeTestResult = null;
      foreach (char firstChar in ValidChars)
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
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.CharClass, Status = RegExStatus.OK };
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
        RegExGroup group = new RegExGroup { Start = 0, Str = pattern, Tag = RegExTag.Subexpression, Status = nextChar == ')' ? RegExStatus.OK : RegExStatus.Unfinished };
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
        RegExItem newItem = new RegExGroup { Start = 0, Str = new string(')', 1), Tag = RegExTag.Subexpression, Status = RegExStatus.Error };
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
        RegExCharSet charset = new RegExCharSet { Start = 0, Str = pattern, Tag = RegExTag.CharSet, Status = nextChar == ']' ? RegExStatus.Error : RegExStatus.Unfinished };
        testInputItem.Items.Add(charset);
        testInputItem.Result = charset.Status;
        if (nextChar == '^')
        {
          pattern = new string(nextChar, 1);
          var subItem = new RegExItem { Start = 1, Str = pattern, Tag = RegExTag.CharSetControlChar, Status = RegExStatus.OK };
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
          RegExItem firstItem = new RegExItem { Start = 0, Str = new string(firstChar, 1), Tag = RegExTag.Quantifier, Status = RegExStatus.Warning };
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
          RegExItem firstItem = new RegExItem { Start = 0, Str = new string(firstChar, 1), Tag = RegExTag.AnchorControl, Status = RegExStatus.OK };
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
      foreach (char nextChar in ValidChars)
      {
        var pattern = new string(new char[] { firstChar, nextChar });
        var testInputItem = new TestItem { Pattern = pattern, Result = RegExStatus.OK };
        RegExItem newItem;
        if (OpeningChars.Contains(nextChar) || QuantifierChars.Contains(nextChar) || AnchorChars.Contains(nextChar)
          || nextChar == ')' || nextChar == '\\')
        {
          var tag = (firstChar == '.') ? RegExTag.DotChar : RegExTag.LiteralChar;
          newItem = new RegExItem { Start = 0, Str = new string(firstChar, 1), Tag = tag, Status = RegExStatus.OK };
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
    #endregion

    #region Test Octal Code Patterns, Test Hexadecimal Patterns, Test Unicode Patterns, Test Control Char Patterns
    /// <summary>
    /// Tests octal code patterns, i.e \NNN. Note that some sequences are recognized as backreferences.
    /// </summary>
    /// <returns></returns>
    static bool? TestOctalCodePatterns()
    {
      bool? wholeTestResult = null;
      var codes = new List<string>();
      for (int digitsCount = 1; digitsCount <= 3; digitsCount++)
      {
        AddDecimalCodes(codes, "", digitsCount);
      }
      for (int i = 0; i < codes.Count; i++)
      {
        var code = codes[i];
        var pattern = "\\" + code;
        var digitCount = code.Length;
        var testInputItem = new TestItem { Pattern = pattern, Result = RegExStatus.OK };
        RegExItem newItem;
        if (digitCount == 1)
        {
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.BackRef, Status = RegExStatus.Warning };
          testInputItem.Items.Add(newItem);
          testInputItem.Result = newItem.Status;
        }
        else
        if (digitCount == 2)
        {
          newItem = new RegExItem { Start = 0, Str = pattern.Substring(0, 2), Tag = RegExTag.BackRef, Status = RegExStatus.Warning };
          testInputItem.Items.Add(newItem);
          testInputItem.Result = newItem.Status;
          newItem = new RegExItem { Start = 2, Str = pattern.Substring(2, 1), Tag = RegExTag.LiteralChar, Status = RegExStatus.OK };
          testInputItem.Items.Add(newItem);
        }
        else if (code.Contains('8') || code.Contains('9'))
        {
          newItem = new RegExItem { Start = 0, Str = pattern.Substring(0, 2), Tag = RegExTag.BackRef, Status = RegExStatus.Warning };
          testInputItem.Items.Add(newItem);
          testInputItem.Result = newItem.Status;
          newItem = new RegExItem { Start = 2, Str = pattern.Substring(2, 2), Tag = RegExTag.LiteralString, Status = RegExStatus.OK };
          testInputItem.Items.Add(newItem);
        }
        else
        {
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.OctalSeq, Status = RegExStatus.OK };
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

    static void AddDecimalCodes(List<string> codes, string precode, int digitsCount)
    {
      for (char digit = '0'; digit <= '9'; digit++)
      {
        var code = precode + digit;
        codes.Add(code);
        if (code.Length < digitsCount)
          AddDecimalCodes(codes, code, digitsCount);
      }
    }

    /// <summary>
    /// Tests hexadecimal code patterns, i.e \xXX. Note that the sequence "\x" can be tested as two-chars pattern.
    /// </summary>
    /// <returns></returns>
    static bool? TestHexadecimalPatterns()
    {
      bool? wholeTestResult = null;
      var codes = new List<string>();
      for (int digitsCount = 1; digitsCount <= 2; digitsCount++)
      {
        AddHexadecimalCodes(codes, "", digitsCount);
      }
      for (int i = 0; i < codes.Count; i++)
      {
        var code = codes[i];
        var pattern = "\\x" + code;
        var digitCount = code.Length;
        var testInputItem = new TestItem { Pattern = pattern, Result = RegExStatus.OK };
        RegExItem newItem;
        int k;
        if ((k = code.IndexOf('g')) >= 0)
        {
          newItem = new RegExItem { Start = 0, Str = pattern.Substring(0, k + 3), Tag = RegExTag.HexadecimalSeq, Status = RegExStatus.Error };
          testInputItem.Items.Add(newItem);
          testInputItem.Result = newItem.Status;
        }
        else if (digitCount == 1)
        {
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.HexadecimalSeq, Status = RegExStatus.Unfinished };
          testInputItem.Items.Add(newItem);
          testInputItem.Result = newItem.Status;
        }
        else
        {
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.HexadecimalSeq, Status = RegExStatus.OK };
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
    /// Tests Unicode code patterns, i.e \uXXXX. Note that the sequence "\u" can be tested as two-chars pattern.
    /// </summary>
    /// <returns></returns>
    static bool? TestUnicodePatterns()
    {
      bool? wholeTestResult = null;
      var codes = new List<string>();
      for (int digitsCount = 1; digitsCount <= 4; digitsCount++)
      {
        AddHexadecimalCodes(codes, "", digitsCount);
      }
      for (int i = 0; i < codes.Count; i++)
      {
        var code = codes[i];
        var pattern = "\\u" + code;
        var digitCount = code.Length;
        var testInputItem = new TestItem { Pattern = pattern, Result = RegExStatus.OK };
        RegExItem newItem;
        int k;
        if ((k = code.IndexOf('g')) >= 0)
        {
          newItem = new RegExItem { Start = 0, Str = pattern.Substring(0, k + 3), Tag = RegExTag.UnicodeSeq, Status = RegExStatus.Error };
          testInputItem.Items.Add(newItem);
          testInputItem.Result = newItem.Status;
        }
        else if (digitCount < 4)
        {
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.UnicodeSeq, Status = RegExStatus.Unfinished };
          testInputItem.Items.Add(newItem);
          testInputItem.Result = newItem.Status;
        }
        else
        {
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.UnicodeSeq, Status = RegExStatus.OK };
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

    static void AddHexadecimalCodes(List<string> codes, string precode, int digitsCount)
    {
      foreach (char digit in HexadecimalTestChars)
      {
        var code = precode + digit;
        codes.Add(code);
        if (code.Length < digitsCount)
          AddHexadecimalCodes(codes, code, digitsCount);
      }
    }

    /// <summary>
    /// Tests control char code patterns, i.e \cX. Note that the sequence "\c" can be tested as two-chars pattern.
    /// </summary>
    /// <returns></returns>
    static bool? TestControlCharPatterns()
    {
      bool? wholeTestResult = null;
      var codes = new List<string>();
      for (int i = 0; i <= 32; i++)
      {
        codes.Add(new string((char)(i + '@'), 1));
      }
      for (char ch = 'a'; ch <= 'z'; ch++)
      {
        codes.Add(new string(ch, 1));
      }
      for (int i = 0; i < codes.Count; i++)
      {
        var code = codes[i];
        var pattern = "\\c" + code;
        var testInputItem = new TestItem { Pattern = pattern, Result = RegExStatus.OK };
        RegExItem newItem;
        if (code[0] == '`')
        {
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.ControlCharSeq, Status = RegExStatus.Error };
          testInputItem.Items.Add(newItem);
          testInputItem.Result = newItem.Status;
        }
        else
        {
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.ControlCharSeq, Status = RegExStatus.OK };
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
    #endregion

    #region Test Unicode Categories

    /// <summary>
    /// Tests unicode categories patterns, i.e \p{ctg} && /P{ctg}. Note that the sequence "\p" can be tested as two-chars pattern.
    /// </summary>
    /// <returns></returns>
    static bool? TestUnicodeCategories()
    {
      bool? wholeTestResult = null;
      var singleTestResult = TestUnicodeCategories("\\p");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;
      singleTestResult = TestUnicodeCategories("\\P");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;
      return wholeTestResult;
    }

    static bool? TestUnicodeCategories(string precode)
    {
      bool? wholeTestResult = null;
      var codes = new List<string>();
      for (int charCount = 1; charCount <= 2; charCount++)
      {
        AddLetters(codes, "", charCount);
      }
      var singleTestResult = RunSingleUnicodeCategoryPatternTest(precode);
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;
      singleTestResult = RunSingleUnicodeCategoryPatternTest(precode + "a");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;
      singleTestResult = RunSingleUnicodeCategoryPatternTest(precode + "{");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;
      singleTestResult = RunSingleUnicodeCategoryPatternTest(precode + "{}");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;
      for (int i = 0; i < codes.Count; i++)
      {
        var code = codes[i];
        singleTestResult = RunSingleUnicodeCategoryPatternTest(precode + "{" + code);
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
      }
      for (int i = 0; i < codes.Count; i++)
      {
        var code = codes[i];
        singleTestResult = RunSingleUnicodeCategoryPatternTest(precode + "{" + code + "}");
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
      }
      for (int i = 0; i < UnicodeBlockNames.Count(); i++)
      {
        var name = UnicodeBlockNames[i];
        singleTestResult = RunSingleUnicodeCategoryPatternTest(precode + "{" + name + "}");
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
      }
      singleTestResult = RunSingleUnicodeCategoryPatternTest(precode + "{" + UnicodeBlockNames[0]);
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;
      singleTestResult = RunSingleUnicodeCategoryPatternTest(precode + "{" + UnicodeBlockNames[0] + "a");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;
      singleTestResult = RunSingleUnicodeCategoryPatternTest(precode + "{" + UnicodeBlockNames[0] + "a}");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;
      return wholeTestResult;
    }

    static void AddLetters(List<string> codes, string precode, int charCount)
    {
      for (char ch = 'a'; ch <= 'z'; ch++)
      {
        var code = precode + ch;
        codes.Add(code);
        if (code.Length < charCount)
          AddLetters(codes, code, charCount);
      }
      for (char ch = 'A'; ch <= 'Z'; ch++)
      {
        var code = precode + ch;
        codes.Add(code);
        if (code.Length < charCount)
          AddLetters(codes, code, charCount);
      }
    }

    /// <summary>
    /// Runs a single Unicode category pattern  test 
    /// </summary>
    /// <returns></returns>
    static bool? RunSingleUnicodeCategoryPatternTest(string pattern)
    {
      var testInputItem = new TestItem { Pattern = pattern, Result = RegExStatus.OK };
      RegExItem newItem;
      if (pattern.Length < 3)
      {
        newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.UnicodeCategorySeq, Status = RegExStatus.Unfinished };
        testInputItem.Items.Add(newItem);
        testInputItem.Result = newItem.Status;
      }
      else
      if (pattern.Length == 3)
      {
        newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.UnicodeCategorySeq, Status = (pattern[2] == '{') ? RegExStatus.Unfinished : RegExStatus.Error };
        testInputItem.Items.Add(newItem);
        testInputItem.Result = newItem.Status;
      }
      else
      if (pattern.Length == 4)
      {
        RegExStatus status;
        if (UnicodeCategoryFirstLetters.Contains(pattern[3]) || pattern[3] == 'I')
          status = RegExStatus.Unfinished;
        else
          status = RegExStatus.Error;
        newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.UnicodeCategorySeq, Status = status };
        testInputItem.Items.Add(newItem);
        testInputItem.Result = newItem.Status;
      }
      else
      if (pattern.Length == 5)
      {
        if (pattern[4] == '}')
        {
          var ch = pattern[3];
          RegExStatus status;
          if (UnicodeCategoryFirstLetters.Contains(pattern[3]))
            status = RegExStatus.OK;
          else
            status = RegExStatus.Error;
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.UnicodeCategorySeq, Status = status };
          testInputItem.Items.Add(newItem);
          testInputItem.Result = newItem.Status;
        }
        else
        {
          var name = pattern.Substring(3, 2);
          RegExStatus status;
          if (UnicodeCategories.Contains(name) || name == "Is")
            status = RegExStatus.Unfinished;
          else
            status = RegExStatus.Error;
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.UnicodeCategorySeq, Status = status };
          testInputItem.Items.Add(newItem);
          testInputItem.Result = newItem.Status;
        }
      }
      else
      if (pattern.Length == 6)
      {
        if (pattern[5] == '}')
        {
          var name = pattern.Substring(3, 2);
          RegExStatus status;
          if (UnicodeCategories.Contains(name))
            status = RegExStatus.OK;
          else
            status = RegExStatus.Error;
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.UnicodeCategorySeq, Status = status };
          testInputItem.Items.Add(newItem);
          testInputItem.Result = newItem.Status;
        }
        else
        {
          var name = pattern.Substring(3, 2);
          RegExStatus status;
          if (UnicodeCategories.Contains(name) || name == "Is")
            status = RegExStatus.Unfinished;
          else
            status = RegExStatus.Error;
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.UnicodeCategorySeq, Status = status };
          testInputItem.Items.Add(newItem);
          testInputItem.Result = newItem.Status;
        }
      }
      else
      {
        if (pattern.LastOrDefault() == '}')
        {
          var name = pattern.Substring(3, pattern.Length - 4);
          RegExStatus status;
          if (UnicodeBlockNames.Contains(name))
            status = RegExStatus.OK;
          else
            status = RegExStatus.Error;
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.UnicodeCategorySeq, Status = status };
          testInputItem.Items.Add(newItem);
          testInputItem.Result = newItem.Status;
        }
        else
        {
          var name = pattern.Substring(3, pattern.Length - 3);
          RegExStatus status;
          if (UnicodeBlockNames.Contains(name))
            status = RegExStatus.Unfinished;
          else
            status = RegExStatus.Error;
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.UnicodeCategorySeq, Status = status };
          testInputItem.Items.Add(newItem);
          testInputItem.Result = newItem.Status;
        }
      }
      TestItem testOutputItem;
      var singleTestResult = RunSingleTest(testInputItem, out testOutputItem);
      testOutputData.Add(testOutputItem);
      return singleTestResult;
    }
    #endregion

    #region Test Charset Patterns
    static bool? TestCharsetPatterns()
    {
      bool? wholeTestResult = null;
      var singleTestResult = RunSingleCharsetTest("[]");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;
      singleTestResult = RunSingleCharsetTest("[[");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;
      foreach (var ch in ValidChars)
      {
        singleTestResult = RunSingleCharsetTest($"[{ch}");
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
      }
      foreach (var ch in ValidChars)
      {
        singleTestResult = RunSingleCharsetTest($"[{ch}]");
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
      }

      foreach (var ch1 in ValidChars)
        foreach (var ch2 in ValidChars)
        {
          singleTestResult = RunSingleCharsetTest($"[{ch1}{ch2}]");
          SetTestResult(ref wholeTestResult, singleTestResult);
          if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
            return false;
        }

      foreach (var ch1 in ValidChars)
        //var ch1 = '_';
        foreach (var ch2 in ValidChars)
          foreach (var ch3 in ValidChars)
          {
            singleTestResult = RunSingleCharsetTest($"[{ch1}{ch2}{ch3}]");
            SetTestResult(ref wholeTestResult, singleTestResult);
            if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
              return false;
          }

      return wholeTestResult;
    }



    static bool? RunSingleCharsetTest(string pattern)
    {
      var testInputItem = new TestItem { Pattern = pattern, Result = RegExStatus.OK };
      RegExCharSet charset = new RegExCharSet { Start = 0, Str = pattern, Tag = RegExTag.CharSet, Status = RegExStatus.OK };
      testInputItem.Items.Add(charset);
      int charNdx = pattern.Length;
      if (pattern.Length < 3)
      {
        if (pattern == "[]")
        {
          charset.Status = RegExStatus.Error;
        }
        else
        {
          charset.Status = RegExStatus.Unfinished;
          var nextChar = pattern[1];
          RegExItem subItem;
          if (nextChar == '\\')
            subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.EscapedChar, Status = RegExStatus.Unfinished };
          else if (nextChar == '^')
            subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.CharSetControlChar, Status = RegExStatus.OK };
          else
            subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.LiteralChar, Status = RegExStatus.OK };
          charset.Items.Add(subItem);
        }
      }
      else if (pattern.Length == 3)
      {
        if (pattern.Last() == ']')
        {
          var thisChar = pattern[1];
          if (thisChar == ']')
          {
            charset.Status = RegExStatus.Error;
            charset.Str = charset.Str.Substring(0, 2);
          }
          else
          {
            RegExItem subItem;

            if (thisChar == '\\')
            {
              charset.Status = RegExStatus.Unfinished;
              subItem = new RegExItem { Start = 1, Str = pattern.Substring(1), Tag = RegExTag.EscapedChar, Status = RegExStatus.OK };
            }
            else
          if (thisChar == '^')
            {
              charset.Status = RegExStatus.Error;
              subItem = new RegExItem { Start = 1, Str = new string(thisChar, 1), Tag = RegExTag.CharSetControlChar, Status = RegExStatus.OK };
            }
            else
            {
              charset.Status = RegExStatus.OK;
              subItem = new RegExItem { Start = 1, Str = new string(thisChar, 1), Tag = RegExTag.LiteralChar, Status = RegExStatus.OK };
            }
            charset.Items.Add(subItem);
          }
        }
        else
        {
          charset.Status = RegExStatus.Unfinished;
        }
      }
      else
      {
        RegExItem subItem;
        charset.Status = RegExStatus.Unfinished;
        for (charNdx = 1; charNdx < pattern.Length; charNdx++)
        {
          char thisChar = pattern[charNdx];
          char nextChar = charNdx < pattern.Length - 1 ? pattern[charNdx + 1] : '\0';
          if (thisChar == ']')
          {
            if (charset.IsEmpty)
            {
              charset.Status = RegExStatus.Error;
              charset.Str = charset.Str.Substring(0, charNdx + 1);
            }
            else
            {
              if (charset.Status == RegExStatus.Unfinished)
                charset.Status = RegExStatus.OK;
              charset.Str = charset.Str.Substring(0, charNdx + 1);
              charNdx++;
            }
            break;
          }
          else
          {
            if (thisChar == '\\')
            {
              if (nextChar != '\0')
              {
                if (Domain2ndChars.Contains(nextChar))
                  subItem = new RegExItem { Start = charNdx, Str = pattern.Substring(charNdx, 2), Tag = RegExTag.CharClass, Status = RegExStatus.OK };
                else
                if (nextChar == 'p' || nextChar == 'P')
                {
                  var k = pattern.IndexOf('}', charNdx);
                  string name;
                  if (k >= 0)
                    name = pattern.Substring(charNdx + 3, k - charNdx - 3);
                  else
                  {
                    name = pattern.Substring(charNdx + 3);
                    k = pattern.Length - 1;
                  }
                  var isOK = UnicodeCategoryFirstLetters.Contains(name) || UnicodeCategories.Contains(name) || UnicodeBlockNames.Contains(name);
                  subItem = new RegExItem { Start = charNdx, Str = pattern.Substring(charNdx, k - charNdx + 1), Tag = RegExTag.UnicodeCategorySeq, Status = isOK ? RegExStatus.OK : RegExStatus.Error };
                  charNdx += k - 1;
                }
                else
                if (nextChar >= '0' && nextChar <= '8')
                  subItem = new RegExItem { Start = charNdx, Str = pattern.Substring(charNdx, 2), Tag = RegExTag.OctalSeq, Status = RegExStatus.OK };
                else
                  subItem = new RegExItem { Start = charNdx, Str = pattern.Substring(charNdx, 2), Tag = RegExTag.EscapedChar, Status = RegExStatus.OK };
                charset.Items.Add(subItem);
                charNdx++;
              }
              else
              {
                subItem = new RegExItem { Start = charNdx, Str = "\\]", Tag = RegExTag.EscapedChar, Status = RegExStatus.OK };
                charset.Items.Add(subItem);
                charset.Status = RegExStatus.Unfinished;
              }
            }
            else
            {
              if (thisChar == '^' && charNdx == 1)
                subItem = new RegExItem { Start = charNdx, Str = new string(thisChar, 1), Tag = RegExTag.CharSetControlChar, Status = RegExStatus.OK };
              else if (thisChar == '-' && charNdx > 1 && nextChar == '[')
              {
                subItem = new RegExItem { Start = charNdx, Str = new string(thisChar, 1), Tag = RegExTag.CharSetControlChar, Status = RegExStatus.OK };
                charset.Items.Add(subItem);
                charNdx += 1;
                int k = pattern.IndexOf(']', charNdx);
                var str = pattern.Substring(charNdx, k - charNdx + 1);
                RegExCharSet intCharset = new RegExCharSet { Start = charNdx, Str = str, Tag = RegExTag.CharSet, Status = (str == "[]") ? RegExStatus.Error : RegExStatus.OK };
                if (str != "[]")
                {
                  subItem = new RegExCharRange { Start = charNdx, Str = pattern.Substring(charNdx, k - charNdx), Tag = RegExTag.CharRange, Status = RegExStatus.OK };
                  intCharset.Items.Add(subItem);
                }
                subItem = intCharset;
                charNdx += k;
              }
              else
              if ((charNdx < pattern.Length - 2) && (nextChar == '-') && (pattern[charNdx + 2] != '[') && (pattern[charNdx + 2] != ']'))
              {
                nextChar = pattern[charNdx + 2];
                subItem = new RegExCharRange { Start = charNdx, Str = pattern.Substring(charNdx, 3), Tag = RegExTag.CharRange, Status = nextChar >= thisChar ? RegExStatus.OK : RegExStatus.Error };
                charNdx += 2;
              }
              else
                subItem = new RegExItem { Start = charNdx, Str = new string(thisChar, 1), Tag = RegExTag.LiteralChar, Status = RegExStatus.OK };
              charset.Items.Add(subItem);
              if (subItem.Status != RegExStatus.OK && charNdx < pattern.Length - 1)
                charset.Status = subItem.Status;
            }
          }
        }
      }
      testInputItem.Result = charset.Status;
      if (testInputItem.Result != RegExStatus.Error && charNdx < pattern.Length)
      {
        var status1 = Tagger.TryParsePattern(pattern.Substring(charNdx));
        testInputItem.Result = status1;
        int n = 0;
        foreach (var newItem in Tagger.Items)
        {
          if (n == 0 && (newItem.Tag == RegExTag.Quantifier || newItem.Tag == RegExTag.AltChar) && newItem.Status == RegExStatus.Warning)
            testInputItem.Result = newItem.Status = RegExStatus.OK;
          n++;
          newItem.MoveStart(charNdx);
          testInputItem.Items.Add(newItem);
        }
      }
      TestItem testOutputItem;
      var singleTestResult = RunSingleTest(testInputItem, out testOutputItem);
      testOutputData.Add(testOutputItem);
      return singleTestResult;
    }
    #endregion

    #region Test Grouping Patterns
    static bool? TestGroupingPatterns()
    {
      bool? wholeTestResult = null;
      bool? singleTestResult;

      //singleTestResult = RunSingleGroupingTest("()");
      //SetTestResult(ref wholeTestResult, singleTestResult);
      //if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
      //  return false;

      singleTestResult = RunNamedGroupTest('\'');
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunNamedGroupTest('<');
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunSingleGroupingTest($"(?:non_capturing_group)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      //foreach (var ch in ValidChars)
      //{
      //  singleTestResult = RunSingleGroupingTest($"({ch}");
      //  SetTestResult(ref wholeTestResult, singleTestResult);
      //  if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
      //    return false;
      //}


      //foreach (var ch in ValidChars)
      //{
      //  singleTestResult = RunSingleGroupingTest($"({ch})");
      //  SetTestResult(ref wholeTestResult, singleTestResult);
      //  if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
      //    return false;
      //}

      //foreach (var ch1 in ValidChars)
      //  foreach (var ch2 in ValidChars)
      //  {
      //    singleTestResult = RunSingleGroupingTest($"({ch1}{ch2})");
      //    SetTestResult(ref wholeTestResult, singleTestResult);
      //    if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
      //      return false;
      //  }

      //foreach (var ch1 in ValidChars)
      //  //var ch1 = '_';
      //  foreach (var ch2 in ValidChars)
      //    foreach (var ch3 in ValidChars)
      //    {
      //      singleTestResult = RunSingleGroupingTest($"({ch1}{ch2}{ch3})");
      //      SetTestResult(ref wholeTestResult, singleTestResult);
      //      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
      //        return false;
      //    }

      return wholeTestResult;
    }

    static bool? RunNamedGroupTest(char nextChar)
    {
      bool? wholeTestResult = null;
      var closingChar = (nextChar == '<') ? '>' : nextChar;
      bool? singleTestResult;

      singleTestResult = RunSingleGroupingTest($"(?{nextChar}name{closingChar}ok)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunSingleGroupingTest($"(?{nextChar}invalid name{closingChar}ok)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunSingleGroupingTest($"(?{nextChar}name1{closingChar}ok)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunSingleGroupingTest($"(?{nextChar}0name{closingChar}ok)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunSingleGroupingTest($"(?{nextChar}valid_name{closingChar}ok)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunSingleGroupingTest($"(?{nextChar}niebłędna_nazwa{closingChar}ok)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunSingleGroupingTest($"(?{nextChar}unfinished_name`ok)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunSingleGroupingTest($"(?{nextChar}unfinished_expr");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunSingleGroupingTest($"(?{nextChar}balancing-group{closingChar}ok)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunSingleGroupingTest($"(?{nextChar}balancing-group-two{closingChar}ok)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      return wholeTestResult;
    }

    static bool? RunSingleGroupingTest(string pattern)
    {
      var testInputItem = new TestItem { Pattern = pattern, Result = null };
      RegExGroup group = new RegExGroup { Start = 0, Str = pattern, Tag = RegExTag.Subexpression, Status = RegExStatus.OK };
      testInputItem.Items.Add(group);
      int charNdx = pattern.Length;
      if (pattern.Length < 3)
      {
        if (pattern == "()")
        {
          group.Status = RegExStatus.OK;
        }
        else
        {
          group.Status = RegExStatus.Unfinished;
          var nextChar = pattern[1];
          RegExItem subItem;
          if (nextChar == '\\')
            subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.EscapedChar, Status = RegExStatus.Unfinished };
          else if (nextChar == '?')
            subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.GroupControlChar, Status = RegExStatus.OK };
          else if (nextChar == '.')
            subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.DotChar, Status = RegExStatus.OK };
          else if (nextChar == '|')
            subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.AltChar, Status = RegExStatus.Warning };
          else if (nextChar == '(')
            subItem = new RegExGroup { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.Subexpression, Status = RegExStatus.Unfinished };
          else if (nextChar == '[')
            subItem = new RegExCharSet { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.CharSet, Status = RegExStatus.Unfinished };
          else if (AnchorChars.Contains(nextChar))
            subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.AnchorControl, Status = RegExStatus.OK };
          else if (QuantifierChars.Contains(nextChar))
            subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.Quantifier, Status = RegExStatus.Warning };
          else
            subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.LiteralChar, Status = RegExStatus.OK };
          group.Items.Add(subItem);
        }
      }
      else
      if (pattern.Length == 3)
      {
        group.Status = RegExStatus.OK;
        var nextChar = pattern[1];
        RegExItem subItem;
        if (nextChar == '\\') // pattern == @"(\)"
        {
          subItem = new RegExItem { Start = 1, Str = pattern.Substring(1), Tag = RegExTag.EscapedChar, Status = RegExStatus.OK };
          group.Status = RegExStatus.Unfinished;
        }
        else if (nextChar == '?') // pattern == @"(?)"
        {
          subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.GroupControlChar, Status = RegExStatus.Warning };
          group.Status = RegExStatus.Warning;
        }
        else if (nextChar == '.') // pattern == @"(.)"
          subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.DotChar, Status = RegExStatus.OK };
        else if (nextChar == '|')
        {
          subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.AltChar, Status = RegExStatus.Warning };
          group.Status = RegExStatus.OK;
        }
        else if (nextChar == '(') // pattern == @"(()"
        {
          subItem = new RegExGroup { Start = 1, Str = pattern.Substring(1), Tag = RegExTag.Subexpression, Status = RegExStatus.OK };
          group.Status = RegExStatus.Unfinished;
        }
        else if (nextChar == ')') // pattern == @"())"
        {
          subItem = null;
          group.Status = RegExStatus.OK;
          group.Str = pattern.Substring(0, 2);
          var nextItem = new RegExGroup { Start = 2, Str = pattern.Substring(2), Tag = RegExTag.Subexpression, Status = RegExStatus.Error };
          testInputItem.Items.Add(nextItem);
          testInputItem.Result = RegExStatus.Error;
        }
        else if (nextChar == '[') // pattern == @"([)"
        {
          var charset = new RegExCharSet { Start = 1, Str = pattern.Substring(1, 2), Tag = RegExTag.CharSet, Status = RegExStatus.Unfinished };
          charset.Items.Add(new RegExItem { Start = 2, Str = pattern.Substring(2, 1), Tag = RegExTag.LiteralChar, Status = RegExStatus.OK });
          subItem = charset;
          group.Status = RegExStatus.Unfinished;
        }
        else if (AnchorChars.Contains(nextChar))
          subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.AnchorControl, Status = RegExStatus.OK };
        else
        if (QuantifierChars.Contains(nextChar)) // pattern == @"(*)", pattern == @"(+)", 
        {
          subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.Quantifier, Status = RegExStatus.Warning };
          group.Status = RegExStatus.OK;
        }
        else
          subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.LiteralChar, Status = RegExStatus.OK };
        if (subItem != null)
          group.Items.Add(subItem);
      }
      else
      {
        group.Status = RegExStatus.OK;
        var nextChar = pattern[1];
        RegExItem subItem = null;
        if (nextChar == '\\')
        {
          subItem = new RegExItem { Start = 1, Str = pattern.Substring(1), Tag = RegExTag.EscapedChar, Status = RegExStatus.OK };
          group.Status = RegExStatus.Unfinished;
        }
        else if (nextChar == '?')
        {
          var groupControlChar = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.GroupControlChar, Status = RegExStatus.Warning };
          group.Items.Add(groupControlChar);
          nextChar = pattern[2];
          if (nextChar == ':') // pattern.StartsWith(@"(:"); // non-capturing group
          {
            group.Tag = RegExTag.NonCapturingGroup;
            groupControlChar.Status = RegExStatus.OK;
            group.Items.Add(new RegExItem { Start = 2, Str = new string(':', 1), Tag = RegExTag.GroupControlChar, Status = RegExStatus.OK });
            var restStr = pattern.Substring(3, pattern.Length - 4);
            Tagger.TryParsePattern(restStr);
            foreach (var item in Tagger.Items)
            {
              item.MoveStart(3);
              group.Items.Add(item);
            }

          }
          else
          if ((nextChar == '\'')  // pattern.StartsWith(@"(?'"); 
            || (nextChar == '<')) // patternStartsWith(@"(?<"); // named group
          {
            group.Tag = RegExTag.NamedGroup;
            groupControlChar.Status = RegExStatus.OK;
            string name = null;
            var openingChar = nextChar;
            var closingChar = (nextChar == '<') ? '>' : nextChar;
            group.Items.Add(new RegExItem { Start = 2, Str = new string(openingChar, 1), Tag = RegExTag.GroupControlChar, Status = RegExStatus.OK });
            var k = pattern.IndexOf(closingChar, 3);
            String restStr = null;
            var closingCharIndex = -1;
            if (k > 0)
            {
              closingCharIndex = k;
              name = pattern.Substring(3, k - 3);
              restStr = pattern.Substring(k + 1, pattern.Length - k - 2);
            }
            else
            {
              for (k = 3; k < pattern.Length; k++)
              {
                var nameChar = pattern[k];
                if (Char.IsLetterOrDigit(nameChar) || nameChar == ' ' || nameChar == '_' || nameChar == '-')
                  name += nameChar;
                else
                {
                  restStr = pattern.Substring(k, pattern.Length - k - 1);
                  break;
                }
              }
            }
            var groupNameItem = new RegExGroupName { Start = 3, Str = name, Tag = RegExTag.GroupName, Status = RegExStatus.OK };
            group.Name = name;
            if (closingCharIndex < 0)
            {
              if (String.IsNullOrEmpty(restStr))
                group.Status = groupNameItem.Status = RegExStatus.Unfinished;
              else
              {
                groupNameItem.Status = RegExStatus.Unfinished;
                group.Status = RegExStatus.Error;
              }
            }
            else
            {
              int priorDashIndex = 0;
              for (int nameIndex = 0; nameIndex < name.Length; nameIndex++)
              {
                var nameChar = name[nameIndex];
                if (nameChar == ' ')
                  groupNameItem.Status = RegExStatus.Warning;
                else
                if (nameIndex == 0 && Char.IsDigit(nameChar))
                  groupNameItem.Status = RegExStatus.Warning;
                else
                if (nameChar == '-') // balancing group
                {
                  group.Tag = RegExTag.BalancingGroup;
                  group.Items.Add(groupNameItem);
                  var dashItem = new RegExItem { Start = group.Start + nameIndex + 3, Str = new string(nameChar, 1), Tag = RegExTag.GroupControlChar, Status = RegExStatus.OK };
                  group.Items.Add(dashItem);
                  if (nameIndex < name.Length)
                  {
                    var nameRest = name.Substring(nameIndex + 1);
                    groupNameItem.Str = name.Substring(priorDashIndex, nameIndex-priorDashIndex);
                    if (priorDashIndex==0)
                      group.Name = groupNameItem.Str;
                    groupNameItem = new RegExGroupName { Start = group.Start + nameIndex + 4, Str = nameRest, Tag = RegExTag.GroupName, Status = RegExStatus.OK };
                    if (priorDashIndex > 0)
                    {
                      dashItem.Status = RegExStatus.Error;
                      groupNameItem.Status = RegExStatus.Warning;
                      group.Status = RegExStatus.Error;
                    }
                    priorDashIndex = nameIndex + 1;
                  }
                  else
                    group.Status = RegExStatus.Error;
                }
                else
                if (!(Char.IsLetterOrDigit(nameChar) || (nameChar == '_')))
                  groupNameItem.Status = RegExStatus.Error;
              }
            }
            group.Items.Add(groupNameItem);
            if (closingCharIndex>=0)
              group.Items.Add(new RegExItem { Start = closingCharIndex, Str = new string(closingChar, 1), Tag = RegExTag.GroupControlChar, Status = RegExStatus.OK });

            if (!String.IsNullOrEmpty(restStr))
            {
              if (groupNameItem.Status == RegExStatus.Unfinished)
              {
                closingCharIndex = 3 + groupNameItem.Length;
                group.Items.Add(new RegExItem { Start = closingCharIndex, Str = restStr.Substring(0, 1), Tag = RegExTag.GroupControlChar, Status = RegExStatus.Error });
                restStr = restStr.Substring(1);
              }
              Tagger.TryParsePattern(restStr);
              foreach (var item in Tagger.Items)
              {
                item.MoveStart(3 + name.Length + ((closingCharIndex>=0) ? 1 : 0));
                group.Items.Add(item);
              }
            }
            if (group.Status == RegExStatus.OK)
              group.Status = groupNameItem.Status;
          }
          else
          if (nextChar == '(') // pattern = @"(?()"
          {
            var status1 = Tagger.TryParsePattern(pattern.Substring(2, pattern.Length - 2));
            foreach (var item in Tagger.Items)
            {
              item.MoveStart(2);
              group.Items.Add(item);
            }
            group.Status = RegExStatus.Unfinished;
          }
          else
          if (nextChar == ')') // pattern = @"(?))"
          {
            var status1 = Tagger.TryParsePattern(pattern.Substring(3));
            foreach (var item in Tagger.Items)
            {
              item.MoveStart(3);
              testInputItem.Items.Add(item);
            }
            group.Status = RegExStatus.Warning;
            group.Str = group.Str.Substring(0, group.Str.Length - 1);
            testInputItem.Result = RegExStatus.Error;
          }
          else
          if (nextChar == '*') // pattern = @"(?*)"
          {
            var status1 = Tagger.TryParsePattern(pattern.Substring(2, pattern.Length - 3));
            foreach (var item in Tagger.Items)
            {
              if (item.Start == 0)
              {
                item.Status = RegExStatus.Warning;
              }
              item.MoveStart(2);
              group.Items.Add(item);
            }
            group.Status = RegExStatus.Warning;
          }
          else
          if (nextChar == '[') // pattern = @"(?[)"
          {
            var status1 = Tagger.TryParsePattern(pattern.Substring(2, pattern.Length - 2));
            foreach (var item in Tagger.Items)
            {
              item.MoveStart(2);
              group.Items.Add(item);
            }
            group.Status = RegExStatus.Unfinished;
          }
          else
          if (nextChar == '\\') // pattern = @"(?\)"
          {
            var status1 = Tagger.TryParsePattern(pattern.Substring(2, pattern.Length - 2));
            foreach (var item in Tagger.Items)
            {
              item.MoveStart(2);
              group.Items.Add(item);
            }
            group.Status = RegExStatus.Unfinished;
          }
          else
          {
            var status1 = Tagger.TryParsePattern(pattern.Substring(2, pattern.Length - 3));
            foreach (var item in Tagger.Items)
            {
              item.MoveStart(2);
              group.Items.Add(item);
            }
            group.Status = RegExStatus.Warning;
          }
        }
        else if (nextChar == '|')
        {
          subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.AltChar, Status = RegExStatus.Warning };
          group.Status = RegExStatus.Warning;
        }
        else if (nextChar == '(')
        {
          var openParCount = pattern.Where(ch => ch == '(').Count();
          var closeParCount = pattern.Where(ch => ch == ')').Count();
          if (openParCount == closeParCount)
          {
            var str = pattern.Substring(1, pattern.Length - 2);
            var status1 = Tagger.TryParsePattern(str);
            foreach (var item in Tagger.Items)
            {
              item.MoveStart(1);
              group.Items.Add(item);
            }
            group.Status = RegExStatus.OK;
          }
          else
          {
            var str = pattern.Substring(1);
            var status1 = Tagger.TryParsePattern(str);
            foreach (var item in Tagger.Items)
            {
              item.MoveStart(1);
              group.Items.Add(item);
            }
            group.Status = status1 == RegExStatus.Error ? RegExStatus.Error : RegExStatus.Unfinished;
          }
        }
        else if (nextChar == ')')
        {
          group.Str = "()";
          group.Status = RegExStatus.OK;
          var str = pattern.Substring(2);
          var status1 = Tagger.TryParsePattern(str);
          foreach (var item in Tagger.Items)
          {
            if (item.Start == 0 && QuantifierChars.Contains(item.Str.First()))
            {
              if (item.Status == RegExStatus.Warning)
                item.Status = RegExStatus.OK;
            }
            else
            if (item.Start == 0 && item.Str == "|")
            {
              item.Status = RegExStatus.Error;
            }

            item.MoveStart(2);
            testInputItem.Items.Add(item);
          }
          testInputItem.Result = status1;
        }
        else if (nextChar == '[')
        {
          var str = pattern.Substring(2);
          int k = pattern.LastIndexOf(']');
          if (k < 0)
            k = pattern.Length - 1;
          else
            str = pattern.Substring(2, k - 2);
          var charset = new RegExCharSet { Start = 1, Str = pattern.Substring(1, k), Tag = RegExTag.CharSet, Status = (k < pattern.Length - 1) ? RegExStatus.OK : RegExStatus.Unfinished };
          if (str == "")
            charset.Status = RegExStatus.Error;
          for (int i = 0; i < str.Length; i++)
          {
            var curChar = str[i];
            if (curChar == '\\' && i < str.Length)
            {
              charset.Items.Add(new RegExItem { Start = i + 2, Str = str.Substring(i, 2), Tag = RegExTag.EscapedChar, Status = RegExStatus.OK });
              i++;
            }
            else
            if (curChar == '^' && i == 0)
              charset.Items.Add(new RegExItem { Start = i + 2, Str = new string(curChar, 1), Tag = RegExTag.CharSetControlChar, Status = RegExStatus.OK });
            else
              charset.Items.Add(new RegExItem { Start = i + 2, Str = new string(curChar, 1), Tag = RegExTag.LiteralChar, Status = RegExStatus.OK });
          }
          subItem = charset;
          group.Status = (k < pattern.Length - 1) ? RegExStatus.OK : RegExStatus.Unfinished;
        }
        else
        {
          if (pattern.EndsWith("|)"))
          {
            var str = pattern.Substring(1);
            var status1 = Tagger.TryParsePattern(str);
            foreach (var item in Tagger.Items)
            {
              if (item.Tag == RegExTag.Subexpression && item.Status == RegExStatus.Error)
                break;
              item.MoveStart(1);
              group.Items.Add(item);
            }
            group.Status = RegExStatus.OK;
          }
          else
          if (pattern.EndsWith("()") || pattern.EndsWith("[)") || pattern.EndsWith("\\)") || pattern.EndsWith("|)"))
          {
            var str = pattern.Substring(1);
            var status1 = Tagger.TryParsePattern(str);
            foreach (var item in Tagger.Items)
            {
              item.MoveStart(1);
              group.Items.Add(item);
            }
            group.Status = RegExStatus.Unfinished;
          }
          else if (pattern.EndsWith("))"))//(QuantifierChars.Contains(nextChar))
          {
            group.Status = RegExStatus.OK;
            var str = pattern.Substring(1, pattern.Length - 2);
            var status1 = Tagger.TryParsePattern(str);
            foreach (var item in Tagger.Items)
            {
              if (item.Status == RegExStatus.Error)
              {
                item.MoveStart(2);
                item.Status = RegExStatus.Error;
                //group.Status = RegExStatus.Warning;
                group.Str = group.Str.Substring(0, group.Str.Length - 1);
                testInputItem.Items.Add(item);
                testInputItem.Result = RegExStatus.Error;
              }
              else
              {
                if (item.Tag == RegExTag.Subexpression)
                {
                  if (item.Status == RegExStatus.Unfinished)
                  {
                    item.Str += ')';
                    item.Status = RegExStatus.OK;
                    group.Status = RegExStatus.Unfinished;
                  }
                }
                else
                if (item.Start == 0 && item.Str == "|")
                {
                  item.Status = RegExStatus.Error;
                }
                item.MoveStart(1);
                group.Items.Add(item);
              }
            }
          }
          else
          {
            var str = pattern.Substring(1, pattern.Length - 2);
            var status1 = Tagger.TryParsePattern(str);
            foreach (var item in Tagger.Items)
            {
              item.MoveStart(1);
              group.Items.Add(item);
            }
            group.Status = RegExStatus.OK;
          }
        }
        if (subItem != null)
        {
          group.Items.Add(subItem);
          group.Status = RegExStatus.OK;
        }
      }
      if (testInputItem.Result == null)
        testInputItem.Result = group.Status;
      if (testInputItem.Result != RegExStatus.Error && charNdx < pattern.Length)
      {
        var status1 = Tagger.TryParsePattern(pattern.Substring(charNdx));
        testInputItem.Result = status1;
        int n = 0;
        foreach (var newItem in Tagger.Items)
        {
          if (n == 0 && (newItem.Tag == RegExTag.Quantifier || newItem.Tag == RegExTag.AltChar) && newItem.Status == RegExStatus.Warning)
            testInputItem.Result = newItem.Status = RegExStatus.OK;
          n++;
          newItem.MoveStart(charNdx);
          testInputItem.Items.Add(newItem);
        }
      }
      TestItem testOutputItem;
      var singleTestResult = RunSingleTest(testInputItem, out testOutputItem);
      testOutputData.Add(testOutputItem);
      return singleTestResult;
    }
    #endregion

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
    /// Runs a single test of one pattern
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
        singleTestResult = (testInputItem.Items == null) || testOutputItem.Items.Equals(testInputItem.Items)
          && (testInputItem.Result == testOutputItem.Result);
      }
      if (testInputItem.PatternItems != null)
      {
        testOutputItem.PatternItems = testOutputItem.Items.GeneratePatternItems();
        if (singleTestResult != false)
          singleTestResult = PatternItemsComparator.AreEqual(testOutputItem.PatternItems, testInputItem.PatternItems);
      }

      if (singleTestResult == false)
      {
        TestOutput.WriteLine($"Expected:");
        ShowTestItem(testInputItem);
        TestOutput.WriteLine($"Obtained:");
        ShowTestItem(testOutputItem);
      }
      else if (singleTestResult != true || !TestRunOptions.HasFlag(TestOptions.SuppressPassedTestResults))
        ShowTestItem(testOutputItem);
      if (testInputItem.Result != null)
      {
        if (singleTestResult == true)
          TestOutput.WriteLine($"test PASSED");
        else if (singleTestResult == false)
          TestOutput.WriteLine($"test FAILED");
      }
      TestOutput.WriteLine();
      TestOutput.Flush();
      return singleTestResult;
    }

    /// <summary>
    /// Saves testOutputData in file depending on suggestion and user decision
    /// </summary>
    /// <param name="filename">name of output file</param>
    /// <param name="shouldSave">if is null then save depends on user decision</param>
    /// <param name="message">message to ask user for decision</param>
    /// <returns></returns>
    static bool TrySaveTestData(string filename)
    {
      bool testDataWritten = false;
      try
      {
        TestOutput.WriteLine($"Writting test data to file \"{filename}\\");
        WriteTestData(filename);
        TestOutput.WriteLine($"written tests: {testOutputData.Count}");
        TestOutput.WriteLine("");
        testDataWritten = true;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
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

    static TestData ReadTestData(string filename)
    {
      var ext = Path.GetExtension(filename)?.ToLowerInvariant();

      if (ext == ".xml")
        using (var reader = File.OpenText(filename))
        {
          return (TestData)XamlServices.Load(reader);
        }
      else if (ext == ".txt")
        using (var reader = File.OpenText(filename))
        {
          return ReadTextInputData(reader, filename);
        }
      return null;
    }

    static TestData ReadTextInputData(TextReader reader, string filename)
    {
      TestData result = new TestData();
      string line;
      TestItem testItem = null;
      int lineNo = 0;
      while ((line = reader.ReadLine()) != null)
      {
        lineNo++;
        if (line != "")
        {
          if (line[0] == '\t')
          {
            if (testItem!=null)
            {
              line = line.Trim();
              var cols = line.Split('\t');
              if (cols.Length!=2)
              {
                result.RemoveAt(result.Count - 1);
                Console.WriteLine($"Error while reading test file \"{filename}\"");
                Console.WriteLine($"  in line {lineNo}:");
                Console.WriteLine($"    As the line starts with a tab character, the line is treated as two column pattern parse result.");
                Console.WriteLine($"    Columns should be separated with tab characters.");
                Console.WriteLine($"    There were {cols.Length} columns found");
                testItem = null; // no further trials for this pattern.
              }
              else
              {
                testItem.Result = RegExStatus.OK;
                if (testItem.PatternItems == null)
                  testItem.PatternItems = new PatternItems();
                testItem.PatternItems.Add(new PatternItem { Str = cols[0], Description = cols[1].Trim() });
              }
            }
          }
          else
          {
            testItem = new TestItem { Pattern = line };
            result.Add(testItem);
          }

        }
      }
      return result;
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
      TestOutput.WriteLine($"TryParse = {item.Result}");
      DrawGraph(TestOutput, item.Pattern, item.Items);
      if (item.PatternItems!=null)
        ShowPatternItems(item.PatternItems);
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
        if (item is RegExCharSet charset)
          Dump(writer, charset.Items, level + 1);
      }
    }

    static List<ConsoleColor> ConsoleColors = new List<ConsoleColor>
    {
      ConsoleColor.Green,
      ConsoleColor.Red,
      ConsoleColor.Cyan,
      ConsoleColor.Magenta,
      ConsoleColor.Yellow,
    };


    static void DrawGraph(TextWriter writer, string pattern, RegExItems items)
    {
      GraphItems graphItems = new GraphItems(items);
      List<GraphItem> itemsList = graphItems.ToList();
      SetupColors(itemsList);
      var plainText = graphItems.PlainText;
      var xCount = plainText.Length + 1;
      var yCount = itemsList.Count;
      ColoredChar[,] frameMatrix = new ColoredChar[xCount, yCount];
      for (int x = 0; x < xCount; x++)
        for (int y = 0; y < yCount; y++)
        {
          frameMatrix[x, y] = new ColoredChar(' ', 0xFFFFFF);
        }

      for (int i = itemsList.Count - 1; i >= 0; i--)
      {
        var item = itemsList[i];

        int yMax = yCount - i - 1;
        int color = item.Color;

        if (item.SubItems == null && yMax != 0)
        {
          if (item.Length == 1)
          {
            for (int y = 0; y < yMax; y++)
            {
              frameMatrix[item.ColStart, y] = new ColoredChar('│', color);
            }
            frameMatrix[item.ColStart, yMax] = new ColoredChar('└', color);
          }
          else
          {
            frameMatrix[item.ColStart, 0] = new ColoredChar('├', color);
            for (int x = 1; x < item.ColCount - 2; x++)
            {
              frameMatrix[item.ColStart + x, 0] = new ColoredChar('─', color);
            }
            frameMatrix[item.ColStart + item.ColCount - 2, 0] = new ColoredChar('┘', color);
            for (int y = 1; y < yMax; y++)
            {
              frameMatrix[item.ColStart, y] = new ColoredChar('│', color);
            }
            frameMatrix[item.ColStart, yMax] = new ColoredChar('└', color);
          }
        }
        else
        {
          for (int y = 0; y < yMax; y++)
          {
            frameMatrix[item.ColStart, y] = new ColoredChar('│', color);
            if (item.Length > 1)
            {
              frameMatrix[item.ColStart + item.ColCount - 2, y] = new ColoredChar('│', color);
            }
          }

          if (item.Length == 1)
          {
            frameMatrix[item.ColStart, yMax] = new ColoredChar('└', color);
          }
          else
          {
            frameMatrix[item.ColStart, yMax] = new ColoredChar('└', color);
            for (int x = 1; x < item.ColCount - 1; x++)
            {
              frameMatrix[item.ColStart + x, yMax] = new ColoredChar('─', color);
            }
            frameMatrix[item.ColStart + item.ColCount - 2, yMax] = new ColoredChar('┴', color);
            //patternColors[item.ColStart + item.Length - 1] = color;
            if (item.SubItems != null)
            {
              for (int y = yMax - 1; y >= 0; y--)
              {
                int lineColor = -1;
                for (int x = item.ColStart + item.ColCount - 3; x >= 0; x--)
                {
                  var curChar = frameMatrix[x, y].Value;
                  if (curChar == '└' || curChar == '─' || curChar == '┴')
                  {
                    lineColor = frameMatrix[x, y].Color;
                    break;
                  }
                  else
                  if (curChar != ' ')
                  {
                    break;
                  }
                }
                if (lineColor != -1)
                {
                  for (int x = item.ColStart + item.ColCount - 3; x >= 0; x--)
                  {
                    if (frameMatrix[x, y].Value == ' ')
                    {
                      frameMatrix[x, y] = new ColoredChar('─', lineColor);
                    }
                    else
                      break;
                  }
                  frameMatrix[item.ColStart + item.ColCount - 1, y] = new ColoredChar('─', lineColor);
                }
              }
            }
          }
        }
      }

      if (TestOutput == Console.Out)
      {
        var coloredText = graphItems.ColoredText;
        foreach (var item in coloredText)
        {
          Console.ForegroundColor = GetConsoleColor(item.Color);
          Console.Write(item.Value);
        }
        //if (items.Inequality != null)
        //  Console.Write($"  Items.Count={items.Inequality.Obtained} expected={items.Inequality.Expected}"); 
        Console.WriteLine();
      }
      else
      {
        TestOutput.WriteLine(graphItems.PlainText);
      }
      for (int i = itemsList.Count - 1; i >= 0; i--)
      {
        var item = itemsList[i];
        int yMax = yCount - i - 1;
        int l = xCount;
        while (l > 0 && frameMatrix[l - 1, yMax].Value == ' ') l--;
        if (TestOutput == Console.Out)
        {
          for (int x = 0; x < l; x++)
          {
            Console.ForegroundColor = GetConsoleColor(frameMatrix[x, yMax].Color);
            Console.Write(frameMatrix[x, yMax].Value);
          }
          Console.WriteLine(" " + item.ToString());
          Console.ResetColor();
        }
        else
        {
          string frameLine = "";
          if (l > 0)
          {
            var frameRow = new char[l];
            for (int x = 0; x < l; x++)
              frameRow[x] = frameMatrix[x, yMax].Value;
            frameLine = new string(frameRow);
          }
          TestOutput.Write(frameLine);
          var s = $"{item}";
          TestOutput.WriteLine($" {s}");
        }
      }
    }

    /// <summary>
    /// Setup colors in list of graph items and keep each adjacent colors different
    /// </summary>
    /// <param name="items"></param>
    static void SetupColors(List<GraphItem> items)
    {
      SetupColors(items, 0, -1);
      int priorColor = -1;
      foreach (var item in items)
      {
        if (item.Color == priorColor)
        {
          item.Color = (item.Color + 1) % ConsoleColors.Count;
        }
        priorColor = item.Color;
      }
    }

    /// <summary>
    /// Recurrent method of colors setup. 
    /// When iterate in subitems, it keeps that the last subitem will have different color than closing mark of the group.
    /// </summary>
    /// <param name="items"></param>
    /// <param name="colorIndex"></param>
    /// <param name="groupColor"></param>
    /// <returns></returns>
    static int SetupColors(List<GraphItem> items, int colorIndex, int groupColor)
    {
      var lastColorIndex = -1;
      int itemColor = -1;
      for (int i = 0; i < items.Count; i++)
      {
        var item = items[i];
        int index = item.Start;
        itemColor = colorIndex;
        if (i == items.Count - 1 && itemColor == groupColor)
        {
          colorIndex = (colorIndex + 1) % ConsoleColors.Count();
          itemColor = colorIndex;
        }
        item.Color = itemColor;
        lastColorIndex = colorIndex;
        colorIndex = (colorIndex + 1) % ConsoleColors.Count();
        if (item.SubItems != null)
        {
          var lastColor = SetupColors(item.SubItems, colorIndex, itemColor);
          if (lastColor == colorIndex)
            colorIndex = (colorIndex + 1) % ConsoleColors.Count();
        }
      }
      return lastColorIndex;
    }

    static ConsoleColor GetConsoleColor(int colorIndex)
    {
      if (colorIndex == 0xFFFFFF)
        return ConsoleColor.White;
      return ConsoleColors[colorIndex];
    }

    static void ShowPatternItems(PatternItems patternItems)
    {
      TestOutput.WriteLine();
      foreach (var item in patternItems)
      {
        string check = "";
        if (item.IsOK == true)
          check = "OK";
        else if (item.IsOK == false)
          check = "ERROR";
        TestOutput.WriteLine($"{item.Str}\t{item.Description}\t{check}");
      }
      TestOutput.WriteLine();
    }


  }
}
