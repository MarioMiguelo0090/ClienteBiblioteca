using ClienteBibliotecaElSaber.ServidorElSaber;
using ClienteBibliotecaElSaber.Utilidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Remoting.Channels;
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
    /// Lógica de interacción para VentanaRegistroDeUsuario.xaml
    /// </summary>
    public partial class VentanaRegistroDeUsuario : Window
    {
        private string _opcionSeleccionada;
        public VentanaRegistroDeUsuario()
        {
            InitializeComponent();
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            ResetearColorDeBordes();
            if (ValidarDatosDeCampos())
            {
                if (VerificarCorreoExistente(txtb_CorreoElectronico.Text) == 0)
                {
                    RegistrarUsuario();
                }
            }
            else
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia, "Datos incorrectos",
                    "Por favor, verifique que los datos ingresados sean los correctos.");
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private int RegistrarUsuario()
        {
            var proxyUsuario = new UsuarioManejadorClient();
            int resultadoOperacion = -1;
            try
            {
                UsuarioBinding usuario = new UsuarioBinding
                {
                    nombre = txtb_Nombre.Text,
                    primerApellido = txtb_PrimerApellido.Text,
                    segundoApellido = txtb_SegundoApellido.Text,
                    telefono = txtb_Telefono.Text,
                    puesto = txtb_Puesto.Text
                };
                
                DireccionBinding direccion = new DireccionBinding
                {
                    calle = txtb_Calle.Text,
                    numero = txtb_Numero.Text,
                    codigoPostal = txtb_CodigoPostal.Text,
                    ciudad = txtb_Ciudad.Text
                };

                string contraseniaGenerada = GeneradorContrasenia.GenerarContrasenia();
                string hashContrasenia = Encriptador.hashToSHA2(contraseniaGenerada);

                AccesoBinding acceso = new AccesoBinding
                {
                    correo = txtb_CorreoElectronico.Text,
                    contrasenia = hashContrasenia,
                    tipoDeUsuario = _opcionSeleccionada,
                };

                if (EnviarCorreo(txtb_CorreoElectronico.Text, contraseniaGenerada))
                {
                    resultadoOperacion = proxyUsuario.RegistrarUsuarioAlaBaseDeDatos(usuario, direccion, acceso);

                    if (resultadoOperacion == 1)
                    {
                        VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoExito, "Registro exitoso",
                            "Los datos han sido registrados de manera correcta");
                        ResetearCampos();
                    }
                    else if (resultadoOperacion == -1)
                    {
                        VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError,
                            Constantes.TituloExcepcionBD, Constantes.ContenidoExcepcionBD);
                    }
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

        private void RadioButton_Checked(object sender, EventArgs e)
        {
            RadioButton tipoUsuario = sender as RadioButton;
            _opcionSeleccionada = tipoUsuario.Content.ToString();
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
            rb_Administrador.BorderBrush = Brushes.Black;
            rb_Bibliotecario.BorderBrush = Brushes.Black;
        }

        private void ResetearCampos()
        {
            txtb_Nombre.Text = string.Empty;
            txtb_PrimerApellido.Text = string.Empty;
            txtb_SegundoApellido.Text = string.Empty;
            txtb_CorreoElectronico.Text = string.Empty;
            txtb_Telefono.Text = string.Empty   ;
            txtb_Calle.Text = string.Empty;
            txtb_Numero.Text = string.Empty;
            txtb_CodigoPostal.Text = string.Empty;
            txtb_Ciudad.Text = string.Empty;
            txtb_Puesto.Text = string.Empty;
            rb_Administrador.IsChecked = false;
            rb_Bibliotecario.IsChecked = false;
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
            bool tipoUsuarioValidado = (rb_Administrador.IsChecked == true) || (rb_Bibliotecario.IsChecked == true);

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

            if (!tipoUsuarioValidado)
            {
                rb_Administrador.BorderBrush = Brushes.Red;
                rb_Bibliotecario.BorderBrush = Brushes.Red;
            }

            return nombreValidado && primerApellidoValidado && segundoApellidoValidado && correoElectronicoValidado
                   && telefonoValidado && calleValidado && numeroValidado && codigoPostalValidado
                   && ciudadValidado && puestoValidado && tipoUsuarioValidado;
        }

        private int VerificarCorreoExistente(string correo)
        {
            var proxyAcceso = new AccesoManejadorClient();
            int resultadoOperacion = -1;
            try
            {
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

        private bool EnviarCorreo(string correoDestino, string contrasenia)
        {
            try
            {
                string correoRemitente = ConfigurationManager.AppSettings["Correo"];
                string contraseniaCorreo = ConfigurationManager.AppSettings["Contrasenia"];
                string servidorSmtp = ConfigurationManager.AppSettings["SmtpServer"];
                int port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
                string asunto = "Registro exitoso - Tu contraseña";
                string cuerpo = "Querido usuario, se le informa que su registro al sistema el saber se ha realizado con éxito, " +
                    $"a continuación se le muestra su contraseña: {contrasenia}. Por favor no comparta este correo con nadie." +
                    "\n\nAtte. Biblioteca El Saber";

                MailMessage mensaje = new MailMessage
                {
                    From = new MailAddress(correoRemitente),
                    Subject = asunto,
                    Body = cuerpo,
                    IsBodyHtml = false
                };

                mensaje.To.Add(correoDestino);

                SmtpClient smtp = new SmtpClient(servidorSmtp, port)
                {
                    Credentials = new NetworkCredential(correoRemitente, contraseniaCorreo),
                    EnableSsl = true
                };

                smtp.Send(mensaje);
                return true;
            }
            catch (Exception ex) when (ex is SmtpFailedRecipientException || ex is SmtpException)
            {
                LoggerManager.Error($"Excepción: {ex.Message}\nTraza: {ex.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error al enviar el correo",
                    "Ocurrió un error al enviar el correo, por favor inténtelo más tarde");
            }
            return false;
        }
    }
}
