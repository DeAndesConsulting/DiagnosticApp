using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Relevamiento.Clases;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.ComponentModel;
using SQLite;
using System.Collections.Generic;
using Relevamiento.Services.Middleware;
using System.Linq;
using Relevamiento.Models;
using Newtonsoft.Json;

namespace Relevamiento.Vistas
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Login : ContentPage
	{
		public Login()
		{
			InitializeComponent();

			//using (SQLite.SQLiteConnection conexion = new SQLite.SQLiteConnection(App.RutaBD))
			//{
			//	_synchronizeDataConfig = conexion.Table<SynchronizeDataConfig>().FirstOrDefault();

				/*if (_synchronizeDataConfig.lastSynchronized.Day != DateTime.Today.Day)
                {
                    _synchronizeDataConfig.isSynchronized = false;
                }*/
			//}

			//OK		
			//Task.Run(async () => await CargaDeDatosInicial()).GetAwaiter().GetResult();

			//Task.Run(async () => await AskForPermissions());

			//TEST
			//Task.Run(async() => await CargaDeDatosInicial());

			//test formulario principal
			//ERP_ASESORES asesor = new ERP_ASESORES();
			//using (SQLite.SQLiteConnection conexion = new SQLiteConnection(App.RutaBD))
			//{
			//	var listaAsesores = conexion.Table<ERP_ASESORES>().ToList();
			//	//listaEmpresas = conexion.Table<ERP_EMPRESAS>().ToList();
			//	asesor = conexion.Table<ERP_ASESORES>().Where(a => a.c_IMEI == "358240051111110").FirstOrDefault();
			//}

			//Usuario User = new Usuario();
			//User.NombreUsuario = asesor.DESCRIPCION;
			//User.NumeroImei = asesor.c_IMEI;
			//CheckNetworkState.isLoged = true;
			//Navigation.PushAsync(new Principal(User));
			//test formulario principal
		}

		protected async override void OnAppearing()
		{
			//base.OnAppearing();
			//PermissionStatus status = await CrossPermissions.Current.RequestPermissionAsync<CalendarPermission>();
			//await CrossPermissions.Current.RequestPermissionsAsync(Permission.Phone);
			//await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
			//ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, REQUEST_LOCATION);


			//CARP
			//await AskForPermissions();
			//         if (!isSynchronizing)
			//         {
			//             isSynchronizing = true;

			//             aiLogin.IsVisible = true;
			//             aiLogin.IsRunning = true;
			//             aiLogin.IsEnabled = true;
			//             lblMensaje.Text = "Sincronizando información... \nEste proceso puede demorar algunos minutos. \nAsegúrese de tener una buena conexión a internet.";

			//             Task.Run(async () => await CargaDeDatosInicial());
			//         }

		}

		private async void btnComenzar_Clicked(object sender, EventArgs e)
		{
			if (ValidarEntries())
			{
				try
				{
					this.ValidarEquipo(true);

					GenericDataConfig genericDataConfigRecord;

					//Llamo al servicio para obtener las preguntas
					var preguntaService = new PreguntaService();
					List<tbPregunta> preguntas = await preguntaService.GetListPregunta();

					if (preguntas != null)
					{
						//guardo o actualizo preguntas en la bd
						using (SQLite.SQLiteConnection conexion = new SQLite.SQLiteConnection(App.RutaBD))
						{
							genericDataConfigRecord = conexion.Query<GenericDataConfig>("select * from GenericDataConfig where Codigo = ?", CodigosApp.CodigoPreguntas).FirstOrDefault();

							if (genericDataConfigRecord != null)
							{
								genericDataConfigRecord.Valor = JsonConvert.SerializeObject(preguntas);
								genericDataConfigRecord.LastUpdate = DateTime.Now;
								conexion.Update(genericDataConfigRecord);
							}
							else
							{
								genericDataConfigRecord = new GenericDataConfig();
								genericDataConfigRecord.Valor = JsonConvert.SerializeObject(preguntas);
								genericDataConfigRecord.LastUpdate = DateTime.Now;
								genericDataConfigRecord.Codigo = CodigosApp.CodigoPreguntas;
								conexion.Insert(genericDataConfigRecord);
							}
						}
					}

					List<tbPregunta> listaPregunta = null;
					using (SQLite.SQLiteConnection conexion = new SQLiteConnection(App.RutaBD))
					{
						genericDataConfigRecord = conexion.Query<GenericDataConfig>("select * from GenericDataConfig where Codigo = ?", CodigosApp.CodigoPreguntas).FirstOrDefault();
					}

					//Deserealizo el json para obtener la lista de preguntas.
					if (genericDataConfigRecord != null)
						listaPregunta = JsonConvert.DeserializeObject<List<tbPregunta>>(genericDataConfigRecord.Valor);

					//Si la lista de preguntas no es nula, entonces avanzo a la pantalla principal
					if (listaPregunta != null)
					{
						//Una vez almacenadas/actualizadas las preguntas, abro la pantalla principal
						tbPaciente paciente = new tbPaciente();
						paciente.pac_dni = Convert.ToInt32(entryDni.Text);
						paciente.pac_nombre = entryNombre.Text;
						paciente.pac_apellido = entryApellido.Text;
						paciente.pac_genero = entryGenero.Text;
						await Navigation.PushAsync(new Principal(paciente));
					}
					else
					{
						await DisplayAlert("Error", "Asegurese de tener internet en el telefono e intente nuevamente.", "Cancelar");
					}

				}
				catch (Exception ex)
				{
					throw ex;
				}
				finally
				{
					this.ValidarEquipo(false);
				}
			}
		}

		private bool ValidarEntries()
		{
			bool validacionEntries = true;

			if (string.IsNullOrEmpty(entryDni.Text))
			{
				lblDNIfail.IsVisible = true;
				validacionEntries = false;
			}

			if (string.IsNullOrEmpty(entryNombre.Text))
			{
				lblNombrefail.IsVisible = true;
				validacionEntries = false;
			}

			if (string.IsNullOrEmpty(entryApellido.Text))
			{
				lblApellidofail.IsVisible = true;
				validacionEntries = false;
			}

			if (string.IsNullOrEmpty(entryGenero.Text))
			{
				lblGenerofail.IsVisible = true;
				validacionEntries = false;
			}

			return validacionEntries;
		}

		private async Task AskForPermissions()
		{
			await CrossPermissions.Current.RequestPermissionsAsync(Permission.Phone);
			await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
		}

		//public void ValidarEquipo()
		public async Task ValidarEquipo()
		{
			//await AskForPermissions();

			try
			{
				//Get Imei
				//string imeiTelefono = DependencyService.Get<IServiceImei>().GetImei();
				string imeiTelefono = Task.Run(async () => await GetImei()).GetAwaiter().GetResult();
				//DevieId.Text = "IMEI = " + imeiTelefono;

				List<ERP_ASESORES> listaAsesores = new List<ERP_ASESORES>();
				//List<ERP_EMPRESAS> listaEmpresas = new List<ERP_EMPRESAS>();
				ERP_ASESORES asesor = new ERP_ASESORES();
				//validacion con asesores
				using (SQLite.SQLiteConnection conexion = new SQLiteConnection(App.RutaBD))
				{
					listaAsesores = conexion.Table<ERP_ASESORES>().ToList();
					//listaEmpresas = conexion.Table<ERP_EMPRESAS>().ToList();
					asesor = conexion.Table<ERP_ASESORES>().Where(a => a.c_IMEI == imeiTelefono).FirstOrDefault();
				}

				if (asesor != null)
				{
					Usuario User = new Usuario();
					User.NombreUsuario = asesor.DESCRIPCION;
					User.NumeroImei = asesor.c_IMEI;
					CheckNetworkState.isLoged = true;

					//aiLogin.IsVisible = false;
					//aiLogin.IsRunning = false;
					//aiLogin.IsEnabled = false;

					//await Navigation.PushAsync(new Principal(User));
					//Esto se usa cuando no te da pelota la vista	
					//Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new Principal(User)));
				}
				else
				{
					if (listaAsesores.Count() == 0)
						lblMensaje.Text = "No se ha podido inicializar la aplicacion por falta de conexion a internet.";
					else
						lblMensaje.Text = "Este equipo no se encuentra habilitado para utilizar la aplicacion. \n Por favor contacte un administrador.";

					lblMensaje.TextColor = Color.Red;
					aiLogin.IsRunning = false;
					aiLogin.IsEnabled = false;
					aiLogin.IsVisible = false;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		async Task<string> GetImei()
		{
			////Verify Permission
			var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Phone);
			//if (status != PermissionStatus.Granted)
			//{
			//	var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Phone);
			//	//Best practice to always check that the key exists
			//	if (results.ContainsKey(Permission.Phone))
			//		status = results[Permission.Phone];
			//}
			//status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
			//if (status != PermissionStatus.Granted)
			//{
			//	var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
			//	//Best practice to always check that the key exists
			//	if (results.ContainsKey(Permission.Location))
			//		status = results[Permission.Location];
			//}

			//Get Imei
			string imei = DependencyService.Get<IServiceImei>().GetImei();
			return imei;
		}

		//void ObtenerImei(object sender, EventArgs e)
		//{
		//	//Get Imei
		//	string imeiTelefono = DependencyService.Get<IServiceImei>().GetImei();
		//	//DevieId.Text = "IMEI = " + imeiTelefono;

		//	List<ERP_ASESORES> listaAsesores = new List<ERP_ASESORES>();
		//	ERP_ASESORES asesor = new ERP_ASESORES();
		//	//validacion con asesores
		//	using (SQLite.SQLiteConnection conexion = new SQLiteConnection(App.RutaBD))
		//	{
		//		listaAsesores = conexion.Table<ERP_ASESORES>().ToList();
		//		asesor = conexion.Table<ERP_ASESORES>().Where(a => a.c_IMEI == imeiTelefono).FirstOrDefault();
		//	}

		//	if (asesor != null)
		//	{
		//		Usuario User = new Usuario();
		//		User.NombreUsuario = asesor.DESCRIPCION;
		//		User.NumeroImei = asesor.c_IMEI;
		//		CheckNetworkState.isLoged = true;
		//		Navigation.PushAsync(new Principal(User));
		//	}
		//	else
		//	{
		//		aiLogin.IsVisible = false;
		//		aiLogin.IsRunning = false;
		//		aiLogin.IsEnabled = false;
		//		lblMensaje.TextColor = Color.Red;
		//		lblMensaje.Text = "Este equipo no se encuentra habilitado para utilizar la aplicacion. \n Por favor contacte un administrador.";
		//	}

		//}

		public void ValidarEquipo(bool parametro)
		{
			try
			{
				//Muestro los controles de login
				lblMensaje.Text = "Iniciando...";
				lblMensaje.IsVisible = parametro;
				aiLogin.IsVisible = parametro;
				aiLogin.IsRunning = parametro;
				cvwActivity.IsVisible = parametro;

				//Device.BeginInvokeOnMainThread(() => { GridAct.IsVisible = false; });

				//Device.BeginInvokeOnMainThread(() =>
				//{
				//	//Muestro los controles de login
				//	sloLogin.IsVisible = true;
				//	//Oculto controles de sync
				//	lblMensaje.IsVisible = false;
				//	aiLogin.IsVisible = false;
				//});
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}

		private void entryDni_TextChanged(object sender, TextChangedEventArgs e)
		{
			lblDNIfail.IsVisible = false;
		}

		private void entryNombre_TextChanged(object sender, TextChangedEventArgs e)
		{
			lblNombrefail.IsVisible = false;
		}

		private void entryApellido_TextChanged(object sender, TextChangedEventArgs e)
		{
			lblApellidofail.IsVisible = false;
		}

		private void entryGenero_TextChanged(object sender, TextChangedEventArgs e)
		{
			lblGenerofail.IsVisible = false;
		}
	}
}