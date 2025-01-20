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
    NameStringIndex nsi = ucd.NameStringIndex;
    foreach (KeyValuePair<string, List<int>> entry in nsi.Take(100))
    {
      Console.WriteLine($"{entry.Key}: {entry.Value.Count}");
    }
    Console.WriteLine($"{nsi.Count} string items created");
    var t1 = DateTime.Now;
    int count1 = 0;
    for (int i = 0; i < 100; i++)
    {
      var codePoints = ucd.Where(item => item.Value.Name.Contains("LATIN")).Select(item => item.Value.CodePoint);
      count1 = codePoints.Count();
    }
    var t2 = DateTime.Now;
    int count2 = 0; Console.WriteLine($"Time of where:  {t2 - t1}, found{count1}");
    for (int i = 0; i < 100; i++)
    {
      var codePoints = nsi["LATIN"];
      count2 = codePoints.Count();
    }
    var t3 = DateTime.Now;
    int count3 = 0;
    Console.WriteLine($"Time of index:  {t3 - t2}, found{count2}");
    List<int> points3 = new();
    for (int i = 0; i < 100; i++)
    {
      var codePoints = new SortedSet<int>(ucd.Where(item => item.Value.Name.IsLike("LATIN*LETTER*"))
        .Select(item => item.Value.CodePoint));
      var aliases = ucd.Where(item => item.Value.Aliases != null).SelectMany(item => item.Value.Aliases!).Where(alias => alias.Alias.IsLike("LATIN*LETTER*")).Select(item => item.CodePoint).ToList();
      foreach (var alias in aliases)
      {
        codePoints.Add(alias);
      }
      count3 = codePoints.Count();
      points3 = codePoints.ToList();
    }
    var t4 = DateTime.Now;
    int count4 = 0;
    Console.WriteLine($"Time of like:   {t4 - t3}, found{count3}");
    List<int> points4 = new ();
    for (int i = 0; i < 100; i++)
    {
      var codePoints = nsi.Search("LATIN*LETTER*").ToList();
      count4 = codePoints.Count();
      points4 = codePoints;
    }
    var t5 = DateTime.Now;
    Console.WriteLine($"Time of search: {t5 - t4}, found{count4}");
    for (int i = 0; i < points3.Count() || i < points4.Count(); i++)
    {
      int? point3 = i < points3.Count() ? points3[i] : null;
      int? point4 = i < points4.Count() ? points4[i] : null;
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
