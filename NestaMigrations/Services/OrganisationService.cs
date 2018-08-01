using NestaMigrations.Model;
using NestaMigrations.ModelCsv.Altec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NestaMigrations.Services
{
    public class OrganisationService
    {
        NestaContext context;

        public OrganisationService(NestaContext context)
        {
            this.context = context;
        }

        public Organisation SearchOrganisation(string organisation)
        {
            var org = this.context.Organisations.Where(c => c.name == organisation).FirstOrDefault();
            return org;
        }
    }

    public class OrganisationSizesId
    {
        public const int MoreThan0LessThan5 = 1;
        public const int MoreThan6LessThan10 = 2;
        public const int MoreThan11LessThan25 = 3;
        public const int MoreThan26LessThan50 = 4;
        public const int MoreThan51LessThan100 = 5;
        public const int MoreThan101LessThan500 = 6;
        public const int MoreThan501LessThan1000 = 7;
        public const int MoreThan1000 = 8;

        public static int GetIdFrom(string value)
        {
            int qty = 0;
            if (Int32.TryParse(value, out qty))
            {
                if (qty <= 5)
                    return MoreThan0LessThan5;
                else if (qty <= 10)
                    return MoreThan6LessThan10;
                else if (qty <= 25)
                    return MoreThan11LessThan25;
                else if (qty <= 50)
                    return MoreThan26LessThan50;
                else if (qty <= 100)
                    return MoreThan51LessThan100;
                else if (qty <= 500)
                    return MoreThan101LessThan500;
                else if (qty <= 1000)
                    return MoreThan501LessThan1000;
                else
                {
                    return MoreThan1000;
                }
            }
            else
            {
                switch (value)
                {
                    case "100% Voluntarios":
                        return MoreThan0LessThan5;
                    case "3 o menos":
                        return MoreThan0LessThan5;
                    case "Entre 11 y 20":
                        return MoreThan11LessThan25;
                    case "Entre 21 y 50":
                        return MoreThan26LessThan50;
                    case "Entre 4 y 5":
                        return MoreThan0LessThan5;
                    case "Entre 51 y 100":
                        return MoreThan501LessThan1000;
                    case "Entre 6 y 10":
                        return MoreThan6LessThan10;
                    case "Mayor a 101":
                        return MoreThan101LessThan500;
                    default:
                        return MoreThan0LessThan5;
                }
            }
        }


    }

    public class OrganisationTags
    {

        public static List<int> GetImpactTagsId(AltecFile org)
        {
            List<int> impactTagsIds = new List<int>();

            if (org.OrgParticipacionCiudadana)
                impactTagsIds.Add(ImpactTags.Participacion);
            if (org.OrgTransparenciaRendicionCuentas)
            {
                impactTagsIds.Add(ImpactTags.FinanzaYEconomia);
                impactTagsIds.Add(ImpactTags.Transparencia);
            }
            if (org.OrgParlamentoAbierto)
                impactTagsIds.Add(ImpactTags.Transparencia);
            if (org.OrgComprasPublicas)
                impactTagsIds.Add(ImpactTags.Transparencia);
            if (org.PeriodismoDatos)
                impactTagsIds.Add(ImpactTags.PeriodismoDeDatos);
            if (org.MapeoCiudadano)
                impactTagsIds.Add(ImpactTags.Participacion);
            if (org.OrgMonitoreoServiciosPublicos)
            {
                impactTagsIds.Add(ImpactTags.CiudadesGobiernosLocales);
                impactTagsIds.Add(ImpactTags.Transporte);
            }

            return impactTagsIds;
        }

        public static List<int> GetImpactTagsId(string text)
        {

            List<int> impactTagsIds = new List<int>();
            List<string> items = text.Split(",").Select(x => x.Trim()).ToList();

            foreach (var item in items)
            {
                switch (item)
                {
                    case "participacion ciudadana":
                    case "participación":
                    case "participación ciudadana":
                    case "ciudadania":
                    case "ciudadanía":
                        impactTagsIds.Add(ImpactTags.Participacion);
                        break;
                    case "cultura":
                    case "cultura libre":
                        impactTagsIds.Add(ImpactTags.CulturaYArte);
                        break;
                    case "transparecia":
                    case "transparencia":
                        impactTagsIds.Add(ImpactTags.Transparencia);
                        break;
                    case "educación":
                        impactTagsIds.Add(ImpactTags.EducacionYCapacitacion);
                        break;
                    case "periodismo":
                    case "periodismo de datos":
                        impactTagsIds.Add(ImpactTags.PeriodismoDeDatos);
                        break;
                    case "salud pública":
                        impactTagsIds.Add(ImpactTags.SaludYBienestar);
                        break;
                    default:
                        break;
                }
            }

            return impactTagsIds;
        }
    }

    public class ImpactTags
    {
        public const int EducacionYCapacitacion = 1;
        public const int Participacion = 2;
        public const int CulturaYArte = 3;
        public const int SaludYBienestar = 4;
        public const int Empleo = 5;
        public const int CiudadesGobiernosLocales = 6;
        public const int EnergiaYMedioAmbiente = 7;
        public const int Ciencia = 8;
        public const int FinanzaYEconomia = 9;
        public const int PeriodismoDeDatos = 10;
        public const int Transparencia = 11;
        public const int Transporte = 12;
    }

}
