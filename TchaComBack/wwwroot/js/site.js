//funcionalidade de ao clicar o botao da mensagem de alerta, saia da tela
$(document).ready(function () {
    $('.closebtn').click(function () {
        $(this).closest('.alert').fadeOut();  // Fechar o alerta ao clicar no botão
    });

    // Fechar automaticamente o alerta após 3 segundos
    setTimeout(function () {
        $('.alert').fadeOut(); // Use um ponto para separar as classes
    }, 3000); // 3000 milissegundos = 3 segundos
});



