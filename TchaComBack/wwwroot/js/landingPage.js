
/*PRESTE ATENÇÃO EM CADA LINHA DE COMANDO ABAIXO PARA VOCÊ ENTENDER O QUE FIZ E O QUE ESTÁ SENDO FEITO */

//=========================================================================================================

// Adiciona um ouvinte de evento para garantir que o código só execute após o carregamento completo do DOM
document.addEventListener("DOMContentLoaded", function () {
    // Função para realizar uma rolagem suave até uma seção específica da página
    function scrollToSection(targetId) {
        // Seleciona o elemento alvo com base no ID fornecido
        const targetElement = document.querySelector(targetId);
        if (!targetElement) {
            // Se o elemento não for encontrado, exibe um erro no console
            console.error(`Elemento com ID "${targetId}" não encontrado.`);
            return;
        }

        // Calcula a posição real do elemento na página, considerando transformações dinâmicas
        const targetPosition = targetElement.getBoundingClientRect().top + window.scrollY;

        // Define a posição inicial da rolagem
        let start = window.scrollY;
        // Calcula a distância que precisa ser percorrida durante a animação
        const distance = targetPosition - start;
        // Define a duração da animação de rolagem (em milissegundos)
        const duration = 10;
        // Captura o tempo de início da animação para calcular o progresso
        const startTime = performance.now();

        // Função recursiva que atualiza a posição da rolagem a cada quadro de animação
        function scrollStep(currentTime) {
            // Calcula o tempo decorrido desde o início da animação
            const elapsed = currentTime - startTime;
            // Calcula o progresso da animação (valor entre 0 e 1)
            const progress = Math.min(elapsed / duration, 1);
            // Aplica uma função de easing para suavizar a transição
            const ease = easeInOutCubic(progress);
            // Atualiza a posição de rolagem da página
            window.scrollTo(0, start + distance * ease);

            // Se a animação ainda não terminou, continua chamando a função recursivamente
            if (progress < 1) {
                requestAnimationFrame(scrollStep);
            }
        }

        // Inicia a animação de rolagem suave
        requestAnimationFrame(scrollStep);
    }

    // Função de easing para criar uma transição suave (cúbica)
    function easeInOutCubic(t) {
        // Retorna um valor ajustado para criar uma aceleração e desaceleração suaves
        return t < 0.5 ? 4 * t * t * t : 1 - Math.pow(-2 * t + 2, 3) / 2;
    }

    // Função para rolar diretamente para o topo da página
    function scrollToTop() {
        // Usa o método scrollTo com comportamento suave para rolar até o topo
        window.scrollTo({
            top: 0, // Define a posição para o topo da página
            behavior: 'smooth' // Habilita a rolagem suave
        });
    }

    // Função para configurar os links de navegação
    const setupNavigation = (linkId, targetId) => {
        // Seleciona o elemento de link com base no ID fornecido
        const linkElement = document.getElementById(linkId);
        if (linkElement) {
            // Adiciona um ouvinte de evento de clique ao link
            linkElement.addEventListener("click", (event) => {
                event.preventDefault(); // Impede o comportamento padrão do link (navegação direta)

                if (targetId === "#home-sessao") {
                    // Se o link for para a seção "Home", rola diretamente para o topo
                    scrollToTop();
                } else {
                    // Para outros links, usa a função scrollToSection para rolar até a seção correspondente
                    scrollToSection(targetId);
                }
            });
        } else {
            // Se o elemento de navegação não for encontrado, exibe um aviso no console
            console.warn(`Elemento de navegação com ID "${linkId}" não encontrado.`);
        }
    };

    // Configura os links de navegação e suas seções correspondentes
    setupNavigation("home-navegacao", "#home-sessao"); // Link "Home" -> Seção "Home"
    setupNavigation("servico-navegacao", "#servico-sessao"); // Link "Serviços" -> Seção "Serviços"
    setupNavigation("sobrenos-navegacao", "#sobrenos-sessao"); // Link "Sobre Nós" -> Seção "Sobre Nós"
    setupNavigation("contatos-navegacao", "#contato-sessao"); // Link "Contatos" -> Seção "Contatos"
    setupNavigation("desenvolvimento-navegacao", "#desenvolvimento-sessao"); // Link "Desenvolvimento" -> Seção "Desenvolvimento"
    setupNavigation("gestaodados-navegacao", "#gestaodados-sessao"); // Link "Gestao Dados" -> Seção "Gestao Dados"
    setupNavigation("relatorios-navegacao", "#relatorios-sessao"); // Link "Relatorios" -> Seção "Relatorios"
});



