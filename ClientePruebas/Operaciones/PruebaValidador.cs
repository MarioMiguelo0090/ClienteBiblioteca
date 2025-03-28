using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ClienteBibliotecaElSaber;
using ClienteBibliotecaElSaber.Utilidades;
using System.Text.RegularExpressions;

namespace ClientePruebas.Operaciones
{
    public class PruebaValidador
    {

        private static Regex _nombreRegex = new Regex(@"^[a-zA-ZñÑáéíóúÁÉÍÓÚ'’-]+(?:\s[a-zA-ZñÑáéíóúÁÉÍÓÚ'’-]+)*$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _primerApellidoRegex = new Regex(@"^[a-zA-ZñÑáéíóúÁÉÍÓÚ'’-]+(?:\s[a-zA-ZñÑáéíóúÁÉÍÓÚ'’-]+)*$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _segundoApellidoRegex = new Regex(@"^[a-zA-ZñÑáéíóúÁÉÍÓÚ'’-]+(?:\s[a-zA-ZñÑáéíóúÁÉÍÓÚ'’-]+)*$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _numeroTelefonicoRegex = new Regex(@"^[0-9]{10}$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _direccionRegex = new Regex(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ0-9,.'""\-\s]+$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _codigoPostalRegex = new Regex(@"^[0-9]{5}$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _fechaRegex = new Regex(@"^(19|20)\d{2}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _numeroCasaRegex = new Regex(@"^[0-9]{1,3}$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _numeroSocioRegex = new Regex(@"^[0-9]{1,5}$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _editorialRegex = new Regex(@"^[a-zA-Z0-9\s]+$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _TituloLibroRegex = new Regex(@"^[\p{L}0-9\s'’""“”\-:;.,!?()]+$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _isbnRegex = new Regex(@"^[0-9]{13}$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
        private static Regex _soloNumerosRegex = new Regex(@"^[0-9]{1,5}$", RegexOptions.None, TimeSpan.FromMilliseconds(1000));

        [Fact]
        public void ValidarNombreExitoso()
        {
            string nombreValido = "Juan Pérez";
            bool esValido = _nombreRegex.IsMatch(nombreValido);
            Assert.True(esValido, "El nombre debería ser válido");
        }

        [Fact]
        public void ValidarPrimerApellido_Exitoso()
        {
            string primerApellidoValido = "Gómez";
            bool resultado = ClienteBibliotecaElSaber.Utilidades.Validador.ValidarPrimerApellido(primerApellidoValido);
            Assert.True(resultado, "El primer apellido debería ser válido.");
        }

        [Fact]
        public void ValidarSegundoApellido_Exitoso()
        {
            string segundoApellidoValido = "Fernández";
            bool resultado = ClienteBibliotecaElSaber.Utilidades.Validador.ValidarSegundoApellido(segundoApellidoValido);
            Assert.True(resultado, "El segundo apellido debería ser válido.");
        }

        [Fact]
        public void ValidarTelefono_Exitoso()
        {
            string telefonoValido = "2281234567";
            bool resultado = ClienteBibliotecaElSaber.Utilidades.Validador.ValidarTelefono(telefonoValido);
            Assert.True(resultado, "El número telefónico debería ser válido.");
        }

        [Fact]
        public void ValidarDireccion_Exitoso()
        {
            string direccionValida = "Calle 123, Colonia Centro";
            bool resultado = ClienteBibliotecaElSaber.Utilidades.Validador.ValidarDireccion(direccionValida);
            Assert.True(resultado, "La dirección debería ser válida.");
        }

        [Fact]
        public void ValidarCodigoPostal_Exitoso()
        {
            string codigoPostalValido = "91000";
            bool resultado = ClienteBibliotecaElSaber.Utilidades.Validador.ValidarCodigoPostal(codigoPostalValido);
            Assert.True(resultado, "El código postal debería ser válido.");
        }

        [Fact]
        public void ValidarFechas_Exitoso()
        {
            string fechaValida = "2024-03-18";
            bool resultado = ClienteBibliotecaElSaber.Utilidades.Validador.ValidarFechas(fechaValida);
            Assert.True(resultado, "La fecha debería ser válida.");
        }

        [Fact]
        public void ValidarNumeroCasa_Exitoso()
        {
            string numeroCasaValido = "123";
            bool resultado = ClienteBibliotecaElSaber.Utilidades.Validador.ValidarNumeroCasa(numeroCasaValido);
            Assert.True(resultado, "El número de casa debería ser válido.");
        }

        [Fact]
        public void ValidarNumeroDeSocio_Exitoso()
        {
            string numeroSocioValido = "4567";
            bool resultado = ClienteBibliotecaElSaber.Utilidades.Validador.ValidarNumeroDeSocio(numeroSocioValido);
            Assert.True(resultado, "El número de socio debería ser válido.");
        }

        [Fact]
        public void ValidarNombreCiudad_Exitoso()
        {
            string ciudadValida = "Xalapa";
            bool resultado = ClienteBibliotecaElSaber.Utilidades.Validador.ValidarNombreCiudad(ciudadValida);
            Assert.True(resultado, "El nombre de la ciudad debería ser válido.");
        }

        [Fact]
        public void ValidarNombreEditorial_Exitoso()
        {
            string editorialValida = "Editorial Alfa";
            bool resultado = ClienteBibliotecaElSaber.Utilidades.Validador.ValidarNombreEditorial(editorialValida);
            Assert.True(resultado, "El nombre de la editorial debería ser válido.");
        }

        [Fact]
        public void ValidarTitulo_Exitoso()
        {
            string tituloValido = "El Principito";
            bool resultado = ClienteBibliotecaElSaber.Utilidades.Validador.ValidarTitulo(tituloValido);
            Assert.True(resultado, "El título del libro debería ser válido.");
        }

        [Fact]
        public void ValidarISBN_Exitoso()
        {
            string isbnValido = "9781234567890";
            bool resultado = ClienteBibliotecaElSaber.Utilidades.Validador.ValidarISBN(isbnValido);
            Assert.True(resultado, "El ISBN debería ser válido.");
        }

        [Fact]
        public void ValidarSoloNumeros_Exitoso()
        {
            string numerosValidos = "3456";
            bool resultado = ClienteBibliotecaElSaber.Utilidades.Validador.ValidarSoloNumeros(numerosValidos);
            Assert.True(resultado, "El valor solo de números debería ser válido.");
        }






        [Fact]
        public void ValidarNombre_Fallido()
        {
            string nombreInvalido = "Juan123";
            bool resultado = Validador.ValidarNombre(nombreInvalido);
            Assert.False(resultado, "El nombre no debería ser válido.");
        }

        [Fact]
        public void ValidarPrimerApellido_Fallido()
        {
            string apellidoInvalido = "Gómez!@#";
            bool resultado = Validador.ValidarPrimerApellido(apellidoInvalido);
            Assert.False(resultado, "El primer apellido no debería ser válido.");
        }


        [Fact]
        public void ValidarSegundoApellido_Fallido()
        {
            string segundoApellidoInvalido = "López22";
            bool resultado = Validador.ValidarSegundoApellido(segundoApellidoInvalido);
            Assert.False(resultado, "El segundo apellido no debería ser válido");
        }

        [Fact]
        public void ValidarTelefono_Fallido()
        {
            string telefonoInvalido = "2281234abc";
            bool resultado = Validador.ValidarTelefono(telefonoInvalido);
            Assert.False(resultado, "El número telefónico no debería ser válido.");
        }

        [Fact]
        public void ValidarDireccion_Fallida()
        {
            string direccionInvalida = "Calle @!#";
            bool resultado = Validador.ValidarDireccion(direccionInvalida);
            Assert.False(resultado, "La dirección no debería ser válida.");
        }

        [Fact]
        public void ValidarCodigoPostal_Fallido()
        {
            string codigoPostalInvalido = "1234a";
            bool resultado = Validador.ValidarCodigoPostal(codigoPostalInvalido);
            Assert.False(resultado, "El código postal no debería ser válido.");
        }

        [Fact]
        public void ValidarFechas_Fallida()
        {
            string fechaInvalida = "2024-15-40";
            bool resultado = Validador.ValidarFechas(fechaInvalida);
            Assert.False(resultado, "La fecha no debería ser válida.");
        }

        [Fact]
        public void ValidarNumeroCasa_Fallido()
        {
            string numeroCasaInvalido = "12A";
            bool resultado = Validador.ValidarNumeroCasa(numeroCasaInvalido);
            Assert.False(resultado, "El número de casa no debería ser válido.");
        }

        [Fact]
        public void ValidarNumeroDeSocio_Fallido()
        {
            string numeroSocioInvalido = "123456";
            bool resultado = Validador.ValidarNumeroDeSocio(numeroSocioInvalido);
            Assert.False(resultado, "El número de socio no debería ser válido.");
        }

        [Fact]
        public void ValidarNombreCiudad_Fallido()
        {
            string ciudadInvalida = "Xal@pa";
            bool resultado = Validador.ValidarNombreCiudad(ciudadInvalida);
            Assert.False(resultado, "El nombre de la ciudad no debería ser válido.");
        }

        [Fact]
        public void ValidarEditorial_Fallido()
        {
            string editorialInvalido = "@lfa Editor1al";
            bool resultado = Validador.ValidarNombreEditorial(editorialInvalido);
            Assert.False(resultado, "El nombre de la editorial inválida");
        }

        [Fact]
        public void ValidarTitulo_Fallido()
        {
            string tituloInvalido = "El Principito #1";
            bool resultado = Validador.ValidarTitulo(tituloInvalido);
            Assert.False(resultado, "El título del libro no debería ser válido.");
        }

        [Fact]
        public void ValidarISBN_Fallido()
        {
            string isbnInvalido = "97812345678"; 
            bool resultado = Validador.ValidarISBN(isbnInvalido);
            Assert.False(resultado, "El ISBN no debería ser válido.");
        }

        [Fact]
        public void ValidarSoloNumeros_Fallido()
        {
            string numerosInvalidos = "12B34"; 
            bool resultado = Validador.ValidarSoloNumeros(numerosInvalidos);
            Assert.False(resultado, "El valor no debería ser válido.");
        }


    }
}
