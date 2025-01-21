using System.Diagnostics;

using Qhta.TextUtils;

namespace Qhta.Unicode.Build;

internal class Program
{
  static void Main(string[] args)
  {
   UnicodeData ucd = UnicodeData.Instance;
    Console.WriteLine($"{ucd.Count} records loaded");
    foreach (var entry in ucd.Where(item => item.Value.SimpleUppercaseMapping is not null || item.Value.SimpleLowercaseMapping is not null || item.Value.SimpleTitlecaseMapping is not null))
    {
      Console.WriteLine($"{entry.Value.CodePoint}: {entry.Value.Name.ToString(),-60} {entry.Value.Category} {entry.Value.CCClass,-5} {entry.Value.BiDiClass,-4} {entry.Value.DecDigit,-1} {entry.Value.Digit,-1} {entry.Value.NumVal,-11} {HexString(entry.Value.SimpleUppercaseMapping),-5} {HexString(entry.Value.SimpleLowercaseMapping),-5} {HexString(entry.Value.SimpleTitlecaseMapping),-5} {entry.Value.Decomposition}");
    }

    var t0 = DateTime.Now;
    List<CodePoint> codePoints1 = new();
    for (int i = 0; i < 100; i++)
    {
      codePoints1 = ucd.Where(item => item.Value.Name.Contains("LATIN")).Select(item => item.Value.CodePoint).ToList();
    }
    var t1 = DateTime.Now;
    var s1 = $"Time of 100 x where \"LATIN\" using LINQ query:";
    Console.WriteLine($"{s1,-70} {t1 - t0}, found {codePoints1.Count()} code points");

    List<CodePoint> codePoints2 = new();
    for (int i = 0; i < 100; i++)
    {
      codePoints2 = ucd.SearchNames("LATIN").ToList();
    }
    var t2 = DateTime.Now;
    var s2 = $"Time of 100 x where \"LATIN\" using name index:";
    Console.WriteLine($"{s2,-70} {t2 - t1}, found {codePoints2.Count()} code points");

    List<CodePoint> codePoints3 = new();
    for (int i = 0; i < 100; i++)
    {
      codePoints3 = new SortedSet<CodePoint>(ucd.Where(item => item.Value.GetAllNames().Any(name => name.IsLike("LATIN*LETTER*")))
        .Select(item => item.Value.CodePoint)).ToList();
    }
    var t3 = DateTime.Now;
    var s3 = $"Time of 100 find like \"LATIN*LETTER*\" using LINQ query:";
    Console.WriteLine($"{s3,-70} {t3 - t2}, found {codePoints3.Count()} code points");

    List<CodePoint> codePoints4 = new ();
    for (int i = 0; i < 100; i++)
    {
      codePoints4 = ucd.SearchNames("LATIN*LETTER*").ToList();
    }
    var t4 = DateTime.Now;
    var s4 = $"Time of 100 find like \"LATIN*LETTER*\" using name index:";
    Console.WriteLine($"{s4,-70} {t4 - t3}, found {codePoints4.Count()} code points");

    List<int>[] codePoints5 = new List<int>[(int)DecompositionType.Compat+1];
    for (int i = 0; i < 100; i++)
    {
      for (DecompositionType type = DecompositionType.Unknown; type <= DecompositionType.Compat; type++)
      {
        codePoints5[(int)type] = ucd.Where(item => item.Value.Decomposition != null &&
          ((item.Value.Decomposition.Type ?? DecompositionType.Unknown) == type)).Select(item=>item.Key).ToList();
      }
    }
    var t5 = DateTime.Now;
    var s5 = $"Time of 1000 find all decomposition types using LINQ query:";
    Console.WriteLine($"{s5,-70} {t5 - t4}, found {codePoints5.Sum(item=>item.Count())} code points");

    List<CodePoint>[] codePoints6 = new List<CodePoint>[(int)DecompositionType.Compat + 1];
    for (int i = 0; i < 100; i++)
    {
      for (DecompositionType type = DecompositionType.Unknown; type <= DecompositionType.Compat; type++)
      {
        codePoints6[(int)type] = ucd.SearchDecomposition(type).ToList();
      }
    }
    var t6 = DateTime.Now;
    var s6 = $"Time of 1000 find all decomposition types using decomposition index:";
    Console.WriteLine($"{s6,-70} {t6 - t5}, found {codePoints6.Sum(item => item.Count())} code points");

    Console.WriteLine("done");

  }

  private static string HexString(int? val)
  {
    return val.HasValue ? val.Value.ToString("X4") : "    ";
  }
}
