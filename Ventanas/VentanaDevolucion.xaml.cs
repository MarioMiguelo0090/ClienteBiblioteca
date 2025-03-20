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
            Txb_NumeroSocio.GotFocus += Txb_NumeroSocio_GotFocus;
            Txb_NumeroSocio.LostFocus += Txb_NumeroSocio_LostFocus;
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
                    
                }

            }
        }

        private void Txb_NumeroSocio_GotFocus(object sender, RoutedEventArgs e)
        {
            WatermarkText.Visibility = Visibility.Collapsed;
        }
        
        private void Txb_NumeroSocio_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Txb_NumeroSocio.Text))
            {
                WatermarkText.Visibility = Visibility.Visible;
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
            LvPrestamos.ItemsSource = null;
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
                
                RegresaVentanaMenuPrincipal();
            }
            catch (EndpointNotFoundException puntoNoEncontrado)
            {
                LoggerManager.Error($"Excepcion: {puntoNoEncontrado.Message}\nTraza {puntoNoEncontrado.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");
                
                RegresaVentanaMenuPrincipal();
            }
            catch (CommunicationException excecpionComunicacion)
            {
                LoggerManager.Error($"Excepcion: {excecpionComunicacion.Message}\nTraza {excecpionComunicacion.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");
                
                RegresaVentanaMenuPrincipal();
            }
            if (prestamosObtenidos.Count == Constantes.ValorPorDefecto)
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia
                    , "Socio sin préstamos", "El socio solicitado no tiene ningún préstamo activo.");
                
                RegresaVentanaMenuPrincipal();
            }
            else
            {
                PrestamoBinding prestamoObtenido = prestamosObtenidos.First();
                if (prestamoObtenido.IdPrestamo == Constantes.ErrorEnLaOperacion)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de base de datos", "No se ha podido establecer conexión a la base de datos, inténtelo de nuevo más tarde.");
                    
                    RegresaVentanaMenuPrincipal();
                }
                else if (prestamoObtenido.IdPrestamo > Constantes.ValorPorDefecto)
                {
                    var listaVisual = prestamosObtenidos.Select(entidad => new PrestamoViewModel
                    {
                        TituloLibro = ObtenerTituloLibro(entidad.FK_IdLibro),
                        FechaInicio = entidad.FechaPrestamo.ToString("dd/MM/yyyy"),
                        FechaDevolucion = entidad.FechaDevolucionEsperada.ToString("dd/MM/yyyy"),
                        EstadoPrestamo = entidad.Estado,
                        IdPrestamo=entidad.IdPrestamo,
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

        private void RegistrarDevolucion(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is int idPrestamo)
            {
                PrestamoViewModel prestamoSeleccionado = LvPrestamos.ItemsSource
               .Cast<PrestamoViewModel>()
               .FirstOrDefault(entidad => entidad.IdPrestamo == idPrestamo);
                if (prestamoSeleccionado != null)
                {
                    if (prestamoSeleccionado.EstadoPrestamo == Enumeradores.Prestamo.Vencido.ToString())
                    {
                        VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia,
                            "Multa generada", "El préstamo ha generado una multa que debe ser pagada.");
                        
                    }
                    else
                    {
                        VentanaRegistroDeDevolucion ventanaRegistroDeDevolucion = new VentanaRegistroDeDevolucion(idPrestamo);
                        ventanaRegistroDeDevolucion.ShowDialog();
                    }
                }    
            }
        }

        private void RegresaVentanaMenuPrincipal()
        {
            /*
            this.Hide();
            VentanaMenuPrincipalBibliotecario ventanaMenuPrincipalBibliotecario = new VentanaMenuPrincipalBibliotecario();
            ventanaMenuPrincipalBibliotecario.ShowDialog();
            this.Show();
            */
            this.Close();
        }

        private void CancelarDevolucion(object sender, RoutedEventArgs e)
        {
            RegresaVentanaMenuPrincipal();
        }
    }

    public class PrestamoViewModel
    {
        public string TituloLibro { get; set; }
        public string FechaInicio { get; set; }
        public string FechaDevolucion { get; set; }
        public string EstadoPrestamo { get; set; }
        public int IdPrestamo { get; set; }      
    }

}
