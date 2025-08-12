using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Qhta.SF.Tools.Views;

/// <summary>
/// Modes for the specific value window.
/// </summary>
public enum SpecificValueWindowMode
{
  /// <summary>
  /// The window is used to edit the value in a text box.
  /// </summary>
  EditViewOnly = 1,
  /// <summary>
  /// The window is used to select a value from a list.
  /// </summary>
  SelectorOnly,
  /// <summary>
  /// The window is used to select or edit a value in a text box or select from a list.
  /// </summary>
  Both
}

/// <summary>
/// Window for selecting or editing a specific value.
/// </summary>
public partial class SpecificValueWindow : Window
{
  /// <summary>
  /// Initializes a new instance of the <see cref="SpecificValueWindow"/> class.
  /// </summary>
  public SpecificValueWindow()
  {
    InitializeComponent();
    Loaded += SpecifiedValueWindow_Loaded;
  }

  private void SpecifiedValueWindow_Loaded(object sender, RoutedEventArgs e)
  {
    UpdateTabControlVisibility();
  }

  /// <summary>
  /// Dependency property for the prompt text displayed in the window.
  /// </summary>
  public static DependencyProperty PromptProperty =
      DependencyProperty.Register(nameof(Prompt), typeof(string), typeof(SpecificValueWindow), new PropertyMetadata(null));

  /// <summary>
  /// Prompt text displayed in the window, guiding the user on what to select.
  /// </summary>
  public string? Prompt
  {
     [DebuggerStepThrough] get => (string?)GetValue(PromptProperty);
     set => SetValue(PromptProperty, value);
  }

  /// <summary>
  /// Dependency property for <see cref="Mode"/> property, which specifies the mode of the window, which can be either EditViewOnly, SelectorOnly, or Both.
  /// </summary>
  public static readonly DependencyProperty ModeProperty =
    DependencyProperty.Register(nameof(Mode), typeof(SpecificValueWindowMode), typeof(SpecificValueWindow), new PropertyMetadata(SpecificValueWindowMode.Both, OnModeChanged));

  /// <summary>
  /// Specifies the mode of the window, which can be either EditViewOnly, SelectorOnly, or Both.
  /// </summary>
  public SpecificValueWindowMode Mode
  {
    get => (SpecificValueWindowMode)GetValue(ModeProperty);
    set => SetValue(ModeProperty, value);
  }

