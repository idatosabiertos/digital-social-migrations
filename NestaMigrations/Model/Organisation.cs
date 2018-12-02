using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NestaMigrations.Model
{
    public class Organisation
    {
        [Key]
        public int id { get; set; }
        public string address { get; set; }
        public int countryID { get; set; }
        public int countryRegionID { get; set; }
        public DateTime created { get; set; }
        public string description { get; set; }
        public string importID { get; set; }
        public bool isPublished { get; set; }
        public bool isWaitingApproval { get; set; }
        public string logo { get; set; }
        public string name { get; set; }
        public int organisationSizeID { get; set; }
        public int organisationTypeID { get; set; }
        public int ownerID { get; set; }
        public int partnersCount { get; set; }
        public int projectsCount { get; set; }
        public string shortDescription { get; set; }
        public DateTime startDate { get; set; }
        public string url { get; set; }
        public string headerImage { get; set; }
    }
}
