using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClienteBibliotecaElSaber.Utilidades
{
    public class ManejadorDeCitasAutores
    {
        public readonly Dictionary<string, string> citasLibros;
        private readonly Random random;

        public ManejadorDeCitasAutores()
        {
            citasLibros = new Dictionary<string, string>
            {
                { 
                    "Gabriel García Márquez - Cien años de soledad", 
                    "\"El secreto de una buena vejez no es otra cosa que un pacto honrado con la soledad. " +
                    "Se llega a la vejez como a todo lo demás, sin aviso. " +
                    "Un día, de repente, te das cuenta de que tienes más pasado que futuro, " +
                    "y es entonces cuando el tiempo, que antes parecía inagotable, empieza a apretarse " +
                    "como un lazo invisible alrededor de los días que quedan.\"" 
                },
                {
                    "Fiódor Dostoyevski - Los hermanos Karamázov",
                    "\"Cada uno de nosotros es culpable de todo y por todos en la tierra, en lo más alto " +
                    "y en lo más profundo, más aún que el otro. Si los hombres lo entendieran, el paraíso " +
                    "llegaría en un instante.\""
                },
                {
                    "Haruki Murakami - Kafka en la orilla",
                    "\"Cuando sales de la tormenta, no eres la misma persona que entró en ella. " +
                    "De eso se trata la tormenta. No hay garantía de que saldrás ileso, pero sí " +
                    "la certeza de que no serás el mismo después de atravesarla.\""
                },
                {
                    "Oscar Wilde - El retrato de Dorian Gray",
                    "\"Vivimos en una época en la que las cosas innecesarias son nuestras únicas necesidades. " +
                    "La sociedad moderna olvida que el arte no es una simple imitación de la vida, " +
                    "sino la revelación de su esencia más profunda. Y si el hombre se aleja de la belleza, " +
                    "se convierte en un esclavo de sus propios deseos efímeros.\""
                },
                {
                    "Julio Cortázar - Rayuela",
                    "\"Andábamos sin buscarnos pero sabiendo que andábamos para encontrarnos. " +
                    "A veces me pregunto si no nos pasamos la vida entera persiguiendo señales que creemos " +
                    "reconocer, repitiendo gestos que no entendemos, sintiendo cosas que ya sentimos antes " +
                    "y que sin embargo nos parecen nuevas.\""
                }
            };

            random = new Random();
        }

        public (string autor, string cita) ObtenerCitaAleatoria()
        {
            int index = random.Next(citasLibros.Count);
            string autor = new List<string>(citasLibros.Keys)[index];
            string cita = citasLibros[autor];
            return (autor, cita);
        }
    }
}
