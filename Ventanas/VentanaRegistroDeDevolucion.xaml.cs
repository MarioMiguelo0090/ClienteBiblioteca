using ClienteBibliotecaElSaber.ServidorElSaber;
using ClienteBibliotecaElSaber.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
    /// Lógica de interacción para VentanaRegistroDeDevolucion.xaml
    /// </summary>
    public partial class VentanaRegistroDeDevolucion : Window
    {
        private int idPrestamo;

        public VentanaRegistroDeDevolucion(int idPrestamo)
        {
            InitializeComponent();
            this.idPrestamo = idPrestamo;            
        }

        private void RegistrarDevolucion(object sender, RoutedEventArgs e)
        {
            if (ValidarInformacion()) 
            {
                DevolucionBinding devolucion=ObtenerDatosDevolucion();
                DevolucionManejadorClient devolucionManejador = new DevolucionManejadorClient();
                try 
                {
                    int resultadoInsercion=devolucionManejador.RegistrarNuevaDevolucion(devolucion);
                    if (resultadoInsercion == Constantes.OperacionExitosa)
                    {
                        VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoExito
                        , "Registro de devolución", "La devolución ha sido registrada.");
                        
                    }
                    else 
                    {
                        VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                        , "Error de base de datos", "No se ha podido establecer conexión a la base de datos, inténtelo de nuevo más tarde.");
                        
                    }
                }
                catch (TimeoutException excepcionTiempo)
                {
                    LoggerManager.Error($"Excepcion: {excepcionTiempo.Message}\nTraza {excepcionTiempo.StackTrace}");
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                        , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");
                    
                }
                catch (EndpointNotFoundException puntoNoEncontrado)
                {
                    LoggerManager.Error($"Excepcion: {puntoNoEncontrado.Message}\nTraza {puntoNoEncontrado.StackTrace}");
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                        , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");
                    
                }
                catch (CommunicationException excecpionComunicacion)
                {
                    LoggerManager.Error($"Excepcion: {excecpionComunicacion.Message}\nTraza {excecpionComunicacion.StackTrace}");
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError
                        , "Error de servidor", "El servidor esta inactivo. Por favor, inténtelo más tarde.");
                    
                }
                RegresaVentanaMenuPrincipal();
            }
            else 
            {
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia
                    , "Datos inválidos", "Los datos que ha ingresado no son los correctos, inténtelo de nuevo.");
                
            }
        }

        private void CancelarRegistroDevolucion(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool ValidarInformacion() 
        {
            bool validacionNotas=ValidadorPrestamo.ValidarNotas(Txb_Notas.Text);
            if (!validacionNotas) 
            {
                Txb_Notas.BorderBrush = Brushes.Red;
            }
            return validacionNotas;
        }

        private DevolucionBinding ObtenerDatosDevolucion() 
        {
            DevolucionBinding devolucion = new DevolucionBinding
            {
                FK_IdPrestamo = idPrestamo,
                Nota = Txb_Notas.Text,
                FechaDevolucion = DateTime.Today,
                EstadoLibro = Chb_Daniado.IsChecked == true ? Enumeradores.Libro.Daniado.ToString() : Enumeradores.Libro.Disponible.ToString(),
            };
            return devolucion;
        }

        private void RegresaVentanaMenuPrincipal()
        {
            this.Hide();
            VentanaMenuPrincipalBibliotecario ventanaMenuPrincipalBibliotecario = new VentanaMenuPrincipalBibliotecario();
            ventanaMenuPrincipalBibliotecario.ShowDialog();
            this.Show();
        }
    }
}
