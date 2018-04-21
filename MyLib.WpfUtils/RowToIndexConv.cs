using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MyLib.WpfUtils
{
  public class RowToIndexConv : IValueConverter
  {

    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      DataGridRow row = value as DataGridRow;
      //if (row.DataContext is INumbered numberedObject)
      //{
      //  if (numberedObject.Number==0)
      //    numberedObject.Number = row.GetIndex() + 1;
      //  return numberedObject.Number;
      //}
      return row.GetIndex() + 1;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
