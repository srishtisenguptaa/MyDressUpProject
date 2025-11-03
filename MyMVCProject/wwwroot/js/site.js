document.addEventListener("DOMContentLoaded", function () {
    const splash = document.getElementById("splash-screen");
    if (!splash) return;

    const hasVisited = sessionStorage.getItem("visited");

    if (!hasVisited) {
        setTimeout(() => {
            splash.classList.add("hidden");
        }, 300000); // stays for 4 seconds now
        sessionStorage.setItem("visited", "true");
    } else {
        splash.classList.add("hidden");
    }
});
