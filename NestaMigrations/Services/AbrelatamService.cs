using CsvHelper;
using NestaMigrations.ModelCsv.Abrelatam;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NestaMigrations.Services
{
    public class AbrelatamService : IService
    {
        private const string filePath = @"C:\Users\jonaa\Repositorio\nesta-migrations\nesta-migrations\NestaMigrations\ModelCsv\Abrelatam\Csv\";

        public static List<AbrelatamFile> ImportData()
        {


            List<AbrelatamFile> fileRows;

            using (TextReader reader = File.OpenText(filePath + "Abrelatam.csv"))
            {
                var csv = new CsvReader(reader);
                fileRows = csv.GetRecords<AbrelatamFile>().ToList();
            }

            return fileRows;
        }

        public static List<AbrelatamCitiesFile> ImportDataCities()
        {


            List<AbrelatamCitiesFile> fileRows;

            using (TextReader reader = File.OpenText(filePath + "AbrelatamCities.csv"))
            {
                var csv = new CsvReader(reader);
                fileRows = csv.GetRecords<AbrelatamCitiesFile>().ToList();
            }

            return fileRows;
        }

        public static List<AbrelatamRelationsFile> ImportDataRelations()
        {


            List<AbrelatamRelationsFile> fileRows;

            using (TextReader reader = File.OpenText(filePath + "AbrelatamRelations.csv"))
            {
                var csv = new CsvReader(reader);
                fileRows = csv.GetRecords<AbrelatamRelationsFile>().ToList();
            }

            return fileRows;
        }
    }
}
