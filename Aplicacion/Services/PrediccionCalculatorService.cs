using Aplicacion.Dtos;
using Aplicacion.Enums;
using Aplicacion.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Services
{
    public class PrediccionCalculatorService
    {
        private readonly PrediccionRepository _repositorio;

        public PrediccionCalculatorService()
        {
            _repositorio = new PrediccionRepository();
        }

        public PrediccionResponseDto Calcular(PrediccionRequestDto dto)
        {
            _repositorio.GuardarEntrada(dto);

            var modo = PredictionService.Instance.ObtenerModo();

            return modo switch
            {
                ModoPrediccion.SMA => CalcularSMA(dto),
                ModoPrediccion.Regresion => CalcularRegresion(dto),
                ModoPrediccion.ROC => CalcularROC(dto),
                _ => throw new InvalidOperationException("Modo de predicción no reconocido.")
            };
        }

        private PrediccionResponseDto CalcularSMA(PrediccionRequestDto dto)
        {
            var valores = dto.Valores;

            if (valores.Count < 20)
            {
                return new PrediccionResponseDto
                {
                    ResultadoTexto = "Se requieren 20 valores para calcular SMA.",
                    ModoUsado = "SMA"
                };
            }

            var smaCorta = valores.TakeLast(5).Average();
            var smaLarga = valores.TakeLast(20).Average();

          
            var diferencia = Math.Abs(smaCorta - smaLarga);
            string tendencia;

            if (diferencia < 0.01m)
            {
                tendencia = "neutra";
            }
            else
            {
                if (smaCorta > smaLarga)
                {
                    tendencia = "alcista";
                }
                else
                {
                    tendencia = "bajista";
                }
            }

            var resultado = new PrediccionResponseDto
            {
                ValorCalculado = Math.Round(smaCorta, 2),
                ValorSecundario = Math.Round(smaLarga, 2),
                ResultadoTexto = $"SMA corta: {smaCorta:F2}, SMA larga: {smaLarga:F2}. Tendencia: {tendencia}.",
                ModoUsado = "SMA"
            };

            _repositorio.GuardarResultado(resultado);
            return resultado;
        }
        private PrediccionResponseDto CalcularRegresion(PrediccionRequestDto dto)
        {
            var valores = dto.Valores;

         
            if (valores.Count < 2)
            {
                return new PrediccionResponseDto
                {
                    ResultadoTexto = "Se requieren al menos 2 valores para calcular regresión.",
                    ModoUsado = "Regresión"
                };
            }

            int cantidad = valores.Count;

            
            List<double> dias = Enumerable.Range(1, cantidad).Select(d => (double)d).ToList();
            List<double> precios = valores.Select(v => (double)v).ToList();

           
            double sumaX = dias.Sum();
            double sumaY = precios.Sum();
            double sumaXY = dias.Zip(precios, (x, y) => x * y).Sum();
            double sumaX2 = dias.Select(x => x * x).Sum();

          
            double numerador = cantidad * sumaXY - sumaX * sumaY;
            double denominador = cantidad * sumaX2 - sumaX * sumaX;

            double pendiente = denominador != 0 ? numerador / denominador : 0;
            double intercepto = (sumaY - pendiente * sumaX) / cantidad;

            
            double valorEstimado = pendiente * 21 + intercepto;

           
            string tendencia;

            if (Math.Abs(pendiente) < 0.0001)
            {
                tendencia = "neutra";
            }
            else if (pendiente > 0)
            {
                tendencia = "alcista";
            }
            else
            {
                tendencia = "bajista";
            }

           
            var resultado = new PrediccionResponseDto
            {
                ValorCalculado = Math.Round((decimal)valorEstimado, 2),
                ValorSecundario = Math.Round((decimal)pendiente, 4),
                ResultadoTexto = $"Valor estimado en día 21: {valorEstimado:F2}. Tendencia: {tendencia}. Pendiente: {pendiente:F4}",
                ModoUsado = "Regresión"
            };

            _repositorio.GuardarResultado(resultado);
            return resultado;
        }

        private PrediccionResponseDto CalcularROC(PrediccionRequestDto dto)
        {
            var valores = dto.Valores;
            int n = 5;

            if (valores.Count < n)
            {
                return new PrediccionResponseDto
                {
                    ResultadoTexto = "Se requieren al menos 5 valores para calcular ROC.",
                    ModoUsado = "ROC"
                };
            }

            var detalles = new List<RocDetalleDto>();
            var rocsValidos = new List<decimal>();

            for (int t = 0; t < valores.Count; t++)
            {
                decimal precio = valores[t];
                string rocTexto;

                if (t < n)
                {
                    rocTexto = "n/a";
                }
                else
                {
                    decimal precioAnterior = valores[t - n];
                    decimal roc = ((precio / precioAnterior) - 1) * 100;
                    roc = Math.Round(roc, 2);
                    rocTexto = $"{roc:F2}%";
                    rocsValidos.Add(roc);
                }

                detalles.Add(new RocDetalleDto
                {
                    Indice = t,
                    Precio = precio,
                    RocTexto = rocTexto
                });
            }


            string tendenciaTexto;

            if (rocsValidos.Any())
            {
                decimal promedioROC = rocsValidos.Average();

                switch (promedioROC)
                {
                    case > 0.5m:
                        tendenciaTexto = "Tendencia: alcista.";
                        break;

                    case < -0.5m:
                        tendenciaTexto = "Tendencia: bajista.";
                        break;

                    default:
                        tendenciaTexto = "Tendencia: neutra.";
                        break;
                }
            }
            else
            {
                tendenciaTexto = "No se pudo determinar la tendencia.";
            }


            var resultado = new PrediccionResponseDto
            {
                DetallesROC = detalles,
                ResultadoTexto = $"Se calcularon {rocsValidos.Count} valores ROC. {tendenciaTexto}",
                ModoUsado = "ROC"
            };

            _repositorio.GuardarResultado(resultado);
            return resultado;
        }



    }

}

