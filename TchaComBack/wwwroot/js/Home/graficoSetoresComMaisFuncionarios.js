document.addEventListener('DOMContentLoaded', function () {
    const dataJson = document.getElementById('dadosGraficos');
    if (!dataJson) return;

    const dados = JSON.parse(dataJson.textContent);

    const ctx = document.getElementById('idGraficoRankingSetores');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: dados.labelsRankingSetores,
            datasets: [{
                label: 'Funcionários por Setor',
                data: dados.quantidadeFuncionariosRankingSetores,
                backgroundColor: 'rgba(60, 179, 113, 0.7)',
                borderColor: 'rgba(60, 179, 113, 1)',
                borderWidth: 1
            }]
        },
        options: {
            indexAxis: 'y', 
            responsive: true,
            plugins: {
                legend: {
                    display: false
                },
                title: {
                    display: false,
                    text: 'Ranking dos Setores com Mais Funcionários'
                }
            },
            scales: {
                x: {
                    beginAtZero: true
                }
            }
        }
    });
});
