using Microsoft.Win32;
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
using System.IO;
using ClienteBibliotecaElSaber.Utilidades;
using System.ServiceModel;
using ClienteBibliotecaElSaber.ServidorElSaber;
using System.Runtime.CompilerServices;
using System.Net.NetworkInformation;

namespace ClienteBibliotecaElSaber.Ventanas
{
    /// <summary>
    /// Lógica de interacción para VentanaRegistroLibro.xaml
    /// </summary>
    public partial class VentanaRegistroLibro : Window
    {
        public VentanaRegistroLibro()
        {
            InitializeComponent();
            CargarDatos();
            dp_FechaLanzamiento.DisplayDateStart = new DateTime(1920, 1, 1);
            dp_FechaLanzamiento.DisplayDateEnd = DateTime.Today;
            dp_FechaLanzamiento.SelectedDate = DateTime.Today;
        }

        private List<EditorialBinding> _editoriales;
        private List<AutorBinding> _autores;
        private List<GeneroBinding> _generos;
        private string _rutaDestinoCliente;
        private string _rutaArchivoOriginal;

        private void CargarDatos()
        {
            try
            {
                LibroManejadorClient libroManejadorClient = new LibroManejadorClient();
                _autores = libroManejadorClient.ObtenerListaDeAutores().ToList();
                _editoriales = libroManejadorClient.ObtenerEditoriales().ToList();
                _generos = libroManejadorClient.ObtenerListaDeGeneros().ToList();   
                cbAutor.ItemsSource = _autores;
                cbEditorial.ItemsSource = _editoriales;
                cbGenero.ItemsSource = _generos;
                cbGenero.DisplayMemberPath = "Genero";
                cbAutor.DisplayMemberPath = "Autor";
                cbEditorial.DisplayMemberPath = "Editorial";
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


        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarDatos())
            {
                int resultadoValidacion = ValidarExistenciaDeLibro();
                if(resultadoValidacion == 0)
                {
                    RealizarRegistroDeLibro();
                }
                else if (resultadoValidacion >= 1)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Datos duplicados", "El ISBN que desea ingresar, ya ha sido registrado previamente o verifique que los datos sean correctos.");
                    
                }
            }
            else
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Datos incorrectos", "Por favor verifique que los datos ingresados sean los correctos.");
            }
        }

        public void RealizarRegistroDeLibro()
        {
            try
            {
                int resultadoGuardadoImagen = GuardarImagenDeLibro();
                if(resultadoGuardadoImagen != -1)
                {
                    LibroManejadorClient libroManejadorClient = new LibroManejadorClient();
                    GeneroBinding genero = cbGenero.SelectedItem as GeneroBinding;
                    AutorBinding autor = cbAutor.SelectedItem as AutorBinding;
                    EditorialBinding editorial = cbEditorial.SelectedItem as EditorialBinding;
                    string fechaSeleccionada = dp_FechaLanzamiento.SelectedDate.Value.ToString("yyyy");
                    LibroBinding libroBinding = new LibroBinding()
                    {
                        Titulo = txb_titulo.Text,
                        Isbn = txb_isbn.Text,
                        autor = autor,
                        editorial = editorial,
                        genero = genero,
                        AnioDePublicacion = fechaSeleccionada,
                        NumeroDePaginas = txb_noPaginas.Text,
                        RutaPortada = _rutaDestinoCliente,
                        Estado = "Disponible",
                        CantidadEjemplares = int.Parse(txb_noEjemplares.Text)
                    };
                    int resultadoRegistro = libroManejadorClient.RegistrarNuevoLibro(libroBinding);
                    if(resultadoRegistro == 1)
                    {
                        VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoExito, "Datos ingresados", "Los datos se han registradod de manera exitosa");     
                    }
                    else
                    {
                        VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error en la conexión a la base de datos", "Se ha perdido la conexión a la base de datos");     
                    }
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

        private int GuardarImagenDeLibro()
        {
            int resultadoGuardado = -1;
            try
            {
                string directorioDestino = System.IO.Path.GetDirectoryName(_rutaDestinoCliente);
                string extension = System.IO.Path.GetExtension(_rutaArchivoOriginal);
                string nuevaRutaDestino = System.IO.Path.Combine(directorioDestino, txb_titulo.Text + extension);
                File.Copy(_rutaArchivoOriginal, nuevaRutaDestino, true);
                resultadoGuardado = 1;
            }
            catch(FileNotFoundException fileNotFoundException)
            {
                LoggerManager.Error($"Excepción de FileNotFoundException: {fileNotFoundException.Message}." +
                                    $"\nTraza: {fileNotFoundException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Comunicacion fallida", "La comunicacion con el servidor se ha perdido, por favor verifique su conexión a internet.");
                
            }
            catch(IOException ioException)
            {
                LoggerManager.Error($"Excepción de IOException: {ioException.Message}." +
                                    $"\nTraza: {ioException.StackTrace}.");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Comunicacion fallida", "La comunicacion con el servidor se ha perdido, por favor verifique su conexión a internet.");
                
            }
            return resultadoGuardado;
        }

        private int ValidarExistenciaDeLibro()
        {
            int resultadoValidacion = -1;
            try
            {
                LibroManejadorClient libroManejadorClient = new LibroManejadorClient();
                int idLibro = libroManejadorClient.ObtenerIdLibroPorISBN(txb_isbn.Text);
                if(idLibro == -1)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente("Error", "Error en la conexión a la base de datos", "Se ha perdido la conexión a la base de datos");
                    
                }
                else
                {
                    resultadoValidacion = idLibro;
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
            return resultadoValidacion;
        }

        private bool ValidarDatos()
        {
            AutorBinding autor = cbAutor.SelectedItem as AutorBinding;
            EditorialBinding editorialBinding = cbEditorial.SelectedItem as EditorialBinding;
            string fechaDePublicacion = dp_FechaLanzamiento.SelectedDate.Value.ToString("yyyy-MM-dd");
            bool validarTitulo = Validador.ValidarTitulo(txb_titulo.Text);
            bool validarNoPaginas = Validador.ValidarSoloNumeros(txb_noPaginas.Text);
            bool validarNoEjemplares = Validador.ValidarSoloNumeros(txb_noEjemplares.Text);
            bool validarIsbn = Validador.ValidarISBN(txb_isbn.Text);
            bool validarFechaDePublicacion = Validador.ValidarFechas(fechaDePublicacion);
            bool validarAutor = Validador.ValidarNombre(autor.Autor);
            bool validarEditorial = Validador.ValidarNombre(editorialBinding.Editorial);

            if (!validarAutor)
            {
                cbAutor.BorderBrush = Brushes.Red;
            }

            if (!validarEditorial)
            {
                cbEditorial.BorderBrush = Brushes.Red;
            }

            if (cbGenero.SelectedItem == null)
            {
                cbGenero.BorderBrush = Brushes.Red;
            }

            if (!validarTitulo)
            {
                txb_titulo.BorderBrush = Brushes.Red;
            }

            if (!validarNoPaginas)
            {
                txb_noPaginas.BorderBrush = Brushes.Red;    
            }

            if (!validarNoEjemplares)
            {
                txb_noEjemplares.BorderBrush = Brushes.Red;
            }

            if (!validarIsbn)
            {
                txb_isbn.BorderBrush = Brushes.Red;    
            }

            if (!validarFechaDePublicacion)
            {
                brd_dp.BorderBrush = Brushes.Red;   
            }

            if(txb_imagen.Text == " ")
            {
                txb_imagen.BorderBrush = Brushes.Red;
            }

            return validarAutor && validarEditorial && autor != null &&  editorialBinding != null && cbGenero.SelectedItem != null 
                && validarNoPaginas && validarNoEjemplares && validarIsbn && validarFechaDePublicacion && txb_imagen.Text != " ";
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AgregarEditorial_Click(object sender, RoutedEventArgs e)
        {
            VentanaRegistrarEditorial ventanaRegistrarEditorial = new VentanaRegistrarEditorial();
            ventanaRegistrarEditorial.ShowDialog();
        }

        private void AgregarAutor_Click(object sender, RoutedEventArgs e)
        {
            VentanaRegistrarAutor ventanaRegistrarAutor = new VentanaRegistrarAutor();
            ventanaRegistrarAutor.ShowDialog();
        }

        private void SubirImagen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Seleccionar imagen",
                Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png",
                Multiselect = false,
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _rutaArchivoOriginal = openFileDialog.FileName;
                long tamanioArchivoImagen = new FileInfo(_rutaArchivoOriginal).Length;
                double tamanioEnMb = tamanioArchivoImagen / (1024.0 * 1024.0);
                if( tamanioEnMb > 2)
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Imagen pesada", "Solo se admiten imagenes menores a 2MB");
                    
                }
                else
                {
                    string nombreArchivo = System.IO.Path.GetFileName(_rutaArchivoOriginal);
                    string directorioBase = AppDomain.CurrentDomain.BaseDirectory;
                    string rutaDestino = System.IO.Path.GetFullPath(System.IO.Path.Combine(directorioBase, "../../"));
                    string rutaFinalDestino = System.IO.Path.Combine(rutaDestino, "ImagenesLibro");
                    if (!Directory.Exists(rutaDestino))
                    {
                        Directory.CreateDirectory(rutaDestino);
                    }
                    _rutaDestinoCliente = System.IO.Path.Combine(rutaFinalDestino, nombreArchivo);
                    txb_imagen.Text = System.IO.Path.GetFileName(_rutaArchivoOriginal);
                }
            }
        }

        private void cbEditorial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
