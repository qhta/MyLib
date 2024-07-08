using Qhta.ObservableObjects;

namespace TestObservableObjectWpfApp;

public abstract class Test : ObservableObject
{
  public string Caption
  {
    get { return _Caption; }
    set
    {
      if (_Caption != value)
      {
        _Caption = value;
        NotifyPropertyChanged(nameof(Caption));
      }
    }
  }
  private string _Caption="";

  public object? DataContext
  {
    get { return _DataContext; }
    set
    {
      if (_DataContext != value)
      {
        _DataContext = value;
        NotifyPropertyChanged(nameof(DataContext));
      }
    }
  }
  private object? _DataContext;

  public string Status
  {
    get { return _Status; }
    set
    {
      if (_Status != value)
      {
        _Status = value;
        NotifyPropertyChanged(nameof(Status));
      }
    }
  }
  private string _Status = "";

  public abstract void Run();
}
