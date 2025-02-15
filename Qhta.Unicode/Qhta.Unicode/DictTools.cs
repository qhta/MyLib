using Qhta.Collections;

namespace Qhta.Unicode;

/// <summary>
/// Tools for working with dictionaries and lists
/// </summary>
public static class DictTools
{

  /// <summary>
  /// Load a CodePoint to String BiDiDictionary from a file
  /// </summary>
  /// <param name="dictionary"></param>
  /// <param name="fileName"></param>
  public static void LoadFromFile(this BiDiDictionary<CodePoint, string> dictionary, string fileName)
  {
    using (var reader = File.OpenText(fileName))
    {
      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        if (line is not null && !line.StartsWith("#"))
        {
          var parts = line.Split([';', ',']);
          if (parts.Length == 2)
          {
            dictionary.Add(parts[0], parts[1]);
          }
        }
      }
    }
  }

  /// <summary>
  /// Load a String Dictionary from a file
  /// </summary>
  /// <param name="dictionary"></param>
  /// <param name="fileName"></param>
  /// <param name="joinKeyWords">true to join words of key with underscores</param>
  public static void LoadFromFile(this IDictionary<string, string> dictionary, string fileName, bool joinKeyWords = false)
  {
    using (var reader = File.OpenText(fileName))
    {
      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        if (line is not null && !line.StartsWith("#"))
        {
          var parts = line.Split([';', ',']);
          if (parts.Length == 2)
          {
            var key = parts[0];
            if (joinKeyWords)
              key = key.Replace(' ', '_');
            dictionary.Add(key, parts[1]);
          }
        }
      }
    }
  }

  /// <summary>
  /// Load a String BiDiDictionary from a file
  /// </summary>
  /// <param name="dictionary"></param>
  /// <param name="fileName"></param>
  /// <param name="joinKeyWords">true to join words of key with underscores</param>
  public static void LoadFromFile(this BiDiDictionary<string, string> dictionary, string fileName, bool joinKeyWords = false)
  {
    using (var reader = File.OpenText(fileName))
    {
      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        if (line is not null && !line.StartsWith("#"))
        {
          var parts = line.Split([';', ',']);
          if (parts.Length == 2)
          {
            var key = parts[0];
            if (joinKeyWords)
              key = key.Replace(' ', '_');
            dictionary.Add(key, parts[1]);
          }
        }
      }
    }
  }

  /// <summary>
  /// Load a String to Int Dictionary from a file
  /// </summary>
  /// <param name="dictionary"></param>
  /// <param name="fileName"></param>
  public static void LoadFromFile(this IDictionary<string, int> dictionary, string fileName)
  {
    using (var reader = File.OpenText(fileName))
    {
      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        if (line is not null && !line.StartsWith("#"))
        {
          var parts = line.Split([';', ',']);
          if (parts.Length == 2)
          {
            dictionary.Add(parts[0], int.Parse(parts[1]));
          }
        }
      }
    }
  }

  /// <summary>
  /// Load an Int to String BiDiDictionary from a file
  /// </summary>
  /// <param name="dictionary"></param>
  /// <param name="fileName"></param>
  public static void LoadFromFile(this BiDiDictionary<int, string> dictionary, string fileName)
  {
    using (var reader = File.OpenText(fileName))
    {
      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        if (line is not null && !line.StartsWith("#"))
        {
          var parts = line.Split([';', ',']);
          if (parts.Length == 2)
          {
            dictionary.Add(int.Parse(parts[0]), parts[1]);
          }
        }
      }
    }
  }

  /// <summary>
  /// Load a String List from a file
  /// </summary>
  /// <param name="list"></param>
  /// <param name="fileName"></param>
  public static void LoadFromFile(this List<string> list, string fileName)
  {
    using (var reader = File.OpenText(fileName))
    {
      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        if (line is not null && !line.StartsWith("#"))
        {
          list.Add(line.Trim());
        }
      }
    }
  }

}