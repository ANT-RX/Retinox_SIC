using Retinox.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Retinox
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.DispatcherUnhandledException += (s, ev) =>
            {
                MessageBox.Show($"FATAL ERROR: {ev.Exception.Message}\n\nStack:\n{ev.Exception.StackTrace}\n\nInner:\n{ev.Exception.InnerException?.Message}", "Crash Dumper", MessageBoxButton.OK, MessageBoxImage.Error);
                ev.Handled = true;
            };

            var inicioView = new InicioView();
            inicioView.Show();
        }
    }
}
