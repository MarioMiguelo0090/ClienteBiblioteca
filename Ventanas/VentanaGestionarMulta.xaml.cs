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
    public partial class VentanaGestionarMulta : Window
    {
        public ObservableCollection<MultaBinding> Multas { get; set; }
        public VentanaGestionarMulta()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Pagar_Click(object sender, RoutedEventArgs e)
        {
            Button boton = sender as Button;
            if (boton != null)
            {
                var datosMulta = boton.DataContext as DatosMulta;
                if (datosMulta != null)
                {
                    MultaManejadorClient multaManejador = new MultaManejadorClient();                   
                    try 
                    {
                        int resultadoPagoMulta=multaManejador.RegistrarPagoMultaPorIdMulta(datosMulta.IdMulta);
                        if (resultadoPagoMulta == Constantes.ErrorEnLaOperacion)
                        {
                            VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                            , "Error de base de datos", "No se ha podido establecer conexión a la base de datos, inténtelo de nuevo más tarde.");
                            RegresaVentanaMenuPrincipal();
                        }
                        else if (resultadoPagoMulta == Constantes.OperacionExitosa) 
                        {
                            VentanaEmergente ventanaEmergenteExito = new VentanaEmergente(Constantes.TipoExito,
                             "Multa pagada", "La muta ha sido pagada.");
                            RegresaVentanaMenuPrincipal();
                        }
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
                }
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool ValidarNumeroSocio()
        {
            return ValidadorPrestamo.ValidarNumeroSocio(txtb_Buscador.Text);
        }

        private void ObtenerMultas(object sender, KeyEventArgs argumento)
        {
            if (argumento.Key == Key.Enter)
            {
                if (ValidarNumeroSocio())
                {
                    BuscarMultas();
                }
                else
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia
                    , "Datos inválidos", "Los datos que ha ingresado no son los correctos, inténtelo de nuevo.");
                }
            }
        }

        private void BuscarMultas() 
        {
            MultaManejadorClient multaManejador = new MultaManejadorClient();
            List <MultaBinding> multasRecuperadas= new List <MultaBinding>();
            try 
            {
                multasRecuperadas=multaManejador.ObtenerMultasPendientesPorNumeroSocio(int.Parse(txtb_Buscador.Text)).ToList();
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
            if (multasRecuperadas.Count == 0)
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia
                    , "Sin multa", "El número de socio colocado no tiene multas activas.");
                RegresaVentanaMenuPrincipal();
            }
            else 
            {
                MultaBinding primerMulta=new MultaBinding();
                primerMulta=multasRecuperadas.First();
                if (primerMulta.idMulta == Constantes.ErrorEnLaOperacion)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                    , "Error de base de datos", "No se ha podido establecer conexión a la base de datos, inténtelo de nuevo más tarde.");
                    RegresaVentanaMenuPrincipal();
                }
                else if (primerMulta.idMulta>Constantes.ValorPorDefecto) 
                {

                    foreach (var multa in multasRecuperadas) 
                    {
                        var multaObtenida = new DatosMulta
                        {
                            IdMulta=multa.idMulta,
                            Titulo = ObtenerTituloLibro(multa.PrestamoAsociado.FK_IdLibro),
                            Estado=multa.Estado,
                            DiasAtraso=multa.MontoTotal,
                            MontoTotal=multa.MontoTotal,
                        };
                        lw_multas.Items.Add(multaObtenida);
                    }                    
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

        private sealed class DatosMulta 
        {
            public int IdMulta { get; set; }

            public string Titulo { get; set; }

            public string Estado { get; set; }

            public double DiasAtraso {  get; set; }

            public double MontoTotal { get; set; }
        }
    }
}
