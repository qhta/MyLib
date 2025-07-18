﻿using System.Windows;

namespace Qhta.UnicodeBuild.Views;

/// <summary>
/// Window for selecting a value from a list.
/// </summary>
public partial class SelectValueWindow : Window
{
  /// <summary>
  /// Initializes a new instance of the <see cref="SelectValueWindow"/> class.
  /// </summary>
  public SelectValueWindow()
  {
    InitializeComponent();
  }

  /// <summary>
  /// Dependency property for the prompt text displayed in the window.
  /// </summary>
  public static DependencyProperty PromptProperty =
      DependencyProperty.Register(nameof(Prompt), typeof(string), typeof(SelectValueWindow), new PropertyMetadata(null));

  /// <summary>
  /// Prompt text displayed in the window, guiding the user on what to select.
  /// </summary>
  public string? Prompt
  {
     get => (string?)GetValue(PromptProperty);
     set => SetValue(PromptProperty, value);
  }

  /// <summary>
  /// Dependency property for the items source, which is the collection of items to select from.
  /// </summary>
  public static DependencyProperty ItemsSourceProperty =
      DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(SelectValueWindow), new PropertyMetadata(null));

  /// <summary>
  /// Items source for the selection, typically a collection of items that can be selected.
  /// </summary>
  public object? ItemsSource
  {
    get => GetValue(ItemsSourceProperty);
    set => SetValue(ItemsSourceProperty, value);
  }

  /// <summary>
  /// Dependency property for the selected item, which is the item currently selected by the user.
  /// </summary>
  public static DependencyProperty SelectedItemProperty =
      DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(SelectValueWindow), new PropertyMetadata(null));

  /// <summary>
  /// Selected item in the selection window. This property is bound to the selected item in the ItemsSource.
  /// </summary>
  public object? SelectedItem
  {
    get => GetValue(SelectedItemProperty);
    set => SetValue(SelectedItemProperty, value);
  }

  /// <summary>
  /// Dependency property for the EmptyCellsOnly flag, which indicates whether to show only empty cells in the selection.
  /// </summary>
  public static DependencyProperty EmptyCellsOnlyProperty =
      DependencyProperty.Register(nameof(EmptyCellsOnly), typeof(bool), typeof(SelectValueWindow), new PropertyMetadata(true));

  /// <summary>
  /// Indicates whether to fill only empty cells in the selection. If true, only items that are empty will be filled.
  /// </summary>
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
