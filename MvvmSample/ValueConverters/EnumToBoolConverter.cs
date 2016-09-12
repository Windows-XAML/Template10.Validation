using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MvvmSample.ValueConverters
{
    public class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var param = parameter as string;
            if (param == null)
                return DependencyProperty.UnsetValue;

            if (!Enum.IsDefined(value.GetType(), value))
                return DependencyProperty.UnsetValue;

            var paramValue = Enum.Parse(value.GetType(), param);
            return paramValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var param = parameter as string;
            if (param == null)
                return DependencyProperty.UnsetValue;

            return Enum.Parse(targetType, param);
        }
    }
}
