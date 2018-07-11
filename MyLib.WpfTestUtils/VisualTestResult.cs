using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyLib.WpfTestUtils
{
  public class VisualTestResult: INotifyPropertyChanged
  {
    public VisualTestResult(string name, int? ordNum=null)
    {
      Name = name;
      OrdNum = ordNum;
      Outcome = UnitTestOutcome.Unknown;
    }
    public string Name { get; protected set; }

    public int? OrdNum { get; protected set; }

    public UnitTestOutcome Outcome 
    {
      get { return outcome; }
      set
      {
        if (outcome!=value)
        {
          outcome = value;
          RaisePropertyChanged("Outcome");
        }
      }
    }
    private UnitTestOutcome outcome;

    public string Message
    {
      get { return message; }
      set
      {
        if (message != value)
        {
          message = value;
          RaisePropertyChanged("Message");
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
          RaisePropertyChanged("ExecTime");
        }
      }
    }
    private long execTime;


    public Window Window { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public void RaisePropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

  }
}
