document.addEventListener('DOMContentLoaded', function () {
    const dataJson = document.getElementById('dadosGraficos');
    if (!dataJson) return;

    const dados = JSON.parse(dataJson.textContent);

    const ctx = document.getElementById('idGraficoFuncionariosPorSexo');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: dados.labelsSexoFuncionarios,
            datasets: [{
                data: dados.quantidadeFuncionariosPorSexo,
                backgroundColor: [
                    'rgba(54, 162, 235, 0.6)',   // Masculino - Azul
                    'rgba(255, 99, 132, 0.6)',   // Feminino - Rosa
                    'rgba(255, 206, 86, 0.6)'    // Outro - Amarelo
                ],
                borderColor: [
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 99, 132, 1)',
                    'rgba(255, 206, 86, 1)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'top'
                }
            }
        }
    });
});