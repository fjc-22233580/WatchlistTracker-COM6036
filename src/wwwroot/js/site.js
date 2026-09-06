// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Allow TempData messages to fade out.
document.querySelectorAll(".auto-dismiss-alert").forEach(alert => {
    setTimeout(() => {
        alert.classList.remove("show");

        setTimeout(() => {
            alert.remove();
        }, 500);
    }, 3000);
});