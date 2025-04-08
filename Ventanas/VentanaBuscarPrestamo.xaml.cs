using ClienteBibliotecaElSaber.ServidorElSaber;
using ClienteBibliotecaElSaber.Utilidades;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class VentanaBuscarPrestamo : Window
    {
        public ObservableCollection<PrestamoBinding> Prestamos { get; set; }
        public VentanaBuscarPrestamo()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            if (lw_prestamos.SelectedItem is PrestamoBinding prestamoSeleccionado)
            {
                this.Hide();
                VentanaEditarPrestamo ventanaEditarPrestamo = new VentanaEditarPrestamo(prestamoSeleccionado);
                ventanaEditarPrestamo.ShowDialog();
                this.Show();
            }                
        }

        private void ObtenerPrestamosActivos(object sender, KeyEventArgs argumento)
        {
            ReestablecerColores();
            if (argumento.Key == Key.Enter)
            {
                if (ValidarNumeroSocio())
                {
                    BuscarPrestamosPorNumeroSocio();
                }
                else if (ValidarISBN())
                {
                    BuscarPrestamosPorISBN();
                }
                else if (dtp_FechaInicio != null) 
                {
                    BuscarPrestamosPorFechaInicio();                     
                }
                else
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia
                    , "Datos inválidos", "Los datos que ha ingresado no son los correctos, inténtelo de nuevo.");
                }
            }
        }

        private void ReestablecerColores()
        {
            txtb_Buscador.BorderBrush = Brushes.White;
        }

        private bool ValidarNumeroSocio()
        {
            return ValidadorPrestamo.ValidarNumeroSocio(txtb_Buscador.Text);
        }

        private bool ValidarISBN() 
        {
            return ValidadorPrestamo.ValidarISBN(txtb_Buscador.Text);
        }

        private void BuscarPrestamosPorNumeroSocio() 
        {
            int numeroSocio = int.Parse(txtb_Buscador.Text);
            PrestamoManejadorClient prestamoManejador = new PrestamoManejadorClient();
            List<PrestamoBinding> prestamosRecuperados=new List<PrestamoBinding>();
            try 
            {
                prestamosRecuperados=prestamoManejador.RecuperarPrestamosActivosPorNumeroSocio(numeroSocio).ToList();
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
            if (prestamosRecuperados.Count == Constantes.ValorPorDefecto)
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia
                    , "Socio sin préstamos", "El socio solicitado no tiene ningún préstamo activo.");
                RegresaVentanaMenuPrincipal();
            }
            else
            {
                PrestamoBinding prestamoObtenido = prestamosRecuperados.First();
                if (prestamoObtenido.IdPrestamo == Constantes.ErrorEnLaOperacion)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de base de datos", "No se ha podido establecer conexión a la base de datos, inténtelo de nuevo más tarde.");
                    RegresaVentanaMenuPrincipal();
                }
                else if (prestamoObtenido.IdPrestamo > Constantes.ValorPorDefecto)
                {
                    foreach (var prestamo in prestamosRecuperados)
                    {
                        prestamo.TituloLibro = ObtenerTituloLibro(prestamo.FK_IdLibro);
                    }
                    Prestamos = new ObservableCollection<PrestamoBinding>(prestamosRecuperados);
                    lw_prestamos.ItemsSource = Prestamos;
                }
            }
        }

        private void RegresaVentanaMenuPrincipal()
        {
            this.Close();
        }

        private string ObtenerTituloLibro(int idLibro)
        {
            LibroManejadorClient libroManejador = new LibroManejadorClient();
            return libroManejador.ObtenerTituloPorIdLibro(idLibro);
        }

        private void BuscarPrestamosPorISBN() 
        {
            string codigoISBN = txtb_Buscador.Text;
            PrestamoManejadorClient prestamoManejador = new PrestamoManejadorClient();
            List<PrestamoBinding> prestamosRecuperados = new List<PrestamoBinding>();
            try
            {
                prestamosRecuperados = prestamoManejador.ObtenerPrestamosActivosPorISBN(codigoISBN).ToList();
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
            if (prestamosRecuperados.Count == Constantes.ValorPorDefecto)
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia
                    , "Socio sin préstamos", "El socio solicitado no tiene ningún préstamo activo.");
                RegresaVentanaMenuPrincipal();
            }
            else
            {
                PrestamoBinding prestamoObtenido = prestamosRecuperados.First();
                if (prestamoObtenido.IdPrestamo == Constantes.ErrorEnLaOperacion)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de base de datos", "No se ha podido establecer conexión a la base de datos, inténtelo de nuevo más tarde.");
                    RegresaVentanaMenuPrincipal();
                }
                else if (prestamoObtenido.IdPrestamo > Constantes.ValorPorDefecto)
                {
                    foreach (var prestamo in prestamosRecuperados)
                    {
                        prestamo.TituloLibro = ObtenerTituloLibro(prestamo.FK_IdLibro);
                    }
                    Prestamos = new ObservableCollection<PrestamoBinding>(prestamosRecuperados);
                    lw_prestamos.ItemsSource = Prestamos;
                }
            }
        }

        private void BuscarPrestamosPorFechaInicio()
        {
            DateTime fechaInicio = DateTime.Parse(dtp_FechaInicio.Text);
            PrestamoManejadorClient prestamoManejador = new PrestamoManejadorClient();
            List<PrestamoBinding> prestamosRecuperados = new List<PrestamoBinding>();
            try
            {
                prestamosRecuperados = prestamoManejador.ObtenerPrestamosActivosPorFechaInicio(fechaInicio).ToList();
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
            if (prestamosRecuperados.Count == Constantes.ValorPorDefecto)
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia
                    , "Socio sin préstamos", "El socio solicitado no tiene ningún préstamo activo.");
                RegresaVentanaMenuPrincipal();
            }
            else
            {
                PrestamoBinding prestamoObtenido = prestamosRecuperados.First();
                if (prestamoObtenido.IdPrestamo == Constantes.ErrorEnLaOperacion)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de base de datos", "No se ha podido establecer conexión a la base de datos, inténtelo de nuevo más tarde.");
                    RegresaVentanaMenuPrincipal();
                }
                else if (prestamoObtenido.IdPrestamo > Constantes.ValorPorDefecto)
                {
                    foreach (var prestamo in prestamosRecuperados)
                    {
                        prestamo.TituloLibro = ObtenerTituloLibro(prestamo.FK_IdLibro);
                    }
                    Prestamos = new ObservableCollection<PrestamoBinding>(prestamosRecuperados);
                    lw_prestamos.ItemsSource = Prestamos;
                }
            }

        }
    }
}
