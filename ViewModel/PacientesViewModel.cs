using Retinox.Model;
using System.Collections.ObjectModel;

namespace Retinox.ViewModel
{
    public class PacientesViewModel : ViewModelBase
    {
        private readonly AppDatabase _database;

        public ObservableCollection<Paciente> Pacientes { get; set; }

        private Paciente _pacienteSeleccionado;
        public Paciente PacienteSeleccionado
        {
            get => _pacienteSeleccionado;
            set { _pacienteSeleccionado = value; OnPropertyChanged(); }
        }

        public PacientesViewModel()
        {
            _database = new AppDatabase();
            Pacientes = new ObservableCollection<Paciente>(_database.GetPacientes());
        }
    }
}
