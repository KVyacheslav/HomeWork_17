using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemBank
{
    /// <summary>
    /// Класс, реализующий логику лога.
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Сообщение лога.
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// Создаем лог.
        /// </summary>
        /// <param name="msg">Сообщение.</param>
        public Log(string msg)
        {
            this.Msg = msg;
        }
    }
}
