using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Retinox.View
{
    /// <summary>
    /// Lógica de interacción para HistorialView.xaml
    /// </summary>
    public partial class HistorialView : Window
    {
        public HistorialView()
        {
            InitializeComponent();
            DataContext = new ViewModel.HistorialViewModel();
        }
        private void btnMinimizar_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnAyuda_Click(object sender, RoutedEventArgs e)
        {
            var infoView = new InfoView();
            infoView.Show();
        }

        private void btnAtras_Click(object sender, RoutedEventArgs e)
        {
            var inicioView = new InicioView();
            inicioView.Show();
            Close();
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("¿Estás seguro de que deseas borrar todas las pruebas de la base de datos?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var db = new Model.AppDatabase();
                db.LimpiarHistorial();
                DataContext = new ViewModel.HistorialViewModel(); // Refrescar la vista
                MessageBox.Show("El historial ha sido borrado exitosamente.", "Limpieza completada", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
