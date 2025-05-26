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
async function exportarFichaFuncionario(id) {
    const ficha = document.querySelector(`#ficha-${id}`);
    if (!ficha) {
        return;

    } else {
        const canvas = await html2canvas(ficha, { scale: 2 });
        const imgData = canvas.toDataURL('image/png');
        const pdf = new jspdf.jsPDF('p', 'mm', 'a4');
        const imgProps = pdf.getImageProperties(imgData);
        const pdfWidth = pdf.internal.pageSize.getWidth();
        const pdfHeight = (imgProps.height * pdfWidth) / imgProps.width;

        const title = "Ficha Cadastral do(a) Funcionário(a)";
        pdf.setFontSize(16);
        pdf.text(title, pdfWidth / 2, 15, { align: "center" });

        pdf.addImage(imgData, 'PNG', 0, 20, pdfWidth, pdfHeight);

        pdf.save("FichaFuncionario.pdf");
    }
}

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

            if (activePopover && activeTrigger === popoverTriggerEl) {
                activePopover.hide();
                activePopover = null;
                activeTrigger = null;
            } else {

                if (activePopover) {
                    activePopover.hide();
                }

                popover.show();
                activePopover = popover;
                activeTrigger = popoverTriggerEl;
            }
        });
    });

    document.body.addEventListener('click', function (event) {
        if (event.target.closest('.close-btn')) {
            if (activePopover) {
                activePopover.hide();
                activePopover = null;
                activeTrigger = null;
            }
        }
    });

    const inputTelefone = document.getElementById('telefone');
    if (inputTelefone) {
        inputTelefone.addEventListener('input', function (e) {
            let value = e.target.value.replace(/\D/g, '');

            if (value.length > 11) value = value.slice(0, 11);

            if (value.length <= 10) {
                value = value.replace(/^(\d{2})(\d{4})(\d{0,4})$/, '($1) $2-$3');
            } else {
                value = value.replace(/^(\d{2})(\d{5})(\d{0,4})$/, '($1) $2-$3');
            }

            e.target.value = value;
        });
    }



    document.querySelectorAll('.dataIngresso').forEach(function (input) {
        input.addEventListener('change', function () {
            const grupo = this.closest('.grupo-ingresso');
            const dataIngresso = new Date(this.value);
            const hoje = new Date();

            const diasTrabalhadosInput = grupo.parentElement.querySelector('.diasTrabalhados');

            if (isNaN(dataIngresso)) {
                diasTrabalhadosInput.value = '';
                return;
            }

            let diasUteis = 0;
            let data = new Date(dataIngresso);

            while (data <= hoje) {
                const diaSemana = data.getDay();
                if (diaSemana !== 0 && diaSemana !== 6) {
                    diasUteis++;
                }
                data.setDate(data.getDate() + 1);
            }

            document.getElementById('diasTrabalhados').value = diasUteis;
        });

        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
        tooltipTriggerList.forEach(function (tooltipTriggerEl) {
            new bootstrap.Tooltip(tooltipTriggerEl)
        })


    });
    document.querySelectorAll("[data-ficha-id]").forEach(botao => {
        botao.addEventListener("click", function () {
            const id = this.dataset.fichaId;
            exportarFichaFuncionario(id);
        });
    });


});

document.addEventListener('DOMContentLoaded', function () {
    const dataNascimento = document.getElementById('dataNascimento');
    const cargoInputGroup = document.getElementById('inputCargoGroup');
    const cargoInput = document.getElementById('cargoInput');
    const cargoSelectGroup = document.getElementById('selectCargoGroup');
    const cargoSelect = document.getElementById('cargoSelect');
    const cargoFinal = document.getElementById('cargoFinal');

    dataNascimento.addEventListener('change', function () {
        const nascimento = new Date(dataNascimento.value);
        const hoje = new Date();
        let idade = hoje.getFullYear() - nascimento.getFullYear();
        const m = hoje.getMonth() - nascimento.getMonth();
        if (m < 0 || (m === 0 && hoje.getDate() < nascimento.getDate())) {
            idade--;
        }

        if (isNaN(idade)) return;

        if (idade < 14) {
            alert("A idade mínima para cadastro é 14 anos.");
            dataNascimento.value = "";
            cargoInput.value = "";
            cargoSelect.value = "";
            cargoInputGroup.classList.add("d-none");
            cargoSelectGroup.classList.add("d-none");
            cargoFinal.value = "";
            return;
        }

        if (idade >= 14 && idade <= 17) {
            cargoInputGroup.classList.add("d-none");
            cargoSelectGroup.classList.remove("d-none");
            cargoInput.value = "";
        } else {
            cargoSelectGroup.classList.add("d-none");
            cargoInputGroup.classList.remove("d-none");
            cargoSelect.value = "";
        }
    });

    document.getElementById("formCompleto").addEventListener("submit", function (event) {
        event.preventDefault();

        const matriculaInput = document.getElementById("matriculaHidden");
        const cargoInputGroup = document.getElementById("inputCargoGroup");
        const cargoInput = document.getElementById("cargoInput");
        const cargoSelectGroup = document.getElementById("selectCargoGroup");
        const cargoSelect = document.getElementById("cargoSelect");
        const cargoFinal = document.getElementById("cargoFinal");

        if (!cargoInputGroup.classList.contains("d-none")) {
            if (!cargoInput.value.trim()) {
                alert("Por favor, informe o cargo.");
                return;
            }

            cargoFinal.value = cargoInput.value;
        } else if (!cargoSelectGroup.classList.contains("d-none")) {
            if (!cargoSelect.value.trim()) {
                alert("Por favor, selecione um cargo.");
                return;
            }

            cargoFinal.value = cargoSelect.value;
        }

        if (!matriculaInput.value) {
            const numeroAleatorio = Math.floor(100000 + Math.random() * 900000);
            matriculaInput.value = numeroAleatorio;
        }

        this.submit(); 
    });

});

document.addEventListener('DOMContentLoaded', function () {
    const dataIngresso = document.getElementById('dataIngresso');

    dataIngresso.addEventListener('change', function () {
        const hoje = new Date();
        const ingresso = new Date(dataIngresso.value);

        hoje.setHours(0, 0, 0, 0);
        ingresso.setHours(0, 0, 0, 0);

        const diffDias = Math.ceil((ingresso - hoje) / (1000 * 60 * 60 * 24));

        if (diffDias > 14) {
            alert("A data de ingresso deve ser até no máximo duas semanas após a data atual.");
            dataIngresso.value = "";
        }
    });
});