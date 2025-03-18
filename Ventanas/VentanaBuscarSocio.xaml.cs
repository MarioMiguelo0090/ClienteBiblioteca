using ClienteBibliotecaElSaber.ServidorElSaber;
using ClienteBibliotecaElSaber.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace ClienteBibliotecaElSaber.Ventanas
{
    public partial class VentanaBuscarSocio : Window
    {
        public VentanaBuscarSocio()
        {
            InitializeComponent();
        }

        private void Buscar_Click(object sender, RoutedEventArgs e)
        {
            lw_socios.Items.Clear();
            txb_Busqueda.BorderBrush = Brushes.White;
            if (ValidarDatosIngresados())
            {
                string campoDeBusqueda = txb_Busqueda.Text;
                bool esSoloNumeros = campoDeBusqueda.All(char.IsDigit);
                if (esSoloNumeros)
                {
                    BuscarSocioPorNumeroDeSocio();
                }
                else
                {
                    BuscarSociosPorNombre();
                }
            }
            else
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Datos incorrectos", "Por favor verifique que los datos ingresados sean los correctos.");
                ventanaEmergente.ShowDialog();
            }
        }

        private void Buscar_Enter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Buscar_Click(sender, e);
            }
        }

        private void Detalles_Click(object sender, RoutedEventArgs e)
        {
            Button botonPresionado = sender as Button;
            SocioDatos socioSeleccionado = botonPresionado.DataContext as SocioDatos;
            if(socioSeleccionado != null)
            {
                SocioBinding socio = new SocioBinding()
                {
                    direccion = socioSeleccionado.direccion,
                    nombre = socioSeleccionado.nombre,
                    primerApellido = socioSeleccionado.primerApellido,
                    segundoApellido = socioSeleccionado.segundoApellido,
                    telefono = socioSeleccionado.telefono,
                    numeroDeSocio = socioSeleccionado.numeroSocio,
                    estado = socioSeleccionado.estado,
                    fechaDeInscripcion = socioSeleccionado.fechaDeInscripcion,
                    fechaDeNacimiento = socioSeleccionado.fechaDeNacimiento
                };
                VerDetallesDeSocio(socio);
            }
            else
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error al obtener", "No se han podido obtener datos del socio seleccionado.");
                ventanaEmergente.ShowDialog();
            }
        }

        private void VerDetallesDeSocio(SocioBinding socio)
        {
            this.Hide();
            VentanaDetallesSocio ventanaDetallesSocio = new VentanaDetallesSocio(socio);
            ventanaDetallesSocio.ShowDialog();
            this.Show();
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Regresar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BuscarSocioPorNumeroDeSocio()
        {
            try
            {
                SocioManejadorClient socioManejadorClient = new SocioManejadorClient();
                SocioBinding socioObtenido = socioManejadorClient.ConsultarSocioPorNumeroDeSocio(int.Parse(txb_Busqueda.Text));
                if(socioObtenido.numeroDeSocio == -1)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error en la conexión a la base de datos", "Se ha perdido la conexión a la base de datos");
                    ventanaEmergente.ShowDialog();
                }
                else if(socioObtenido.numeroDeSocio == 0)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Resultados no encontrados", "El numero de socio que desea buscar, no se encuentra registrado en la base de datos.");
                    ventanaEmergente.ShowDialog();
                }
                else
                {
                    SocioDatos socio = new SocioDatos()
                    {
                        numeroSocio = socioObtenido.numeroDeSocio,
                        nombre = socioObtenido.nombre,
                        primerApellido = socioObtenido.primerApellido,
                        segundoApellido = socioObtenido.segundoApellido,
                        fechaDeInscripcion = socioObtenido.fechaDeInscripcion,
                        fechaDeNacimiento = socioObtenido.fechaDeNacimiento,
                        telefono = socioObtenido.telefono,
                        estado = socioObtenido.estado,
                        direccion = socioObtenido.direccion,
                    };
                    lw_socios.Items.Add(socio);
                }
            }
            catch (EndpointNotFoundException endpointNotFoundException)
            {
                LoggerManager.Error($"Excepción de EndpointNotFoundException: {endpointNotFoundException.Message}." +
                                    $"\nTraza: {endpointNotFoundException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Punto de conexión fallido", "No se ha podido establecer conexión con el servidor.");
                ventanaEmergente.ShowDialog();
            }
            catch (TimeoutException timeoutException)
            {
                LoggerManager.Error($"Excepción de TimeoutException: {timeoutException.Message}." +
                                    $"\nTraza: {timeoutException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Tiempo de espera agotado", "El tiempo de espera ha caducado, inténtelo de nuevo.");
                ventanaEmergente.ShowDialog();
            }
            catch (CommunicationException communicationException)
            {
                LoggerManager.Error($"Excepción de CommunicationException: {communicationException.Message}." +
                                    $"\nTraza: {communicationException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Comunicacion fallida", "La comunicacion con el servidor se ha perdido, por favor verifique su conexión a internet.");
                ventanaEmergente.ShowDialog();
            }
        }

        private void BuscarSociosPorNombre()
        {
            try
            {
                SocioManejadorClient socioManejadorClient = new SocioManejadorClient();
                List<SocioBinding> sociosObtenido = socioManejadorClient.ConsultarSociosPorNombre(txb_Busqueda.Text).ToList();
                if (sociosObtenido[0].numeroDeSocio == -1)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error en la conexión a la base de datos", "Se ha perdido la conexión a la base de datos");
                    ventanaEmergente.ShowDialog();
                }
                else if (sociosObtenido[0].numeroDeSocio == 0)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Resultados no encontrados", "El nombre(s) de socio(s) que desea buscar, no se encuentra registrado en la base de datos.");
                    ventanaEmergente.ShowDialog();
                }
                else
                {
                    CargarSociosEncontrados(sociosObtenido);
                }
                
            }
            catch (EndpointNotFoundException endpointNotFoundException)
            {
                LoggerManager.Error($"Excepción de EndpointNotFoundException: {endpointNotFoundException.Message}." +
                                    $"\nTraza: {endpointNotFoundException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Punto de conexión fallido", "No se ha podido establecer conexión con el servidor.");
                ventanaEmergente.ShowDialog();
            }
            catch (TimeoutException timeoutException)
            {
                LoggerManager.Error($"Excepción de TimeoutException: {timeoutException.Message}." +
                                    $"\nTraza: {timeoutException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Tiempo de espera agotado", "El tiempo de espera ha caducado, inténtelo de nuevo.");
                ventanaEmergente.ShowDialog();
            }
            catch (CommunicationException communicationException)
            {
                LoggerManager.Error($"Excepción de CommunicationException: {communicationException.Message}." +
                                    $"\nTraza: {communicationException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Comunicacion fallida", "La comunicacion con el servidor se ha perdido, por favor verifique su conexión a internet.");
                ventanaEmergente.ShowDialog();
            }
        }

        private void CargarSociosEncontrados(List<SocioBinding> socios)
        {
            foreach(var socioObtenido in socios)
            {
                SocioDatos socio = new SocioDatos()
                {
                    numeroSocio = socioObtenido.numeroDeSocio,
                    nombre = socioObtenido.nombre,
                    primerApellido = socioObtenido.primerApellido,
                    segundoApellido = socioObtenido.segundoApellido,
                    fechaDeInscripcion = socioObtenido.fechaDeInscripcion,
                    fechaDeNacimiento = socioObtenido.fechaDeNacimiento,
                    telefono = socioObtenido.telefono,
                    estado = socioObtenido.estado,
                    direccion = socioObtenido.direccion,
                };
                lw_socios.Items.Add(socio);
            }
        }

        private bool ValidarDatosIngresados()
        {
            bool validacionDatoBusqueda = Validador.ValidarNombre(txb_Busqueda.Text);

            if (!validacionDatoBusqueda)
            {
                validacionDatoBusqueda = Validador.ValidarNumeroDeSocio(txb_Busqueda.Text);
                if (!validacionDatoBusqueda)
                {
                    txb_Busqueda.BorderBrush = Brushes.Red;
                }
            }
            return validacionDatoBusqueda;
        }

        private sealed class SocioDatos
        {
            public int numeroSocio { get; set; }
            public string nombre { get; set; }
            public string primerApellido { get; set; }
            public string segundoApellido { get; set; }
            public string telefono { get; set; }
            public string estado { get; set; }
            public DateTime fechaDeInscripcion { get; set; }
            public DateTime fechaDeNacimiento { get; set; }

            public DireccionBinding direccion { get; set; }
        }
    }
}
