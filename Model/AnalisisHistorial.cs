using SQLite;
using System;

namespace Retinox.Model
{
    public class AnalisisHistorial
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int PacienteId { get; set; }

        public DateTime Fecha { get; set; }

        [MaxLength(200)]
        public string Diagnostico { get; set; }
    }
}
