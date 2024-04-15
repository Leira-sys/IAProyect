using System;
using System.Collections.Generic;

namespace ApíRecognizer.Models
{
    public partial class LogAzure
    {
        public int Id { get; set; }
        public string? Tipo { get; set; }
        public string? Descripcion { get; set; }
        public DateTime? FechaHora { get; set; }
        public string? Base64 { get; set; }
        public string? DocumentJson { get; set; }
        public bool? Estado { get; set; }
    }
}
