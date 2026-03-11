using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Provides static utility methods for working with <c>OnOffType</c> elements, enabling convenient retrieval of boolean
/// values with support for default options.
/// </summary>
/// <remarks>This class is intended to simplify the handling of <c>OnOffType</c> elements, particularly in
/// scenarios where the element may be null or its value is not explicitly set. The methods in this class help ensure
/// consistent and predictable behavior when interpreting optional or missing values in Open XML documents.</remarks>
public static class OnOffTypeTools
{
  /// <summary>
  /// Gets the boolean value of an <c>OnOffType</c> element, or a default value if the element is null or does not have a value.
  /// </summary>
  /// <param name="onOff">The element to get the value from.</param>
  /// <param name="defaultValue">The default value to return if the element is null or does not have a value.</param>
  /// <returns>The boolean value of the element, or the default value.</returns>
  [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
  public static bool GetValueOrDefault(this DXW.OnOffType onOff, bool defaultValue = false)
  {
    return onOff.Val?.Value ?? defaultValue;
  }

  /// <summary>
  /// Gets the boolean value of an <c>OnOffType</c> element, or a default value if the element is null or does not have a value.
  /// </summary>
  /// <param name="onOff">The element to get the value from.</param>
  /// <param name="defaultValue">The default value to return if the element is null or does not have a value.</param>
  /// <returns>The boolean value of the element, or the default value.</returns>
  [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
  public static bool GetValueOrDefault(this DXM.OnOffType onOff, bool defaultValue = false)
  {
    if (onOff.Val?.Value != null)
    {
      if (onOff.Val.Value == DXM.BooleanValues.On || onOff.Val.Value == DXM.BooleanValues.True ||
          onOff.Val.Value == DXM.BooleanValues.One)
        return true;
      if (onOff.Val.Value == DXM.BooleanValues.Off || onOff.Val.Value == DXM.BooleanValues.False ||
          onOff.Val.Value == DXM.BooleanValues.Zero)
        return false;
    }
    return defaultValue;
  }


  /// <summary>
  /// Gets the boolean value of an <c>OnOffType</c> element, or a default value if the element is null or does not have a value.
  /// </summary>
  /// <param name="onOff">The element to get the value from.</param>
  /// <param name="defaultValue">The default value to return if the element is null or does not have a value.</param>
  /// <returns>The boolean value of the element, or the default value.</returns>
  [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
  public static bool GetValueOrDefault(this DXO10W.OnOffType onOff, bool defaultValue = false)
  {
    if (onOff.Val?.Value != null)
    {
      if (onOff.Val.Value == DXO10W.OnOffValues.One || onOff.Val.Value == DXO10W.OnOffValues.True)
        return true;
      if (onOff.Val.Value == DXO10W.OnOffValues.Zero || onOff.Val.Value == DXO10W.OnOffValues.False)
        return false;
    }
    return defaultValue;
  }

  /// <summary>
  /// Gets the boolean value of an <c>OnOffType</c> element, or a default value if the element is null or does not have a value.
  /// </summary>
  /// <param name="onOff">The element to get the value from.</param>
  /// <param name="defaultValue">The default value to return if the element is null or does not have a value.</param>
  /// <returns>The boolean value of the element, or the default value.</returns>
  [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

  public static bool GetValueOrDefault(this DXO13W.OnOffType onOff, bool defaultValue = false)
  {
    return onOff.Val?.Value ?? defaultValue;
  }


  /// <summary>
  /// Gets the boolean value of an <c>OnOffType</c> element, or a default value if the element is null or does not have a value.
  /// </summary>
  /// <param name="onOff">The element to get the value from.</param>
  /// <param name="defaultValue">The default value to return if the element is null or does not have a value.</param>
  /// <returns>The boolean value of the element, or the default value.</returns>
  [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
  public static bool GetValueOrDefault(this DXW.OnOffOnlyType onOff, bool defaultValue = false)
  {
    if (onOff.Val?.Value != null)
    {
      if (onOff.Val.Value == DXW.OnOffOnlyValues.On)
        return true;
      if (onOff.Val.Value == DXW.OnOffOnlyValues.Off)
        return false;
    }
    return defaultValue;
  }
}