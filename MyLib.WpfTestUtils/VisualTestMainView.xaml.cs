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

    public static DependencyProperty OrientationProperty = DependencyProperty.Register
      ("Orientation", typeof(Orientation), typeof(VisualTestMainView),
       new PropertyMetadata(Orientation.Vertical));
    public Orientation Orientation
    {
      get => (Orientation)GetValue(OrientationProperty);
      set => SetValue(OrientationProperty, value);
    }

    public override void OnApplyTemplate()
    {
      switch (Orientation)
      {
        case Orientation.Vertical:
          MainGrid.RowDefinitions.Add(new RowDefinition { Height=new GridLength(1,GridUnitType.Star) });
          MainGrid.RowDefinitions.Add(new RowDefinition { Height=new GridLength(5) });
          MainGrid.RowDefinitions.Add(new RowDefinition { Height=new GridLength(1, GridUnitType.Star) });
          Splitter.SetValue(Grid.RowProperty, 1);
          Splitter.Height=5;
          Splitter.ResizeDirection=GridResizeDirection.Rows;
          TestResults.SetValue(Grid.RowProperty, 2);
          break;
        case Orientation.Horizontal:
          MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width=new GridLength(1, GridUnitType.Star) });
          MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width=new GridLength(5) });
          MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width=new GridLength(1, GridUnitType.Star) });
          Splitter.SetValue(Grid.ColumnProperty, 1);
          Splitter.Width=5;
          Splitter.HorizontalAlignment=HorizontalAlignment.Stretch;
          TestResults.SetValue(Grid.ColumnProperty, 2);
          break;
      }
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
