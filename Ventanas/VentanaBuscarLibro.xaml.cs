using ClienteBibliotecaElSaber.ServidorElSaber;
using ClienteBibliotecaElSaber.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ClienteBibliotecaElSaber.Ventanas
{
    public partial class VentanaBuscarLibro : Window
    {
        public VentanaBuscarLibro()
        {
            InitializeComponent();
        }

        private void Buscar_Click(object sender, RoutedEventArgs e)
        {
            txb_Titulo.BorderBrush = Brushes.White;
            if (ValidarDatosIngresados())
            {
                string campoDeBusqueda = txb_Titulo.Text;
                BuscarLibrosPorTitulo();
            }
            else
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Datos incorrectos", "Por favor verifique que los datos ingresados sean correctos.");
                ventanaEmergente.ShowDialog();
            }
        }

        private void Buscar_Enter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Buscar_Click(sender, e);
            }
        }

        private void Detalles_Click(object sender, RoutedEventArgs e)
        {
            Button botonPresionado = sender as Button;
            LibroDatos libroSeleccionado = botonPresionado.DataContext as LibroDatos;
            if (libroSeleccionado != null)
            {
                VerDetallesDeLibro(libroSeleccionado);
            }
            else
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error al obtener", "No se han podido obtener datos del libro seleccionado.");
                ventanaEmergente.ShowDialog();
            }
        }

        private void VerDetallesDeLibro(LibroDatos libro)
        {
            this.Hide();

            LibroBinding libroBinding = new LibroBinding();
            libroBinding.IdLibro = libro.idLibro;
            libroBinding.Titulo = libro.titulo;

            int idAutor = 0;
            int.TryParse(libro.autor, out idAutor);
            libroBinding.FK_IdAutor = idAutor;

            int idEditorial = 0;
            int.TryParse(libro.editorial, out idEditorial);
            libroBinding.FK_IdEditorial = idEditorial;

            int idGenero = 0;
            int.TryParse(libro.genero, out idGenero);
            libroBinding.FK_IdGenero = idGenero;

            libroBinding.AnioDePublicacion = libro.fechaPublicacion.ToString("yyyy-MM-dd");

            libroBinding.Estado = libro.estado;
            libroBinding.RutaPortada = libro.Imagen;

            VentanaDetallesLibro ventanaDetallesLibro = new VentanaDetallesLibro(libroBinding);
            ventanaDetallesLibro.ShowDialog();
            this.Show();
        }

        private void BuscarLibrosPorTitulo()
        {
            try
            {
                LibroManejadorClient libroManejadorClient = new LibroManejadorClient();
                List<LibroBinding> librosObtenidos = libroManejadorClient.ObtenerLibrosPorTitulo(txb_Titulo.Text).ToList();

                if (librosObtenidos.Count == 0 || librosObtenidos[0].IdLibro == 0)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion,
                        "Resultados no encontrados", "El título que desea buscar no se encuentra registrado.");
                    ventanaEmergente.ShowDialog();
                }

                else
                {
                    CargarLibrosEncontrados(librosObtenidos);
                }
            }
            catch (EndpointNotFoundException ex)
            {
                LoggerManager.Error($"Error de conexión: {ex.Message}\n{ex.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Punto de conexión fallido", "No se ha podido establecer conexión con el servidor.");
                ventanaEmergente.ShowDialog();
            }
        }

        private void CargarLibrosEncontrados(List<LibroBinding> libros)
        {
            lw_libros.Items.Clear();
            foreach (var libroObtenido in libros)
            {
                DateTime fechaPublicacion;
                if (!DateTime.TryParse(libroObtenido.AnioDePublicacion, out fechaPublicacion))
                {
                    fechaPublicacion = DateTime.Now;
                }

                var libro = new LibroDatos()
                {
                    idLibro = libroObtenido.IdLibro,
                    titulo = libroObtenido.Titulo,
                    autor = libroObtenido.FK_IdAutor.ToString(),
                    editorial = libroObtenido.FK_IdEditorial.ToString(),
                    genero = libroObtenido.FK_IdGenero.ToString(),
                    fechaPublicacion = fechaPublicacion,
                    estado = libroObtenido.Estado,
                    Imagen = libroObtenido.RutaPortada
                };

                lw_libros.Items.Add(libro);
            }
        }


        private bool ValidarDatosIngresados()
        {
            bool validacionTitulo = Validador.ValidarTitulo(txb_Titulo.Text);
            if (!validacionTitulo)
            {
                txb_Titulo.BorderBrush = Brushes.Red;
            }
            return validacionTitulo;
        }

        private sealed class LibroDatos
        {
            public int idLibro { get; set; }
            public string titulo { get; set; }
            public string autor { get; set; }
            public string editorial { get; set; }
            public string genero { get; set; }
            public DateTime fechaPublicacion { get; set; }
            public string estado { get; set; }
            public string Imagen { get; set; }
        }

    }
}
