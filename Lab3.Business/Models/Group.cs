using System;
using System.Collections.Generic;
using System.Text;

namespace Lab3.Business.Models
{
    public class Group : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public int ParticipantsCount { get; set; }
    }
}
