using Retinox.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Retinox.ViewModel
{
    public class ResultadoViewModel : ViewModelBase
    {
        private readonly OnnxModelEvaluator _evaluator;
        private readonly PdfGeneratorService _pdfService;

        private string _resultadoTexto = "Analizando...";
        public string ResultadoTexto
        {
            get => _resultadoTexto;
            set { _resultadoTexto = value; OnPropertyChanged(); }
        }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public ICommand ExportarPdfCommand { get; }

        public ResultadoViewModel()
        {
            // Usamos la carpeta raíz de ejecución (donde vive el .exe y donde pegaste tus archivos)
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            // Generamos un 'clases.txt' con tus 5 clases por defecto si el archivo llegara a borrarse
            string archivoClases = Path.Combine(baseDir, "clases.txt");
            if (!File.Exists(archivoClases))
            {
                File.WriteAllLines(archivoClases, new[]
                {
                    "Ojo normal",
                    "Retinopatía ligera",
                    "Retinopatía moderada",
                    "Retinopatía severa",
                    "Retinopatía proliferativa"
                });
            }

            // Buscamos el modelo en la raíz
            string modelPath = Path.Combine(baseDir, "modelo_odir.onnx");
            
            _evaluator = new OnnxModelEvaluator(modelPath);
            _pdfService = new PdfGeneratorService();

            ExportarPdfCommand = new RelayCommand(ExportarPdf, CanExportarPdf);

            AnalizarImagen();
        }

        private void AnalizarImagen()
        {
            if (AppSession.CurrentPaciente == null || string.IsNullOrEmpty(AppSession.CurrentRutaImagen))
            {
                ResultadoTexto = "Datos insuficientes para el análisis.";
                return;
            }

            IsBusy = true;
            ResultadoTexto = "Analizando imagen... por favor espere";

            Task.Run(() =>
            {
                try
                {
                    string prediccion = _evaluator.Predict(AppSession.CurrentRutaImagen);
                    
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ResultadoTexto = prediccion;
                        AppSession.CurrentPredictionResult = prediccion;
                        
                        // Actualizar padecimiento en BD si es necesario
                        AppSession.CurrentPaciente.Padecimiento = prediccion;
                        var db = new Model.AppDatabase();
                        db.SavePaciente(AppSession.CurrentPaciente);

                        // Guardar en el historial
                        db.InsertHistorial(new Model.AnalisisHistorial
                        {
                            PacienteId = AppSession.CurrentPaciente.Id,
                            Fecha = DateTime.Now,
                            Diagnostico = prediccion
                        });
                    });
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ResultadoTexto = $"Error: {ex.Message}";
                    });
                }
                finally
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        IsBusy = false;
                        CommandManager.InvalidateRequerySuggested();
                    });
                }
            });
        }

        private bool CanExportarPdf(object obj)
        {
            return !IsBusy && AppSession.CurrentPaciente != null && !string.IsNullOrEmpty(ResultadoTexto) && ResultadoTexto != "Analizando...";
        }

        private void ExportarPdf(object obj)
        {
            string msj = _pdfService.GenerarReporte(AppSession.CurrentPaciente, AppSession.CurrentRutaImagen, ResultadoTexto);
            MessageBox.Show(msj, "Exportar a PDF", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
