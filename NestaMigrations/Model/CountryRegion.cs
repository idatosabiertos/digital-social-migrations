using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NestaMigrations.Model
{
    public class CountryRegion
    {
        public int id { get; set; }
        public int countryID { get; set; }
        public string name { get; set; }
        public decimal lat { get; set; }
        public decimal lng { get; set; }
    }
}
