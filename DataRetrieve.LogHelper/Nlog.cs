using NLog;
using System;

namespace DataRetrieve.LogHelper
{
    public class Nlog : ILog
    {
        private static Nlog uniqueInstance;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        public static Nlog getInstance()
        {
            if (uniqueInstance == null) uniqueInstance = new Nlog();
            return uniqueInstance;
        }
        public  void Trace(string Message) {
            logger.Trace(Message);
        }
        public  void Debug(string Message) {
            logger.Debug(Message);
        }
        public  void Info(string Message)
        {
            logger.Info(Message);
        }
        public  void Warn(string Message)
        {
            logger.Warn(Message);
        }
        public  void Error(string Message)
        {
            logger.Error(Message);
        }
        public  void Fatal(string Message)
        {
            logger.Fatal(Message);
        }
    }
}
