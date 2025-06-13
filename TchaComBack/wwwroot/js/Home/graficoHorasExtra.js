
function formatDecimalToTime(decimal) {
    const hours = Math.floor(decimal);
    const minutes = Math.round((decimal - hours) * 60);
    return `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}`;
}

document.addEventListener('DOMContentLoaded', function () {
    const dataJson = document.getElementById('dadosGraficos');
    if (!dataJson) return;

    const dados = JSON.parse(dataJson.textContent);

    const ctx = document.getElementById('idGraficoHorasExtras');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: dados.labelsMesesHorasExtras,
            datasets: [
                {
                    label: 'Horas Extras',
                    data: dados.dataHorasExtrasPorMes,
                    backgroundColor: 'rgba(54, 162, 235, .2)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    fill: true, 
                    tension: 0.4, 
                    pointRadius: 4,
                    pointBackgroundColor: 'white',
                    pointBorderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 2
                }
            ]
        },
        options: {
            responsive: true,
            plugins: {
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            const value = context.raw;
                            const formatted = formatDecimalToTime(value);
                            return ` ${context.dataset.label}: ${formatted}`;
                        }
                    }
                },
                legend: {
                    display: false
                },
                datalabels: {
                    anchor: 'end',
                    align: 'top',
                    formatter: function (value) {
                        return formatDecimalToTime(value);
                    },
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
                        color: 'white'
                    }
                }
            }
        },
        plugins: [ChartDataLabels]
    });
});
