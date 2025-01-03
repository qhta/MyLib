﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Qhta.TypeUtils;

/// <summary>
///   Reference Article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
///   Provides a method for clone object with a deep copy of properties.
///   Binary Serialization is used to perform the copy.
/// </summary>
public static class ObjectCopier
{
  /// <summary>
  ///   Perform a deep copy of the object properties.
  /// </summary>
  /// <typeparam name="T">The type of object being copied.</typeparam>
  /// <param name="source">The object instance to copy.</param>
  /// <returns>The copied object.</returns>
  [DebuggerStepThrough]
  public static T? Clone<T>(this T source)
  {
    // Don't serialize a null object, simply return the default for that object
    if (ReferenceEquals(source, null)) return default;

//    if (!typeof(T).IsSerializable) throw new ArgumentException($"The type {typeof(T).Name} must be serializable.", "source");
#pragma warning disable SYSLIB0011
    IFormatter formatter = new BinaryFormatter();
    Stream stream = new MemoryStream();
    using (stream)
    {
      formatter.Serialize(stream, source);
      stream.Seek(0, SeekOrigin.Begin);
      return (T)formatter.Deserialize(stream);
    }
  }
}