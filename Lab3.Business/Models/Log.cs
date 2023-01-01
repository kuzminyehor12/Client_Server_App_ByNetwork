using Lab3.Business.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lab3.Business.Models
{
    public class Log : BaseEntity
    {
        public string CommandText { get; set; }
        public string CommandResult { get; set; }
        public DateTime CommandDate { get; set; }
    }
}
