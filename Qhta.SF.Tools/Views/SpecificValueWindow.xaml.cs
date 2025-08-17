using System.ComponentModel;
using System.Diagnostics;
using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;

using Syncfusion.Data;
using Syncfusion.Windows.Controls.Input;

namespace Qhta.SF.Tools.Views;

/// <summary>
/// Modes for the specific window.
/// </summary>
public enum SpecificWindowMode
{
  /// <summary>
  /// The window is used to edit the value for FillCommand.
  /// </summary>
  Fill = 1,
  /// <summary>
  /// The window is used to edit the value for FindCommand.
  /// </summary>
  Find,
  /// <summary>
  /// The window is used to edit the value for FindAndReplaceCommand.
  /// </summary>
  FindAndReplace
}

/// <summary>
/// Modes for the specific edit view.
/// </summary>
public enum SpecificViewMode
{
  /// <summary>
  /// <see cref="SpecificValueEdit"/> is visible.
  /// </summary>
  Edit = 1,
  /// <summary>
  /// <see cref="SpecificValueSelector"/> is visible.
  /// </summary>
  Selector,
  /// <summary>
  /// Both views are visible.
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
    Replace = false;    
    Loaded += SpecifiedValueWindow_Loaded;
  }

  private void SpecifiedValueWindow_Loaded(object sender, RoutedEventArgs e)
  {
    UpdateTabControlVisibility();
  }

  #region Prompt
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
    [DebuggerStepThrough]
    get => (string?)GetValue(PromptProperty);
    set => SetValue(PromptProperty, value);
  }
  #endregion

  #region WindowMode
  /// <summary>
  /// Dependency property for <see cref="WindowMode"/> property, which specifies the which specific view is visible, which can be either EditViewOnly, SelectorOnly, or Both.
  /// </summary>
  public static readonly DependencyProperty WindowModeProperty =
    DependencyProperty.Register(nameof(WindowMode), typeof(SpecificWindowMode), typeof(SpecificValueWindow), new PropertyMetadata(SpecificWindowMode.Fill, OnWindowModeChanged));

  /// <summary>
  /// Specifies the mode of the window, which can be either EditViewOnly, SelectorOnly, or Both.
  /// </summary>
  public SpecificWindowMode WindowMode
  {
    get => (SpecificWindowMode)GetValue(WindowModeProperty);
    set => SetValue(WindowModeProperty, value);
  }

  private static void OnWindowModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if (d is SpecificValueWindow window)
    {
      window.UpdateCheckBoxesVisibility();
      window.UpdateTabControlVisibility();
    }
  }

  private void UpdateCheckBoxesVisibility()
  {
    switch (WindowMode)
    {
      case SpecificWindowMode.Fill:
        OverWriteNonEmptyCellsButton.Visibility = Visibility.Visible;
        FindInSequenceComboBox.Visibility = Visibility.Collapsed;
        break;

      case SpecificWindowMode.Find:
        OverWriteNonEmptyCellsButton.Visibility = Visibility.Collapsed;
        FindInSequenceComboBox.Visibility = Visibility.Visible;
        ValueSelector.ReplaceCheckBox.Visibility = Visibility.Collapsed;
        ValueEdit.ReplaceCheckBox.Visibility = Visibility.Collapsed;
        break;

      case SpecificWindowMode.FindAndReplace:
        OverWriteNonEmptyCellsButton.Visibility = Visibility.Collapsed;
        FindInSequenceComboBox.Visibility = Visibility.Visible;
        ValueSelector.ReplaceCheckBox.Visibility = Visibility.Visible;
        ValueEdit.ReplaceCheckBox.Visibility = Visibility.Visible;
        break;
    }
  }
  #endregion

  #region ViewMode
  /// <summary>
  /// Dependency property for <see cref="ViewMode"/> property, which specifies the which specific view is visible, which can be either EditViewOnly, SelectorOnly, or Both.
  /// </summary>
  public static readonly DependencyProperty ViewModeProperty =
    DependencyProperty.Register(nameof(ViewMode), typeof(SpecificViewMode), typeof(SpecificValueWindow), new PropertyMetadata(SpecificViewMode.Both, OnViewModeChanged));

  /// <summary>
  /// Specifies the mode of the window, which can be either EditViewOnly, SelectorOnly, or Both.
  /// </summary>
  public SpecificViewMode ViewMode
  {
    get => (SpecificViewMode)GetValue(ViewModeProperty);
    set => SetValue(ViewModeProperty, value);
  }

  private static void OnViewModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if (d is SpecificValueWindow window)
    {
      window.UpdateTabControlVisibility();
    }
  }

  private void UpdateTabControlVisibility()
  {
    switch (ViewMode)
    {
      case SpecificViewMode.Edit:
        TabControl.Style = (Style)FindResource("HiddenTabControlStyle");
        TabControl.SelectedIndex = 1;
        break;

      case SpecificViewMode.Selector:
        TabControl.Style = (Style)FindResource("HiddenTabControlStyle");
        TabControl.SelectedIndex = 0;
        break;

      case SpecificViewMode.Both:
        TabControl.Style = (Style)FindResource(typeof(TabControl)); // Default style
        break;
    }
  }

  /// <summary>
  /// Gets or sets the current view mode of the window.
  /// </summary>
  /// <returns></returns>
  public SpecificViewMode CurrentViewMode
  {
    get
    {
      if (ViewMode == SpecificViewMode.Both)
        return (TabControl.SelectedIndex == 0) ? SpecificViewMode.Selector : SpecificViewMode.Edit;
      return ViewMode;
    }
    set
    {
      if (ViewMode == SpecificViewMode.Both)
      {
        TabControl.SelectedIndex = (value == SpecificViewMode.Edit) ? 0 : 1;
      }
      else
      {
        TabControl.SelectedIndex = 0; // Only one view is visible, so always select the first tab.
      }
    }
  }
  #endregion

  #region TextValue
  /// <summary>
  /// Dependency property for the <see cref="TextValue"/> property, which is the value entered by the user in the text box.
  /// </summary>
  public static DependencyProperty TextValueProperty =
    DependencyProperty.Register(nameof(TextValue), typeof(string), typeof(SpecificValueWindow), new PropertyMetadata(null));

  /// <summary>
  /// Text value entered by the user in the text box.
  /// </summary>
  public string? TextValue
  {
    [DebuggerStepThrough]
    get => (string?)GetValue(TextValueProperty);
    set => SetValue(TextValueProperty, value);
  }
  #endregion

  #region Replace
  /// <summary>
  /// Dependency property for the <see cref="Replace"/> property.
  /// </summary>
  public static DependencyProperty ReplaceProperty =
    DependencyProperty.Register(nameof(Replace), typeof(bool), typeof(SpecificValueWindow), new PropertyMetadata(true, 
      (d,e)=> (d as SpecificValueWindow)?.ReplaceChanged(e)));

  private void ReplaceChanged(DependencyPropertyChangedEventArgs e)
  {
    if (e.NewValue is bool replace)
    {
      ValueEdit.OnNotifyPropertyChanged(this, new PropertyChangedEventArgs(nameof(Replace)));
      ValueSelector.OnNotifyPropertyChanged(this, new PropertyChangedEventArgs(nameof(Replace)));
    }
  }

  /// <summary>
  /// Should use replacement text.
  /// </summary>
  public bool Replace
  {
    [DebuggerStepThrough]
    get => (bool)GetValue(ReplaceProperty);
    set => SetValue(ReplaceProperty, value);
  }

  #endregion

  #region ReplacementText
  /// <summary>
  /// Dependency property for the <see cref="ReplacementText"/> property.
  /// </summary>
  public static DependencyProperty ReplacementTextProperty =
    DependencyProperty.Register(nameof(ReplacementText), typeof(string), typeof(SpecificValueWindow), new PropertyMetadata(null));

  /// <summary>
  /// Replacement text entered by the user.
  /// </summary>
  public string? ReplacementText
  {
    [DebuggerStepThrough]
    get => (string?)GetValue(ReplacementTextProperty);
    set => SetValue(ReplacementTextProperty, value);
  }
  #endregion

  #region ListValueSelection
  /// <summary>
  /// Dependency property for the <see cref="ItemsSource"/>, which is the collection of items to select from.
  /// </summary>
  public static DependencyProperty ItemsSourceProperty =
      DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(SpecificValueWindow), new PropertyMetadata(null));

  /// <summary>
  /// Items source for the selection, typically a collection of items that can be selected.
  /// </summary>
  public object? ItemsSource
  {
    [DebuggerStepThrough]
    get => GetValue(ItemsSourceProperty);
    set => SetValue(ItemsSourceProperty, value);
  }

  /// <summary>
  /// Dependency property for the <see cref="SelectedItem"/>, which is the item currently selected by the user.
  /// </summary>
  public static DependencyProperty SelectedItemProperty =
      DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(SpecificValueWindow), new PropertyMetadata(null));

  /// <summary>
  /// Selected item in the selection window. This property is bound to the selected item in the ItemsSource.
  /// </summary>
  public object? SelectedItem
  {
    [DebuggerStepThrough]
    get => GetValue(SelectedItemProperty);
    set => SetValue(SelectedItemProperty, value);
  }

  /// <summary>
  /// Dependency property for the <see cref="ReplacementSource"/>, which is the collection of replacement items to select from.
  /// </summary>
  public static DependencyProperty ReplacementSourceProperty =
    DependencyProperty.Register(nameof(ReplacementSource), typeof(object), typeof(SpecificValueWindow), new PropertyMetadata(null));

  /// <summary>
  /// Replacement source for the selection, typically a collection of Replacement that can be selected.
  /// </summary>
  public object? ReplacementSource
  {
    [DebuggerStepThrough]
    get => GetValue(ReplacementSourceProperty);
    set => SetValue(ReplacementSourceProperty, value);
  }
  /// <summary>
  /// Dependency property for the <see cref="ReplacementItem"/>, which is the item selected for Replacement by the user.
  /// </summary>
  public static DependencyProperty ReplacementItemProperty =
    DependencyProperty.Register(nameof(ReplacementItem), typeof(object), typeof(SpecificValueWindow), new PropertyMetadata(null));

  /// <summary>
  /// Replacement item in the selection window. This property is bound to the selected item in the ItemsSource.
  /// </summary>
  public object? ReplacementItem
  {
    [DebuggerStepThrough]
    get => GetValue(ReplacementItemProperty);
    set => SetValue(ReplacementItemProperty, value);
  }
  #endregion

  #region OverwriteNonEmptyCells
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
    [DebuggerStepThrough]
    get => (bool)GetValue(OverwriteNonEmptyCellsProperty);
    set => SetValue(OverwriteNonEmptyCellsProperty, value);
  }
  #endregion

  #region FindInSequence

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

  #region FilterType
  /// <summary>
  /// Dependency property for the <see cref="FilterType"/> property.
  /// </summary>
  public static DependencyProperty FilterTypeProperty =
    DependencyProperty.Register(nameof(FilterType), typeof(FilterType), typeof(SpecificValueWindow), new PropertyMetadata(FilterType.Contains));


  /// <summary>
  /// Determines the type of filter to apply when searching for values in the data grid.
  /// </summary>
  public FilterType FilterType
  {
    [DebuggerStepThrough]
    get => (FilterType)GetValue(FilterTypeProperty);
    set => SetValue(FilterTypeProperty, value);
  }
  #endregion

  #region CaseSensitive
  /// <summary>
  /// Dependency property for the <see cref="CaseSensitive"/> flag, which indicates whether to search text with case-sensitivity.
  /// </summary>
  public static DependencyProperty CaseSensitiveProperty =
    DependencyProperty.Register(nameof(CaseSensitive), typeof(bool), typeof(SpecificValueWindow), new PropertyMetadata(false));


  /// <summary>
  /// Indicates whether to search text with case-sensitivity.
  /// </summary>
  public bool CaseSensitive
  {
    [DebuggerStepThrough]
    get => (bool)GetValue(CaseSensitiveProperty);
    set => SetValue(CaseSensitiveProperty, value);
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

  private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    if (sender is TabControl tabControl && ViewMode == SpecificViewMode.Both)
    {
      if (tabControl.SelectedIndex == 1)
      {
        if (SelectedItem is ISelectableItem selectableItem)
        {
          var displayName = selectableItem.DisplayName;
          if (!displayName.StartsWith("(")) TextValue = selectableItem.DisplayName;
        }
      }
      else
      if (tabControl.SelectedIndex == 0)
      {
        if (!String.IsNullOrEmpty(TextValue))
        {
          if (ItemsSource is IQueryable<ISelectableItem> selectableItems)
          {
            var foundItem = selectableItems.FirstOrDefault(item => item.DisplayName == TextValue);
            if (foundItem != null) SelectedItem = foundItem;
          }
        }
      }
    }
  }
}
