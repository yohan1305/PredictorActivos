using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Dtos
{
    public class PrediccionRequestDto
    {
        public List<DateTime> Fechas { get; set; } = new();
        public List<decimal> Valores { get; set; } = new();

    }
}
