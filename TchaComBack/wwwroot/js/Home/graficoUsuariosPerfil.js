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
                    backgroundColor: 'rgba(60, 179, 113, 0.6)',
                    borderColor: 'rgba(60, 179, 113, 1)',
                    borderWidth: 1
                },
                {
                    label: 'Usuários Inativos',
                    data: dados.usuariosInativosPorPerfil,
                    backgroundColor: 'rgba(220, 20, 60, 0.6)',
                    borderColor: 'rgba(220, 20, 60, 1)',
                    borderWidth: 1
                }
            ]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'top'
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    title: {
                        display: false,
                        text: 'Quantidade de Usuários'
                    }
                },
                x: {
                    title: {
                        display: true,
                        text: 'Perfil de Usuário'
                    }
                }
            }
        }
    });
});
