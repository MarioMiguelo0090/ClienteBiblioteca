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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClienteBibliotecaElSaber.Ventanas
{
    /// <summary>
    /// Lógica de interacción para VentanaRegistroDePrestamo.xaml
    /// </summary>
    public partial class VentanaRegistroDePrestamo : Window
    {
        public VentanaRegistroDePrestamo()
        {
            InitializeComponent();
        }
        private void Buscar_Enter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
            }
        }

        private void RegistrarPrestamo(object sender, RoutedEventArgs e)
        {
            ReestablecerColores();
            if (ValidarInformacion())
            {
                int idLibro = ObtenerIdLibro();
                if (idLibro > Constantes.ValorPorDefecto)
                {
                    if (ValidarDisponibilidad(idLibro))
                    {
                        if (ValidarSocioSinPrestamosVencidos()) 
                        {
                            PrestamoBinding prestamo = new PrestamoBinding();
                            prestamo = ObtenerDatosPrestamo();
                            PrestamoManejadorClient prestamoManejador = new PrestamoManejadorClient();
                            int resultadoRegistro = Constantes.ValorPorDefecto;
                            try
                            {
                                resultadoRegistro = prestamoManejador.RegistrarNuevoPrestamo(prestamo);
                            }
                            catch (TimeoutException excepcionTiempo)
                            {
                                LoggerManager.Error($"Excepcion: {excepcionTiempo.Message}\nTraza {excepcionTiempo.StackTrace}");
                                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                                    , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");                            
                            }
                            catch (EndpointNotFoundException puntoNoEncontrado)
                            {
                                LoggerManager.Error($"Excepcion: {puntoNoEncontrado.Message}\nTraza {puntoNoEncontrado.StackTrace}");
                                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                                    , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");                              
                            }
                            catch (CommunicationException excecpionComunicacion)
                            {
                                LoggerManager.Error($"Excepcion: {excecpionComunicacion.Message}\nTraza {excecpionComunicacion.StackTrace}");
                                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                                    , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");                              
                            }
                            switch (resultadoRegistro)
                            {
                                case Constantes.ErrorEnLaOperacion:
                                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                                    , "Error de base de datos", "No se ha podido establecer conexión a la base de datos, inténtelo de nuevo más tarde.");
                                    break;
                                case Constantes.OperacionExitosa:
                                    VentanaEmergente ventanaEmergenteExito = new VentanaEmergente(Constantes.TipoExito,
                                    "Registro de préstamo exitoso", $"El préstamo ha sido registrado de manera exitosa, la fecha de devolución es {Dp_FechaDevolucion.Text}");
                                    break;
                            }
                        }                        
                    }
                }
                else if (idLibro == Constantes.ErrorEnLaOperacion)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de base de datos", "No se ha podido establecer conexión a la base de datos, inténtelo de nuevo más tarde.");
                }
                RegresaVentanaMenuPrincipal();
            }
            else 
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia
                    , "Datos inválidos", "Los datos que ha ingresado no son los correctos, inténtelo de nuevo.");
            }                                       
        }

        private void CancelarRegistro(object sender, RoutedEventArgs e)
        {
            RegresaVentanaMenuPrincipal();
        }

        private bool ValidarInformacion()
        {
            bool validacionIsbn = ValidadorPrestamo.ValidarISBN(Txb_ISBN.Text);
            bool validacionNumeroSocio = ValidadorPrestamo.ValidarNumeroSocio(Txb_NumeroSocio.Text);
            bool validacionNotas = ValidadorPrestamo.ValidarNotas(Txb_Notas.Text);
            if (!validacionIsbn)
            {
                Txb_ISBN.BorderBrush = Brushes.Red;
            }
            if (!validacionNumeroSocio) 
            {
                Txb_NumeroSocio.BorderBrush= Brushes.Red;
            }
            if (!validacionNotas) 
            {
                Txb_Notas.BorderBrush= Brushes.Red;
            }
            return validacionIsbn && validacionNumeroSocio && validacionNotas;
        }

        private PrestamoBinding ObtenerDatosPrestamo() 
        {
            PrestamoBinding prestamo = new PrestamoBinding
            {
                FK_IdSocio = int.Parse(Txb_NumeroSocio.Text),
                Nota = Txb_Notas.Text,
                FechaPrestamo = DateTime.Today,
                FechaDevolucionEsperada = Dp_FechaDevolucion.SelectedDate ?? DateTime.Today.AddDays(7),
                FK_IdLibro = ObtenerIdLibro(),
                FK_IdUsuario = SingletonBibliotecario.Instancia.Usuario.IdUsuario
            };
            return prestamo;    
        }

        private int ObtenerIdLibro() 
        {
            LibroManejadorClient libroManejador = new LibroManejadorClient();
            int idLibro=Constantes.ValorPorDefecto;
            try
            {
                idLibro = libroManejador.ObtenerIdLibroPorISBN(Txb_ISBN.Text);
            }
            catch (TimeoutException excepcionTiempo)
            {
                LoggerManager.Error($"Excepcion: {excepcionTiempo.Message}\nTraza {excepcionTiempo.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");               
            }
            catch (EndpointNotFoundException puntoNoEncontrado)
            {
                LoggerManager.Error($"Excepcion: {puntoNoEncontrado.Message}\nTraza {puntoNoEncontrado.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");              
            }
            catch (CommunicationException excecpionComunicacion) 
            {
                LoggerManager.Error($"Excepcion: {excecpionComunicacion.Message}\nTraza {excecpionComunicacion.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");              
            }
            return idLibro;
        }

        private void ReestablecerColores() 
        {
            Txb_ISBN.BorderBrush = Brushes.White;
            Txb_NumeroSocio.BorderBrush = Brushes.White;
            Txb_Notas.BorderBrush = Brushes.White;
        }

        private void RegresaVentanaMenuPrincipal() 
        {
            this.Close();
        }

        private bool ValidarDisponibilidad(int idLibro) 
        {
            bool validacion = false;
            LibroManejadorClient libroManejador=new LibroManejadorClient();
            int resultadoValidacion = 0;
            try 
            {
                resultadoValidacion = libroManejador.ValidarDisponibilidadPorIdLibro(idLibro);
            }
            catch (TimeoutException excepcionTiempo)
            {
                LoggerManager.Error($"Excepcion: {excepcionTiempo.Message}\nTraza {excepcionTiempo.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");               
            }
            catch (EndpointNotFoundException puntoNoEncontrado)
            {
                LoggerManager.Error($"Excepcion: {puntoNoEncontrado.Message}\nTraza {puntoNoEncontrado.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");              
            }
            catch (CommunicationException excecpionComunicacion)
            {
                LoggerManager.Error($"Excepcion: {excecpionComunicacion.Message}\nTraza {excecpionComunicacion.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");               
            }            
            switch (resultadoValidacion) 
            {
                case Constantes.ErrorEnLaOperacion:
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de base de datos", "No se ha podido establecer conexión a la base de datos, inténtelo de nuevo más tarde.");
                    break;
                case Constantes.ValorPorDefecto:
                    VentanaEmergente ventanaEmergenteLibroAgotado = new VentanaEmergente(Constantes.TipoError
                    , "Libros agotados", "El libro que intenta dar en préstamo esta agotado.");
                    break;
                case Constantes.OperacionExitosa:
                    validacion = true;
                    break;                
            }
            return validacion;
        }

        private bool ValidarSocioSinPrestamosVencidos() 
        {
            bool validacion = false;
            PrestamoManejadorClient prestamoManejador = new PrestamoManejadorClient();
            int validacionPrestamos = Constantes.ValorPorDefecto;
            try 
            {
                validacionPrestamos=prestamoManejador.ValidarPrestamosVencidosPorNumeroSocio(int.Parse(Txb_NumeroSocio.Text));
            }
            catch (TimeoutException excepcionTiempo)
            {
                LoggerManager.Error($"Excepcion: {excepcionTiempo.Message}\nTraza {excepcionTiempo.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");             
            }
            catch (EndpointNotFoundException puntoNoEncontrado)
            {
                LoggerManager.Error($"Excepcion: {puntoNoEncontrado.Message}\nTraza {puntoNoEncontrado.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");              
            }
            catch (CommunicationException excecpionComunicacion)
            {
                LoggerManager.Error($"Excepcion: {excecpionComunicacion.Message}\nTraza {excecpionComunicacion.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");              
            }
            switch (validacionPrestamos)
            {
                case Constantes.ErrorEnLaOperacion:
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de base de datos", "No se ha podido establecer conexión a la base de datos, inténtelo de nuevo más tarde.");
                    break;
                case Constantes.ValorPorDefecto:
                    validacion = true;
                    break;
                case Constantes.OperacionExitosa:
                    VentanaEmergente ventanaEmergenteMulta = new VentanaEmergente(Constantes.TipoAdvertencia
                    , "Multa pendiente", $"El socio con número {Txb_NumeroSocio.Text} tiene al menos una multa sin pagar, esta debe ser pagada antes de realizar otro préstamo.");
                    break;
            }
            return validacion;
        }
    }
}
