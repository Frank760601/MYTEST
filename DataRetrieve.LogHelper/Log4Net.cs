using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataRetrieve.LogHelper
{
    public class Log4Net:ILog
    {
        private static Log4Net uniqueInstance;
        ILoggerRepository repository = log4net.LogManager.CreateRepository("NETCoreRepository");
        log4net.ILog log;
        public Log4Net()
        {
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
            log = log4net.LogManager.GetLogger(repository.Name, "NETCorelog4net");
        }
        public static Log4Net getInstance()
        {
            if (uniqueInstance == null) uniqueInstance = new Log4Net();
            return uniqueInstance;
        }
        public void Trace(string Message) {
            new NotImplementedException();
        }
        public void Debug(string Message)
        {
            log.Debug(Message);
        }
        public void Info(string Message)
        {
            log.Info(Message);
        }
        public void Warn(string Message)
        {
            log.Warn(Message);
        }
        public void Error(string Message)
        {
            log.Error(Message);
        }
        public void Fatal(string Message)
        {
            log.Fatal(Message);
        }
    }
}
