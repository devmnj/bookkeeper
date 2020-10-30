using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace accounts
{
    class StringDateCnverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)

        {

            if (value is string) { return string.Format("{0:dd/mm/yyyy}", value); }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
