const State = {
    user: null,
    products: [],
    services: [],
    currentShopCategory: null,
    currentImageIndex: 0,
    currentImagesArray: []
};
let imagesToDelete = [];

document.addEventListener("DOMContentLoaded", async () => {
    initNavigation();
    initModalEvents();
    await checkAuth();
    loadServices();
});

function initNavigation() {
    document.querySelectorAll(".nav-link").forEach(link => {
        link.addEventListener("click", (e) => {
            e.preventDefault();
            closeShopDropdown();
            navigateTo(link.getAttribute("data-target"));
        });
    });

    document.addEventListener("click", (e) => {
        if (!document.getElementById("shopDropdown").contains(e.target)) {
            closeShopDropdown();
        }
    });
}

function navigateTo(pageId) {
    document.querySelectorAll(".nav-link").forEach(l => l.classList.remove("active"));
    document.querySelectorAll(".page-section").forEach(s => s.classList.add("hidden"));
    document.getElementById("shopDropdown").querySelector(".nav-dropdown__btn").classList.remove("active");

    const targetLink = document.querySelector(`.nav-link[data-target="${pageId}"]`);
    if (targetLink) targetLink.classList.add("active");
    document.getElementById(pageId).classList.remove("hidden");
}

function toggleShopDropdown() {
    document.getElementById("shopDropdownMenu").classList.toggle("hidden");
}

function closeShopDropdown() {
    document.getElementById("shopDropdownMenu").classList.add("hidden");
}

function openShopPage(category) {
    closeShopDropdown();

    const titles = {
        Unliquid: "Неликвиды",
        OurProducts: "Наша продукция",
        GeneralGoods: "Товары широкого потребления"
    };

    State.currentShopCategory = category;
    document.getElementById("shopPageTitle").innerText = titles[category] ?? "Интернет-магазин";

    document.querySelectorAll(".nav-link").forEach(l => l.classList.remove("active"));
    document.querySelectorAll(".page-section").forEach(s => s.classList.add("hidden"));
    document.getElementById("shopDropdown").querySelector(".nav-dropdown__btn").classList.add("active");
    document.getElementById("shop-page").classList.remove("hidden");

    document.getElementById("addProductBtn").classList.toggle("hidden", !isAdmin());

    loadProducts(category);
}

async function checkAuth() {
    try {
        const res = await fetch("/api/auth/me");
        if (res.ok) {
            State.user = await res.json();
            onAuthSuccess();
        } else {
            onAuthClear();
        }
    } catch {
        onAuthClear();
    }
}

function onAuthSuccess() {
    const btn = document.getElementById("authBtn");
    btn.innerText = "Выйти";
    btn.style.backgroundColor = "#c0392b";
    btn.onclick = logout;

    document.getElementById("cabinetNavLink").classList.remove("hidden");
    document.getElementById("cabinetEmail").innerText = State.user.email;
    document.getElementById("cabinetRole").innerText = State.user.role === "Admin" ? "Администратор" : "Пользователь";

    if (State.user.role === "Admin") {
        document.getElementById("adminNavLink").classList.remove("hidden");
        document.getElementById("addServiceBtn").classList.remove("hidden");
        loadStats();
    }

    renderServiceCards();
    if (State.currentShopCategory) renderProductCards();
}

function onAuthClear() {
    State.user = null;
    const btn = document.getElementById("authBtn");
    btn.innerText = "Войти";
    btn.style.backgroundColor = "";
    btn.onclick = openAuthModal;

    document.getElementById("cabinetNavLink").classList.add("hidden");
    document.getElementById("adminNavLink").classList.add("hidden");
    document.getElementById("addServiceBtn").classList.add("hidden");
    document.getElementById("addProductBtn").classList.add("hidden");

    renderServiceCards();
    if (State.currentShopCategory) renderProductCards();
}

async function logout() {
    await fetch("/api/auth/logout", { method: "POST" });
    onAuthClear();
    navigateTo("about-page");
}

function isAdmin() {
    return State.user?.role === "Admin";
}

function openAuthModal() {
    showLoginForm();
    document.getElementById("authModal").classList.remove("hidden");
}

function closeAuthModal() {
    document.getElementById("authModal").classList.add("hidden");
    clearAuthErrors();
}

function showLoginForm() {
    document.getElementById("loginForm").classList.remove("hidden");
    document.getElementById("registerForm").classList.add("hidden");
    clearAuthErrors();
}

