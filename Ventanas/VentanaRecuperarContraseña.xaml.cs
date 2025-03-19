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
    /// Lógica de interacción para VentanaRecuperarContraseña.xaml
    /// </summary>
    public partial class VentanaRecuperarContraseña : Window
    {
        public VentanaRecuperarContraseña()
        {
            InitializeComponent();
            PaginaRecuperarContrasenia paginaRecuperarContrasenia = new PaginaRecuperarContrasenia();
            MarcoPrincipal.Navigate(paginaRecuperarContrasenia);
        }
    }
}
