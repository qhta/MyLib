using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLib.MultiThreadingObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace MyLib.WpfTestUtils
{
  /// <summary>
  /// Klasa specjalnego kontekstu wizualnego do testów
  /// </summary>
  public class VisualTestContext : TestContext, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(string propertyName)
    {
      if (PropertyChanged!=null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Konstruktor inicjujący - wymaga podania writera dla metody <c>TestContext.WriteLine</c>
    /// </summary>
    /// <param name="writer"></param>
    public VisualTestContext()
      : base()
    {
      Results = new DispatchedCollection<VisualTestResult>();
      Results.Add(new VisualTestOverallResult("AllTests"));
    }

    /// <summary>
    /// Writer do zapisywania komunikatów z testów przez TestContext.WriteLine
    /// </summary>
    public TextWriter Writer { get; set; }

    /// <summary>
    /// Czas zwłoki między inicjacją a uruchomieniem pierwszego testu (w milisekundach)
    /// </summary>
    public int InitialDelay { get; set; }

    /// <summary>
    /// Czas zwłoki między uruchomieniami kolejnych testów (w milisekundach)
    /// </summary>
    public int InternalDelay { get; set; }

    /// <summary>
    /// Wyniki testów - mogą być użyte jako <c>ImageSource</c> w komponencie <c>ListView</c>
    /// </summary>
    public DispatchedCollection<VisualTestResult> Results { get; protected set; }

    public VisualTestExecution TestExecution { get; set; }

    #region metody konieczne dla implementacji z TestContext

    #region obsługa komunikatów
    /// <summary>
    /// Wypisuje komunikat poprzez writer przekazany przez konstruktor, 
    /// a jeśli dodano plik wynikowy, to i zapis do tego pliku.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="args"></param>
    public override void WriteLine(string format, params object[] args)
    {
      if (Writer != null)
        Writer.WriteLine(format, args);
      if (FileWriter != null)
      {
        FileWriter.WriteLine(format, args);
        FileWriter.Flush();
      }
    }
    TextWriter FileWriter;

    /// <summary>
    /// Nie zaimplementowano
    /// </summary>
    public override void AddResultFile(string fileName)
    {
      if (fileName == null)
        FileWriter = null;
      FileWriter = System.IO.File.CreateText(fileName);
    }
    #endregion

    #region metody związane z połączeniem danych
    /// <summary>
    /// Nie zaimplementowano
    /// </summary>
    public override DbConnection DataConnection
    {
      get { throw new NotImplementedException("VisualTestContext.DataConnection not implemented"); }
    }

    /// <summary>
    /// Nie zaimplementowano
    /// </summary>
    public override System.Data.DataRow DataRow
    {
      get { throw new NotImplementedException("VisualTestContext.DataRow not implemented"); }
    }
    #endregion

    #region metody obsługi timera

    public Dictionary<string, System.Timers.Timer> Timers { get; protected set; }

    /// <summary>
    /// Utworzenie nowego timera
    /// </summary>
    public override void BeginTimer(string timerName)
    {
      if (Timers == null)
        Timers = new Dictionary<string, System.Timers.Timer>();
      Timers.Add(timerName, new System.Timers.Timer());
    }

    /// <summary>
    /// Zatrzymanie i zniszczenie timera
    /// </summary>
    public override void EndTimer(string timerName)
    {
      System.Timers.Timer timer;
      if (Timers.TryGetValue(timerName, out timer))
      {
        timer.Dispose();
        Timers.Remove(timerName);
      }
    }

    #endregion

    #region wynik bieżącego testu
    /// <summary>
    /// Wynik bieżącego testu - możliwy do ustawienia
    /// </summary>
    public override UnitTestOutcome CurrentTestOutcome
    {
      get { return currentTestOutcome; }
    }
    private UnitTestOutcome currentTestOutcome;

    public void SetCurrentTestOutcome(UnitTestOutcome value)
    {
      currentTestOutcome = value;
    }
    #endregion

    #region dodatkowe właściwości
    /// <summary>
    /// Nie zaimplementowano
    /// </summary>
    public override IDictionary Properties
    {
      get
      {
        if (properties == null)
          properties = new Dictionary<string, object>();
        return properties;
      }
    }
    protected Dictionary<string, object> properties;

    #endregion

    #endregion

  }
}