function showRegisterForm() {
    document.getElementById("loginForm").classList.add("hidden");
    document.getElementById("registerForm").classList.remove("hidden");
    clearAuthErrors();
}

function clearAuthErrors() {
    ["loginError", "registerError", "registerSuccess"].forEach(id => {
        const el = document.getElementById(id);
        el.style.display = "none";
        el.innerText = "";
    });
}

function showError(elementId, message) {
    const el = document.getElementById(elementId);
    el.innerText = message;
    el.style.display = "block";
}

async function submitLogin() {
    const email = document.getElementById("loginEmail").value.trim();
    const password = document.getElementById("loginPassword").value;

    const res = await fetch("/api/auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password })
    });

    if (res.ok) {
        State.user = await res.json();
        closeAuthModal();
        onAuthSuccess();
    } else {
        const err = await res.json();
        showError("loginError", err.detail || "Неверный Email или пароль.");
    }
}

async function submitRegister() {
    const email = document.getElementById("registerEmail").value.trim();
    const password = document.getElementById("registerPassword").value;

    const res = await fetch("/api/auth/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password })
    });

    if (res.ok) {
        const successEl = document.getElementById("registerSuccess");
        successEl.innerText = "Аккаунт создан! Теперь войдите.";
        successEl.style.display = "block";
        setTimeout(() => showLoginForm(), 1500);
    } else {
        const err = await res.json();
        showError("registerError", err.detail || "Ошибка регистрации.");
    }
}

async function submitChangePassword(e) {
    e.preventDefault();

    const errorEl = document.getElementById("changePasswordError");
    const successEl = document.getElementById("changePasswordSuccess");
    errorEl.style.display = "none";
    successEl.style.display = "none";

    const res = await fetch("/api/auth/change-password", {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            currentPassword: document.getElementById("currentPassword").value,
            newPassword: document.getElementById("newPassword").value
        })
    });

    if (res.ok) {
        successEl.style.display = "block";
        document.getElementById("changePasswordForm").reset();
    } else {
        const err = await res.json();
        errorEl.innerText = err.detail || "Ошибка смены пароля.";
        errorEl.style.display = "block";
    }
}

async function loadServices() {
    const r = await fetch("/api/services");
    if (!r.ok) return;
    State.services = await r.json();
    renderServiceCards();
}

function renderServiceCards() {
    const grid = document.getElementById("services-grid");
    grid.innerHTML = State.services.map(s => {
        const cover = s.images.length > 0 ? s.images[0].imageUrl : "/images/no-image.png";
        return `
        <div class="service-card">
            <div>
                <img src="${cover}" alt="Фото" style="width:100%; height:150px; object-fit:cover; border-radius:4px; margin-bottom:10px;">
                <h3>${s.title}</h3>
                <p>${(s.description ?? "").substring(0, 60)}...</p>
                <div class="price">от ${s.priceFrom} BYN</div>
            </div>
            <div style="display:flex; gap:10px; margin-top:15px;">
                <button class="btn-action call" style="padding:5px 10px; font-size:13px;" onclick="viewService(${s.id})">Открыть</button>
                ${isAdmin() ? `
                <button class="btn-add" style="background:#2980b9; padding:5px 10px; font-size:13px;" onclick="openServiceForm(${s.id})">Ред.</button>
                <button class="btn-auth" style="background:#c0392b; padding:5px 10px; font-size:13px;" onclick="deleteService(${s.id})">Х</button>` : ""}
            </div>
        </div>`;
    }).join("");
}

function viewService(id) {
    const s = State.services.find(x => x.id === id);
    hideAllModalForms();
    document.getElementById("modalViewBody").classList.remove("hidden");
    renderGallery(s.images);
    document.getElementById("modalTitle").innerText = s.title;
    document.getElementById("modalDescription").innerText = s.description;
    document.getElementById("modalPrice").innerText = `Цена: от ${s.priceFrom} BYN`;
    document.getElementById("modalQty").innerText = "Услуга";
    document.getElementById("contactEmail").href = `mailto:sales@lighttorgtrans.by?subject=Услуга: ${encodeURIComponent(s.title)}`;
    document.getElementById("productModal").classList.remove("hidden");
}

