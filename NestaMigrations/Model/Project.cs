using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NestaMigrations.Model
{
    public class Project
    {
        public int id { get; set; }
        public int ownerID { get; set; }
        public string name { get; set; }
        public string shortDescription { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string status { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int countryID { get; set; }
        public int countryRegionID { get; set; }
        public int organisationsCount { get; set; }
        public string importID { get; set; }
        public DateTime created { get; set; }
        public DateTime lastUpdate { get; set; }
        public string logo { get; set; }
        public string headerImage { get; set; }
        public string socialImpact { get; set; }
        public bool isWaitingApproval { get; set; }
        public bool isPublished { get; set; }
    }
}
