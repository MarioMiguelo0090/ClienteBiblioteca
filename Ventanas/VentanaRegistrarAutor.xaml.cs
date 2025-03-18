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
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class VentanaRegistrarAutor : Window
    {
        public VentanaRegistrarAutor()
        {
            InitializeComponent();
        }

        public void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarDatos())
            {
                RealizarRegistroDeAutor();
            }
            else
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Datos incorrectos", "Por favor verifique que los datos ingresados sean los correctos.");
                ventanaEmergente.ShowDialog();
            }
        }

        private bool ValidarDatos()
        {
            bool nombreAutorValidado = Validador.ValidarNombre(txb_nombreAutor.Text);
            if (!nombreAutorValidado)
            {
                txb_nombreAutor.BorderBrush = Brushes.Red;
            }
            return nombreAutorValidado;
        }

        private void RealizarRegistroDeAutor()
        {
            try
            {
                ServidorElSaber.LibroManejadorClient libroManejador = new ServidorElSaber.LibroManejadorClient();
                int resultadoRegistro = libroManejador.RegistrarNuevoAutor(txb_nombreAutor.Text);
                if (resultadoRegistro == 1)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoExito, "Operación exitosa", "Los datos se han registrado con éxito en la base de datos.");
                    ventanaEmergente.ShowDialog();
                }
                else
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente("Error", "Error en la conexión a la base de datos", "Se ha perdido la conexión a la base de datos");
                    ventanaEmergente.ShowDialog();
                }
            }
            catch (EndpointNotFoundException endpointNotFoundException)
            {
                LoggerManager.Error($"Excepción de EndpointNotFoundException: {endpointNotFoundException.Message}." +
                                    $"\nTraza: {endpointNotFoundException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Punto de conexión fallido", "No se ha podido establecer conexión con el servidor.");
                ventanaEmergente.ShowDialog();
            }
            catch (TimeoutException timeoutException)
            {
                LoggerManager.Error($"Excepción de TimeoutException: {timeoutException.Message}." +
                                    $"\nTraza: {timeoutException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Tiempo de espera agotado", "El tiempo de espera ha caducado, inténtelo de nuevo.");
                ventanaEmergente.ShowDialog();
            }
            catch (CommunicationException communicationException)
            {
                LoggerManager.Error($"Excepción de CommunicationException: {communicationException.Message}." +
                                    $"\nTraza: {communicationException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Comunicacion fallida", "La comunicacion con el servidor se ha perdido, por favor verifique su conexión a internet.");
                ventanaEmergente.ShowDialog();
            }
            this.Close();
        }

        public void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
