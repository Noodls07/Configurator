using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator
{
    public enum StateTypes
    {
        /// <summary>
        /// Reading from DB
        /// </summary>
        [Description("Синхронизировано")]
        Loading = 0,
        /// <summary>
        /// Readed from DB
        /// </summary>
        [Description("Синхронизировано")]
        Sync = 1,

        /// <summary>
        /// Changed by user
        /// </summary>
        [Description("Изменено")]
        Modify = 2,

        /// <summary>
        /// Added by user
        /// </summary>
        [Description("Добавлено")]
        Added = 3,
        /// <summary>
        /// Deleted by user
        /// </summary>
        [Description("Удалено")]
        Deleted = 4,
    }
}
