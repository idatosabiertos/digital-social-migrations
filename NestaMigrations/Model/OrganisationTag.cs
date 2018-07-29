using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NestaMigrations.Model
{
    public class OrganisationTag
    {
        [Key]
        public int organisationTagId { get; set; }
        public int organisationId { get; set; }
        public int tagId { get; set; }
    }
}
