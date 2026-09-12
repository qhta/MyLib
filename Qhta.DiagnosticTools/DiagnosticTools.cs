using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Qhta.DiagnosticTools;

/// <summary>
///  This class contains counters for measuring the time taken by various operations in the Qhta.DiagnosticTools library.
/// </summary>
public static class DiagnosticTools
{
  /// <summary>
  /// Invokes a method using reflection, with the option to not wrap exceptions in a TargetInvocationException.
  /// </summary>
  /// <param name="methodInfo">The method information to invoke.</param>
  /// <param name="target">The object on which to invoke the method.</param>
  /// <param name="parameters">The parameters to pass to the method.</param>
  /// <returns>The result of the method invocation.</returns>
  public static object? InvokeDiag(this MethodInfo methodInfo, object? target, object?[]? parameters = null)
  {
    #if NET8_0_OR_GREATER
    return methodInfo.Invoke(target, BindingFlags.DoNotWrapExceptions, null, parameters, null);
#else
    return methodInfo.Invoke(target, parameters);
    #endif
  }

  /// <summary>
  /// Invokes a constructor using reflection, with the option to not wrap exceptions in a TargetInvocationException.
  /// </summary>
  /// <param name="constructorInfo">The constructor information to invoke.</param>
  /// <param name="parameters">The parameters to pass to the constructor.</param>
  /// <returns>The result of the constructor invocation.</returns>
  public static object InvokeDiag(this ConstructorInfo constructorInfo, object?[]? parameters = null)
  {
    #if NET8_0_OR_GREATER
    return constructorInfo.Invoke(BindingFlags.DoNotWrapExceptions, null, parameters, null);
    #else
    return constructorInfo.Invoke(parameters);
    #endif
  }

  /// <summary>
  /// Sets the value of a property using reflection, with the option to not wrap exceptions in a TargetInvocationException.
  /// </summary>
  /// <param name="propertyInfo">The property information to set.</param>
  /// <param name="target">The object on which to set the property value.</param>
  /// <param name="value">The value to set.</param>
  public static void SetValueDiag(this PropertyInfo propertyInfo, object? target, object? value, [CallerMemberName] string callerName = "")
  {
    //Debug.WriteLine($"{propertyInfo}.SetValue({target}, {value}) in {callerName}");

#if NET8_0_OR_GREATER
    propertyInfo.SetValue(target, value, BindingFlags.DoNotWrapExceptions, null, null, CultureInfo.InvariantCulture);
#else
    propertyInfo.SetValue(target, value);
    #endif
  }

  /// <summary>
  /// Gets the value of a property using reflection, with the option to not wrap exceptions in a TargetInvocationException.
  /// </summary>
  /// <param name="propertyInfo">The property information to get.</param>
  /// <param name="target">The object from which to get the property value.</param>
  /// <param name="callerName">The name of the method or property that called this method.</param>
  /// <returns>The value of the property.</returns>
  public static object? GetValueDiag(this PropertyInfo propertyInfo, object? target, [CallerMemberName] string callerName = "")
  {
    #if NET8_0_OR_GREATER
    return propertyInfo.GetValue(target, BindingFlags.DoNotWrapExceptions, null, null, CultureInfo.InvariantCulture);
    #else
    return propertyInfo.GetValue(target);
    #endif
  }
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
      if (GetCustomAttributeCounts[attributeType].SubCounters.ContainsKey(member))
      {
        GetCustomAttributeCounts[attributeType].SubCounters[member]++;
      }
      else
      { 
        GetCustomAttributeCounts[attributeType].SubCounters[member] = 1;
      }
    }
    else
    {
      // ReSharper disable once UseObjectOrCollectionInitializer
      GetCustomAttributeCounts[attributeType] = new DiagnosticCounter();
      GetCustomAttributeCounts[attributeType].SubCounters[member] = 1;
    }
  }

  
  /// <summary>
  /// Counter for the number of times GetCustomAttributeDiag is called for a specific attribute type.
  /// </summary>
  public static readonly Dictionary<Type, DiagnosticCounter> GetCustomAttributeCounts = new Dictionary<Type, DiagnosticCounter>();

  /// <summary>
  /// Increments the counter for the number of times a method is called directly, with an optional message to differentiate between different call contexts.
  /// </summary>
  /// <param name="callerType">The type of the calling class.</param>
  /// <param name="methodName">The name of the calling method.</param>
  /// <param name="message">An optional message to differentiate between different call contexts.</param>
  /// <param name="time">The time taken by the method call.</param>
  public static void CountDirectCall(Type callerType, string methodName = "", string message ="", double time = 0)
  {
    string key = $"{callerType.Name}.{methodName}";
    if (DirectCallCounters.ContainsKey(key))
    {
      if (DirectCallCounters[key].SubCounters.ContainsKey(message))
      {
        var countTime = DirectCallCounters[key].SubCounters[message];
        DirectCallCounters[key].SubCounters[message] = (countTime.Count + 1, countTime.Time + time);
      }
      else
      {
        DirectCallCounters[key].SubCounters[message] = (1, time);
      }
    }
    else
    {
      // ReSharper disable once UseObjectOrCollectionInitializer
      DirectCallCounters[key] = new DiagnosticCounterWithTimer();
      DirectCallCounters[key].SubCounters[message] = (1, time);
    }
  }

  public static readonly Dictionary<string, DiagnosticCounterWithTimer> DirectCallCounters 
    = new Dictionary<string, DiagnosticCounterWithTimer>();
}