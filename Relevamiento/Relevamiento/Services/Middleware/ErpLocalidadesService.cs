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
    public class ErpLocalidadesService
    {
        private readonly IGenericServiceRepository _genericServiceRepository;

        private readonly Label _lblLocalidadesCreate;
        private readonly Label _lblLocalidadesUpdate;
        private readonly Label _lblLocalidadesDelete;

        public ErpLocalidadesService()
        {
            _genericServiceRepository = new GenericServiceRepository();
        }

        public ErpLocalidadesService(Label lblLocalidadesCreate, Label lblLocalidadesUpdate, Label lblLocalidadesDelete)
        {
            _genericServiceRepository = new GenericServiceRepository();

            _lblLocalidadesCreate = lblLocalidadesCreate;
            _lblLocalidadesUpdate = lblLocalidadesUpdate;
            _lblLocalidadesDelete = lblLocalidadesDelete;
        }

        public async Task<LocalidadesServiceModel> GetListLocalidades(List<ERP_LOCALIDADES> lstLocalidades)
        {
            UriBuilder builder = new UriBuilder(ApiEndpoints.BaseApiUrl)
            {
                Path = ApiEndpoints.ErpLocalidades
            };

            var result = await _genericServiceRepository.PostAsync<List<ERP_LOCALIDADES>, LocalidadesServiceModel>(builder.ToString(), lstLocalidades);

            return result;
        }

        public async Task SynchronizeLocalidades()
        {
            var localidadesServiceModel = await GetListLocalidades(CreateListLocalidades());

            //Respetar el orden para una mejor performance
            UpdateLocalidades(localidadesServiceModel);
            DeleteLocalidades(localidadesServiceModel);
            CreateLocalidades(localidadesServiceModel);
        }

        public void CreateLocalidades(LocalidadesServiceModel localidadesServiceModel)
        {
            using (SQLite.SQLiteConnection conexion = new SQLite.SQLiteConnection(App.RutaBD))
            {
                localidadesServiceModel.listaCreate.ForEach(m => conexion.Insert(m));
                if (_lblLocalidadesCreate != null)
                    _lblLocalidadesCreate.Text = "Created";
            }
        }

        public void UpdateLocalidades(LocalidadesServiceModel localidadesServiceModel)
        {
            if (isAlreadyCreated())
            {
                if (localidadesServiceModel.listaUpdate.Count() > 0)
                {
                    using (SQLite.SQLiteConnection conexion = new SQLite.SQLiteConnection(App.RutaBD))
                    {
                        localidadesServiceModel.listaUpdate.ForEach(m => conexion.Update(m));
                        if (_lblLocalidadesUpdate != null)
                            _lblLocalidadesUpdate.Text = "Updated";
                    }
                }
            }
        }

        public void DeleteLocalidades(LocalidadesServiceModel localidadesServiceModel)
        {
            if (isAlreadyCreated())
            {
                if (localidadesServiceModel.listaDelete.Count() > 0)
                {
                    using (SQLite.SQLiteConnection conexion = new SQLite.SQLiteConnection(App.RutaBD))
                    {
                        localidadesServiceModel.listaDelete.ForEach(m => conexion.Delete(m));
                        if (_lblLocalidadesDelete != null)
                            _lblLocalidadesDelete.Text = "Deleted";
                    }
                }
            }
        }

        public bool isAlreadyCreated()
        {
            int countLocalidades;

            using (SQLite.SQLiteConnection conexion = new SQLite.SQLiteConnection(App.RutaBD))
            {
                countLocalidades = conexion.Table<ERP_LOCALIDADES>().Count();
            }

            return countLocalidades > 0 ? true : false;
        }

        public List<ERP_LOCALIDADES> CreateListLocalidades()
        {
            if (isAlreadyCreated())
            {
                using (SQLite.SQLiteConnection conexion = new SQLite.SQLiteConnection(App.RutaBD))
                {
                    return conexion.Table<ERP_LOCALIDADES>().ToList();
                }
            }

            return new List<ERP_LOCALIDADES>();
        }
    }
}
