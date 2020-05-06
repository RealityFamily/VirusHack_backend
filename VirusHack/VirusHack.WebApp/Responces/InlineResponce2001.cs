using System;
using System.Runtime.Serialization;

namespace VirusHack.WebApp.Responces
{
    [DataContract]
    public class InlineResponse2001
    {
        [DataMember(Name = "link")]
        public string Link { get; set; }
    }
}
