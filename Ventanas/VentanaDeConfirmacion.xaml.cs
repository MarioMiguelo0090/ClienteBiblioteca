﻿using System;
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
    /// Lógica de interacción para VentanaDeConfirmacion.xaml
    /// </summary>
    public partial class VentanaDeConfirmacion : Window
    {
        public VentanaDeConfirmacion(string titulo, string contenido)
        {
            InitializeComponent();
            Txbl_Titulo.Text = titulo;
            Txbl_Contenido.Text = contenido; 
        }
        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
