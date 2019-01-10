using CsvHelper;
using NestaMigrations.ModelCsv.Abrelatam;
using NestaMigrations.ModelCsv.CleanedDB.csv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NestaMigrations.Services
{
    public class DataFixedService : IService
    {
        private const string FILE_PROJECTS = @"C:\Users\jonaa\Repositorio\digital-social-migrations\NestaMigrations\ModelCsv\CleanedDB\csv\merge dec 6- impact tags-projects.csv";

        public static List<Merge6DecemberImpactTagsProjects> ImportProjects()
        {


            List<Merge6DecemberImpactTagsProjects> fileRows;

            using (TextReader reader = File.OpenText(FILE_PROJECTS))
            {
                var csv = new CsvReader(reader, new CsvHelper.Configuration.Configuration() { Encoding = Encoding.UTF8});
                fileRows = csv.GetRecords<Merge6DecemberImpactTagsProjects>().ToList();
            }


            return fileRows;
        }
    }
}
