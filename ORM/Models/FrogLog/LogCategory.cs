using System;
using System.Runtime.Serialization;

namespace ORM.Models.FrogLog
{
    [DataContract]
    [Serializable]
    public enum LogCategory
    {
        [EnumMember] Debug = 1,
        [EnumMember] Performance = 2,
        [EnumMember] Info = 3,
        [EnumMember] Warning = 4,
        [EnumMember] Error = 5,
        [EnumMember] Croak = 6,
        [EnumMember] Audit = 7,
    }
}