function openServiceForm(id = null) {
    hideAllModalForms();
    document.getElementById("serviceForm").classList.remove("hidden");
    document.getElementById("serviceForm").reset();
    imagesToDelete = [];
    const container = document.getElementById("currentServiceImages");
    container.innerHTML = "";

    if (id) {
        const s = State.services.find(x => x.id === id);
        document.getElementById("editServiceId").value = s.id;
        document.getElementById("editServiceTitle").value = s.title;
        document.getElementById("editServiceDescription").value = s.description;
        document.getElementById("editServicePrice").value = s.priceFrom;
        document.getElementById("serviceFormTitle").innerText = "Редактировать услугу";
        renderEditableImages(s.images, container);
    } else {
        document.getElementById("editServiceId").value = "";
        document.getElementById("serviceFormTitle").innerText = "Добавить услугу";
    }
    document.getElementById("productModal").classList.remove("hidden");
}

async function deleteService(id) {
    if (confirm("Удалить?")) {
        await fetch(`/api/services/${id}`, { method: "DELETE" });
        loadServices();
    }
}

async function loadProducts(category) {
    const r = await fetch(`/api/products/${category}`);
    if (!r.ok) return;
    State.products = await r.json();
    renderProductCards();
}

function renderProductCards() {
    const grid = document.getElementById("shop-grid");
    grid.innerHTML = State.products.map(p => {
        const cover = p.images.length > 0 ? p.images[0].imageUrl : "/images/no-image.png";
        return `
        <div class="service-card">
            <div>
                <img src="${cover}" alt="Фото" style="width:100%; height:150px; object-fit:cover; border-radius:4px; margin-bottom:10px;">
                <h3>${p.name}</h3>
                <p>${(p.description ?? "").substring(0, 60)}...</p>
                <div class="price">${p.price} BYN</div>
            </div>
            <div style="display:flex; gap:10px; margin-top:15px;">
                <button class="btn-action call" style="padding:5px 10px; font-size:13px;" onclick="viewProduct(${p.id})">Открыть</button>
                ${isAdmin() ? `
                <button class="btn-add" style="background:#2980b9; padding:5px 10px; font-size:13px;" onclick="openProductForm(${p.id})">Ред.</button>
                <button class="btn-auth" style="background:#c0392b; padding:5px 10px; font-size:13px;" onclick="deleteProduct(${p.id})">Х</button>` : ""}
            </div>
        </div>`;
    }).join("");
}

function viewProduct(id) {
    const p = State.products.find(x => x.id === id);
    hideAllModalForms();
    document.getElementById("modalViewBody").classList.remove("hidden");
    renderGallery(p.images);
    document.getElementById("modalTitle").innerText = p.name;
    document.getElementById("modalDescription").innerText = p.description;
    document.getElementById("modalPrice").innerText = `Цена: ${p.price} BYN`;
    document.getElementById("modalQty").innerText = `Остаток: ${p.quantity}`;
    document.getElementById("contactEmail").href = `mailto:sales@lighttorgtrans.by?subject=Закупка: ${encodeURIComponent(p.name)}`;
    document.getElementById("productModal").classList.remove("hidden");
}

function openProductForm(id = null) {
    hideAllModalForms();
    document.getElementById("productForm").classList.remove("hidden");
    document.getElementById("productForm").reset();
    imagesToDelete = [];
    const container = document.getElementById("currentProductImages");
    container.innerHTML = "";

    if (id) {
        const p = State.products.find(x => x.id === id);
        document.getElementById("editProductId").value = p.id;
        document.getElementById("editProductCategory").value = p.category;
        document.getElementById("editProductName").value = p.name;
        document.getElementById("editProductDescription").value = p.description;
        document.getElementById("editProductPrice").value = p.price;
        document.getElementById("editProductQty").value = p.quantity;
        document.getElementById("productFormTitle").innerText = "Редактировать товар";
        renderEditableImages(p.images, container);
    } else {
        document.getElementById("editProductId").value = "";
        document.getElementById("editProductCategory").value = State.currentShopCategory;
        document.getElementById("productFormTitle").innerText = "Добавить товар";
    }
    document.getElementById("productModal").classList.remove("hidden");
}

async function deleteProduct(id) {
    if (confirm("Удалить?")) {
        await fetch(`/api/products/${id}`, { method: "DELETE" });
        loadProducts(State.currentShopCategory);
    }
}

