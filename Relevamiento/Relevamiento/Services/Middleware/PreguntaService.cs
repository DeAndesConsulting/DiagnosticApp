using Relevamiento.Clases;
using Relevamiento.Repository;
using Relevamiento.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Relevamiento.Services.Middleware
{
	public class PreguntaService
	{
		private readonly IGenericServiceRepository _genericServiceRepository;

		public PreguntaService()
		{
			_genericServiceRepository = new GenericServiceRepository();
		}

		public async Task<List<tbPregunta>> GetListPregunta()
		{
			UriBuilder builder = new UriBuilder(ApiEndpoints.BaseApiUrl)
			{
				Path = ApiEndpoints.Pregunta
			};
			var result = await _genericServiceRepository.GetAsync<List<tbPregunta>>(builder.ToString());

			return result;
		}
	}
}
