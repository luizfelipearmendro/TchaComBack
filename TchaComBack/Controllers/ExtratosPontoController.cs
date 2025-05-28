using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using TchaComBack.Data;
using TchaComBack.Helper;
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

        public IActionResult Index(string mesAno)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var usuario = db.Usuarios.Find(idUsuario);
            if (usuario == null || usuario.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            var matricula = usuario.Matricula;

            var hoje = DateTime.Today;

            if (string.IsNullOrEmpty(mesAno))
            {
                mesAno = hoje.ToString("yyyy-MM");
            }

            var anoMes = mesAno.Split("-");
            int ano = int.Parse(anoMes[0]);
            int mes = int.Parse(anoMes[1]);
            string nomeMes = new DateTime(ano, mes, 1).ToString("MMMM", new CultureInfo("pt-BR"));
            string nomeMesMaiusc = Utilitarios.PrimeiraLetraMaiuscula(nomeMes);

            int mesSelecionado = mes;
            int anoSelecionado = ano;

            var pontos = db.ExtratosPonto
                .Where(p => p.Matricula == matricula &&
                            p.DataBatida.Month == mesSelecionado &&
                            p.DataBatida.Year == anoSelecionado)
                .OrderByDescending(p => p.DataBatida)
                .ToList();

            var cargo = db.Funcionarios
                          .Where(f => f.Matricula == usuario.Matricula)
                          .Select(f => f.Cargo)
                          .FirstOrDefault();


            bool isCargoSimplificado = cargo != null &&
                (cargo.ToLower().Contains("menor aprendiz") ||
                 cargo.ToLower().Contains("estagiário") ||
                 cargo.ToLower().Contains("jovem aprendiz"));

            var viewModel = new EspelhoPontoViewModel
            {
                Mes = mesSelecionado,
                Ano = anoSelecionado,
                Pontos = pontos
            };

            ViewBag.NomeCompleto = usuario.NomeCompleto;
            ViewBag.Email = usuario.Email;
            ViewBag.TipoPerfil = usuario.TipoPerfil;
            ViewBag.MesAnoAtual = mesAno;
            ViewBag.AnoAtual = ano;
            ViewBag.NomeMes = nomeMesMaiusc;
            ViewBag.Cargo = isCargoSimplificado;

            return View(viewModel);
        }

        public IActionResult RegistrarPonto()
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios.Find(idUsuario);
            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            var sessionIdUsuario = dbconsult.Id;

            var horario = TimeOnly.FromDateTime(DateTime.Now);

            var cargo = db.Funcionarios
                          .Where(f => f.Matricula == dbconsult.Matricula)
                          .Select(f => f.Cargo)
                          .FirstOrDefault();


            bool isCargoSimplificado = cargo != null &&
                (cargo.ToLower().Contains("menor aprendiz") ||
                 cargo.ToLower().Contains("estagiário")     ||
                 cargo.ToLower().Contains("jovem aprendiz"  ));

            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
            ViewBag.Email = dbconsult.Email;
            ViewBag.TipoPerfil = dbconsult.TipoPerfil;
            ViewBag.Horario = horario;
            ViewBag.Cargo = isCargoSimplificado;

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

            var funcionario = db.Funcionarios.FirstOrDefault(f => f.Matricula == usuario.Matricula);
            if (funcionario == null)
            {
                TempData["MensagemErro"] = "Funcionário não encontrado.";
                return RedirectToAction("RegistrarPonto");
            }

            string cargo = funcionario.Cargo?.ToLower() ?? "";
            bool isAprendiz = cargo.Contains("aprendiz") || cargo.Contains("estagiário");

            var matricula = funcionario.Matricula;
            var hoje = DateTime.Today;
            var agora = TimeOnly.FromDateTime(DateTime.Now);

            var configuracoes = db.Configuracoes.FirstOrDefault(c => c.Ativo && (isAprendiz
                    ? c.NomeConfiguracao.Contains("Aprendiz") || c.NomeConfiguracao.Contains("Estagiario")
                    : !c.NomeConfiguracao.Contains("Aprendiz") && !c.NomeConfiguracao.Contains("Estagiario")
            ));

            if (isAprendiz && agora > configuracoes.FimExpediente)
            {
                TempData["MensagemErro"] = "Batida não permitida após o fim do expediente para aprendizes e estagiários.";
                return RedirectToAction("RegistrarPonto");
            }

            if (configuracoes == null)
            {
                TempData["MensagemErro"] = "Nenhuma configuração de expediente ativa encontrada.";
                return RedirectToAction("RegistrarPonto");
            }

            DayOfWeek diaHoje = hoje.DayOfWeek;
            if (diaHoje < (DayOfWeek)configuracoes.PrimeiroDiaExpediente || diaHoje > (DayOfWeek)configuracoes.UltimoDiaExpediente)
            {
                TempData["MensagemErro"] = "Hoje não está dentro do expediente definido.";
                return RedirectToAction("RegistrarPonto");
            }

            var ponto = db.ExtratosPonto
                          .FirstOrDefault(p => p.Matricula == matricula && p.DataBatida == hoje);

            if (ponto == null)
            {
                ponto = new ExtratoPontoModel
                {
                    Matricula = matricula,
                    DataBatida = hoje
                };

                TimeOnly limiteEntrada = configuracoes.InicioExpediente.AddMinutes(15);

                if (agora <= limiteEntrada)
                    ponto.HoraEntrada1 = agora;

                else
                    ponto.HoraEntrada2 = agora;

                db.ExtratosPonto.Add(ponto);
            }
            else if (ponto.HoraEntrada1 != null && ponto.HoraSaida2 == null)
            {
                if (isAprendiz)
                {
                    ponto.HoraSaida2 = agora;
                }
                else
                {
                    if (ponto.HoraSaida1 == null)
                        ponto.HoraSaida1 = agora;

                    else if (ponto.HoraEntrada2 == null)
                        ponto.HoraEntrada2 = agora;

                    else if (ponto.HoraSaida2 == null)
                        ponto.HoraSaida2 = agora;
                }
            }
            else if (ponto.HoraEntrada1 == null && ponto.HoraEntrada2 != null && !isAprendiz)
            {
                ponto.HoraSaida2 = agora;
            }

            if (ponto.HoraEntrada1 != null && ponto.HoraSaida2 != null)
            {
                TimeSpan totalTrabalhado = isAprendiz ? ponto.HoraSaida2.Value - ponto.HoraEntrada1.Value : CalcularTotalHoras(ponto);

                TimeSpan cargaEsperada = isAprendiz ? configuracoes.FimExpediente - configuracoes.InicioExpediente
                      : (configuracoes.IntervaloInicio - configuracoes.InicioExpediente)
                      + (configuracoes.FimExpediente - configuracoes.IntervaloFim);

                if (totalTrabalhado > cargaEsperada)
                {
                    ponto.HorasExtras = TimeOnly.FromTimeSpan(totalTrabalhado - cargaEsperada);
                }
                else if (totalTrabalhado < cargaEsperada)
                {
                    ponto.HorasFaltas = TimeOnly.FromTimeSpan(cargaEsperada - totalTrabalhado);
                }

                if (string.IsNullOrEmpty(ponto.Justificativa))
                {
                    if (ponto.HoraEntrada1.HasValue && ponto.HoraEntrada1 > configuracoes.InicioExpediente)
                        ponto.Justificativa = "Atraso na entrada";

                    else if (!isAprendiz && ponto.HoraEntrada2.HasValue && ponto.HoraEntrada2 > configuracoes.IntervaloFim)
                        ponto.Justificativa = "Atraso no retorno do almoço";

                    else if (ponto.HoraSaida2.HasValue && ponto.HoraSaida2 < configuracoes.FimExpediente)
                        ponto.Justificativa = "Saída antecipada";
                }
            }
            else if (ponto.HoraEntrada1 == null && ponto.HoraEntrada2 != null && !isAprendiz)
            {
                TimeSpan totalTrabalhado = isAprendiz ? ponto.HoraSaida2.Value - ponto.HoraEntrada1.Value : CalcularTotalHoras(ponto);

                TimeSpan cargaEsperada = isAprendiz ? configuracoes.FimExpediente - configuracoes.InicioExpediente
                      : (configuracoes.IntervaloInicio - configuracoes.InicioExpediente)
                      + (configuracoes.FimExpediente - configuracoes.IntervaloFim);

                if (totalTrabalhado > cargaEsperada)
                {
                    ponto.HorasExtras = TimeOnly.FromTimeSpan(totalTrabalhado - cargaEsperada);
                }
                else if (totalTrabalhado < cargaEsperada)
                {
                    ponto.HorasFaltas = TimeOnly.FromTimeSpan(cargaEsperada - totalTrabalhado);
                }

                if (string.IsNullOrEmpty(ponto.Justificativa))
                {
                    if (!isAprendiz && ponto.HoraEntrada1 == null && ponto.HoraSaida1 == null && ponto.HoraEntrada2.HasValue)
                        ponto.Justificativa = "Não esteve presente no primeiro período do expediente";

                    else if (ponto.HoraSaida2.HasValue && ponto.HoraSaida2 < configuracoes.FimExpediente)
                        ponto.Justificativa = "Saída antecipada";
                }
            }

                db.SaveChanges();
            TempData["MensagemSucesso"] = "Ponto registrado com sucesso!";
            return RedirectToAction("RegistrarPonto");
        }

        private TimeSpan CalcularTotalHoras(ExtratoPontoModel ponto)
        {
            TimeSpan total = TimeSpan.Zero;

            if (ponto.HoraEntrada1.HasValue && ponto.HoraSaida1.HasValue)
                total += ponto.HoraSaida1.Value - ponto.HoraEntrada1.Value;

            if (ponto.HoraEntrada2.HasValue && ponto.HoraSaida2.HasValue)
                total += ponto.HoraSaida2.Value - ponto.HoraEntrada2.Value;

            return total;
        }

        public IActionResult ExportarEspelhoPontos(int? mes, int? ano)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var usuario = db.Usuarios.Find(idUsuario);
            if (usuario == null || usuario.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            var matricula = usuario.Matricula;

            var hoje = DateTime.Today;
            int mesSelecionado = mes ?? hoje.Month;
            int anoSelecionado = ano ?? hoje.Year;

            var pontos = db.ExtratosPonto
                .Where(p => p.Matricula == matricula &&
                            p.DataBatida.Month == mesSelecionado &&
                            p.DataBatida.Year == anoSelecionado)
                .OrderBy(p => p.DataBatida)
                .ToList();

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Espelho de Ponto");

            // cabeçalhos
            worksheet.Cell(1, 1).Value = "Data";
            worksheet.Cell(1, 2).Value = "Entrada Manhã";
            worksheet.Cell(1, 3).Value = "Saída Manhã";
            worksheet.Cell(1, 4).Value = "Entrada Tarde";
            worksheet.Cell(1, 5).Value = "Saída Tarde";
            worksheet.Cell(1, 6).Value = "Horas Extras";
            worksheet.Cell(1, 7).Value = "Horas Faltas";
            worksheet.Cell(1, 8).Value = "Justificativa";

            int row = 2;
            foreach (var ponto in pontos)
            {
                worksheet.Cell(row, 1).Value = ponto.DataBatida.ToString("dd/MM/yyyy");
                worksheet.Cell(row, 2).Value = ponto.HoraEntrada1?.ToString("HH:mm") ?? "-";
                worksheet.Cell(row, 3).Value = ponto.HoraSaida1?.ToString("HH:mm") ?? "-";
                worksheet.Cell(row, 4).Value = ponto.HoraEntrada2?.ToString("HH:mm") ?? "-";
                worksheet.Cell(row, 5).Value = ponto.HoraSaida2?.ToString("HH:mm") ?? "-";
                worksheet.Cell(row, 6).Value = ponto.HorasExtras?.ToString("HH:mm") ?? "-";
                worksheet.Cell(row, 7).Value = ponto.HorasFaltas?.ToString("HH:mm") ?? "-";
                worksheet.Cell(row, 8).Value = ponto.Justificativa ?? "-";
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            string nomeArquivo = $"EspelhoPonto_{mesSelecionado:D2}_{anoSelecionado}.xlsx";

            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nomeArquivo);
        }
    }
}