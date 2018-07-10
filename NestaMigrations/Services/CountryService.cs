using NestaMigrations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NestaMigrations.Services
{
    public class CountryService : IService
    {
        NestaContext context;

        public CountryService(NestaContext context) {
            this.context = context;
        }

        public Country SearchCountry(string countryName) {
            var country = this.context.Countries.Where(c => c.name == countryName).FirstOrDefault();
            return country;
        }

        public CountryRegion SearchRegion(string regionName)
        {
            var region = this.context.CountryRegions.Where(c => c.name == regionName).FirstOrDefault();
            return region;
        }
    }
}
