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

//document.addEventListener('DOMContentLoaded', function () {
//    const searchInput = document.getElementById('searchInput');
//    const searchForm = document.getElementById('searchForm');

//    if (searchForm && searchInput) {

//        // Handle Enter key search (form submit)
//        searchForm.addEventListener('submit', (e) => {
//            e.preventDefault(); // prevent full form reload

//            const query = searchInput.value.trim().toLowerCase();

//            if (query) {
//                // Save the query to localStorage so Index page can read it
//                localStorage.setItem('dressup-search', query);

//                // Redirect to Index (or wherever results are displayed)
//                window.location.href = '/Home/Index';
//            }
//        });

//        // Optional: add "Enter to search" hint on focus
//        searchInput.addEventListener('focus', () => {
//            searchInput.setAttribute('placeholder', 'Type and press Enter to search...');
//        });

//        searchInput.addEventListener('blur', () => {
//            searchInput.setAttribute('placeholder', 'Search clothes (e.g. women dress, men jacket)');
//        });
//    }

//    // Optional: theme toggle or any other global script logic can stay here
//});


