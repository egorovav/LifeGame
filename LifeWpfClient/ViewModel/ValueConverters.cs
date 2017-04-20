using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LifeWpfClient.ViewModel
{
    public class GameTypeIntValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            GameType _gameType = (GameType)value;
            return (int)_gameType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int _index = (int)value;
            return (GameType)_index;
        }
    }
}
