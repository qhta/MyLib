using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Qhta.ObservableObjects;

namespace TestObservableObjectWpfApp;

public class TestObservableList : Test
{
  public TestObservableList()
  {
    Caption = "Test ObservableList";
  }

  public override async void Run()
  {
    Debug.WriteLine($"Start {Caption}");
    var testList = new ObservableList<int>();
    //BindingOperations.EnableCollectionSynchronization(testList, testList.LockObject);
    DataContext = testList;
    testList.PropertyChanged += TestList_PropertyChanged;
    int[] testArray = new int[30];
    int k = 0;
    await Task.Run(() =>
    {
      Status = "Test Add 30 items";
      k = 0;
      for (int i = 0; i < 30; i++)
      {
        testList.Add(++k);
        Thread.Sleep(100);
      }
      Thread.Sleep(2000);

      Status = "Test Remove 30 items from begin";
      k = 0;
      for (int i = 0; i < 30; i++)
      {
        testList.Remove(++k);
        Thread.Sleep(100);
      }
      Thread.Sleep(2000);

      Status = "Test Add 30 items";
      k = 0;
      for (int i = 0; i < 30; i++)
      {
        testList.Add(++k);
        Thread.Sleep(100);
      }
      Thread.Sleep(2000);

      Status = "Test Remove 30 items from end";
      for (int i = 0; i < 30; i++)
      {
        testList.Remove(k--);
        Thread.Sleep(100);
      }
      Thread.Sleep(2000);

      Status = "Test AddRange 100000 items";
      k = 0;
      var intList = new List<int>();
      for (int i = 0; i < 100000; i++)
      {
        intList.Add(++k);
      }
      testList.AddRange(intList);
      Thread.Sleep(2000);

      Status = "Test Clear";
      testList.Clear();
      Thread.Sleep(1000);

      Status = "Test Add 1000 items full speed";
      k = 0;
      for (int i = 0; i < 1000; i++)
      {
        testList.Add(++k);
      }
      Thread.Sleep(2000);

      Status = "Test Clear";
      testList.Clear();
      Thread.Sleep(1000);

      Status = "Test Insert 30 items at index 0";
      k = 0;
      for (int i = 0; i < 30; i++)
      {
        testList.Insert(0, ++k);
        Thread.Sleep(100);
      }
      Thread.Sleep(2000);

      Status = "Test Remove 30 items at index 0";
      k = 0;
      for (int i = 0; i < 30; i++)
      {
        testList.RemoveAt(0);
        Thread.Sleep(100);
      }
      Thread.Sleep(2000);

      Status = "Test Insert 1000 items at index 0 full speed";
      k = 0;
      for (int i = 0; i < 1000; i++)
      {
        testList.Insert(0, ++k);
      }
      Thread.Sleep(2000);

      Status = "Test Sort";
      testList.Sort();
      Thread.Sleep(2000);

      Status = "Test Reverse";
      testList.Reverse();
      Thread.Sleep(2000);

      Status = "Test Clear";
      testList.Clear();
      Thread.Sleep(1000);

      Status = "Test Passed";
      Debug.WriteLine($"End {Caption}");
    });
    Debug.WriteLine($"Exit {Caption}");

  }

  private void TestList_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs args)
  {
    var status = Status;
    var k = status.IndexOf('.');
    if (k >=0)
      status = status.Substring(0, k);
    var propInfo = sender!.GetType().GetProperty(args.PropertyName!);
    var value = propInfo?.GetValue(sender, null);
    Status = status + $". {args.PropertyName}={value}";
  }
}
