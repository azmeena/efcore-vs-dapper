using System;
using System.Runtime.Serialization;

namespace ORM.Models.FrogLog
{
    [DataContract]
    [Serializable]
    public class LogEntryInsertionResult
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public LogEntryResultType ResultType { get; set; }
    }
}