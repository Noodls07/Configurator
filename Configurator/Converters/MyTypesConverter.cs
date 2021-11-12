using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Configurator.ViewModel;

namespace Configurator
{
    static class MyTypesConverter
    {
        public static Tuple<string, int> ConvertToDB(TypeOfRv value)
        {
            switch (value)
            {
                case TypeOfRv.Integer:
                case TypeOfRv.Group:
                case TypeOfRv.SysNumIO:
                    return new Tuple<string, int>("INTEGER", 11 );
                case TypeOfRv.Text:
                    return new Tuple<string, int>("VARCHAR", 50);
                default:
                    return new Tuple<string, int>("INTEGER", 11);
            }
        }

        public static TypeOfRv GetEnumValue(string attr)
        {
            try
            {
                for (int i = 0; i < Enum.GetValues(typeof(TypeOfRv)).Length; i++)
                {
                    object aa = Enum.GetValues(typeof(TypeOfRv)).GetValue(i);
                    Type type = aa.GetType();
                    MemberInfo[] memInfo = type.GetMember(type.GetEnumName(aa));
                    DescriptionAttribute descriptionAttribute = memInfo[0]
                        .GetCustomAttributes(typeof(DescriptionAttribute), false)
                        .FirstOrDefault() as DescriptionAttribute;
                    if (descriptionAttribute != null && descriptionAttribute.Description == attr)
                        return (TypeOfRv)Enum.Parse(typeof(TypeOfRv), aa.ToString());
                }
            }
            catch (Exception)
            {
                return TypeOfRv.Integer;
            }
            return TypeOfRv.Integer;


        }

        public static string GetEnumDescription<T>(this T e) where T : IConvertible
        {
            try
            {
                if (e is Enum)
                {
                    Type type = e.GetType();
                    Array values = Enum.GetValues(type);

                    foreach (int val in values)
                    {
                        if (val == e.ToInt32(CultureInfo.InvariantCulture))
                        {
                            MemberInfo[] memInfo = type.GetMember(type.GetEnumName(val));
                            DescriptionAttribute descriptionAttribute = memInfo[0]
                                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                                .FirstOrDefault() as DescriptionAttribute;

                            if (descriptionAttribute != null)
                                return descriptionAttribute.Description;
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }


    }
}
