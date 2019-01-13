using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NestaMigrations.ModelCsv.CleanedDB.csv
{
    public class Merge6DecemberImpactTagsProjects
    {
        public string idUnico { get; set; }
        public string idInitiative { get; set; }
        public string nameInitiative { get; set; }
        public string nameOrg { get; set; }
        public string idOrg { get; set; }
        public string country { get; set; }
        public string idCountry { get; set; }
        public string city { get; set; }
        public string idCity { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string rangoEmpleados { get; set; }
        public string orgCount { get; set; }
        public string projCount { get; set; }

        public int GetOrgSize() {
            switch (this.rangoEmpleados)
            {
                case "0-5 personas":
                    return 0;
                case "6-10 personas":
                    return 1;
                case "11-25 personas":
                    return 2;
                case "26-50 personas":
                    return 3;
                case "51-100 personas":
                    return 4;
                case "101-500 personas":
                    return 5;
                case "501-1000 personas":
                    return 6;
                case "> 1000 personas":
                    return 7;
                default:
                    return 0;
            }
        }
    }
}
