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
})