document.addEventListener("DOMContentLoaded", function () {
    const alturaJanela = window.innerHeight;
    const rodape = document.querySelector('footer');
    const conteudo = document.querySelector('.conteúdo');
    const animacaoRolagem = document.getElementById('rolagem-animação');
    const principalAnimacaoRolagem = document.getElementById('principal-rolagem-animação');
    const cabecalho = document.querySelector('header');
    const invólucroParalaxe = document.querySelector('.invólucro-paralaxe');
    const cabecalhonavegacao = document.getElementById('cabecalho-box-one');

    if (!rodape || !conteudo || !animacaoRolagem || !principalAnimacaoRolagem || !cabecalho || !invólucroParalaxe) {
        console.error("Elemento necessário não encontrado no DOM.");
        return;
    }

    
    function calcularDimensoes() {
        const alturaRodape = rodape.offsetHeight;
        const alturaConteudo = conteudo.offsetHeight;
        const alturaDocumento = alturaJanela + alturaConteudo + alturaRodape - 20;

       
        animacaoRolagem.style.height = `${alturaDocumento}px`;
        principalAnimacaoRolagem.style.height = `${alturaDocumento}px`;
        cabecalho.style.height = `${alturaJanela}px`;
        invólucroParalaxe.style.marginTop = `${alturaJanela}px`;

     
        rolagemRodape(window.scrollY, alturaRodape);
    }

    function rolagemRodape(posicaoRolagem, alturaRodape) {
        if (posicaoRolagem >= alturaRodape) {
            rodape.style.bottom = '0px';
        } else {
            rodape.style.bottom = `-${alturaRodape * 2}px`;
        }

        if (posicaoRolagem >= conteudo.offsetTop) {
            cabecalhonavegacao.classList.add('scroll-active');
        } else {
            cabecalhonavegacao.classList.remove('scroll-active');
        }
    }

    function aplicarParallax() {
        const posicaoRolagem = window.scrollY;
        principalAnimacaoRolagem.style.top = `-${posicaoRolagem}px`;

        const posicaoFundo = Math.max(0, 50 - (posicaoRolagem * 100 / principalAnimacaoRolagem.offsetHeight));
        cabecalho.style.backgroundPositionY = `${posicaoFundo}%`;

        rolagemRodape(posicaoRolagem, rodape.offsetHeight);
    }

    calcularDimensoes();

    window.addEventListener('resize', calcularDimensoes);
    window.addEventListener('scroll', aplicarParallax);

    function estaElementoNaTela(el) {
        const retangulo = el.getBoundingClientRect();
        return (
            retangulo.top >= 0 &&
            retangulo.left >= 0 &&
            retangulo.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
            retangulo.right <= (window.innerWidth || document.documentElement.clientWidth)
        );
    }

    function aplicarAnimacoesScroll() {
        const elementos = document.querySelectorAll('.animar-com-scroll');
        elementos.forEach((elemento) => {
            if (estaElementoNaTela(elemento)) {
                elemento.classList.add('active');
            }
        });
    }

    // Aplica animações iniciais
    aplicarAnimacoesScroll();
    window.addEventListener('scroll', aplicarAnimacoesScroll);
 


});




