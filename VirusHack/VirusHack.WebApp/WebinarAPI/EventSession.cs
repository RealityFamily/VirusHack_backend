using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirusHack.WebApp.WebinarAPI
{
    public class EventSession
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string StartsAt { get; set; }
        public string EndsAt { get; set; }
        public long Duration { get; set; }
        public long EventId { get; set; }
        public long QuestionCount { get; set; }
        public long ChatMessageCount { get; set; }
        public List<Connection> Connections { get; set; }
        public long UserChatMessageCount { get; set; }
        public long UserQuestionCount { get; set; }
        public List<object> AdditionalFieldValues { get; set; }
    }
}
