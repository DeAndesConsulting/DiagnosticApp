using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Relevamiento.Clases;
using Relevamiento.Vistas;
using SQLite;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Relevamiento.Models;
using Relevamiento.Services.Middleware;
using System.Diagnostics;

namespace Relevamiento
{
	public partial class App : Application
	{
		public static ERP_ASESORES globalAsesor = new ERP_ASESORES();
		public static string RutaBD;
		public App(string rutaBD)
		{
			InitializeComponent();

			//Linea para obtener permisos
			//Task.Run(async() => await ObtenerPermisos()).GetAwaiter().GetResult();

			RutaBD = rutaBD;
			VersionTracking.Track();
			bool firsttime = VersionTracking.IsFirstLaunchForCurrentVersion;

			using (SQLite.SQLiteConnection conexion = new SQLite.SQLiteConnection(RutaBD))
			{
				conexion.CreateTable<GenericDataConfig>();
				conexion.CreateTable<TbRequest>();

				//Debug.WriteLine($"{"LOCALIDADES: " + conexion.Table<ERP_LOCALIDADES>().Count().ToString()}");
				//Debug.WriteLine($"{"LOCALIDADES: " + countLocalidades.ToString()}");
			}

			//DROP DE TABLAS
			/*using (SQLite.SQLiteConnection conexion = new SQLite.SQLiteConnection(RutaBD))
			{
				//conexion.DropTable<_ARTICULOS>();
			}*/

			MainPage = new NavigationPage(new Login())
			{
				BarBackgroundColor = Color.FromHex("#aed4ff"),
				BarTextColor = Color.Gray
			};
		}

		private async Task ObtenerPermisos()
		{
			//Verify Permission
			var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Phone);
			if (status != PermissionStatus.Granted)
			{
				var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Phone);
				//Best practice to always check that the key exists
				if (results.ContainsKey(Permission.Phone))
					status = results[Permission.Phone];
			}
			status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
			if (status != PermissionStatus.Granted)
			{
				var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
				//Best practice to always check that the key exists
				if (results.ContainsKey(Permission.Location))
					status = results[Permission.Location];
			}
		}

		protected override void OnStart()
		{
			// Handle when your app starts
			CheckNetworkState.StartListening();
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
			CheckNetworkState.StopListening();
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
			CheckNetworkState.StartListening();
		}

		private bool TableExists(string tableName)
		{
			bool sw = false;
			try
			{
				using (SQLite.SQLiteConnection connection = new SQLite.SQLiteConnection(RutaBD))
				{
					string query = string.Format("SELECT name FROM sqlite_master WHERE type='table' AND name='{0}';", tableName);
					SQLiteCommand cmd = connection.CreateCommand(query);
					var item = connection.Query<object>(query);
					if (item.Count > 0)
						sw = true;
					return sw;
				}
			}
			catch (SQLiteException ex)
			{
				throw ex;
			}
		}

	}
}
