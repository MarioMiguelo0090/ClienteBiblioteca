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
    public partial class VentanaEditarSocio : Window
    {
        private SocioBinding socioOriginal;

        public VentanaEditarSocio(SocioBinding socio)
        {
            InitializeComponent();
            socioOriginal = socio;
            CargarDatosDelSocio(socio);
        }

        private void CargarDatosDelSocio(SocioBinding socio)
        {
            txb_nombre.Text = socio.nombre;
            txb_primerApellido.Text = socio.primerApellido;
            txb_segundoApellido.Text = socio.segundoApellido;
            txb_telefono.Text = socio.telefono;
            txb_direccion.Text = socio.direccion.calle;
            txb_numero.Text = socio.direccion.numero;
            txb_codigoPostal.Text = socio.direccion.codigoPostal;
            txb_ciudad.Text = socio.direccion.ciudad;
        }

        private void ResetearBordes()
        {
            txb_nombre.BorderBrush = Brushes.White;
            txb_primerApellido.BorderBrush = Brushes.White;
            txb_segundoApellido.BorderBrush = Brushes.White;
            txb_telefono.BorderBrush = Brushes.White;
            txb_direccion.BorderBrush = Brushes.White;
            txb_numero.BorderBrush = Brushes.White;
            txb_codigoPostal.BorderBrush = Brushes.White;
            txb_ciudad.BorderBrush = Brushes.White;
        }

        private bool ValidarDatos()
        {
            bool nombreValido = Validador.ValidarNombre(txb_nombre.Text);
            bool primerApellidoValido = Validador.ValidarNombre(txb_primerApellido.Text);
            bool segundoApellidoValido = Validador.ValidarNombre(txb_segundoApellido.Text);
            bool telefonoValido = Validador.ValidarTelefono(txb_telefono.Text);
            bool direccionValida = Validador.ValidarDireccion(txb_direccion.Text);
            bool codigoPostalValido = Validador.ValidarCodigoPostal(txb_codigoPostal.Text);

            if (!nombreValido) txb_nombre.BorderBrush = Brushes.Red;
            if (!primerApellidoValido) txb_primerApellido.BorderBrush = Brushes.Red;
            if (!segundoApellidoValido) txb_segundoApellido.BorderBrush = Brushes.Red;
            if (!telefonoValido) txb_telefono.BorderBrush = Brushes.Red;
            if (!direccionValida) txb_direccion.BorderBrush = Brushes.Red;
            if (!codigoPostalValido) txb_codigoPostal.BorderBrush = Brushes.Red;

            return nombreValido && primerApellidoValido && segundoApellidoValido && telefonoValido && direccionValida && codigoPostalValido;
        }

        private bool ValidarModificacionDeCampos()
        {
            return
                txb_nombre.Text != socioOriginal.nombre ||
                txb_primerApellido.Text != socioOriginal.primerApellido ||
                txb_segundoApellido.Text != socioOriginal.segundoApellido ||
                txb_telefono.Text != socioOriginal.telefono ||
                txb_direccion.Text != socioOriginal.direccion.calle ||
                txb_numero.Text != socioOriginal.direccion.numero ||
                txb_codigoPostal.Text != socioOriginal.direccion.codigoPostal ||
                txb_ciudad.Text != socioOriginal.direccion.ciudad;
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            RealizarEdicionSocio();
        }

        private void RealizarEdicionSocio()
        {
            ResetearBordes();

            if (!ValidarDatos())
            {
                new VentanaEmergente(Constantes.TipoAdvertencia, "Datos inválidos", "Por favor corrija los campos marcados en rojo.");
                return;
            }

            if (!ValidarModificacionDeCampos())
            {
                new VentanaEmergente(Constantes.TipoInformacion, "Sin cambios", "Debe modificar al menos un campo para guardar los cambios.");
                return;
            }

            try
            {
                SocioBinding socioEditado = new SocioBinding()
                {
                    nombre = txb_nombre.Text,
                    primerApellido = txb_primerApellido.Text,
                    segundoApellido = txb_segundoApellido.Text,
                    telefono = txb_telefono.Text,
                    direccion = new DireccionBinding()
                    {
                        calle = txb_direccion.Text,
                        numero = txb_numero.Text,
                        codigoPostal = txb_codigoPostal.Text,
                        ciudad = txb_ciudad.Text
                    }
                };

                int resultadoValidacion = ValidarInexistenciaDeSocio();

                if (resultadoValidacion==1)
                {
                    new VentanaEmergente(Constantes.TipoError, "Duplicado", "Ya existe otro socio con el mismo número de teléfono.");
                    return;
                }

                SocioManejadorClient socioManejador = new SocioManejadorClient();

                int resultado = socioManejador.EditarDatosSocio(socioOriginal.numeroDeSocio, socioEditado);

                if (resultado > 0)
                {
                    new VentanaEmergente(Constantes.TipoInformacion, "Éxito", "Datos del socio actualizados correctamente.");
                    Close();
                    new VentanaBuscarSocio().ShowDialog();
                }
                else
                {
                    new VentanaEmergente(Constantes.TipoError, "Error", "No se pudo actualizar el socio. Verifica que los datos no estén duplicados.");
                }

            }
            catch (Exception ex)
            {
                LoggerManager.Error($"Error al editar socio: {ex.Message}\n{ex.StackTrace}");
                new VentanaEmergente(Constantes.TipoError, "Error inesperado", "Ocurrió un error al editar el socio. Inténtalo de nuevo.");
            }
        }



        private int ValidarInexistenciaDeSocio()
        {
            int resultadoValidacion = -1;
            try
            {
                SocioManejadorClient socioManejadorClient = new SocioManejadorClient();
                int resultadoVerificacion = socioManejadorClient.VerificarExistenciaDeSocioEnBaseDeDatos(txb_telefono.Text);
                if (resultadoVerificacion == -1)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente("Error", "Error en la conexión a la base de datos", "Se ha perdido la conexión a la base de datos");

                }
                resultadoValidacion = resultadoVerificacion;
            }
            catch (EndpointNotFoundException endpointNotFoundException)
            {
                LoggerManager.Error($"Excepción de EndpointNotFoundException: {endpointNotFoundException.Message}." +
                                    $"\nTraza: {endpointNotFoundException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente("Error", "Punto de conexión fallido", "No se ha podido establecer conexión con el servidor.");

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

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txb_nombre_TextChanged(object sender, TextChangedEventArgs e) { }
        private void txb_primerApellido_TextChanged(object sender, TextChangedEventArgs e) { }
        private void txb_segundoApellido_TextChanged(object sender, TextChangedEventArgs e) { }
        private void txb_telefono_TextChanged(object sender, TextChangedEventArgs e) { }
        private void txb_direccion_TextChanged(object sender, TextChangedEventArgs e) { }
        private void txb_codigoPostal_TextChanged(object sender, TextChangedEventArgs e) { }
        private void txb_numero_TextChanged(object sender, TextChangedEventArgs e) { }
        private void txb_ciudad_TextChanged(object sender, TextChangedEventArgs e) { }
    }
}
