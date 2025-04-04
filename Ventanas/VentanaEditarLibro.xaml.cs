using ClienteBibliotecaElSaber.ServidorElSaber;
using ClienteBibliotecaElSaber.Utilidades;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Media;

namespace ClienteBibliotecaElSaber.Ventanas
{
    public partial class VentanaEditarLibro : Window
    {
        private List<EditorialBinding> _editoriales;
        private List<AutorBinding> _autores;
        private List<GeneroBinding> _generos;
        private string _rutaDestinoCliente;
        private string _rutaArchivoOriginal;
        private LibroBinding libro;
        private byte[] nuevaImagen;

        public VentanaEditarLibro(LibroBinding datosDelLibro)
        {
            InitializeComponent();
            CargarDatosCombobox();
            CargarDatosDeLibro(datosDelLibro);
            this.libro = datosDelLibro;
        }

        private void CargarDatosCombobox()
        {
            try
            {
                LibroManejadorClient libroManejadorClient = new LibroManejadorClient();
                _autores = libroManejadorClient.ObtenerListaDeAutores().ToList();
                _editoriales = libroManejadorClient.ObtenerEditoriales().ToList();
                _generos = libroManejadorClient.ObtenerListaDeGeneros().ToList();
                cmb_Autor.ItemsSource = _autores;
                cmb_Editorial.ItemsSource = _editoriales;
                cmb_Genero.ItemsSource = _generos;
                cmb_Genero.DisplayMemberPath = "Genero";
                cmb_Autor.DisplayMemberPath = "Autor";
                cmb_Editorial.DisplayMemberPath = "Editorial";
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

        private void CargarDatosDeLibro(LibroBinding datosLibro)
        {
            int anio = int.Parse(datosLibro.AnioDePublicacion);
            DateTime anioDePublicacion = new DateTime(anio, 1, 1);
            txb_Titulo.Text = datosLibro.Titulo;
            txb_ISBN.Text = datosLibro.Isbn;
            txb_Estado.Text = datosLibro.Estado;
            txb_NumeroDePaginas.Text = datosLibro.NumeroDePaginas;
            txb_CantidadEjemplares.Text = datosLibro.CantidadEjemplares.ToString();
            txb_imagen.Text = datosLibro.RutaPortada.ToString();
            dtp_fechaAñoCreacionLibro.SelectedDate = anioDePublicacion;
            cmb_Autor.SelectedIndex = datosLibro.autor.IdAutor - 1;
            cmb_Editorial.SelectedIndex = datosLibro.editorial.IdEditorial - 1;
            cmb_Genero.SelectedIndex = datosLibro.genero.IdGenero - 1;
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
                if (tamanioEnMb > 2)
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

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            ResetearBordes();
            if (ValidarDatos())
            {
                if (!ValidarModificacionDeCampos())
                {
                    RealizarModificacionDatosLibro();
                }
                else
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Sin cambios realizados", "Para realizar una modificación, por favor modifique uno de los campos.");
                }
            }
            else
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoInformacion, "Datos inválidos", "Por favor verifique que los datos ingresados o seleccionados sean los correctos.");
            }
        }

