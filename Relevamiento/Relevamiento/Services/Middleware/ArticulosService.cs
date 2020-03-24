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
    public class ArticulosService
    {
        private readonly IGenericServiceRepository _genericServiceRepository;

        private readonly Label _lblArticulosCreate;
        private readonly Label _lblArticulosUpdate;
        private readonly Label _lblArticulosDelete;

        public ArticulosService()
        {
            _genericServiceRepository = new GenericServiceRepository();
        }

        public ArticulosService(Label lblArticulosCreate, Label lblArticulosUpdate, Label lblArticulosDelete)
        {
            _genericServiceRepository = new GenericServiceRepository();

            _lblArticulosCreate = lblArticulosCreate;
            _lblArticulosUpdate = lblArticulosUpdate;
            _lblArticulosDelete = lblArticulosDelete;
        }

        public async Task<ArticulosServiceModel> GetListArticulos(List<_ARTICULOS> lstArticulos)
        {
            UriBuilder builder = new UriBuilder(ApiEndpoints.BaseApiUrl)
            {
                Path = ApiEndpoints.Articulos
            };

            var result = await _genericServiceRepository.PostAsync<List<_ARTICULOS>, ArticulosServiceModel>(builder.ToString(), lstArticulos);

            return result;
        }

        public async Task SynchronizeArticulos()
        {
            var articulosServiceModel = await GetListArticulos(CreateListArticulos());

            //Respetar el orden para una mejor performance
            UpdateArticulos(articulosServiceModel);
            DeleteArticulos(articulosServiceModel);
            CreateArticulos(articulosServiceModel);
        }

        public void CreateArticulos(ArticulosServiceModel articulosServiceModel)
        {
            using (SQLite.SQLiteConnection conexion = new SQLite.SQLiteConnection(App.RutaBD))
            {
                articulosServiceModel.listaCreate.ForEach(m => conexion.Insert(m));
                if (_lblArticulosCreate != null)
                    _lblArticulosCreate.Text = "Created";
            }
        }

        public void UpdateArticulos(ArticulosServiceModel articulosServiceModel)
        {
            if (isAlreadyCreated())
            {
                if (articulosServiceModel.listaUpdate.Count() > 0)
                {
                    using (SQLite.SQLiteConnection conexion = new SQLite.SQLiteConnection(App.RutaBD))
                    {
                        articulosServiceModel.listaUpdate.ForEach(m => conexion.Update(m));
                        if (_lblArticulosUpdate != null)
                            _lblArticulosUpdate.Text = "Updated";
                    }
                }
            }
        }

        public void DeleteArticulos(ArticulosServiceModel articulosServiceModel)
        {
            if (isAlreadyCreated())
            {
                if (articulosServiceModel.listaDelete.Count() > 0)
                {
                    using (SQLite.SQLiteConnection conexion = new SQLite.SQLiteConnection(App.RutaBD))
                    {
                        articulosServiceModel.listaDelete.ForEach(m => conexion.Delete(m));
                        if (_lblArticulosDelete != null)
                            _lblArticulosDelete.Text = "Deleted";
                    }
                }
            }
        }

        public bool isAlreadyCreated()
        {
            int countArticulos;

            using (SQLite.SQLiteConnection conexion = new SQLite.SQLiteConnection(App.RutaBD))
            {
                countArticulos = conexion.Table<_ARTICULOS>().Count();
            }

            return countArticulos > 0 ? true : false;
        }

        public List<_ARTICULOS> CreateListArticulos()
        {
            if (isAlreadyCreated())
            {
                using (SQLite.SQLiteConnection conexion = new SQLite.SQLiteConnection(App.RutaBD))
                {
                    return conexion.Table<_ARTICULOS>().ToList();
                }
            }

            return new List<_ARTICULOS>();
        }
    }
}
