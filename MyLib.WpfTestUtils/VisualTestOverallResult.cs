using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.WpfTestUtils
{
  public class VisualTestOverallResult: VisualTestResult
  {
    public VisualTestOverallResult(string name) : base(name) { }

    public int OverallTestsCount
    {
      get { return overallTestsCount; }
      set
      {
        if (overallTestsCount!=value)
        {
          overallTestsCount = value;
          RaisePropertyChanged("OverallTestsCount");
          RaisePropertyChanged("Message");
        }
      }
    }
    private int overallTestsCount;

    public int PassedTestsCount
    {
      get { return passedTestsCount; }
      set
      {
        if (passedTestsCount != value)
        {
          passedTestsCount = value;
          RaisePropertyChanged("PassedTestsCount");
          RaisePropertyChanged("Message");
        }
      }
    }
    private int passedTestsCount;

    public int FailedTestsCount
    {
      get { return failedTestsCount; }
      set
      {
        if (failedTestsCount != value)
        {
          failedTestsCount = value;
          RaisePropertyChanged("FailedTestsCount");
          RaisePropertyChanged("Message");
        }
      }
    }
    private int failedTestsCount;

    public int InconclusiveTestsCount
    {
      get { return inconclusiveTestsCount; }
      set
      {
        if (inconclusiveTestsCount != value)
        {
          inconclusiveTestsCount = value;
          RaisePropertyChanged("InconclusiveTestsCount");
          RaisePropertyChanged("Message");
        }
      }
    }
    private int inconclusiveTestsCount;

    public new string Message
    {
      get
      {
        switch (Outcome)
        {
          case UnitTestOutcome.InProgress:
          case UnitTestOutcome.Passed:
            return String.Format("{0} of {1} tests passed", PassedTestsCount, OverallTestsCount);
          case UnitTestOutcome.Failed:
          case UnitTestOutcome.Inconclusive:
            string result = String.Format("{0} of {1} tests passed", PassedTestsCount, OverallTestsCount);
            if (FailedTestsCount!=0)
              result += String.Format(", {0} failed", FailedTestsCount);
            if (InconclusiveTestsCount!=0)
              result += String.Format(", {0} inconclusive", InconclusiveTestsCount);
            return result;
        }
        return null;
      }
    }
  
  }
}
