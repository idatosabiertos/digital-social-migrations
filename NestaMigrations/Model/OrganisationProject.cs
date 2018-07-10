using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NestaMigrations.Model
{
    public class OrganisationProject
    {
        public int id { get; set; }
        public int organisationID { get; set; }
        public int projectID { get; set; }

        /*
         ALTER TABLE `nesta`.`organisation-projects` 
ADD COLUMN `id` INT(10) NOT NULL AUTO_INCREMENT AFTER `projectID`,
ADD PRIMARY KEY (`id`);
         */
    }
}
