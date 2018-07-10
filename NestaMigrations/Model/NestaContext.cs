using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NestaMigrations.Model
{
    public class NestaContext : DbContext
    {
        public NestaContext(DbContextOptions<NestaContext> options) : base(options)
        {

        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<CountryRegion> CountryRegions { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<OrganisationType> OrganisationTypes { get; set; }
        public DbSet<OrganisationProject> OrganisationProjects { get; set; }
        public DbSet<Project> Projects { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Table names
            modelBuilder.Entity<Country>().ToTable("countries");
            modelBuilder.Entity<CountryRegion>().ToTable("country-regions");
            modelBuilder.Entity<Organisation>().ToTable("organisations");
            modelBuilder.Entity<OrganisationType>().ToTable("orgnisation-types");
            modelBuilder.Entity<Project>().ToTable("projects");
            modelBuilder.Entity<OrganisationProject>().ToTable("organisation-projects");
        }
    }
}
