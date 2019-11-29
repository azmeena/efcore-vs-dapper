using Microsoft.Extensions.Logging;
using ORM.Models;
using ORM.Models.FrogLog;
using System;
using System.Collections.Generic;

namespace ORM.LogProvider
{
    public class DBLogger : ILogger
    {
        public static DBLogger Instance { get; private set; }
        private string _categoryName;
        private Func<string, LogLevel, bool> _filter;
        protected SqlHelper Helper;
        private int MessageMaxLength = 4000;
        public string RunGroupName { private get; set; }
        public string ApplicationName { get; set; }
        public string ProgramName { get; set; }
        public string SqlServer { get; set; }
        public string Environment { get; set; }
        public LogCategory MinimumLoggingLevel { get; set; }
        public bool IsInitialized { get; private set; }

        public DBLogger(string categoryName, Func<string, LogLevel, bool> filter, string connectionString)
        {
            _categoryName = categoryName;
            _filter = filter;
            Helper = new SqlHelper(connectionString);
            //TODO:Init pass args from config file
            Initialize("Logger Factory","Audit","Test","server");
            Instance = this;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }
            var message = formatter(state, exception);
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (exception != null)
            {
                message += "\n" + exception.ToString();
            }
            message = message.Length > MessageMaxLength ? message.Substring(0, MessageMaxLength) : message;
      
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (_filter == null || _filter(_categoryName, logLevel));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        #region Custom


        public void Initialize(string applicationName, string minimumLogCategory, string environment, string serverName)
        {
            var minLoggingLevel = LogCategory.Info;

            if (!string.IsNullOrEmpty(minimumLogCategory)) Enum.TryParse(minimumLogCategory, true, out minLoggingLevel);

            ApplicationName = applicationName;
            Environment = environment;
            MinimumLoggingLevel = minLoggingLevel;
            SqlServer = serverName;
            IsInitialized = true;

        }

        public void Log(LogCategory logCategory, string methodName, Exception exception = null, string message = null, string runGroup = null, string keyValue = null, IEnumerable<object> additionalData = null, string programName = null)
        {
            if (!IsInitialized) return;
            try
            {
                var entry = new LogEntry()
                {
                   
                    LogCategory = logCategory.ToString(),
                    Method = methodName,
                    ApplicationID = 193,
                    UserID = "azmeena.narsingani@bexar.org",
                    ComputerID = "D1NF99H2",
                    AdditionalData = additionalData == null ? "NULL" : additionalData.ToString(),
                    #endregion

                    CreatedDateTime = DateTime.Now,
                    ExceptionMessage = exception == null ? "NULL" : exception.Message,
                    StackTrace = exception == null ? "NULL" : exception.StackTrace,
                    RunGroupName = string.IsNullOrWhiteSpace(runGroup) ? this.RunGroupName : runGroup,
                    Text = message,
                    ApplicationName = ApplicationName,
                    Environment = this.Environment,
                    ProgramName = string.IsNullOrWhiteSpace(programName) ? this.ProgramName : programName,
                    KeyValue = keyValue
                };
                if (MinimumLoggingLevel >= logCategory)
                {
                    Helper.InsertLog(entry);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}