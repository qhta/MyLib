//StringtoImageConverter class added by the syncfusion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Qhta.Syncfusion.WPF.Utils.Converters
{
    public class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string str = value.ToString();

                if (str == "Sufficient")
                    return @"Assets/Sufficient.png";
                else if (str == "Insufficient")
                    return @"Assets/Insufficient.png";
                else if (str == "Perfect")
                    return @"Assets/Perfect.png";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
