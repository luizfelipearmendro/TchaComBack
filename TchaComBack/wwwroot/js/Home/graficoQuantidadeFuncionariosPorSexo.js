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
                    'rgba(255, 165, 0, 0.6)',   // Masculino - Azul
                    'rgba(138, 43, 226, 0.6)',   // Feminino - Rosa
                    'rgba(255, 206, 86, 0.6)'    // Outro - Amarelo
                ],
                borderColor: [
                    'rgba(255, 165, 0, 1)',
                    'rgba(138, 43, 226, 1)',
                    'rgba(255, 206, 86, 1)'
                ],
                borderWidth: 1
            }]
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
            }
        }
    });
});