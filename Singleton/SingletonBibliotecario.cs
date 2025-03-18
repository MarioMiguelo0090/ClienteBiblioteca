using ClienteBibliotecaElSaber.ServidorElSaber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClienteBibliotecaElSaber.Singleton
{
    public class SingletonBibliotecario
    {
        private static SingletonBibliotecario _instancia;
        private static readonly object _lock = new object();

        public int IdAcceso { get; private set; }
        public string Correo { get; private set; }
        public string TipoDeUsuario { get; private set; }
        UsuarioBinding Usuario { get; set; }

        private SingletonBibliotecario() { }

        public static SingletonBibliotecario Instancia
        {
            get
            {
                lock (_lock)
                {
                    if (_instancia == null)
                    {
                        _instancia = new SingletonBibliotecario();
                    }
                    return _instancia;
                }
            }
        }

        public void IniciarSesion(AccesoBinding acceso)
        {
            IdAcceso = acceso.IdAcceso;
            Correo = acceso.correo;
            TipoDeUsuario = acceso.tipoDeUsuario;
            Usuario = acceso.IdUsuario;
        }

        public void CerrarSesion()
        {
            _instancia = null;
        }
    }
}
