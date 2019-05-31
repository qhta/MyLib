using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Qhta.DbUtils
{
  /// <summary>
  /// Klasa oferująca metody pomocnicze dla baz danych
  /// </summary>
  public static class DbUtilities
  {
    #region wyliczenie informacji
    /// <summary>
    /// Wyliczenie informacji o silnikach baz danych z dostępnych bibliotek
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<DbEngineClass> EnumerateEngineClasses()
    {
      Assembly assembly = Assembly.GetCallingAssembly();
      AssemblyName[] refAssemblies = assembly.GetReferencedAssemblies();
      var result = EnumerateEngineClasses(refAssemblies);
      if (result.Count()==0)
        throw new InvalidOperationException($"No engines found. To see engines add a reference to an engine assembly to the calling assembly and add any instruction that invokes the code from the engine assembly");
      return result;
    }

    /// <summary>
    /// Wyliczenie informacji o silnikach baz danych z podanych bibliotek
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<DbEngineClass> EnumerateEngineClasses(IEnumerable<AssemblyName> refAssemblies)
    {
      List<DbEngineClass> result = new List<DbEngineClass>();
      foreach (AssemblyName aName in refAssemblies)
      {
        try
        {
          var refAssembly = Assembly.Load(aName);
          result.AddRange(EnumerateEngineClasses(refAssembly));
        }
        catch(Exception ex)
        {
          Debug.WriteLine(ex.Message);
        }
      }
      return result;
    }

    /// <summary>
    /// Wyliczenie informacji o silnikach baz danych z bibliotek o podanych ścieżkach
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<DbEngineClass> EnumerateEngineClasses(IEnumerable<string> files)
    {
      List<DbEngineClass> result = new List<DbEngineClass>();
      foreach (var aName in files)
      {
        var refAssembly = Assembly.LoadFile(aName);
        result.AddRange(EnumerateEngineClasses(refAssembly));
      }
      return result;
    }

    /// <summary>
    /// Wyliczenie informacji o silnikach baz danych z podanej biblioteki
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<DbEngineClass> EnumerateEngineClasses(Assembly refAssembly)
    {
      Debug.WriteLine($"Search engine classes in {refAssembly.FullName}");
      List<DbEngineClass> result = new List<DbEngineClass>();

      foreach (Type aType in refAssembly.ExportedTypes)
      {
        if (aType.GetInterface(nameof(IDbEngine))!=null && !aType.IsAbstract)
        {
          result.Add(CreateEngineClass(aType));
        }
      }
      return result;
    }

    public static DbEngineClass CreateEngineClass(Type aType)
    {
      string id = aType.GetCustomAttribute<EngineIDAttribute>()?.Identifier
        ?? aType.Name.Replace("Engine", "").ToUpper();
      string name = aType.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? aType.Name;
      string description = aType.GetCustomAttribute<DescriptionAttribute>()?.Description;
      return new DbEngineClass
      {
        ID = id,
        Name = name,
        Description = description,
        AssemblyQualifiedName = aType.AssemblyQualifiedName,
        Type = aType,
      };
    }
    private static Dictionary<Type, DbEngine> engines = new Dictionary<Type, DbEngine>();
    /// <summary>
    /// Podanie silnika danych
    /// </summary>
    /// <returns></returns>
    public static DbEngine GetEngine(DbEngineClass aClass)
    {
      DbEngine result;
      if (!engines.TryGetValue(aClass.Type, out result))
      {
        result = (DbEngine)aClass.Type.GetConstructor(new Type[0]).Invoke(new object[0]);
        engines.Add(aClass.Type, result);
      }
      return result;
    }

    /// <summary>
    /// Konwersja łańcucha na boolean
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool StringToBool(this string str)
    {
      switch (str.ToLower())
      {
        case "true":
        case "yes":
          return true;
        case "false":
        case "no":
          return false;
      }
      throw new InvalidCastException($"Cannot convert \"{str}\" to boolean value");
    }

    /// <summary>
    /// Sprawdzenie, czy źródło danych reprezentuje serwer lokalny
    /// </summary>
    /// <param name="dataSource"></param>
    /// <returns></returns>
    public static bool IsLocalDataSource(this string dataSource)
    {
      return dataSource.StartsWith("(local)") || dataSource.StartsWith(".") || dataSource.StartsWith(Environment.MachineName);
    }
    #endregion

  }

}



