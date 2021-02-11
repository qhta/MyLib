using Qhta.RegularExpressions;
using Qhta.RegularExpressions.ConsoleDrawing;
using Qhta.RegularExpressions.Descriptions;

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
      TestReplacementPatterns = 0x2000,
    }

    const string QuantifierChars = "?+*";
    const string AnchorChars = "^$";
    const string OpeningChars = "([";
    const string Anchor2ndChars = "AZzGbB";
    const string Domain2ndChars = "DdWwSs";
    const string UnicodeCategoryFirstLetters = "LNMPSZC";
    const string HexadecimalTestChars = "09AFafg";
    const string LiteralReplacementChars = "$&`'_";
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
    static RegExConsolePresenter Presenter = new RegExConsolePresenter();
    static PatternItemsGenerator Generator = new PatternItemsGenerator();

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
      TestOutput.WriteLine($"\t/$ - test substitution patterns");
      TestOutput.WriteLine($"");
      TestOutput.WriteLine($"TestInputFile can be of the type:");
      TestOutput.WriteLine($"\txml - the whole test data is read with test results");
      TestOutput.WriteLine($"\ttxt - only patterns are read (each in a separate line)");
      TestOutput.WriteLine($"TestOutputFile is always of type xml. Is is written if the overall test result is passed.");
      TestOutput.WriteLine($"\tIf the input test file is specified and is not preceded by the \\i option, and the output file ");
      TestOutput.WriteLine($"\tis not specified with the \\o option, then the input file may be replaced with new test results.");
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
              case '$':
                options |= TestOptions.TestReplacementPatterns;
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
        if (TestInputData == null || TestInputData.Count == 0)
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


      if (TestRunOptions.HasFlag(TestOptions.TestReplacementPatterns))
      {
        partTestResult = TestReplacementPatterns();
        SetTestResult(ref wholeTestResult, partTestResult);
        if (wholeTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return wholeTestResult;
        ShowTestResult("Replace patterns test result is {0}", partTestResult);
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
        var testSearchPattern = new TestPattern { Pattern = pattern, Result = RegExStatus.OK };
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
          newItem = new RegExQuantifier { Start = 0, Str = pattern, Tag = RegExTag.Quantifier, Status = RegExStatus.Warning };
        else if (AnchorChars.Contains(ch))
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.AnchorControl, Status = RegExStatus.OK };
        else if (ch == '|')
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.AltChar, Status = RegExStatus.Warning };
        else
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.LiteralChar, Status = RegExStatus.OK };
        testSearchPattern.Items.Add(newItem);
        testSearchPattern.Result = newItem.Status;
        TestPattern testOutputItem;
        var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem);
        testOutputData.AddSearchPattern(testOutputItem);
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
        else if (firstChar == '.')
          singleTestResult = Test1stCharDot();
        else if (firstChar == '{')
          singleTestResult = Test1stCharOpeningBrace();
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
        var testSearchPattern = new TestPattern { Pattern = pattern, Result = RegExStatus.OK };
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
        else if (nextChar == 'k')
          newItem = new RegExBackRef { Start = 0, Str = pattern, Tag = RegExTag.BackRef, Status = RegExStatus.Unfinished };
        else
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.EscapedChar, Status = RegExStatus.OK };
        testSearchPattern.Items.Add(newItem);
        testSearchPattern.Result = newItem.Status;
        TestPattern testOutputItem;
        var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem);
        testOutputData.AddSearchPattern(testOutputItem);
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
        var testSearchPattern = new TestPattern { Pattern = pattern, Result = RegExStatus.OK };
        RegExGroup group = new RegExGroup { Start = 0, Str = pattern, Tag = RegExTag.Subexpression, Status = nextChar == ')' ? RegExStatus.OK : RegExStatus.Unfinished };
        testSearchPattern.Items.Add(group);
        testSearchPattern.Result = group.Status;
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
        TestPattern testOutputItem;
        var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem);
        testOutputData.AddSearchPattern(testOutputItem);
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
        var testSearchPattern = new TestPattern { Pattern = pattern, Result = RegExStatus.OK };
        RegExItem newItem = new RegExGroup { Start = 0, Str = new string(')', 1), Tag = RegExTag.Subexpression, Status = RegExStatus.Error };
        testSearchPattern.Items.Add(newItem);
        testSearchPattern.Result = newItem.Status;
        TestPattern testOutputItem;
        var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem);
        testOutputData.AddSearchPattern(testOutputItem);
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
        var testSearchPattern = new TestPattern { Pattern = pattern, Result = RegExStatus.OK };
        RegExCharSet charset = new RegExCharSet { Start = 0, Str = pattern, Tag = RegExTag.CharSet, Status = nextChar == ']' ? RegExStatus.Error : RegExStatus.Unfinished };
        testSearchPattern.Items.Add(charset);
        testSearchPattern.Result = charset.Status;
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
        TestPattern testOutputItem;
        var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem);
        testOutputData.AddSearchPattern(testOutputItem);
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
        var testSearchPattern = new TestPattern { Pattern = pattern, Result = RegExStatus.OK };
        RegExItem firstItem = new RegExItem { Start = 0, Str = new string('|', 1), Tag = RegExTag.AltChar, Status = RegExStatus.Warning };
        testSearchPattern.Items.Add(firstItem);
        var nextResult = Tagger.TryParsePattern(new string(nextChar, 1));
        testSearchPattern.Result = RegExTools.Max(firstItem.Status, nextResult);
        foreach (var nextItem in Tagger.Items)
        {
          nextItem.Start += 1;
          testSearchPattern.Items.Add(nextItem);
          if (nextChar == '|')
          {
            testSearchPattern.Result = nextItem.Status = RegExStatus.Unfinished;
          }
        }

        TestPattern testOutputItem;
        var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem);
        testOutputData.AddSearchPattern(testOutputItem);
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
        var testSearchPattern = new TestPattern { Pattern = pattern, Result = RegExStatus.OK };
        if (nextChar == '?')
        {
          RegExItem newItem = new RegExQuantifier { Start = 0, Str = pattern, Tag = RegExTag.Quantifier, Status = RegExStatus.Warning };
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
        }
        else
        if (nextChar == '|')
        {
          RegExItem newItem = new RegExQuantifier { Start = 0, Str = new string(firstChar, 1), Tag = RegExTag.Quantifier, Status = RegExStatus.Warning };
          testSearchPattern.Items.Add(newItem);
          newItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.AltChar, Status = RegExStatus.Unfinished };
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
        }
        else
        if (firstChar=='{' && char.IsDigit(nextChar))
        {
          RegExItem newItem = new RegExQuantifier { Start = 0, Str = pattern, Tag = RegExTag.Quantifier, Status = RegExStatus.Unfinished };
          testSearchPattern.Items.Add(newItem);
          //newItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.AltChar, Status = RegExStatus.Unfinished };
          //testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
        }
        else
        {
          RegExItem firstItem = new RegExQuantifier { Start = 0, Str = new string(firstChar, 1), Tag = RegExTag.Quantifier, Status = RegExStatus.Warning };
          testSearchPattern.Items.Add(firstItem);
          var nextResult = Tagger.TryParsePattern(new string(nextChar, 1));
          testSearchPattern.Result = RegExTools.Max(firstItem.Status, nextResult);
          foreach (var nextItem in Tagger.Items)
          {
            nextItem.Start += 1;
            testSearchPattern.Items.Add(nextItem);
          }
        }
        TestPattern testOutputItem;
        var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem);
        testOutputData.AddSearchPattern(testOutputItem);
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
        var testSearchPattern = new TestPattern { Pattern = pattern, Result = RegExStatus.OK };
        if (QuantifierChars.Contains(nextChar))
        {
          RegExItem firstItem = new RegExItem { Start = 0, Str = new string(firstChar, 1), Tag = RegExTag.AnchorControl, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(firstItem);
          testSearchPattern.Result = firstItem.Status;
          RegExItem nextItem = new RegExQuantifier{ Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.Quantifier, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(nextItem);
        }
        else
        if (nextChar=='|')
        {
          RegExItem firstItem = new RegExItem { Start = 0, Str = new string(firstChar, 1), Tag = RegExTag.AnchorControl, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(firstItem);
          testSearchPattern.Result = firstItem.Status;
          RegExItem nextItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.AltChar, Status = RegExStatus.Unfinished };
          testSearchPattern.Items.Add(nextItem);
          testSearchPattern.Result = RegExStatus.Unfinished;
        }
        else
        {
          RegExItem firstItem = new RegExItem { Start = 0, Str = new string(firstChar, 1), Tag = RegExTag.AnchorControl, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(firstItem);
          var nextResult = Tagger.TryParsePattern(new string(nextChar, 1));
          testSearchPattern.Result = RegExTools.Max(firstItem.Status, nextResult);
          foreach (var nextItem in Tagger.Items)
          {
            nextItem.Start += 1;
            testSearchPattern.Items.Add(nextItem);
          }
        }
        TestPattern testOutputItem;
        var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem);
        testOutputData.AddSearchPattern(testOutputItem);
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
      }
      return wholeTestResult;
    }

    /// <summary>
    /// Tests two-chars patterns where the first char is '.' 
    /// </summary>
    /// <returns></returns>
    static bool? Test1stCharDot()
    {
      bool? wholeTestResult = null;
      var firstChar = '.';
      for (char nextChar = ' '; nextChar <= '\x7E'; nextChar++)
      {
        var pattern = new string(new char[] { firstChar, nextChar });
        var testSearchPattern = new TestPattern { Pattern = pattern, Result = RegExStatus.OK };
        if (QuantifierChars.Contains(nextChar))
        {
          RegExItem firstItem = new RegExItem { Start = 0, Str = new string(firstChar, 1), Tag = RegExTag.DotChar, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(firstItem);
          testSearchPattern.Result = firstItem.Status;
          RegExItem nextItem = new RegExQuantifier { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.Quantifier, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(nextItem);
        }
        else
        if (nextChar == '|')
        {
          RegExItem firstItem = new RegExItem { Start = 0, Str = new string(firstChar, 1), Tag = RegExTag.DotChar, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(firstItem);
          testSearchPattern.Result = firstItem.Status;
          RegExItem nextItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.AltChar, Status = RegExStatus.Unfinished };
          testSearchPattern.Items.Add(nextItem);
          testSearchPattern.Result = RegExStatus.Unfinished;
        }
        else
        {
          RegExItem firstItem = new RegExItem { Start = 0, Str = new string(firstChar, 1), Tag = RegExTag.DotChar, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(firstItem);
          var nextResult = Tagger.TryParsePattern(new string(nextChar, 1));
          testSearchPattern.Result = RegExTools.Max(firstItem.Status, nextResult);
          foreach (var nextItem in Tagger.Items)
          {
            nextItem.Start += 1;
            testSearchPattern.Items.Add(nextItem);
          }
        }
        TestPattern testOutputItem;
        var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem);
        testOutputData.AddSearchPattern(testOutputItem);
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
      }
      return wholeTestResult;
    }

    /// <summary>
    /// Tests two-chars patterns where the first char is '{' 
    /// </summary>
    /// <returns></returns>
    static bool? Test1stCharOpeningBrace()
    {
      bool? wholeTestResult = null;
      var firstChar = '{';
      for (char nextChar = ' '; nextChar <= '\x7E'; nextChar++)
      {
        var pattern = new string(new char[] { firstChar, nextChar });
        var testSearchPattern = new TestPattern { Pattern = pattern, Result = RegExStatus.OK };
        if (char.IsDigit(nextChar))
        {
          RegExItem quantifier = new RegExQuantifier { Start = 0, Str = pattern, Tag = RegExTag.Quantifier, Status = RegExStatus.Unfinished };
          testSearchPattern.Items.Add(quantifier);
          quantifier.Items.Add(new RegExItem { Tag = RegExTag.Number, Start = 1, Str = new string(nextChar, 1), Status = RegExStatus.OK });
          testSearchPattern.Result = RegExStatus.Unfinished;
        }
        else
        if (nextChar == '|')
        {
          RegExItem firstItem = new RegExItem { Start = 0, Str = new string(firstChar, 1), Tag = RegExTag.LiteralChar, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(firstItem);
          testSearchPattern.Result = firstItem.Status;
          RegExItem nextItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.AltChar, Status = RegExStatus.Unfinished };
          testSearchPattern.Items.Add(nextItem);
          testSearchPattern.Result = RegExStatus.Unfinished;
        }
        else 
        if (AnchorChars.Contains(nextChar) || OpeningChars.Contains(nextChar) || nextChar==')' || nextChar=='.' || nextChar=='\\')
        {
          RegExItem firstItem = new RegExItem { Start = 0, Str = new string(firstChar, 1), Tag = RegExTag.LiteralChar, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(firstItem);
          var nextResult = Tagger.TryParsePattern(new string(nextChar, 1));
          testSearchPattern.Result = RegExTools.Max(firstItem.Status, nextResult);
          foreach (var nextItem in Tagger.Items)
          {
            nextItem.Start += 1;
            testSearchPattern.Items.Add(nextItem);
          }
        }
        else
        if (QuantifierChars.Contains(nextChar))
        {
          RegExItem firstItem = new RegExItem { Start = 0, Str = new string(firstChar, 1), Tag = RegExTag.LiteralChar, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(firstItem);
          RegExItem nextItem = new RegExQuantifier { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.Quantifier, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(nextItem);
        }
        else
        if (nextChar=='}')
        {
          RegExItem firstItem = new RegExQuantifier{ Start = 0, Str = pattern, Tag = RegExTag.Quantifier, Status = RegExStatus.Error };
          testSearchPattern.Items.Add(firstItem);
          testSearchPattern.Result = RegExStatus.Error;
        }
        else
        {
          RegExItem firstItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.LiteralString, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(firstItem);
        }
        TestPattern testOutputItem;
        var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem);
        testOutputData.AddSearchPattern(testOutputItem);
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
        var testSearchPattern = new TestPattern { Pattern = pattern, Result = RegExStatus.OK };
        RegExItem newItem;
        if (OpeningChars.Contains(nextChar) || QuantifierChars.Contains(nextChar) || AnchorChars.Contains(nextChar)
          || nextChar == ')' || nextChar == '\\' || nextChar == '.')
        {
          var tag = (firstChar == '.') ? RegExTag.DotChar : RegExTag.LiteralChar;
          newItem = new RegExItem { Start = 0, Str = new string(firstChar, 1), Tag = tag, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(newItem);
          var nextResult = Tagger.TryParsePattern(new string(nextChar, 1));
          testSearchPattern.Result = RegExTools.Max(newItem.Status, nextResult);
          foreach (var nextItem in Tagger.Items)
          {
            nextItem.Start += 1;
            if (QuantifierChars.Contains(nextChar))
            {
              nextItem.Status = RegExStatus.OK;
              testSearchPattern.Result = RegExStatus.OK;
            }
            testSearchPattern.Items.Add(nextItem);
          }
        }
        else if (nextChar == '|')
        {
          var tag = (firstChar == '.') ? RegExTag.DotChar : RegExTag.LiteralChar;
          newItem = new RegExItem { Start = 0, Str = new string(firstChar, 1), Tag = tag, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(newItem);
          newItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.AltChar, Status = RegExStatus.Unfinished };
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
        }
        else
        {
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.LiteralString, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
        }
        TestPattern testOutputItem;
        var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem);
        testOutputData.AddSearchPattern(testOutputItem);
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
        var testSearchPattern = new TestPattern { Pattern = pattern, Result = RegExStatus.OK };
        RegExItem newItem;
        if (digitCount == 1)
        {
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.BackRef, Status = RegExStatus.Warning };
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
        }
        else
        if (digitCount == 2)
        {
          newItem = new RegExItem { Start = 0, Str = pattern.Substring(0, 2), Tag = RegExTag.BackRef, Status = RegExStatus.Warning };
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
          newItem = new RegExItem { Start = 2, Str = pattern.Substring(2, 1), Tag = RegExTag.LiteralChar, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(newItem);
        }
        else if (code.Contains('8') || code.Contains('9'))
        {
          newItem = new RegExItem { Start = 0, Str = pattern.Substring(0, 2), Tag = RegExTag.BackRef, Status = RegExStatus.Warning };
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
          newItem = new RegExItem { Start = 2, Str = pattern.Substring(2, 2), Tag = RegExTag.LiteralString, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(newItem);
        }
        else
        {
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.OctalSeq, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
        }
        TestPattern testOutputItem;
        var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem);
        testOutputData.AddSearchPattern(testOutputItem);

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
        var testSearchPattern = new TestPattern { Pattern = pattern, Result = RegExStatus.OK };
        RegExItem newItem;
        int k;
        if ((k = code.IndexOf('g')) >= 0)
        {
          newItem = new RegExItem { Start = 0, Str = pattern.Substring(0, k + 3), Tag = RegExTag.HexadecimalSeq, Status = RegExStatus.Error };
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
        }
        else if (digitCount == 1)
        {
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.HexadecimalSeq, Status = RegExStatus.Unfinished };
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
        }
        else
        {
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.HexadecimalSeq, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
        }
        TestPattern testOutputItem;
        var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem);
        testOutputData.AddSearchPattern(testOutputItem);

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
        var testSearchPattern = new TestPattern { Pattern = pattern, Result = RegExStatus.OK };
        RegExItem newItem;
        int k;
        if ((k = code.IndexOf('g')) >= 0)
        {
          newItem = new RegExItem { Start = 0, Str = pattern.Substring(0, k + 3), Tag = RegExTag.UnicodeSeq, Status = RegExStatus.Error };
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
        }
        else if (digitCount < 4)
        {
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.UnicodeSeq, Status = RegExStatus.Unfinished };
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
        }
        else
        {
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.UnicodeSeq, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
        }
        TestPattern testOutputItem;
        var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem);
        testOutputData.AddSearchPattern(testOutputItem);

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
        var testSearchPattern = new TestPattern { Pattern = pattern, Result = RegExStatus.OK };
        RegExItem newItem;
        if (code[0] == '`')
        {
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.ControlCharSeq, Status = RegExStatus.Error };
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
        }
        else
        {
          newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.ControlCharSeq, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
        }
        TestPattern testOutputItem;
        var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem);
        testOutputData.AddSearchPattern(testOutputItem);

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
      var testSearchPattern = new TestPattern { Pattern = pattern, Result = RegExStatus.OK };
      RegExItem newItem;
      if (pattern.Length < 3)
      {
        newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.UnicodeCategorySeq, Status = RegExStatus.Unfinished };
        testSearchPattern.Items.Add(newItem);
        testSearchPattern.Result = newItem.Status;
      }
      else
      if (pattern.Length == 3)
      {
        newItem = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.UnicodeCategorySeq, Status = (pattern[2] == '{') ? RegExStatus.Unfinished : RegExStatus.Error };
        testSearchPattern.Items.Add(newItem);
        testSearchPattern.Result = newItem.Status;
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
        testSearchPattern.Items.Add(newItem);
        testSearchPattern.Result = newItem.Status;
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
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
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
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
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
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
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
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
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
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
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
          testSearchPattern.Items.Add(newItem);
          testSearchPattern.Result = newItem.Status;
        }
      }
      TestPattern testOutputItem;
      var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem);
      testOutputData.AddSearchPattern(testOutputItem);
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
      var testSearchPattern = new TestPattern { Pattern = pattern, Result = RegExStatus.OK };
      RegExCharSet charset = new RegExCharSet { Start = 0, Str = pattern, Tag = RegExTag.CharSet, Status = RegExStatus.OK };
      testSearchPattern.Items.Add(charset);
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
                subItem.Items.Add(new RegExItem { Start = charNdx + 0, Str = new string(thisChar, 1), Tag = RegExTag.LiteralChar, Status = RegExStatus.OK });
                subItem.Items.Add(new RegExItem { Start = charNdx + 1, Str = new string('-', 1), Tag = RegExTag.CharSetControlChar, Status = RegExStatus.OK });
                subItem.Items.Add(new RegExItem { Start = charNdx + 2, Str = new string(nextChar, 1), Tag = RegExTag.LiteralChar, Status = RegExStatus.OK });
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
      testSearchPattern.Result = charset.Status;
      if (testSearchPattern.Result != RegExStatus.Error && charNdx < pattern.Length)
      {
        var status1 = Tagger.TryParsePattern(pattern.Substring(charNdx));
        testSearchPattern.Result = status1;
        int n = 0;
        foreach (var newItem in Tagger.Items)
        {
          if (n == 0 && (newItem.Tag == RegExTag.Quantifier || newItem.Tag == RegExTag.AltChar) && newItem.Status == RegExStatus.Warning)
            testSearchPattern.Result = newItem.Status = RegExStatus.OK;
          n++;
          newItem.MoveStart(charNdx);
          testSearchPattern.Items.Add(newItem);
        }
      }
      TestPattern testOutputItem;
      var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem);
      testOutputData.AddSearchPattern(testOutputItem);
      return singleTestResult;
    }
    #endregion

    #region Test Grouping Patterns
    static bool? TestGroupingPatterns()
    {
      bool? wholeTestResult = null;
      bool? singleTestResult;


      singleTestResult = RunSingleGroupingTest("()");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunNonCapturingGroupTests();
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunNamedGroupingTests('\'');
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunNamedGroupingTests('<');
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunIncompleteGroupingTests('<');
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      return wholeTestResult;
    }

    static string OmitFirstCharInGroup = ")?";
    static string OmitSecondCharInGroup = "\\)?";

    static bool? RunIncompleteGroupingTests(char nextChar)
    {
      bool? wholeTestResult = null;
      bool? singleTestResult;
      foreach (var ch in ValidChars)
      {
        singleTestResult = RunSingleGroupingTest($"({ch}");
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
      }

      foreach (var ch in ValidChars)
      {
        singleTestResult = RunSingleGroupingTest($"({ch})");
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
      }

      foreach (var ch1 in ValidChars)
        foreach (var ch2 in ValidChars)
          if (!OmitFirstCharInGroup.Contains(ch1) && !OmitSecondCharInGroup.Contains(ch1))
          {
            singleTestResult = RunSingleGroupingTest($"({ch1}{ch2}");
            SetTestResult(ref wholeTestResult, singleTestResult);
            if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
              return false;
          }
      return wholeTestResult;
    }

    static bool? RunNamedGroupingTests(char nextChar)
    {
      bool? wholeTestResult = null;
      var closingChar = (nextChar == '<') ? '>' : nextChar;
      bool? singleTestResult;

      singleTestResult = RunNamedCapturingGroupTest($"(?{nextChar}name{closingChar}ok)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunNamedCapturingGroupTest($"(?{nextChar}invalid name{closingChar}ok)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunNamedCapturingGroupTest($"(?{nextChar}name1{closingChar}ok)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunNamedCapturingGroupTest($"(?{nextChar}0name{closingChar}ok)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunNamedCapturingGroupTest($"(?{nextChar}valid_name{closingChar}ok)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunNamedCapturingGroupTest($"(?{nextChar}niebłędna_nazwa{closingChar}ok)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunNamedCapturingGroupTest($"(?{nextChar}unfinished_name`ok)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunNamedCapturingGroupTest($"(?{nextChar}unfinished_expr");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunNamedCapturingGroupTest($"(?{nextChar}balancing-group{closingChar}ok)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunNamedCapturingGroupTest($"(?{nextChar}balancing-group-two{closingChar}ok)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      return wholeTestResult;
    }

    static bool? RunNonCapturingGroupTests()
    {
      bool? wholeTestResult = null;
      bool? singleTestResult;

      singleTestResult = RunNonCapturingGroupTest($"(?:non_capturing_group)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunNonCapturingGroupTest($"(?:)");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunNonCapturingGroupTest($"(?:");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;
      return wholeTestResult;
    }

    static bool? RunSingleGroupingTest(string pattern)
    {
      if (pattern.Length < 3)
        return RunTwoCharGroupingTest(pattern);
      else
      if (pattern.Length == 3)
        return RunThreeCharGroupingTest(pattern);
      else
        throw new InvalidOperationException($"Pattern \"{pattern}\" must be tested using other method");
    }

    static bool? RunTwoCharGroupingTest(string pattern)
    {
      var testSearchPattern = new TestPattern { Pattern = pattern, Result = null };
      RegExGroup group = new RegExGroup { Start = 0, Str = pattern, Tag = RegExTag.Subexpression, Status = RegExStatus.OK };
      testSearchPattern.Items.Add(group);
      int charNdx = pattern.Length;
      if (pattern.Length < 3)
      {
        if (pattern == "()")
          group.Status = RegExStatus.OK;
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
          else if (nextChar == '{')
            subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.LiteralChar, Status = RegExStatus.OK };
          else if (AnchorChars.Contains(nextChar))
            subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.AnchorControl, Status = RegExStatus.OK };
          else if (QuantifierChars.Contains(nextChar))
            subItem = new RegExQuantifier { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.Quantifier, Status = RegExStatus.Warning };
          else
            subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.LiteralChar, Status = RegExStatus.OK };
          group.Items.Add(subItem);
        }
      }
      return FinishSingleGroupingTest(testSearchPattern, group, pattern, charNdx);
    }

    static bool? RunThreeCharGroupingTest(string pattern)
    {
      if (pattern.Last() == ')')
        return RunThreeCharClosedGroupingTest(pattern);
      else
        return RunThreeCharOpenedGroupingTest(pattern);
    }

    static bool? RunThreeCharClosedGroupingTest(string pattern)
    {
      var testSearchPattern = new TestPattern { Pattern = pattern, Result = null };
      RegExGroup group = new RegExGroup { Start = 0, Str = pattern, Tag = RegExTag.Subexpression, Status = RegExStatus.OK };
      testSearchPattern.Items.Add(group);
      int charNdx = pattern.Length;
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
          testSearchPattern.Items.Add(nextItem);
          testSearchPattern.Result = RegExStatus.Error;
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
          subItem = new RegExQuantifier { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.Quantifier, Status = RegExStatus.Warning };
          group.Status = RegExStatus.OK;
        }
        else
          subItem = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.LiteralChar, Status = RegExStatus.OK };
        if (subItem != null)
          group.Items.Add(subItem);
      }
      return FinishSingleGroupingTest(testSearchPattern, group, pattern, charNdx);
    }

    static bool? RunThreeCharOpenedGroupingTest(string pattern)
    {
      var testSearchPattern = new TestPattern { Pattern = pattern, Result = null };
      RegExGroup group = new RegExGroup { Start = 0, Str = pattern, Tag = RegExTag.Subexpression, Status = RegExStatus.OK };
      testSearchPattern.Items.Add(group);
      int charNdx = pattern.Length;
      if (pattern.Length == 3)
      {
        group.Status = RegExStatus.Unfinished;
        var nextChar = pattern[1];
        RegExItem subItem;
        if (nextChar == '\\') // pattern starts with @"(\"
        {
          subItem = new RegExItem { Start = 1, Str = pattern.Substring(1), Tag = RegExTag.EscapedChar, Status = RegExStatus.OK };
          group.Items.Add(subItem);
          group.Status = subItem.Status;
        }
        else
        {
          Tagger.TryParsePattern(pattern.Substring(1));
          var items = Tagger.Items;
          foreach (var item in items)
          {
            item.MoveStart(1);
            group.Items.Add(item);
          }
        }
      }
      return FinishSingleGroupingTest(testSearchPattern, group, pattern, charNdx);
    }

    static bool? FinishSingleGroupingTest(TestPattern testSearchPattern, RegExGroup group, string pattern, int charNdx)
    {
      if (testSearchPattern.Result == null)
        testSearchPattern.Result = group.Status;
      if (testSearchPattern.Result != RegExStatus.Error && charNdx < pattern.Length)
      {
        var status1 = Tagger.TryParsePattern(pattern.Substring(charNdx));
        testSearchPattern.Result = status1;
        int n = 0;
        foreach (var newItem in Tagger.Items)
        {
          if (n == 0 && (newItem.Tag == RegExTag.Quantifier || newItem.Tag == RegExTag.AltChar) && newItem.Status == RegExStatus.Warning)
            testSearchPattern.Result = newItem.Status = RegExStatus.OK;
          n++;
          newItem.MoveStart(charNdx);
          testSearchPattern.Items.Add(newItem);
        }
      }
      TestPattern testOutputItem;
      var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem);
      testOutputData.AddSearchPattern(testOutputItem);
      return singleTestResult;
    }

    static bool? RunNamedCapturingGroupTest(string pattern)
    {
      RegExGroup group = new RegExGroup { Start = 0, Str = pattern, Tag = RegExTag.NamedGroup, Status = RegExStatus.OK };
      int charNdx = 1;
      var nextChar = charNdx < pattern.Length ? pattern[charNdx] : '\0';
      if (nextChar == '\0')
        group.Status = RegExStatus.Unfinished;
      else
      if (nextChar == '?')
      {
        var groupControlChar = new RegExItem { Start = 1, Str = new string(nextChar, 1), Tag = RegExTag.GroupControlChar, Status = RegExStatus.OK };
        group.Items.Add(groupControlChar);
        charNdx++;
        nextChar = nextChar = charNdx < pattern.Length ? pattern[charNdx] : '\0';
        if (nextChar == '\0')
          group.Status = RegExStatus.Unfinished;
        else
        if ((nextChar == '\'') || (nextChar == '<'))
        {
          group.Tag = RegExTag.NamedGroup;
          var closingChar = (nextChar == '<') ? '>' : nextChar;
          var nameQuote = CreateNameQuote(group, pattern, ref charNdx, closingChar);
          group.Items.Add(nameQuote);
          nextChar = nextChar = charNdx < pattern.Length ? pattern[charNdx] : '\0';
          while (nextChar == '-')
          { // add second name to balancing group name quote
            var isOK = group.Tag != RegExTag.BalancingGroup;
            group.Tag = RegExTag.BalancingGroup;
            var dashItem = new RegExItem { Tag = RegExTag.GroupControlChar, Start = charNdx, Str = pattern.Substring(charNdx, 1), Status = isOK ? RegExStatus.OK : RegExStatus.Error };
            nameQuote.Items.Add(dashItem);
            nameQuote.Str += dashItem.Str;
            charNdx++;
            AddNameToNameQuote(nameQuote, pattern, ref charNdx, closingChar, isOK);
            nextChar = nextChar = charNdx < pattern.Length ? pattern[charNdx] : '\0';
            if (nextChar == '\0')
            {
              group.Status = RegExStatus.Unfinished;
              break;
            }
            else
              group.Status = isOK ? RegExStatus.OK : RegExStatus.Error;
          }
          if (nextChar != '\0' && nameQuote.Status == RegExStatus.Unfinished)
          { // mark syntax error
            group.Items.Add(new RegExItem { Tag = RegExTag.GroupControlChar, Start = charNdx, Str = pattern.Substring(charNdx, 1), Status = RegExStatus.Error });
            group.Status = RegExStatus.Error;
            charNdx++;
          }
          if (charNdx < pattern.Length)
            PrepareRestGroupItemsToTest(group, pattern, ref charNdx);
        }
        else
          throw new InvalidOperationException($"Pattern \"{pattern}\" is not a named capturing group");
      }
      return FinishGroupTest(group, pattern, charNdx);
    }

    static RegExNameQuote CreateNameQuote(RegExGroup group, string pattern, ref int charNdx, char closingChar)
    {
      var nameQuote = new RegExNameQuote { Tag = RegExTag.NameQuote, Start = charNdx, Str = pattern.Substring(charNdx, 1), Status = RegExStatus.Unfinished };
      charNdx++;
      var groupName = AddNameToNameQuote(nameQuote, pattern, ref charNdx, closingChar, true);
      group.Name = groupName;
      if (nameQuote.Status == RegExStatus.Unfinished)
        group.Status = nameQuote.Status;
      else
      if (!IsValidName(groupName))
        group.Status = RegExStatus.Warning;
      return nameQuote;
    }

    static string AddNameToNameQuote(RegExNameQuote nameQuote, string pattern, ref int charNdx, char closingChar, bool isOK)
    {
      var seqStart = charNdx;
      String groupName = null;
      char curChar = '\0';
      var k = pattern.IndexOfAny(new char[] { closingChar, '-' }, charNdx);
      if (k > 0)
      {
        charNdx = k;
        curChar = (charNdx < pattern.Length) ? pattern[charNdx] : '\0';
        if (curChar == closingChar)
        {
          nameQuote.Str += pattern.Substring(seqStart, charNdx - seqStart + 1);
          groupName = pattern.Substring(seqStart, charNdx - seqStart);
          charNdx++;
          nameQuote.Status = RegExStatus.OK;
        }
        else
        {
          nameQuote.Str += pattern.Substring(seqStart, charNdx - seqStart);
          groupName = pattern.Substring(seqStart, charNdx - seqStart);
          nameQuote.Status = RegExStatus.OK;
        }
      }
      else
      {
        for (; charNdx < pattern.Length + 1; charNdx++)
        {
          curChar = (charNdx < pattern.Length) ? pattern[charNdx] : '\0';
          if (Char.IsLetterOrDigit(curChar) || curChar == ' ' || curChar == '_')
            groupName += curChar;
          else
            break;
        }
        nameQuote.Str += pattern.Substring(seqStart, charNdx - seqStart);
        if (curChar != closingChar && curChar != '-')
          nameQuote.Status = RegExStatus.Unfinished;
      }
      if (groupName != null)
      {
        var groupNameItem = new RegExGroupName { Start = seqStart, Str = groupName, Tag = RegExTag.GroupName, Status = isOK ? RegExStatus.OK : RegExStatus.Warning };
        nameQuote.Items.Add(groupNameItem);
        if (!IsValidName(groupName))
          groupNameItem.Status = RegExStatus.Warning;
        if (nameQuote.Status == RegExStatus.Unfinished)
          groupNameItem.Status = nameQuote.Status;
      }
      return groupName;
    }

    static bool IsValidName(string groupName)
    {
      return !(groupName.Contains(' ') || Char.IsDigit(groupName.FirstOrDefault()) || groupName.Contains('-'));
    }

    static bool? RunNonCapturingGroupTest(string pattern)
    {
      RegExGroup group = new RegExGroup { Start = 0, Str = pattern, Tag = RegExTag.NonCapturingGroup, Status = RegExStatus.OK };
      int charNdx = 1;
      var nextChar = charNdx < pattern.Length ? pattern[charNdx] : '\0';
      if (nextChar == '\0')
        group.Status = RegExStatus.Unfinished;
      else if (nextChar == '?')
      {
        var groupControlChar = new RegExItem { Start = charNdx, Str = new string(nextChar, 1), Tag = RegExTag.GroupControlChar, Status = RegExStatus.OK };
        group.Items.Add(groupControlChar);
        charNdx++;
        nextChar = charNdx < pattern.Length ? pattern[charNdx] : '\0';
        if (nextChar == '\0')
          group.Status = RegExStatus.Unfinished;
        else
        if (nextChar == ':')
        {
          group.Tag = RegExTag.NonCapturingGroup;
          groupControlChar.Status = RegExStatus.OK;
          group.Items.Add(new RegExItem { Start = charNdx, Str = new string(':', 1), Tag = RegExTag.GroupControlChar, Status = RegExStatus.OK });
          charNdx++;
          PrepareRestGroupItemsToTest(group, pattern, ref charNdx);
        }
        else
          throw new InvalidOperationException($"Pattern \"{pattern}\" is not a non-capturing group");
      }
      return FinishGroupTest(group, pattern, charNdx);
    }

    static void PrepareRestGroupItemsToTest(RegExGroup group, string pattern, ref int charNdx)
    {
      if (pattern.Length > charNdx)
      {
        int k = pattern.IndexOf(')', charNdx);
        var restStr = pattern.Substring(charNdx, k - charNdx);
        Tagger.TryParsePattern(restStr);
        foreach (var item in Tagger.Items)
        {
          item.MoveStart(charNdx);
          group.Items.Add(item);
        }
        charNdx = k + 1;
      }
      else
        group.Status = RegExStatus.Unfinished;
    }

    static bool? FinishGroupTest(RegExGroup group, string pattern, int charNdx)
    {
      var testSearchPattern = new TestPattern { Pattern = pattern, Result = null };
      testSearchPattern.Items.Add(group);
      if (testSearchPattern.Result == null)
        testSearchPattern.Result = group.Status;
      if (testSearchPattern.Result != RegExStatus.Error && charNdx < pattern.Length)
      {
        var status1 = Tagger.TryParsePattern(pattern.Substring(charNdx));
        testSearchPattern.Result = status1;
        int n = 0;
        foreach (var newItem in Tagger.Items)
        {
          if (n == 0 && (newItem.Tag == RegExTag.Quantifier || newItem.Tag == RegExTag.AltChar) && newItem.Status == RegExStatus.Warning)
            testSearchPattern.Result = newItem.Status = RegExStatus.OK;
          n++;
          newItem.MoveStart(charNdx);
          testSearchPattern.Items.Add(newItem);
        }
      }
      TestPattern testOutputItem;
      var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem);
      testOutputData.AddSearchPattern(testOutputItem);
      return singleTestResult;
    }

    #endregion

    #region Test Replacement Patterns

    static bool? TestReplacementPatterns()
    {
      bool? wholeTestResult = null;
      bool? singleTestResult;


      singleTestResult = RunTwoCharReplacementTests();
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunNamedGroupReplacementTests();
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      return wholeTestResult;
    }

    static bool? RunTwoCharReplacementTests()
    {
      bool? wholeTestResult = null;
      foreach (char secondChar in ValidChars)
        if (secondChar != '{')
        {
          bool? singleTestResult;
          singleTestResult = RunTwoCharReplacementTest($"${secondChar}");

          SetTestResult(ref wholeTestResult, singleTestResult);
          if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
            return false;
        }
      return wholeTestResult;
    }

    static bool? RunTwoCharReplacementTest(string pattern)
    {
      var testSearchPattern = new TestPattern { Pattern = pattern, Result = RegExStatus.OK };
      RegExItem replacement;
      int charNdx = pattern.Length;
      if (pattern.Length < 3)
      {
        var nextChar = pattern[1];
        if (LiteralReplacementChars.Contains(nextChar))
        {
          replacement = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.Replacement, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(replacement);
        }
        else
        if (char.IsDigit(nextChar))
        {
          replacement = new RegExBackRef { Start = 0, Str = pattern, Tag = RegExTag.Replacement, Status = RegExStatus.OK, Number = (int)(nextChar - '0') };
          testSearchPattern.Items.Add(replacement);
        }
        else
        if (nextChar == '\\')
        {
          replacement = new RegExItem { Start = 0, Str = pattern.Substring(0, 1), Tag = RegExTag.LiteralChar, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(replacement);
          replacement = new RegExItem { Start = 1, Str = pattern.Substring(1, 1), Tag = RegExTag.EscapedChar, Status = RegExStatus.Unfinished };
          testSearchPattern.Items.Add(replacement);
          testSearchPattern.Result = RegExStatus.Unfinished;
        }
        else
        {
          replacement = new RegExItem { Start = 0, Str = pattern, Tag = RegExTag.LiteralString, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(replacement);
        }
      }
      TestPattern testOutputItem;
      var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem, SearchOrReplace.Replace);
      testOutputData.AddSearchPattern(testOutputItem);
      return singleTestResult;
    }


    static bool? RunNamedGroupReplacementTests()
    {
      bool? wholeTestResult = null;
      bool? singleTestResult;

      var testString = "${name}";
      for (int n = 1; n <= testString.Length; n++)
      {
        var substring = testString.Substring(0, n);
        singleTestResult = RunNamedGroupReplacementTest(substring);
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
      }

      singleTestResult = RunNamedGroupReplacementTest("${invalid name}");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunNamedGroupReplacementTest("${NiebłędnaNazwa}");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunNamedGroupReplacementTest("${0invalidname}");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunNamedGroupReplacementTest("${invalid-name}");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      singleTestResult = RunNamedGroupReplacementTest("${invalid-name-unfinished");
      SetTestResult(ref wholeTestResult, singleTestResult);
      if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
        return false;

      return wholeTestResult;
    }

    static bool? RunNamedGroupReplacementTest(string pattern)
    {
      var testSearchPattern = new TestPattern { Pattern = pattern, Result = RegExStatus.OK };
      RegExItem replacement;
      int charNdx = pattern.Length;
      if (pattern.Length == 1)
      {
        replacement = new RegExItem { Start = 0, Str = pattern.Substring(0, 1), Tag = RegExTag.LiteralChar, Status = RegExStatus.OK };
        testSearchPattern.Items.Add(replacement);
      }
      else
      {
        var nextChar = pattern.LastOrDefault();
        if (nextChar == '}')
        {
          testSearchPattern.Result = RegExStatus.OK;
          replacement = new RegExBackRef { Start = 0, Str = pattern, Tag = RegExTag.Replacement, Status = RegExStatus.OK };
          testSearchPattern.Items.Add(replacement);
          var nameQuote = new RegExNameQuote { Start = 1, Str = pattern.Substring(1), Tag = RegExTag.NameQuote, Status = RegExStatus.OK };
          replacement.Items.Add(nameQuote);
          if (pattern.Length > 2)
          {
            var name = pattern.Substring(2, pattern.Length - 3);
            testSearchPattern.Result = PrepareNameQuote(replacement as RegExBackRef, nameQuote, name);
          }
        }
        else
        {
          testSearchPattern.Result = RegExStatus.Unfinished;
          replacement = new RegExBackRef { Start = 0, Str = pattern, Tag = RegExTag.Replacement, Status = RegExStatus.Unfinished };
          testSearchPattern.Items.Add(replacement);
          var nameQuote = new RegExNameQuote { Start = 1, Str = pattern.Substring(1), Tag = RegExTag.NameQuote, Status = RegExStatus.Unfinished };
          replacement.Items.Add(nameQuote);
          if (pattern.Length > 2)
          {
            var name = pattern.Substring(2);
            testSearchPattern.Result = PrepareNameQuote(replacement as RegExBackRef, nameQuote, name);
          }
        }
      }
      TestPattern testOutputItem;
      var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem, SearchOrReplace.Replace);
      testOutputData.AddSearchPattern(testOutputItem);
      return singleTestResult;
    }

    static RegExStatus PrepareNameQuote(RegExBackRef replacement, RegExNameQuote nameQuote, string name)
    {
      RegExGroupName nameItem = null;

      if (name.Contains('-'))
      {
        var names = name.Split('-');
        int namePos = 2;
        for (int i = 0; i < names.Count(); i++)
        {
          if (i > 0)
          {
            var dashItem = new RegExItem { Start = namePos, Str = "-", Tag = RegExTag.GroupControlChar, Status = (i==1) ? RegExStatus.Error : RegExStatus.Warning };
            namePos++;
            nameQuote.Items.Add(dashItem);
          }
          else
            replacement.Name = names[i];
          nameItem = new RegExGroupName { Start = namePos, Str = names[i], Tag = RegExTag.GroupName, Status = (i == 0) ? RegExStatus.OK : RegExStatus.Warning };
          namePos += names[i].Length;
          nameQuote.Items.Add(nameItem);
        }
        return replacement.Status = nameQuote.Status = RegExStatus.Error;
      }
      else
      {
        nameItem = new RegExGroupName { Start = 2, Str = name, Tag = RegExTag.GroupName, Status = replacement.Str.EndsWith("}") ? RegExStatus.OK : RegExStatus.Unfinished };
        nameQuote.Items.Add(nameItem);
        replacement.Name = name;
      }
      if (!IsValidName(replacement.Name))
      {
        return replacement.Status = nameItem.Status = RegExStatus.Warning;
      }
      return replacement.Str.EndsWith("}") ?  RegExStatus.OK: RegExStatus.Unfinished;
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
        var testItem = TestInputData[i];
        TestPattern testOutputPattern;
        var singleTestResult = RunSingleTest(testItem.Search, out testOutputPattern);
        testOutputData.AddSearchPattern(testOutputPattern);
        SetTestResult(ref wholeTestResult, singleTestResult);
        if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
          return false;
        if (testItem.Replace != null)
        {
          singleTestResult = RunSingleTest(testItem.Replace, out testOutputPattern, SearchOrReplace.Replace);
          testOutputData.AddReplacePattern(testOutputPattern);
          SetTestResult(ref wholeTestResult, singleTestResult);
          if (singleTestResult == false && TestRunOptions.HasFlag(TestOptions.BreakAfterFirstFailure))
            return false;
        }
      }
      return wholeTestResult;
    }

    /// <summary>
    /// Runs a single test of one pattern
    /// </summary>
    /// <param name="testInputPattern"></param>
    /// <param name="testOutputPattern"></param>
    /// <returns></returns>
    static bool? RunSingleTest(TestPattern testInputPattern, out TestPattern testOutputPattern, SearchOrReplace kind = SearchOrReplace.Search)
    {
      var pattern = testInputPattern.Pattern;
      TestOutput.WriteLine($"pattern = \"{pattern}\"");
      testOutputPattern = new TestPattern { Pattern = pattern };
      Tagger.Kind = kind;
      testOutputPattern.Result = Tagger.TryParsePattern(pattern);
      testOutputPattern.Items = Tagger.Items;
      bool? singleTestResult = null;
      if (testInputPattern.Result != null)
      {
        singleTestResult = (testInputPattern.Items == null) || testOutputPattern.Items.Equals(testInputPattern.Items)
          && (testInputPattern.Result == testOutputPattern.Result);
      }
      if (testInputPattern.PatternItems != null)
      {
        Generator.Kind = kind;
        testOutputPattern.PatternItems = Generator.GeneratePatternItems(testOutputPattern.Items);
        if (singleTestResult != false)
          singleTestResult = PatternItemsComparator.AreEqual(testOutputPattern.PatternItems, testInputPattern.PatternItems);
      }

      if (singleTestResult == false)
      {
        TestOutput.WriteLine($"Expected:");
        ShowTestItem(testInputPattern);
        TestOutput.WriteLine($"Obtained:");
        ShowTestItem(testOutputPattern);
      }
      else if (singleTestResult != true || !TestRunOptions.HasFlag(TestOptions.SuppressPassedTestResults))
        ShowTestItem(testOutputPattern);
      if (testInputPattern.Result != null)
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
          var testSearchPattern = new TestPattern { Pattern = pattern };
          TestPattern testOutputItem;
          var singleTestResult = RunSingleTest(testSearchPattern, out testOutputItem);
          TestOutput.WriteLine("");
          Console.Write("Do you accept test result? (Y/N) ");
          var key1 = Console.ReadKey();
          Console.WriteLine();
          if (key1.Key == ConsoleKey.Y)
          {
            testOutputData.AddSearchPattern(testOutputItem);
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
      TestPattern testPattern = null;
      int lineNo = 0;
      while ((line = reader.ReadLine()) != null)
      {
        lineNo++;
        if (line.Trim() != "")
        {
          if (line[0] == '\t')
          {
            if (testPattern != null)
            {
              line = line.Substring(1);
              var cols = line.Split('\t');
              if (cols.Length != 2)
              {
                result.RemoveAt(result.Count - 1);
                Console.WriteLine($"Error while reading test file \"{filename}\"");
                Console.WriteLine($"  in line {lineNo}:");
                Console.WriteLine($"    As the line starts with a tab character, the line is treated as two column pattern parse result.");
                Console.WriteLine($"    Columns should be separated with tab characters.");
                Console.WriteLine($"    There were {cols.Length} columns found");
                testPattern = null; // no further trials for this pattern.
              }
              else
              {
                testPattern.Result = RegExStatus.OK;
                if (testPattern.PatternItems == null)
                  testPattern.PatternItems = new PatternItems();
                testPattern.PatternItems.Add(new PatternItem { Str = cols[0], Description = cols[1].Trim() });
              }
            }
            else
            {
              Debug.WriteLine("Result data encountered without preceding pattern data:");
              Debug.WriteLine(line);
              throw new IOException("Result data encountered without preceding pattern data");
            }
          }
          else
          {
            if (testPattern == null)
            {
              testPattern = new TestPattern { Pattern = line };
              result.AddSearchPattern(testPattern);
            }
            else
            {
              testPattern = new TestPattern { Pattern = line };
              result.AddReplacePattern(testPattern);
            }
          }
        }
        else
          testPattern = null;
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

    static void ShowTestItem(TestPattern item)
    {
      TestOutput.WriteLine($"TryParse = {item.Result}");
      if (item.Items.Count > 0)
        Presenter.DrawGraph(TestOutput, item.Pattern, item.Items);
      if (item.PatternItems != null)
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
