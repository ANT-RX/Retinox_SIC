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
    /// Lógica de interacción para PacientesView.xaml
    /// </summary>
    public partial class PacientesView : Window
    {
        public PacientesView()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.PacientesViewModel();
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
    }
}
