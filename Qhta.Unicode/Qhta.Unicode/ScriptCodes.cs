using Qhta.Collections;

namespace Qhta.Unicode;

/// <summary>
/// Unicode script codes loaded from Scripts.txt
/// </summary>
public static class ScriptCodes
{
  /// <summary>
  /// Unicode script codes loaded from Scripts.txt
  /// </summary>
  public static BiDiDictionary<string, string> UcdScriptNames => _UcdScriptNames ??= LoadUcdScriptNames("ScriptCodes.txt");

  private static BiDiDictionary<string, string>? _UcdScriptNames;

  private static BiDiDictionary<string, string> LoadUcdScriptNames(string fileName)
  {
    //var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    //var fileName1 = Path.Combine(baseDirectory, fileName);
    //while (!baseDirectory!.EndsWith("\\bin")) baseDirectory = Path.GetDirectoryName(baseDirectory);
    //var fileName2 = Path.Combine(baseDirectory, fileName);
    //if (!File.Exists(fileName2) && File.Exists(fileName1))
    //  File.Copy(fileName1, fileName2);
    //if (File.Exists(fileName2) && File.Exists(fileName1))
    //  if (File.GetLastWriteTime(fileName1) > File.GetLastWriteTime(fileName2))
    //    File.Copy(fileName1, fileName2, true);
    //fileName = fileName2;
    var result = new BiDiDictionary<string, string>();
    using (var reader = new StreamReader(fileName))
    {
      while (reader.ReadLine() is { } line)
      {
        if (line.StartsWith("#"))
          continue;
        var parts = line.Split(';');
        if (parts.Length < 2)
          continue;
        var scriptCode = parts[0].Trim();
        var scriptName = parts[1].Trim();
        result.Add(scriptCode, scriptName);
      }
    }
    return result;
  }
}