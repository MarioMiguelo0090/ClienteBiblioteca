using ClienteBibliotecaElSaber.ServidorElSaber;
using ClienteBibliotecaElSaber.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public partial class VentanaEdicionUsuario : Window
    {
        private AccesoBinding acceso;
        public VentanaEdicionUsuario(AccesoBinding acceso)
        {
            InitializeComponent();
            this.acceso = acceso;
            llenarDatosUsuario();
        }

        private void llenarDatosUsuario()
        {
            txtb_Nombre.Text = acceso.IdUsuario.nombre;
            txtb_PrimerApellido.Text = acceso.IdUsuario.primerApellido;
            txtb_SegundoApellido.Text = acceso.IdUsuario.segundoApellido;
            txtb_CorreoElectronico.Text = acceso.correo;
            txtb_Telefono.Text = acceso.IdUsuario.telefono;
            txtb_Calle.Text = acceso.IdUsuario.direccion.calle;
            txtb_Numero.Text = acceso.IdUsuario.direccion.numero;
            txtb_CodigoPostal.Text = acceso.IdUsuario.direccion.codigoPostal;
            txtb_Ciudad.Text = acceso.IdUsuario.direccion.ciudad;
            txtb_Puesto.Text = acceso.IdUsuario.puesto;
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            ResetearColorDeBordes();
            if (ValidarDatosDeCampos())
            {
                if (!ValidarDatosNuevos())
                {
                    if (VerificarCorreoExistente(txtb_CorreoElectronico.Text) == 0)
                    {
                        EditarUsuario();
                        this.Close();
                    }
                }
                else
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia,
                     "Campos sin modificar", "Para que los nuevos datos puedan guardarse en la base de datos, primero debe editarlos.");

                }
            }
            else
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia,
                    "Datos inválidso", "Por favor, verifique que los datos ingresados sean los correctos");
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ResetearColorDeBordes()
        {
            txtb_Nombre.BorderBrush = Brushes.Transparent;
            txtb_PrimerApellido.BorderBrush = Brushes.Transparent;
            txtb_SegundoApellido.BorderBrush = Brushes.Transparent;
            txtb_CorreoElectronico.BorderBrush = Brushes.Transparent;
            txtb_Telefono.BorderBrush = Brushes.Transparent;
            txtb_Calle.BorderBrush = Brushes.Transparent;
            txtb_Numero.BorderBrush = Brushes.Transparent;
            txtb_CodigoPostal.BorderBrush = Brushes.Transparent;
            txtb_Ciudad.BorderBrush = Brushes.Transparent;
            txtb_Puesto.BorderBrush = Brushes.Transparent;
        }

        private int EditarUsuario()
        {
            var proxyUsuario = new UsuarioManejadorClient();
            int resultadoOperacion = -1;

            try
            {
                UsuarioBinding usuarioEditado = new UsuarioBinding
                {
                    nombre = txtb_Nombre.Text.Trim(),
                    primerApellido = txtb_PrimerApellido.Text.Trim(),
                    segundoApellido = txtb_SegundoApellido.Text.Trim(),
                    telefono = txtb_Telefono.Text,
                    puesto = txtb_Puesto.Text.Trim(),
                    direccion = new DireccionBinding
                    {
                        calle = txtb_Calle.Text.Trim(),
                        numero = txtb_Numero.Text,
                        codigoPostal = txtb_CodigoPostal.Text,
                        ciudad = txtb_Ciudad.Text.Trim(),
                    }
                };

                resultadoOperacion = proxyUsuario.EditarInformacionUsuarioPorIdAcceso(acceso.IdAcceso, usuarioEditado, txtb_CorreoElectronico.Text.Trim());

                if (resultadoOperacion == 1)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoExito,
                        "Edición exitosa", "Lo nuevos datos se han guardado de manera exitosa");
                }
                else if (resultadoOperacion == 0)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError,
                        Constantes.TituloExcepcionBD, Constantes.ContenidoExcepcionBD);
                }
            }
            catch (Exception ex) when (ex is EndpointNotFoundException || ex is TimeoutException || ex is CommunicationException)
            {
                LoggerManager.Error($"Excepción: {ex.Message}\nTraza: {ex.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError,
                    Constantes.TituloExcepcionServidor, Constantes.ContenidoExcepcionServidor);
            }
            finally
            {
                if (proxyUsuario.State == CommunicationState.Opened)
                {
                    proxyUsuario.Close();
                }
            }

            return resultadoOperacion;
        }

        private bool ValidarDatosNuevos()
        {
            return
                txtb_Nombre.Text == acceso.IdUsuario.nombre &&
                txtb_PrimerApellido.Text == acceso.IdUsuario.primerApellido &&
                txtb_SegundoApellido.Text == acceso.IdUsuario.segundoApellido &&
                txtb_CorreoElectronico.Text == acceso.correo &&
                txtb_Telefono.Text == acceso.IdUsuario.telefono &&
                txtb_Calle.Text == acceso.IdUsuario.direccion.calle &&
                txtb_Numero.Text == acceso.IdUsuario.direccion.numero &&
                txtb_CodigoPostal.Text == acceso.IdUsuario.direccion.codigoPostal &&
                txtb_Ciudad.Text == acceso.IdUsuario.direccion.ciudad &&
                txtb_Puesto.Text == acceso.IdUsuario.puesto;
        }


        private bool ValidarDatosDeCampos()
        {
            bool nombreValidado = Validador.ValidarNombre(txtb_Nombre.Text);
            bool primerApellidoValidado = Validador.ValidarPrimerApellido(txtb_PrimerApellido.Text);
            bool segundoApellidoValidado = Validador.ValidarSegundoApellido(txtb_SegundoApellido.Text);
            bool correoElectronicoValidado = Validador.ValidarCorreo(txtb_CorreoElectronico.Text);
            bool telefonoValidado = Validador.ValidarTelefono(txtb_Telefono.Text);
            bool calleValidado = Validador.ValidarCalle(txtb_Calle.Text);
            bool numeroValidado = Validador.ValidarNumeroCasa(txtb_Numero.Text);
            bool codigoPostalValidado = Validador.ValidarCodigoPostal(txtb_CodigoPostal.Text);
            bool ciudadValidado = Validador.ValidarCalle(txtb_Ciudad.Text);
            bool puestoValidado = Validador.ValidarNombre(txtb_Puesto.Text);
            
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

            if (!correoElectronicoValidado)
            {
                txtb_CorreoElectronico.BorderBrush = Brushes.Red;

            }

            if (!telefonoValidado)
            {
                txtb_Telefono.BorderBrush = Brushes.Red;
            }

            if (!calleValidado)
            {
                txtb_Calle.BorderBrush = Brushes.Red;
            }

            if (!numeroValidado)
            {
                txtb_Numero.BorderBrush = Brushes.Red;
            }

            if (!codigoPostalValidado)
            {
                txtb_CodigoPostal.BorderBrush = Brushes.Red;
            }

            if (!ciudadValidado)
            {
                txtb_Ciudad.BorderBrush = Brushes.Red;
            }

            if (!puestoValidado)
            {
                txtb_Puesto.BorderBrush = Brushes.Red;
            }

            return nombreValidado && primerApellidoValidado && segundoApellidoValidado && correoElectronicoValidado
                   && telefonoValidado && calleValidado && numeroValidado && codigoPostalValidado
                   && ciudadValidado && puestoValidado;
        }

        private int VerificarCorreoExistente(string correo)
        {
            var proxyAcceso = new AccesoManejadorClient();
            int resultadoOperacion = -1;
            try
            {
                if (txtb_CorreoElectronico.Text == acceso.correo) return 0;

                resultadoOperacion = proxyAcceso.VerificarCorreoExistente(correo);
                if (resultadoOperacion == 1)
                {
                    txtb_CorreoElectronico.BorderBrush = Brushes.Red;
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion,
                        "Correo existente", "El correo proporcionado actualmente se encuentra registrado en el sistema.");
                }
            }
            catch (Exception ex) when (ex is EndpointNotFoundException || ex is TimeoutException || ex is CommunicationException)
            {
                LoggerManager.Error($"Excepción: {ex.Message}\nTraza: {ex.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError,
                    Constantes.TituloExcepcionServidor, Constantes.ContenidoExcepcionServidor);
            }
            finally
            {
                if (proxyAcceso.State == CommunicationState.Opened)
                {
                    proxyAcceso.Close();
                }
            }

            return resultadoOperacion;
        }
    }
}
