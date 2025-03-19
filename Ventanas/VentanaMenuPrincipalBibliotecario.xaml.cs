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
    /// Lógica de interacción para VentanaMenuPrincipal.xaml
    /// </summary>
    public partial class VentanaMenuPrincipalBibliotecario : Window
    {
        private readonly ManejadorDeCitasAutores manejadorDeCitasAutores;
        public VentanaMenuPrincipalBibliotecario()
        {
            InitializeComponent();
            manejadorDeCitasAutores = new ManejadorDeCitasAutores();
            var (autor, cita) = manejadorDeCitasAutores.ObtenerCitaAleatoria();
            txtbl_Autor.Text = autor.ToString();
            txtbl_Cita.Text = cita.ToString();
            txtbl_Bienvenida.Text = $"Bienvenido, {SingletonBibliotecario.Instancia.ObtenerNombreCompleto()}";
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

        private void IrVentanaSocios_Click(object sender, RoutedEventArgs e)
        {

        }

        private void IrVentanaLibro_Click(object sender, RoutedEventArgs e)
        {
            VentanaRegistroDePrestamo ventanaRegistroDePrestamo = new VentanaRegistroDePrestamo();
            ventanaRegistroDePrestamo.Show();
            this.Hide();
        }

        private void IrVentanaDevolucion_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            VentanaDevolucion ventanaDevolucion = new VentanaDevolucion();
            ventanaDevolucion.ShowDialog();
            this.Show();
        }

        private void CerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            SingletonBibliotecario.Instancia.CerrarSesion();
            VentanaInicioDeSesion ventanaInicioDeSesion = new VentanaInicioDeSesion();
            ventanaInicioDeSesion.Show();
            this.Close();
        }
    }
}
