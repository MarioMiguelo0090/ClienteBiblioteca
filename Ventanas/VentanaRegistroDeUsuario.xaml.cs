using ClienteBibliotecaElSaber.ServidorElSaber;
using ClienteBibliotecaElSaber.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.ServiceModel;
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
    /// <summary>
    /// Lógica de interacción para VentanaRegistroDeUsuario.xaml
    /// </summary>
    public partial class VentanaRegistroDeUsuario : Page
    {
        private string _opcionSeleccionada;
        public VentanaRegistroDeUsuario()
        {
            InitializeComponent();
        }

        private int RegistrarUsuario()
        {
            var proxyUsuario = new UsuarioManejadorClient();
            
            try
            {
                UsuarioBinding usuario = new UsuarioBinding
                {
                    nombre = Txb_Nombre.Text,
                    primerApellido = Txb_PrimerApellido.Text,
                    segundoApellido = Txb_SegundoApellido.Text,
                    telefono = Txb_Telefono.Text,
                    puesto = Txb_Puesto.Text
                };
                Console.WriteLine($"Nombre: {usuario.nombre}, Apellido1: {usuario.primerApellido}, Apellido2: {usuario.segundoApellido}");
                DireccionBinding direccion = new DireccionBinding
                {
                    calle = Txb_Calle.Text,
                    numero = Txb_Numero.Text,
                    codigoPostal = Txb_CodigoPostal.Text,
                    ciudad = Txb_Ciudad.Text
                };

                AccesoBinding acceso = new AccesoBinding
                {
                    correo = Txb_CorreoElectronico.Text,
                    contrasenia = GeneradorContrasenia.GenerarContrasenia(),
                    tipoDeUsuario = _opcionSeleccionada,
                };
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoExito, "Registro exitoso", "El usuario se ha registrado exitosamente");
                ventanaEmergente.ShowDialog();
                return proxyUsuario.RegistrarUsuarioAlaBaseDeDatos(usuario, direccion, acceso);
            }
            catch (EndpointNotFoundException endpointNotFoundException)
            {
                LoggerManager.Error($"Excepción de EndpointNotFoundException: {endpointNotFoundException.Message}." +
                                    $"\nTraza: {endpointNotFoundException.StackTrace}.");
            }
            catch (TimeoutException timeoutException)
            {
                LoggerManager.Error($"Excepción de TimeoutException: {timeoutException.Message}." +
                                    $"\nTraza: {timeoutException.StackTrace}.");
            }
            catch (CommunicationException communicationException)
            {
                LoggerManager.Error($"Excepción de CommunicationException: {communicationException.Message}." +
                                    $"\nTraza: {communicationException.StackTrace}.");
            }

            return -1;
        }

        private void RadioButton_Checked(object sender, EventArgs e)
        {
            RadioButton tipoUsuario = sender as RadioButton;
            _opcionSeleccionada = tipoUsuario.Content.ToString();
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            RegistrarUsuario();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Content = null;
        }
    }
}
