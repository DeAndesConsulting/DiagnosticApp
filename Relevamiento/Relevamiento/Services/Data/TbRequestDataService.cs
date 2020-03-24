using Relevamiento.Models;
using Relevamiento.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Relevamiento.Services.Data
{
    public class TbRequestDataService
    {
        private IGenericRepository<TbRequest> _genericRepository; 
        public TbRequestDataService()
        {
            _genericRepository = new GenericRepository<TbRequest>();
        }

        public TbRequest getByCodigo(string reqCodigo)
        {
            var result = _genericRepository.GetAll()
                .Where(tbRequest => tbRequest.req_codigo == reqCodigo).FirstOrDefault();

            return result;
        }

        public bool isInserted(string reqCodigo)
        {
            var result = getByCodigo(reqCodigo) != null ? true : false;

            return result;
        }

        public bool Insert(TbRequest tbRequest)
        {
            var result = _genericRepository.Insert(tbRequest);

            return result;
        }

        public bool Update(TbRequest tbRequest)
        {
            var result = _genericRepository.Update(tbRequest);

            return result;
        }
    }
}
