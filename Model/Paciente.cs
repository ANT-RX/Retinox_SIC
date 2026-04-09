using SQLite;
using System;

namespace Retinox.Model
{
    public class Paciente
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique, MaxLength(50)]
        public string NSS { get; set; }

        [MaxLength(150)]
        public string Nombre { get; set; }

        public int Edad { get; set; }

        [MaxLength(200)]
        public string Padecimiento { get; set; }

        [Ignore]
        public string DescripcionBusqueda 
        { 
            get => $"{NSS} - {Nombre}"; 
            set { /* Dummy setter para evitar InvalidOperationException por TwoWay binding WPF en ComboBox editable */ } 
        }

        public override string ToString()
        {
            return $"{Nombre} - {NSS}";
        }
    }
}