function renderGallery(images) {
    const container = document.getElementById("modalGallery");
    const prevBtn = document.getElementById("prevSlideBtn");
    const nextBtn = document.getElementById("nextSlideBtn");

    State.currentImagesArray = images || [];
    State.currentImageIndex = 0;

    if (State.currentImagesArray.length === 0) {
        container.innerHTML = `<img src="/images/no-image.png" class="active" style="cursor:default;">`;
        prevBtn.classList.add("hidden");
        nextBtn.classList.add("hidden");
        return;
    }

    container.innerHTML = State.currentImagesArray.map((img, i) =>
        `<img src="${img.imageUrl}" class="${i === 0 ? "active" : ""}" alt="Фото">`
    ).join("");

    const hasMany = State.currentImagesArray.length > 1;
    prevBtn.classList.toggle("hidden", !hasMany);
    nextBtn.classList.toggle("hidden", !hasMany);
}

function changeSlide(direction) {
    const imgs = document.querySelectorAll("#modalGallery img");
    if (imgs.length <= 1) return;
    imgs[State.currentImageIndex].classList.remove("active");
    State.currentImageIndex = (State.currentImageIndex + direction + imgs.length) % imgs.length;
    imgs[State.currentImageIndex].classList.add("active");
}

function renderEditableImages(images, container) {
    if (!images || images.length === 0) {
        container.innerHTML = "<em>Нет загруженных фото</em>";
        return;
    }
    container.innerHTML = images.map(img => `
        <div id="img-block-${img.id}" style="position:relative; width:80px; height:80px; display:inline-block; margin-right:8px;">
            <img src="${img.imageUrl}" style="width:100%; height:100%; object-fit:cover; border-radius:4px;">
            <button type="button" onclick="queueImageDelete(${img.id})" style="position:absolute; top:-5px; right:-5px; background:#c0392b; color:white; border:none; border-radius:50%; width:20px; height:20px; cursor:pointer;">&times;</button>
        </div>
    `).join("");
}

function queueImageDelete(id) {
    imagesToDelete.push(id);
    document.getElementById(`img-block-${id}`).remove();
}

function hideAllModalForms() {
    document.getElementById("modalViewBody").classList.add("hidden");
    document.getElementById("productForm").classList.add("hidden");
    document.getElementById("serviceForm").classList.add("hidden");
}

async function loadStats() {
    const res = await fetch("/api/admin/stats");
    if (!res.ok) return;
    const stats = await res.json();
    document.getElementById("statServices").innerText = stats.servicesCount;
    document.getElementById("statUnliquids").innerText = stats.unliquidsCount;
    document.getElementById("statOurProducts").innerText = stats.ourProductsCount;
    document.getElementById("statGeneralGoods").innerText = stats.generalGoodsCount;
}

