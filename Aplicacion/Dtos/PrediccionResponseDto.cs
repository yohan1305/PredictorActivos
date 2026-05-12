using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Dtos
{
    public class PrediccionResponseDto
    {
        public string ResultadoTexto { get; set; } = string.Empty;
        public decimal? ValorCalculado { get; set; } // Para regresión o SMA
        public decimal? ValorSecundario { get; set; } // SMA larga o pendiente
        public List<RocDetalleDto>? DetallesROC { get; set; } // Para modo ROC

        public string ModoUsado { get; set; } = string.Empty;

    }
}
