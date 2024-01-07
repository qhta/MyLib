using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Qhta.ObservableObjects;

namespace TestObservableObjectWpfApp;

public class TestObservableDictionary : Test
{
  public TestObservableDictionary()
  {
    Caption = "Test ObservableDictionary";
  }

  public override async void Run()
  {
    Debug.WriteLine($"Start {Caption}");
    var testList = new ObservableDictionary<int, string>();
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
        testList.Add(++k, k.ToString("X8"));
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
        testList.Add(++k,k.ToString("X8"));
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
      var intStringList = new List<(int, string)>();
      for (int i = 0; i < 100000; i++)
      {
        intStringList.Add((++k,k.ToString("X8")));
      }
      testList.AddRange(intStringList);
      Thread.Sleep(2000);

      Status = "Test Clear";
      testList.Clear();
      Thread.Sleep(1000);

      Status = "Test Add 1000 items full speed";
      k = 0;
      for (int i = 0; i < 1000; i++)
      {
        testList.Add(++k, k.ToString("X8"));
      }
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
