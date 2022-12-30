using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qhta.TypeUtils;

/// <summary>
///  Attribute to set at non-comparable property.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class NonComparableAttribute : Attribute
{

}

/// <summary>
/// The result of object comparison
/// </summary>
public class CompareResult
{
  /// <summary>
  /// Indicates if objects are equal
  /// </summary>
  public bool AreEqual { get; set; }
  /// <summary>
  /// If objects are not equal then this value holds the property path to first different properties
  /// </summary>
  public string? DiffPath { get; set; }
}

/// <summary>
/// Utility class to object deep comparison
/// </summary>
public static class ObjectComparer
{
  /// <summary>
  /// Compares two objects in deep. All public read/write properties are compared
  /// except those marked with NonComparable attribute.
  /// If objects are different then diffPath is returned with a property path to first different properties.
  /// If objects are of different type then diffPath is "@Type".
  /// </summary>
  /// <param name="this"></param>
  /// <param name="other"></param>
  /// <returns></returns>
  public static CompareResult AreEqual(object @this, object other)
  {
    return AreEqual(@this, other, string.Empty);
  }


  private static CompareResult AreEqual(object? @this, object? other, string propName)
  {
    if (@this == null && other == null)
      return new CompareResult { AreEqual = true };
    if (@this != null && other == null)
    {
      if (@this is ICollection collection)
        if (collection.Count == 0)
          return new CompareResult { AreEqual = true };
      return new CompareResult { AreEqual = false, DiffPath = propName };
    }
    if (@this == null && other != null)
    {
      if (other is ICollection collection)
        if (collection.Count == 0)
          return new CompareResult { AreEqual = true };
      return new CompareResult { AreEqual = false, DiffPath = propName };
    }
    if (@this!=null && other!=null && @this.GetType() != other.GetType())
    {
      return new CompareResult { AreEqual = false, DiffPath = "@Type" };
    }

    var props = @this?.GetType().GetProperties();
    bool compared = false;
    if (props!=null)
    foreach (var propInfo in props)
    {
      var getMethod = propInfo.GetMethod;
      var setMethod = propInfo.SetMethod;
      if (getMethod != null && setMethod != null)
      {
        if (propInfo.GetCustomAttributes(true).FirstOrDefault(item => item is NonComparableAttribute) == null)
        {
          if (propInfo.GetIndexParameters().Length == 0)
          {
            var value1 = getMethod.Invoke(@this, new object[0]);
            var value2 = getMethod.Invoke(other, new object[0]);
            //Console.WriteLine($"Comparing property {propName}");
            var propCompResult = AreEqual(value1, value2, propInfo.Name);
            compared = true;
            if (!propCompResult.AreEqual)
            {
              var diffPath = propCompResult.DiffPath;
              if (diffPath != null)
                diffPath = propInfo.Name + "." + diffPath;
              else
                diffPath = propInfo.Name;
              return new CompareResult { AreEqual = false, DiffPath = diffPath };
            }
          }
        }
      }
      if (@this is ICollection thisCollection
          && other is ICollection otherCollection)
      {
        var thisCount = thisCollection.Count;
        var otherCount = otherCollection.Count;
        if (thisCount != otherCount)
        {
          return new CompareResult { AreEqual = false, DiffPath = "Count" };
        }
        var thisIterator = thisCollection.GetEnumerator();
        var otherIterator = otherCollection.GetEnumerator();
        thisIterator.Reset();
        otherIterator.Reset();
        int itemNumber = 0;
        while (thisIterator.MoveNext())
        {
          otherIterator.MoveNext();
          var value1 = thisIterator.Current;
          var value2 = otherIterator.Current;
          string index = itemNumber.ToString();
          if (value1 != null && value2 != null)
          {
            var id1 = GetItemId(value1);
            var id2 = GetItemId(value2);
            if (id1 == id2 && id1 != null)
              index = id1;
          }
          var indexerName = $"{propName}[{index}]";
          //Console.WriteLine($"Comparing item {indexerName}");
          var itemCompResult = AreEqual(value1, value2, indexerName);
          compared = true;
          if (!itemCompResult.AreEqual)
          {
            var diffPath = itemCompResult.DiffPath;
            if (diffPath != null)
              diffPath = indexerName + "." + diffPath;
            else
              diffPath = indexerName;
            return new CompareResult { AreEqual = false, DiffPath = diffPath };
          }
          itemNumber++;
        }
      }
    }
    if (!compared)
    {
      var isEqual = @this?.Equals(other);
      if (isEqual!=true)
        return new CompareResult { AreEqual = false };
    }
    return new CompareResult { AreEqual = true };
  }

