using Relevamiento.Clases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relevamiento.Services.Models
{
    public class EmpresasServiceModel
    {
        public List<ERP_EMPRESAS> listaCreate { get; set; }
        public List<ERP_EMPRESAS> listaUpdate { get; set; }
        public List<ERP_EMPRESAS> listaDelete { get; set; }
    }
}
