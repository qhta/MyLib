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
          NotifyPropertyChanged(nameof(OverallTestsCount));
          NotifyPropertyChanged(nameof(Message));
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
          NotifyPropertyChanged(nameof(PassedTestsCount));
          NotifyPropertyChanged(nameof(Message));
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
          NotifyPropertyChanged(nameof(FailedTestsCount));
          NotifyPropertyChanged(nameof(Message));
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
          NotifyPropertyChanged(nameof(InconclusiveTestsCount));
          NotifyPropertyChanged(nameof(Message));
        }
      }
    }
    private int inconclusiveTestsCount;

    public int CancelledTestsCount
    {
      get { return cancelledTestsCount; }
      set
      {
        if (cancelledTestsCount != value)
        {
          cancelledTestsCount = value;
          NotifyPropertyChanged(nameof(CancelledTestsCount));
          NotifyPropertyChanged(nameof(Message));
        }
      }
    }
    private int cancelledTestsCount;

    public new string Message
    {
      get
      {
        string result = String.Format("{0} of {1} tests passed", PassedTestsCount, OverallTestsCount);
        if (FailedTestsCount!=0)
          result += String.Format(", {0} failed", FailedTestsCount);
        if (InconclusiveTestsCount!=0)
          result += String.Format(", {0} inconclusive", InconclusiveTestsCount);
        if (CancelledTestsCount!=0)
          result += String.Format(", {0} cancelled", CancelledTestsCount);
        return result;
      }
    }
  
  }
}
