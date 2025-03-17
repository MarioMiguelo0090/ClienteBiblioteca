using log4net;
using log4net.Config;
using System;

namespace ClienteBibliotecaElSaber.Utilidades
{
    public static class LoggerManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static LoggerManager()
        {
            XmlConfigurator.Configure();
        }

        public static void Informacion(string mensaje)
        {
            Logger.Info(mensaje);
        }

        public static void Depuracion(string mensaje)
        {
            Logger.Debug(mensaje);
        }

        public static void Advertencia(string mensaje)
        {
            Logger.Warn(mensaje);
        }

        public static void Error(string mensaje)
        {
            Logger.Error(mensaje);
        }

        public static void Fatal(string mensaje)
        {
            Logger.Fatal(mensaje);
        }
    }
}
