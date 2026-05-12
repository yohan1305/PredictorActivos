using Aplicacion.Dtos;
using Aplicacion.Services;
using Aplicacion.ViewModels;
using Aplicacion.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Predictor_Activos.Controllers
{
    public class HomeController : Controller
    {
        private readonly PrediccionCalculatorService _servicio;

        private static PrediccionFormularioViewModel _ultimoFormulario = new();
        private static PrediccionResponseDto? _ultimoResultado;

        public HomeController()
        {
            _servicio = new PrediccionCalculatorService();
        }

       
        [HttpGet]
        public IActionResult Index()
        {
            
            if (_ultimoFormulario.Datos.Count == 0)
            {
                _ultimoFormulario = new PrediccionFormularioViewModel
                {
                    Datos = Enumerable.Range(0, 20)
                        .Select(_ => new ParDatoViewModel
                        {
                            Fecha = DateTime.MinValue,
                            Valor = 0m
                        }).ToList()
                };
            }

           
            ViewBag.ModoUsado = PredictionService.Instance.ObtenerModo().ToString();
            ViewBag.Resultado = _ultimoResultado;
            ViewBag.Error = TempData["Error"];
            ViewBag.Exito = TempData["Exito"];

            return View(_ultimoFormulario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CalcularPrediccion(PrediccionFormularioViewModel modelo)
        {
            // Forzar interpretación de números con punto decimal
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

            if (modelo.Datos.Any(d => d.Valor is null || d.Valor <= 0))
            {
                ModelState.AddModelError("", "Todos los valores deben ser mayores a cero.");
                return View("Index", modelo);
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Hay errores en el formulario. Verifica los campos marcados.";
                return View("Index", modelo);
            }

            var request = new PrediccionRequestDto
            {
                Fechas = modelo.Datos.Select(d => d.Fecha!.Value).ToList(),
                Valores = modelo.Datos.Select(d => d.Valor!.Value).ToList()
            };

            var resultado = _servicio.Calcular(request);

            _ultimoFormulario = modelo;
            _ultimoResultado = resultado;

            TempData["Exito"] = "Predicción calculada con éxito.";
            return RedirectToAction("Index");
        }

        // Mostrar vista de modos disponibles
        [HttpGet]
        public IActionResult Modos()
        {
            ViewBag.ModoActual = PredictionService.Instance.ObtenerModo();
            return View();
        }

        // Cambiar modo de predicción
        [HttpPost]
        public IActionResult CambiarModo(string modo)
        {
            if (Enum.TryParse<ModoPrediccion>(modo, out var nuevoModo))
            {
                PredictionService.Instance.CambiarModo(nuevoModo);
                TempData["Mensaje"] = $"Modo cambiado a {nuevoModo}.";
            }
            else
            {
                TempData["Mensaje"] = "Modo inválido.";
            }

            return RedirectToAction("Modos");
        }
    }
}