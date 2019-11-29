using System;
using System.Runtime.Serialization;

namespace ORM.Models.FrogLog
{
    [DataContract]
    [Serializable]
    public enum LogEntryResultType
    {
        [EnumMember] Success,
        [EnumMember] Failure,
    }
}