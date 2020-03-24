using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Relevamiento.Repository
{
    public interface IGenericServiceRepository
    {
        Task<T> GetAsync<T>(string uri, string authToken = "");
        Task<T> GetAsync<T>(string uri);
        //Task<T> PostAsync<T>(string uri, T data, string authToken = "");
        Task<T> PutAsync<T>(string uri, T data, string authToken = "");
        Task DeleteAsync(string uri, string authToken = "");
        Task<R> PostAsync<T, R>(string uri, T data, string authToken = "");
		//Task<T> PostGetAllAsync<T>(string uri, T data, string authToken = "");
	}
}
