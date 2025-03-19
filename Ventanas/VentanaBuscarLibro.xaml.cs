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
        private List<GeneroBinding> _generos;

        public VentanaBuscarLibro()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                LibroManejadorClient libroManejadorClient = new LibroManejadorClient();
                _generos = libroManejadorClient.ObtenerListaDeGeneros().ToList();
                cmb_genero.ItemsSource = _generos;
                cmb_genero.DisplayMemberPath = "Genero";
            }
            catch (EndpointNotFoundException endpointNotFoundException)
            {
                LoggerManager.Error($"Excepción de EndpointNotFoundException: {endpointNotFoundException.Message}." +
                                    $"\nTraza: {endpointNotFoundException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Punto de conexión fallido", "No se ha podido establecer conexión con el servidor.");
                ventanaEmergente.ShowDialog();
            }
            catch (TimeoutException timeoutException)
            {
                LoggerManager.Error($"Excepción de TimeoutException: {timeoutException.Message}." +
                                    $"\nTraza: {timeoutException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Tiempo de espera agotado", "El tiempo de espera ha caducado, inténtelo de nuevo.");
                ventanaEmergente.ShowDialog();
            }
            catch (CommunicationException communicationException)
            {
                LoggerManager.Error($"Excepción de CommunicationException: {communicationException.Message}." +
                                    $"\nTraza: {communicationException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Comunicacion fallida", "La comunicacion con el servidor se ha perdido, por favor verifique su conexión a internet.");
                ventanaEmergente.ShowDialog();
            }
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
                LibroBinding libroBinding = new LibroBinding()
                {
                    idLibro = libroSeleccionado.idLibro,
                    Titulo = libroSeleccionado.titulo,
                    RutaPortada = libroSeleccionado.Imagen,
                    Estado = libroSeleccionado.estado,
                    AnioDePublicacion = libroSeleccionado.fechaPublicacion.ToString(),

                };
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
            libroBinding.idLibro = libro.idLibro;
            libroBinding.Titulo = libro.titulo;
            libroBinding.autor = libro.autor;
            libroBinding.editorial = libro.editorial; 
            libroBinding.genero = libro.genero;
            libroBinding.AnioDePublicacion = libro.fechaPublicacion.ToString("yyyy");
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

                if (librosObtenidos.Count == 0 || librosObtenidos[0].idLibro == 0)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion,
                        "Resultados no encontrados", "El título que desea buscar no se encuentra registrado.");
                    ventanaEmergente.ShowDialog();
                }
                else if (librosObtenidos[0].idLibro == -1)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error en la base de datos", "No se ha podido establecer conexión a la base de datos.");
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
            catch (TimeoutException timeoutException)
            {
                LoggerManager.Error($"Excepción de TimeoutException: {timeoutException.Message}." +
                                    $"\nTraza: {timeoutException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Tiempo de espera agotado", "El tiempo de espera ha caducado, inténtelo de nuevo.");
                ventanaEmergente.ShowDialog();
            }
            catch (CommunicationException communicationException)
            {
                LoggerManager.Error($"Excepción de CommunicationException: {communicationException.Message}." +
                                    $"\nTraza: {communicationException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Comunicacion fallida", "La comunicacion con el servidor se ha perdido, por favor verifique su conexión a internet.");
                ventanaEmergente.ShowDialog();
            }
        }

        public void Detalles_Click(object sender, EventArgs e)
        {

        }

        public void Editar_Click(object sender, EventArgs e)
        {

        }

        public void Eliminar_Click(object sender, EventArgs e)
        {

        }

        public void Regresar_Click(object sender, EventArgs e)
        {

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
                    idLibro = libroObtenido.idLibro,
                    titulo = libroObtenido.Titulo,
                    autor = libroObtenido.autor,
                    editorial = libroObtenido.editorial,
                    genero = libroObtenido.genero,
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
            public AutorBinding autor { get; set; }
            public EditorialBinding editorial { get; set; }
            public GeneroBinding genero { get; set; }
            public DateTime fechaPublicacion { get; set; }
            public string estado { get; set; }
            public string Imagen { get; set; }
        }

    }
}
