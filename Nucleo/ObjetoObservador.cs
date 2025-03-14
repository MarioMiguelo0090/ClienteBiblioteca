using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClienteBibliotecaElSaber.Nucleo
{
    class ObjetoObservador : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string nombre = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }
    }
}
