using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWManager

{
    public interface ILoggingService
    {
        void Log(string message);
        void Error(string message);
        void Warning(string message);
    }
}