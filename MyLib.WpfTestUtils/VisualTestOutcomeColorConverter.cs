using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MyLib.WpfTestUtils
{
  public class VisualTestOutcomeColorConverter: IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      switch ((UnitTestOutcome)value)
      {
        case UnitTestOutcome.InProgress:
          return Brushes.Blue;
        case UnitTestOutcome.Passed:
          return Brushes.Green;
        case UnitTestOutcome.Failed:
        case UnitTestOutcome.Error:
          return Brushes.Red;
        case UnitTestOutcome.Inconclusive:
          return Brushes.Magenta;
        case UnitTestOutcome.Timeout:
        case UnitTestOutcome.Aborted:
          return Brushes.Maroon;
      }
      return Brushes.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException("VisualTestOutcomeColorConverter.ConvertBack not implemented");
    }
  }
}
