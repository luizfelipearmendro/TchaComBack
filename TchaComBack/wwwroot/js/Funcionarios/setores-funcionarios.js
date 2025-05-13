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
});


document.addEventListener('DOMContentLoaded', function () {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    tooltipTriggerList.forEach(function (tooltipTriggerEl) {
        new bootstrap.Tooltip(tooltipTriggerEl)
    })
});


async function exportarFichaFuncionario(id) {
    const ficha = document.querySelector(`#ficha-${id}`);
    if (!ficha) return;

    const canvas = await html2canvas(ficha, { scale: 2 });
    const imgData = canvas.toDataURL('image/png');
    const pdf = new jspdf.jsPDF('p', 'mm', 'a4');
    const imgProps = pdf.getImageProperties(imgData);
    const pdfWidth = pdf.internal.pageSize.getWidth();
    const pdfHeight = (imgProps.height * pdfWidth) / imgProps.width;

    // Adiciona título centralizado no topo
    const title = "Ficha Cadastral do(a) Funcionário(a)";
    pdf.setFontSize(16);
    pdf.text(title, pdfWidth / 2, 15, { align: "center" });

    // Adiciona imagem abaixo do título (ajustando a posição Y para não sobrepor o texto)
    pdf.addImage(imgData, 'PNG', 0, 20, pdfWidth, pdfHeight);

    pdf.save("FichaFuncionario.pdf");
}