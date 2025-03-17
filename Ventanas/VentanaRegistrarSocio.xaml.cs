using ClienteBibliotecaElSaber.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClienteBibliotecaElSaber.Ventanas
{
    public partial class VentanaRegistrarSocio : Window
    {
        public VentanaRegistrarSocio()
        {
            InitializeComponent();
            LimitarDatePickers();
        }

        private void LimitarDatePickers()
        {
            dp_FechaInscripcion.DisplayDateStart = new DateTime(2025, 1, 1);
            dp_FechaInscripcion.DisplayDateEnd = DateTime.Today;
            dp_FechaInscripcion.SelectedDate = DateTime.Today;
            dp_FechaNacimiento.DisplayDateStart = new DateTime(1920, 1, 1);
            dp_FechaNacimiento.DisplayDateEnd = DateTime.Today;
            dp_FechaNacimiento.SelectedDate = DateTime.Today;
        }

        private void Registrar_Click(object sender, RoutedEventArgs e)
        {
            ResetearColorDeBordes();
            if (ValidarDatosDeCampos())
            {

            }
            else
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente("Informacion", "Datos incorrectos", "Por favor verifique que los datos ingresados sean los correctos.");
                ventanaEmergente.ShowDialog();  
            }
        }

        private void ResetearColorDeBordes()
        {
            txtb_Nombre.BorderBrush = Brushes.White;
            txtb_PrimerApellido.BorderBrush = Brushes.White;
            txtb_SegundoApellido.BorderBrush = Brushes.White;
            txtb_Telefono.BorderBrush = Brushes.White;
            txtb_Direccion.BorderBrush = Brushes.White;
            txtb_CodigoPostal.BorderBrush = Brushes.White;
            dp_FechaInscripcion.BorderBrush = Brushes.White;
            dp_FechaNacimiento.BorderBrush = Brushes.White;
        }

        private bool ValidarDatosDeCampos()
        {
            bool nombreValidado = Validador.ValidarNombre(txtb_Nombre.Text);
            bool primerApellidoValidado = Validador.ValidarPrimerApellido(txtb_PrimerApellido.Text);
            bool segundoApellidoValidado = Validador.ValidarSegundoApellido(txtb_SegundoApellido.Text);
            bool telefonoValido = Validador.ValidarTelefono(txtb_Telefono.Text);
            bool direccionValidado = Validador.ValidarDireccion(txtb_Direccion.Text);
            bool codigoPostalValidado = Validador.ValidarCodigoPostal(txtb_CodigoPostal.Text);
            bool fechaDeInscripcionValidada = Validador.ValidarFechas(dp_FechaInscripcion.Text);
            bool fechaDeNacimientoValidada = Validador.ValidarFechas(dp_FechaNacimiento.Text);

            if (!nombreValidado)
            {
                txtb_Nombre.BorderBrush = Brushes.Red;
            }

            if (!primerApellidoValidado)
            {
                txtb_PrimerApellido.BorderBrush = Brushes.Red;
            }

            if (!segundoApellidoValidado)
            {
                txtb_SegundoApellido.BorderBrush = Brushes.Red;
            }

            if (!telefonoValido)
            {
                txtb_Telefono.BorderBrush = Brushes.Red;
            }

            if (!direccionValidado)
            {
                txtb_Direccion.BorderBrush = Brushes.Red;
            }

            if (!codigoPostalValidado)
            {
                txtb_CodigoPostal.BorderBrush = Brushes.Red;
            }

            if (!fechaDeInscripcionValidada)
            {
                dp_FechaInscripcion.BorderBrush = Brushes.Red;
            }

            if (!fechaDeNacimientoValidada)
            {
                dp_FechaNacimiento.BorderBrush = Brushes.Red;   
            }

            return nombreValidado && primerApellidoValidado && segundoApellidoValidado && telefonoValido &&
                direccionValidado && codigoPostalValidado && fechaDeNacimientoValidada && fechaDeInscripcionValidada;
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
