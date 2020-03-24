using Relevamiento.Clases;
using Relevamiento.Repository;
using Relevamiento.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Relevamiento.Services.Middleware
{
    public class ErpAsesoresService 
    {
        private readonly IGenericServiceRepository _genericServiceRepository;

        private readonly Label _lblAsesoresCreate;
        private readonly Label _lblAsesoresUpdate;
        private readonly Label _lblAsesoresDelete;

        public ErpAsesoresService()
        {
            _genericServiceRepository = new GenericServiceRepository();
        }
       
        public ErpAsesoresService(Label lblAsesoresCreate, Label lblAsesoresUpdate, Label lblAsesoresDelete)
        {
            _genericServiceRepository = new GenericServiceRepository();

            _lblAsesoresCreate = lblAsesoresCreate;
            _lblAsesoresUpdate = lblAsesoresUpdate;
            _lblAsesoresDelete = lblAsesoresDelete;
        }

        public async Task<AsesoresServiceModel> GetListAsesores(List<ERP_ASESORES> lstAsesores)
        {
            UriBuilder builder = new UriBuilder(ApiEndpoints.BaseApiUrl)
            {
                Path = ApiEndpoints.ErpAsesores
            };

            var result = await _genericServiceRepository.PostAsync<List<ERP_ASESORES>, AsesoresServiceModel>(builder.ToString(), lstAsesores);

            return result;
        }

        public async Task SynchronizeAsesores()
        {
            var asesoresServiceModel = await GetListAsesores(CreateListAsesores());

            //Respetar el orden para una mejor performance
            UpdateAsesores(asesoresServiceModel);
            DeleteAsesores(asesoresServiceModel);
            CreateAsesores(asesoresServiceModel);
        }

        public void CreateAsesores(AsesoresServiceModel asesoresServiceModel)
        {
            using (SQLite.SQLiteConnection conexion = new SQLite.SQLiteConnection(App.RutaBD))
            {
                asesoresServiceModel.listaCreate.ForEach(m => conexion.Insert(m));
                if(_lblAsesoresCreate != null)
                    _lblAsesoresCreate.Text = "Created";
            }
        }

        public void UpdateAsesores(AsesoresServiceModel asesoresServiceModel)
        {
            if (isAlreadyCreated())
            {
                if (asesoresServiceModel.listaUpdate.Count() > 0)
                {
                    using (SQLite.SQLiteConnection conexion = new SQLite.SQLiteConnection(App.RutaBD))
                    {
                        asesoresServiceModel.listaUpdate.ForEach(m => conexion.Update(m));
                        if (_lblAsesoresUpdate != null)
                            _lblAsesoresUpdate.Text = "Updated";
                    }
                }
            }
        }

        public void DeleteAsesores(AsesoresServiceModel asesoresServiceModel)
        {
            if (isAlreadyCreated())
            {
                if (asesoresServiceModel.listaDelete.Count() > 0)
                {
                    using (SQLite.SQLiteConnection conexion = new SQLite.SQLiteConnection(App.RutaBD))
                    {
                        asesoresServiceModel.listaDelete.ForEach(m => conexion.Delete(m));
                        if (_lblAsesoresDelete != null)
                            _lblAsesoresDelete.Text = "Deleted";
                    }
                }
            }
        }

        public bool isAlreadyCreated()
        {
            int countAsesores;

            using (SQLite.SQLiteConnection conexion = new SQLite.SQLiteConnection(App.RutaBD))
            {
                countAsesores = conexion.Table<ERP_ASESORES>().Count();
            }

            return countAsesores > 0 ? true : false;
        }

        public List<ERP_ASESORES> CreateListAsesores()
        {
            if (isAlreadyCreated())
            {
                using (SQLite.SQLiteConnection conexion = new SQLite.SQLiteConnection(App.RutaBD))
                {
                    return conexion.Table<ERP_ASESORES>().ToList();
                }
            }

            return new List<ERP_ASESORES>();
        }
    }
}
