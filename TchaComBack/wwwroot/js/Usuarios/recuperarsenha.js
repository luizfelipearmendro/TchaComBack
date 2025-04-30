function closeAlert(alertId) {
const alertElement = document.getElementById(alertId);
if (alertElement) {
    alertElement.style.animation = 'fadeOut 0.5s ease-in-out';
    setTimeout(() => {
        alertElement.remove();
    }, 500);
}
}