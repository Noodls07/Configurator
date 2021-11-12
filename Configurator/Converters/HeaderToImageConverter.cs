using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;


namespace Configurator
{
    [ValueConversion(typeof(string),typeof(BitmapImage))]
    public class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string image = "Images/icons-Obj.png";//img by def
            //var aaa = Properties.Resources.icons_Obj;
            if (value == null)
            {
                image = "Images/icons-Obj.png";
                return new BitmapImage(new Uri($"pack://application:,,,/{image}"));
            }

            string val = System.Convert.ToString(value);

            if (val.Contains('|'))
            {
                value = System.Convert.ToInt32(val.Remove(0, val.IndexOf('|') + 1));
            }

            //value += (value as string).Remove(0, (value as string).IndexOf('|'));

            switch ((TypeOfRv)value)
            {
                case TypeOfRv.Integer:
                    image = "Images/icons-Int.png";
                    break;
                case TypeOfRv.Text:
                    image = "Images/icons-Text.png";
                    break;
                case TypeOfRv.SysNumIO:
                    image = "Images/icons-SysNum.png";
                    break;
                case TypeOfRv.Group:
                    image = "Images/icons-Group.png";
                    break;
                default:
                    image = "Images/icons-Unknown.png";
                    break;
            }
            return new BitmapImage( new Uri($"pack://application:,,,/{image}"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
