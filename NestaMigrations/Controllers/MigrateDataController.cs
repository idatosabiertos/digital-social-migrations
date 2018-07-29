using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NestaMigrations.Model;
using NestaMigrations.ModelCsv.Abrelatam;
using NestaMigrations.Services;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace NestaMigrations.Controllers
{
    [Route("api/migrate/script/")]
    public class MigrateDataController : Controller
    {

        private readonly NestaContext _context;
        private readonly CountryService _countryService;
        private readonly OrganisationTypesService _organisationTypesService;

        public MigrateDataController(
            NestaContext context,
            CountryService countryService,
            OrganisationTypesService organisationTypesService)
        {
            _context = context;
            _countryService = countryService;
            _organisationTypesService = organisationTypesService;
        }

        #region altec
        [HttpGet("altec-organisations")]
        public int MigrateOrganisationsAltec()
        {
            var rows = AltecService.ImportData();
            var organisationsQuery = rows.GroupBy(x => x.OrgNombreOrganizacion).ToList();
            int i = 0;
            List<Organisation> organisations = new List<Organisation>();


            foreach (var queryItem in organisationsQuery)
            {
                int countryId, countryRegionId, organisationTypeID, projectsCount;
                var organisation = queryItem.First();
                var country = _countryService.SearchCountry(organisation.OrgPais);
                var region = _countryService.SearchRegion(organisation.OrgCiudad);


                countryId = country != null ? country.id : 0;
                countryRegionId = region != null ? region.id : 0;
                organisationTypeID = _organisationTypesService.GetOrganisationTypeIdFromName(organisation.OrgTipoOrganizacion);
                projectsCount = queryItem.Count();

                string url = organisation.OrgWeb;
                try
                {
                    url = new UriBuilder(organisation.OrgWeb).Uri.ToString();
                }
                catch (Exception)
                {
                }

                DateTime createDate = new DateTime();
                int createYear = DateTime.Now.Year;
                if (Int32.TryParse(organisation.OrgFechaCreacion, out createYear))
                    createDate = new DateTime(createYear, 1, 1);

                var org = new Organisation()
                {
                    address = $"{organisation.OrgCiudad}, {organisation.OrgPais}",
                    created = DateTime.UtcNow,
                    countryID = countryId,
                    countryRegionID = countryRegionId,
                    isPublished = false,
                    isWaitingApproval = true,
                    name = organisation.OrgNombreOrganizacion.Length > 250 ? organisation.OrgNombreOrganizacion.Substring(0, 250) : organisation.OrgNombreOrganizacion,
                    organisationTypeID = organisationTypeID,
                    projectsCount = projectsCount,
                    url = url,
                    description = "",
                    logo = "",
                    shortDescription = "",
                    headerImage = "",
                    importID = "",
                    ownerID = 0,
                    startDate = createDate,
                    organisationSizeID = OrganisationSizesId.GetIdFrom(organisation.EmpleadosRangoCantidad)
                };

                organisations.Add(org);

                this._context.Organisations.Add(org);

                i++;
            }

            this._context.SaveChanges();

            return i;
        }

        [HttpGet("altec-projects")]
        public int MigrateProjectsAltec()
        {
            var rows = AltecService.ImportData();
            var iniciativasQuery = rows.GroupBy(x => x.NombreIniciativa).ToList();
            int projectsCount = 0;

            List<Project> projects = new List<Project>();


            foreach (var queryItem in iniciativasQuery)
            {
                var projectCsv = queryItem.First();

                int countryId, countryRegionId;
                var country = _countryService.SearchCountry(projectCsv.OrgPais);
                var region = _countryService.SearchRegion(projectCsv.OrgCiudad);
                countryId = country != null ? country.id : 0;
                countryRegionId = region != null ? region.id : 0;

                var proj = new Project()
                {
                    created = DateTime.UtcNow,
                    countryID = countryId,
                    countryRegionID = countryRegionId,
                    isPublished = false,
                    isWaitingApproval = true,
                    name = projectCsv.NombreIniciativa.Length > 250 ? projectCsv.NombreIniciativa.Substring(0, 250) : projectCsv.NombreIniciativa,
                    url = "",
                    description = "",
                    logo = "",
                    shortDescription = "",
                    headerImage = "",
                    socialImpact = "",
                    organisationsCount = Math.Max(projectCsv.CantidadOrganizaciones, queryItem.Count()),
                    status = "closed",
                    importID = ""
                };

                projects.Add(proj);

                this._context.Projects.Add(proj);

                projectsCount++;
            }

            this._context.SaveChanges();

            return projectsCount;
        }

        [HttpGet("altec-projects-organisations")]
        public int MigrateProjectsOrganisationsAltec()
        {
            var rows = AltecService.ImportData();
            var projectsOrganisationsQuery = rows.Select(po => new { project = po.NombreIniciativa, organisation = po.OrgNombreOrganizacion}).ToList();
            int projectsOrganisationsCount = 0;

            List<OrganisationProject> organisationProjects = new List<OrganisationProject>();

            foreach (var opCsv in projectsOrganisationsQuery)
            {
                string orgName = opCsv.organisation.Length > 250 ? opCsv.organisation.Substring(0, 250) : opCsv.organisation;
                string proName = opCsv.project.Length > 250 ? opCsv.project.Substring(0, 250) : opCsv.project;


                var org = this._context.Organisations.Where(o => o.name == orgName).First();
                var pro = this._context.Projects.Where(o => o.name == opCsv.project).First();

                var op = new OrganisationProject()
                {
                    organisationID = org.id,
                    projectID = pro.id
                };

                bool existDB = this._context.OrganisationProjects.Where(x => x.organisationID == op.organisationID && x.projectID == op.projectID).Any();
                bool existCsv = organisationProjects.Where(x => x.organisationID == op.organisationID && x.projectID == op.projectID).Any();

                if (!existCsv && !existDB) {
                    this._context.OrganisationProjects.Add(op);
                    organisationProjects.Add(op);
                    projectsOrganisationsCount++;
                }
            }

            this._context.SaveChanges();

            return projectsOrganisationsCount;
        }

        [HttpGet("altec-users")]
        public int MigrateUsersAltec() {
            var rows = AltecService.ImportData();
            var organisationsQuery = rows.GroupBy(x => x.OrgNombreOrganizacion).ToList();
            int i = 0;

            List<Users> users = new List<Users>();

            foreach (var queryItem in organisationsQuery)
            {
                var organisation = queryItem.First();

                var user = new Users()
                {
                    email = organisation.Mail,
                    company = this.shortOrganization(organisation.OrgNombreOrganizacion),
                    fname = organisation.UserFullName,
                    bio = "",
                    isAdmin = false,
                    isDisabled = false,
                    isSuperAdmin = false,
                    cityName = "",
                    countryName = "",
                    facebookUID = "",
                    googleUID = "",
                    jobTitle = "",
                    lname = "",
                    location = "",
                    profilePic = "",
                    profileURL = "",
                    role = UserRoles.User,
                    showEmail = false,
                    twitterUID ="",
                    password = "",
                    gitHubUID = ""
                };

                this._context.Users.Add(user);

                i++;
            }

            this._context.SaveChanges();

            return i;
        }

        [HttpGet("altec-organisation-users")]
        public int MigrateUsersOrganisationsAltec()
        {
            var rows = AltecService.ImportData();
            var organisationsQuery = rows.GroupBy(x => x.OrgNombreOrganizacion).ToList();
            int i = 0;

            List<Users> users = new List<Users>();

            foreach (var queryItem in organisationsQuery)
            {
                var orgItem = queryItem.First();
                var userId = this._context.Users.Where(x => x.email == orgItem.Mail).First().id;
                var orgName = this.shortOrganization(orgItem.OrgNombreOrganizacion);

                var organisationDB = this._context.Organisations.Where(x => x.name == orgName).First();
                organisationDB.ownerID = userId;
                
                i++;
            }

            this._context.SaveChanges();

            return i;
        }

        [HttpGet("altec-organisation-tags")]
        public int MigrateOrganisationTagsAltec()
        {
            var rows = AltecService.ImportData();
            var organisationsQuery = rows.GroupBy(x => this.shortOrganization(this.RemoveDiacritics(x.OrgNombreOrganizacion.Trim().ToUpper()))).Distinct().ToList();
            int i = 0;

            List<Users> users = new List<Users>();

            foreach (var queryItem in organisationsQuery)
            {
                var orgItem = queryItem.First();

                var orgId = this._context.Organisations.Where(x => x.name == this.shortOrganization(orgItem.OrgNombreOrganizacion)).First().id;
                var tags = OrganisationTags.GetImpactTagsId(orgItem).Distinct().ToList();
                foreach (var tag in tags)
                {
                    this._context.OrganisationTags.Add(new OrganisationTag() { organisationId = orgId, tagId = tag });
                }

                i++;
            }

            this._context.SaveChanges();

            return i;
        }
        #endregion

        #region abrelatam
        [HttpGet("abrelatam-organisations")]
        public int MigrateOrganisationsAbrelatam()
        {
            var rows = AbrelatamService.ImportData();
            var organisationsQuery = rows.Where(r => r.Tipo1 == AbrelatamTypes.Organization).ToList();
            int i = 0;
            List<Organisation> organisations = new List<Organisation>();


            foreach (var organisation in organisationsQuery)
            {
                int countryId, countryRegionId, organisationTypeID, projectsCount;
                var country = _countryService.SearchCountry(organisation.Pais);
                var region = _countryService.SearchRegion(organisation.Ciudad);


                countryId = country != null ? country.id : 0;
                countryRegionId = region != null ? region.id : 0;
                //organisationTypeID = _organisationTypesService.GetOrganisationTypeIdFromName(organisation.OrgTipoOrganizacion);
                //projectsCount = queryItem.Count();

                string url = organisation.Website;
                try
                {
                    url = new UriBuilder(organisation.Website).Uri.ToString();
                }
                catch (Exception)
                {
                }

                var org = new Organisation()
                {
                    address = $"{organisation.Ciudad}, {organisation.Pais}",
                    created = DateTime.UtcNow,
                    countryID = countryId,
                    countryRegionID = countryRegionId,
                    isPublished = false,
                    isWaitingApproval = true,
                    name = organisation.Nombre.Length > 250 ? organisation.Nombre.Substring(0, 250) : organisation.Nombre,
                    //organisationTypeID = organisationTypeID,
                    //projectsCount = projectsCount,
                    url = url,
                    description = "",
                    logo = "",
                    shortDescription = "",
                    headerImage = "",
                    importID = "",
                    ownerID = 0
                };

                organisations.Add(org);

                this._context.Organisations.Add(org);

                i++;
            }

            this._context.SaveChanges();

            return i;
        }

        [HttpGet("abrelatam-projects")]
        public int MigrateProjectsAbrelatam()
        {
            var rows = AbrelatamService.ImportData();
            var iniciativasQuery = rows.Where(r => r.Tipo1 == AbrelatamTypes.Project).ToList();
            int projectsCount = 0;

            List<Project> projects = new List<Project>();


            foreach (var projectCsv in iniciativasQuery)
            {
                int countryId, countryRegionId;
                var country = _countryService.SearchCountry(projectCsv.Pais);
                var region = _countryService.SearchRegion(projectCsv.Ciudad);
                countryId = country != null ? country.id : 0;
                countryRegionId = region != null ? region.id : 0;

                var proj = new Project()
                {
                    created = DateTime.UtcNow,
                    countryID = countryId,
                    countryRegionID = countryRegionId,
                    isPublished = false,
                    isWaitingApproval = true,
                    name = projectCsv.Nombre.Length > 250 ? projectCsv.Nombre.Substring(0, 250) : projectCsv.Nombre,
                    url = "",
                    description = "",
                    logo = "",
                    shortDescription = "",
                    headerImage = "",
                    socialImpact = "",
                    //organisationsCount = Math.Max(projectCsv.CantidadOrganizaciones, queryItem.Count()),
                    status = "closed",
                    importID = ""
                };

                projects.Add(proj);

                this._context.Projects.Add(proj);

                projectsCount++;
            }

            this._context.SaveChanges();

            return projectsCount;
        }

        [HttpGet("abrelatam-projects-organisations")]
        public int MigrateProjectsOrganisationsAbrelatam()
        {
            var rows = AbrelatamService.ImportDataRelations();
            var projectsOrganisationsQuery = rows.Select(po => new { project = po.Origen, organisation = po.Destino }).ToList();
            int projectsOrganisationsCount = 0;

            List<OrganisationProject> organisationProjects = new List<OrganisationProject>();

            foreach (var opCsv in projectsOrganisationsQuery)
            {
                string orgName = opCsv.organisation.Length > 250 ? opCsv.organisation.Substring(0, 250) : opCsv.organisation;
                string proName = opCsv.project.Length > 250 ? opCsv.project.Substring(0, 250) : opCsv.project;


                var org = this._context.Organisations.Where(o => o.name == orgName).FirstOrDefault();
                var pro = this._context.Projects.Where(o => o.name == opCsv.project).FirstOrDefault();

                if (org == null || pro == null) {
                    Console.WriteLine($"Warning {orgName} {proName}");
                    continue;
                }

                var op = new OrganisationProject()
                {
                    organisationID = org.id,
                    projectID = pro.id
                };

                bool existDB = this._context.OrganisationProjects.Where(x => x.organisationID == op.organisationID && x.projectID == op.projectID).Any();
                bool existCsv = organisationProjects.Where(x => x.organisationID == op.organisationID && x.projectID == op.projectID).Any();

                if (!existCsv && !existDB)
                {
                    this._context.OrganisationProjects.Add(op);
                    organisationProjects.Add(op);
                    projectsOrganisationsCount++;
                }
            }

            this._context.SaveChanges();

            return projectsOrganisationsCount;
        }
        #endregion

        #region generic
        public void UpdateCities() {

        }
        #endregion


        public string shortOrganization(string organisation) {
            return  organisation.Length > 250 ? organisation.Substring(0, 250) : organisation;

        }

        string RemoveDiacritics(string text)
        {
            string formD = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char ch in formD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
