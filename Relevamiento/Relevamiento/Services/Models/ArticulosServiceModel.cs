using Relevamiento.Clases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relevamiento.Services.Models
{
    public class ArticulosServiceModel
    {
        public List<_ARTICULOS> listaCreate { get; set; }
        public List<_ARTICULOS> listaUpdate { get; set; }
        public List<_ARTICULOS> listaDelete { get; set; }
    }
}
