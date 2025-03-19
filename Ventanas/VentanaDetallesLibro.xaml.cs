using ClienteBibliotecaElSaber.ServidorElSaber;
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
    /// Lógica de interacción para DetallesLibro.xaml
    /// </summary>
    public partial class VentanaDetallesLibro : Window
    {
        private LibroDatos libro;

        public VentanaDetallesLibro(LibroBinding datosDelLibro)
        {
            InitializeComponent();
            CargarDatosDeLibro(datosDelLibro);
        }

        public VentanaDetallesLibro(LibroDatos libro)
        {
            this.libro = libro;
        }

        public void CargarDatosDeLibro(LibroBinding datosLibro)
        {
            txb_Titulo.Text = datosLibro.Titulo;
            txb_Autor.Text = datosLibro.FK_IdAutor.ToString();
            txb_ISBN.Text = datosLibro.Isbn;
            txb_Editorial.Text = datosLibro.FK_IdEditorial.ToString();
            txb_FechaPublicacion.Text = datosLibro.AnioDePublicacion.ToString();
            txb_Genero.Text = datosLibro.FK_IdGenero.ToString();
            txb_NumeroDePaginas.Text = datosLibro.NumeroDePaginas.ToString();
            txb_Estado.Text = datosLibro.Estado;
            txb_CantidadEjemplares.Text = datosLibro.CantidadEjemplares.ToString();
            txb_CantidadEjemplaresPrestados.Text = datosLibro.CantidadEjemplaresPrestados.ToString();
        }

        private void Regresar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            VentanaBuscarLibro ventanaBuscarLibro = new VentanaBuscarLibro();
            ventanaBuscarLibro.ShowDialog();
        }
    }
}