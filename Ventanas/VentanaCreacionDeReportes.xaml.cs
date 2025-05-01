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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClienteBibliotecaElSaber.Ventanas
{
    /// <summary>
    /// Lógica de interacción para VentanaCreacionDeReportes.xaml
    /// </summary>
    public partial class VentanaCreacionDeReportes : Window
    {
        public VentanaCreacionDeReportes()
        {
            InitializeComponent();
            dtp_FechaBusqueda.SelectedDate = DateTime.Now;  
            dtp_FechaBusqueda.DisplayDateEnd = DateTime.Today;
            dtp_FechaBusqueda.DisplayDateStart = new DateTime(2022, 1, 1);
            dtp_FechaFinBusqueda.SelectedDate = DateTime.Now;
            dtp_FechaFinBusqueda.DisplayDateEnd = DateTime.Today;
            dtp_FechaFinBusqueda.DisplayDateStart = new DateTime(2022, 1, 1);
            dtp_FechaBusqueda.Visibility = Visibility.Collapsed;
            dtp_FechaFinBusqueda.Visibility = Visibility.Collapsed;
            btn_cancelarReporteLibrosMasPrestados.Visibility = Visibility.Collapsed;
            btn_crearReporteLibrosMasPrestados.Visibility = Visibility.Collapsed;
        }

        private void SeleccionarCriterioBusqueda(object sender, RoutedEventArgs e)
        {
           RadioButton seleccionado = sender as RadioButton;    
           if(seleccionado != null)
           {
                switch(seleccionado.Tag.ToString())
                {
                    case "Dia":
                        dtp_FechaBusqueda.Visibility = Visibility.Visible;
                        dtp_FechaFinBusqueda.Visibility = Visibility.Collapsed;
                        break;
                    case "Semana":
                        dtp_FechaBusqueda.Visibility= Visibility.Visible;
                        dtp_FechaFinBusqueda.Visibility = Visibility.Visible;
                        break;
                    case "Mes":
                        dtp_FechaBusqueda.Visibility = Visibility.Visible;
                        dtp_FechaFinBusqueda.Visibility = Visibility.Visible;
                        break;
                    case "Anio":
                        dtp_FechaBusqueda.Visibility = Visibility.Visible;
                        dtp_FechaFinBusqueda.Visibility = Visibility.Visible;
                        break;
                }
           }
            btn_cancelarReporteLibrosMasPrestados.Visibility = Visibility.Visible;
            btn_crearReporteLibrosMasPrestados.Visibility = Visibility.Visible;
        }

        private void btn_GenerarReportePrestamosPendientes_Click(object sender, RoutedEventArgs e)
        {
            ServidorElSaber.ReporteSocioConPrestamosPendientesClient socioConPrestamosPendientesClient = new ServidorElSaber.ReporteSocioConPrestamosPendientesClient();
            try
            {
                byte[] reportePrestamosPendientes = socioConPrestamosPendientesClient.GenerarReporteSocioConPrestamosPendientes();
                if (reportePrestamosPendientes.Length == 255)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error en la conexión a la base de datos", "No se ha podido establecer conexión con la base de datos.");
                }
                else if (reportePrestamosPendientes.Length > 0)
                {
                    string rutaDescargas = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                    string rutaArchivo = System.IO.Path.Combine(rutaDescargas, "ReportePrestamosPendientes.pdf");
                    System.IO.File.WriteAllBytes(rutaArchivo, reportePrestamosPendientes);
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoExito, "Informe generado", "Se ha generado el reporte de préstamos pendientes se ha generado de manera exitosa, podrá encontrarlo disponible en la carpeta de descargas de este dispositivo.");
                }
                else
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Sin prestamos pendientes", "No se encuentra ningún prestamo pendiente.");
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
        }


        private void btn_crearReporteInventarioLibros_Click(object sender, RoutedEventArgs e)
        {
            ServidorElSaber.ReporteInventarioLibroManejadorClient reporteInventarioLibroManejadorClient = new ServidorElSaber.ReporteInventarioLibroManejadorClient();
            try
            {
                byte[] reporteInventarioLibro =  reporteInventarioLibroManejadorClient.ObtenerReporteInventarioLibros();
                if(reporteInventarioLibro.Length == 255)
                {
                     VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error en la conexión a la base de datos", "No se ha podido establecer conexión con la base de datos.");
                }
                else if(reporteInventarioLibro.Length > 0)
                {
                     string rutaDescargas = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                     string rutaArchivo = System.IO.Path.Combine(rutaDescargas, "ReporteInventarioLibros.pdf");
                     System.IO.File.WriteAllBytes(rutaArchivo, reporteInventarioLibro);
                     VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoExito, "Informe generado", "Se ha generado el reporte inventario de libros de manera exitosa, podrá encontrarlo disponible en la carpeta de descargas de este dispositivo.");
                }
                else
                {
                     VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Sin Libros registrados", "No se encuentra con libros registrados en la base de datos");
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
        }

        private void btn_crearReporteLibrosMasPrestados_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarFechasReporteLibrosMasPrestados())
            {
                ObtenerReporteLibrosMasPrestados();
            }
            else
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Fechas inválidas", "Verifique que las fechas sean correctas y que la fecha de inicio de busqueda sea menor o igual a la fecha final de búsqueda.");
            }
        }

        private void ObtenerReporteLibrosMasPrestados()
        {
            ServidorElSaber.ReporteLibroMasPrestadoManejadorClient reporteLibroMasPrestadoManejadorClient = new ServidorElSaber.ReporteLibroMasPrestadoManejadorClient();
            try
            {
                DateTime? fechaInicio = dtp_FechaBusqueda.SelectedDate;
                DateTime? fechaFin = dtp_FechaFinBusqueda.SelectedDate;
                byte[] reporteInventarioLibro = reporteLibroMasPrestadoManejadorClient.ObtenerReporteLibrosMasPrestado(fechaInicio.Value.ToString("yyyy-MM-dd"),fechaFin.Value.ToString("yyyy-MM-dd"));
                if (reporteInventarioLibro.Length == 255)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error en la conexión a la base de datos", "No se ha podido establecer conexión con la base de datos.");
                }
                else if (reporteInventarioLibro.Length > 0)
                {
                    string rutaDescargas = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                    string rutaArchivo = System.IO.Path.Combine(rutaDescargas, "ReporteLibrosMasPrestados.pdf");
                    System.IO.File.WriteAllBytes(rutaArchivo, reporteInventarioLibro);
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoExito, "Informe generado", "Se ha generado el reporte de libros más prestados de manera exitosa, podrá encontrarlo disponible en la carpeta de descargas de este dispositivo.");
                }
                else
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Sin prestamos encontrados", "No se encuentra ningún préstamo realizado en las fechas seleccionadas.");
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
        }

        private void btn_crearReporteMultas_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarFechasReportesMulta())
            {
                ObtenerReporteMultasPagadas();
            }
            else{
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Fechas inválidas", "Verifique que las fechas sean correctas y que la fecha de inicio de busqueda sea menor o igual a la fecha final de búsqueda.");
            }
        }

        private void ObtenerReporteMultasPagadas()
        {
            ServidorElSaber.ReporteMultasPagadasClient reporteMultasPagadasClient = new ServidorElSaber.ReporteMultasPagadasClient();
            try
            {
                DateTime? fechaInicio = dtp_FechaBusquedaMulta.SelectedDate;
                DateTime? fechaFin = dtp_FechaFinBusquedaMulta.SelectedDate;
                byte[] reporteInventarioLibro = reporteMultasPagadasClient.ObtenerReporteMultasPagadasEnFechas(fechaInicio.Value.ToString("yyyy-MM-dd"), fechaFin.Value.ToString("yyyy-MM-dd"));
                if (reporteInventarioLibro.Length == 255)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error en la conexión a la base de datos", "No se ha podido establecer conexión con la base de datos.");
                }
                else if (reporteInventarioLibro.Length > 0)
                {
                    string rutaDescargas = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                    string rutaArchivo = System.IO.Path.Combine(rutaDescargas, "ReporteMultasPagadas.pdf");
                    System.IO.File.WriteAllBytes(rutaArchivo, reporteInventarioLibro);
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoExito, "Informe generado", "Se ha generado el reporte de las multas pagadas de manera exitosa, podrá encontrarlo disponible en la carpeta de descargas de este dispositivo.");
                }
                else
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Sin multas encontradas", "No se encuentra ninguna multa pagada en las fechas seleccionadas.");
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
        }

        private bool ValidarFechasReportesMulta()
        {
            DateTime? fechaInicioBusqueda = dtp_FechaBusquedaMulta.SelectedDate;
            DateTime? fechaFinBusqueda = dtp_FechaFinBusquedaMulta.SelectedDate;
            bool fechaInicioValidada = Validador.ValidarFechas(fechaInicioBusqueda.Value.ToString("yyyy-MM-dd"));
            bool fechaFinValidad = Validador.ValidarFechas(fechaFinBusqueda.Value.ToString("yyyy-MM-dd"));
            return fechaInicioValidada && fechaFinValidad && fechaInicioBusqueda.Value < fechaFinBusqueda.Value;
        }

        private bool ValidarFechasReporteLibrosMasPrestados()
        {
            DateTime? fechaInicioSeleccionada = dtp_FechaBusqueda.SelectedDate;
            DateTime? fechaFinSeleccionada = dtp_FechaFinBusqueda.SelectedDate;
            bool fechaInicioValidada = Validador.ValidarFechas(fechaInicioSeleccionada.Value.ToString("yyyy-MM-dd"));
            bool fechaFinValidada = Validador.ValidarFechas(fechaFinSeleccionada.Value.ToString("yyyy-MM-dd"));
            return fechaInicioValidada && fechaFinValidada && fechaInicioSeleccionada.Value <= fechaFinSeleccionada.Value;
        }

        private void Dp_FechaSeleccionada_Loaded(object sender, RoutedEventArgs e)
        {
            if (dtp_FechaBusqueda.Template.FindName("PART_TextBox", dtp_FechaBusqueda) is DatePickerTextBox textBox)
            {
                textBox.Text = "Seleccione fecha de inicio"; 
            }
        }

        private void Dp_FechaFinSeleccionada_Loaded(object sender, RoutedEventArgs e)
        {
            if (dtp_FechaBusqueda.Template.FindName("PART_TextBox", dtp_FechaFinBusqueda) is DatePickerTextBox textBox)
            {
                textBox.Text = "Seleccione fecha de fin";
            }
        }

        private void BotonReporteInventarioLibro_Click(object sender, RoutedEventArgs e)
        {
            OcultarBordes();
            brd_InventarioLibros.Visibility = Visibility.Visible;
        }

        private void BotonReporteLibrosMasPrestados_Click(object sender, RoutedEventArgs e)
        {
            OcultarBordes();
            brd_LibrosMasPrestados.Visibility = Visibility.Visible;
        }

        private void btn_CancelarReportePrestamosPendientes_Click(object sender, RoutedEventArgs e)
        {
            OcultarBordes();
            img_logo.Visibility = Visibility.Visible;
        }

        private void OcultarBordes()
        {
            img_logo.Visibility = Visibility.Collapsed;
            brd_InventarioLibros.Visibility = Visibility.Collapsed;
            brd_LibrosMasPrestados.Visibility = Visibility.Collapsed;
            brd_PrestamosPendientes.Visibility = Visibility.Collapsed;
            brd_MultasPagadas.Visibility = Visibility.Collapsed;
        }

        private void btn_cancelarReporteLibrosMasPrestados_Click(object sender, RoutedEventArgs e)
        {
            OcultarBordes();
            img_logo.Visibility = Visibility.Visible;
            btn_cancelarReporteLibrosMasPrestados.Visibility = Visibility.Collapsed;
            btn_crearReporteLibrosMasPrestados.Visibility = Visibility.Collapsed;
            dtp_FechaBusqueda.Visibility = Visibility.Collapsed;
            dtp_FechaFinBusqueda.Visibility = Visibility.Collapsed;
        }

        private void btn_cancelarCreacionReporteInventario_Click(object sender, RoutedEventArgs e)
        {
            OcultarBordes();
            img_logo.Visibility = Visibility.Visible;
        }

        private void BotonRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_PrestamosPendientes_Click(object sender, RoutedEventArgs e)
        {
            OcultarBordes();
            brd_PrestamosPendientes.Visibility = Visibility.Visible;
        }

        private void btn_cancelarReporteMultas_Click(object sender, RoutedEventArgs e)
        {
            OcultarBordes();
            img_logo.Visibility = Visibility.Visible;
        }

        private void BotonReporteMultas_Click(object sender, RoutedEventArgs e)
        {
            OcultarBordes();
            brd_MultasPagadas.Visibility = Visibility.Visible;
        }
    }
}
