using ClienteBibliotecaElSaber.ServidorElSaber;
using ClienteBibliotecaElSaber.Singleton;
using ClienteBibliotecaElSaber.Utilidades;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public partial class VentanaBuscarUsuario : Window
    {
        private ICollectionView vistaFiltrada;
        private ObservableCollection<AccesoBinding> accesos;
        public ObservableCollection<AccesoBinding> Accesos 
        {
            get { return accesos; }
            set
            {
                accesos = value;
                OnPropertyChanged(nameof(Accesos));
            }
        } 
        
        public event PropertyChangedEventHandler PropertyChanged;

        public VentanaBuscarUsuario()
        {
            InitializeComponent();
            this.DataContext = this;
            Accesos = new ObservableCollection<AccesoBinding>();
            ObtenerUsuarios();

            vistaFiltrada = CollectionViewSource.GetDefaultView(Accesos);
            vistaFiltrada.Filter = FiltroUsuarios;
        }

        private bool FiltroUsuarios(object item)
        {
            var usuario = item as AccesoBinding;
            if (usuario == null) return false;

            string filtroTipoUsuario = rb_Todos.IsChecked == true ? "" :
                                       rb_Administrador.IsChecked == true ? rb_Administrador.Content.ToString() :
                                       rb_Bibliotecario.IsChecked == true ? rb_Bibliotecario.Content.ToString() : "";

            bool cumpleFiltroTipoUsuario = string.IsNullOrEmpty(filtroTipoUsuario) || 
                usuario.tipoDeUsuario.Equals(filtroTipoUsuario, StringComparison.OrdinalIgnoreCase);
            bool cumpleFiltroBusqueda = string.IsNullOrEmpty(txtb_Buscador.Text) || 
                                        usuario.IdUsuario.nombre.IndexOf(txtb_Buscador.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                        usuario.IdUsuario.primerApellido.IndexOf(txtb_Buscador.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                        usuario.IdUsuario.segundoApellido.IndexOf(txtb_Buscador.Text, StringComparison.OrdinalIgnoreCase) >= 0;

            return cumpleFiltroTipoUsuario && cumpleFiltroBusqueda;
        }

        private void txtbBuscador_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (vistaFiltrada != null)
            {
                vistaFiltrada.Filter = FiltroUsuarios;
                vistaFiltrada.Refresh();
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (vistaFiltrada != null)
            {
                vistaFiltrada.Filter = FiltroUsuarios;
                vistaFiltrada.Refresh();
            }
        }

        private void ObtenerUsuarios()
        {
            var proxyAcceso = new AccesoManejadorClient();

            try
            {
                List<AccesoBinding> listaAccesos = proxyAcceso.ObtenerUsuarios().ToList();
                if (!(listaAccesos.First().IdAcceso == -1))
                {
                    Accesos.Clear();

                    foreach (var acceso in listaAccesos)
                    {
                        if (acceso != null && acceso.IdAcceso != -1)
                        {
                            Accesos.Add(acceso);
                        }
                    }
                }
                else
                {
                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError,
                        Constantes.TituloExcepcionBD, Constantes.ContenidoExcepcionBD);
                }
            }
            catch (Exception ex) when (ex is EndpointNotFoundException || ex is TimeoutException || ex is CommunicationException)
            {
                LoggerManager.Error($"Excepción: {ex.Message}\nTraza: {ex.StackTrace}");
                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError,
                    Constantes.TituloExcepcionServidor, Constantes.ContenidoExcepcionServidor);
            }
            finally
            {
                if (proxyAcceso.State == CommunicationState.Opened)
                {
                    proxyAcceso.Close();
                }
            }
        }

        private void MostrarFiltro_Click(object sender, EventArgs e)
        {
            if (canvas_Filtro.IsVisible)
            {
                canvas_Filtro.Visibility = Visibility.Collapsed;
            }
            else
            {
                canvas_Filtro.Visibility = Visibility.Visible;
            }
            
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            Button boton = sender as Button;
            if (boton != null)
            {
                AccesoBinding acceso = (AccesoBinding)listaUsuarios.SelectedItem;
                if (acceso != null)
                {
                    IrVentanaEdicionUsuario(acceso);
                }
            }
        }

        private void IrVentanaEdicionUsuario(AccesoBinding acceso)
        {
            VentanaEdicionUsuario ventanaEdicionUsuario = new VentanaEdicionUsuario(acceso);
            ventanaEdicionUsuario.ShowDialog();
            ObtenerUsuarios();
            listaUsuarios.Items.Refresh();
            this.Show();
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            Button boton = sender as Button;
            if (boton != null)
            {
                AccesoBinding acceso = (AccesoBinding)listaUsuarios.SelectedItem;
                if (acceso != null)
                {
                    if (SingletonAdministrador.Instancia.IdAcceso != acceso.IdAcceso)
                    {
                        VentanaDeConfirmacion ventanaDeConfirmacion = new VentanaDeConfirmacion("Confirmación de eliminación",
                            $"¿Desea eliminar al {acceso.tipoDeUsuario} {acceso.IdUsuario.nombre} {acceso.IdUsuario.primerApellido} {acceso.IdUsuario.segundoApellido}".Trim() + "?");
                        bool? respuesta = ventanaDeConfirmacion.ShowDialog();

                        if (respuesta == true)
                        {
                            var proxyUsuario = new UsuarioManejadorClient();
                            try
                            {
                                int resultado = proxyUsuario.DesactivarUsuarioPorIdUsuario(acceso.IdAcceso);

                                if (resultado == 1)
                                {
                                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoExito, "Eliminación exitosa",
                                        "Se ha eliminado al usuario de forma exitosa");
                                    ObtenerUsuarios();
                                    listaUsuarios.Items.Refresh();
                                }
                                else if (resultado == 0)
                                {
                                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, "Error inesperado",
                                        "Ha ocurrido un error inesperado al eliminar al usuario, por favor intentelo más tarde");
                                }
                                else
                                {
                                    VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError, Constantes.TituloExcepcionBD, Constantes.ContenidoExcepcionBD);
                                }
                            }
                            catch (Exception ex) when (ex is EndpointNotFoundException || ex is TimeoutException || ex is CommunicationException)
                            {
                                LoggerManager.Error($"Excepción: {ex.Message}\nTraza: {ex.StackTrace}");
                                VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoError,
                                    Constantes.TituloExcepcionServidor, Constantes.ContenidoExcepcionServidor);
                            }
                            finally
                            {
                                if (proxyUsuario.State == CommunicationState.Opened)
                                {
                                    proxyUsuario.Close();
                                }
                            }
                        }
                    }
                    else
                    {
                        VentanaEmergente ventanaEmergente = new VentanaEmergente(Constantes.TipoAdvertencia, "Error de eliminación",
                            "No es posible eliminar su propio usuario, favor de seleccionar otro");
                    }
                }
            }
        }

        private void Regresar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
