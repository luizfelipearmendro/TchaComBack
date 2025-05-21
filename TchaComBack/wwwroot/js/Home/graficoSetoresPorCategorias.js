document.addEventListener('DOMContentLoaded', function () {
    const dataJson = document.getElementById('dadosGraficos');
    if (!dataJson) return;

    const dados = JSON.parse(dataJson.textContent);

    const ctx = document.getElementById('idGraficoSetoresPorCategorias');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: dados.labelsCategorias,
            datasets: [
                {
                    label: 'Setores Ativos',
                    data: dados.dataSetoresAtivos,
                    backgroundColor: 'rgba(255, 165, 0,1)',
                    borderColor: 'rgba(255, 165, 0, 1)',
                    borderWidth: 1
                },
                {
                    label: 'Setores Inativos',
                    data: dados.dataSetoresInativos,
                    backgroundColor: 'rgba(138, 43, 226, 1)',
                    borderColor: 'rgba(138, 43, 226, 1)',
                    borderWidth: 1
                }
            ]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'top',
                    labels: {
                        color: 'white'
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        color: 'white'
                    }
                },
                x: {
                    title: {
                        display: false,
                        text: 'Categorias'
                    },
                    ticks: {
                        color: 'white'
                    }
                }
            }
        }
    });
});
