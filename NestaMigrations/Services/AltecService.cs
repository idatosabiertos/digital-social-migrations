using CsvHelper;
using NestaMigrations.ModelCsv.Altec;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NestaMigrations.Services
{
    public class AltecService : IService
    {
        public static List<AltecFile> ImportData() {

            List<AltecFile> altecFileRows;

            using (TextReader reader = File.OpenText(@"C:\Users\jonaa\Repositorio\nesta-migrations\nesta-migrations\NestaMigrations\ModelCsv\altec.csv"))
            {
                var csv = new CsvReader(reader);
                altecFileRows = csv.GetRecords<AltecFile>().ToList();
                //var listPaises = records.GroupBy(x => x.OrgPais).Select(p => new { key = p.Key, count = p.Count() }).OrderByDescending(o => o.count).ToList();
                //var listOrganizaciones = records.GroupBy(x => x.OrgNombreOrganizacion).Select(p => new { key = p.Key, count = p.Count() }).OrderByDescending(o => o.count).ToList();
                //var lineaFinanciamiento = records.GroupBy(x => x.LineaFinanciamiento).Select(p => new { key = p.Key, count = p.Count() }).OrderByDescending(o => o.count).ToList
            }

            return altecFileRows;
        }
    }
}
