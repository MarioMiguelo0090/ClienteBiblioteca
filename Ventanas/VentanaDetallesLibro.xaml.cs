using ClienteBibliotecaElSaber.ServidorElSaber;
using System;
using System.Collections.Generic;
using System.IO;
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
        private LibroBinding libro;

        public VentanaDetallesLibro(LibroBinding datosDelLibro)
        {
            InitializeComponent();
            CargarDatosDeLibro(datosDelLibro);
            this.libro = datosDelLibro;
        }

        public void CargarDatosDeLibro(LibroBinding datosLibro)
        {
            txb_Titulo.Text = datosLibro.Titulo;
            txb_Autor.Text = datosLibro.autor.Autor;
            txb_ISBN.Text = datosLibro.Isbn;
            txb_Editorial.Text = datosLibro.editorial.Editorial;
            txb_FechaPublicacion.Text = datosLibro.AnioDePublicacion.ToString();
            txb_Genero.Text = datosLibro.genero.Genero;
            txb_NumeroDePaginas.Text = datosLibro.NumeroDePaginas.ToString();
            txb_Estado.Text = datosLibro.Estado;
            txb_CantidadEjemplares.Text = datosLibro.CantidadEjemplares.ToString();
            txb_CantidadEjemplaresPrestados.Text = datosLibro.CantidadEjemplaresPrestados.ToString();
            if(datosLibro.imagenLibro != null && datosLibro.imagenLibro.Length > 0)
            {
                img_Libro.Source = ConvertirBytesAImagen(datosLibro.imagenLibro);
            }
        }

        private BitmapImage ConvertirBytesAImagen(byte[] imageData)
        {
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze(); 
                return bitmap;
            }
        }

        private void Regresar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            VentanaBuscarLibro ventanaBuscarLibro = new VentanaBuscarLibro();
            ventanaBuscarLibro.ShowDialog();
        }
    }
}