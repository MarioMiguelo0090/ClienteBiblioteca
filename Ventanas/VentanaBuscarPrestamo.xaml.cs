using ClienteBibliotecaElSaber.ServidorElSaber;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class VentanaBuscarPrestamo : Window
    {
        public ObservableCollection<PrestamoBinding> Prestamos { get; set; }
        public VentanaBuscarPrestamo()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            VentanaEditarPrestamo ventanaEditarPrestamo = new VentanaEditarPrestamo();
            ventanaEditarPrestamo.ShowDialog();
            this.Show();
        }
    }
}
