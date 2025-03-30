using ClienteBibliotecaElSaber.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;

namespace ClienteBibliotecaElSaber.Ventanas
{
    /// <summary>
    /// Lógica de interacción para RegistrarEditorial.xaml
    /// </summary>
    public partial class VentanaRegistrarEditorial : Window
    {
        public VentanaRegistrarEditorial()
        {
            InitializeComponent();
        }

        public void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            txb_nombreEditorial.BorderBrush = Brushes.White;
            if (ValidarDatos())
            {
                RealizarRegistroDeEditorial();
            }
            else
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Datos incorrectos", "Por favor verifique que los datos ingresados sean los correctos.");
                
            }
        }

        private bool ValidarDatos()
        {
            bool nombreEditorialValido = Validador.ValidarNombreEditorial(txb_nombreEditorial.Text);
            if (!nombreEditorialValido)
            {
                txb_nombreEditorial.BorderBrush = Brushes.Red;
            }
           
            return nombreEditorialValido;
        }

        private void RealizarRegistroDeEditorial()
        {
            try
            {
                ServidorElSaber.LibroManejadorClient libroManejador = new ServidorElSaber.LibroManejadorClient();
                int resultadoRegistro = libroManejador.RegistrarNuevaEditorial(txb_nombreEditorial.Text);
                if (resultadoRegistro == 1)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoExito, "Operación exitosa", "Los datos se han registrado con éxito en la base de datos.");
                    
                }
                else
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente("Error", "Error en la conexión a la base de datos", "Se ha perdido la conexión a la base de datos");
                    
                }
            }
            catch (EndpointNotFoundException endpointNotFoundException)
            {
                LoggerManager.Error($"Excepción de EndpointNotFoundException: {endpointNotFoundException.Message}." +
                                    $"\nTraza: {endpointNotFoundException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Punto de conexión fallido", "No se ha podido establecer conexión con el servidor.");
                
            }
            catch (TimeoutException timeoutException)
            {
                LoggerManager.Error($"Excepción de TimeoutException: {timeoutException.Message}." +
                                    $"\nTraza: {timeoutException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Tiempo de espera agotado", "El tiempo de espera ha caducado, inténtelo de nuevo.");
                
            }
            catch (CommunicationException communicationException)
            {
                LoggerManager.Error($"Excepción de CommunicationException: {communicationException.Message}." +
                                    $"\nTraza: {communicationException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Comunicacion fallida", "La comunicacion con el servidor se ha perdido, por favor verifique su conexión a internet.");
                
            }
            this.Close();
        }


        public void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
