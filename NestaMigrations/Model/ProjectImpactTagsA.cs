using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NestaMigrations.Model
{
    public class ProjectTags
    {
        [Key]
        public int id { get; set; }
        public int projectID { get; set; }
        public int tagID { get; set; }
    }
}
