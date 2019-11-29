using ORM.Models;
using ORM.Models.FrogLog;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ORM.LogProvider
{
    public class SqlHelper
    {
        private const string _sp="sp_log";
        private string ConnectionString { get; set; }

        public SqlHelper(string connectionStr)
        {
            ConnectionString = connectionStr;
        }

        private bool ExecuteNonQuery(string commandStr, List<SqlParameter> paramList)
        {
            bool result = false;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }

                using (SqlCommand command = new SqlCommand(commandStr, conn))
                {
                    command.Parameters.AddRange(paramList.ToArray());
                    int count = command.ExecuteNonQuery();
                    result = count > 0;
                }
            }
            return result;
        }

        public bool InsertLog(LogEntry entry)
        {
            var command = $@"INSERT INTO [dbo].[LoggerFactoryLog] ([ApplicationID],[ApplicationName],[LogCategory], [CreatedDateTime], [RunGroupName],  [ExceptionMessage], [StackTrace],  [Method],  [Text],  [AdditionalData],  [UserID],  [ComputerID],  [Environment],  [ProgramName], [KeyValue]) VALUES (@ApplicationID, @ApplicationName, @LogCategory, @CreatedDateTime, @RunGroupName, @ExceptionMessage, @StackTrace, @Method, @Text, @AdditionalData, @UserID, @ComputerID, @Environment, @ProgramName, @KeyValue)";
            var paramList = new List<SqlParameter>
            {
                
                new SqlParameter("ApplicationID", entry.ApplicationID),
                new SqlParameter("ApplicationName", entry.ApplicationName),
                new SqlParameter("LogCategory", entry.LogCategory),
                new SqlParameter("CreatedDateTime", entry.CreatedDateTime),
                new SqlParameter("RunGroupName", entry.RunGroupName),
                new SqlParameter("ExceptionMessage", entry.ExceptionMessage),
                new SqlParameter("StackTrace", entry.StackTrace),
                new SqlParameter("Method", entry.Method),
                new SqlParameter("Text", entry.Text),
                new SqlParameter("AdditionalData", entry.AdditionalData),
                new SqlParameter("UserID", entry.UserID),
                new SqlParameter("ComputerID", entry.ComputerID),
                new SqlParameter("Environment", entry.Environment),
                new SqlParameter("ProgramName", entry.ProgramName),
                new SqlParameter("KeyValue", entry.KeyValue)        
            };

            return ExecuteNonQuery(command, paramList);

            //    try
            //    {
            //        using (SqlConnection connection = new SqlConnection(ConnectionString))
            //        {
            //            if (connection.State == ConnectionState.Broken)
            //                connection.Close();
            //            if (connection.State == ConnectionState.Closed)
            //                connection.Open();
            //            using (var sqlCommand = new SqlCommand(_sp, connection))
            //            {
            //                sqlCommand.CommandType = CommandType.StoredProcedure;
            //                sqlCommand.Parameters.AddWithValue("@ApplicationID", entry.ApplicationID);
            //                sqlCommand.Parameters.AddWithValue("@ApplicationName", entry.ApplicationName);
            //                sqlCommand.Parameters.AddWithValue("@CategoryID", entry.CategoryID);
            //                sqlCommand.Parameters.AddWithValue("@CreatedDateTime", entry.CreatedDateTime);
            //                sqlCommand.Parameters.AddWithValue("@RunGroupName", entry.RunGroupName);
            //                sqlCommand.Parameters.AddWithValue("@StackTrace", entry.StackTrace);
            //                sqlCommand.Parameters.AddWithValue("@Method", entry.Method);
            //                sqlCommand.Parameters.AddWithValue("@Text", entry.Text);
            //                sqlCommand.Parameters.AddWithValue("@AdditionalData", entry.AdditionalData);
            //                sqlCommand.Parameters.AddWithValue("@UserID", entry.UserID);
            //                sqlCommand.Parameters.AddWithValue("@ComputerID", entry.ComputerID);
            //                sqlCommand.Parameters.AddWithValue("@Environment", entry.Environment);
            //                sqlCommand.Parameters.AddWithValue("@ProgramName", entry.ProgramName);
            //                sqlCommand.Parameters.AddWithValue("@KeyValue", entry.KeyValue);
            //                sqlCommand.Parameters.AddWithValue("@ExceptionMessage", entry.ExceptionMessage);
            //                sqlCommand.ExecuteNonQuery();
            //            }
            //        }
            //        return new LogEntryInsertionResult()
            //        {
            //            ResultType = LogEntryResultType.Success
            //        };
            //    }
            //    catch (Exception ex)
            //    {
            //        return new LogEntryInsertionResult()
            //        {
            //            ResultType = LogEntryResultType.Failure,
            //            Message = ex.Message
            //        };
            //    }
        }

    }
}