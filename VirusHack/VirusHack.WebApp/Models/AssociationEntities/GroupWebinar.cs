using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirusHack.WebApp.Models.AssociationEntities
{
    public class GroupWebinar
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
        public Guid WebinatId { get; set; }
        public Webinar Webinar { get; set; }
    }
}
