
function formatDecimalToTime(decimal) {
    const hours = Math.floor(decimal);
    const minutes = Math.round((decimal - hours) * 60);
    return `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}`;
}

document.addEventListener('DOMContentLoaded', function () {
    const dataJson = document.getElementById('dadosGraficos');
    if (!dataJson) return;

    const dados = JSON.parse(dataJson.textContent);

    const ctx = document.getElementById('idGraficoBarrasLaterais');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: dados.labelsDiasSemana,
            datasets: [{
                label: 'Horas Faltas',
                data: dados.dataHorasFaltasPorDia,
                borderColor: 'rgba(255, 99, 132, 1)',
                backgroundColor: 'rgba(255, 99, 132, 1)',
                borderWidth: 1,
                borderRadius: 6
            }]
        },
        options: {
            indexAxis: 'y', // Isso faz as barras ficarem horizontais
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            const value = context.raw;
                            const formatted = formatDecimalToTime(value);
                            return ` Horas Faltas: ${formatted}`;
                        }
                    }
                },
                legend: {
                    display: false,
                    position: 'bottom'
                },
                datalabels: {
                    align: 'end',
                    anchor: 'bottom',
                    formatter: function(value) {
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
                x: {
                    ticks: {
                        callback: function(value) {
                            return formatDecimalToTime(value);
                        }
                    },
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Horas Faltas',
                    },
                    ticks: {
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
                y: {
                    ticks: {
                        beginAtZero: true
                    }
                }
            }
        },
        plugins: [ChartDataLabels]
    });
});
