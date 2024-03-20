
using ServidorPostIt.Models;
using ServidorPostIt.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorPostIt.ViewModels
{
    public class NotaViewModel : INotifyPropertyChanged
    {
        NotaServer server = new();

        public ObservableCollection<Nota> Notas { get; set; } = new();
        public NotaViewModel()
        {
            server.NotaRecibida += Server_NotaRecibida;
            server.Iniciar();
        }
        Random random = new();
        private void Server_NotaRecibida(object? sender, Nota e)
        {
            random.Next(-5, 6);
            Notas.Add(e);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
