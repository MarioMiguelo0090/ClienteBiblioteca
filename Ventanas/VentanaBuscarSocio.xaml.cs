using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ClienteBibliotecaElSaber.Ventanas
{
    public partial class VentanaBuscarSocio : Window
    {
        public VentanaBuscarSocio()
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
                Buscar_Click(sender, e);
            }
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

        }
    }
}
