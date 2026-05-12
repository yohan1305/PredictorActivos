using Aplicacion.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Services
{
    public class PredictionService
    {
        private static readonly PredictionService _instance = new PredictionService();
        public static PredictionService Instance => _instance;

        private ModoPrediccion _modoActual = ModoPrediccion.SMA;

        private PredictionService() { }

        public ModoPrediccion ObtenerModo()
        {
            return _modoActual;
        }

        public void CambiarModo(ModoPrediccion nuevoModo)
        {
            _modoActual = nuevoModo;
        }
    }

}
