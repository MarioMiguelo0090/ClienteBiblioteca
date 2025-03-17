using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClienteBibliotecaElSaber.Utilidades
{
    public static class GeneradorContrasenia
    {
        private static Regex _ContraseniaRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?])[^ ]{8,10}$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));

        public static string GenerarContrasenia()
        {
            const string minusculas = "abcdefghijklmnopqrstuvwxyz";
            const string mayusculas = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numeros = "0123456789";
            const string especiales = "!@#$%()_+-=[]{};<>?";
            const string todos = minusculas + mayusculas + numeros + especiales;
            string resultado = string.Empty;
            while (!_ContraseniaRegex.IsMatch(resultado))
            {
                Random rand = new Random();
                int longitud = rand.Next(8, 11);
                char unaMinuscula = minusculas[rand.Next(minusculas.Length)];
                char unaMayuscula = mayusculas[rand.Next(mayusculas.Length)];
                char unNumero = numeros[rand.Next(numeros.Length)];
                char unEspecial = especiales[rand.Next(especiales.Length)];
                char[] contrasenia = new char[longitud];
                contrasenia[0] = unaMinuscula;
                contrasenia[1] = unaMayuscula;
                contrasenia[2] = unNumero;
                contrasenia[3] = unEspecial;
                for (int i = 4; i < longitud; i++)
                {
                    contrasenia[i] = todos[rand.Next(todos.Length)];
                }
                contrasenia = contrasenia.OrderBy(x => rand.Next()).ToArray();
                resultado = new string(contrasenia);
            }
            return resultado;
        }
    }
}
