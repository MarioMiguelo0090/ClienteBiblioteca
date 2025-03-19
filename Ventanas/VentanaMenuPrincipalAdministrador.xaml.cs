using ClienteBibliotecaElSaber.Singleton;
using ClienteBibliotecaElSaber.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// Lógica de interacción para VentanaMenuPrincipalAdministrador.xaml
    /// </summary>
    public partial class VentanaMenuPrincipalAdministrador : Window
    {
        private readonly ManejadorDeCitasAutores manejadorDeCitasAutores;

        public VentanaMenuPrincipalAdministrador()
        {
            InitializeComponent();
            manejadorDeCitasAutores = new ManejadorDeCitasAutores();
            var (autor, cita) = manejadorDeCitasAutores.ObtenerCitaAleatoria();
            Txbl_Autor.Text = autor.ToString();
            Txbl_Cita.Text = cita.ToString();
            txtbl_Bienvenida.Text = $"Bienvenido, {SingletonAdministrador.Instancia.ObtenerNombreCompleto()}";
        }

        private void IrVentanaRegistrarUsuario_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            VentanaRegistroDeUsuario ventanaRegistroDeUsuario = new VentanaRegistroDeUsuario();
            ventanaRegistroDeUsuario.ShowDialog();
            this.Show();
        }

        private void Salir_Click(object sender, RoutedEventArgs e)
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

        private void CerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            SingletonAdministrador.Instancia.CerrarSesion();
            VentanaInicioDeSesion ventanaInicioDeSesion = new VentanaInicioDeSesion();
            ventanaInicioDeSesion.Show();
            this.Close();
        }
    }
}
