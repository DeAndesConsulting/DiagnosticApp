using System;
using System.Collections.Generic;
using System.Text;

namespace Relevamiento.Clases
{
    public class ApiEndpoints
    {
        public const string BaseApiUrl = "http://dacservicesdiagnosticapp.azurewebsites.net/";

		//TEST
		//public const string ErpAsesores = "DACServicesTest/api/ServiceErpAsesores/Synchronize";
		//public const string ErpEmpresas = "DACServicesTest/api/ServiceErpEmpresas/Synchronize";
		//public const string ErpLocalidades = "DACServicesTest/api/ServiceErpLocalidades/Synchronize";
		//public const string Articulos = "DACServicesTest/api/ServiceArticulo/Synchronize";

		//PROD
		public const string Pregunta = "api/ServicePregunta/";
		public const string Formulario = "api/ServiceFormulario";

		public const string ErpAsesores = "DACServices/api/ServiceErpAsesores/Synchronize";
		public const string ErpEmpresas = "DACServices/api/ServiceErpEmpresas/Synchronize";
		public const string ErpLocalidades = "DACServices/api/ServiceErpLocalidades/Synchronize";
		public const string Articulos = "DACServices/api/ServiceArticulo/Synchronize";
	}
}
