const State = {
    role: "User",
    unliquids: [],
    services: []
};

document.addEventListener("DOMContentLoaded", () => {
    initNavigation();
    initAuth();
    loadServices();
    loadUnliquids();
    initModalEvents();
});

function initNavigation() {
    document.querySelectorAll(".nav-link").forEach(link => {
        link.addEventListener("click", (e) => {
            e.preventDefault();
            document.querySelectorAll(".nav-link").forEach(l => l.classList.remove("active"));
            document.querySelectorAll(".page-section").forEach(s => s.classList.add("hidden"));
            link.classList.add("active");
            document.getElementById(link.getAttribute("data-target")).classList.remove("hidden");
        });
    });
}

function initAuth() {
    const btn = document.getElementById("authBtn");
    const addProductBtn = document.getElementById("addAdminProductBtn");
    const addServiceBtn = document.getElementById("addServiceBtn");

    btn.addEventListener("click", () => {
        State.role = State.role === "User" ? "Admin" : "User";
        btn.innerText = State.role === "Admin" ? "Выйти (Админ)" : "Войти как Админ";
        btn.style.backgroundColor = State.role === "Admin" ? "#c0392b" : "#e67e22";

        addProductBtn.classList.toggle("hidden", State.role !== "Admin");
        addServiceBtn.classList.toggle("hidden", State.role !== "Admin");

        renderServiceCards();
        renderUnliquidCards();
    });
}

async function loadServices() {
    try {
        const r = await fetch("/api/services");
        State.services = await r.json();
        renderServiceCards();
    } catch {
        document.getElementById("services-grid").innerHTML = "Ошибка связи с сервером.";
    }
}

function renderServiceCards() {
    const grid = document.getElementById("services-grid");
    grid.innerHTML = State.services.map(s => `
        <div class="service-card">
            <div>
                <img src="${s.imageUrl}" alt="Фото" style="width:100%; height:150px; object-fit:cover; border-radius:4px; margin-bottom:10px;">
                <h3>${s.title}</h3>
                <p>${s.description}</p>
                <div class="price">от ${s.priceFrom} BYN</div>
            </div>
            <div style="display:flex; gap:10px; margin-top:15px;">
                <button class="btn-action call" style="padding:5px 10px; font-size:13px; cursor:pointer;" onclick="viewService(${s.id})">Открыть</button>
                ${State.role === "Admin" ? `
                <button class="btn-add" style="background:#2980b9; padding:5px 10px; font-size:13px;" onclick="openServiceForm(${s.id})">Ред.</button>
                <button class="btn-auth" style="background:#c0392b; padding:5px 10px; font-size:13px;" onclick="deleteService(${s.id})">Х</button>` : ''}
            </div>
        </div>
    `).join('');
}

function viewService(id) {
    const s = State.services.find(x => x.id === id);
    hideAllModalForms();
    document.getElementById("modalViewBody").classList.remove("hidden");

    document.getElementById("modalImage").src = s.imageUrl;
    document.getElementById("modalTitle").innerText = s.title;
    document.getElementById("modalDescription").innerText = s.description;
    document.getElementById("modalPrice").innerText = `Цена: от ${s.priceFrom} BYN`;
    document.getElementById("modalQty").innerText = `Услуга`;

    document.getElementById("contactEmail").href = `mailto:sales@lighttorgtrans.by?subject=Заказ услуги: ${encodeURIComponent(s.title)}`;
    document.getElementById("productModal").classList.remove("hidden");
}

function openServiceForm(id = null) {
    hideAllModalForms();
    document.getElementById("serviceForm").classList.remove("hidden");
    const form = document.getElementById("serviceForm");
    form.reset();

    if (id) {
        const s = State.services.find(x => x.id === id);
        document.getElementById("editServiceId").value = s.id;
        document.getElementById("editServiceTitle").value = s.title;
        document.getElementById("editServiceDescription").value = s.description;
        document.getElementById("editServicePrice").value = s.priceFrom;
        document.getElementById("serviceFormTitle").innerText = "Редактировать услугу";
    } else {
        document.getElementById("editServiceId").value = "";
        document.getElementById("serviceFormTitle").innerText = "Добавить услугу";
    }
    document.getElementById("productModal").classList.remove("hidden");
}

async function deleteService(id) {
    if (confirm("Удалить услугу из прейскуранта?")) {
        await fetch(`/api/services/${id}`, { method: 'DELETE' });
        await loadServices();
    }
}

