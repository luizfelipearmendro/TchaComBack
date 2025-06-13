document.addEventListener('DOMContentLoaded', function () {
    const dataJson = document.getElementById('dadosGraficos');
    if (!dataJson) return;

    const dados = JSON.parse(dataJson.textContent);

    const canvas = document.getElementById('idGraficoRankingSetores');
    if (!canvas) return;

    const ctx = canvas.getContext('2d');

    const gradient = ctx.createLinearGradient(0, 0, canvas.width, 0); 
    gradient.addColorStop(0, '#8A2BE2');  // roxo
    gradient.addColorStop(1, '#FFA500');  // laranja

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: dados.labelsRankingSetores,
            datasets: [{
                label: 'Funcionários por Setor',
                data: dados.quantidadeFuncionariosRankingSetores,
                backgroundColor: gradient,
                borderColor: gradient,
                borderWidth: 1
            }]
        },
        options: {
            indexAxis: 'y',
            responsive: true,
            plugins: {
                legend: {
                    display: false,
                    labels: {
                        position: 'bottom',
                        color: 'white'
                    }
                },
                title: {
                    display: false,
                    text: 'Ranking dos Setores com Mais Funcionários'
                }
            },
            datalabels: {
                anchor: 'end',
                align: 'bottom',
                formatter: function (value) {
                    return formatDecimalToTime(value);
                },
                font: {
                    weight: 'bold',
                    size: 14
                }
            },
            scales: {
                x: {
                    beginAtZero: true,
                    ticks: {
                        display: false,
                    },
                    grid: {
                        display: false
                    }
                },
                y: {
                    beginAtZero: true,
                    ticks: {
                        
                    },
                }
            }
        },
        plugins: [ChartDataLabels]
    });
});
