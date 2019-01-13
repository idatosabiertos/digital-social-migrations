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
        private const string FILE_TAG = @"C:\Users\jonaa\Repositorio\digital-social-migrations\NestaMigrations\ModelCsv\CleanedDB\csv\merge dec 6- impact tags-impact-tags.csv";
        private const string PROJECT_IMPACT_TAGS = @"C:\Users\jonaa\Repositorio\digital-social-migrations\NestaMigrations\ModelCsv\CleanedDB\csv\MergeProjectImpactTags.csv";
        private const string ORGS_TYPE = @"C:\Users\jonaa\Repositorio\digital-social-migrations\NestaMigrations\ModelCsv\CleanedDB\csv\merge dec 6- impact tags-orgs type.csv";

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

        public static List<MergeDec6ImpactTagsTags> ImportTags()
        {


            List<MergeDec6ImpactTagsTags> fileRows;

            using (TextReader reader = File.OpenText(FILE_TAG))
            {
                var csv = new CsvReader(reader, new CsvHelper.Configuration.Configuration() { Encoding = Encoding.UTF8 });
                fileRows = csv.GetRecords<MergeDec6ImpactTagsTags>().ToList();
            }


            return fileRows;
        }

        public static List<MergeDec6ProjectTags> ImportProjectTags()
        {


            List<MergeDec6ProjectTags> fileRows;

            using (TextReader reader = File.OpenText(PROJECT_IMPACT_TAGS))
            {
                var csv = new CsvReader(reader, new CsvHelper.Configuration.Configuration() { Encoding = Encoding.UTF8 });
                fileRows = csv.GetRecords<MergeDec6ProjectTags>().ToList();
            }


            return fileRows;
        }

        public static List<MergeDec6ImpactTagsOrgType> ImportOrgTypes()
        {


            List<MergeDec6ImpactTagsOrgType> fileRows;

            using (TextReader reader = File.OpenText(ORGS_TYPE))
            {
                var csv = new CsvReader(reader, new CsvHelper.Configuration.Configuration() { Encoding = Encoding.UTF8 });
                fileRows = csv.GetRecords<MergeDec6ImpactTagsOrgType>().ToList();
            }


            return fileRows;
        }

    }
}
