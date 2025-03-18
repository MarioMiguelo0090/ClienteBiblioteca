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
    /// Lógica de interacción para VentanaDevolucion.xaml
    /// </summary>
    public partial class VentanaDevolucion : Window
    {
        public VentanaDevolucion()
        {
            InitializeComponent();            
        }

        private void MyTextBox_KeyDown(object sender, KeyEventArgs argumento)
        {
            ReestablecerColores();
            if (argumento.Key == Key.Enter)
            {
                if (ValidarNumeroSocio())
                {
                    BuscarPrestamos();
                }
                else
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia
                    , "Datos inválidos", "Los datos que ha ingresado no son los correctos, inténtelo de nuevo.");
                    ventanaEmergente.ShowDialog();
                }

            }
        }

        private bool ValidarNumeroSocio()
        {
            bool validacionNumeroSocio = ValidadorPrestamo.ValidarNumeroSocio(Txb_NumeroSocio.Text);
            if (!validacionNumeroSocio)
            {
                Txb_NumeroSocio.BorderBrush = Brushes.Red;
            }
            return validacionNumeroSocio;
        }

        private void BuscarPrestamos()
        {
            PrestamoManejadorClient prestamoManejador = new PrestamoManejadorClient();
            List<PrestamoBinding> prestamosObtenidos = new List<PrestamoBinding>();
            try
            {
                prestamosObtenidos = prestamoManejador.ObtenerPrestamosActivosYVencidosPorNumeroSocio(int.Parse(Txb_NumeroSocio.Text)).ToList();
            }
            catch (TimeoutException excepcionTiempo)
            {
                LoggerManager.Error($"Excepcion: {excepcionTiempo.Message}\nTraza {excepcionTiempo.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");
                ventanaEmergente.ShowDialog();
            }
            catch (EndpointNotFoundException puntoNoEncontrado)
            {
                LoggerManager.Error($"Excepcion: {puntoNoEncontrado.Message}\nTraza {puntoNoEncontrado.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");
                ventanaEmergente.ShowDialog();
            }
            catch (CommunicationException excecpionComunicacion)
            {
                LoggerManager.Error($"Excepcion: {excecpionComunicacion.Message}\nTraza {excecpionComunicacion.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");
                ventanaEmergente.ShowDialog();
            }
            if (prestamosObtenidos.Count == Constantes.ValorPorDefecto)
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia
                    , "Socio sin préstamos", "El socio solicitado no tiene ningún préstamo activo.");
                ventanaEmergente.ShowDialog();
            }
            else
            {
                PrestamoBinding prestamoObtenido = prestamosObtenidos.First();
                if (prestamoObtenido.IdPrestamo == Constantes.ErrorEnLaOperacion)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de base de datos", "No se ha podido establecer conexión a la base de datos, inténtelo de nuevo más tarde.");
                    ventanaEmergente.ShowDialog();
                }
                else if (prestamoObtenido.IdPrestamo > Constantes.ValorPorDefecto)
                {
                    var listaVisual = prestamosObtenidos.Select(entidad => new PrestamoViewModel
                    {
                        TituloLibro = ObtenerTituloLibro(entidad.FK_IdLibro),
                        FechaInicio = entidad.FechaPrestamo.ToString("dd/MM/yyyy"),
                        FechaDevolucion = entidad.FechaDevolucionEsperada.ToString("dd/MM/yyyy"),
                        EstadoPrestamo = entidad.Estado,
                    }).ToList();
                    LvPrestamos.ItemsSource = listaVisual;
                }
            }
        }

        private void ReestablecerColores()
        {
            Txb_NumeroSocio.BorderBrush = Brushes.White;
        }

        private string ObtenerTituloLibro(int idLibro)
        {
            LibroManejadorClient libroManejador = new LibroManejadorClient();
            return libroManejador.ObtenerTituloPorIdLibro(idLibro);
        }
    }

    public class PrestamoViewModel
    {
        public string TituloLibro { get; set; }
        public string FechaInicio { get; set; }
        public string FechaDevolucion { get; set; }

        public string EstadoPrestamo { get; set; }
    }

}
