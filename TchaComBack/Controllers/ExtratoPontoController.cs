using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using TchaComBack.Data;
using TchaComBack.Models;

namespace TchaComBack.Controllers
{
    public class ExtratoPontoController : Controller
    {
        private readonly ApplicationDbContext db;
        public ExtratoPontoController(ApplicationDbContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RegistrarPonto()
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

            if (dbconsult == null) return RedirectToAction("Index", "Login");

            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
            ViewBag.Email = dbconsult.Email;
            ViewBag.TipoPerfil = dbconsult.TipoPerfil;
            ViewBag.Senha = dbconsult.Senha;

            return View();
        }


        [HttpPost]
        public IActionResult RegistrarBatida()
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            try
            {
                var usuario = db.Usuarios.FirstOrDefault(u => u.Id == idUsuario);
                if (usuario == null || usuario.Matricula == null)
                {
                    TempData["Erro"] = "Usuário não encontrado ou matrícula não definida.";
                    return RedirectToAction("RegistrarPonto");
                }

                var funcionario = db.Funcionarios.FirstOrDefault(f => f.Matricula == usuario.Matricula);
                if (funcionario == null)
                {
                    TempData["Erro"] = "Funcionário não encontrado.";
                    return RedirectToAction("RegistrarPonto");
                }

                var hoje = DateTime.Today;
                var agora = DateTime.Now.TimeOfDay;
                int minutosAgora = (int)agora.TotalMinutes;

                var registro = db.ExtratoPonto
                    .FirstOrDefault(e => e.Matricula == funcionario.Matricula && e.DataBatida == hoje);

                if (registro != null)
                {
                    bool batidasCompletas = registro.HoraEntrada1.HasValue &&
                                            registro.HoraSaida1.HasValue &&
                                            registro.HoraEntrada2.HasValue &&
                                            registro.HoraSaida2.HasValue;

                    if (batidasCompletas)
                    {
                        // Já registrou as 4 batidas hoje, bloquear nova batida
                        TempData["Erro"] = "Todas as batidas de hoje já foram registradas. Aguarde até a meia-noite para registrar novas batidas.";
                        return RedirectToAction("RegistrarPonto");
                    }
                }
                TimeSpan metaHorasDiarias;
                TimeSpan horasACumprir;

                switch (funcionario.TipoContrato)
                {
                    case "1": // CLT
                        metaHorasDiarias = TimeSpan.FromHours(8);
                        horasACumprir = TimeSpan.FromHours(220);
                        break;

                    case "2": // Aprendiz / Estagiário
                        metaHorasDiarias = TimeSpan.FromHours(6);
                        horasACumprir = TimeSpan.FromHours(150);
                        break;

                    default:
                        TempData["Erro"] = $"Tipo de contrato não reconhecido: {funcionario.TipoContrato}";
                        return RedirectToAction("RegistrarPonto");
                }

                if (registro == null)
                {
                    registro = new ExtratoPontoModel
                    {
                        Matricula = funcionario.Matricula.Value,
                        DataBatida = hoje,
                        HoraEntrada1 = minutosAgora,
                        HorasACumprir = (int)horasACumprir.TotalMinutes
                    };

                    db.ExtratoPonto.Add(registro);
                    db.SaveChanges();
                    TempData["Mensagem"] = "Entrada registrada com sucesso!";
                }
                else
                {
                    if (!registro.HoraEntrada1.HasValue)
                    {
                        registro.HoraEntrada1 = minutosAgora;
                        TempData["Mensagem"] = "Entrada 1 registrada com sucesso!";
                    }
                    else if (!registro.HoraSaida1.HasValue)
                    {
                        registro.HoraSaida1 = minutosAgora;
                        TempData["Mensagem"] = "Saída 1 registrada com sucesso!";
                    }
                    else if (!registro.HoraEntrada2.HasValue)
                    {
                        registro.HoraEntrada2 = minutosAgora;
                        TempData["Mensagem"] = "Entrada 2 registrada com sucesso!";
                    }
                    else if (!registro.HoraSaida2.HasValue)
                    {
                        registro.HoraSaida2 = minutosAgora;
                        TempData["Mensagem"] = "Saída 2 registrada com sucesso!";
                    }
                    else
                    {
                        TempData["Erro"] = "Todas as batidas de hoje já foram registradas.";
                        return RedirectToAction("RegistrarPonto");
                    }

                    registro.HorasACumprir = (int)horasACumprir.TotalMinutes;
                }

                // Calcular horas trabalhadas convertendo os minutos para TimeSpan
                TimeSpan totalTrabalhado = TimeSpan.Zero;

                if (registro.HoraEntrada1.HasValue && registro.HoraSaida1.HasValue)
                {
                    totalTrabalhado += TimeSpan.FromMinutes(registro.HoraSaida1.Value - registro.HoraEntrada1.Value);
                }
                if (registro.HoraEntrada2.HasValue && registro.HoraSaida2.HasValue)
                {
                    totalTrabalhado += TimeSpan.FromMinutes(registro.HoraSaida2.Value - registro.HoraEntrada2.Value);
                }

                registro.HorasTrabalhadas = (int)totalTrabalhado.TotalMinutes;

                // Cálculo de horas positivas, negativas e extras com base na meta diária
                if (totalTrabalhado > metaHorasDiarias)
                {
                    var positivas = totalTrabalhado - metaHorasDiarias;
                    registro.HorasPositivas = (int)positivas.TotalMinutes;
                    registro.HorasNegativas = 0;
                    registro.HorasExtras = registro.HorasPositivas;
                }
                else if (totalTrabalhado < metaHorasDiarias)
                {
                    var negativas = metaHorasDiarias - totalTrabalhado;
                    registro.HorasNegativas = (int)negativas.TotalMinutes;
                    registro.HorasPositivas = 0;
                    registro.HorasExtras = 0;
                }
                else
                {
                    registro.HorasPositivas = 0;
                    registro.HorasNegativas = 0;
                    registro.HorasExtras = 0;
                }

                db.ExtratoPonto.Update(registro);
                db.SaveChanges();

                return RedirectToAction("RegistrarPonto");
            }
            catch (Exception ex)
            {
                TempData["Erro"] = $"Erro ao registrar ponto: {ex.Message}";
                return RedirectToAction("RegistrarPonto");
            }
        }
     
    }

}
