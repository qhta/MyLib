using System.Diagnostics;

using Qhta.TextUtils;

namespace Qhta.Unicode.Build;

internal class Program
{
  static async Task Main(string[] args)
  {
    var ucdFileName = "UnicodeData.txt";
    var aliasesFileName = "NameAliases.txt";
    await DownloadFileNameAsync(ucdFileName);
    await DownloadFileNameAsync(aliasesFileName);

    UnicodeData ucd = new UnicodeData(ucdFileName);
    Console.WriteLine($"{ucd.Count} records loaded");
    //foreach (var entry in ucd.Where(item => item.Value.Decomposition?.Type!=null))
    //{
    //  Console.WriteLine($"{entry.Value.CodePoint:X4}: {entry.Value.Name.ToString(),-60} {entry.Value.Category} {entry.Value.CCClass,-5} {entry.Value.BiDiClass,-4} {entry.Value.Decomposition}");
    //}

    int aliasCount = ucd.LoadAliases(aliasesFileName);
    Console.WriteLine($"{aliasCount} name aliases loaded");
    NameWordIndex nameIndex = ucd.NameWordIndex;
    //foreach (KeyValuePair<string, List<int>> entry in nwi.Take(100))
    //{
    //  Console.WriteLine($"{entry.Key}: {entry.Value.Count}");
    //}
    Console.WriteLine($"{nameIndex.Count} string items created");

    var t0 = DateTime.Now;
    List<int> codePoints1 = new();
    for (int i = 0; i < 100; i++)
    {
      codePoints1 = ucd.Where(item => item.Value.Name.Contains("LATIN")).Select(item => item.Value.CodePoint).ToList();
    }
    var t1 = DateTime.Now;
    var s1 = $"Time of 100 x where \"LATIN\" using LINQ query:";
    Console.WriteLine($"{s1,-70} {t1 - t0}, found {codePoints1.Count()} code points");

    List<int> codePoints2 = new();
    for (int i = 0; i < 100; i++)
    {
      codePoints2 = nameIndex["LATIN"];
    }
    var t2 = DateTime.Now;
    var s2 = $"Time of 100 x where \"LATIN\" using name index:";
    Console.WriteLine($"{s2,-70} {t2 - t1}, found {codePoints2.Count()} code points");

    List<int> codePoints3 = new();
    for (int i = 0; i < 100; i++)
    {
      codePoints3 = new SortedSet<int>(ucd.Where(item => item.Value.GetAllNames().Any(name => name.IsLike("LATIN*LETTER*")))
        .Select(item => item.Value.CodePoint)).ToList();
    }
    var t3 = DateTime.Now;
    var s3 = $"Time of 100 find like \"LATIN*LETTER*\" using LINQ query:";
    Console.WriteLine($"{s3,-70} {t3 - t2}, found {codePoints3.Count()} code points");

    List<int> codePoints4 = new ();
    for (int i = 0; i < 100; i++)
    {
      codePoints4 = nameIndex.Search("LATIN*LETTER*").ToList();
    }
    var t4 = DateTime.Now;
    var s4 = $"Time of 100 find like \"LATIN*LETTER*\" using name index:";
    Console.WriteLine($"{s4,-70} {t4 - t3}, found {codePoints4.Count()} code points");

    //for (int i = 0; i < codePoints3.Count() || i < codePoints4.Count(); i++)
    //{
    //  int? point3 = i < codePoints3.Count() ? codePoints3[i] : null;
    //  int? point4 = i < codePoints4.Count() ? codePoints4[i] : null;
    //  var ok = (point3 == point4);
    //  if (!ok)
    //    Debug.WriteLine($"{point3:X4} <-> {point4:X4}");
    //}

    List<int>[] codePoints5 = new List<int>[(int)DecompositionType.Compat+1];
    for (int i = 0; i < 1; i++)
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

    List<int>[] codePoints6 = new List<int>[(int)DecompositionType.Compat + 1];
    for (int i = 0; i < 1; i++)
    {
      for (DecompositionType type = DecompositionType.Unknown; type <= DecompositionType.Compat; type++)
      {
        codePoints6[(int)type] = ucd.DecompositionIndex[type].ToList();
      }
    }
    var t6 = DateTime.Now;
    var s6 = $"Time of 1000 find all decomposition types using decomposition index:";
    Console.WriteLine($"{s6,-70} {t6 - t5}, found {codePoints6.Sum(item => item.Count())} code points");

    Console.WriteLine("done");

  }

  private static async Task DownloadFileNameAsync(string fileName)
  {
    var filePath = Path.Combine(Environment.CurrentDirectory, fileName);
    var url = "https://www.unicode.org/Public/UNIDATA/" + fileName;
    using (HttpClient client = new HttpClient())
    {
      if (!File.Exists(filePath))
      {
        string content = client.GetStringAsync(url).Result;
        await System.IO.File.WriteAllTextAsync(filePath, content);
      }
    }
  }
}
