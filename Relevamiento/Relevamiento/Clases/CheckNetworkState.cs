using Newtonsoft.Json;
using Relevamiento.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Relevamiento.Clases
{
    public static class CheckNetworkState
    {
        public static bool hasConnectivity = false;
        private static bool isAlreadyInitialized = false;
        public static bool isLoged = false;

        public static void StartListening()
        {
            if (!isAlreadyInitialized)
            {
                Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
                isAlreadyInitialized = true;
            }

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                hasConnectivity = true;
        }

        public static void StopListening()
        {
            if (isAlreadyInitialized)
            {
                Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
                isAlreadyInitialized = false;
            }
        }

        private static async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            hasConnectivity = false;

            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                hasConnectivity = true;

                if(isLoged)
                    await CheckPendingsTbRquests();
            }
        }

        private static async Task CheckPendingsTbRquests()
        {
            List<TbRequest> lstTbRequest = new List<TbRequest>();
            using (SQLite.SQLiteConnection conexion = new SQLiteConnection(App.RutaBD))
            {
                lstTbRequest = conexion.Query<TbRequest>("SELECT * FROM TbRequest WHERE req_estado = false").ToList();

                foreach (TbRequest req in lstTbRequest)
                {
                    await SendPostRelevamiento(req.req_json, req);
                }
            }
        }

        private static async Task SendPostRelevamiento(string jsonRelevamiento, TbRequest tbRequestToUpdate)
        {
            try
            {
                //String content que serealiza la clase a string
                StringContent stringContent =
                        new StringContent(jsonRelevamiento, Encoding.UTF8, "application/json");

                HttpClient httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMinutes(30);

				//TEST
				//string urlPost = "http://iserver.itris.com.ar:7101/DACServicesTest/api/Relevamiento";

				//PROD
				string urlPost = "http://iserver.itris.com.ar:7101/DACServices/api/Relevamiento";

				//variable que se utiliza para tomar la respuesta
				HttpResponseMessage httpResponseMessage;

                //Se ejecuta el post y se lo asigna a la variable que contiene la respuesta
                httpResponseMessage = await httpClient.PostAsync(new Uri(urlPost), stringContent);

                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    //Obtengo el mensaje de respuesta del server
                    var stringResponse = httpResponseMessage.Content.ReadAsStringAsync().Result;

                    //Serializo la repsuesta que viene en formato json al tipo de clase
                    //ACA TENES QUE TENER LA RESPUESTA DEL SERVICIO
                    ItrisPlanillaEntity respuesta = JsonConvert.DeserializeObject<ItrisPlanillaEntity>(stringResponse);

                    //Dato a guardar en tabla tbRequest
                    string requestBody = JsonConvert.SerializeObject(respuesta);

                    using (SQLite.SQLiteConnection conexion = new SQLiteConnection(App.RutaBD))
                    {
                        tbRequestToUpdate.req_json = requestBody;
                        tbRequestToUpdate.req_estado = true;
                        int result = conexion.Update(tbRequestToUpdate);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}


