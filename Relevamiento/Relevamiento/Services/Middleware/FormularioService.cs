using Relevamiento.Clases;
using Relevamiento.Repository;
using Relevamiento.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Relevamiento.Services.Middleware
{
	public class FormularioService
	{
		private readonly IGenericServiceRepository _genericServiceRepository;

		public FormularioService()
		{
			_genericServiceRepository = new GenericServiceRepository();
		}

		public async Task<FormularioServiceModel> Post(FormularioServiceModel model)
		{
			UriBuilder builder = new UriBuilder(ApiEndpoints.BaseApiUrl)
			{
				Path = ApiEndpoints.Formulario
			};

			var result = await _genericServiceRepository.PostAsync<FormularioServiceModel, FormularioServiceModel>(builder.ToString(), model);

			return result;
		}

	}
}
