using Qhta.ObservableObjects;

namespace TestObservableObjectWpfApp;

public class Tests: ObservableObject
{

  public Test? ObservableListTest
  {
    get { return _ObservableListTest; }
    set
    {
      if (_ObservableListTest != value)
      {
        _ObservableListTest = value;
        NotifyPropertyChanged(nameof(ObservableListTest));
      }
    }
  }
  private Test? _ObservableListTest;


  public Test? ObservableDictionaryTest
  {
    get { return _ObservableDictionaryTest; }
    set
    {
      if (_ObservableDictionaryTest != value)
      {
        _ObservableDictionaryTest = value;
        NotifyPropertyChanged(nameof(ObservableDictionaryTest));
      }
    }
  }
  private Test? _ObservableDictionaryTest;
}
