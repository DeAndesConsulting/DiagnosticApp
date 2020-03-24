using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relevamiento.Models
{
    public class SynchronizeDataConfig
    {
        [PrimaryKey]
        public int ID { get; set; }
        public bool isSynchronized { get; set; }
        public DateTime lastSynchronized { get; set; }
        public bool isFirstTimeSynchronizedReady { get; set; }
        public bool isFirstTimeLoggedReady { get; set; }
        public string c_IMEI { get; set; }
        public bool isArticulosReady { get; set; }
        public bool isAsesoresReady { get; set; }
        public bool isEmpresasReady { get; set; }
        public bool isLocalidadesReady { get; set; }
    }

    public class GenericDataConfig
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
		[Unique]
		public string Codigo { get; set; }
		public string Valor { get; set; }
        //public bool isSynchronized { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
