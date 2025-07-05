using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Qhta.UnicodeBuild.Views;
/// <summary>
/// Interaction logic for SelectValueView.xaml
/// </summary>
public partial class SelectValueWindow : Window
{
  public SelectValueWindow()
  {
    InitializeComponent();
  }

  public static DependencyProperty PromptProperty =
      DependencyProperty.Register(nameof(Prompt), typeof(string), typeof(SelectValueWindow), new PropertyMetadata(null));
  public string? Prompt
  {
     get => (string?)GetValue(PromptProperty);
     set => SetValue(PromptProperty, value);
  }

  public static DependencyProperty ItemsSourceProperty =
      DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(SelectValueWindow), new PropertyMetadata(null));

  public object? ItemsSource
  {
    get => GetValue(ItemsSourceProperty);
    set => SetValue(ItemsSourceProperty, value);
  }

  public static DependencyProperty SelectedItemProperty =
      DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(SelectValueWindow), new PropertyMetadata(null));

  public object? SelectedItem
  {
    get => GetValue(SelectedItemProperty);
    set => SetValue(SelectedItemProperty, value);
  }

  public static DependencyProperty EmptyCellsOnlyProperty =
      DependencyProperty.Register(nameof(EmptyCellsOnly), typeof(bool), typeof(SelectValueWindow), new PropertyMetadata(true));

  public bool EmptyCellsOnly
  {
    get => (bool)GetValue(EmptyCellsOnlyProperty);
    set => SetValue(EmptyCellsOnlyProperty, value);
  }

  private void OkButton_OnClick(object sender, RoutedEventArgs e)
  {
     var window = Window.GetWindow(this);
     if (window == null) return;
    window.DialogResult = true;
    window.Close();
  }


  private void CancelButton_OnClick(object sender, RoutedEventArgs e)
  {
    var window = Window.GetWindow(this);
    if (window == null) return;
    window.DialogResult = false;
    window.Close();
  }
}
