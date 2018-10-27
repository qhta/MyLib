using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Qhta.WPF.Controls
{
  /// <summary>
  /// Kontrolka łącząca <c>CheckBox</c> i <c>TextBlock</c> potrzebna 
  /// do poprawnego zastosowania <c>CheckBoxa</c> w <c>ComboBoxie</c>.
  /// Bez niej kliknięcie na tekst w samym <c>CheckBoxie</c> powoduje 
  /// zaznaczenie/odznaczenie kratki, a nie wybór elementu w <c>ComboBoxie</c>.
  /// </summary>
  public partial class ComboBoxCheckItem : UserControl
  {
    /// <summary>
    /// Konstruktor inicjujący
    /// </summary>
    public ComboBoxCheckItem ()
    {
      InitializeComponent ();
      internalCheckBox.Click += new RoutedEventHandler (internalCheckBox_Click);
    }

    /// <summary>
    /// Przechwycone zdarzenie kliknięcia kratki
    /// </summary>
    void internalCheckBox_Click (object sender, RoutedEventArgs e)
    {
      OnClick (e);
    }

    /// <summary>
    /// Właściwość zależna dla właściwości <see cref="Text"/>
    /// </summary>
    public static DependencyProperty TextProperty = DependencyProperty.Register
      ("Text", typeof (string), typeof (ComboBoxCheckItem));


    /// <summary>
    /// Reprezentuje tekst wyświetlany obok kratki
    /// </summary>
    public string Text
    {
      get { return (string)GetValue (TextProperty); }
      set { SetValue (TextProperty, value); }
    }

    /// <summary>
    /// Właściwość zależna dla właściwości <see cref="IsChecked"/>
    /// </summary>
    public static DependencyProperty IsCheckedProperty = DependencyProperty.Register
      ("IsChecked", typeof (bool?), typeof (ComboBoxCheckItem), new PropertyMetadata(false));


    /// <summary>
    /// Reprezentuje stan kratki
    /// </summary>
    [Bindable (true)]
    [Category ("Behavior")]
    public bool? IsChecked
    {
      get { return (bool?)GetValue (IsCheckedProperty); }
      set { SetValue (IsCheckedProperty, value); }
    }

    /// <summary>
    /// Właściwość statyczna dla właściwości <see cref="IsThreeStatr"/>
    /// </summary>
    public static readonly DependencyProperty IsThreeStateProperty = DependencyProperty.Register
      ("IsThreeState", typeof (bool), typeof (ComboBoxCheckItem), new PropertyMetadata (false));

    /// <summary>
    /// Czy właściwość <see cref="IsChecked"/> może mieć wartość <c>null</c>?
    /// </summary>
    [Bindable (true)]
    [Category ("Behavior")]
    public bool IsThreeState 
    {
      get { return (bool)GetValue (IsThreeStateProperty); }
      set { SetValue (IsThreeStateProperty, value); }
    }

    /// <summary>
    /// Zdarzenie kliknięcia kratki
    /// </summary>
    public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
          "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ComboBoxCheckItem));

    public event RoutedEventHandler Click
    {
      add { AddHandler (ClickEvent, value); }
      remove { RemoveHandler (ClickEvent, value); }
    }

    protected virtual void OnClick (RoutedEventArgs e)
    {
      RoutedEventArgs newEventArgs = new RoutedEventArgs (ComboBoxCheckItem.ClickEvent);
      RaiseEvent (newEventArgs);
    }

  }

}
