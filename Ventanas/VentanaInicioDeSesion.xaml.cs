using ClienteBibliotecaElSaber.ServidorElSaber;
using ClienteBibliotecaElSaber.Utilidades;
using System.ServiceModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ClienteBibliotecaElSaber.Singleton;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace ClienteBibliotecaElSaber.Ventanas
{
    /// <summary>
    /// Lógica de interacción para VentanaInicioDeSesion.xaml
    /// </summary>
    public partial class VentanaInicioDeSesion : Window
    {
        public VentanaInicioDeSesion()
        {
            InitializeComponent();
        }

        private void IniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            ResetearColorDeBordes();
            if (ValidarDatosDeCampos())
            {
                string contraseniaHasheada;
                var proxyAcceso = new AccesoManejadorClient();
                AccesoBinding acceso = new AccesoBinding();

                try
                {
                    if (pb_Contrasenia.Visibility == Visibility.Visible)
                    {
                        contraseniaHasheada = Encriptador.hashToSHA2(pb_Contrasenia.Password);
                        acceso = proxyAcceso.IniciarSesion(txtb_Correo.Text, contraseniaHasheada);
                    }
                    else
                    {
                        contraseniaHasheada = Encriptador.hashToSHA2(txtb_Contrasenia.Text);
                        acceso = proxyAcceso.IniciarSesion(txtb_Correo.Text, contraseniaHasheada);
                    }
                    

                    if (acceso.IdAcceso != -3 && acceso.IdAcceso != -1 && acceso.IdAcceso != 0)
                    {
                        if (acceso.tipoDeUsuario == Constantes.TipoAdministrador)
                        {
                            SingletonAdministrador.Instancia.IniciarSesion(acceso);
                            VentanaMenuPrincipalAdministrador ventanaMenuPrincipalAdministrador = new VentanaMenuPrincipalAdministrador();
                            ventanaMenuPrincipalAdministrador.Show();
                            this.Close();
                        }
                        else if (acceso.tipoDeUsuario == Constantes.TipoBibliotecario)
                        {
                            SingletonBibliotecario.Instancia.IniciarSesion(acceso);
                            VentanaMenuPrincipalBibliotecario ventanaMenuPrincipalBibliotecario = new VentanaMenuPrincipalBibliotecario();
                            ventanaMenuPrincipalBibliotecario.Show();
                            this.Close();
                        }
                    }
                    else if (acceso.IdAcceso == 0)
                    {
                        VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion,
                            "Datos incorrectos", "El correo electrónico y/o contraseña son incorrectos");
                    }
                    else if (acceso.IdAcceso == -3)
                    {
                        VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia,
                            "Sesión activa", "La cuenta a la que intenta acceder actualmente se encuentra con la sesión activa.");
                    }
                    else
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
                    if (proxyAcceso.State == CommunicationState.Opened)
                    {
                        proxyAcceso.Close();
                    }
                }
            }
            else
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia,
                    "Campos vacíos", "Favor de ingresar el correo electrónico y/o contraseña");
            }
        }

        private void Salir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ResetearColorDeBordes()
        {
            txtb_Correo.BorderBrush = Brushes.Transparent;
            pb_Contrasenia.BorderBrush = Brushes.Transparent;
            txtb_Contrasenia.BorderBrush = Brushes.Transparent;
        }

        private bool ValidarDatosDeCampos()
        {
            bool usuarioNoValidado = string.IsNullOrWhiteSpace(txtb_Correo.Text);
            bool contraseniaNoValidado;

            if (pb_Contrasenia.Visibility == Visibility.Visible)
            {
                contraseniaNoValidado = string.IsNullOrWhiteSpace(pb_Contrasenia.Password);
            }
            else
            {
                contraseniaNoValidado = string.IsNullOrEmpty(txtb_Contrasenia.Text);
            }

            if (usuarioNoValidado)
            {
                txtb_Correo.BorderBrush = Brushes.Red;
            }

            if (contraseniaNoValidado)
            {
                pb_Contrasenia.BorderBrush = Brushes.Red;
                txtb_Contrasenia.BorderBrush= Brushes.Red;
            }

            return !usuarioNoValidado && !contraseniaNoValidado;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CambiarVisibilidadContrasenia(object sender, RoutedEventArgs e)
        {
            if (tbtn_VisibilidadContrasenia.IsChecked == true)
            {
                txtb_Contrasenia.Text = pb_Contrasenia.Password;
                pb_Contrasenia.Visibility = Visibility.Collapsed;
                txtb_Contrasenia.Visibility = Visibility.Visible;

                img_Visibilidad.Source = new BitmapImage(new Uri("pack://application:,,,/Imagenes/Iconos/IconoVisibleOscuro.png"));
            }
            else
            {
                pb_Contrasenia.Password = txtb_Contrasenia.Text;
                pb_Contrasenia.Visibility = Visibility.Visible;
                txtb_Contrasenia.Visibility = Visibility.Collapsed;
                
                img_Visibilidad.Source = new BitmapImage(new Uri("pack://application:,,,/Imagenes/Iconos/IconoNoVisibleOscuro.png"));
            }
        }

        private void PbCambioDeContrasenia(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var textoSugerido = ContraseñaHelper.EncontrarHijoVisual<TextBlock>(passwordBox, "TextoSugerido");
            ContraseñaHelper.ActualizarVisibilidadTextoSugerido(passwordBox, textoSugerido);
        }

        private void IrVentanaRecuperarContraseña(object sender, MouseButtonEventArgs e)
        {
            VentanaRecuperarContraseña ventanaRecuperarContraseña = new VentanaRecuperarContraseña();
            ventanaRecuperarContraseña.ShowDialog();
        }
    }
}
