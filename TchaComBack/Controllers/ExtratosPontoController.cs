using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TchaComBack.Data;
using TchaComBack.Models;

namespace TchaComBack.Controllers
{
    public class ExtratosPontoController : Controller
    {
        private readonly ILogger<ExtratosPontoController> _logger;
        private readonly ApplicationDbContext db;
        private readonly IMemoryCache _cache;

        public ExtratosPontoController(ILogger<ExtratosPontoController> logger, IMemoryCache cache, ApplicationDbContext db)
        {
            _logger = logger;
            this.db = db;
            _cache = cache;
        }

        public int sessionIdUsuario
        {
            get
            {
                int sessionIdUsuario = 0;
                if (HttpContext.Session.GetInt32("Id") != null)
                    sessionIdUsuario = (int)HttpContext.Session.GetInt32("Id");
                return sessionIdUsuario;
            }
        }

        public IActionResult Index()
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios.Find(idUsuario);
            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            var sessionIdUsuario = dbconsult.Id;

            return View();
        }

        public IActionResult RegistrarPonto()
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios.Find(idUsuario);
            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            var sessionIdUsuario = dbconsult.Id;

            return View();
        }

        [HttpPost]
        public IActionResult RegistrarPonto(ExtratoPontoModel extratoPonto)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var usuario = db.Usuarios.Find(idUsuario);
            if (usuario == null || usuario.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            var matricula = usuario.Matricula; // Assumindo que o campo Matricula vem da model Usuario
            var hoje = DateTime.Today;

            var ponto = db.ExtratosPonto
                          .FirstOrDefault(p => p.Matricula == matricula && p.DataBatida == hoje);

            var agora = TimeOnly.FromDateTime(DateTime.Now);

            if (ponto == null)
            {
                // Primeira batida do dia
                ponto = new ExtratoPontoModel
                {
                    Matricula = matricula,
                    DataBatida = hoje,
                    HoraEntrada1 = agora
                };
                db.ExtratosPonto.Add(ponto);
            }
            else if (ponto.HoraEntrada1 != null && ponto.HoraSaida1 == null)
            {
                ponto.HoraSaida1 = agora;
            }
            else if (ponto.HoraEntrada2 == null)
            {
                ponto.HoraEntrada2 = agora;
            }
            else if (ponto.HoraSaida2 == null)
            {
                ponto.HoraSaida2 = agora;

                // Exemplo: cálculo simples de horas extras
                var cargaHoraria = new TimeSpan(8, 0, 0); // 8 horas
                var totalHoras = CalcularTotalHoras(ponto);
                if (totalHoras > cargaHoraria)
                {
                    ponto.HorasExtras = TimeOnly.FromTimeSpan(totalHoras - cargaHoraria);
                }
                else if (totalHoras < cargaHoraria)
                {
                    ponto.HorasFaltas = TimeOnly.FromTimeSpan(cargaHoraria - totalHoras);
                }
            }

            db.SaveChanges();

            TempData["Mensagem"] = "Ponto registrado com sucesso!";
            return RedirectToAction("RegistrarPonto");
        }

        private TimeSpan CalcularTotalHoras(ExtratoPontoModel ponto)
        {
            TimeSpan total = TimeSpan.Zero;

            if (ponto.HoraEntrada1.HasValue && ponto.HoraSaida1.HasValue)
                total += ponto.HoraSaida1.Value.ToTimeSpan() - ponto.HoraEntrada1.Value.ToTimeSpan();

            if (ponto.HoraEntrada2.HasValue && ponto.HoraSaida2.HasValue)
                total += ponto.HoraSaida2.Value.ToTimeSpan() - ponto.HoraEntrada2.Value.ToTimeSpan();

            return total;
        }
    }
}