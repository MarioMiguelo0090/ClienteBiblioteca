using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClienteBibliotecaElSaber.Utilidades
{
    public static class ValidadorPrestamo
    {
        private static Regex _numeroSocioRegex = new Regex(@"^\d{1,6}$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _isbnRegex = new Regex(@"^\d{10}|\d{13}$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _notasRegex = new Regex(@"^[a-zA-Z0-9ñÑáéíóúÁÉÍÓÚ\s.,'-]{0,255}$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));

        public static bool ValidarNumeroSocio(string numeroSocio)
        {
            return !string.IsNullOrWhiteSpace(numeroSocio) && _numeroSocioRegex.IsMatch(numeroSocio);
        }

        public static bool ValidarISBN(string isbn)
        {
            return !string.IsNullOrWhiteSpace(isbn) && _isbnRegex.IsMatch(isbn);
        }

        public static bool ValidarNotas(string notas)
        {
            return notas.Length <= 255 && _notasRegex.IsMatch(notas);
        }
    }
}
