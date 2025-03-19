using ClienteBibliotecaElSaber.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel;
using System.ServiceModel.Channels;
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
using ClienteBibliotecaElSaber.ServidorElSaber;
using System.Runtime.CompilerServices;
using System.Globalization;
using Microsoft.SqlServer.Server;

namespace ClienteBibliotecaElSaber.Ventanas
{
    public partial class VentanaRegistrarSocio : Window
    {
        public VentanaRegistrarSocio()
        {
            InitializeComponent();
            LimitarDatePickers();
        }

        private void LimitarDatePickers()
        {
            dp_FechaInscripcion.DisplayDateStart = new DateTime(2025, 1, 1);
            dp_FechaInscripcion.DisplayDateEnd = DateTime.Today;
            dp_FechaInscripcion.SelectedDate = DateTime.Today;
            dp_FechaNacimiento.DisplayDateStart = new DateTime(1920, 1, 1);
            dp_FechaNacimiento.DisplayDateEnd = DateTime.Today;
            dp_FechaNacimiento.SelectedDate = DateTime.Today;
        }

        private void Registrar_Click(object sender, RoutedEventArgs e)
        {
            ResetearColorDeBordes();
            if (ValidarDatosDeCampos())
            {
                int resultadoValidacionExistencia = ValidarInexistenciaDeSocio();
                if(resultadoValidacionExistencia == 1)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Socio duplicado", "El número de telefono a registrar ya ha sido registrado previamente, verifique la inexisencia del socio.");
                    
                }
                else if(resultadoValidacionExistencia == 0)
                {
                    RealizarRegistroDeSocio();
                }
            }
            else
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Datos incorrectos", "Por favor verifique que los datos ingresados sean los correctos.");
                  
            }
        }

        private void RealizarRegistroDeSocio()
        {
            try
            { 
                CultureInfo cultura = CultureInfo.InvariantCulture;
                string fechaNacimientoDatePicker = dp_FechaNacimiento.SelectedDate.Value.ToString("yyyy-MM-dd");
                string fechaInscripcionDatePicker = dp_FechaInscripcion.SelectedDate.Value.ToString("yyyy-MM-dd");
                DateTime fechaDeInscripcionObtenida;
                DateTime fechaDeNacimientoObtenida;
                bool validadorFechaInscripcion = DateTime.TryParse(fechaNacimientoDatePicker, out fechaDeInscripcionObtenida);
                bool validadorFechaNacimiento = DateTime.TryParse(fechaInscripcionDatePicker, out fechaDeNacimientoObtenida);
                if(validadorFechaInscripcion && validadorFechaNacimiento)
                {
                    SocioManejadorClient socioManejadorClient = new SocioManejadorClient();
                    DireccionBinding direccion = new DireccionBinding()
                    {
                        calle = txtb_calle.Text,
                        ciudad = txtb_ciudad.Text,
                        numero = txtb_numero.Text,
                        codigoPostal = txtb_CodigoPostal.Text
                    };
                    SocioBinding socio = new SocioBinding()
                    {
                        nombre = txtb_Nombre.Text,
                        primerApellido = txtb_PrimerApellido.Text,
                        segundoApellido = txtb_SegundoApellido.Text,
                        telefono = txtb_Telefono.Text,
                        fechaDeInscripcion = fechaDeInscripcionObtenida,
                        fechaDeNacimiento = fechaDeNacimientoObtenida,
                        direccion = direccion
                    };
                    int resultadoRegistroSocio = socioManejadorClient.RegistrarSocioEnBaseDeDatos(socio);
                    if (resultadoRegistroSocio == 1)
                    {
                        VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoExito, "Operación exitosa", "Los datos se han registrado con éxito en la base de datos.");
                        
                    }
                    else
                    {
                        VentanaEmergente ventanaEmergente = new VentanaEmergente("Error", "Error en la conexión a la base de datos", "Se ha perdido la conexión a la base de datos");
                        
                    }
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

        private int ValidarInexistenciaDeSocio()
        {
            int resultadoValidacion = -1;
            try
            {
                SocioManejadorClient socioManejadorClient = new SocioManejadorClient();
                int resultadoVerificacion = socioManejadorClient.VerificarExistenciaDeSocioEnBaseDeDatos(txtb_Telefono.Text);
                if(resultadoVerificacion == -1)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente("Error", "Error en la conexión a la base de datos", "Se ha perdido la conexión a la base de datos");
                    
                }
                resultadoValidacion = resultadoVerificacion;
            }
            catch (EndpointNotFoundException endpointNotFoundException)
            {
                LoggerManager.Error($"Excepción de EndpointNotFoundException: {endpointNotFoundException.Message}." +
                                    $"\nTraza: {endpointNotFoundException.StackTrace}."); 
                VentanaEmergente ventanaEmergente =new VentanaEmergente("Error","Punto de conexión fallido","No se ha podido establecer conexión con el servidor.");
                
            }
            catch (TimeoutException timeoutException)
            {
                LoggerManager.Error($"Excepción de TimeoutException: {timeoutException.Message}." +
                                    $"\nTraza: {timeoutException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente("Informacion", "Tiempo de espera agotado", "El tiempo de espera ha caducado, inténtelo de nuevo.");
                
            }
            catch (CommunicationException communicationException)
            {
                LoggerManager.Error($"Excepción de CommunicationException: {communicationException.Message}." +
                                    $"\nTraza: {communicationException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente("Error", "Comunicacion fallida", "La comunicacion con el servidor se ha perdido, por favor verifique su conexión a internet.");
                
            }
            return resultadoValidacion;
        }

        private void ResetearColorDeBordes()
        {
            txtb_Nombre.BorderBrush = Brushes.White;
            txtb_PrimerApellido.BorderBrush = Brushes.White;
            txtb_SegundoApellido.BorderBrush = Brushes.White;
            txtb_Telefono.BorderBrush = Brushes.White;
            txtb_calle.BorderBrush = Brushes.White;
            txtb_CodigoPostal.BorderBrush = Brushes.White;
            txtb_numero.BorderBrush = Brushes.White;
            txtb_ciudad.BorderBrush = Brushes.White;
            brd_FechaDeInscripcion.BorderBrush = Brushes.White;
            brd_FechaDeNacimiento.BorderBrush = Brushes.White;
        }

        public bool ValidarDatosDeCampos()
        {
            string fechaNacimientoDatePicker = dp_FechaNacimiento.SelectedDate.Value.ToString("yyyy-MM-dd");
            string fechaInscripcionDatePicker = dp_FechaInscripcion.SelectedDate.Value.ToString("yyyy-MM-dd");
            bool nombreValidado = Validador.ValidarNombre(txtb_Nombre.Text);
            bool primerApellidoValidado = Validador.ValidarPrimerApellido(txtb_PrimerApellido.Text);
            bool segundoApellidoValidado = Validador.ValidarSegundoApellido(txtb_SegundoApellido.Text);
            bool telefonoValido = Validador.ValidarTelefono(txtb_Telefono.Text);
            bool calleValidador = Validador.ValidarDireccion(txtb_calle.Text);
            bool codigoPostalValidado = Validador.ValidarCodigoPostal(txtb_CodigoPostal.Text);
            bool fechaDeInscripcionValidada = Validador.ValidarFechas(fechaNacimientoDatePicker.ToString());
            bool fechaDeNacimientoValidada = Validador.ValidarFechas(fechaInscripcionDatePicker.ToString());
            bool numeroCasaValidado = Validador.ValidarNumeroCasa(txtb_numero.Text);
            bool ciudadValidado = Validador.ValidarNombreCiudad(txtb_ciudad.Text);

            if (!nombreValidado)
            {
                txtb_Nombre.BorderBrush = Brushes.Red;
            }

            if (!primerApellidoValidado)
            {
                txtb_PrimerApellido.BorderBrush = Brushes.Red;
            }

            if (!segundoApellidoValidado)
            {
                txtb_SegundoApellido.BorderBrush = Brushes.Red;
            }

            if (!telefonoValido)
            {
                txtb_Telefono.BorderBrush = Brushes.Red;
            }

            if (!calleValidador)
            {
                txtb_calle.BorderBrush = Brushes.Red;
            }

            if (!codigoPostalValidado)
            {
                txtb_CodigoPostal.BorderBrush = Brushes.Red;
            }

            if (!fechaDeInscripcionValidada)
            {
                brd_FechaDeInscripcion.BorderBrush = Brushes.Red;
            }

            if (!fechaDeNacimientoValidada)
            {
                brd_FechaDeNacimiento.BorderBrush = Brushes.Red;   
            }

            if (!ciudadValidado)
            {
                txtb_ciudad.BorderBrush = Brushes.Red;
            }

            if (!numeroCasaValidado)
            {
                txtb_numero.BorderBrush = Brushes.Red;
            }

            return nombreValidado && primerApellidoValidado && segundoApellidoValidado && telefonoValido &&
                calleValidador && codigoPostalValidado && fechaDeNacimientoValidada && fechaDeInscripcionValidada
                && ciudadValidado && numeroCasaValidado;
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
