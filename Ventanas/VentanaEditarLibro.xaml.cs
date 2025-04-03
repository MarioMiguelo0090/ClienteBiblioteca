using ClienteBibliotecaElSaber.ServidorElSaber;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace ClienteBibliotecaElSaber.Ventanas
{
    public partial class VentanaEditarLibro : Window
    {
        private LibroBinding libro;
        private byte[] nuevaImagen;

        public VentanaEditarLibro(LibroBinding datosDelLibro)
        {
            InitializeComponent();
            CargarDatosDeLibro(datosDelLibro);
            this.libro = datosDelLibro;
        }

        private void CargarDatosDeLibro(LibroBinding datosLibro)
        {

        }

        private void SubirImagen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