  private static string? GetItemId(object item)
  {
    var idProp = item.GetType().GetProperty("ID") ?? item.GetType().GetProperty("Id");
    if (idProp != null)
    {
      return idProp.GetValue(item)?.ToString();
    }
    return null;
  }

  /// <summary>
  /// Asynchronous object comparison
  /// </summary>
  /// <param name="this"></param>
  /// <param name="other"></param>
  /// <returns></returns>
  public static CompareResult AreEqualAsync(object @this, object other)
  {
    return AreEqualAsync(@this, other, string.Empty).Result;
  }

  private static Task<CompareResult> AreEqualAsync(object? @this, object? other, string propName)
  {
    return Task.Factory.StartNew<CompareResult>(() =>
    {
      //Console.WriteLine($"Run Thread {Thread.CurrentThread.ManagedThreadId}");
      if (@this == null && other == null)
        return new CompareResult { AreEqual = true };
      if (@this != null && other == null)
      {
        if (@this is ICollection collection)
          if (collection.Count == 0)
            return new CompareResult { AreEqual = true };
        return new CompareResult { AreEqual = false, DiffPath = propName };
      }
      if (@this == null && other != null)
      {
        if (other is ICollection collection)
          if (collection.Count == 0)
            return new CompareResult { AreEqual = true };
        return new CompareResult { AreEqual = false, DiffPath = propName };
      }
      if (@this?.GetType() != other?.GetType())
      {
        return new CompareResult { AreEqual = false, DiffPath = "@Type" };
      }

      var props = @this?.GetType().GetProperties();
      bool compared = false;
      List<Task<CompareResult>> propCompTasksList = new List<Task<CompareResult>>(props.Count());
      if (props!=null)
      foreach (var propInfo in props)
      {
        var getMethod = propInfo.GetMethod;
        var setMethod = propInfo.SetMethod;
        if (getMethod != null && setMethod != null)
        {
          if (propInfo.GetCustomAttributes(true).FirstOrDefault(item => item is NonComparableAttribute) == null)
          {
            if (propInfo.GetIndexParameters().Length == 0)
            {
              var value1 = getMethod.Invoke(@this, new object[0]);
              var value2 = getMethod.Invoke(other, new object[0]);
              //Console.WriteLine($"Comparing property {propName}");
              propCompTasksList.Add(AreEqualAsync(value1, value2, propInfo.Name));
              compared = true;
            }
          }
        }
        var propCompTasks = propCompTasksList.ToArray();
        Task.WaitAll(propCompTasks);
        foreach (var taskResult in propCompTasks)
        {
          var propCompResult = taskResult.Result;
          if (!propCompResult.AreEqual)
          {
            var diffPath = propCompResult.DiffPath;
            if (diffPath != null)
              diffPath = propInfo.Name + "." + diffPath;
            else
              diffPath = propInfo.Name;
            return new CompareResult { AreEqual = false, DiffPath = diffPath };
          }
        }
        if (@this is ICollection thisCollection
            && other is ICollection otherCollection)
        {
          var thisCount = thisCollection.Count;
          var otherCount = otherCollection.Count;
          if (thisCount != otherCount)
          {
            return new CompareResult { AreEqual = false, DiffPath = "Count" };
          }
          var thisIterator = thisCollection.GetEnumerator();
          var otherIterator = otherCollection.GetEnumerator();
          thisIterator.Reset();
          otherIterator.Reset();
          int itemNumber = 0;
          List<Task<CompareResult>> itemCompTasksList = new List<Task<CompareResult>>(props.Count());
          while (thisIterator.MoveNext())
          {
            otherIterator.MoveNext();
            var value1 = thisIterator.Current;
            var value2 = otherIterator.Current;
            string index = itemNumber.ToString();
            if (value1 != null && value2 != null)
            {
              var id1 = GetItemId(value1);
              var id2 = GetItemId(value2);
              if (id1 == id2 && id1 != null)
                index = id1;
            }
            var indexerName = $"{propName}[{index}]";
            //Console.WriteLine($"Comparing item {indexerName}");
            itemCompTasksList.Add(AreEqualAsync(value1, value2, indexerName));
            compared = true;
            itemNumber++;
          }
          var itemCompTasks = itemCompTasksList.ToArray();
          Task.WaitAll(itemCompTasks);
          foreach (var taskResult in itemCompTasks)
          {
            var itemCompResult = taskResult.Result;
            if (!itemCompResult.AreEqual)
            {
              var diffPath = itemCompResult.DiffPath;
              if (diffPath != null)
                diffPath = propInfo.Name + "." + diffPath;
              else
                diffPath = propInfo.Name;
              return new CompareResult { AreEqual = false, DiffPath = diffPath };
            }
          }

        }
      }
      if (!compared)
      {
        var isEqual = @this?.Equals(other);
        if (isEqual!=true)
          return new CompareResult { AreEqual = false };
      }
      return new CompareResult { AreEqual = true };
    });
  }


