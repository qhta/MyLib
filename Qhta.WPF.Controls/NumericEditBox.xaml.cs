using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Qhta.WPF.Utils;

namespace Qhta.WPF.Controls
{
  /// <summary>
  /// Kontrolka łącząca <c>TextBox</c> z <see cref="NumericUpDown"/>
  /// </summary>
  public partial class NumericEditBox : UserControl
  {
    /// <summary>
    /// Konstruktor inicjujący
    /// </summary>
    public NumericEditBox()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Ustawienie konwertera przy zastosowaniu szablonu
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      var numericValueConverter = (NumericValueConverter)FindResource("NumericValueConverter");
      numericValueConverter.Culture = this.ContentCulture;
      numericValueConverter.Format = this.ContentStringFormat;
      NumericUpDown.ValueChanged+=NumericUpDown_ValueChanged;
    }

    private void NumericUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> args)
    {
      OnValueChanged(args.OldValue, args.NewValue);
      //Value=args.NewValue;
    }

    /// <summary>
    /// Krok, o który zwiększa/zmniejsza się wartość.
    /// </summary>
    public decimal Increment
    {
      get { return (decimal)GetValue(IncrementProperty); }
      set { SetValue(IncrementProperty, value); }
    }

    /// <summary>
    /// Właściwość zależna dla właściwości <see cref="Increment"/>
    /// </summary>
    public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register
      ("Increment", typeof(decimal), typeof(NumericEditBox),
        new PropertyMetadata(1m));

    /// <summary>
    /// Krok, o który zwiększa/zmniejsza się wartość przy przytrzymaniu klawisza modifikacji
    /// </summary>
    public decimal PageIncrement
    {
      get { return (decimal)GetValue(PageIncrementProperty); }
      set { SetValue(PageIncrementProperty, value); }
    }

    /// <summary>
    /// Właściwość zależna dla właściwości <see cref="PageIncrement"/>
    /// </summary>
    public static readonly DependencyProperty PageIncrementProperty = DependencyProperty.Register
      ("PageIncrement", typeof(decimal), typeof(NumericEditBox),
        new PropertyMetadata(10m));

    /// <summary>
    /// Klawisz modifikacji. Powoduje zwiększanie/zmniejszanie wartości o krok <see cref="PageIncrement"/>
    /// </summary>
    public ModifierKeys ModifierKey
    {
      get { return (ModifierKeys)GetValue(ModifierKeyProperty); }
      set { SetValue(ModifierKeyProperty, value); }
    }

    /// <summary>
    /// Właściwość zależna dla właściwości <see cref="PageIncrement"/>
    /// </summary>
    public static readonly DependencyProperty ModifierKeyProperty = DependencyProperty.Register
      ("ModifierKey", typeof(ModifierKeys), typeof(NumericEditBox),
        new PropertyMetadata(ModifierKeys.Shift));


    /// <summary>
    /// Wartość maksymalna
    /// </summary>
    public decimal Maximum
    {
      get { return (decimal)GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

    /// <summary>
    /// Właściwość zależna dla właściwości <see cref="Maximum"/>
    /// </summary>
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register
      ("Maximum", typeof(decimal), typeof(NumericEditBox),
        new PropertyMetadata(decimal.MaxValue));

    /// <summary>
    /// Wartość minimalna
    /// </summary>
    public decimal Minimum
    {
      get { return (decimal)GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

    /// <summary>
    /// Właściwość zależna dla właściwości <see cref="Minimum"/>
    /// </summary>
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register
      ("Minimum", typeof(decimal), typeof(NumericEditBox),
         new PropertyMetadata(0m));

    /// <summary>
    /// Wartość aktualna
    /// </summary>
    public decimal Value
    {
      get { return (decimal)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    /// <summary>
    /// Właściwość zależna właściwości <see cref="Value"/>
    /// </summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register
      ("Value", typeof(decimal), typeof(NumericEditBox),
        new PropertyMetadata(0m,
          new PropertyChangedCallback(OnValuePropertyChanged)));

    /// <summary> 
    ///   Statyczna obsługa zdarzenia zmiany właściwości <see cref="ValueProperty"/>
    /// </summary>
    private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      NumericEditBox ctrl = (NumericEditBox)d;
      ctrl.OnValueChanged((decimal)e.OldValue, (decimal)e.NewValue);
    }

    /// <summary> 
    /// Obsługa zdarzenia zmiany wartości aktualnej
    /// </summary> 
    protected virtual void OnValueChanged(decimal oldValue, decimal newValue)
    {
      this.Value = newValue;
      if (ValueChanged != null)
      {
        RoutedPropertyChangedEventArgs<decimal> args = new RoutedPropertyChangedEventArgs<decimal>(oldValue, newValue);
        ValueChanged.Invoke(this, args);
      }
    }

    /// <summary>
    /// Zdarzenie zmiany wartości aktualnej
    /// </summary>
    public event RoutedPropertyChangedEventHandler<decimal> ValueChanged;


    #region ContentCulture property
    /// <summary>
    /// Kultura konwersji
    /// </summary>
    public CultureInfo ContentCulture
    {
      get { return (CultureInfo)GetValue(ContentCultureProperty); }
      set { SetValue(ContentCultureProperty, value); }
    }

    /// <summary>
    /// Właściwość zależna dla właściwości <see cref="ContentCulture"/>
    /// </summary>
    public static readonly DependencyProperty ContentCultureProperty = DependencyProperty.Register
      ("ContentCulture", typeof(CultureInfo), typeof(NumericEditBox),
         new PropertyMetadata(CultureInfo.InvariantCulture, ContentCulturePropertyChanged));

    private static void ContentCulturePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var c = sender as NumericEditBox;
      var numericValueConverter = (NumericValueConverter)c.FindResource("NumericValueConverter");
      numericValueConverter.Culture = c.ContentCulture;
    }
    #endregion

    #region ContentStringFormat property
    /// <summary>
    /// Kultura konwersji
    /// </summary>
    public new string ContentStringFormat
    {
      get { return (string)GetValue(ContentStringFormatProperty); }
      set { SetValue(ContentStringFormatProperty, value); }
    }

    /// <summary>
    /// Właściwość zależna dla właściwości <see cref="ContentStringFormat"/>
    /// </summary>
    public new static readonly DependencyProperty ContentStringFormatProperty = DependencyProperty.Register
      ("ContentStringFormat", typeof(string), typeof(NumericEditBox),
         new PropertyMetadata(null, ContentStringFormatPropertyChanged));

    private static void ContentStringFormatPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var c = sender as NumericEditBox;
      var numericValueConverter = (NumericValueConverter)c.FindResource("NumericValueConverter");
      numericValueConverter.Format = c.ContentStringFormat;
    }
    #endregion

    #region CharacterCasing property
    public CharacterCasing CharacterCasing
    {
      get => (CharacterCasing)GetValue(CharacterCasingProperty);
      set => SetValue(CharacterCasingProperty, value);
    }
    public static readonly DependencyProperty CharacterCasingProperty = DependencyProperty.Register
      ("CharacterCasing", typeof(CharacterCasing), typeof(NumericEditBox));
    #endregion

    #region MaxLength property
    [DefaultValue(0)]
    [Localizability(LocalizationCategory.None, Modifiability = Modifiability.Unmodifiable)]
    public int MaxLength
    {
      get => (int)GetValue(MaxLengthProperty);
      set => SetValue(MaxLengthProperty, value);
    }
    public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register
      ("MaxLength", typeof(int), typeof(NumericEditBox));
    #endregion

    #region TextAlignment property
    public TextAlignment TextAlignment
    {
      get => (TextAlignment)GetValue(TextAlignmentProperty);
      set => SetValue(TextAlignmentProperty, value);
    }
    public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register
      ("TextAlignment", typeof(TextAlignment), typeof(NumericEditBox),
        new FrameworkPropertyMetadata(TextAlignment.Right,
          FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    #endregion

    #region TextDecorations property
    public TextDecorationCollection TextDecorations
    {
      get => (TextDecorationCollection)GetValue(TextDecorationsProperty);
      set => SetValue(TextDecorationsProperty, value);
    }
    public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register
      ("TextDecorations", typeof(TextDecorationCollection), typeof(NumericEditBox));
    #endregion

  }
}