async function adjustProductPrices(category) {
    const inputIds = {
        Unliquid: "unliquidPricePercent",
        OurProducts: "ourProductsPricePercent",
        GeneralGoods: "generalGoodsPricePercent"
    };
    const msgIds = {
        Unliquid: "unliquidPriceMsg",
        OurProducts: "ourProductsPriceMsg",
        GeneralGoods: "generalGoodsPriceMsg"
    };
    const labels = {
        Unliquid: "неликвидов",
        OurProducts: "нашей продукции",
        GeneralGoods: "товаров широкого потребления"
    };

    const msgEl = document.getElementById(msgIds[category]);
    const percent = parseFloat(document.getElementById(inputIds[category]).value);

    msgEl.className = "admin-msg hidden";
    msgEl.innerText = "";

    if (isNaN(percent) || percent === 0) {
        msgEl.className = "admin-msg error";
        msgEl.innerText = "Введите ненулевое значение.";
        return;
    }

    const confirmText = percent > 0
        ? `Повысить все цены ${labels[category]} на ${percent}%?`
        : `Снизить все цены ${labels[category]} на ${Math.abs(percent)}%?`;

    if (!confirm(confirmText)) return;

    const res = await fetch(`/api/admin/adjust-product-prices/${category}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ percent })
    });

    if (res.ok) {
        msgEl.className = "admin-msg success";
        msgEl.innerText = `Цены успешно обновлены на ${percent > 0 ? "+" : ""}${percent}%.`;
        document.getElementById(inputIds[category]).value = "";
        if (State.currentShopCategory === category) loadProducts(category);
        loadStats();
    } else {
        const err = await res.json();
        msgEl.className = "admin-msg error";
        msgEl.innerText = err.detail || "Ошибка при обновлении цен.";
    }
}

async function adjustPrices(type) {
    const msgEl = document.getElementById("servicePriceMsg");
    const percent = parseFloat(document.getElementById("servicePricePercent").value);

    msgEl.className = "admin-msg hidden";
    msgEl.innerText = "";

    if (isNaN(percent) || percent === 0) {
        msgEl.className = "admin-msg error";
        msgEl.innerText = "Введите ненулевое значение.";
        return;
    }

    if (!confirm(percent > 0 ? `Повысить все цены услуг на ${percent}%?` : `Снизить все цены услуг на ${Math.abs(percent)}%?`)) return;

    const res = await fetch("/api/admin/adjust-service-prices", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ percent })
    });

    if (res.ok) {
        msgEl.className = "admin-msg success";
        msgEl.innerText = `Цены услуг обновлены на ${percent > 0 ? "+" : ""}${percent}%.`;
        document.getElementById("servicePricePercent").value = "";
        await Promise.all([loadServices(), loadStats()]);
    } else {
        const err = await res.json();
        msgEl.className = "admin-msg error";
        msgEl.innerText = err.detail || "Ошибка при обновлении цен.";
    }
}

function initModalEvents() {
    const productModal = document.getElementById("productModal");

    document.querySelector(".close-modal").addEventListener("click", () =>
        productModal.classList.add("hidden"));

    productModal.addEventListener("click", (e) => {
        if (e.target === productModal) productModal.classList.add("hidden");
    });

    document.getElementById("prevSlideBtn").addEventListener("click", () => changeSlide(-1));
    document.getElementById("nextSlideBtn").addEventListener("click", () => changeSlide(1));
    document.getElementById("modalGallery").addEventListener("click", () => {
        if (State.currentImagesArray.length > 1) changeSlide(1);
    });

    document.getElementById("closeAuthModal").addEventListener("click", closeAuthModal);
    document.getElementById("authModal").addEventListener("click", (e) => {
        if (e.target === document.getElementById("authModal")) closeAuthModal();
    });
    document.getElementById("switchToRegister").addEventListener("click", (e) => {
        e.preventDefault();
        showRegisterForm();
    });
    document.getElementById("switchToLogin").addEventListener("click", (e) => {
        e.preventDefault();
        showLoginForm();
    });
    document.getElementById("loginSubmitBtn").addEventListener("click", submitLogin);
    document.getElementById("registerSubmitBtn").addEventListener("click", submitRegister);

    document.getElementById("changePasswordForm").addEventListener("submit", submitChangePassword);

    document.getElementById("serviceForm").addEventListener("submit", async (e) => {
        e.preventDefault();
        const id = document.getElementById("editServiceId").value;
        const formData = new FormData();
        formData.append("title", document.getElementById("editServiceTitle").value);
        formData.append("description", document.getElementById("editServiceDescription").value);
        formData.append("priceFrom", parseFloat(document.getElementById("editServicePrice").value));
        const files = document.getElementById("editServiceImagesInput").files;
        for (let i = 0; i < files.length; i++) formData.append("newImages", files[i]);
        imagesToDelete.forEach(imgId => formData.append("deleteImageIds", imgId));
        await fetch(id ? `/api/services/${id}` : "/api/services", { method: id ? "PUT" : "POST", body: formData });
        productModal.classList.add("hidden");
        loadServices();
    });

    document.getElementById("productForm").addEventListener("submit", async (e) => {
        e.preventDefault();
        const id = document.getElementById("editProductId").value;
        const category = document.getElementById("editProductCategory").value;
        const formData = new FormData();
        formData.append("name", document.getElementById("editProductName").value);
        formData.append("description", document.getElementById("editProductDescription").value);
        formData.append("price", parseFloat(document.getElementById("editProductPrice").value));
        formData.append("quantity", document.getElementById("editProductQty").value);
        formData.append("category", category);
        const files = document.getElementById("editProductImagesInput").files;
        for (let i = 0; i < files.length; i++) formData.append("newImages", files[i]);
        imagesToDelete.forEach(imgId => formData.append("deleteImageIds", imgId));
        await fetch(id ? `/api/products/${id}` : "/api/products", { method: id ? "PUT" : "POST", body: formData });
        productModal.classList.add("hidden");
        loadProducts(State.currentShopCategory);
    });

    document.getElementById("authBtn").onclick = openAuthModal;
    document.getElementById("addServiceBtn").onclick = () => openServiceForm();
    document.getElementById("addProductBtn").onclick = () => openProductForm();
}