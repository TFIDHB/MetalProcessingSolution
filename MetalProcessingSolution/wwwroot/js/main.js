const State = {
    user: null,
    unliquids: [],
    services: [],
    currentImageIndex: 0,
    currentImagesArray: []
};
let imagesToDelete = [];

document.addEventListener("DOMContentLoaded", async () => {
    initNavigation();
    initModalEvents();
    await checkAuth();
    loadServices();
    loadUnliquids();
});


function initNavigation() {
    document.querySelectorAll(".nav-link").forEach(link => {
        link.addEventListener("click", (e) => {
            e.preventDefault();
            navigateTo(link.getAttribute("data-target"));
        });
    });
}

function navigateTo(pageId) {
    document.querySelectorAll(".nav-link").forEach(l => l.classList.remove("active"));
    document.querySelectorAll(".page-section").forEach(s => s.classList.add("hidden"));

    const targetLink = document.querySelector(`.nav-link[data-target="${pageId}"]`);
    if (targetLink) targetLink.classList.add("active");
    document.getElementById(pageId).classList.remove("hidden");
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

    renderServiceCards();
    renderUnliquidCards();
}

function onAuthClear() {
    State.user = null;
    const btn = document.getElementById("authBtn");
    btn.innerText = "Войти";
    btn.style.backgroundColor = "";
    btn.onclick = openAuthModal;

    document.getElementById("cabinetNavLink").classList.add("hidden");

    renderServiceCards();
    renderUnliquidCards();
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
                <p>${s.description.substring(0, 60)}...</p>
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


async function loadUnliquids() {
    const r = await fetch("/api/unliquid");
    State.unliquids = await r.json();
    renderUnliquidCards();
}

function renderUnliquidCards() {
    const grid = document.getElementById("unliquid-grid");
    grid.innerHTML = State.unliquids.map(p => {
        const cover = p.images.length > 0 ? p.images[0].imageUrl : "/images/no-image.png";
        return `
        <div class="service-card">
            <div>
                <img src="${cover}" alt="Фото" style="width:100%; height:150px; object-fit:cover; border-radius:4px; margin-bottom:10px;">
                <h3>${p.name}</h3>
                <p>${p.description.substring(0, 60)}...</p>
                <div class="price">${p.price} BYN</div>
            </div>
            <div style="display:flex; gap:10px; margin-top:15px;">
                <button class="btn-action call" style="padding:5px 10px; font-size:13px;" onclick="viewProduct(${p.id})">Открыть</button>
                ${isAdmin() ? `
                <button class="btn-add" style="background:#2980b9; padding:5px 10px; font-size:13px;" onclick="openUnliquidForm(${p.id})">Ред.</button>
                <button class="btn-auth" style="background:#c0392b; padding:5px 10px; font-size:13px;" onclick="deleteProduct(${p.id})">Х</button>` : ""}
            </div>
        </div>`;
    }).join("");
}

function viewProduct(id) {
    const p = State.unliquids.find(x => x.id === id);
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

function openUnliquidForm(id = null) {
    hideAllModalForms();
    document.getElementById("modalForm").classList.remove("hidden");
    document.getElementById("modalForm").reset();
    imagesToDelete = [];
    const container = document.getElementById("currentProductImages");
    container.innerHTML = "";

    if (id) {
        const p = State.unliquids.find(x => x.id === id);
        document.getElementById("formProductId").value = p.id;
        document.getElementById("editName").value = p.name;
        document.getElementById("editDescription").value = p.description;
        document.getElementById("editPrice").value = p.price;
        document.getElementById("editQty").value = p.quantity;
        document.getElementById("formTitle").innerText = "Редактировать товар";
        renderEditableImages(p.images, container);
    } else {
        document.getElementById("formProductId").value = "";
        document.getElementById("formTitle").innerText = "Добавить неликвид";
    }
    document.getElementById("productModal").classList.remove("hidden");
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
    document.getElementById("modalForm").classList.add("hidden");
    document.getElementById("serviceForm").classList.add("hidden");
}


async function deleteService(id) {
    if (confirm("Удалить?")) {
        await fetch(`/api/services/${id}`, { method: "DELETE" });
        loadServices();
    }
}

async function deleteProduct(id) {
    if (confirm("Удалить?")) {
        await fetch(`/api/unliquid/${id}`, { method: "DELETE" });
        loadUnliquids();
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

    document.getElementById("modalForm").addEventListener("submit", async (e) => {
        e.preventDefault();
        const id = document.getElementById("formProductId").value;
        const formData = new FormData();
        formData.append("name", document.getElementById("editName").value);
        formData.append("description", document.getElementById("editDescription").value);
        formData.append("price", parseFloat(document.getElementById("editPrice").value));
        formData.append("quantity", document.getElementById("editQty").value);
        const files = document.getElementById("editProductImagesInput").files;
        for (let i = 0; i < files.length; i++) formData.append("newImages", files[i]);
        imagesToDelete.forEach(imgId => formData.append("deleteImageIds", imgId));
        await fetch(id ? `/api/unliquid/${id}` : "/api/unliquid", { method: id ? "PUT" : "POST", body: formData });
        productModal.classList.add("hidden");
        loadUnliquids();
    });

    document.getElementById("authBtn").onclick = openAuthModal;

    document.getElementById("addServiceBtn").onclick = () => openServiceForm();
    document.getElementById("addAdminProductBtn").onclick = () => openUnliquidForm();
}