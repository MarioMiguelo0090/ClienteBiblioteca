using ClienteBibliotecaElSaber.ServidorElSaber;
using ClienteBibliotecaElSaber.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private List<AutorBinding> _autores;
        private string _criterioDeBusqueda;

        public VentanaBuscarLibro()
        {
            InitializeComponent();
            CargarDatos();
        }

        public void SeleccionarCriterioBusqueda(object sender, EventArgs e)
        {
            RadioButton seleccion = sender as RadioButton;
            if (seleccion != null)
            {
                EsconderElementosBusqueda();
                VaciarDatosIngresados();
                string criterio = seleccion.Tag.ToString();
                _criterioDeBusqueda = seleccion.Tag.ToString();
                switch (criterio)
                {
                    case "ISBN":
                        txb_ISBN.Visibility = Visibility.Visible;
                        lbl_isbn.Visibility = Visibility.Visible;
                        break;
                    case "Titulo":
                        txb_Titulo.Visibility = Visibility.Visible;
                        lbl_titulo.Visibility = Visibility.Visible;
                        break;
                    case "Genero":
                        cmb_Genero.Visibility = Visibility.Visible;
                        lbl_genero.Visibility = Visibility.Visible;
                        break;
                    case "Autor":
                        cmb_autor.Visibility = Visibility.Visible;
                        lbl_autor.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void VaciarDatosIngresados()
        {
            txb_ISBN.Text = "";
            txb_Titulo.Text = "";
            cmb_autor.SelectedItem = null;
            cmb_Genero.SelectedItem = null;
        }

        private void EsconderElementosBusqueda()
        {
            txb_ISBN.Visibility = Visibility.Hidden;
            lbl_isbn.Visibility = Visibility.Hidden;
            txb_Titulo.Visibility = Visibility.Hidden;
            lbl_titulo.Visibility = Visibility.Hidden;
            cmb_autor.Visibility = Visibility.Hidden;
            lbl_autor.Visibility = Visibility.Hidden;
            cmb_Genero.Visibility = Visibility.Hidden;
            lbl_genero.Visibility = Visibility.Hidden;
        }

        private void CargarDatos()
        {
            try
            {
                LibroManejadorClient libroManejadorClient = new LibroManejadorClient();
                _generos = libroManejadorClient.ObtenerListaDeGeneros().ToList();
                _autores = libroManejadorClient.ObtenerListaDeAutores().ToList();
                cmb_Genero.ItemsSource = _generos;
                cmb_Genero.DisplayMemberPath = "Genero";
                cmb_autor.ItemsSource = _autores;
                cmb_autor.DisplayMemberPath = "Autor";
            }
            catch (EndpointNotFoundException endpointNotFoundException)
            {
                LoggerManager.Error($"Excepción de EndpointNotFoundException: {endpointNotFoundException.Message}." +
                                    $"\nTraza: {endpointNotFoundException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Punto de conexión fallido", "No se ha podido establecer conexión con el servidor.");
            }
            catch (TimeoutException timeoutException)
            {
                LoggerManager.Error($"Excepción de TimeoutException: {timeoutException.Message}." +
                                    $"\nTraza: {timeoutException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Tiempo de espera agotado", "El tiempo de espera ha caducado, inténtelo de nuevo.");
            }
            catch (CommunicationException communicationException)
            {
                LoggerManager.Error($"Excepción de CommunicationException: {communicationException.Message}." +
                                    $"\nTraza: {communicationException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Comunicacion fallida", "La comunicacion con el servidor se ha perdido, por favor verifique su conexión a internet.");
            }
        }

        private void Buscar_Click(object sender, RoutedEventArgs e)
        {
            txb_Titulo.BorderBrush = Brushes.White;
            txb_ISBN.BorderBrush = Brushes.White;
            switch (_criterioDeBusqueda)
            {
                case "ISBN":
                    BuscarLibrosMedianteISBN();
                    break;
                case "Titulo":
                    BuscarLibrosMedianteTitulo();
                    break;
                case "Genero":
                    BuscarLibrosMedianteGenero();
                    break;
                case "Autor":
                    BuscarLibrosMedianteAutor();
                    break;
                default:
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Selección de búsqueda vacía", "Por favor seleccione un tipo de busqueda antes de empezar");
                    break;
            }
            
           
        }

        private void BuscarLibrosMedianteGenero()
        {
            if (ValidarSeleccionPorGenero())
            {
                BuscarLibrosPorGenero();
            }
            else
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Opción sin seleccionar", "Por favor seleccione un género para poder realizar la consulta.");
            }
        }

        private void BuscarLibrosPorGenero()
        {
            try
            {
                LibroManejadorClient libroManejadorClient = new LibroManejadorClient();
                GeneroBinding genero = cmb_Genero.SelectedItem as GeneroBinding;
                List<LibroBinding> librosObtenidos = libroManejadorClient.ObtenerLibrosPorIdGenero(genero.IdGenero).ToList();

                if (librosObtenidos.Count == 0 || librosObtenidos[0].idLibro == 0)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion,
                        "Resultados no encontrados", "El título que desea buscar no se encuentra registrado.");
                }
                else if (librosObtenidos[0].idLibro == -1)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error en la base de datos", "No se ha podido establecer conexión a la base de datos.");
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
            }
            catch (TimeoutException timeoutException)
            {
                LoggerManager.Error($"Excepción de TimeoutException: {timeoutException.Message}." +
                                    $"\nTraza: {timeoutException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Tiempo de espera agotado", "El tiempo de espera ha caducado, inténtelo de nuevo.");
            }
            catch (CommunicationException communicationException)
            {
                LoggerManager.Error($"Excepción de CommunicationException: {communicationException.Message}." +
                                    $"\nTraza: {communicationException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Comunicacion fallida", "La comunicacion con el servidor se ha perdido, por favor verifique su conexión a internet.");
            }
        }

        private void BuscarLibrosMedianteAutor()
        {
            if (ValidarSeleecionPorAutor())
            {
                BuscarLibrosPorAutor();
            }
            else
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Opción sin seleccionar", "Por favor seleccione un autor para poder realizar la consulta.");
            }
        }

        private void BuscarLibrosPorAutor()
        {
            try
            {
                LibroManejadorClient libroManejadorClient = new LibroManejadorClient();
                AutorBinding autor = cmb_autor.SelectedItem as AutorBinding;
                List<LibroBinding> librosObtenidos = libroManejadorClient.ObtenerLibrosPorIdAutor(autor.IdAutor).ToList();

                if (librosObtenidos.Count == 0 || librosObtenidos[0].idLibro == 0)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion,
                        "Resultados no encontrados", "El título que desea buscar no se encuentra registrado.");
                }
                else if (librosObtenidos[0].idLibro == -1)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error en la base de datos", "No se ha podido establecer conexión a la base de datos.");
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
            }
            catch (TimeoutException timeoutException)
            {
                LoggerManager.Error($"Excepción de TimeoutException: {timeoutException.Message}." +
                                    $"\nTraza: {timeoutException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Tiempo de espera agotado", "El tiempo de espera ha caducado, inténtelo de nuevo.");
            }
            catch (CommunicationException communicationException)
            {
                LoggerManager.Error($"Excepción de CommunicationException: {communicationException.Message}." +
                                    $"\nTraza: {communicationException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Comunicacion fallida", "La comunicacion con el servidor se ha perdido, por favor verifique su conexión a internet.");
            }
        }


        private void BuscarLibrosMedianteISBN()
        {
            if (ValidarDatosIngresadosISBN())
            {
                BuscarLibrosPorISBN();
            }
            else
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Datos incorrectos", "Por favor verifique que los datos ingresados sean correctos.");
            }
        }

        private void BuscarLibrosPorISBN()
        {
            try
            {
                LibroManejadorClient libroManejadorClient = new LibroManejadorClient();
                List<LibroBinding> librosObtenidos = libroManejadorClient.ObtenerLibrosPorISBN(txb_ISBN.Text).ToList();

                if (librosObtenidos.Count == 0 || librosObtenidos[0].idLibro == 0)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion,
                        "Resultados no encontrados", "El título que desea buscar no se encuentra registrado.");
                }
                else if (librosObtenidos[0].idLibro == -1)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error en la base de datos", "No se ha podido establecer conexión a la base de datos.");
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
            }
            catch (TimeoutException timeoutException)
            {
                LoggerManager.Error($"Excepción de TimeoutException: {timeoutException.Message}." +
                                    $"\nTraza: {timeoutException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Tiempo de espera agotado", "El tiempo de espera ha caducado, inténtelo de nuevo.");
            }
            catch (CommunicationException communicationException)
            {
                LoggerManager.Error($"Excepción de CommunicationException: {communicationException.Message}." +
                                    $"\nTraza: {communicationException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Comunicacion fallida", "La comunicacion con el servidor se ha perdido, por favor verifique su conexión a internet.");
            }
        }


        private void BuscarLibrosMedianteTitulo()
        {
            if (ValidarDatosIngresadosTitulo())
            {
                BuscarLibrosPorTitulo();
            }
            else
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Datos incorrectos", "Por favor verifique que los datos ingresados sean correctos.");
            }
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
                }
                else if (librosObtenidos[0].idLibro == -1)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error en la base de datos", "No se ha podido establecer conexión a la base de datos.");
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
            }
            catch (TimeoutException timeoutException)
            {
                LoggerManager.Error($"Excepción de TimeoutException: {timeoutException.Message}." +
                                    $"\nTraza: {timeoutException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Tiempo de espera agotado", "El tiempo de espera ha caducado, inténtelo de nuevo.");
            }
            catch (CommunicationException communicationException)
            {
                LoggerManager.Error($"Excepción de CommunicationException: {communicationException.Message}." +
                                    $"\nTraza: {communicationException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Comunicacion fallida", "La comunicacion con el servidor se ha perdido, por favor verifique su conexión a internet.");
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
            libroBinding.NumeroDePaginas = libro.numeroDePaginas;
            libroBinding.CantidadEjemplares = libro.cantidadEjemplares;
            libroBinding.CantidadEjemplaresPrestados = libro.cantidadEjemplaresPrestados;
            libroBinding.Isbn = libro.isbn;
            VentanaDetallesLibro ventanaDetallesLibro = new VentanaDetallesLibro(libroBinding);
            ventanaDetallesLibro.ShowDialog();
            this.Show();
        }

        public void Editar_Click(object sender, EventArgs e)
        {
            this.Close();
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
                    isbn =  libroObtenido.Isbn,
                    autor = libroObtenido.autor,
                    editorial = libroObtenido.editorial,
                    genero = libroObtenido.genero,
                    fechaPublicacion = fechaPublicacion,
                    estado = libroObtenido.Estado,
                    Imagen = libroObtenido.RutaPortada,
                    cantidadEjemplares = libroObtenido.CantidadEjemplares,
                    cantidadEjemplaresPrestados = libroObtenido.CantidadEjemplaresPrestados,
                    numeroDePaginas = libroObtenido.NumeroDePaginas,
                };

                lw_libros.Items.Add(libro);
            }
        }

        private bool ValidarDatosIngresadosISBN()
        {
            bool validacionISBN = Validador.ValidarISBN(txb_ISBN.Text);
            if (!validacionISBN)
            {
                txb_ISBN.BorderBrush = Brushes.Red;
            }
            return validacionISBN;
        }

        private bool ValidarSeleccionPorGenero()
        {
            bool resultadoValidacion = false;
            if(cmb_Genero.SelectedItem != null)
            {
                resultadoValidacion = true;
            }
            return resultadoValidacion;
        }

        private bool ValidarSeleecionPorAutor()
        {
            bool resultadoValidacion = false;
            if (cmb_autor.SelectedItem != null)
            {
                resultadoValidacion = true;
            }
            return resultadoValidacion;
        }


        private bool ValidarDatosIngresadosTitulo()
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
            public string isbn { get; set; }
            public AutorBinding autor { get; set; }
            public EditorialBinding editorial { get; set; }
            public GeneroBinding genero { get; set; }
            public DateTime fechaPublicacion { get; set; }
            public string estado { get; set; }
            public string Imagen { get; set; }
            public string numeroDePaginas {  get; set; }
            public int cantidadEjemplares { get; set; }
            public int cantidadEjemplaresPrestados { get; set; }
        }

    }
}
