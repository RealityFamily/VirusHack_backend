using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace VirusHack.WebApp.Responces
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class InlineResponse200
    {
        [DataMember(Name = "token")]
        public string Token { get; set; }
    }
}
