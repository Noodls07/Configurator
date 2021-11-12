using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator
{
    public enum TypeOfRv
    {
        /// <summary>
        /// Ничего
        /// </summary>
        //None = 0,

        /// <summary>
        /// Целочисленные данные.
        /// </summary>
        [Description("Целое значение")]
        Integer = 1,

        /// <summary>
        /// Числа с точкой.
        /// </summary>
        //[Description("Число с плавающей точкой")]
        //Float = 2,

        /// <summary>
        /// Дата. Формат ГГГГММДД.
        /// </summary>
        //[Description("Дата (формат : ГГГГММДД)")]
        //Date = 3,

        /// <summary>
        /// Текст. До 255 символов.
        /// </summary>foat
        [Description("Текст")]
        Text = 4,


        /// <summary>
        /// Словарное значение.
        /// </summary>
        //[Description("Словарь")]
        //Dictionary = 8,

        /// <summary>
        /// 
        /// </summary>
        [Description("Системный номер")]
        SysNumIO = 11,

        /// <summary>
        /// Групповой реквизит.
        /// </summary>
        [Description("Группа")]
        Group = 96
    }
}
