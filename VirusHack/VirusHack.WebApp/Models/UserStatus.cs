using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace VirusHack.WebApp.Models
{
    public enum UserStatus
    {
        Student = 1,
        Teacher = 2,
        Admin = 3
    }
}
