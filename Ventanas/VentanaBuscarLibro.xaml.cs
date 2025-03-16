using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            sp_Resultados.Children.Clear();
        }

        private void Buscar_Enter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Buscar_Click(sender, e);
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
            MessageBox.Show("Mostrar detalles del libro.", "Detalles", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Abrir editor del libro.", "Editar", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Confirmar eliminación del libro.", "Eliminar", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void Regresar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
