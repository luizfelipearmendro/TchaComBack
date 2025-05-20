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
                    backgroundColor: 'rgba(255, 165, 0, 0.6)',
                    borderColor: 'rgba(255, 165, 0, 1)',
                    borderWidth: 1
                },
                {
                    label: 'Funcionários Inativos',
                    data: dados.dataFuncionariosInativos,
                    backgroundColor: 'rgba(138, 43, 226, 0.6)',
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
                        text: 'Setores'
                    },
                    ticks: {
                        color: 'white'
                    }
                }
            }
        }
    });
});