  //private static async Task<CompareResult> AreEqualAsync(object @this, object other, PropertyInfo propInfo)
  //{
  //  var task = Task.Factory.StartNew<CompareResult>(() =>
  //  {
  //    var getMethod = propInfo.GetMethod;
  //    var setMethod = propInfo.SetMethod;
  //    if (getMethod != null && setMethod != null)
  //    {
  //      if (propInfo.GetCustomAttributes(true).FirstOrDefault(item => item is NonComparableAttribute) == null)
  //      {
  //        if (propInfo.GetIndexParameters().Length == 0)
  //        { }
  //      }
  //    }
  //    return new CompareResult { AreEqual = true };
  //  });
  //  return await task;
  //}

  //private static async Task<CompareResult> CreateComparePropertyTask(object @this, object other, PropertyInfo propInfo)
  //{
  //  var getMethod = propInfo.GetMethod;
  //  var setMethod = propInfo.SetMethod;
  //  if (getMethod != null && setMethod != null)
  //  {
  //    if (propInfo.GetCustomAttributes(true).FirstOrDefault(item => item is NonComparableAttribute) == null)
  //    {
  //      if (propInfo.GetIndexParameters().Length == 0)
  //      {
  //        var value1 = getMethod.Invoke(@this, new object[0]);
  //        var value2 = getMethod.Invoke(other, new object[0]);
  //        var propEqualsTask = EqualsAsync(value1, value2, propInfo.Name);
  //        propEqualsTask.ContinueWith((Task<CompareResult> priorTask) => { });
  //        //{
  //        //  if (!propEqualsTask.Result.IsEqual)
  //        //  {
  //        //    var diffPath = propEqualsTask.Result.DiffPath;
  //        //    if (diffPath != null)
  //        //      diffPath = propInfo.Name + "." + diffPath;
  //        //    else
  //        //      diffPath = propInfo.Name;
  //        //    return new CompareResult { IsEqual = false, DiffPath = diffPath };
  //        //  }
  //        //});
  //        //compared = true;
  //      }
  //    }
  //  }
  //  return null;

  //}

  //private static async Task<CompareResult> EqualsAsync(object @this, object other, string propName)
  //{
  //  if (@this == null && other == null)
  //    return new CompareResult { IsEqual = true };
  //  if (@this != null && other == null)
  //  {
  //    if (@this is ICollection collection)
  //      if (collection.Count == 0)
  //        return new CompareResult { IsEqual = true };
  //    return new CompareResult { IsEqual = false, DiffPath = propName };
  //  }
  //  if (@this == null && other != null)
  //  {
  //    if (other is ICollection collection)
  //      if (collection.Count == 0)
  //        return new CompareResult { IsEqual = true };
  //    return new CompareResult { IsEqual = false, DiffPath = propName };
  //  }
  //  if (@this.GetType() != other.GetType())
  //  {
  //    return new CompareResult { IsEqual = false, DiffPath = "@Type" };
  //  }

