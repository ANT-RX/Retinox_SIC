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
    /// Lógica de interacción para AnalisisView.xaml
    /// </summary>
    public partial class AnalisisView : Window
    {
        public AnalisisView()
        {
            InitializeComponent();
            var vm = new ViewModel.AnalisisViewModel();
            vm.OnIrAResultados += () =>
            {
                var resultadoView = new ResultadoView();
                resultadoView.Show();
                this.Close();
            };
            this.DataContext = vm;
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

        private void btnAdelante_Click(object sender, RoutedEventArgs e)
        {
            // La lógica ahora la hace el comando vinculado al botón, pero podemos dejar el método vacío
            // o eliminar la asignación de evento si ya la quitamos en XAML.
        }

        private void btnCargarImagen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Imágenes (*.jpg;*.png)|*.jpg;*.png";

            if (dlg.ShowDialog() == true)
            {
                // Actualizamos el nombre en la celda de texto
                txtNombreArchivo.Text = System.IO.Path.GetFileName(dlg.FileName);
            }
        }

        private void btnNuevoPaciente_Click(object sender, RoutedEventArgs e)
        {
            var nuevoPacienteView = new NuevoPacienteView();
            nuevoPacienteView.Show();
            Close();
        }
    }
}
