using Retinox.Model;
using System;
using System.Windows;
using System.Windows.Input;

namespace Retinox.ViewModel
{
    public class NuevoPacienteViewModel : ViewModelBase
    {
        private readonly AppDatabase _database;

        private string _nombre;
        public string Nombre
        {
            get => _nombre;
            set { _nombre = value; OnPropertyChanged(); }
        }

        private string _nss;
        public string NSS
        {
            get => _nss;
            set { _nss = value; OnPropertyChanged(); }
        }

        private string _edad;
        public string Edad
        {
            get => _edad;
            set { _edad = value; OnPropertyChanged(); }
        }

        public ICommand GuardarCommand { get; }
        
        public Action OnGuardadoExitoso;

        public NuevoPacienteViewModel()
        {
            _database = new AppDatabase();
            GuardarCommand = new RelayCommand(GuardarPaciente);
        }

        private void GuardarPaciente(object obj)
        {
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(NSS) || string.IsNullOrWhiteSpace(Edad))
            {
                MessageBox.Show("Por favor, llene todos los campos.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(Edad, out int edadInt))
            {
                MessageBox.Show("La edad debe ser un número válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var paciente = new Paciente
                {
                    Nombre = Nombre,
                    NSS = NSS,
                    Edad = edadInt,
                    Padecimiento = "Por diagnosticar"
                };

                _database.SavePaciente(paciente);
                MessageBox.Show("Paciente guardado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // Limpiar campos
                Nombre = string.Empty;
                NSS = string.Empty;
                Edad = string.Empty;
                
                OnGuardadoExitoso?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al guardar el paciente: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
