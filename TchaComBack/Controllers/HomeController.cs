using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using TchaComBack.Data;
using TchaComBack.Models;
using Microsoft.Extensions.Caching.Memory;

namespace TchaComBack.Controllers;

public class HomeController : Controller
{
    readonly private ApplicationDbContext _db;
    private readonly IMemoryCache _cache;
    public HomeController(ApplicationDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public int sessionidusuario
    {
        get
        {
            int sessionidusuario = 0;
            if (HttpContext.Session.GetInt32("idUsuario") != null)
                sessionidusuario = (int)HttpContext.Session.GetInt32("idUsuario");
            return sessionidusuario;
        }
    }

    public IActionResult Index()
    {
        var idUsuario = HttpContext.Session.GetInt32("idUsuario");
        if (idUsuario == null) return RedirectToAction("Index", "Login");

        var dbconsult = _db.Usuarios.Find(idUsuario);
        if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
            return RedirectToAction("Index", "Login");

     

        var viewModel = new HomeViewModel();

        GraficosHorasFaltasxHorasExtras(viewModel);
        GraficosHorasFaltas(viewModel);
        GraficosHorasExtras(viewModel);
        GraficoRankingDiasComMaisFaltas(viewModel);
        GraficoJustificativas(viewModel);
        TotalHorasFaltas(viewModel);
        TotalHorasExtras(viewModel);
        GraficoDiasTrabalhados(viewModel);

        ViewBag.nomeCompleto = dbconsult.NomeCompleto;
        ViewBag.email = dbconsult.Email;
        ViewBag.tipoPerfil = dbconsult.TipoPerfil;

        return View(viewModel);
    }

    private void TotalHorasFaltas(HomeViewModel viewModel)
    {
        var idUsuario = HttpContext.Session.GetInt32("idUsuario");
        if (idUsuario == null) return;

        var usuarioLogado = _db.Usuarios.FirstOrDefault(u => u.Id == idUsuario);
        if (usuarioLogado == null) return;

        double totalHorasFaltas = 0;

        if (usuarioLogado.TipoPerfil == 1)
        {
            // Tipo 1: Todos os colaboradores
            totalHorasFaltas = _db.ExtratosPonto
                .Where(e => e.HorasFaltas.HasValue)
                .AsEnumerable() 
                .Sum(e => e.HorasFaltas.Value.ToTimeSpan().TotalHours);
        }
        else
        {
            // Tipo 2 ou 3: Apenas o funcionário logado
            var funcionario = _db.Funcionarios
                .FirstOrDefault(f => f.Matricula == usuarioLogado.Matricula);

            if (funcionario == null)
                return;

            totalHorasFaltas = _db.ExtratosPonto
                .Where(e => e.Matricula == funcionario.Matricula && e.HorasFaltas.HasValue)
                .AsEnumerable() // ❗ Executa o cálculo na memória
                .Sum(e => e.HorasFaltas.Value.ToTimeSpan().TotalHours);
        }

        // Preenche o ViewModel
        viewModel.TotalHorasFaltas = totalHorasFaltas;
        viewModel.TotalHorasFaltasFormatado = FormatarHorasComSinal(totalHorasFaltas, isFalta: true);
    }

    private void TotalHorasExtras(HomeViewModel viewModel)
    {
        var idUsuario = HttpContext.Session.GetInt32("idUsuario");
        if (idUsuario == null) return;

        var usuarioLogado = _db.Usuarios.FirstOrDefault(u => u.Id == idUsuario);
        if (usuarioLogado == null) return;

        double totalHorasExtras = 0;

        if (usuarioLogado.TipoPerfil == 1)
        {
            // Tipo 1: Todos os colaboradores
            totalHorasExtras = _db.ExtratosPonto
                .Where(e => e.HorasExtras.HasValue)
                .AsEnumerable()
                .Sum(e => e.HorasExtras.Value.ToTimeSpan().TotalHours);
        }
        else
        {
            // Tipo 2 ou 3: Apenas o funcionário logado
            var funcionario = _db.Funcionarios
                .FirstOrDefault(f => f.Matricula == usuarioLogado.Matricula);

            if (funcionario == null)
                return;

            totalHorasExtras = _db.ExtratosPonto
                .Where(e => e.Matricula == funcionario.Matricula && e.HorasExtras.HasValue)
                .AsEnumerable() 
                .Sum(e => e.HorasExtras.Value.ToTimeSpan().TotalHours);
        }

        // Preenche o ViewModel
        viewModel.TotalHorasExtras = totalHorasExtras;
        viewModel.TotalHorasExtrasFormatado = FormatarHorasComSinal(totalHorasExtras, isFalta: false);
    }


    private void GraficosHorasFaltasxHorasExtras(HomeViewModel viewModel)
    {
        var idUsuario = HttpContext.Session.GetInt32("idUsuario");
        if (idUsuario == null) return;

        var usuarioLogado = _db.Usuarios.FirstOrDefault(u => u.Id == idUsuario);
        if (usuarioLogado == null) return;

        var nomesSetores = new List<string>();
        var quantidadeHorasFaltas = new List<double>();
        var quantidadeHorasExtras = new List<double>();
        var labelsHorasFaltasFormatadas = new List<string>();
        var labelsHorasExtrasFormatadas = new List<string>();

        if (usuarioLogado.TipoPerfil == 1)
        {
            // Lógica atual: agrupar por setor
            var extratos = _db.ExtratosPonto.ToList();
            var funcionarios = _db.Funcionarios.ToList();
            var setores = _db.Setores.ToList();

            var agrupadoPorSetor = extratos
                .Join(funcionarios,
                      e => e.Matricula,
                      f => f.Matricula,
                      (e, f) => new { e, f })
                .Join(setores,
                      ef => ef.f.SetorId,
                      s => s.Id,
                      (ef, s) => new
                      {
                          SetorNome = s.Nome,
                          HorasFaltas = ef.e.HorasFaltas,
                          HorasExtras = ef.e.HorasExtras
                      })
                .GroupBy(x => x.SetorNome)
                .Select(g => new
                {
                    NomeSetor = g.Key,
                    TotalHorasFaltas = g.Sum(x =>
                        x.HorasFaltas?.ToTimeSpan().TotalHours ?? 0),
                    TotalHorasExtras = g.Sum(x =>
                        x.HorasExtras?.ToTimeSpan().TotalHours ?? 0)
                })
                .ToList();

            foreach (var item in agrupadoPorSetor)
            {
                nomesSetores.Add(item.NomeSetor);
                quantidadeHorasFaltas.Add(item.TotalHorasFaltas);
                quantidadeHorasExtras.Add(item.TotalHorasExtras);

                labelsHorasFaltasFormatadas.Add(FormatarHorasComSinal(item.TotalHorasFaltas, isFalta: true));
                labelsHorasExtrasFormatadas.Add(FormatarHorasComSinal(item.TotalHorasExtras, isFalta: false));
            }
        }
        else if (usuarioLogado.TipoPerfil == 2 || usuarioLogado.TipoPerfil == 3)
        {
            // Busca o funcionário pelo usuário logado
            var funcionario = _db.Funcionarios
                .FirstOrDefault(f => f.Matricula == usuarioLogado.Matricula);

            if (funcionario != null)
            {
                // Busca todos os registros do ponto do usuário logado
                var extratosDoUsuario = _db.ExtratosPonto
                    .Where(e => e.Matricula == funcionario.Matricula)
                    .ToList();

                // Soma total de horas extras e faltas
                double totalHorasFaltas = extratosDoUsuario
                    .Sum(e => e.HorasFaltas?.ToTimeSpan().TotalHours ?? 0);

                double totalHorasExtras = extratosDoUsuario
                    .Sum(e => e.HorasExtras?.ToTimeSpan().TotalHours ?? 0);

                // Adiciona os dados como único item no gráfico
                nomesSetores.Add($"{funcionario.Nome}"); // Exibe nome do usuário
                quantidadeHorasFaltas.Add(totalHorasFaltas);
                quantidadeHorasExtras.Add(totalHorasExtras);

                labelsHorasFaltasFormatadas.Add(FormatarHorasComSinal(totalHorasFaltas, isFalta: true));
                labelsHorasExtrasFormatadas.Add(FormatarHorasComSinal(totalHorasExtras, isFalta: false));
            }
        }

        viewModel.NomesSetores = nomesSetores;
        viewModel.QuantidadeHorasFaltas = quantidadeHorasFaltas;
        viewModel.QuantidadeHorasExtras = quantidadeHorasExtras;
        viewModel.LabelsHorasFaltasFormatadas = labelsHorasFaltasFormatadas;
        viewModel.LabelsHorasExtrasFormatadas = labelsHorasExtrasFormatadas;
    }

    private void GraficosHorasFaltas(HomeViewModel viewModel)
    {
        var idUsuario = HttpContext.Session.GetInt32("idUsuario");
        if (idUsuario == null) return;

        var usuarioLogado = _db.Usuarios.FirstOrDefault(u => u.Id == idUsuario);
        if (usuarioLogado == null) return;

        var meses = new List<string>();
        var horasFaltasPorMes = new List<double>();
        var labelsHorasFaltasFormatadas = new List<string>();

        // Variável que vai armazenar os extratos a serem usados
        IEnumerable<ExtratoPontoModel> extratosQuery;

        if (usuarioLogado.TipoPerfil == 1)
        {
            // Tipo 1: Todos os funcionários
            extratosQuery = _db.ExtratosPonto.ToList();
        }
        else
        {
            // Tipo 2 ou 3: Apenas o funcionário logado
            var funcionario = _db.Funcionarios
                .FirstOrDefault(f => f.Matricula == usuarioLogado.Matricula);

            if (funcionario == null)
                return;

            extratosQuery = _db.ExtratosPonto
                .Where(e => e.Matricula == funcionario.Matricula)
                .ToList();
        }

        // Agrupa por Mês/Ano e soma as horas faltas
        var agrupadoPorMes = extratosQuery
            .Where(e => e.HorasFaltas.HasValue) // Garante que HorasFaltas não seja nulo
            .GroupBy(e => new { Mes = e.DataBatida.Month, Ano = e.DataBatida.Year }) // Extrai mês e ano da data
            .Select(g => new
            {
                MesAno = new DateTime(g.Key.Ano, g.Key.Mes, 1), // Cria uma data válida para ordenação
                TotalHorasFaltas = g.Sum(e =>
                    e.HorasFaltas?.ToTimeSpan().TotalHours ?? 0) // Converte TimeOnly para horas decimais
            })
            .OrderBy(x => x.MesAno) // Ordena cronologicamente
            .ToList();

        foreach (var item in agrupadoPorMes)
        {
            meses.Add(item.MesAno.ToString("MMM/yyyy")); // Ex: "Jan/2024"
            horasFaltasPorMes.Add(item.TotalHorasFaltas);
            labelsHorasFaltasFormatadas.Add(FormatarHorasComSinal(item.TotalHorasFaltas, isFalta: true));
        }

        // Atualiza o ViewModel com os novos dados
        viewModel.LabelsMesesHorasFaltas = meses;
        viewModel.QuantidadeHorasFaltasPorMes = horasFaltasPorMes;
        viewModel.LabelsHorasFaltasPorMesFormatadas = labelsHorasFaltasFormatadas;
    }



    private void GraficosHorasExtras(HomeViewModel viewModel)
    {
        var idUsuario = HttpContext.Session.GetInt32("idUsuario");
        if (idUsuario == null) return;

        var usuarioLogado = _db.Usuarios.FirstOrDefault(u => u.Id == idUsuario);
        if (usuarioLogado == null) return;

        var meses = new List<string>();
        var horasExtrasPorMes = new List<double>();
        var labelsHorasExtrasFormatadas = new List<string>();

        // Variável que vai armazenar os extratos a serem usados
        IEnumerable<ExtratoPontoModel> extratosQuery;

        if (usuarioLogado.TipoPerfil == 1)
        {
            // Tipo 1: Todos os funcionários
            extratosQuery = _db.ExtratosPonto.ToList();
        }
        else
        {
            // Tipo 2 ou 3: Apenas o funcionário logado
            var funcionario = _db.Funcionarios
                .FirstOrDefault(f => f.Matricula == usuarioLogado.Matricula);

            if (funcionario == null)
                return;

            extratosQuery = _db.ExtratosPonto
                .Where(e => e.Matricula == funcionario.Matricula)
                .ToList();
        }

        // Agrupa por Mês/Ano e soma as horas faltas
        var agrupadoPorMes = extratosQuery
            .Where(e => e.HorasExtras.HasValue) // Garante que HorasFaltas não seja nulo
            .GroupBy(e => new { Mes = e.DataBatida.Month, Ano = e.DataBatida.Year }) // Extrai mês e ano da data
            .Select(g => new
            {
                MesAno = new DateTime(g.Key.Ano, g.Key.Mes, 1), // Cria uma data válida para ordenação
                TotalHorasExtras = g.Sum(e =>
                    e.HorasExtras?.ToTimeSpan().TotalHours ?? 0) // Converte TimeOnly para horas decimais
            })
            .OrderBy(x => x.MesAno) // Ordena cronologicamente
            .ToList();

        foreach (var item in agrupadoPorMes)
        {
            meses.Add(item.MesAno.ToString("MMM/yyyy")); // Ex: "Jan/2024"
            horasExtrasPorMes.Add(item.TotalHorasExtras);
            labelsHorasExtrasFormatadas.Add(FormatarHorasComSinal(item.TotalHorasExtras, isFalta: false));
        }

        // Atualiza o ViewModel com os novos dados
        viewModel.LabelsMesesHorasExtras = meses;
        viewModel.QuantidadeHorasExtrasPorMes = horasExtrasPorMes;
        viewModel.LabelsHorasExtrasPorMesFormatadas = labelsHorasExtrasFormatadas;
    }


    private void GraficoRankingDiasComMaisFaltas(HomeViewModel viewModel)
    {
        var idUsuario = HttpContext.Session.GetInt32("idUsuario");
        if (idUsuario == null) return;

        var usuarioLogado = _db.Usuarios.FirstOrDefault(u => u.Id == idUsuario);
        if (usuarioLogado == null) return;

        var diasSemana = new List<string>();
        var horasFaltasPorDia = new List<double>();
        var labelsHorasFaltasFormatadas = new List<string>();

        IEnumerable<ExtratoPontoModel> extratosQuery;

        if (usuarioLogado.TipoPerfil == 1)
        {
            // Tipo 1: Todos os funcionários
            extratosQuery = _db.ExtratosPonto.ToList();
        }
        else
        {
            // Tipo 2 ou 3: Apenas o funcionário logado
            var funcionario = _db.Funcionarios
                .FirstOrDefault(f => f.Matricula == usuarioLogado.Matricula);

            if (funcionario == null)
                return;

            extratosQuery = _db.ExtratosPonto
                .Where(e => e.Matricula == funcionario.Matricula)
                .ToList();
        }

        // Agrupar por dia da semana e somar as horas faltas
        var agrupadoPorDiaDaSemana = extratosQuery
            .Where(e => e.HorasFaltas.HasValue && e.DataBatida != null) // Garante campos válidos
            .GroupBy(e => e.DataBatida.DayOfWeek) // Agrupa pelo dia da semana
            .Select(g => new
            {
                DiaSemana = g.Key,
                TotalHorasFaltas = g.Sum(e =>
                    e.HorasFaltas?.ToTimeSpan().TotalHours ?? 0) // Converte TimeOnly para horas decimais
            })
            .OrderByDescending(x => x.TotalHorasFaltas) // Ordena do maior para o menor
            .ToList();

        foreach (var item in agrupadoPorDiaDaSemana)
        {
            diasSemana.Add(TraduzirDia(item.DiaSemana)); // Ex: "Segunda-feira"
            horasFaltasPorDia.Add(item.TotalHorasFaltas);
            labelsHorasFaltasFormatadas.Add(FormatarHorasComSinal(item.TotalHorasFaltas, isFalta: true));
        }

        // Preenche o ViewModel
        viewModel.LabelsDiasSemana = diasSemana;
        viewModel.QuantidadeHorasFaltasPorDia = horasFaltasPorDia;
        viewModel.LabelsHorasFaltasPorDiaFormatadas = labelsHorasFaltasFormatadas;
    }

    private void GraficoJustificativas(HomeViewModel viewModel)
    {
        var idUsuario = HttpContext.Session.GetInt32("idUsuario");
        if (idUsuario == null) return;

        var usuarioLogado = _db.Usuarios.FirstOrDefault(u => u.Id == idUsuario);
        if (usuarioLogado == null) return;

        var justificativas = new List<string>();
        var ocorrencias = new List<int>();

        IEnumerable<ExtratoPontoModel> extratosQuery;

        if (usuarioLogado.TipoPerfil == 1)
        {
            extratosQuery = _db.ExtratosPonto.ToList();
        }
        else
        {
            var funcionario = _db.Funcionarios
                .FirstOrDefault( f => f.Matricula == usuarioLogado.Matricula );

            if (funcionario == null)
                return;

            extratosQuery = _db.ExtratosPonto
                .Where(e => e.Matricula == funcionario.Matricula)
                .ToList();
        }

        var agrupadoPorJustificativa = extratosQuery
            .Where( e => !string.IsNullOrWhiteSpace(e.Justificativa))
            .GroupBy( e =>  e.Justificativa.Trim())
            .Select( g => new
            {
                Justificativas = g.Key,
                Quantidade = g.Count()
            })
            .OrderByDescending( x => x.Quantidade )
            .ToList();

        foreach (var item in agrupadoPorJustificativa)
        {
            justificativas.Add(item.Justificativas);
            ocorrencias.Add(item.Quantidade);
        }

        viewModel.LabelsJustificativas = justificativas;
        viewModel.QuantidadeOcorrenciasPorJustificativa = ocorrencias;
    }

    private void GraficoDiasTrabalhados(HomeViewModel viewModel)
    {
        var idUsuario = HttpContext.Session.GetInt32("idUsuario");
        if (idUsuario == null) return;

        var usuarioLogado = _db.Usuarios.FirstOrDefault(u => u.Id == idUsuario);
        if (usuarioLogado == null) return;

        // Listas para o gráfico
        var meses = new List<string>();
        var diasTrabalhadosPorMes = new List<int>();

        try
        {
            if (usuarioLogado.TipoPerfil == 1)
            {
                Console.WriteLine("Buscando dados para TipoPerfil == 1");

                var todosRegistros = _db.ExtratosPonto.ToList(); // Carrega TUDO para depurar

                var comDataBatida = todosRegistros.Where(e => e.DataBatida != null).ToList();

                var dados = comDataBatida
                    .GroupBy(e => new { e.DataBatida.Year, e.DataBatida.Month })
                    .Select(g => new
                    {
                        MesAno = new DateTime(g.Key.Year, g.Key.Month, 1),
                        DiasTrabalhados = g.Select(e => new { e.Matricula, Data = e.DataBatida.Date }).Distinct().Count()
                    })
                    .OrderBy(g => g.MesAno)
                    .ToList();


                foreach (var item in dados)
                {
                    Console.WriteLine($"{item.MesAno.ToString("MMM/yyyy")} → {item.DiasTrabalhados} dias trabalhados");
                    meses.Add(item.MesAno.ToString("MMM/yyyy"));
                    diasTrabalhadosPorMes.Add(item.DiasTrabalhados);
                }
            }
            else
            {
                var funcionario = _db.Funcionarios
                    .FirstOrDefault(f => f.Matricula == usuarioLogado.Matricula);

                if (funcionario == null)
                {
                    return;
                }


                var registrosDoUsuario = _db.ExtratosPonto
                    .Where(e => e.Matricula == funcionario.Matricula)
                    .ToList();


                var comDataBatida = registrosDoUsuario
                    .Where(e => e.DataBatida != null)
                    .ToList();


                var dados = comDataBatida
                    .GroupBy(d => new { d.DataBatida.Year, d.DataBatida.Month })
                    .Select(g => new
                    {
                        MesAno = new DateTime(g.Key.Year, g.Key.Month, 1),
                        DiasTrabalhados = g.Select(x => x.DataBatida.Date).Distinct().Count()
                    })
                    .OrderBy(g => g.MesAno)
                    .ToList();


                foreach (var item in dados)
                {
                    Console.WriteLine($"{item.MesAno.ToString("MMM/yyyy")} → {item.DiasTrabalhados} dias trabalhados");
                    meses.Add(item.MesAno.ToString("MMM/yyyy"));
                    diasTrabalhadosPorMes.Add(item.DiasTrabalhados);
                }
            }

            // Preenche o ViewModel
            viewModel.LabelsDiasTrabalhadosPorMes = meses;
            viewModel.TotalDiasTrabalhadosPorMes = diasTrabalhadosPorMes;

            // Total geral
            viewModel.TotalDiasTrabalhados = diasTrabalhadosPorMes.Sum();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar dias trabalhados: {ex.Message}");
        }
    }

    private string FormatarHorasComSinal(double horas, bool isFalta)
    {
        var valorComSinal = isFalta ? -Math.Abs(horas) : Math.Abs(horas);
        var sinal = valorComSinal >= 0 ? "+" : "-";
        var horasAbsolutas = Math.Abs(horas);

        var horasInt = (int)horasAbsolutas;
        var minutos = (int)((horasAbsolutas - horasInt) * 60);

        return $"{sinal}{horasInt:D2}:{minutos:D2}";
    }


    private string TraduzirDia(DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Monday => "Segunda-feira",
            DayOfWeek.Tuesday => "Terça-feira",
            DayOfWeek.Wednesday => "Quarta-feira",
            DayOfWeek.Thursday => "Quinta-feira",
            DayOfWeek.Friday => "Sexta-feira",
            DayOfWeek.Saturday => "Sabado",
            DayOfWeek.Sunday => "Domingo",
            _ => day.ToString()
        };
    }
}
