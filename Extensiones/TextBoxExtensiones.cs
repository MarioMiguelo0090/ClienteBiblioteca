﻿using System.Windows;

namespace ClienteBibliotecaElSaber.Extensiones
{
    public static class TextBoxExtensiones
    {
        public static readonly DependencyProperty PropiedadTextoSugerido =
            DependencyProperty.RegisterAttached(
                "TextoSugerido",
                typeof(string),
                typeof(TextBoxExtensiones),
                new FrameworkPropertyMetadata(string.Empty)
            );
        public static string GetTextoSugerido(DependencyObject obj)
        {
            return (string)obj.GetValue(PropiedadTextoSugerido);
        }

        public static void SetTextoSugerido(DependencyObject obj, string valor)
        {
            obj.SetValue(PropiedadTextoSugerido, valor);
        }
    }
}
