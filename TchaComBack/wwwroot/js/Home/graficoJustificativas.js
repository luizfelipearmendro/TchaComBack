
document.addEventListener('DOMContentLoaded', function () {
    const dataJson = document.getElementById('dadosGraficos');
    if (!dataJson) return;

    const dados = JSON.parse(dataJson.textContent);

    const ctx = document.getElementById('idGraficoJustificativas');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: dados.labelsJustificativas,
            datasets: [{
                label: 'Quantidade de Ocorrências',
                data: dados.dataOcorrenciasPorJustificativa,
                backgroundColor: 'rgba(54, 162, 235, 1)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1,
                borderRadius: 6
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
                            return ` ${context.raw} ocorrências`;
                        }
                    }
                },
                datalabels: {
                    anchor: 'end',
                    align: 'bottom',
                    formatter: function (value) {
                        return formatDecimalToTime(value);
                    },
                    color: 'white',
                    font: {
                        weight: 'bold',
                        size: 12,

                    }
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
                    }
                }
            }
        },
        plugins: [ChartDataLabels]
    });
});
