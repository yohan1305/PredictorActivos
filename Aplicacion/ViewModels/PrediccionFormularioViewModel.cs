using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.ViewModels
{
    public class PrediccionFormularioViewModel
    {
        [Required]
        [MinLength(20, ErrorMessage = "Debes ingresar 20 pares de datos.")]
        public List<ParDatoViewModel> Datos { get; set; } = new();


    }
}
