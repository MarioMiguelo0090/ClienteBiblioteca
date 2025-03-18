﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ClienteBibliotecaElSaber.Utilidades
{
    public static class Validador
    {
        private static Regex _nombreRegex = new Regex(@"^[a-zA-ZñÑáéíóúÁÉÍÓÚ'’-]+(?:\s[a-zA-ZñÑáéíóúÁÉÍÓÚ'’-]+)*$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _primerApellidoRegex = new Regex(@"^[a-zA-ZñÑáéíóúÁÉÍÓÚ'’-]+(?:\s[a-zA-ZñÑáéíóúÁÉÍÓÚ'’-]+)*$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _segundoApellidoRegex = new Regex(@"^[a-zA-ZñÑáéíóúÁÉÍÓÚ'’-]+(?:\s[a-zA-ZñÑáéíóúÁÉÍÓÚ'’-]+)*$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _numeroTelefonicoRegex = new Regex(@"^[0-9]{10}$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _correoRegex = new Regex(@"^[a-zA-Z0-9](?!.*[.-]{2})[a-zA-Z0-9.-]*[a-zA-Z0-9]@[a-zA-Z0-9](?!.*[.-]{2})[a-zA-Z0-9.-]*\.[a-zA-Z]{2,}$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _direccionRegex = new Regex(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ0-9,.'""\-\s]+$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _codigoPostalRegex = new Regex(@"^[0-9]{5}$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _fechaRegex = new Regex(@"^(19|20)\d{2}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _numeroCalleRegex = new Regex(@"^[0-9]{3}$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));

        public static bool ValidarPatronRegex(string datos,Regex regex)
        {
            bool esValido = false;
            try
            {
                esValido = regex.IsMatch(datos);
            }
            catch (RegexMatchTimeoutException)
            {
                esValido = false;
            }
            return esValido;
        }

        public static bool ValidarNombre(string nombre)
        {
            bool esValido = false;
            string nombreLimpio = Regex.Replace(nombre.Trim(),@"\s+"," ",RegexOptions.None,TimeSpan.FromMilliseconds(1000));
            if (!string.IsNullOrWhiteSpace(nombreLimpio) && ValidarPatronRegex(nombreLimpio,_nombreRegex))
            {
                esValido = true;
            }
            return esValido;
        }

        public static bool ValidarPrimerApellido(string primerApellido)
        {
            bool esValido = false;
            string primerApellidoLimpio = Regex.Replace(primerApellido.Trim(), @"\s+", " ", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
            if (!string.IsNullOrWhiteSpace(primerApellidoLimpio) && ValidarPatronRegex(primerApellidoLimpio, _primerApellidoRegex))
            {
                esValido = true;
            }
            return esValido;
        }

        public static bool ValidarSegundoApellido(string segundoApellido)
        {
            bool esValido = false;
            string segundoApellidoLimpio = Regex.Replace(segundoApellido.Trim(), @"\s+", " ", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
            if (string.IsNullOrWhiteSpace(segundoApellido))
            {
                esValido = true;
            }
            else if(ValidarPatronRegex(segundoApellidoLimpio,_segundoApellidoRegex))
            { 
               esValido= true;
            }
            return esValido;
        }

        public static bool ValidarTelefono(string telefono)
        {
            bool esValido = false;
            string telefonoLimpio = Regex.Replace(telefono.Trim(), @"\s+", " ", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
            if (!string.IsNullOrWhiteSpace(telefonoLimpio) && ValidarPatronRegex(telefonoLimpio,_numeroTelefonicoRegex))
            {
                esValido = true;
            }
            return esValido;
        }

        public static bool ValidarDireccion(string direccion)
        {
            bool esValido = false;
            string direccionLimpia = Regex.Replace(direccion.Trim(), @"\s+", " ", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
            if (!string.IsNullOrWhiteSpace(direccionLimpia) && ValidarPatronRegex(direccionLimpia, _direccionRegex))
            {
                esValido = true;
            }
            return esValido;
        }

        public static bool ValidarCodigoPostal(string codigoPostal)
        {
            bool esValido = false;
            string codigoPostalLimpio = Regex.Replace(codigoPostal.Trim(), @"\s+", " ", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
            if (!string.IsNullOrWhiteSpace(codigoPostalLimpio) && ValidarPatronRegex(codigoPostalLimpio, _codigoPostalRegex))
            {
                esValido = true;
            }
            return esValido;
        }

        public static bool ValidarFechas(string fecha)
        {
            bool esValido = false;
            if (!string.IsNullOrWhiteSpace(fecha) && ValidarPatronRegex(fecha, _fechaRegex))
            {
                esValido = true;
            }
            return esValido;
        }

        public static bool ValidarCorreo(string correo)
        {
            bool esValido = false;
            string correoLimpio = Regex.Replace(correo.Trim(), @"\s+", " ", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
            if (!string.IsNullOrWhiteSpace(correoLimpio) && ValidarPatronRegex(correoLimpio, _correoRegex))
            {
                esValido = true;
            }
            return esValido;
        }

        public static bool ValidarCalle(string calle)
        {
            bool esValido = false;
            string calleLimpio = Regex.Replace(calle.Trim(), @"\s+", " ", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
            if (!string.IsNullOrWhiteSpace(calleLimpio) && ValidarPatronRegex(calleLimpio, _direccionRegex))
            {
                esValido = true;
            }
            return esValido;
        }

        public static bool ValidarNumeroCalle(string numeroCalle)
        {
            bool esValido = false;
            string numeroCalleLimpio = Regex.Replace(numeroCalle.Trim(), @"\s+", " ", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
            if (!string.IsNullOrWhiteSpace(numeroCalleLimpio) && ValidarPatronRegex(numeroCalleLimpio, _numeroCalleRegex))
            {
                esValido = true;
            }
            return esValido;
        }
    }
}
