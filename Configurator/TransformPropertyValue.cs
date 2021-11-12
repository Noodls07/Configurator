using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Configurator
{
    public static class TransformPropertyValue
    {
        public static string ConvertToData(this PropertyInfo propertyInfo, object dataObject)
        {
            BooleanToStringConverter bc = new BooleanToStringConverter();

            switch (propertyInfo.Name)
            {
                case "Many":
                    return (string)bc.ConvertTo(propertyInfo.GetValue(dataObject), typeof(string));
                case "Type":
                    return MyTypesConverter.GetEnumDescription((TypeOfRv)Convert.ToInt32(propertyInfo.GetValue(dataObject)));
                default:
                    return (string)propertyInfo.GetValue(dataObject);
            }
        }
    }
}
