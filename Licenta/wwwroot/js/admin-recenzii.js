let idRecenzieDeSters = null;
let modal = null;
let confirmBtn = null;

document.addEventListener("DOMContentLoaded", () => {
    modal = new bootstrap.Modal(document.getElementById('confirmDeleteModal'));
    confirmBtn = document.getElementById('confirmDeleteBtn');

    confirmBtn.addEventListener("click", async () => {
        if (!idRecenzieDeSters) return;

        const res = await fetch(`/api/recenzii/${idRecenzieDeSters}`, { method: "DELETE" });
        if (res.ok) {
            modal.hide();

            const alertBox = document.getElementById("success-alert");
            alertBox.classList.remove("d-none");

            setTimeout(() => {
                alertBox.classList.add("d-none");
            }, 4000);

            fetchAllReviewsForAdmin();
        } else {
            alert("Eroare la ștergere.");
        }
    });


    fetchAllReviewsForAdmin();
});

async function fetchAllReviewsForAdmin() {
    const container = document.getElementById("admin-reviews");
    container.innerHTML = "<p class='text-muted'>Se încarcă recenziile...</p>";

    try {
        const response = await fetch("/api/recenzii/admin/all");
        const data = await response.json();

        if (!data.length) {
            container.innerHTML = `
                <div class="alert alert-warning text-center shadow-sm p-3 rounded-3 mt-4">
                    Nu există recenzii adăugate de utilizatori.
                </div>`;
            return;
        }

        container.innerHTML = `
            <div class="table-responsive">
                <table class="table align-middle">
                    <thead class="table-light">
                        <tr>
                            <th>Locație</th>
                            <th>Utilizator</th>
                            <th>Email</th>
                            <th>Rating</th>
                            <th>Comentariu</th>
                            <th>Data</th>
                            <th>Acțiune</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${data.map(r => `
                            <tr data-id="${r.idRecenzie}">
                                <td>${r.locatie}</td>
                                <td>${r.utilizator}</td>
                                <td>${r.email}</td>
                                <td>${r.rating}/5</td>
                                <td>${r.comentariu.length > 120 ? r.comentariu.substring(0, 120) + "..." : r.comentariu}</td>
                                <td>${new Date(r.dataRecenzie).toLocaleDateString()}</td>
                                <td>
                                    <button class="btn btn-sm btn-outline-danger open-delete-modal" data-id="${r.idRecenzie}">
                                        <i class="bi bi-trash me-1"></i> Șterge
                                    </button>
                                </td>
                            </tr>
                        `).join('')}
                    </tbody>
                </table>
            </div>
        `;

        document.querySelectorAll('.open-delete-modal').forEach(btn => {
            btn.addEventListener('click', () => {
                idRecenzieDeSters = btn.getAttribute('data-id');
                modal.show();
            });
        });

    } catch (err) {
        container.innerHTML = `
            <div class="alert alert-danger shadow-sm p-3 rounded-3 mt-4">
                A apărut o eroare la încărcarea recenziilor.
            </div>`;
    }
}
