using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace VirusHack.WebApp.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LessonType
    {
        [EnumMember(Value = "practic")]
        Practic = 1,

        [EnumMember(Value = "lecture")]
        Lecture = 2,

        [EnumMember(Value = "lab")]
        Lab = 3
    }
}
