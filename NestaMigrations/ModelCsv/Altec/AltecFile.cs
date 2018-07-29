using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NestaMigrations.ModelCsv.Altec
{
    public class AltecFile
    {
        public string NombreIniciativa { get; set; }
        public int CantidadOrganizaciones { get; set; }

        public string Alianza { get; set; }
        public string Alianza2 { get; set; }
        public string UserFullName { get; set; }
        public string Mail { get; set; }
        public string UsernameSkype { get; set; }
        public string EmpleadosRangoCantidad { get; set; }
        public string EmpleadosTotalMujeres { get; set; }
        public string EmpleadosTotalLideres { get; set; }
        public string EmpleadosTotalMujeresLideres { get; set; }
        public string EmpleadosTotalDirectores { get; set; }
        public string EmpleadosTotalDirectoresMujeres { get; set; }
        public string VoluntariosActivos { get; set; }
        public string OrgFechaCreacion { get; set; }
        
        public string LineaFinanciamiento { get; set; }
        //Tematicas
        public bool ParticipacionCiudadana { get; set; }
        public bool TransparenciaRendicionCuentas { get; set; }
        public bool PeriodismoDatos { get; set; }
        public bool MapeoCiudadano { get; set; }
        public bool MonitoreoServiciosPublicos { get; set; }
        public bool UsoDatosAbiertos { get; set; }
        public string OtraTematica { get; set; }

        public string OrgNombreOrganizacion { get; set; }
        public string OrgTipoOrganizacion { get; set; }
        public string OrgPais { get; set; }
        public string OrgCiudad { get; set; }
        public string OrgWeb { get; set; }


        public string OrgPrincipalAlinza { get; set; }
        public string OrgDesarrolloPlataformasAplicativosTecnologicos { get; set; }
        public string OrgVariosPaises { get; set; }
        public string OrgSpinOffIncubacion { get; set; }

        //Tematicas organizacion
        public bool OrgParticipacionCiudadana { get; set; }
        public bool OrgTransparenciaRendicionCuentas { get; set; }
        public bool OrgParlamentoAbierto { get; set; }
        public bool OrgComprasPublicas { get; set; }
        public bool OrgPeriodismoDatos { get; set; }
        public bool OrgMapeoCiudadano { get; set; }
        public bool OrgMonitoreoServiciosPublicos { get; set; }
        public string OrgOtraTematica { get; set; }

        //El desarrollo tecnológico asociado a la iniciativa propuesta supone...
        public bool EscalamientoPlataformaExistente { get; set; }
        public bool AdaptacionPlataformaExistente { get; set; }
        public bool DesarrolloPlataformaNueva { get; set; }
        public string Otros { get; set; }

        
    }
}