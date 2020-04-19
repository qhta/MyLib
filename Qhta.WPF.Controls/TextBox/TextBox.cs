using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Qhta.WPF.Controls
{
  /// <summary>
  /// 
  /// </summary>
  public class TextBox : System.Windows.Controls.TextBox
  {

    public TextBox() : base()
    {
      DefaultStyleKey = typeof(TextBox);
    }


    #region ClearButton
    /*
    public static DependencyProperty IsClearEnabledProperty = DependencyProperty.Register("IsClearEnabled", typeof(bool), typeof(TextBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public bool IsClearEnabled
    {
      get
      {
        return (bool)GetValue(IsClearEnabledProperty);
      }
      set
      {
        SetValue(IsClearEnabledProperty, value);
      }
    }
    */

    /*
    public static DependencyProperty ClearButtonTemplateProperty = DependencyProperty.Register("ClearButtonTemplate", typeof(DataTemplate), typeof(TextBox), new FrameworkPropertyMetadata(default(DataTemplate), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public DataTemplate ClearButtonTemplate
    {
      get
      {
        return (DataTemplate)GetValue(ClearButtonTemplateProperty);
      }
      set
      {
        SetValue(ClearButtonTemplateProperty, value);
      }
    }
    */
    #endregion

    #region EnterButton
    /*
    public static DependencyProperty ShowEnterButtonProperty = DependencyProperty.Register("ShowEnterButton", typeof(bool), typeof(TextBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public bool ShowEnterButton
    {
      get
      {
        return (bool)GetValue(ShowEnterButtonProperty);
      }
      set
      {
        SetValue(ShowEnterButtonProperty, value);
      }
    }
    */

    /*
    public static DependencyProperty EnterButtonTemplateProperty = DependencyProperty.Register("EnterButtonTemplate", typeof(DataTemplate), typeof(TextBox), new FrameworkPropertyMetadata(default(DataTemplate), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public DataTemplate EnterButtonTemplate
    {
      get
      {
        return (DataTemplate)GetValue(EnterButtonTemplateProperty);
      }
      set
      {
        SetValue(EnterButtonTemplateProperty, value);
      }
    }
    */
    #endregion

    #region InnerPadding
    /*
    public static DependencyProperty InnerPaddingProperty = DependencyProperty.Register("InnerPadding", typeof(Thickness), typeof(TextBox), new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public Thickness InnerPadding
    {
      get
      {
        return (Thickness)GetValue(InnerPaddingProperty);
      }
      set
      {
        SetValue(InnerPaddingProperty, value);
      }
    }
    */
    #endregion

    #region Character Masking
    /*
    public static DependencyProperty IsCharacterMaskingEnabledProperty = DependencyProperty.Register("IsCharacterMaskingEnabled", typeof(bool), typeof(TextBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public bool IsCharacterMaskingEnabled
    {
      get
      {
        return (bool)GetValue(IsCharacterMaskingEnabledProperty);
      }
      set
      {
        SetValue(IsCharacterMaskingEnabledProperty, value);
      }
    }
    */

    /*
    public static DependencyProperty CharacterMaskForegroundProperty = DependencyProperty.Register("CharacterMaskForeground", typeof(Brush), typeof(TextBox), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public Brush CharacterMaskForeground
    {
      get
      {
        return (Brush)GetValue(CharacterMaskForegroundProperty);
      }
      set
      {
        SetValue(CharacterMaskForegroundProperty, value);
      }
    }
    */
    #endregion

    #region Placeholder
    /*
  public static DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(TextBox), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

  public string Placeholder
  {
    get
    {
      return (string)GetValue(PlaceholderProperty);
    }
    set
    {
      SetValue(PlaceholderProperty, value);
    }
  }
  */

    /*
    public static DependencyProperty PlaceholderStyleProperty = DependencyProperty.Register("PlaceholderStyle", typeof(Style), typeof(TextBox), new FrameworkPropertyMetadata(default(Style), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public Style PlaceholderStyle
    {
      get
      {
        return (Style)GetValue(PlaceholderStyleProperty);
      }
      set
      {
        SetValue(PlaceholderStyleProperty, value);
      }
    }
    */
    #endregion

    #region ScrollViewerTemplate
    /*
  public static DependencyProperty ScrollViewerTemplateProperty = DependencyProperty.Register("ScrollViewerTemplate", typeof(ControlTemplate), typeof(TextBox), new FrameworkPropertyMetadata(default(ControlTemplate), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

  public ControlTemplate ScrollViewerTemplate
  {
    get
    {
      return (ControlTemplate)GetValue(ScrollViewerTemplateProperty);
    }
    set
    {
      SetValue(ScrollViewerTemplateProperty, value);
    }
  }
  */
    #endregion

    #region SelectAllOnFocus
    /*
    public static DependencyProperty SelectAllOnFocusProperty = DependencyProperty.Register("SelectAllOnFocus", typeof(bool), typeof(TextBox), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public bool SelectAllOnFocus
    {
      get
      {
        return (bool)GetValue(SelectAllOnFocusProperty);
      }
      set
      {
        SetValue(SelectAllOnFocusProperty, value);
      }
    }

    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      base.OnGotKeyboardFocus(e);

      if (SelectAllOnFocus)
        SelectAll();
    }

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseLeftButtonDown(e);

      if (SelectAllOnFocus && !IsKeyboardFocusWithin && !OnPreviewMouseLeftButtonDownHandled(e))
      {
        Focus();
        e.Handled = true;
      }
    }

    /// <remarks>
    /// Normally, focus is obtained when left mouse button is pressed.
    /// When clicking buttons that might be contained in the template,
    /// focus is obtained first, thus, requiring a second click in 
    /// order to actually click the button. To prevent this, we must
    /// detect whether or not the intention is to click a button or 
    /// focus. Therefore, if the element clicked IS a button, handle 
    /// the focus; otherwise, focus!
    /// </remarks>
    protected virtual bool OnPreviewMouseLeftButtonDownHandled(MouseButtonEventArgs e, Type[] HandledTypes = null)
    {
      var Parent = (e.OriginalSource as DependencyObject);

      HandledTypes = HandledTypes == null ? _.New(typeof(Button), typeof(ToggleButton)) : HandledTypes;

      while (!(Parent is TextBox))
      {
        Parent = Parent.GetParent();
        if (Parent.IsAny(HandledTypes))
          break;
      }

      return Parent.IsAny(HandledTypes);
    }

    */
    #endregion

    #region SelectAllOnTripleClick

    //public event EventHandler<RoutedEventArgs> TripleClick;

    /*
    public static DependencyProperty SelectAllOnTripleClickProperty = DependencyProperty.Register("SelectAllOnTripleClick", typeof(bool), typeof(TextBox), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public bool SelectAllOnTripleClick
    {
      get
      {
        return (bool)GetValue(SelectAllOnTripleClickProperty);
      }
      set
      {
        SetValue(SelectAllOnTripleClickProperty, value);
      }
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);

      if (e.ClickCount == 3)
      {
        OnTripleClick();
        if (SelectAllOnTripleClick)
          SelectAll();
      }
    }

    protected virtual void OnTripleClick(RoutedEventArgs e = null)
    {
      TripleClick?.Invoke(this, e == null ? new RoutedEventArgs() : e);
    }
    */
    #endregion

    #region ToggleButton
    /*
    public static DependencyProperty ShowToggleButtonProperty = DependencyProperty.Register("ShowToggleButton", typeof(bool), typeof(TextBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public bool ShowToggleButton
    {
      get
      {
        return (bool)GetValue(ShowToggleButtonProperty);
      }
      set
      {
        SetValue(ShowToggleButtonProperty, value);
      }
    }

    public static DependencyProperty ToggleButtonTemplateProperty = DependencyProperty.Register("ToggleButtonTemplate", typeof(DataTemplate), typeof(TextBox), new FrameworkPropertyMetadata(default(DataTemplate), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public DataTemplate ToggleButtonTemplate
    {
      get
      {
        return (DataTemplate)GetValue(ToggleButtonTemplateProperty);
      }
      set
      {
        SetValue(ToggleButtonTemplateProperty, value);
      }
    }
    */
    #endregion

    #region ClearCommand
    /*
    private ICommand clearCommand;

    public ICommand ClearCommand
    {
      get
      {
        clearCommand = clearCommand ?? new RelayCommand(() => Text = string.Empty, () => IsFocused && Text.Length > 0 && !IsReadOnly);
        return clearCommand;
      }
    }
    */
    #endregion

    #region EnterCommand
    /*
    private ICommand enterCommand;

    public ICommand EnterCommand
    {
      get
      {
        enterCommand = enterCommand ?? new RelayCommand(() => OnEntered(Text), () => true);
        return enterCommand;
      }
    }
    */
    #endregion

    #region OnEnterKeyDown    
    /*
    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);

      if (e.Key == Key.Enter)
        OnEntered(Text);
    }

    public event EventHandler<TextSubmittedEventArgs> Entered;

    protected virtual void OnEntered(string Text)
    {
      Entered?.Invoke(this, new TextSubmittedEventArgs(Text));
    }
    */
    #endregion

  }
}
