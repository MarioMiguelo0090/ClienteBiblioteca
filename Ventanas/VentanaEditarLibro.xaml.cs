using ClienteBibliotecaElSaber.ServidorElSaber;
using ClienteBibliotecaElSaber.Utilidades;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Windows;

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
                cmb_Autor.DisplayMemberPath = "Genero";
                cmb_Autor.DisplayMemberPath = "Autor";
                cmb_Autor.DisplayMemberPath = "Editorial";
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
            DateTime anioDePublicacion = new DateTime(anio,1,1);
            txb_Titulo.Text = datosLibro.autor.Autor;
            txb_ISBN.Text = datosLibro.Isbn;
            txb_Estado.Text = datosLibro.Estado;
            txb_NumeroDePaginas.Text = datosLibro.NumeroDePaginas;
            txb_CantidadEjemplares.Text = datosLibro.CantidadEjemplares.ToString();
            txb_imagen.Text = datosLibro.RutaPortada.ToString();
            dtp_fechaAñoCreacionLibro.SelectedDate = anioDePublicacion;
            cmb_Editorial.SelectedItem = datosLibro.editorial;
            cmb_Autor.SelectedItem = datosLibro.autor;
            cmb_Genero.SelectedItem = datosLibro.genero;
        }

        private void SubirImagen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
