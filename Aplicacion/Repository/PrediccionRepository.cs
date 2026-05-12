using Aplicacion.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Repository
{
    public class PrediccionRepository
    {
        private static readonly List<PrediccionRequestDto> _historialEntradas = new();
        private static readonly List<PrediccionResponseDto> _historialResultados = new();

        public void GuardarEntrada(PrediccionRequestDto dto)
        {
            _historialEntradas.Add(dto);
        }

        public void GuardarResultado(PrediccionResponseDto dto)
        {
            _historialResultados.Add(dto);
        }

        public List<PrediccionRequestDto> ObtenerEntradas()
        {
            return _historialEntradas;
        }

        public List<PrediccionResponseDto> ObtenerResultados()
        {
            return _historialResultados;
        }

    }
}
