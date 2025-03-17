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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClienteBibliotecaElSaber.Ventanas
{
    /// <summary>
    /// Lógica de interacción para VentanaRegistroLibro.xaml
    /// </summary>
    public partial class VentanaRegistroLibro : Window
    {
        public VentanaRegistroLibro()
        {
            InitializeComponent();
            CargarDatos();
        }

        private List<string> autores = new List<string> { "Autor 1", "Autor 2", "Autor 3" }; 
        private List<string> editoriales = new List<string> { "Editorial A", "Editorial B", "Editorial C" };

        private void CargarDatos()
        {
            cbAutor.ItemsSource = autores;
            cbEditorial.ItemsSource = editoriales;
        }

        private void Autor_BusquedaPerdida(object sender, RoutedEventArgs e)
        {
            string nuevoAutor = cbAutor.Text.Trim();
            if (!string.IsNullOrEmpty(nuevoAutor) && !autores.Contains(nuevoAutor))
            {
                autores.Add(nuevoAutor);
                cbAutor.ItemsSource = null; // Resetear ItemsSource
                cbAutor.ItemsSource = autores;
            }
        }

        private void Editorial_BusquedaPerdida(object sender, RoutedEventArgs e)
        {
            string nuevaEditorial = cbEditorial.Text.Trim();
            if (!string.IsNullOrEmpty(nuevaEditorial) && !editoriales.Contains(nuevaEditorial))
            {
                editoriales.Add(nuevaEditorial);
                cbEditorial.ItemsSource = null; // Resetear ItemsSource
                cbEditorial.ItemsSource = editoriales;
            }
        }

        private void ComboBox_Genero(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
