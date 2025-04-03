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
    /// Lógica de interacción para VentanaDetallesSocio.xaml
    /// </summary>
    public partial class VentanaDetallesSocio : Window
    {
        public VentanaDetallesSocio(SocioBinding datosDelSocio)
        {
            InitializeComponent();
            CargarDatosDeSocio(datosDelSocio);
        }

        public void CargarDatosDeSocio(SocioBinding datosSocio)
        {
            txb_codigoPostal.Text = datosSocio.direccion.codigoPostal;
            txb_direccion.Text = datosSocio.direccion.calle + " " + datosSocio.direccion.numero + ", " + datosSocio.direccion.ciudad;
            txb_fechaDeInscripcion.Text = datosSocio.fechaDeInscripcion.ToString("yyyy-MM-dd");
            txb_fechaDeNacimiento.Text = datosSocio.fechaDeNacimiento.ToString("yyyy-MM-dd");
            txb_nombre.Text = datosSocio.nombre;
            txb_primerApellido.Text = datosSocio.primerApellido;
            txb_segundoApellido.Text = datosSocio.segundoApellido;
            txb_telefono.Text = datosSocio.telefono;
        }

        private void Regresar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
