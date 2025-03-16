using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ClienteBibliotecaElSaber.Ventanas
{
    public partial class VentanaBuscarSocio : Page
    {
        private List<Socio> socios = new List<Socio>
        {
            new Socio { NumeroSocio = 1, Nombre = "Juan Pérez" },
            new Socio { NumeroSocio = 2, Nombre = "María González" },
            new Socio { NumeroSocio = 3, Nombre = "Carlos Ramírez" }
        };

        public VentanaBuscarSocio(Socio socioSeleccionado)
        {
            InitializeComponent();
            Ltv_Resultados.ItemsSource = new List<Socio>();
        }

        private void Buscar_Click(object sender, RoutedEventArgs e)
        {
            string filtro = Txb_Busqueda.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(filtro))
            {
                var resultados = socios.FindAll(s =>
                    s.Nombre.ToLower().Contains(filtro) || s.NumeroSocio.ToString().Contains(filtro));

                Ltv_Resultados.ItemsSource = resultados;
            }
            else
            {
                MessageBox.Show("Por favor ingrese un nombre o número de socio para buscar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Detalles_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Buscar_Enter(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Buscar_Click(sender, e);
            }
        }

        private void Regresar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }

    public class Socio
    {
        public int NumeroSocio { get; set; }
        public string Nombre { get; set; }
    }
}
