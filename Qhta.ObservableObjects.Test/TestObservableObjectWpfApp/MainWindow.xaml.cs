using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Qhta.ObservableObjects;
using Qhta.WPF.Behaviors;

namespace TestObservableObjectWpfApp;

public partial class MainWindow : Window
{
  public MainWindow()
  {
    InitializeComponent();
    ObservableObject.CommonDispatcher = new DispatcherBridge(Dispatcher);
    var tests = new Tests();
    DataContext = tests;
    tests.ObservableListTest = new TestObservableList();
    tests.ObservableDictionaryTest = new TestObservableDictionary();
  }

  public void StartTestObservableDictionary()
  {
    var testDictionary = new ObservableDictionary<int, int>();
    ObservableDictionaryView.DataContext = testDictionary;
    //BindingOperations.EnableCollectionSynchronization(testDictionary, testDictionary.LockObject);
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
      for (int i = 0; i < 30; i++)
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

  private void StartObservableListTest_Click(object sender, RoutedEventArgs e)
  {
    (DataContext as Tests)?.ObservableListTest?.Run();
  }

  private void StartObservableDictionaryTest_Click(object sender, RoutedEventArgs e)
  {
    (DataContext as Tests)?.ObservableDictionaryTest?.Run();
  }
}


