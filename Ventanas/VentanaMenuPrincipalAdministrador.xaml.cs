using ClienteBibliotecaElSaber.ServidorElSaber;
using ClienteBibliotecaElSaber.Singleton;
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
    /// Lógica de interacción para VentanaMenuPrincipalAdministrador.xaml
    /// </summary>
    public partial class VentanaMenuPrincipalAdministrador : Window
    {
        private readonly ManejadorDeCitasAutores manejadorDeCitasAutores;

        public VentanaMenuPrincipalAdministrador()
        {
            InitializeComponent();
            manejadorDeCitasAutores = new ManejadorDeCitasAutores();
            var (autor, cita) = manejadorDeCitasAutores.ObtenerCitaAleatoria();
            Txbl_Autor.Text = autor.ToString();
            Txbl_Cita.Text = cita.ToString();
            txtbl_Bienvenida.Text = $"Bienvenido, {SingletonAdministrador.Instancia.ObtenerNombreCompleto()}";
            this.Closing += EventoCierreAbrupto;
        }

        private void IrVentanaBuscarUsuario_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            VentanaBuscarUsuario ventanaBuscarUsuario = new VentanaBuscarUsuario();
            ventanaBuscarUsuario.ShowDialog();
            this.Show();
        }

        private void IrVentanaRegistrarUsuario_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            VentanaRegistroDeUsuario ventanaRegistroDeUsuario = new VentanaRegistroDeUsuario();
            ventanaRegistroDeUsuario.ShowDialog();
            this.Show();
        }

        private void IrVentanaBuscarLibro_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            VentanaBuscarLibro ventanaBuscarLibro = new VentanaBuscarLibro();
            ventanaBuscarLibro.ShowDialog();
            this.Show();
        }

        private void IrVentanaRegistrarLibro_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            VentanaRegistroLibro ventanaRegistroLibro = new VentanaRegistroLibro();
            ventanaRegistroLibro.ShowDialog();
            this.Show();
        }

        private void IrVentanaReporte_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            VentanaCreacionDeReportes ventanaCreacionDeReportes = new VentanaCreacionDeReportes();
            ventanaCreacionDeReportes.ShowDialog();
            this.Show();
        }

        private void Salir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            CerrarSesion();
            SingletonAdministrador.Instancia.CerrarSesion();
            VentanaInicioDeSesion ventanaInicioDeSesion = new VentanaInicioDeSesion();
            ventanaInicioDeSesion.Show();
            this.Close();
        }

        private void EventoCierreAbrupto(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CerrarSesion();
        }

        private void CerrarSesion()
        {
            var proxyAcceso = new AccesoManejadorClient();
            string correo = SingletonAdministrador.Instancia.Correo;

            try
            {
                if (!string.IsNullOrEmpty(correo))
                {
                    if (proxyAcceso.CerrarSesion(correo) == -1)
                    {
                        VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError,
                            "Error al cerrar la sesión", "Ocurrio un error inesperado al cerrar la sesión");
                    }
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
        }
    }
}
