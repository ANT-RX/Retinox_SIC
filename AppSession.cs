using Retinox.Model;

namespace Retinox
{
    public static class AppSession
    {
        public static Paciente CurrentPaciente { get; set; }
        public static string CurrentRutaImagen { get; set; }
        public static string CurrentPredictionResult { get; set; }
    }
}
