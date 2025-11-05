////const accessKey = "bRnhJBbnJV9x01BGGB1GdjaJYY6Xp-UX8sT-20rv8rE"; // <-- Replace this

////const gallery = document.getElementById("gallery");
////const loader = document.getElementById("loader");
////let page = 1;
////let isLoading = false;

////async function loadImages() {
////    if (isLoading) return;
////    isLoading = true;
////    loader.style.display = "block";

////    const queries = ["dress", "gown", "fashion", "outfit", "jacket", "clothes"];
////    const query = queries[Math.floor(Math.random() * queries.length)];

////    const apiUrl = `https://api.unsplash.com/search/photos?page=${page}&query=${query}&per_page=8&client_id=${accessKey}`;

////    try {
////        const res = await fetch(apiUrl);
////        const data = await res.json();
////        displayImages(data.results);
////        page++;
////    } catch (err) {
////        console.error("Error fetching images:", err);
////    } finally {
////        isLoading = false;
////        loader.style.display = "none";
////    }
////}

////function displayImages(images) {
////    images.forEach((img) => {
////        const card = document.createElement("div");
////        card.classList.add("image-card");

////        card.innerHTML = `
////      <img src="${img.urls.regular}" alt="${img.alt_description || 'fashion image'}">
////      <div class="image-info">
////        <h6>${img.alt_description?.split(" ")[0] || "Outfit"}</h6>
////        <p>${img.user.name || "Unknown Brand"}</p>
////        <p>⭐ ${(Math.random() * 1 + 4).toFixed(1)} | ₹${(Math.random() * 3000 + 999).toFixed(0)}</p>
////      </div>
////    `;

////        gallery.appendChild(card);
////    });
////}

////// Infinite scroll setup
////window.addEventListener("scroll", () => {
////    if (window.innerHeight + window.scrollY >= document.body.offsetHeight - 500 && !isLoading) {
////        loadImages();
////    }
////});

////loadImages();

//const gallery = document.getElementById("gallery");
//const loader = document.getElementById("loader");
//let index = 0;
//const chunkSize = 20;
//let isLoading = false;

//// 🔹 Local image array
//const images = [
//    { src: 'https://images.unsplash.com/photo-1520974735194-8d95c9d7d1b4?auto=format&fit=crop&w=800&q=80', title: 'Evening Gown', brand: 'Mango', price: '₹4,299', rating: '⭐ 4.9' },
//    { src: 'https://images.unsplash.com/photo-1512436991641-6745cdb1723f?auto=format&fit=crop&w=800&q=80', title: 'Denim Jacket', brand: 'Levis', price: '₹2,199', rating: '⭐ 4.7' },
//    { src: 'https://images.unsplash.com/photo-1490481651871-ab68de25d43d?auto=format&fit=crop&w=800&q=80', title: 'Casual Tee', brand: 'H&M', price: '₹799', rating: '⭐ 4.2' },
//    { src: 'https://images.unsplash.com/photo-1520974735194-8d95c9d7d1b4?auto=format&fit=crop&w=800&q=80', title: 'Floral Dress', brand: 'Zara', price: '₹1,499', rating: '⭐ 4.5' },
//    { src: 'https://images.unsplash.com/photo-1491553895911-0055eca6402d?auto=format&fit=crop&w=800&q=80', title: 'Blazer Look', brand: 'H&M', price: '₹1,999', rating: '⭐ 4.6' },
//    { src: 'https://images.unsplash.com/photo-1542060748-10c28b62716a?auto=format&fit=crop&w=800&q=80', title: 'Street Style', brand: 'Only', price: '₹2,899', rating: '⭐ 4.8' },
//    { src: 'https://images.unsplash.com/photo-1503341455253-b2e723bb3dbb?auto=format&fit=crop&w=800&q=80', title: 'Classic Saree', brand: 'FabIndia', price: '₹3,199', rating: '⭐ 4.9' },
//    { src: 'https://images.unsplash.com/photo-1514995669114-b7d26063b2d7?auto=format&fit=crop&w=800&q=80', title: 'Winter Coat', brand: 'Allen Solly', price: '₹3,799', rating: '⭐ 4.7' },
//    { src: 'https://images.unsplash.com/photo-1542038784456-1ea8e935640e?auto=format&fit=crop&w=800&q=80', title: 'Boho Dress', brand: 'Max', price: '₹1,899', rating: '⭐ 4.3' },
//    { src: 'https://images.unsplash.com/photo-1520975922071-cd1f0d4a3a43?auto=format&fit=crop&w=800&q=80', title: 'Summer Outfit', brand: 'Vero Moda', price: '₹1,299', rating: '⭐ 4.1' }
//];

//// 🔹 Function to display image cards
//function loadImages() {
//    if (isLoading) return;
//    isLoading = true;
//    loader.style.display = "block";

//    const nextBatch = images.slice(index, index + chunkSize);

//    nextBatch.forEach(img => {
//        const card = document.createElement("div");
//        card.classList.add("image-card");
//        card.innerHTML = `
//      <img src="${img.src}" alt="${img.title}">
//      <div class="image-info">
//        <h6>${img.title}</h6>
//        <p>${img.brand}</p>
//        <p>${img.rating} | ${img.price}</p>
//      </div>
//    `;
//        gallery.appendChild(card);
//    });

//    index += chunkSize;
//    isLoading = false;
//    loader.style.display = "none";
//}

//// 🔹 Infinite Scroll
//window.addEventListener("scroll", () => {
//    if (window.innerHeight + window.scrollY >= document.body.offsetHeight - 500 && !isLoading && index < images.length) {
//        loadImages();
//    }
//});

//loadImages();

