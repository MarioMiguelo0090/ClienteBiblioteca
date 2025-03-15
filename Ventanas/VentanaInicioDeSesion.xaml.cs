using ClienteBibliotecaElSaber.Utilidades;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


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

        private void BotonClicIniciarSesion(object sender, RoutedEventArgs e)
        {

        }

        private void BotonClicSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void PBCambioDeContraseña(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var textoSugerido = ContraseñaHelper.EncontrarHijoVisual<TextBlock>(passwordBox, "TextoSugerido");
            ContraseñaHelper.ActualizarVisibilidadTextoSugerido(passwordBox, textoSugerido);
        }
    }
}
