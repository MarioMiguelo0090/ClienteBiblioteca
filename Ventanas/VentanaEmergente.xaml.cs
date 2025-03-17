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
    /// Lógica de interacción para VentanaEmergente.xaml
    /// </summary>
    public partial class VentanaEmergente : Window
    {
        public VentanaEmergente(string tipoVentanaEmergente, string titulo, string contenido)
        {
            InitializeComponent();
            ColocarTipoDeIcono(tipoVentanaEmergente);
            Txbl_Titulo.Text = titulo;
            Txbl_Contenido.Text = contenido;
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ColocarTipoDeIcono(string tipoVentanaEmergente)
        {
            switch (tipoVentanaEmergente)
            {
                case Constantes.TipoInformacion:
                    Img_IconoVentanaEmergente.Source = new BitmapImage(new Uri("pack://application:,,,/Imagenes/Iconos/Info.png"));
                    break;
                case Constantes.TipoExito:
                    Img_IconoVentanaEmergente.Source = new BitmapImage(new Uri("pack://application:,,,/Imagenes/Iconos/Exito.png"));
                    break;
                case Constantes.TipoAdvertencia:
                    Img_IconoVentanaEmergente.Source = new BitmapImage(new Uri("pack://application:,,,/Imagenes/Iconos/Advertencia.png"));
                    break;
                case Constantes.TipoError:
                    Img_IconoVentanaEmergente.Source = new BitmapImage(new Uri("pack://application:,,,/Imagenes/Iconos/Error.png"));
                    break;
            }
        }
    }
}
