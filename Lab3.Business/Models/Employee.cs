using System;
using System.Collections.Generic;
using System.Text;

namespace Lab3.Business.Models
{
    public class Employee : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Guid? SpecializationId { get; set; }
        public string SpecalizationName { get; set; }
        public Guid? GroupId { get; set; }
        public string GroupName { get; set; }
    }
}
