using System.Reflection;
using System.Runtime.CompilerServices;

namespace Qhta.DiagnosticTools;

/// <summary>
///  This class contains counters for measuring the time taken by various operations in the Qhta.DiagnosticTools library.
/// </summary>
public static class DiagnosticTools
{
  /// <summary>
  /// Replaces the standard GetCustomAttributeDiag method with a version that increments a counter for the number of times it is called for a specific attribute type.
  /// </summary>
  /// <typeparam name="T">The type of the attribute to retrieve.</typeparam>
  /// <param name="element">The member from which to retrieve the attribute.</param>
  /// <param name="inherit">Whether to search this member's inheritance chain to find the attributes.</param>
  /// <param name="member">The name of the calling member.</param>
  /// <returns>The attribute of type T if found; otherwise, null.</returns>
  public static T? GetCustomAttributeDiag<T>(this MemberInfo element, bool inherit = true, [CallerMemberName] string member = "") where T : Attribute
  {
    CountGetCustomAttribute(typeof(T), member);
    // ReSharper disable once InvokeAsExtensionMember
    return System.Reflection.CustomAttributeExtensions.GetCustomAttribute<T>(element, inherit);
  }

  /// <summary>
  /// Increments the counter for the number of times GetCustomAttributeDiag is called for a specific attribute type.
  /// </summary>
  /// <param name="attributeType">The type of the attribute for which to increment the counter.</param>
  /// <param name="member">The name of the calling member.</param>
  public static void CountGetCustomAttribute(Type attributeType, [CallerMemberName] string member = "")
  {
    if (GetCustomAttributeCounts.ContainsKey(attributeType))
    {
      if (GetCustomAttributeCounts[attributeType].Callers.ContainsKey(member))
      {
        GetCustomAttributeCounts[attributeType].Callers[member]++;
      }
      else
      { 
        GetCustomAttributeCounts[attributeType].Callers[member] = 1;
      }
    }
    else
    {
      // ReSharper disable once UseObjectOrCollectionInitializer
      GetCustomAttributeCounts[attributeType] = new DiagnosticCounter();
      GetCustomAttributeCounts[attributeType].Callers[member] = 1;
    }
  }

  
  /// <summary>
  /// Counter for the number of times GetCustomAttributeDiag is called for a specific attribute type.
  /// </summary>
  public static readonly Dictionary<Type, DiagnosticCounter> GetCustomAttributeCounts = new Dictionary<Type, DiagnosticCounter>();
}