  //  var props = @this.GetType().GetProperties();
  //  bool compared = false;
  //  List<Task<CompareResult>> propCompareTasksList = new List<Task<CompareResult>>(props.Count());
  //  foreach (var propInfo in props)
  //  {
  //    var propTask = CreateComparePropertyTask(@this, other, propInfo);
  //    if (propTask != null)
  //    {
  //      propTask.Start();
  //      propCompareTasksList.Add(propTask);
  //      compared = true;
  //    }
  //  }
  //  var propCompareTasks = propCompareTasksList.ToArray();
  //  Task.WaitAll(propCompareTasks);
  //  foreach (var taskResult in propCompareTasks)
  //    if (!taskResult.Result.IsEqual)
  //      return taskResult.Result;
  //  //if (@this is ICollection thisCollection
  //  //&& other is ICollection otherCollection)
  //  //  {
  //  //    var thisCount = thisCollection.Count;
  //  //    var otherCount = otherCollection.Count;
  //  //    if (thisCount != otherCount)
  //  //    {
  //  //      diffPath = "Count";
  //  //      return false;
  //  //    }
  //  //    var thisIterator = thisCollection.GetEnumerator();
  //  //    var otherIterator = otherCollection.GetEnumerator();
  //  //    thisIterator.Reset();
  //  //    otherIterator.Reset();
  //  //    int itemNumber = 0;
  //  //    while (thisIterator.MoveNext())
  //  //    {
  //  //      otherIterator.MoveNext();
  //  //      var value1 = thisIterator.Current;
  //  //      var value2 = otherIterator.Current;
  //  //      string index = itemNumber.ToString();
  //  //      if (value1 != null && value2 != null)
  //  //      {
  //  //        var id1 = GetItemId(value1);
  //  //        var id2 = GetItemId(value2);
  //  //        if (id1 == id2)
  //  //          index = id1;
  //  //      }
  //  //      var indexerName = $"{propName}[{index}]";
  //  //      if (!Equals(value1, value2, indexerName, out diffPath))
  //  //      {
  //  //        if (diffPath != null)
  //  //          diffPath = indexerName + "." + diffPath;
  //  //        else
  //  //          diffPath = indexerName;
  //  //        return false;
  //  //      }
  //  //      itemNumber++;
  //  //    }
  //  //    compared = true;
  //  //  }
  //  //  if (!compared)
  //  //    return @this.Equals(other);
  //  //  return true;
  //  return new CompareResult { IsEqual = true };
  //}

  //private static Task<CompareResult> CreateComparePropertyTask(object @this, object other, PropertyInfo propInfo)
  //{
  //  var getMethod = propInfo.GetMethod;
  //  var setMethod = propInfo.SetMethod;
  //  if (getMethod != null && setMethod != null)
  //  {
  //    if (propInfo.GetCustomAttributes(true).FirstOrDefault(item => item is NonComparableAttribute) == null)
  //    {
  //      if (propInfo.GetIndexParameters().Length == 0)
  //      {
  //        var value1 = getMethod.Invoke(@this, new object[0]);
  //        var value2 = getMethod.Invoke(other, new object[0]);
  //        var propEqualsTask = EqualsAsync(value1, value2, propInfo.Name);
  //        propEqualsTask.ContinueWith((Task<CompareResult> priorTask) => { });
  //        //{
  //        //  if (!propEqualsTask.Result.IsEqual)
  //        //  {
  //        //    var diffPath = propEqualsTask.Result.DiffPath;
  //        //    if (diffPath != null)
  //        //      diffPath = propInfo.Name + "." + diffPath;
  //        //    else
  //        //      diffPath = propInfo.Name;
  //        //    return new CompareResult { IsEqual = false, DiffPath = diffPath };
  //        //  }
  //        //});
  //        //compared = true;
  //      }
  //    }
  //  }
  //  return null;

  //}



}