using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.WpfTestUtils
{
  public class TestMethodInfo: INotifyPropertyChanged
  {
    public TestMethodInfo(int number, MethodInfo method)
    {
      Number = number;
      Method = method;
    }
    public int Number { get; private set; }
    public MethodInfo Method { get; private set; }
    public TestState State
    {
      get => _State;
      set
      {
        if (_State!=value)
        {
          _State=value;
          NotifyPropertyChanged(nameof(State));
        }
      }
    }
    private TestState _State;

    public List<TestMethodInfo> RunAfter { get; set; }

    public VisualTestResult Result { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(string propertyName)
    {
      if (PropertyChanged!=null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
