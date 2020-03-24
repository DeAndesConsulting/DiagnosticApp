using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relevamiento.Models
{
    public class TbRequest
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string req_codigo { get; set; }
        public string req_json { get; set; }
        public bool req_estado { get; set; }
    }
}
