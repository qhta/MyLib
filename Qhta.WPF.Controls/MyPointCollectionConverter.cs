using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Qhta.WPF.Controls
{
  /// <summary>
  /// Konwerter kolekcji punktów
  /// </summary>
  public class MyPointCollectionConverter : IValueConverter
  {
    #region IValueConverter Members

    /// <summary>
    /// Konwersja wprost
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var regPtsColl = new PointCollection(); //regular points collection.
      var obsPtsColl = (ObservableCollection<Point>)value; //observable which is used to raise INCC event.
      foreach (var point in obsPtsColl)
        regPtsColl.Add(point);
      return regPtsColl;
    }

    /// <summary>
    /// Konwersja wstecz - niezaimplementowana
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return null;
    }

    #endregion
  }
}
