using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NestaMigrations.Model;
using NestaMigrations.ModelCsv.Abrelatam;
using NestaMigrations.Services;
using Newtonsoft.Json;
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
            var projectsOrganisationsQuery = rows.Select(po => new { project = po.NombreIniciativa, organisation = po.OrgNombreOrganizacion }).ToList();
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

        [HttpGet("altec-users")]
        public int MigrateUsersAltec()
        {
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
                    twitterUID = "",
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

        [HttpGet("altec-countries")]
        public int MigrateCountriesAltec()
        {
            var rows = AltecService.ImportData();
            var countries = rows.Select(x => x.OrgPais).Distinct().ToList();
            int i = 0;

            foreach (var countryName in countries)
            {
                var country = this._context.Countries.Where(x => x.name == countryName).FirstOrDefault();
                if (country == null)
                {
                    this._context.Countries.Add(new Country()
                    {
                        name = countryName
                    });
                }
                else
                {
                    //
                }

                i++;
            }

            this._context.SaveChanges();

            return i;
        }

        [HttpGet("altec-country-regions")]
        public int MigrateCountryRegions()
        {
            var rows = AltecService.ImportData();
            var regions = rows.Select(x => new { country = x.OrgPais, region = x.OrgCiudad }).Distinct().ToList();
            int i = 0;

            foreach (var r in regions)
            {
                var country = this._context.Countries.Where(x => x.name == r.country).First();
                var region = this._context.CountryRegions.Where(x => x.name == r.region).FirstOrDefault();

                if (region == null)
                {
                    this._context.CountryRegions.Add(new CountryRegion()
                    {
                        countryID = country.id,
                        name = r.region
                    });
                }
                else
                {
                    region.countryID = country.id;
                }

                i++;
            }

            this._context.SaveChanges();

            return i;
        }
        #endregion

        #region abrelatam
        [HttpGet("abrelatam-users")]
        public int MigrateUsersAbrelatam()
        {
            var rows = AbrelatamService.ImportData();
            var users = rows.Select(u => new
            {
                organisation = u.Nombre,
                userName = u.PuntoContacto,
                mail = u.Mail
            }).Distinct().ToList();

            int i = 0;

            foreach (var u in users)
            {
                if (!String.IsNullOrEmpty(u.mail))
                {

                    var user = new Users()
                    {
                        email = u.mail,
                        company = this.shortOrganization(u.organisation),
                        fname = u.userName,
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
                        twitterUID = "",
                        password = "",
                        gitHubUID = ""
                    };

                    this._context.Users.Add(user);

                    i++;
                }
            }

            this._context.SaveChanges();

            return i;
        }

        [HttpGet("abrelatam-organisation-users")]
        public int MigrateUsersOrganisationsAbrelatam()
        {
            var rows = AbrelatamService.ImportData();
            var organisationsQuery = rows.Where(y => y.Tipo1 == AbrelatamTypes.Organization && !String.IsNullOrEmpty(y.Mail)).GroupBy(x => x.Nombre).ToList();
            int i = 0;

            List<Users> users = new List<Users>();

            foreach (var queryItem in organisationsQuery)
            {
                var orgItem = queryItem.First();
                var userId = this._context.Users.Where(x => x.email == orgItem.Mail).First().id;
                var orgName = this.shortOrganization(orgItem.Nombre);

                var organisationDB = this._context.Organisations.Where(x => x.name == orgName).First();
                organisationDB.ownerID = userId;

                i++;
            }

            this._context.SaveChanges();

            return i;
        }

        [HttpGet("abrelatam-organisations")]
        public int MigrateOrganisationsAbrelatam()
        {
            var rows = AbrelatamService.ImportData();
            var organisationsQuery = rows.Where(r => r.Tipo1 == AbrelatamTypes.Organization).GroupBy(o => o.Nombre).ToList();
            int i = 0;
            List<Organisation> organisations = new List<Organisation>();


            foreach (var item in organisationsQuery)
            {
                var organisation = item.First();
                int countryId, countryRegionId, organisationTypeID, projectsCount;
                var country = _countryService.SearchCountry(organisation.Pais);
                var region = _countryService.SearchRegion(organisation.Ciudad);


                countryId = country != null ? country.id : 0;
                countryRegionId = region != null ? region.id : 0;
                organisationTypeID = _organisationTypesService.GetOrganisationTypeIdFromName(organisation.Categoria);
                projectsCount = item.Count();

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
                    organisationTypeID = organisationTypeID,
                    projectsCount = projectsCount,
                    url = url,
                    description = organisation.Descripcion,
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

                if (org == null || pro == null)
                {
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

        [HttpGet("abrelatam-countries")]
        public int MigrateCountriesAbrelatam()
        {
            var rows = AbrelatamService.ImportDataCities();
            var countries = rows.Select(x => x.Pais).Distinct().ToList();
            int i = 0;

            foreach (var countryName in countries)
            {
                var country = this._context.Countries.Where(x => x.name == countryName).FirstOrDefault();
                if (country == null)
                {
                    this._context.Countries.Add(new Country()
                    {
                        name = countryName
                    });
                }
                else
                {
                    //
                }

                i++;
            }

            this._context.SaveChanges();

            return i;
        }

        [HttpGet("abrelatam-country-regions")]
        public int MigrateCountryRegionsAbrelatam()
        {
            var rows = AbrelatamService.ImportDataCities();
            var regions = rows.Select(x => new { country = x.Pais, region = x.Nombre }).Distinct().ToList();
            int i = 0;

            foreach (var r in regions)
            {
                var country = this._context.Countries.Where(x => x.name == r.country).First();
                var region = this._context.CountryRegions.Where(x => x.name == r.region).FirstOrDefault();

                if (region == null)
                {
                    this._context.CountryRegions.Add(new CountryRegion()
                    {
                        countryID = country.id,
                        name = r.region
                    });
                }
                else
                {
                    region.countryID = country.id;
                }

                i++;
            }

            this._context.SaveChanges();

            return i;
        }

        [HttpGet("abrelatam-organisation-tags")]
        public int MigrateOrganisationTagsAbrelatam()
        {
            var rows = AbrelatamService.ImportData();
            var organisationsQuery = rows.Where(o => o.Tipo1 == AbrelatamTypes.Organization).GroupBy(x => this.shortOrganization(this.RemoveDiacritics(x.Nombre.Trim().ToUpper()))).Distinct().ToList();
            int i = 0;

            List<Users> users = new List<Users>();

            foreach (var queryItem in organisationsQuery)
            {
                var orgItem = queryItem.First();

                var orgId = this._context.Organisations.Where(x => x.name == this.shortOrganization(orgItem.Nombre)).First().id;
                var tags = OrganisationTags.GetImpactTagsId(orgItem.Temas).Distinct().ToList();
                foreach (var tag in tags)
                {
                    if (!this._context.OrganisationTags.Where(x => x.organisationId == orgId && x.tagId == tag).Any())
                        this._context.OrganisationTags.Add(new OrganisationTag() { organisationId = orgId, tagId = tag });
                }

                i++;
            }

            this._context.SaveChanges();

            return i;
        }
        #endregion

        #region generic
        [HttpGet("update-cities")]
        public void UpdateCities()
        {

            var regions = this._context.CountryRegions.Where(x => x.lat == 0 || x.lng == 0);

            foreach (var region in regions)
            {
                var country = this._context.Countries.Where(x => x.id == region.countryID).First();
                var url = $"http://maps.google.com/maps/api/geocode/json?address={region.name},%20{country.name}&KEY=AIzaSyADNJOaF6I7UD2_OS5ZWqnLSIPDWuDJ44o&fields=address_component";

                string locRes = Get(url);
                dynamic obj = JsonConvert.DeserializeObject<Object>(locRes);
                var lat = Convert.ToDecimal(obj.results[0].geometry.location.lat);
                var lng = Convert.ToDecimal(obj.results[0].geometry.location.lng);

                region.lat = lat;
                region.lng = lng;
                System.Threading.Thread.Sleep(2000);
                Console.WriteLine($"{region.id} {region.name}, {country.name} {lat} {lng}");
                this._context.SaveChanges();
            }

        }


        public string shortOrganization(string organisation)
        {
            return organisation.Length > 250 ? organisation.Substring(0, 250) : organisation;

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

        public string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        [HttpGet("fix-regions")]
        public int FixCountryRegions()
        {
            int i = 0;

            var organisaciones = AbrelatamService.ImportData();
            foreach (var item in organisaciones)
            {
                var pais = item.Pais;
                var ciudad = item.Ciudad;

                var orgDB = this._context.Organisations.Where(x => x.name == item.Nombre && x.countryRegionID == 0).FirstOrDefault();
                if (orgDB != null)
                {
                    var cityDB = this._context.CountryRegions.Where(x => x.name == ciudad).First();

                    orgDB.countryID = cityDB.countryID;
                    orgDB.countryRegionID = cityDB.id;
                    i++;
                }



            }


            var organisacionesA = AltecService.ImportData();
            foreach (var item in organisacionesA)
            {
                var pais = item.OrgPais;
                var ciudad = item.OrgCiudad;

                var orgDB = this._context.Organisations.Where(x => x.name == item.OrgNombreOrganizacion && x.countryRegionID == 0).FirstOrDefault();
                if (orgDB != null)
                {
                    var cityDB = this._context.CountryRegions.Where(x => x.name == ciudad).First();

                    orgDB.countryID = cityDB.countryID;
                    orgDB.countryRegionID = cityDB.id;
                    i++;
                }



            }

            this._context.SaveChanges();

            return i;
        }



        #endregion


        [HttpGet("translate-pt")]
        public int TranslatePT()
        {
            int i = 0, u = 0, row = 1;
            List<string> notFoundIndexList = new List<string>();
            List<int> notFoundRowList = new List<int>();

            var translates = TranslateService.ImportData();
            foreach (var item in translates)
            {
                var index = item.index;
                var pt = item.pt;

                var itemToUpdate = this._context.Translates.Where(x => x.index == index).FirstOrDefault();
                if (itemToUpdate != null)
                {
                    itemToUpdate.de = pt;
                    i++;
                }
                else
                {
                    u++;
                    notFoundIndexList.Add(index);
                    notFoundRowList.Add(row);
                }

                row++;
            }

            this._context.SaveChanges();

            return i;
        }


        [HttpGet("data-fixed-countries")]
        public int DataFixedCountriesAbrelatam()
        {
            var rows = DataFixedService.ImportProjects();
            var countries = rows.ToList();
            int i = 0;

            Dictionary<int, Country> dic = new Dictionary<int, Country>();

            foreach (var country in countries)
            {

                int idCountry = 0;
                if (Int32.TryParse(country.idCountry, out idCountry))
                    dic.TryAdd(idCountry, new Country()
                    {
                        id = idCountry,
                        name = country.country
                    });
                else
                    Console.WriteLine($"COUNTRY WITHOUT ID {country.country}");

                i++;
            }

            this._context.Countries.AddRange(dic.Select(x => x.Value).ToList());
            this._context.SaveChanges();
            return i;
        }

        [HttpGet("data-fixed-cities")]
        public int DataFixedCitiesAbrelatam()
        {
            var rows = DataFixedService.ImportProjects();
            var cities = rows.ToList();
            int i = 0;

            Dictionary<int, CountryRegion> dic = new Dictionary<int, CountryRegion>();

            foreach (var r in rows)
            {
                try
                {
                    int idCountry = Convert.ToInt32(r.idCountry);
                    int idCity = Convert.ToInt32(r.idCity);
                    decimal latitude = Convert.ToDecimal(r.latitude);
                    decimal longitude = Convert.ToDecimal(r.longitude);

                    dic.TryAdd(idCity, new CountryRegion()
                    {
                        id = idCity,
                        countryID = idCountry,
                        lat = latitude,
                        lng = longitude,
                        name = r.city
                    });
                }
                catch (Exception)
                {
                    Console.WriteLine($"REGION ERROR {r.city}");
                }

                i++;
            }

            this._context.CountryRegions.AddRange(dic.Select(x => x.Value).ToList());
            this._context.SaveChanges();
            return i;
        }

        [HttpGet("data-fixed-projects")]
        public int DataFixedCitiesProjects()
        {
            var rows = DataFixedService.ImportProjects().ToList();

            int i = 0;

            Dictionary<int, Project> dic = new Dictionary<int, Project>();

            foreach (var r in rows)
            {
                try
                {
                    int idProject = Convert.ToInt32(r.idInitiative);
                    int idCountry = Convert.ToInt32(r.idCountry);
                    int idCity = Convert.ToInt32(r.idCity);

                    int count = 0;
                    Int32.TryParse(r.orgCount, out count);

                    var x = dic.TryAdd(idProject, new Project()
                    {
                        countryID = idCountry,
                        countryRegionID = idCity,
                        importID = idProject.ToString(),
                        description = r.nameInitiative,
                        id = idProject,
                        isPublished = true,
                        lastUpdate = DateTime.Now,
                        isWaitingApproval = false,
                        name = r.nameInitiative,
                        shortDescription = r.nameInitiative,
                        ownerID = 1,
                        organisationsCount = count,
                        status = "closed",
                        created = DateTime.Now,
                        headerImage = "",
                        logo= "",
                        socialImpact = "",
                        url = ""
                    });

                    if(!x)
                        Console.WriteLine(idProject);
                }
                catch (Exception)
                {
                    Console.WriteLine($"PROJECT ERROR {r.nameInitiative}");
                }

                i++;
            }

            this._context.Projects.AddRange(dic.Select(x => x.Value).ToList());
            this._context.SaveChanges();
            return i;
        }
    }
}
