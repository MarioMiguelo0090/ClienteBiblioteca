using ClienteBibliotecaElSaber.ServidorElSaber;
using ClienteBibliotecaElSaber.Singleton;
using ClienteBibliotecaElSaber.Utilidades;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClienteBibliotecaElSaber.Ventanas
{
    public partial class VentanaBuscarLibro : Window
    {
        private List<GeneroBinding> _generos;
        private List<AutorBinding> _autores;
        private string _criterioDeBusqueda;
        private bool _tienePermisos;

        public VentanaBuscarLibro()
        {
            InitializeComponent();
            CargarDatos();
            VerificarPermisos();
        }
        
        public void VerificarPermisos()
        {
            if(SingletonBibliotecario.Instancia.IdAcceso != 0)
            {
                _tienePermisos = false;
            }
            else
            {
                _tienePermisos = true;
            }
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
                LibroManejadorClient libroManejadorClient = new LibroManejadorClient();
                byte[] imagenLibro = libroManejadorClient.ObtenerImagenLibro(libroSeleccionado.titulo);
                if(imagenLibro == null)
                {
                    imagenLibro = ObtenerImagenPorDefecto();
                    VerDetallesDeLibro(libroSeleccionado, imagenLibro);
                }
                else
                {
                    VerDetallesDeLibro(libroSeleccionado, imagenLibro);
                }
                
            }
            else
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error al obtener", "No se han podido obtener datos del libro seleccionado.");
            }
        }

        private void VerDetallesDeLibro(LibroDatos libro, byte[] imagen)
        {
            LibroBinding libroBinding = new LibroBinding();
            libroBinding.idLibro = libro.idLibro;
            libroBinding.Titulo = libro.titulo;
            libroBinding.autor = libro.autor;
            libroBinding.editorial = libro.editorial; 
            libroBinding.genero = libro.genero;
            libroBinding.AnioDePublicacion = libro.fechaPublicacion.ToString("yyyy");
            libroBinding.Estado = libro.estado;
            libroBinding.imagenLibro = libro.Imagen;
            libroBinding.NumeroDePaginas = libro.numeroDePaginas;
            libroBinding.CantidadEjemplares = libro.cantidadEjemplares;
            libroBinding.CantidadEjemplaresPrestados = libro.cantidadEjemplaresPrestados;
            libroBinding.Isbn = libro.isbn;
            VentanaDetallesLibro ventanaDetallesLibro = new VentanaDetallesLibro(libroBinding);
            ventanaDetallesLibro.ShowDialog();
            this.Show();
        }

        private void EditarLibro(LibroDatos libro, byte[] imagen)
        {
            LibroBinding libroBinding = new LibroBinding();
            libroBinding.idLibro = libro.idLibro;
            libroBinding.Titulo = libro.titulo;
            libroBinding.autor = libro.autor;
            libroBinding.editorial = libro.editorial;
            libroBinding.genero = libro.genero;
            libroBinding.AnioDePublicacion = libro.fechaPublicacion.ToString("yyyy");
            libroBinding.Estado = libro.estado;
            libroBinding.imagenLibro = libro.Imagen;
            libroBinding.NumeroDePaginas = libro.numeroDePaginas;
            libroBinding.CantidadEjemplares = libro.cantidadEjemplares;
            libroBinding.CantidadEjemplaresPrestados = libro.cantidadEjemplaresPrestados;
            libroBinding.Isbn = libro.isbn;
            libroBinding.RutaPortada = libro.rutaImagen;
            VentanaEditarLibro ventanaEditarLibro = new VentanaEditarLibro(libroBinding);
            ventanaEditarLibro.ShowDialog();
            this.Show();
        }

        public void Editar_Click(object sender, EventArgs e)
        {
            Button botonPresionado = sender as Button;
            LibroDatos libroSeleccionado = botonPresionado.DataContext as LibroDatos;
            if (libroSeleccionado != null)
            {
                LibroManejadorClient libroManejadorClient = new LibroManejadorClient();
                byte[] imagenLibro = libroManejadorClient.ObtenerImagenLibro(libroSeleccionado.titulo);
                if (imagenLibro == null)
                {
                    imagenLibro = ObtenerImagenPorDefecto();
                    EditarLibro(libroSeleccionado, imagenLibro);

                }
                else
                {
                    EditarLibro(libroSeleccionado, imagenLibro);
                }

            }
            else
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error al obtener", "No se han podido obtener datos del libro seleccionado.");
            }
        }

        public void Desactivar_Click(object sender, EventArgs e)
        {
            Button botonPresionado = sender as Button;
            LibroDatos libroSeleccionado = botonPresionado.DataContext as LibroDatos;
            if(libroSeleccionado != null)
            {
                if (VerificarLibroSinPrestamosActivos(libroSeleccionado))
                {
                    RealizarDesactivacionLibro(libroSeleccionado);
                }
                else
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Libro en prestamo", "No es posible desactivar el libro, ya que cuenta con ejemplares en préstamo");
                }
            }
        }

        private void RealizarDesactivacionLibro(LibroDatos libroDatos)
        {
            try
            {
                ServidorElSaber.LibroManejadorClient libroManejadorClient = new ServidorElSaber.LibroManejadorClient();
                int resultadoModificacion = libroManejadorClient.CambiarEstadoDeLibro(libroDatos.isbn, "Desactivado");
                if(resultadoModificacion == 1)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoExito, "Libro desactivado", "El libro seleccionado se ha desactivado con éxito");
                    lw_libros.Items.Clear();
                }
                else if(resultadoModificacion == -1)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error en la conexión a la base de datos", "Se ha perdido la conexión a la base de datos");
                }
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

        private bool VerificarLibroSinPrestamosActivos(LibroDatos libroSeleccionado)
        {
            bool validacion = false;
            ServidorElSaber.PrestamoManejadorClient prestamoManejadorClient = new ServidorElSaber.PrestamoManejadorClient();
            try
            {
                List<PrestamoBinding> prestamosActivos = prestamoManejadorClient.ObtenerPrestamosActivosPorISBN(libroSeleccionado.isbn).ToList();
                if(prestamosActivos.Count == 0)
                {
                    validacion = true;
                }
                else if (prestamosActivos[0].IdPrestamo == -1)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error en la conexión a la base de datos", "Se ha perdido la conexión a la base de datos");

                }
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
            return validacion;
        }

        public void Activar_Click(object sender, EventArgs e)
        {
            Button botonPresionado = sender as Button;
            LibroDatos libroSeleccionado = botonPresionado.DataContext as LibroDatos;
            if (libroSeleccionado != null)
            {
                try
                {
                    ServidorElSaber.LibroManejadorClient libroManejadorClient = new ServidorElSaber.LibroManejadorClient();
                    int resultadoModificacion = libroManejadorClient.CambiarEstadoDeLibro(libroSeleccionado.isbn, "Disponible");
                    if (resultadoModificacion == 1)
                    {
                        VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoExito, "Libro Activado", "El libro ha sido activado de manera éxitosa");
                        lw_libros.Items.Clear();
                    }
                    else if (resultadoModificacion == -1)
                    {
                        VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error en la conexión a la base de datos", "Se ha perdido la conexión a la base de datos");
                    }
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
        }

        public void Regresar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CargarLibrosEncontrados(List<LibroBinding> libros)
        {
            lw_libros.Items.Clear();
            try
            {
                foreach (var libroObtenido in libros)
                {
                    DateTime fechaPublicacion;
                    if (!DateTime.TryParse(libroObtenido.AnioDePublicacion, out fechaPublicacion))
                    {
                        fechaPublicacion = DateTime.Now;
                    }
                    LibroManejadorClient libroManejadorClient = new LibroManejadorClient();
                    byte[] imagenLibro = libroManejadorClient.ObtenerImagenLibro(libroObtenido.RutaPortada);
                    if(imagenLibro == null)
                    {
                        imagenLibro = ObtenerImagenPorDefecto();
                    }
                    bool botonActivar = false;
                    bool botonDesactivar = false;
                    if(libroObtenido.Estado.Equals("Disponible") && _tienePermisos)
                    {
                        botonDesactivar = true;
                    }
                    else if (libroObtenido.Estado.Equals("Desactivado") && _tienePermisos)
                    {
                        botonActivar = true;
                    }
                    var libro = new LibroDatos()
                        {
                            idLibro = libroObtenido.idLibro,
                            titulo = libroObtenido.Titulo,
                            isbn = libroObtenido.Isbn,
                            autor = libroObtenido.autor,
                            editorial = libroObtenido.editorial,
                            genero = libroObtenido.genero,
                            fechaPublicacion = fechaPublicacion,
                            estado = libroObtenido.Estado,
                            Imagen = imagenLibro,
                            cantidadEjemplares = libroObtenido.CantidadEjemplares,
                            cantidadEjemplaresPrestados = libroObtenido.CantidadEjemplaresPrestados,
                            numeroDePaginas = libroObtenido.NumeroDePaginas,
                            rutaImagen = libroObtenido.RutaPortada,
                            MostrarBotonActivar = botonActivar,
                            MostrarBotonDesactivar = botonDesactivar
                        };

                    lw_libros.Items.Add(libro);
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

        private byte[] ObtenerImagenPorDefecto()
        {
            string rutaImagen = "ImagenesLibro\\ImagenPorDefecto.jpg";
            byte[] imagenObtenida = null;
            try
            {
                string[] extensiones = { ".jpg", ".jpeg", ".png" };
                string directorioBase = AppDomain.CurrentDomain.BaseDirectory;
                string servidorPath = Path.GetFullPath(Path.Combine(directorioBase, "../../"));
                string rutaDestino = Path.Combine(servidorPath, rutaImagen);
                foreach (var extension in extensiones)
                {
                    if (File.Exists(rutaDestino))
                    {
                        imagenObtenida = File.ReadAllBytes(rutaDestino);
                    }
                }
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                LoggerManager.Error("Error al obtener el archivo: "+ fileNotFoundException);
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                LoggerManager.Error("Acceso a datos no autorizado: "+ unauthorizedAccessException);
            }
            catch (IOException IOException)
            {
                LoggerManager.Error(IOException.Message);
            }
            return imagenObtenida;
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
            public byte[] Imagen { get; set; }
            public string rutaImagen { get; set; }
            public string numeroDePaginas {  get; set; }
            public int cantidadEjemplares { get; set; }
            public int cantidadEjemplaresPrestados { get; set; }
            public bool MostrarBotonDesactivar { get; set; }
            public bool MostrarBotonActivar { get; set; }
        }

    }
}