        private void RealizarModificacionDatosLibro()
        {
            try
            {
                string resultadoGuardadoImagen = GuardarImagenDeLibro();
                if(resultadoGuardadoImagen != "-1")
                {
                    LibroManejadorClient libroManejadorClient = new LibroManejadorClient();
                    GeneroBinding genero = cmb_Genero.SelectedItem as GeneroBinding;
                    AutorBinding autor = cmb_Autor.SelectedItem as AutorBinding;
                    EditorialBinding editorial = cmb_Editorial.SelectedItem as EditorialBinding;
                    string fechaSeleccionada = dtp_fechaAñoCreacionLibro.SelectedDate.Value.ToString("yyyy");
                    LibroBinding libroBinding = new LibroBinding()
                    {
                        Titulo = txb_Titulo.Text,
                        Isbn = txb_ISBN.Text,
                        autor = autor,
                        editorial = editorial,
                        genero = genero,
                        AnioDePublicacion = fechaSeleccionada,
                        NumeroDePaginas = txb_NumeroDePaginas.Text,
                        RutaPortada = _rutaDestinoCliente,
                        Estado = "Disponible",
                        CantidadEjemplares = int.Parse(txb_CantidadEjemplares.Text)
                    };
                    int resultadoModificacion = libroManejadorClient.EditarDetallesDeLibro(txb_ISBN.Text, libroBinding);
                    if(resultadoModificacion == 1)
                    {
                        VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoExito, "Operacion exitosa", "Los datos han sido modificado de manera exitosa");
                        RegresarDeVentana();
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

        private void RegresarDeVentana()
        {
            this.Close();
            VentanaBuscarLibro ventanaBuscarLibro = new VentanaBuscarLibro();
            ventanaBuscarLibro.ShowDialog();
        }

        private string GuardarImagenDeLibro()
        {
            string resultadoGuardado = "-1";
            if(libro.RutaPortada != txb_imagen.Text)
            {
                try
                {
                    string extension = System.IO.Path.GetExtension(_rutaArchivoOriginal);
                    byte[] imagenEnBytes = File.ReadAllBytes(_rutaArchivoOriginal);
                    LibroManejadorClient libroManejadorClient = new LibroManejadorClient();
                    resultadoGuardado = libroManejadorClient.GuardarImagenLibro(txb_Titulo.Text, imagenEnBytes, extension);
                    _rutaDestinoCliente = resultadoGuardado;
                }
                catch (FileNotFoundException fileNotFoundException)
                {
                    LoggerManager.Error($"Excepción de FileNotFoundException: {fileNotFoundException.Message}." +
                                        $"\nTraza: {fileNotFoundException.StackTrace}.");
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Comunicacion fallida", "La comunicacion con el servidor se ha perdido, por favor verifique su conexión a internet.");

                }
                catch (IOException ioException)
                {
                    LoggerManager.Error($"Excepción de IOException: {ioException.Message}." +
                                        $"\nTraza: {ioException.StackTrace}.");
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Comunicacion fallida", "La comunicacion con el servidor se ha perdido, por favor verifique su conexión a internet.");

                }
            }
            else
            {
                resultadoGuardado = libro.RutaPortada;
                _rutaDestinoCliente = resultadoGuardado;
            }
            return resultadoGuardado;
        }

        private void ResetearBordes()
        {
            cmb_Autor.BorderBrush = Brushes.White;
            cmb_Editorial.BorderBrush = Brushes.White;
            cmb_Genero.BorderBrush = Brushes.White;
            txb_Titulo.BorderBrush = Brushes.White;
            txb_NumeroDePaginas.BorderBrush = Brushes.White;
            txb_CantidadEjemplares.BorderBrush = Brushes.White;
            dtp_fechaAñoCreacionLibro.BorderBrush = Brushes.White;
            txb_imagen.BorderBrush = Brushes.White;
        }

        private bool ValidarModificacionDeCampos()
        {
            AutorBinding autor = cmb_Autor.SelectedItem as AutorBinding;
            EditorialBinding editorial = cmb_Editorial.SelectedItem as EditorialBinding;
            GeneroBinding genero = cmb_Genero.SelectedItem as GeneroBinding;
            string anioPublicacion = dtp_fechaAñoCreacionLibro.SelectedDate.Value.ToString("yyyy");
            return txb_Titulo.Text != libro.Titulo && txb_CantidadEjemplares.Text != libro.CantidadEjemplares.ToString() && txb_NumeroDePaginas.Text != libro.NumeroDePaginas.ToString()
                && txb_imagen.Text != libro.RutaPortada && anioPublicacion != libro.AnioDePublicacion.ToString() && autor.IdAutor != libro.autor.IdAutor && editorial.IdEditorial != libro.editorial.IdEditorial
                && genero.IdGenero != libro.genero.IdGenero;
        }

        private bool ValidarDatos()
        {
            AutorBinding autor = cmb_Autor.SelectedItem as AutorBinding;
            EditorialBinding editorialBinding = cmb_Editorial.SelectedItem as EditorialBinding;
            string fechaDePublicacion = dtp_fechaAñoCreacionLibro.SelectedDate.Value.ToString("yyyy-MM-dd");
            bool validarTitulo = Validador.ValidarTitulo(txb_Titulo.Text);
            bool validarNoPaginas = Validador.ValidarSoloNumeros(txb_NumeroDePaginas.Text);
            bool validarNoEjemplares = Validador.ValidarSoloNumeros(txb_CantidadEjemplares.Text);
            bool validarFechaDePublicacion = Validador.ValidarFechas(fechaDePublicacion);
            bool validarAutor = Validador.ValidarNombreAutor(autor.Autor);
            bool validarEditorial = Validador.ValidarNombre(editorialBinding.Editorial);

            if (!validarAutor)
            {
                cmb_Autor.BorderBrush = Brushes.Red;
            }

            if (!validarEditorial)
            {
                cmb_Editorial.BorderBrush = Brushes.Red;
            }

            if (cmb_Genero.SelectedItem == null)
            {
                cmb_Genero.BorderBrush = Brushes.Red;
            }

            if (!validarTitulo)
            {
                txb_Titulo.BorderBrush = Brushes.Red;
            }

            if (!validarNoPaginas)
            {
                txb_NumeroDePaginas.BorderBrush = Brushes.Red;
            }

            if (!validarNoEjemplares)
            {
                txb_CantidadEjemplares.BorderBrush = Brushes.Red;
            }

            if (!validarFechaDePublicacion)
            {
                dtp_fechaAñoCreacionLibro.BorderBrush = Brushes.Red;
            }

            if (txb_imagen.Text == " ")
            {
                txb_imagen.BorderBrush = Brushes.Red;
            }

            return validarAutor && validarEditorial && autor != null && editorialBinding != null && cmb_Genero.SelectedItem != null
                && validarNoPaginas && validarNoEjemplares && validarFechaDePublicacion && txb_imagen.Text != " ";
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            VentanaBuscarLibro ventanaBuscarLibro = new VentanaBuscarLibro();
            ventanaBuscarLibro.ShowDialog();
        }
    }
}
