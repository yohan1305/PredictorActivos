using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Dtos
{
    public class RocDetalleDto //Para el Roc
    {
        public int Indice { get; set; }
        public decimal Precio { get; set; }
        public string? RocTexto { get; set; }
    }
}
