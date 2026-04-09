using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace Retinox.Model
{
    public class AppDatabase
    {
        private SQLiteConnection _database;

        public AppDatabase()
        {
            SQLitePCL.Batteries_V2.Init();

            // Ruta a la base de datos local en la carpeta AppData del usuario
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Retinox.db3");
            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<Paciente>();
            _database.CreateTable<AnalisisHistorial>();
        }

        public List<Paciente> GetPacientes()
        {
            return _database.Table<Paciente>().ToList();
        }

        public Paciente GetPacienteByNSS(string nss)
        {
            return _database.Table<Paciente>().FirstOrDefault(p => p.NSS == nss);
        }

        public int SavePaciente(Paciente paciente)
        {
            if (paciente.Id != 0)
            {
                return _database.Update(paciente);
            }
            else
            {
                return _database.Insert(paciente);
            }
        }

        public int DeletePaciente(Paciente paciente)
        {
            return _database.Delete(paciente);
        }

        public List<AnalisisHistorial> GetHistorial()
        {
            return _database.Table<AnalisisHistorial>().OrderByDescending(h => h.Fecha).ToList();
        }

        public List<AnalisisHistorial> GetHistorialByPacienteId(int pacienteId)
        {
            return _database.Table<AnalisisHistorial>().Where(h => h.PacienteId == pacienteId).OrderByDescending(h => h.Fecha).ToList();
        }

        public int InsertHistorial(AnalisisHistorial historial)
        {
            return _database.Insert(historial);
        }

        public void LimpiarHistorial()
        {
            _database.DropTable<AnalisisHistorial>();
            _database.CreateTable<AnalisisHistorial>();
        }
    }
}
