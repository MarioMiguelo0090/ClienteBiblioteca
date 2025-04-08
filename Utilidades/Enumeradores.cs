using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClienteBibliotecaElSaber.Utilidades
{
    public class Enumeradores
    {
        public enum EstadoUsuario
        {
            Activo,
            Desactivado
        }

        public enum Libro
        {
            Daniado,
            Disponible,
        }

        public enum Prestamo 
        {
            Activo,
            Vencido,
        }
    }
}
