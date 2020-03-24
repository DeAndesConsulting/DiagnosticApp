using Newtonsoft.Json;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Relevamiento.Clases;
using Relevamiento.Models;
using Relevamiento.Services.Middleware;
using Relevamiento.Services.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Relevamiento.Vistas
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Formulario : ContentPage
	{
		private FormularioServiceModel _formularioServiceModel = new FormularioServiceModel();
		private tbPaciente _paciente = null;
		private List<tbPregunta> _listaPreguntas = null;

		public Formulario(tbPaciente paciente)
		{
			InitializeComponent();

			//Seteo el paciente al modelo
			_paciente = paciente;

			//Obtengo las preguntas de sqlite
			GenericDataConfig genericDataConfigRecord;

			using (SQLite.SQLiteConnection conexion = new SQLiteConnection(App.RutaBD))
			{
				genericDataConfigRecord = conexion.Query<GenericDataConfig>("select * from GenericDataConfig where Codigo = ?", CodigosApp.CodigoPreguntas).FirstOrDefault();
			}

			//Deserealizo el json para obtener la lista de preguntas.
			if (genericDataConfigRecord != null)
			{
				_listaPreguntas = JsonConvert.DeserializeObject<List<tbPregunta>>(genericDataConfigRecord.Valor);

				Label lbl;
				CheckBox checkBox;
				int i = 0;
				foreach (var obj in _listaPreguntas)
				{
					gridFormulario.RowDefinitions.Add(new RowDefinition());
					lbl = new Label()
					{
						AutomationId = "lbl_" + obj.pre_id.ToString(),
						Text = obj.pre_pregunta,
						FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
					};
					checkBox = new CheckBox() { Color = Color.Blue, AutomationId = "cbx_" + obj.pre_id.ToString() };

					gridFormulario.Children.Add(lbl, 0, i);
					gridFormulario.Children.Add(checkBox, 1, i);
					i++;
				}
			}
			else
			{
				DisplayAlert("Error","Asegúrese de tener conexión a internet.","Ok.");
			}
		}

		private void btnAnterior_Clicked(object sender, EventArgs e)
		{

		}

		private void btnSiguiente_Clicked(object sender, EventArgs e)
		{

		}

		private async void btnEnviar_Clicked(object sender, EventArgs e)
		{
			try
			{
				DisplayActivityIndicator(true);

				//Asigno el código del request al formulario
				_formularioServiceModel.CodigoRequest = DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss:ffff");

				//Seteo los datos del paciente que se obtienen en el constructor al modelo
				_formularioServiceModel.Paciente = _paciente;

				//Creo el objeto para el cuestionario del modelo
				_formularioServiceModel.Cuestionario = new tbCuestionario() { cue_fecha = DateTime.Now };

				//Asigno latitud y longitud al cuestionario
				this.SetearLatitudLongitud();

				//Obtengo las respuestas del formulario xaml
				List<tbCuestionarioPregunta> listaCuestionarioPregunta = this.ObtenerRespuestasDelPaciente();

				//Asigno las respuestas del formulario al modelo
				_formularioServiceModel.ListaCuestionarioPregunta = listaCuestionarioPregunta;

				//Realizo el calculo de las respuestas del paciente contras las respuestas validas de las preguntas.
				_formularioServiceModel.Cuestionario.cue_diagnostico = this.CompararRespuestas(listaCuestionarioPregunta);

				//Realizo el post al servicio
				string jsonFormulario = JsonConvert.SerializeObject(_formularioServiceModel);

				//Llamo al servicio para obtener las preguntas
				FormularioService formularioService = new FormularioService();
				var result = await formularioService.Post(_formularioServiceModel);

				//Dejo de mostrar el activity indicator
				this.DisplayActivityIndicator(false);

				if (result.Cuestionario.cue_id != 0)
				{
					await DisplayAlert("Aviso", "Sus datos se enviaron correctamente.", "Ok");
					await Navigation.PopAsync(true);
				}
				else
					await DisplayAlert("Error", "No se pudo enviar sus datos, asegurese de tener acceso a internet.", "Ok");
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// Obtengo las respuestas del paciente de los controlles
		/// </summary>
		/// <returns>Lista de tbCuestionarioPregunta con las respuestas del paciente.</returns>
		private List<tbCuestionarioPregunta> ObtenerRespuestasDelPaciente()
		{
			List<tbCuestionarioPregunta> listaCuestionarioPregunta = new List<tbCuestionarioPregunta>();
			tbCuestionarioPregunta cuestionarioPregunta;

			foreach (var vista in gridFormulario.Children)
			{
				//Obtengo los valores de los controles checkbox
				string[] arreglo = vista.AutomationId.Split('_');

				if (arreglo[0] == "cbx")
				{
					int idPregunta = Convert.ToInt32(arreglo[1]);
					bool respuesta = ((CheckBox)vista).IsChecked;

					//Creo el objeto para el modelo que va viaja al servicio
					cuestionarioPregunta = new tbCuestionarioPregunta()
					{
						pre_id = idPregunta,
						cpr_respuesta = respuesta
					};

					listaCuestionarioPregunta.Add(cuestionarioPregunta);
				}
			}

			return listaCuestionarioPregunta;
		}

		/// <summary>
		/// Funcion que compara el resultado de las respuestas del paciente contra las respuestas que indican el valor
		/// positivo (listado de preguntas).
		/// </summary>
		/// <param name="listaCuestionarioPregunta">Listado de respuestas del paciente.</param>
		/// <returns>Si el diagnostico es positivo o negativo a partir de un valor parametrizable.</returns>
		private bool CompararRespuestas(List<tbCuestionarioPregunta> listaCuestionarioPregunta)
		{
			try
			{
				//Contador que se incrementa a partir de las respuestas del paciente contra las repuestas "correctas"
				//del listado de preguntas.
				int cantidadRespuestasCoincidentes = 0;
				double promedio = 0.0;
				double valorParametrizable = 95.0;

				foreach (var cp in listaCuestionarioPregunta)
				{
					tbPregunta pregunta = _listaPreguntas.Where(x => x.pre_id == cp.pre_id).FirstOrDefault();
					if (cp.cpr_respuesta == pregunta.pre_respuesta_positiva)
						cantidadRespuestasCoincidentes++;
				}

				promedio = (cantidadRespuestasCoincidentes * 100) / _listaPreguntas.Count();

				if (promedio >= valorParametrizable)
					return true;

				return false;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private async void SetearLatitudLongitud()
		{
			//Obtengo la ubicacion del paciente
			Location location = null;
			var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

			if (status == PermissionStatus.Granted)
			{
				location = await Geolocation.GetLastKnownLocationAsync();
			}

			if (location != null)
			{
				_formularioServiceModel.Cuestionario.cue_latitud = location.Latitude.ToString();
				_formularioServiceModel.Cuestionario.cue_longitud = location.Longitude.ToString();
			}
			else
			{
				_formularioServiceModel.Cuestionario.cue_latitud = "0";
				_formularioServiceModel.Cuestionario.cue_longitud = "0";
			}
		}

		public void DisplayActivityIndicator(bool parametro)
		{
			try
			{
				aiLogin.IsVisible = parametro;
				aiLogin.IsRunning = parametro;
				cvwActivity.IsVisible = parametro;
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}
	}
}