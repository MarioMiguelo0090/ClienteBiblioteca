using ClienteBibliotecaElSaber.ServidorElSaber;
using ClienteBibliotecaElSaber.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Proxies;
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
    /// Lógica de interacción para PaginaValidacionCodigo.xaml
    /// </summary>
    public partial class PaginaValidacionCodigo : Page
    {
        private string _correo;
        public PaginaValidacionCodigo(string correo)
        {
            InitializeComponent();
            _correo = correo;
        }

        private void Verificar_Click(object sender, RoutedEventArgs e)
        {
            txtb_Codigo.BorderBrush = Brushes.Transparent;
            bool resultadoOperacion = false;

            if (!string.IsNullOrWhiteSpace(txtb_Codigo.Text))
            {
                var proxyRestablecimientoCuenta = new RestablecimientoCuentaManejadorClient();

                try
                {
                    resultadoOperacion = proxyRestablecimientoCuenta.VerificarCodigoDeVerificacion(_correo, txtb_Codigo.Text);
                    if (resultadoOperacion)
                    {
                        int resultado = proxyRestablecimientoCuenta.RestablecimientoDeContrasenia(_correo);
                        if (resultado == 1)
                        {
                            VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoExito, "Contraseña reestablecida",
                                "Estimado usuario, su nueva contraseña ha sido enviada al correo de manera correcta.");
                            var ventana = Window.GetWindow(this);
                            ventana.Close();
                        }
                        else if (resultado == -2)
                        {
                            VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error al enviar el correo",
                                "Ocurrió un error al enviar el correo, por favor inténtelo más tarde");
                        }
                        else
                        {
                            VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, Constantes.TituloExcepcionBD, Constantes.ContenidoExcepcionBD);
                        }
                    }
                    else
                    {
                        VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia, "Código incorrecto",
                            "El código proporcionado es incorrecto, favor de verificarlo.");
                        txtb_Codigo.Text = "";
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
                txtb_Codigo.BorderBrush = Brushes.Transparent;
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia, "Campo vacío",
                    "Por favor, ingrese el código para su verificación");
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            var ventana = Window.GetWindow(this);
            ventana?.Close();
        }
    }
}
