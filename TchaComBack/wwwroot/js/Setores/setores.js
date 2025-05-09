document.addEventListener("DOMContentLoaded", function () {
    const conteudo = document.getElementById('conteudo-setores');
    const filtro = document.getElementById('id-filtro');
    const toggleButton = document.getElementById('toggle-filtro');
    const form = document.getElementById('id-filtro');

    //function rolagemAoRodape(posicaoRolagem) {
    //    if (!conteudo || !filtro) {
    //        console.error("Elemento '#conteudo-setores' ou '#id-filtro' não encontrado!");
    //        return;
    //    }

    //    const topoConteudo = conteudo.offsetTop;
    //    const alturaConteudo = conteudo.offsetHeight;
    //    const baseConteudo = topoConteudo + alturaConteudo;
    //    if (posicaoRolagem >= topoConteudo && posicaoRolagem < baseConteudo) {
    //        filtro.classList.add('filtro-scroll-active');
    //    } else {
    //        filtro.classList.remove('filtro-scroll-active');

    //        if (form.classList.contains('collapsed')) {
    //            form.style.display = 'block';
    //            form.classList.remove('collapsed');
    //        }
    //    }
    //}
    //rolagemAoRodape(window.scrollY);

    //window.addEventListener('scroll', () => {
    //    rolagemAoRodape(window.scrollY);
    //});

    //toggleButton.addEventListener('click', function () {
    //    form.classList.toggle('collapsed');

    //    if (form.classList.contains('collapsed')) {
    //        toggleButton.textContent = '˅';
    //    } else {
    //        toggleButton.textContent = '˄';
    //    }
    //});
   


    let activePopover = null;
    let activeTrigger = null;

    document.querySelectorAll('[data-bs-toggle="popover"]').forEach(popoverTriggerEl => {
        const popover = new bootstrap.Popover(popoverTriggerEl, {
            customClass: 'custom-popover',
            html: true,
            sanitize: false,
            trigger: 'manual'
        });

        popoverTriggerEl.addEventListener('click', function (e) {
            e.preventDefault();

            // Se clicar no mesmo botão, fecha
            if (activePopover && activeTrigger === popoverTriggerEl) {
                activePopover.hide();
                activePopover = null;
                activeTrigger = null;
            } else {
                // Fecha anterior
                if (activePopover) {
                    activePopover.hide();
                }

                popover.show();
                activePopover = popover;
                activeTrigger = popoverTriggerEl;
            }
        });
    });

    // Fecha ao clicar no "X" dentro do popover
    document.body.addEventListener('click', function (event) {
        if (event.target.closest('.close-btn')) {
            if (activePopover) {
                activePopover.hide();
                activePopover = null;
                activeTrigger = null;
            }
        }
    });

});
function closeAlert(alertId) {
    const alertElement = document.getElementById(alertId);
    if (alertElement) {
        alertElement.style.animation = 'fadeOut 0.5s ease-in-out';
        setTimeout(() => {
            alertElement.style.display = 'none';
        }, 500);
    }
}