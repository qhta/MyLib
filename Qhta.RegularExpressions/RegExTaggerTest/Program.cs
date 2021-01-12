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
    const string QuantifierChars = "?+*";
    const string AnchorChars = "^$";

    static List<string> testPatterns = new List<string>
    {
      //@"\b(\w+?)\s\1\b",
      //@"((\w+)[\s.])+.",
      //@"(?<name_012>subexpression[a-zA-Z0-9])",
    };

    static string testInputFileName = null;
    static string testOutputFileName = null;
    static bool testOneChars = false;
    static bool testTwoChars = false;
    static bool isInput = false;
    static bool isOutput = false;

    static TestData testInputData, testOutputData;
    static RegExTagger Tagger = new RegExTagger();

    static void Main(string[] args)
    {


      if (args.Length == 0)
      {

      }
      else
      {
        for (int i = 0; i < args.Length; i++)
        {
          //Console.WriteLine($"args[{i}]: {args[i]}");
          var arg = args[i];
          if (arg[0] == '/')
          {
            switch (arg[1])
            {
              case 'o':
                isOutput = true;
                break;
              case 'i':
                isInput = true;
                break;
              case '1':
                testOneChars = true;
                break;
              case '2':
                testTwoChars = true;
                break;
            }
          }
          else
          {
            if (isOutput)
            {
              testOutputFileName = arg;
            }
            else
            {
              testInputFileName = arg;
            }
          }
        }
        Console.WriteLine($"");
      }

      if (testOutputFileName == null && !isInput)
        testOutputFileName = testInputFileName;

      if (testInputFileName != null)
      {
        Console.WriteLine($"reading test data from file \"{testInputFileName}\"");
        ReadTestData(testInputFileName);
        Console.WriteLine($"read tests: {testInputData.Count}");
        Console.WriteLine($"");
      }
      else
        CreateTestData();
      testOutputData = new TestData();

      bool? wholeTestResult = null;

      if (testOneChars)
        wholeTestResult = TestOneCharPatterns();

      if (testInputData.Count>0)
        wholeTestResult = TestInputData();

      TrySaveTestData(wholeTestResult);

      bool moreTestsAdded = TryAddMoreTests();

      if (moreTestsAdded)
        TrySaveAddedTests();
    }

    static void WriteUsage()
    {
      Console.WriteLine($"arguments: [testInputFileName [/o testOutputFileName]]");
      Console.WriteLine($"options:");
      Console.WriteLine($"\t/1 - test one-char patterns");
      Console.WriteLine($"\t/2 - test two-chars patterns");
      Console.WriteLine($"\t/i - intput file is input-only (is not rewritten)");
    }

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

    static bool? RunOneTest(TestItem testInputItem, out TestItem testOutputItem)
    {
      var pattern = testInputItem.Pattern;
      Console.WriteLine($"pattern = {pattern}");
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
          Console.WriteLine($"test PASSED");
        else if (testResultOK == false)
          Console.WriteLine($"test FAILED");
      }
      Console.WriteLine($"Result = {testOutputItem.Result}");
      Dump(testOutputItem.Items);
      Console.WriteLine($"");
      return testResultOK;
    }

    static bool? TestInputData()
    {
      bool? wholeTestResult = null;
      for (int i = 0; i < testInputData.Count; i++)
      {
        Console.WriteLine($"Test {i + 1}:");
        var testInputItem = testInputData[i];
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

    static void CreateTestData()
    {
      testInputData = new TestData();
      foreach (var pattern in testPatterns)
      {
        var item = new TestItem { Pattern = pattern };
        testInputData.Add(item);
      }
    }

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

    static bool TrySaveTestData(bool? wholeTestResult)
    {
      bool testDataWritten = false;
      if (testOutputFileName != null)
      {
        if (wholeTestResult == false)
        {
          Console.Write($"Test results differ from expected. Do you want to write test data to file \"{testOutputFileName}\"? (Y/N)");
          var key1 = Console.ReadKey();
          Console.WriteLine();
          if (key1.Key == ConsoleKey.Y)
          {
            WriteTestData(testOutputFileName);
            Console.WriteLine($"written tests: {testOutputData.Count}");
            Console.WriteLine("");
            testDataWritten = true;
          }
        }
        else if (testOutputFileName != testInputFileName)
        {
          Console.WriteLine($"Writting test data to file \"{testOutputFileName}\\");
          WriteTestData(testOutputFileName);
          Console.WriteLine($"written tests: {testOutputData.Count}");
          Console.WriteLine("");
          testDataWritten = true;
        }
      }
      return testDataWritten;
    }

    static bool TryAddMoreTests()
    {
      bool added = false;
      bool shouldRepeat = true;
      while (shouldRepeat)
      {
        Console.Write("Do you want do run new test? (Y/N) ");
        var key2 = Console.ReadKey();
        Console.WriteLine();
        if (key2.Key == ConsoleKey.Y)
        {
          Console.Write("enter test pattern:");
          var pattern = Console.ReadLine();
          if (pattern == "")
            break;
          var testInputItem = new TestItem { Pattern = pattern };
          TestItem testOutputItem;
          var oneTestResult = RunOneTest(testInputItem, out testOutputItem);
          Console.WriteLine("");
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

    static bool TrySaveAddedTests()
    {
      bool saved = false;
      Console.Write($"You have added some test. Do you want to save all test data to file \"{testOutputFileName}\"? (Y/N) ");
      var key1 = Console.ReadKey();
      Console.WriteLine();
      if (key1.Key == ConsoleKey.Y)
      {
        WriteTestData(testOutputFileName);
        Console.WriteLine($"written tests: {testOutputData.Count}");
        Console.WriteLine("");
        saved = true;
      }
      return saved;
    }

    static void ReadTestData(string filename)
    {
      using (var reader = File.OpenText(filename))
      {
        testInputData = (TestData)XamlServices.Load(reader);
      }
    }

    static void WriteTestData(string filename)
    {
      using (var writer = File.CreateText(filename))
      {
        XamlServices.Save(writer, testOutputData);
      }
    }


    static void Dump(RegExItems items, int level = 0)
    {
      foreach (var item in items)
      {
        var s = $"{item.GetType().Name}: {item}";
        Console.WriteLine(new string(' ', level * 2) + s);
        if (item is RegExGroup group)
        {
          if (group.Name != null)
            Console.WriteLine(new string(' ', (level + 1) * 2) + $"Name=\"{group.Name}\"");
          Dump(group.Items, level + 1);
        }
        else
        if (item is RegExCharset charset)
          Dump(charset.Items, level + 1);
      }
    }
  }
}
