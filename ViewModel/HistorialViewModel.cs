using Retinox.Model;
using System.Collections.ObjectModel;
using System.Linq;

namespace Retinox.ViewModel
{
    public class HistorialViewModel : ViewModelBase
    {
        private readonly AppDatabase _database;

        public ObservableCollection<string> RegistrosHistorial { get; set; }

        public HistorialViewModel()
        {
            _database = new AppDatabase();
            CargarHistorial();
        }

        private void CargarHistorial()
        {
            var historiales = _database.GetHistorial();
            var pacientes = _database.GetPacientes().ToDictionary(p => p.Id, p => p.Nombre);

            var lista = new ObservableCollection<string>();
            foreach (var h in historiales)
            {
                string nombrePaciente = pacientes.ContainsKey(h.PacienteId) ? pacientes[h.PacienteId] : "Paciente Desconocido";
                lista.Add($"{nombrePaciente} --- {h.Diagnostico} --- {h.Fecha:dd/MM/yyyy HH:mm}");
            }
            RegistrosHistorial = lista;
        }
    }
}
