
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace VirusHack.WebApp.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LessonStatus
    {
        [EnumMember(Value = "future")]
        Future = 1,

        [EnumMember(Value = "present")]
        Present = 2,

        [EnumMember(Value = "missed")]
        Missed = 3
    }
}
