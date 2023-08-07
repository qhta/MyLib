using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Qhta.ObservableObjects;

namespace TestWPF
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      StartTestObservableList();
    }

    private object testCollection;

    public void StartTestObservableList()
    {
      var testList = new ObservableList<int>();
      testCollection = testList;
      BindingOperations.EnableCollectionSynchronization(testList, testList.LockObject);
      DataContext = testList;
      int[] testArray = new int[30];
      int k=0;
      Task.Run(() =>
      {
        Thread.Sleep(1000);
        k = 0;
        for (int i = 0; i < 30; i++)
        {
          testList.Add(++k);
          Thread.Sleep(100);
        }
        Thread.Sleep(1000);
        k = 0;
        for (int i = 0; i < 30; i++)
        {
          testList.Remove(++k);
          Thread.Sleep(100);
        }
        Thread.Sleep(1000);
        k = 0;
        for (int i = 0; i < 30; i++)
        {
          testList.Add(++k);
          Thread.Sleep(100);
        }
        Thread.Sleep(1000);
        for (int i = 0; i < 30; i++)
        {
          testList.Remove(k--);
          Thread.Sleep(100);
        }
        Thread.Sleep(1000);
        k = 0;
        var intList = new List<int>();
        for (int i = 0; i < 100000; i++)
        {
          intList.Add(++k);
        }
        testList.AddRange(intList);
      });
    }

    public void StartTestObservableDictionary()
    {
      var testDictionary = new ObservableDictionary<int, int>();
      testCollection = testDictionary;
      BindingOperations.EnableCollectionSynchronization(testDictionary, testDictionary.LockObject);
      DataContext = testDictionary;
      int[] testArray = new int[30];
      var random = new System.Random();
      for (int i = 0; i < 30; i++)
        testArray[i] = random.Next();
      Task.Run(() =>
      {
        Thread.Sleep(3000);
        for (int i = 0; i < 30; i++)
        {
          int n = testArray[i];
          testDictionary.Add(n, n);
          //Thread.Sleep(100);
        }
        Thread.Sleep(1000);
        for (int i = 0; i< 30; i++)
        {
          int n = testArray[i];
          testDictionary.Remove(n);
          Thread.Sleep(100);
        }
        Thread.Sleep(500);
        for (int i = 0; i < 10; i++)
        {
          int n = testArray[i];
          testDictionary.Add(n, n);
          Thread.Sleep(100);
        }
        Thread.Sleep(100);
        testDictionary.Clear();
        for (int i = 0; i < 10; i++)
        {
          int n = testArray[i];
          testDictionary.Add(n, n);
          Thread.Sleep(100);
        }
      });
    }
  }
}
