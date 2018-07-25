using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLib.MultiThreadingObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyLib.WpfTestUtils
{
  public class VisualTestResult: DispatchedObject
  {
    public VisualTestResult(string name)
    {
      Name = name;
    }

    public VisualTestResult(TestMethodInfo testMethod)
    {
      Name = testMethod.Method.Name;
      OrdNum = testMethod.Number;
      Outcome = TestState.Unknown;
      testMethod.PropertyChanged+=TestMethod_PropertyChanged;
      testMethod.Result=this;
    }

    private void TestMethod_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName==nameof(TestMethodInfo.State))
        Outcome = (sender as TestMethodInfo).State;
    }

    public int? OrdNum { get; protected set; }

    public TestState Outcome 
    {
      get { return outcome; }
      set
      {
        if (outcome!=value)
        {
          outcome = value;
          NotifyPropertyChanged(nameof(Outcome));
        }
      }
    }
    private TestState outcome;

    public string Message
    {
      get { return message; }
      set
      {
        if (message != value)
        {
          message = value;
          NotifyPropertyChanged(nameof(Message));
        }
      }
    }
    private string message;

    public long ExecTime
    {
      get { return execTime; }
      set
      {
        if (execTime != value)
        {
          execTime = value;
          NotifyPropertyChanged(nameof(ExecTime));
        }
      }
    }
    private long execTime;


    public Window Window { get; set; }

  }
}