  private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if (d is SpecificValueWindow window)
    {
      window.UpdateTabControlVisibility();
    }
  }

  private void UpdateTabControlVisibility()
  {
    switch (Mode)
    {
      case SpecificValueWindowMode.EditViewOnly:
        TabControl.Style = (Style)FindResource("HiddenTabControlStyle");
        TabControl.SelectedIndex = 0;
        break;

      case SpecificValueWindowMode.SelectorOnly:
        TabControl.Style = (Style)FindResource("HiddenTabControlStyle");
        TabControl.SelectedIndex = 1;
        break;

      case SpecificValueWindowMode.Both:
        TabControl.Style = (Style)FindResource(typeof(TabControl)); // Default style
        break;
    }
  }

  #region TextValueEdit

  /// <summary>
  /// Dependency property for the <see cref="TextValue"/> property, which is the value entered by the user in the text box.
  /// </summary>
  public static DependencyProperty TextValueProperty =
    DependencyProperty.Register(nameof(TextValue), typeof(object), typeof(SpecificValueWindow), new PropertyMetadata(null));

  /// <summary>
  /// Text value entered by the user in the text box.
  /// </summary>
  public object? TextValue
  {
    [DebuggerStepThrough]
    get => GetValue(TextValueProperty);
    set => SetValue(TextValueProperty, value);
  }
  #endregion

  #region ListValueSelection
  /// <summary>
  /// Dependency property for the items source, which is the collection of items to select from.
  /// </summary>
  public static DependencyProperty ItemsSourceProperty =
      DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(SpecificValueWindow), new PropertyMetadata(null));

  /// <summary>
  /// Items source for the selection, typically a collection of items that can be selected.
  /// </summary>
  public object? ItemsSource
  {
    [DebuggerStepThrough] get => GetValue(ItemsSourceProperty);
    set => SetValue(ItemsSourceProperty, value);
  }

  /// <summary>
  /// Dependency property for the selected item, which is the item currently selected by the user.
  /// </summary>
  public static DependencyProperty SelectedItemProperty =
      DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(SpecificValueWindow), new PropertyMetadata(null));

  /// <summary>
  /// Selected item in the selection window. This property is bound to the selected item in the ItemsSource.
  /// </summary>
  public object? SelectedItem
  {
    [DebuggerStepThrough] get => GetValue(SelectedItemProperty);
    set => SetValue(SelectedItemProperty, value);
  }
  #endregion

  #region OverwriteNonEmptyCells
  /// <summary>
  /// Dependency property for the <see cref="ShowOverwriteNonEmptyCells"/> flag, which indicates whether to show <see cref="OverwriteNonEmptyCells"/> checkbox.
  /// </summary>
  public static DependencyProperty ShowOverwriteNonEmptyCellsProperty =
    DependencyProperty.Register(nameof(ShowOverwriteNonEmptyCells), typeof(bool), typeof(SpecificValueWindow), new PropertyMetadata(true));

  /// <summary>
  /// Indicates whether to fill only empty cells in the selection. If true, only items that are empty will be filled.
  /// </summary>
  public bool ShowOverwriteNonEmptyCells
  {
    [DebuggerStepThrough]
    get => (bool)GetValue(ShowOverwriteNonEmptyCellsProperty);
    set => SetValue(ShowOverwriteNonEmptyCellsProperty, value);
  }

  /// <summary>
  /// Dependency property for the <see cref="OverwriteNonEmptyCells"/> flag, which indicates whether to fill only empty cells in the selection.
  /// </summary>
  public static DependencyProperty OverwriteNonEmptyCellsProperty =
      DependencyProperty.Register(nameof(OverwriteNonEmptyCells), typeof(bool), typeof(SpecificValueWindow), new PropertyMetadata(true));


  /// <summary>
  /// Indicates whether to fill only empty cells in the selection. If true, only items that are empty will be filled.
  /// </summary>
  public bool OverwriteNonEmptyCells
  {
    [DebuggerStepThrough] get => (bool)GetValue(OverwriteNonEmptyCellsProperty);
    set => SetValue(OverwriteNonEmptyCellsProperty, value);
  }
  #endregion

  #region FindInSequence
  /// <summary>
  /// Dependency property for the <see cref="ShowFindInSequence"/> flag, which indicates whether to show <see cref="FindInSequence"/> combobox.
  /// </summary>
  public static DependencyProperty ShowFindInSequenceProperty =
    DependencyProperty.Register(nameof(ShowFindInSequence), typeof(bool), typeof(SpecificValueWindow), new PropertyMetadata(true));

  /// <summary>
  /// Indicates whether to fill only empty cells in the selection. If true, only items that are empty will be filled.
  /// </summary>
  public bool ShowFindInSequence
  {
    [DebuggerStepThrough]
    get => (bool)GetValue(ShowFindInSequenceProperty);
    set => SetValue(ShowFindInSequenceProperty, value);
  }

  /// <summary>
  /// Dependency property for the <see cref="FindInSequence"/> flag, which indicates how to find value in the context of the current selection.
  /// </summary>
  public static DependencyProperty FindInSequenceProperty =
    DependencyProperty.Register(nameof(FindInSequence), typeof(FindInSequence), typeof(SpecificValueWindow), new PropertyMetadata(FindInSequence.FindNext));


  /// <summary>
  /// Indicates whether to fill only empty cells in the selection. If true, only items that are empty will be filled.
  /// </summary>
  public FindInSequence FindInSequence
  {
    [DebuggerStepThrough]
    get => (FindInSequence)GetValue(FindInSequenceProperty);
    set => SetValue(FindInSequenceProperty, value);
  }
  #endregion

  #region Ok/Cancel Buttons
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
  #endregion
}
