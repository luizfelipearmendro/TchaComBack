
document.addEventListener('DOMContentLoaded', function () {
    const dataJson = document.getElementById('dadosGraficos');
    if (!dataJson) return;

    const dados = JSON.parse(dataJson.textContent);

    const ctx = document.getElementById('idGraficoDiasTrabalhados');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: dados.labelsMesesDiasTrabalhados,
            datasets: [{
                label: 'Dias Trabalhados por Mês',
                data: dados.dataDiasTrabalhadosPorMes,
                borderColor: 'white', // linha branca
                backgroundColor: 'rgba(255, 255, 255, 0.2)', // preenchimento sutil branco
                fill: true,
                tension: 0.4,
                pointRadius: 4,
                pointBackgroundColor: 'white', // pontos brancos
                pointBorderColor: 'white' // borda dos pontos branca
            }]

        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            return `${context.raw} dias`;
                        }
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

                        // Opcional: se quiser manter ticks invisíveis mas funcionais
                        callback: function (value) {
                            return formatDecimalToTime(value);
                        }
                    },
                    // Remove as linhas de grade horizontais
                    grid: {
                        display: false
                    }
                },
                x: {
                    ticks: {
                        color: 'white'
                    },
                    grid: {
                        display: false
                    }
                }
            }
        },
       plugins: [ChartDataLabels]
    });
});
