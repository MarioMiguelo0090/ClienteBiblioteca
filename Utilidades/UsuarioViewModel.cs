using ClienteBibliotecaElSaber.ServidorElSaber;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClienteBibliotecaElSaber.Utilidades
{
    public class UsuarioViewModel
    {
        public ObservableCollection<UsuarioBinding> Usuarios {  get; set; }

        public UsuarioViewModel()
        {
            Usuarios = new ObservableCollection<UsuarioBinding>
            {
                new UsuarioBinding
                {
                    nombre = "Fernando",
                    primerApellido = "Gutierrez",
                    segundoApellido = "Hernandez",
                    puesto = "Administrador"
                }
            };
        }
    }
}
