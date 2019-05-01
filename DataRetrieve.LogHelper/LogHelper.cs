using System;
using System.Collections.Generic;
using System.Text;

namespace DataRetrieve.LogHelper
{

    public interface ILog
    {
        void Trace(string Message);
        void Debug(string Message);
        void Info(string Message);
        void Warn(string Message);
        void Error(string Message);
        void Fatal(string Message);
    }
    public static class ILogHelper
    {
        public static ILog GetLog(string Log)
        {
            switch (Log)
            {
                case "Nlog":
                    return Nlog.getInstance();
                case "Log4Net":
                    return Log4Net.getInstance();
            }
            return Nlog.getInstance();
        }
    }
}
