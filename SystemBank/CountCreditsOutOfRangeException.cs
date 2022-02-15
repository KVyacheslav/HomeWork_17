using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemBank
{
    public class CountCreditsOutOfRangeException : Exception
    {
        public new readonly string Message;

        public CountCreditsOutOfRangeException()
        {
            this.Message = "Ошибка! Количество банковских кредитов не может быть более 1.";
        }
    }
}
