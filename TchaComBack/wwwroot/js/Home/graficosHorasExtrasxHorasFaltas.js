﻿document.addEventListener('DOMContentLoaded', function () {
    const dataJson = document.getElementById('dadosGraficos');
    if (!dataJson) return;

    const dados = JSON.parse(dataJson.textContent);

    const ctx = document.getElementById('idGraficoHorasExtrasxHorasFaltas');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: dados.labelsSetores,
            datasets: [
                {
                    label: 'Horas Extras',
                    data: dados.dataHorasExtras,
                    borderColor: 'rgba(40, 130, 70, 1)',         
                    backgroundColor: 'rgba(40, 130, 70, 1)',   
                    borderWidth: 1,
                    borderRadius: 6
                },
                {
                    label: 'Horas Faltas',
                    data: dados.dataHorasFaltas,
                    borderColor: 'rgba(255, 99, 132, 1)', 
                    backgroundColor: 'rgba(255, 99, 132, 1)', 
                    borderWidth: 1,
                    borderRadius: 6
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
                    position: 'bottom',
                    labels: {
                        usePointStyle: true,
                        pointStyle: 'circle', 
                        padding: 20
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
                    // Remove as linhas verticais do grid (opcionais)
                    grid: {
                        display: false
                    },
                    ticks: {
                        font: {
                            size: 14
                        }
                    }
                }
            }
        },
        plugins: [ChartDataLabels]
    });
});