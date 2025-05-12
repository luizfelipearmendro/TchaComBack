//document.addEventListener("DOMContentLoaded", function () {
//    const conteudo = document.getElementById('conteudo-funcionarios');
//    const filtro = document.getElementById('id-filtro-func');
//    const toggleButton = document.getElementById('toggle-filtro-func');
//    const form = document.getElementById('id-filtro-func');

//    function rolagemAoRodape(posicaoRolagem) {
//        if (!conteudo || !filtro) {
//            console.error("Elemento '#conteudo-funcionarios' ou '#id-filtro' não encontrado!");
//            return;
//        }

//        const topoConteudo = conteudo.offsetTop;
//        const alturaConteudo = conteudo.offsetHeight;
//        const baseConteudo = topoConteudo + alturaConteudo;
//        if (posicaoRolagem >= topoConteudo && posicaoRolagem < baseConteudo) {
//            filtro.classList.add('filtro-scroll-active');
//        } else {
//            filtro.classList.remove('filtro-scroll-active');

//            if (form.classList.contains('collapsed')) {
//                form.style.display = 'block';
//                form.classList.remove('collapsed');
//            }
//        }
//    }
//    rolagemAoRodape(window.scrollY);

//    window.addEventListener('scroll', () => {
//        rolagemAoRodape(window.scrollY);
//    });

//    toggleButton.addEventListener('click', function () {
//        form.classList.toggle('collapsed');

//        if (form.classList.contains('collapsed')) {
//            toggleButton.textContent = '˅';
//        } else {
//            toggleButton.textContent = '˄';
//        }
//    });



//});

function closeAlert(alertId) {
    const alertElement = document.getElementById(alertId);
    if (alertElement) {
        alertElement.style.animation = 'fadeOut 0.5s ease-in-out';
        setTimeout(() => {
            alertElement.style.display = 'none';
        }, 500);
    }
}

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.btn-popover-toggle').forEach(button => {
        button.addEventListener('click', function (e) {
            const id = button.getAttribute('data-id');
            const popover = document.getElementById(`popover-${id}`);

            // Esconde todos os outros popovers
            document.querySelectorAll('.popover-content').forEach(p => {
                if (p !== popover) p.style.display = 'none';
            });

            // Toggle no atual
            popover.style.display = popover.style.display === 'block' ? 'none' : 'block';

            // Fecha se clicar fora
            document.addEventListener('click', function closePopover(e) {
                if (!popover.contains(e.target) && !button.contains(e.target)) {
                    popover.style.display = 'none';
                    document.removeEventListener('click', closePopover);
                }
            });
        });
    });

    // Fecha ao clicar no botão X
    document.querySelectorAll('.close-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        btn.closest('.popover-content').style.display = 'none';
    });
    });
});


document.getElementById('telefone').addEventListener('input', function (e) {
    let value = e.target.value.replace(/\D/g, '');

    if (value.length > 11) value = value.slice(0, 11);

    if (value.length <= 10) {
        // Fixo: (00) 0000-0000
        value = value.replace(/^(\d{2})(\d{4})(\d{0,4})$/, '($1) $2-$3');
    } else {
        // Celular: (00) 00000-0000
        value = value.replace(/^(\d{2})(\d{5})(\d{0,4})$/, '($1) $2-$3');
    }

    e.target.value = value;
});



document.getElementById('dataIngresso').addEventListener('change', function () {
    const dataIngresso = new Date(this.value);
    const hoje = new Date();

    if (isNaN(dataIngresso)) {
        document.getElementById('diasTrabalhados').value = '';
        return;
    }

    let diasUteis = 0;
    let data = new Date(dataIngresso);

    while (data <= hoje) {
        const diaSemana = data.getDay();
        if (diaSemana !== 0 && diaSemana !== 6) { // 0 = Domingo, 6 = Sábado
            diasUteis++;
        }
        data.setDate(data.getDate() + 1);
    }

    document.getElementById('diasTrabalhados').value = diasUteis;
});