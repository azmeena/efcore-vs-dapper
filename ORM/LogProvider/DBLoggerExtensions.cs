using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ORM.Models.FrogLog;

namespace ORM.LogProvider
{
    public static class DBLoggerExtensions
    {
        public static ILoggerFactory AddContext(this ILoggerFactory factory,
            Func<string, LogLevel, bool> filter = null, string connectionStr = null)
        {
            factory.AddProvider(new DBLoggerProvider(filter, connectionStr));
            return factory;
        }

        public static ILoggerFactory AddContext(this ILoggerFactory factory, LogLevel minLevel, string connectionStr)
        {
            return AddContext(
                factory,
                (_, logLevel) => logLevel >= minLevel, connectionStr);
        }

        #region Custom
        public static void Debug(this ILogger<DBLogger> logger, string methodName, string message = null, string runGroup = null, string keyValue = null, IEnumerable<object> additionalData = null, string programName = null, Exception exception = null)
        {
            DBLogger.Instance?.Log(LogCategory.Debug, methodName, exception, message, runGroup, keyValue, additionalData, programName);
        }

        public static void Performance(this ILogger<DBLogger> logger, string methodName, Stopwatch curWatch, string message = null, string runGroup = null, string keyValue = null, string programName = null)
        {
            DBLogger.Instance?.Log(LogCategory.Performance, methodName, null, $"{message} Elapsed time: {curWatch.Elapsed:G}", runGroup, keyValue, null, programName);
        }

        public static void Performance(this ILogger<DBLogger> logger, string methodName, string elapsedTime, string message = null, string runGroup = null, string keyValue = null, string programName = null)
        {
            DBLogger.Instance?.Log(LogCategory.Performance, methodName, null, $"{message} Elapsed time: {elapsedTime}", runGroup, keyValue, null, programName);
        }

        public static void Error(this ILogger<DBLogger> logger, string methodName, string message = null, string runGroup = null, string keyValue = null, IEnumerable<object> additionalData = null, string programName = null, Exception exception = null)
        {
            DBLogger.Instance.Log(LogCategory.Error, methodName, exception, message, runGroup, keyValue, additionalData, programName);
        }

        public static void Audit(this ILogger<DBLogger> logger, string methodName, string message = null, string runGroup = null, string keyValue = null, IEnumerable<object> additionalData = null, string programName = null, Exception exception = null)
        {
            DBLogger.Instance?.Log(LogCategory.Audit, methodName, exception, message, runGroup, keyValue, additionalData, programName);
        }

        public static void Croak(this ILogger<DBLogger> logger, string methodName, string message = null, string runGroup = null, string keyValue = null, IEnumerable<object> additionalData = null, string programName = null, Exception exception = null)
        {
            DBLogger.Instance?.Log(LogCategory.Croak, methodName, exception, message, runGroup, keyValue, additionalData, programName);
        }

        public static void Info(this ILogger<DBLogger> logger, string methodName, string message = null, string runGroup = null, string keyValue = null, IEnumerable<object> additionalData = null, string programName = null, Exception exception = null)
        {
            DBLogger.Instance?.Log(LogCategory.Info, methodName, exception, message, runGroup, keyValue, additionalData, programName);
        }

        public static void Warning(this ILogger<DBLogger> logger, string methodName, string message = null, string runGroup = null, string keyValue = null, IEnumerable<object> additionalData = null, string programName = null, Exception exception = null)
        {
            DBLogger.Instance?.Log(LogCategory.Warning, methodName, exception, message, runGroup, keyValue, additionalData, programName);
        }

        public static void Initialize(this ILogger<DBLogger> logger, string applicationName, string minimumLogCategory, string environment, string serverName)
        {
            DBLogger.Instance?.Initialize(applicationName, minimumLogCategory, environment, serverName);
        }

        #endregion

    }
}