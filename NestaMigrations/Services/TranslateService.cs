using CsvHelper;
using NestaMigrations.ModelCsv.Translate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NestaMigrations.Services
{
    public class TranslateService
    {
        private const string filePath = @"C:\Users\jonaa\Repositorio\digital-social-migrations\NestaMigrations\ModelCsv\Translate\";

        public static List<Translate> ImportData()
        {
            List<Translate> fileRows;

            using (TextReader reader = File.OpenText(filePath + "translate-pt.csv"))
            {
                var csv = new CsvReader(reader);
                fileRows = csv.GetRecords<Translate>().ToList();
            }

            return fileRows;
        }
    }
}
