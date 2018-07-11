using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLib.WpfTestUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyLib.WpfTestUtils
{
  /// <summary>
  /// Interaction logic for VisualTestMainView.xaml
  /// </summary>
  public partial class VisualTestMainView : UserControl, INotifyPropertyChanged
  {
    public VisualTestMainView()
    {
      InitializeComponent();
    }

    public VisualTestContext VisualTestContext { get; set; }

    public Type TestClass { get; set; }

    public bool AutoStart { get; set; }

    public int InitialDelay { get; set; }

    public int InternalDelay { get; set; }

    public void Start()
    {
      if (TestClass != null)
      {
        writer = new VisualTextWriter(TestContextOutput);
        if (VisualTestContext!=null)
          TestContext = VisualTestContext;
        else
          TestContext = new VisualTestContext();
        if (TestContext.Writer==null)
          TestContext.Writer = writer;
        TestResults.ItemsSource = TestContext.Results;
        TestContext.InitialDelay = InitialDelay;
        TestContext.InternalDelay = InternalDelay;
        testExecution = new VisualTestExecution(TestClass);
        testExecution.Run(TestContext);
      }
    }

    VisualTextWriter writer;
    public VisualTestContext TestContext
    {
      get => _testContext;
      set
      {
        if (_testContext!=value)
        {
          _testContext=value;
          NotifyPropertyChanged(nameof(TestContext));
        }
      }
    }
    private VisualTestContext _testContext;
    VisualTestExecution testExecution;

    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged(string propertyName)
    {
      if (PropertyChanged!=null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      if (AutoStart)
        Start();
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
      if (testExecution != null)
        testExecution.Terminate();
      if (TestContext != null)
        foreach (VisualTestResult testResult in TestContext.Results)
          if (testResult.Window != null)
            testResult.Window.Close();
    }


  }
}
