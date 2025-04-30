document.addEventListener("DOMContentLoaded", function () {
    const conteudo = document.getElementById('conteudo-funcionarios');
    const filtro = document.getElementById('id-filtro-func');
    const toggleButton = document.getElementById('toggle-filtro-func');
    const form = document.getElementById('id-filtro-func');

    function rolagemAoRodape(posicaoRolagem) {
        if (!conteudo || !filtro) {
            console.error("Elemento '#conteudo-funcionarios' ou '#id-filtro' não encontrado!");
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



});