using Relevamiento.Clases;
using Relevamiento.Services.Middleware;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Relevamiento.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Principal : ContentPage, INotifyPropertyChanged
    {
		private tbPaciente _paciente;
        public Principal(tbPaciente paciente)
        {
            InitializeComponent();

			_paciente = paciente;
            lblUsuario.Text = "¡Hola " + paciente.pac_nombre + "!";

            BindingContext = this;
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get => _IsBusy;
            set
            {
                _IsBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override void OnAppearing()
        {
        }

		private async void btnComenzar_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new Formulario(_paciente));
		}
	}

}