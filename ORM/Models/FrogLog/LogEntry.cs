using System;
using System.Runtime.Serialization;
using ORM.Domain;

namespace ORM.Models.FrogLog
{
    [DataContract]
    [Serializable]
    public class LogEntry
    { 
        [DataMember]
        public int? ApplicationID { get; set; }

        [DataMember]
        public string ApplicationName { get; set; }

        [DataMember]
        public string Environment { get; set; }

        [DataMember]
        public DateTime CreatedDateTime { get; set; }

        [DataMember]
        public string LogCategory { get; set; }
       
        [DataMember]
        public string RunGroupName { get; set; }

        [DataMember]
        public string KeyValue { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string AdditionalData { get; set; }

        [DataMember]
        public string Method { get; set; }

        [DataMember]
        public string UserID { get; set; }

        [DataMember]
        public string ComputerID { get; set; }

        [DataMember]
        public string ProgramName { get; set; }

        [DataMember]
        public string ExceptionMessage { get; set; }

        [DataMember]
        public string StackTrace { get; set; }
    }
}