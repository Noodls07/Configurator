using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Configurator
{
    //[ValueConversion(typeof(bool), typeof(string))]
    /// <summary>
    /// ConvertTo -> From bool to String/Int
    /// ConvertFrom -> From String/Int to bool
    /// </summary>
    class BooleanToStringConverter : BooleanConverter
    {
       private string[] yes_no = { "Нет", "Да" };
       private int[] yes_no_int = { 0, 1 };

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is bool @bool)
            {
                if (destinationType == typeof(string))
                {
                    if (!@bool) return yes_no[0];
                    if (@bool) return yes_no[1];
                }
                if (destinationType == typeof(int))
                {
                    if (!@bool) return yes_no_int[0];
                    if (@bool) return yes_no_int[1];
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is int @int)
            {
                if (yes_no_int[0] == @int) return false;
                if (yes_no_int[1] == @int) return true;
            }
            else if (value is string @string)
            {
                if (yes_no[0] == @string) return false;
                if (yes_no[1] == @string) return true;

            }
            return base.ConvertFrom(context, culture, value);
        }

    }
}
