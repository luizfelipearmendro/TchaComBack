document.addEventListener("DOMContentLoaded", function () {
    const conteudo = document.getElementById('conteudo-setores');
    const filtro = document.getElementById('id-filtro');
    const toggleButton = document.getElementById('toggle-filtro');
    const form = document.getElementById('id-filtro');

    function rolagemAoRodape(posicaoRolagem) {
        if (!conteudo || !filtro) {
            console.error("Elemento '#conteudo-setores' ou '#id-filtro' não encontrado!");
            return;
        }

        const topoConteudo = conteudo.offsetTop;
        const alturaConteudo = conteudo.offsetHeight;
        const baseConteudo = topoConteudo + alturaConteudo;
        if (posicaoRolagem >= topoConteudo && posicaoRolagem < baseConteudo) {
            filtro.classList.add('filtro-scroll-active');
        } else {
            filtro.classList.remove('filtro-scroll-active');

            if (form.classList.contains('collapsed')) {
                form.style.display = 'block'; 
                form.classList.remove('collapsed'); 
            }
        }
    }
    rolagemAoRodape(window.scrollY);

    window.addEventListener('scroll', () => {
        rolagemAoRodape(window.scrollY);
    });

    toggleButton.addEventListener('click', function () {
        form.classList.toggle('collapsed');

        if (form.classList.contains('collapsed')) {
            toggleButton.textContent = '˅'; 
        } else {
            toggleButton.textContent = '˄'; 
        }
    });

    // Variável global para manter controle do popover ativo
    let activePopover = null;

    // Inicializa popovers
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function (popoverTriggerEl) {
        const popover = new bootstrap.Popover(popoverTriggerEl, {
            customClass: 'custom-popover',
            html: true,
            sanitize: false // Permite que HTML seja renderizado (necessário para o botão fechar funcionar)
        });

        popoverTriggerEl.addEventListener('click', function (e) {
            e.preventDefault();

            // Se o botão clicado já é o ativo, fecha e remove referência
            if (activePopover === popover) {
                popover.hide();
                activePopover = null;
            } else {
                // Fecha o anterior (se houver)
                if (activePopover) {
                    activePopover.hide();
                }

                // Mostra o novo
                popover.show();
                activePopover = popover;
            }
        });
    });

    // Fecha ao clicar no "X"
    document.body.addEventListener('click', function (event) {
        if (event.target.classList.contains('close-btn')) {
            if (activePopover) {
                activePopover.hide();
                activePopover = null;
            }
        }
    });
});