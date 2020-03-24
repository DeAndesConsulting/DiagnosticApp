using Relevamiento.Clases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relevamiento.Services.Models
{
	public class FormularioServiceModel
	{
		public tbPaciente Paciente { get; set; }
		public tbCuestionario Cuestionario { get; set; }
		public List<tbCuestionarioPregunta> ListaCuestionarioPregunta { get; set; }
		public string CodigoRequest { get; set; }
	}
}
