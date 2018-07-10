using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NestaMigrations.Services
{
    public class OrganisationTypesService
    {
        public int GetOrganisationTypeIdFromName(string name) {
            switch (name)
            {
                case "Empresa privada(startup)":
                case "Empresa privada (startup)":
                    return 1;

                case "Organización civil sin fines de lucro (ONG)":
                case "Organización civil sin fines de lucro":
                    return 6;

                case "Fundación internacional":
                case "Organización internacional":
                    return 3;

                case "Empresa privada (no incluye startup)":
                case "Empresa privada(no incluye startup)":
                    return 9;

                case "Gobierno":
                case "Ente gubernamental":
                    return 4;

                case "Universidad":
                case "Universidad/Instituto de investigación":
                    return 7;

                case "Empresa social":
                    return 2;

                case "Incubadora/aceleradora":
                    return 5;

                case "Fundación empresaria":
                case "Otro":
                default:
                    return 8;
            }
        }
    }
}
