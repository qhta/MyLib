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
    int aliasCount = ucd.LoadAliases(aliasesFileName);
    Console.WriteLine($"{aliasCount} name aliases loaded");
    NameWordIndex nwi = ucd.NameWordIndex;
    foreach (KeyValuePair<string, List<int>> entry in nwi.Take(100))
    {
      Console.WriteLine($"{entry.Key}: {entry.Value.Count}");
    }
    Console.WriteLine($"{nwi.Count} string items created");

    var t0 = DateTime.Now;
    List<int> codePoints1 = new();
    for (int i = 0; i < 100; i++)
    {
      codePoints1 = ucd.Where(item => item.Value.Name.Contains("LATIN")).Select(item => item.Value.CodePoint).ToList();
    }
    var t1 = DateTime.Now;
    Console.WriteLine($"Time of where:  {t1 - t0}, found{codePoints1.Count()}");

    List<int> codePoints2 = new();
    for (int i = 0; i < 100; i++)
    {
      codePoints2 = nwi["LATIN"];
    }
    var t2 = DateTime.Now;
    Console.WriteLine($"Time of index:  {t2 - t1}, found{codePoints2.Count()}");

    List<int> codePoints3 = new();
    for (int i = 0; i < 100; i++)
    {
      codePoints3 = new SortedSet<int>(ucd.Where(item => item.Value.GetAllNames().Any(name => name.IsLike("LATIN*LETTER*")))
        .Select(item => item.Value.CodePoint)).ToList();
    }
    var t3 = DateTime.Now;
    Console.WriteLine($"Time of like:   {t3 - t2}, found{codePoints3.Count()}");

    List<int> codePoints4 = new ();
    for (int i = 0; i < 100; i++)
    {
      codePoints4 = nwi.Search("LATIN*LETTER*").ToList();
    }
    var t4 = DateTime.Now;
    Console.WriteLine($"Time of search: {t4 - t3}, found{codePoints4.Count()}");


    for (int i = 0; i < codePoints3.Count() || i < codePoints4.Count(); i++)
    {
      int? point3 = i < codePoints3.Count() ? codePoints3[i] : null;
      int? point4 = i < codePoints4.Count() ? codePoints4[i] : null;
      var ok = (point3 == point4);
      if (!ok)
        Debug.WriteLine($"{point3:X4} <-> {point4:X4}");
    }
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
