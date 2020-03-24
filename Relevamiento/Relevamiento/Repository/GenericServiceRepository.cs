using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Relevamiento.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Relevamiento.Repository
{
    public class GenericServiceRepository : IGenericServiceRepository
    {       
        public Task DeleteAsync(string uri, string authToken = "")
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetAsync<T>(string uri, string authToken = "")
        {
            try
            {
                HttpClient httpClient = CreateHttpClient(uri);
                HttpResponseMessage responseMessage = await httpClient.GetAsync(uri);

                string jsonResult = string.Empty;

                if (responseMessage.IsSuccessStatusCode)
                {
                    jsonResult = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                    var json = JsonConvert.DeserializeObject<T>(jsonResult);
                    return json;
                }

                //error
                if (responseMessage.StatusCode == HttpStatusCode.Forbidden ||
                    responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new ServiceAuthenticationException(jsonResult);
                }

                throw new HttpRequestExceptionEx(responseMessage.StatusCode, jsonResult);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ ex.GetType().Name + " : " + ex.Message}");
                throw ex;
            }
        }

		public async Task<T> GetAsync<T>(string uri)
		{
			try
			{
				HttpClient httpClient = CreateHttpClient(uri);
				HttpResponseMessage responseMessage = await httpClient.GetAsync(uri);

				string jsonResult = string.Empty;

				if (responseMessage.IsSuccessStatusCode)
				{
					jsonResult = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

					var json = JsonConvert.DeserializeObject<T>(jsonResult);
					return json;
				}

				//error
				if (responseMessage.StatusCode == HttpStatusCode.Forbidden ||
					responseMessage.StatusCode == HttpStatusCode.Unauthorized)
				{
					throw new ServiceAuthenticationException(jsonResult);
				}

				throw new HttpRequestExceptionEx(responseMessage.StatusCode, jsonResult);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"{ ex.GetType().Name + " : " + ex.Message}");
				throw ex;
			}
		}

		/*public async Task<T> PostAsync<T>(string uri, T data, string authToken = "")
        {
            try
            {
                HttpClient httpClient = CreateHttpClient(uri);

                var content = new StringContent(JsonConvert.SerializeObject(data));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage responseMessage = await httpClient.PostAsync(uri, content);

                string jsonResult = string.Empty;

                if (responseMessage.IsSuccessStatusCode)
                {
                    jsonResult = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                    var json = JsonConvert.DeserializeObject<T>(jsonResult);
                    return json;
                }

                if (responseMessage.StatusCode == HttpStatusCode.Forbidden ||
                    responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new ServiceAuthenticationException(jsonResult);
                }

                throw new HttpRequestExceptionEx(responseMessage.StatusCode, jsonResult);

            }
            catch (Exception e)
            {
                Debug.WriteLine($"{ e.GetType().Name + " : " + e.Message}");
                throw;
            }
        }*/

		/*public Task<R> PostAsync<T, R>(string uri, T data, string authToken = "")
        {
            throw new NotImplementedException();
        }*/

		public async Task<R> PostAsync<T, R>(string uri, T data, string authToken = "")
        {
            try
            {
                HttpClient httpClient = CreateHttpClient(uri);
                httpClient.Timeout = TimeSpan.FromMinutes(30);

                string jsonData = string.Empty;

                var content = new StringContent(JsonConvert.SerializeObject(data));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                Debug.WriteLine($"{ "SERVICIO: " + uri }");

                HttpResponseMessage responseMessage = await httpClient.PostAsync(uri, content);

                string jsonResult = string.Empty;

                if (responseMessage.IsSuccessStatusCode)
                {
                    jsonResult = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                    //object oJson = JsonConvert.DeserializeObject(jsonResult);
                    //JObject obj = JObject.Parse(oJson.ToString());
                    //string _operationType = obj[operationType].ToString();

                    var json = JsonConvert.DeserializeObject<R>(jsonResult);
                    return json;
                }

                if (responseMessage.StatusCode == HttpStatusCode.Forbidden ||
                    responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new ServiceAuthenticationException(jsonResult);
                }

                throw new HttpRequestExceptionEx(responseMessage.StatusCode, jsonResult);

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ ex.GetType().Name + " : " + ex.Message}");
                throw ex;
            }
        }

		public Task<T> PutAsync<T>(string uri, T data, string authToken = "")
        {
            throw new NotImplementedException();
        }

        private HttpClient CreateHttpClient(string authToken)
        {
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders
                .Accept.Add(new MediaTypeWithQualityHeaderValue("aplication/json"));

            if (!string.IsNullOrEmpty(authToken))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", authToken);
            }

            return httpClient;
        }
    }
}
