let traseuDeSters = null;

document.addEventListener("DOMContentLoaded", () => {
    fetchUserTrasee();
    const confirmBtn = document.getElementById("confirmDeleteBtn");
    confirmBtn.addEventListener("click", async () => {
        if (!traseuDeSters) return;
        const res = await fetch(`/api/TraseePersonalizate/${traseuDeSters}`, {
            method: 'DELETE'
        });
        const modalElement = document.getElementById('confirmDeleteModal');
        bootstrap.Modal.getInstance(modalElement).hide();
        if (res.ok) {
            afiseazaMesaj("Traseul a fost șters!", "success");
            fetchUserTrasee();
        } else {
            afiseazaMesaj("Eroare la ștergere.", "danger");
        }
        traseuDeSters = null;
    });
});

function afiseazaMesaj(text, tip = "success") {
    const mesajDiv = document.getElementById("status-message");
    mesajDiv.className = `alert alert-${tip}`;
    mesajDiv.textContent = text;
    mesajDiv.classList.remove("d-none");
    setTimeout(() => mesajDiv.classList.add("d-none"), 4000);
}

function formatDenumireTraseu(denumire) {
    if (denumire.includes("Traseu Culinar")) {
        const match = denumire.match(/Traseu Culinar - (.+?) #(\d+)/);
        if (match) {
            const oras = match[1];
            const numar = match[2];
            return `Traseul meu personalizat culinar ${oras} #${numar}`;
        }
    }

    return denumire;
}

async function fetchUserTrasee() {
    const container = document.getElementById("user-trasee-list");
    try {
        const response = await fetch("/api/TraseePersonalizate");
        if (!response.ok) throw new Error("Eroare la încărcare");
        const data = await response.json();
        if (!data.length) {
            container.innerHTML = "<p class='text-center text-muted'>Nu ai niciun traseu personalizat încă.</p>";
            return;
        }
        container.innerHTML = "<ul class='list-group shadow-sm rounded-4 p-0 overflow-hidden'></ul>";
        const ul = container.querySelector("ul");
        data.forEach(traseu => {
            const li = document.createElement("li");
            li.className = "list-group-item d-flex justify-content-between align-items-center";

            const denumireFormatata = formatDenumireTraseu(traseu.denumireTraseu);

            li.innerHTML = `
                <div class="d-flex justify-content-between align-items-center w-100 flex-wrap">
                    <div class="me-3">
                        <div class="fw-semibold">${denumireFormatata}</div>
                        <small class="text-muted">Creat la: ${new Date(traseu.dataCreare).toLocaleDateString()}</small>
                    </div>
                    <div class="d-flex gap-2 mt-2 mt-sm-0">
                        <a href="/Trasee/EditareTraseuPersonalizat?traseuId=${traseu.idTraseu}" class="btn btn-sm btn-outline-primary">
                            <i class="bi bi-eye me-1"></i> Vizualizează
                        </a>
                        <button class="btn btn-sm btn-outline-danger" onclick="arataModalStergere(${traseu.idTraseu})">
                            <i class="bi bi-trash me-1"></i> Șterge
                        </button>
                    </div>
                </div>
            `;
            ul.appendChild(li);
        });
    } catch (err) {
        container.innerHTML = "<p class='text-danger'>Eroare la încărcarea traseelor.</p>";
    }
}

function arataModalStergere(id) {
    traseuDeSters = id;
    const modal = new bootstrap.Modal(document.getElementById('confirmDeleteModal'));
    modal.show();
}