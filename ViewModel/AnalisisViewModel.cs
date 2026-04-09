using Retinox.Model;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace Retinox.ViewModel
{
    public class AnalisisViewModel : ViewModelBase
    {
        private readonly AppDatabase _database;

        public ObservableCollection<Paciente> Pacientes { get; set; }

        private Paciente _pacienteSeleccionado;
        public Paciente PacienteSeleccionado
        {
            get => _pacienteSeleccionado;
            set { _pacienteSeleccionado = value; OnPropertyChanged(); }
        }

        private string _nombreArchivoImagen = "Ninguna imagen seleccionada";
        public string NombreArchivoImagen
        {
            get => _nombreArchivoImagen;
            set { _nombreArchivoImagen = value; OnPropertyChanged(); }
        }

        private string _rutaArchivoImagenCompleta;
        public string RutaArchivoImagenCompleta
        {
            get => _rutaArchivoImagenCompleta;
            set { _rutaArchivoImagenCompleta = value; OnPropertyChanged(); }
        }

        public ICommand CargarImagenCommand { get; }
        public ICommand IrAResultadoCommand { get; }

        public Action OnIrAResultados;

        public AnalisisViewModel()
        {
            _database = new AppDatabase();
            Pacientes = new ObservableCollection<Paciente>(_database.GetPacientes());

            CargarImagenCommand = new RelayCommand(CargarImagen);
            IrAResultadoCommand = new RelayCommand(IrAResultado);
        }

        private void CargarImagen(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos de Imagen|*.jpg;*.jpeg;*.png;",
                Title = "Seleccionar imagen de retina"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                RutaArchivoImagenCompleta = openFileDialog.FileName;
                NombreArchivoImagen = Path.GetFileName(RutaArchivoImagenCompleta);
            }
        }

        private void IrAResultado(object obj)
        {
            if (PacienteSeleccionado == null)
            {
                MessageBox.Show("Por favor, seleccione un paciente de la lista.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(RutaArchivoImagenCompleta))
            {
                MessageBox.Show("Por favor, cargue una imagen de retina.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Aquí se pasa la info estática temporalmente o se guarda en Singleton. Para simplificar, 
            // guardaremos el paciente y la ruta de imagen seleccionada en variables globales / singleton
            AppSession.CurrentPaciente = PacienteSeleccionado;
            AppSession.CurrentRutaImagen = RutaArchivoImagenCompleta;

            Application.Current.Dispatcher.Invoke(() =>
            {
                OnIrAResultados?.Invoke();
            });
        }
    }
}