async function loadUnliquids() {
    try {
        const r = await fetch("/api/unliquid");
        State.unliquids = await r.json();
        renderUnliquidCards();
    } catch { console.error("Ошибка синхронизации."); }
}

function renderUnliquidCards() {
    const grid = document.getElementById("unliquid-grid");
    grid.innerHTML = State.unliquids.map(p => `
        <div class="service-card">
            <div>
                <img src="${p.imageUrl}" alt="Фото" style="width:100%; height:150px; object-fit:cover; border-radius:4px; margin-bottom:10px;">
                <h3>${p.name}</h3>
                <p>${p.description.substring(0, 50)}...</p>
                <div class="price">${p.price} BYN</div>
            </div>
            <div style="display:flex; gap:10px; margin-top:15px;">
                <button class="btn-action call" style="padding:5px 10px; font-size:13px; cursor:pointer;" onclick="viewProduct(${p.id})">Открыть</button>
                ${State.role === "Admin" ? `
                <button class="btn-add" style="background:#2980b9; padding:5px 10px; font-size:13px;" onclick="openUnliquidForm(${p.id})">Ред.</button>
                <button class="btn-auth" style="background:#c0392b; padding:5px 10px; font-size:13px;" onclick="deleteProduct(${p.id})">Х</button>` : ''}
            </div>
        </div>
    `).join('');
}

function viewProduct(id) {
    const p = State.unliquids.find(x => x.id === id);
    hideAllModalForms();
    document.getElementById("modalViewBody").classList.remove("hidden");

    document.getElementById("modalImage").src = p.imageUrl;
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
    const form = document.getElementById("modalForm");
    form.reset();

    if (id) {
        const p = State.unliquids.find(x => x.id === id);
        document.getElementById("formProductId").value = p.id;
        document.getElementById("editName").value = p.name;
        document.getElementById("editDescription").value = p.description;
        document.getElementById("editPrice").value = p.price;
        document.getElementById("editQty").value = p.quantity;
        document.getElementById("formTitle").innerText = "Редактировать товар";
    } else {
        document.getElementById("formProductId").value = "";
        document.getElementById("formTitle").innerText = "Добавить неликвид";
    }
    document.getElementById("productModal").classList.remove("hidden");
}

async function deleteProduct(id) {
    if (confirm("Удалить позицию?")) {
        await fetch(`/api/unliquid/${id}`, { method: 'DELETE' });
        await loadUnliquids();
    }
}

function hideAllModalForms() {
    document.getElementById("modalViewBody").classList.add("hidden");
    document.getElementById("modalForm").classList.add("hidden");
    document.getElementById("serviceForm").classList.add("hidden");
}

function initModalEvents() {
    const m = document.getElementById("productModal");
    document.querySelector(".close-modal").addEventListener("click", () => m.classList.add("hidden"));

    document.getElementById("modalForm").addEventListener("submit", async (e) => {
        e.preventDefault();
        const id = document.getElementById("formProductId").value;

        const formData = new FormData();
        formData.append("name", document.getElementById("editName").value);
        formData.append("description", document.getElementById("editDescription").value);
        formData.append("price", parseFloat(document.getElementById("editPrice").value));
        formData.append("quantity", document.getElementById("editQty").value);

        const fileInput = document.getElementById("editProductImage");
        if (fileInput.files.length > 0) {
            formData.append("imageFile", fileInput.files[0]);
        }

        await fetch(id ? `/api/unliquid/${id}` : '/api/unliquid', {
            method: id ? 'PUT' : 'POST',
            body: formData
        });

        m.classList.add("hidden");
        await loadUnliquids();
    });

    document.getElementById("serviceForm").addEventListener("submit", async (e) => {
        e.preventDefault();
        const id = document.getElementById("editServiceId").value;

        const formData = new FormData();
        formData.append("title", document.getElementById("editServiceTitle").value);
        formData.append("description", document.getElementById("editServiceDescription").value);
        formData.append("priceFrom", parseFloat(document.getElementById("editServicePrice").value));

        const fileInput = document.getElementById("editServiceImage");
        if (fileInput.files.length > 0) {
            formData.append("imageFile", fileInput.files[0]);
        }

        await fetch(id ? `/api/services/${id}` : '/api/services', {
            method: id ? 'PUT' : 'POST',
            body: formData
        });

        m.classList.add("hidden");
        await loadServices();
    });
}