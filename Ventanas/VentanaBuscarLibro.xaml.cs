using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClienteBibliotecaElSaber.Ventanas
{
    public partial class VentanaBuscarLibro : Page
    {
        public VentanaBuscarLibro()
        {
            InitializeComponent();
        }

        private void Buscar_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Buscar_Enter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
            }
        }

        private void ComboBox_Genero(object sender, SelectionChangedEventArgs e)
        {
        }

        private void Editorial_BusquedaPerdida(object sender, RoutedEventArgs e)
        {
        }

        private void Detalles_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
          
        }

        private void Regresar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
