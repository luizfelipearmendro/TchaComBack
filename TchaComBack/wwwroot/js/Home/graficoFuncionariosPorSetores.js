document.addEventListener('DOMContentLoaded', function () {
    const dataJson = document.getElementById('dadosGraficos');
    if (!dataJson) return;

    const dados = JSON.parse(dataJson.textContent);

    const ctx = document.getElementById('idGraficoFuncionariosPorSetores');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: dados.labelsSetores,
            datasets: [
                {
                    label: 'Funcionários Ativos',
                    data: dados.dataFuncionariosAtivos,
                    backgroundColor: 'rgba(255, 165, 0, 1)',
                    borderColor: 'rgba(255, 165, 0, 1)',
                    borderWidth: 1,
                    borderRadius: 0
                },
                {
                    label: 'Funcionários Inativos',
                    data: dados.dataFuncionariosInativos,
                    backgroundColor: 'rgba(138, 43, 226, 1)',
                    borderColor: 'rgba(138, 43, 226, 1)',
                    borderWidth: 1,
                    borderRadius: 6
                }
            ]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                    }
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
                y: {
                    beginAtZero: true,
                    ticks: {
                        // Oculta os números do eixo Y
                        display: false,
                    },
                    // Remove as linhas de grade horizontais
                    grid: {
                        display: false
                    }
                },
                x: {
                    title: {
                        display: false,
                        text: 'Setores'
                    },
                    ticks: {
                    }
                }
            }
        },
        plugins: [ChartDataLabels]

    });
});
