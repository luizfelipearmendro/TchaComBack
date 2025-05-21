document.addEventListener('DOMContentLoaded', function () {
    const dataJson = document.getElementById('dadosGraficos');
    if (!dataJson) return;

    const dados = JSON.parse(dataJson.textContent);

    const ctx = document.getElementById('idGraficoUsuariosPorPerfil');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: dados.labelsPerfis,
            datasets: [
                {
                    label: 'Usuários Ativos',
                    data: dados.usuariosAtivosPorPerfil,
                    backgroundColor: 'rgba(255, 165, 0, 1)',
                    borderColor:'rgba(255, 165, 0, 1)',
                    borderWidth: 1
                },
                {
                    label: 'Usuários Inativos',
                    data: dados.usuariosInativosPorPerfil,
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
                    title: {
                        display: false,
                        text: 'Quantidade de Usuários',
                        color: 'white'
                    },
                    ticks: {
                        color: 'white' 
                    }
                },
                x: {
                    title: {
                        display: false,
                        text: 'Perfil de Usuário',
                        color: 'white'
                    },
                    ticks: {
                        color: 'white'
                    }
                }
            }
        }
    });
});
