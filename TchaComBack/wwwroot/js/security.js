(function () {
    // Bloquear botão direito do mouse
    document.addEventListener("contextmenu", function (event) {
        event.preventDefault();
    });

    // Bloquear teclas de atalho para DevTools
    window.addEventListener("keydown", function (event) {
        if (
            event.key === "F12" ||
            (event.ctrlKey && event.shiftKey && (event.key === "I" || event.key === "J")) ||
            (event.ctrlKey && event.key === "U")
        ) {
            event.preventDefault();
            return false;
        }
    });

    // Detectar DevTools pelo tamanho da janela
    function detectDevTools() {
        if (window.outerWidth - window.innerWidth > 160 || window.outerHeight - window.innerHeight > 160) {
            document.body.innerHTML = "";
            window.close();
        }
    }
    setInterval(detectDevTools, 1000);
    window.addEventListener("resize", detectDevTools);

    // Detectar abertura do console
    (function () {
        var element = new Image();
        Object.defineProperty(element, 'id', {
            get: function () {
                window.location.href = "about:blank"; // Redireciona para página em branco
                throw new Error("DevTools não permitido");
            }
        });
        console.log("%c", element);
    })();

    // Debugger para congelar a execução do script
    (function () {
        function devtoolsDetector() {
            console.profile();
            console.profileEnd();
            if (console.clear) console.clear();
            if (console.profiles.length > 0) {
                window.location.href = "about:blank"; // Fecha a página
            }
        }
        setInterval(devtoolsDetector, 1000);
    })();

    // Delay para carregar os bloqueios (evita manipulação rápida)
    setTimeout(function () {
        console.log = function () { };
        console.warn = function () { };
        console.error = function () { };
    }, 3000);
})();
