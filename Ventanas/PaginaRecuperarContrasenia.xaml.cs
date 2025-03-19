using ClienteBibliotecaElSaber.ServidorElSaber;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClienteBibliotecaElSaber.Ventanas
{
    /// <summary>
    /// Lógica de interacción para PaginaRecuperarContrasenia.xaml
    /// </summary>
    public partial class PaginaRecuperarContrasenia : Page
    {
        public PaginaRecuperarContrasenia()
        {
            InitializeComponent();
        }

        private void Recuperar_Click(object sender, RoutedEventArgs e)
        {
            txtb_Correo.BorderBrush = Brushes.Transparent;
            int resultadoOperacion = -1;

            if (Validador.ValidarCorreo(txtb_Correo.Text))
            {
                if (ValidarCorreoExistente() != 1)
                {
                    return;
                }

                var proxyRestablecimientoCuenta = new RestablecimientoCuentaManejadorClient();

                try
                {
                    resultadoOperacion = proxyRestablecimientoCuenta.CorreoDeRestablecimientoDeContrasenia(txtb_Correo.Text);
                    if (resultadoOperacion == 1)
                    {
                        var ventana = Window.GetWindow(this) as VentanaRecuperarContraseña;
                        if (ventana != null)
                        {
                            PaginaValidacionCodigo paginaValidacionCodigo = new PaginaValidacionCodigo(txtb_Correo.Text);
                            ventana.MarcoPrincipal.Navigate(paginaValidacionCodigo);
                        }
                    }
                    else if (resultadoOperacion == -2)
                    {
                        VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error al enviar el correo",
                            "Ocurrió un error al enviar el correo, por favor inténtelo más tarde");
                    }
                    else
                    {
                        VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, Constantes.TituloExcepcionBD, Constantes.ContenidoExcepcionBD);
                    }
                }
                catch (Exception ex) when (ex is EndpointNotFoundException || ex is TimeoutException || ex is CommunicationException)
                {
                    LoggerManager.Error($"Excepción: {ex.Message}\nTraza: {ex.StackTrace}");
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError,
                        Constantes.TituloExcepcionServidor, Constantes.ContenidoExcepcionServidor);
                }
                finally
                {
                    if (proxyRestablecimientoCuenta.State == CommunicationState.Opened)
                    {
                        proxyRestablecimientoCuenta.Close();
                    }
                }
            }
            else
            {
                txtb_Correo.BorderBrush = Brushes.Red;
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia, "Datos incorrectos",
                    "Por favor, verifique que el correo sea correcto");
            }
        }

        private int ValidarCorreoExistente()
        {
            var proxyAcceso = new AccesoManejadorClient();
            int resultadoOperacion = -1;

            try
            {
                resultadoOperacion = proxyAcceso.VerificarCorreoExistente(txtb_Correo.Text);
                if (resultadoOperacion == 0)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia, "Correo no registrado",
                        "El correo proporcionado no se encuentra registrado en el sistema, favor de verificarlo.");
                }
                else if (resultadoOperacion == -1)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, Constantes.TituloExcepcionBD, Constantes.ContenidoExcepcionBD);
                }
            }
            catch (Exception ex) when (ex is EndpointNotFoundException || ex is TimeoutException || ex is CommunicationException)
            {
                LoggerManager.Error($"Excepción: {ex.Message}\nTraza: {ex.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError,
                    Constantes.TituloExcepcionServidor, Constantes.ContenidoExcepcionServidor);
            }
            finally
            {
                if (proxyAcceso.State == CommunicationState.Opened)
                {
                    proxyAcceso.Close();
                }
            }

            return resultadoOperacion;
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            var ventana = Window.GetWindow(this);
            ventana?.Close();
        }
    }
}
