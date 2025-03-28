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
    /// Lógica de interacción para VentanaCreacionDeReportes.xaml
    /// </summary>
    public partial class VentanaCreacionDeReportes : Window
    {
        public VentanaCreacionDeReportes()
        {
            InitializeComponent();
        }

        private void btn_crearReporteLibrosMasPrestados_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SeleccionarCriterioBusqueda(object sender, RoutedEventArgs e)
        {

        }

        private void BotonReporteInventarioLibro_Click(object sender, RoutedEventArgs e)
        {
            OcultarBordes();
            brd_InventarioLibros.Visibility = Visibility.Visible;
        }

        private void BotonReporteLibrosMasPrestados_Click(object sender, RoutedEventArgs e)
        {
            OcultarBordes();
            brd_LibrosMasPrestados.Visibility = Visibility.Visible;
        }

        private void OcultarBordes()
        {
            img_logo.Visibility = Visibility.Collapsed;
            brd_InventarioLibros.Visibility = Visibility.Collapsed;
            brd_LibrosMasPrestados.Visibility = Visibility.Collapsed;
        }
    }
}
