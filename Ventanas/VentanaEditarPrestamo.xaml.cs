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
using System.Windows.Shapes;

namespace ClienteBibliotecaElSaber.Ventanas
{
    /// <summary>
    /// Lógica de interacción para VentanaEditarPrestamo.xaml
    /// </summary>
    public partial class VentanaEditarPrestamo : Window
    {
        private PrestamoBinding prestamoActual;
        public VentanaEditarPrestamo(PrestamoBinding prestamo)
        {
            InitializeComponent();
            prestamoActual= prestamo;
            CargarDatosPrestamo();
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarEdicion())
            {
                EditarPrestamo();
            }
            else 
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia
                , "Datos inválidos", "Los datos que ha ingresado no son los correctos, inténtelo de nuevo.");
            }
        }

        private void EditarPrestamo() 
        {
            DateTime fechaDevolucionEditada = DateTime.Parse(dtp_FechaDevolucion.Text);
            PrestamoManejadorClient prestamoManejador = new PrestamoManejadorClient();
            int resultadoEdicion=Constantes.ValorPorDefecto;
            try 
            {
                resultadoEdicion=prestamoManejador.EditarPrestamoPorId(prestamoActual.IdPrestamo, txtb_Notas.Text, fechaDevolucionEditada);
            }
            catch (TimeoutException excepcionTiempo)
            {
                LoggerManager.Error($"Excepcion: {excepcionTiempo.Message}\nTraza {excepcionTiempo.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");
                RegresaVentanaMenuPrincipal();
            }
            catch (EndpointNotFoundException puntoNoEncontrado)
            {
                LoggerManager.Error($"Excepcion: {puntoNoEncontrado.Message}\nTraza {puntoNoEncontrado.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");
                RegresaVentanaMenuPrincipal();
            }
            catch (CommunicationException excepcionComunicacion)
            {
                LoggerManager.Error($"Excepcion: {excepcionComunicacion.Message}\nTraza {excepcionComunicacion.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");
                RegresaVentanaMenuPrincipal();
            }
            if (resultadoEdicion == Constantes.ErrorEnLaOperacion)
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de base de datos", "No se ha podido establecer conexión a la base de datos, inténtelo de nuevo más tarde.");
                RegresaVentanaMenuPrincipal();
            }
            else if (resultadoEdicion == Constantes.OperacionExitosa) 
            {
                VentanaEmergente ventanaEmergenteExito = new VentanaEmergente(Constantes.TipoExito,
                "Edición correcta", "Los datos han sido editados correctamente.");
                RegresaVentanaMenuPrincipal();
            }
        }

        private void RegresaVentanaMenuPrincipal()
        {
            this.Close();
        }

        private bool ValidarEdicion() 
        {
            return ValidadorPrestamo.ValidarNotas(txtb_Notas.Text) && dtp_FechaDevolucion.Text!=null;
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CargarDatosPrestamo() 
        {
            txtb_Titulo.Text = prestamoActual.TituloLibro;
            txtb_NoSocio.Text = prestamoActual.FK_IdSocio.ToString();
            dp_FechaInicio.Text = prestamoActual.FechaPrestamo.ToString();
            txtb_Notas.Text = prestamoActual.Nota.ToString();
            dtp_FechaDevolucion.Text = prestamoActual.FechaDevolucionEsperada.ToString();
        }
    }
}
