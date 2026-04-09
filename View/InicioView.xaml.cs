using Microsoft.Win32;
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
    /// Lógica de interacción para InicioView.xaml
    /// </summary>
    public partial class InicioView : Window
    {
        public InicioView()
        {
            InitializeComponent();
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

        private void btnAnalisis_Click(object sender, RoutedEventArgs e)
        {
            var analisisView = new AnalisisView();
            analisisView.Show();
            Close();
        }

        private void btnPacientes_Click(object sender, RoutedEventArgs e)
        {
            var pacientesView = new PacientesView();
            pacientesView.Show();
            Close();
        }

        private void btnHistorial_Click(object sender, RoutedEventArgs e)
        {
            var historialView = new HistorialView();
            historialView.Show();
            Close();
        }
    }
}
