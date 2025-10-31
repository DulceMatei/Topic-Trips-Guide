// Elemente DOM
const mapContainer = document.getElementById('map-container');
const locationList = document.getElementById('location-list');
const statusMessage = document.getElementById('status-message');
const transportSelect = document.getElementById('transport-mode');

// Stocare globala
const markers = [];
const routes = [];
const routeCache = {};
let locations = [];
let activeListItem = null;
const routeId = document.getElementById("page-template")?.dataset.traseuId || 1;
let isAdmin = false;
const isPersonalizat = document.body.getAttribute('data-is-personalizat') === 'true';
const apiPrefix = isPersonalizat ? '/api/traseepersonalizate' : '/api/trasee';

const markerStyles = document.createElement('style');
markerStyles.innerHTML = `
.custom-marker {
    background: linear-gradient(135deg, #FF6B35 0%, #F7931E 100%);
    border: 3px solid white;
    border-radius: 50%;
    width: 38px;
    height: 38px;
    display: flex;
    align-items: center;
    justify-content: center;
    box-shadow: 0 4px 15px rgba(0,0,0,0.4);
    position: relative;
    transition: all 0.3s ease;
}

.custom-marker::after {
    content: '';
    position: absolute;
    bottom: -10px;
    left: 50%;
    transform: translateX(-50%);
    border-left: 10px solid transparent;
    border-right: 10px solid transparent;
    border-top: 10px solid #FF6B35;
    filter: drop-shadow(0 2px 4px rgba(0,0,0,0.3));
}

.start-marker {
    background: linear-gradient(135deg, #00C851 0%, #007E33 100%) !important;
}

.start-marker::after {
    border-top-color: #00C851 !important;
}

.end-marker {
    background: linear-gradient(135deg, #FF3547 0%, #CC0000 100%) !important;
}

.end-marker::after {
    border-top-color: #FF3547 !important;
}

.marker-number {
    color: white;
    font-weight: bold;
    font-size: 15px;
    text-shadow: 2px 2px 4px rgba(0,0,0,0.7);
}

.custom-div-icon {
    background: transparent !important;
    border: none !important;
}

.custom-marker:hover {
    transform: scale(1.15);
    box-shadow: 0 6px 20px rgba(0,0,0,0.5);
}

/* Efecte vizuale suplimentare */
.route-line {
    transition: all 0.3s ease;
}

.route-line:hover {
    opacity: 1 !important;
    filter: drop-shadow(0 0 12px rgba(0,0,0,0.6)) !important;
}

/* Animație pentru markere */
@keyframes pulse {
    0% { transform: scale(1); }
    50% { transform: scale(1.05); }
    100% { transform: scale(1); }
}

.custom-marker.active {
    animation: pulse 2s infinite;
}

/* Stil pentru popup-uri */
.leaflet-popup-content-wrapper {
    border-radius: 8px;
    box-shadow: 0 4px 20px rgba(0,0,0,0.3);
}

.leaflet-popup-tip {
    box-shadow: 0 2px 10px rgba(0,0,0,0.2);
}
`;
document.head.appendChild(markerStyles);

let traseuId;

document.addEventListener('DOMContentLoaded', () => {
    isAdmin = document.body.getAttribute('data-is-admin') === 'true';
    traseuId = document.getElementById("page-template")?.dataset.traseuId;

    loadTariSiOrase().then(() => {
        actualizeazaCampuriNoi();
    });
    document.getElementById('selectTara').addEventListener('change', actualizeazaCampuriNoi);
    document.getElementById('selectOras').addEventListener('change', actualizeazaCampuriNoi);
    initializeApp();

    let locatieDeStersId = null;

    document.body.addEventListener("click", (e) => {
        if (e.target.classList.contains("btnStergeDefinitiv")) {
            locatieDeStersId = e.target.dataset.id;
            const modal = new bootstrap.Modal(document.getElementById('modalConfirmStergere'));
            modal.show();
        }
    });

    document.getElementById("btnConfirmaStergere").addEventListener("click", async () => {
        if (!locatieDeStersId) return;

        const response = await fetch(`/api/trasee/${locatieDeStersId}`, {
            method: 'DELETE'
        });

        const modal = bootstrap.Modal.getInstance(document.getElementById('modalConfirmStergere'));
        modal.hide();

        if (response.ok) {
            showStatus("Locația a fost ștearsă definitiv!", "success");
            await fetchLocations();
            renderLocationList();
            recalculateRoutes();
        } else {
            showStatus("Eroare la ștergere!", "danger");
        }

        locatieDeStersId = null;
    });
});



// Initializare harta Leaflet
const map = L.map(mapContainer);
L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '© OpenStreetMap contributors'
}).addTo(map);

// Functie pentru afisarea mesajelor de status
let statusTimeout = null;

function showStatus(message, type) {
    if (!statusMessage) return;

    if (statusTimeout) {
        clearTimeout(statusTimeout);
    }

    statusMessage.textContent = message;
    statusMessage.className = `alert alert-${type}`;
    statusMessage.classList.remove('d-none');

    statusTimeout = setTimeout(() => {
        statusMessage.classList.add('d-none');
    }, 5000);
}

function showModalStatus(message, type) {
    const modalStatus = document.getElementById('modalStatusMessage');
    if (!modalStatus) return;

    modalStatus.textContent = message;
    modalStatus.className = `alert alert-${type}`;
    modalStatus.classList.remove('d-none');

    setTimeout(() => {
        modalStatus.classList.add('d-none');
    }, 5000);
}

async function loadTariSiOrase() {
    const taraSelect = document.getElementById("selectTara");
    const orasSelect = document.getElementById("selectOras");

    const resTari = await fetch('/api/tari');
    const tari = await resTari.json();

    taraSelect.innerHTML = '<option value="">-- alege --</option>';
    tari.forEach(t => {
        const opt = document.createElement('option');
        opt.value = t.id;
        opt.textContent = t.denumire;
        taraSelect.appendChild(opt);
    });

    taraSelect.addEventListener('change', async () => {
        const taraId = taraSelect.value;
        const resOrase = await fetch(`/api/orase?taraId=${taraId}`);
        const orase = await resOrase.json();
        orasSelect.innerHTML = '<option value="">-- alege --</option>';
        orase.forEach(o => {
            const opt = document.createElement('option');
            opt.value = o.id;
            opt.textContent = o.denumire;
            orasSelect.appendChild(opt);
        });
    });
}

document.getElementById("formAdaugaLocatie").addEventListener("submit", async (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);
    const dto = Object.fromEntries(formData.entries());
    dto.traseuId = traseuId;

    for (const key in dto) {
        if (dto[key] === "") {
            dto[key] = null;
        }
    }

    const response = await fetch('/api/trasee/create-locatie', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(dto)
    });

    if (response.ok) {
        const modalEl = document.getElementById('modalAdaugaLocatie');
        const modal = bootstrap.Modal.getInstance(modalEl);
        modal.hide();
        e.target.reset();
        actualizeazaCampuriNoi();
        await fetchLocations();
        renderLocationList();
        recalculateRoutes();
        showModalStatus("Locația a fost adăugată!", "success");
    } else {
        let msg = "Eroare la adăugare locație!";
        try {
            const text = await response.text();
            try {
                const json = JSON.parse(text);
                msg = json?.title || json?.message || msg;
            } catch {
                msg = text;
            }
        } catch { }
        showModalStatus(msg, "danger");
    }

});

function actualizeazaCampuriNoi() {
    const selectTara = document.getElementById('selectTara');
    const selectOras = document.getElementById('selectOras');
    const inputTaraNoua = document.getElementById('inputTaraNoua');
    const inputOrasNou = document.getElementById('inputOrasNou');

    // Tara noua
    if (selectTara.value) {
        inputTaraNoua.disabled = true;
        inputTaraNoua.value = '';
    } else {
        inputTaraNoua.disabled = false;
    }

    // Oras nou
    if (selectOras.value) {
        inputOrasNou.disabled = true;
        inputOrasNou.value = '';
    } else {
        inputOrasNou.disabled = false;
    }
}


function getRandomColor() {
    const colors = [
        '#FF6B35', '#00C851', '#FF3547', '#007BFF', '#6F42C1',
        '#FD7E14', '#20C997', '#E83E8C', '#6610F2', '#DC3545'
    ];
    return colors[Math.floor(Math.random() * colors.length)];
}

function createNumberedIcon(number, type = 'normal') {
    let bgColor = 'linear-gradient(135deg, #FF6B35 0%, #F7931E 100%)';

    if (type === 'start') {
        bgColor = 'linear-gradient(135deg, #00C851 0%, #007E33 100%)';
    } else if (type === 'end') {
        bgColor = 'linear-gradient(135deg, #FF3547 0%, #CC0000 100%)';
    }

    return L.divIcon({
        html: `<div class="custom-marker ${type === 'start' ? 'start-marker' : type === 'end' ? 'end-marker' : ''}" style="background: ${bgColor}">
                 <div class="marker-number">${number}</div>
               </div>`,
        className: 'custom-div-icon',
        iconSize: [38, 38],
        iconAnchor: [19, 38],
        popupAnchor: [0, -38]
    });
}

function highlightLocation(index) {
    if (activeListItem) {
        activeListItem.classList.remove('active');
    }
    const listItems = locationList.querySelectorAll('li');
    if (listItems[index]) {
        listItems[index].classList.add('active');
        activeListItem = listItems[index];
    }
}
document.addEventListener('click', (event) => {
    const clickedInsideList = locationList.contains(event.target);
    if (!clickedInsideList && activeListItem) {
        activeListItem.classList.remove('active');
        activeListItem = null;
    }
});


function formatTimp(minute) {
    if (minute < 60) {
        return `${minute} ${minute === 1 ? 'minut' : 'minute'}`;
    }

    const ore = Math.floor(minute / 60);
    const min = minute % 60;

    const oreText = ore === 1 ? '1 oră' : `${ore} ore`;
    const minText = min === 0 ? '' : `${min === 1 ? '1 minut' : `${min} minute`}`;
    const text = min === 0 ? oreText : `${oreText} și ${minText}`;

    const paranteza = (ore > 0 && min > 0) ? ` (${minute} minute)` : '';

    return `${text}${paranteza}`;
}

async function getRoute(startIndex, endIndex) {
    try {
        const startLocation = locations[startIndex];
        const endLocation = locations[endIndex];

        if (!startLocation || !endLocation || !startLocation.lat || !startLocation.lon || !endLocation.lat || !endLocation.lon) {
            console.warn(`Date invalide pentru ruta ${startIndex} → ${endIndex}.`);
            return null;
        }

        showStatus('Se calculează ruta...', 'info');

        const transportMode = transportSelect.value;
        const cacheKey = `${startIndex}-${endIndex}-${transportMode}`;

        if (routeCache[cacheKey]) {
            return routeCache[cacheKey];
        }

        const routeUrl = `/api/routing/route?start=${startLocation.lon},${startLocation.lat}&end=${endLocation.lon},${endLocation.lat}&mode=${transportMode}`;
        const response = await fetch(routeUrl);

        if (!response.ok) {
            console.warn(`Eroare API ${response.status}: ${response.statusText}`);
            return null;
        }

        const data = await response.json();
        if (!data.features || !data.features[0] || !data.features[0].geometry) {
            console.warn(`OpenRouteService nu a returnat rută validă: ${startLocation.name} → ${endLocation.name}`);
            return null;
        }

        const routeCoords = data.features[0].geometry.coordinates.map(coord => [coord[1], coord[0]]);

        const routeLine = L.polyline(routeCoords, {
            color: getRandomColor(),
            weight: 6,
            opacity: 0.9,
            dashArray: '12, 8',
            lineCap: 'round',
            lineJoin: 'round',
            className: 'route-line'
        }).addTo(map);

        const routeElement = routeLine.getElement();
        if (routeElement) {
            routeElement.style.filter = 'drop-shadow(0 0 8px rgba(0,0,0,0.4))';
        }

        const properties = data.features[0].properties;
        const distance = (properties.summary.distance / 1000).toFixed(2);
        const duration = Math.round(properties.summary.duration / 60);

        const routeId = `route-${startIndex}-${endIndex}-${Date.now()}`;
        routeLine.bindPopup(`
            <div style="font-family: Arial, sans-serif; padding: 5px;">
                <b style="color: #333; font-size: 14px;">De la ${startLocation.name} la ${endLocation.name}</b><br>
                <div style="margin: 8px 0;">
                    <span style="color: #007BFF; font-weight: bold;">📍 Distanță:</span> ${distance} km<br>
                    <span style="color: #28A745; font-weight: bold;">⏱️ Durată:</span> ${formatTimp(duration)}<br>
                </div>
                <button class="btn btn-sm btn-outline-primary toggle-route" data-route-id="${routeId}" 
                        style="font-size: 12px; padding: 4px 8px;">
                    👁️ Arată/Ascunde
                </button>
            </div>
        `);

        const routeInfo = {
            id: routeId,
            line: routeLine,
            start: startIndex,
            end: endIndex,
            distance,
            duration,
            visible: true
        };

        routes.push(routeInfo);
        routeCache[cacheKey] = routeInfo;

        map.on('popupopen', () => {
            const toggleBtn = document.querySelector(`.toggle-route[data-route-id="${routeId}"]`);
            if (toggleBtn) {
                toggleBtn.addEventListener('click', () => {
                    const routeToToggle = routes.find(r => r.id === routeId);
                    if (routeToToggle) {
                        toggleRouteVisibility(routeToToggle);
                    }
                });
            }
        });

        showStatus('', '');
        return routeInfo;
    } catch (error) {
        console.error('Eroare la rutare:', error);
        showStatus(`Eroare la calcularea rutei: ${error.message}`, 'danger');
        return null;
    }
}
const additionalStyles = `
.route-line {
    transition: all 0.3s ease;
}

.route-line:hover {
    opacity: 1 !important;
    filter: drop-shadow(0 0 12px rgba(0,0,0,0.6)) !important;
}

/* Animație pentru markere */
@keyframes pulse {
    0% { transform: scale(1); }
    50% { transform: scale(1.05); }
    100% { transform: scale(1); }
}

.custom-marker.active {
    animation: pulse 2s infinite;
}

/* Stil pentru popup-uri */
.leaflet-popup-content-wrapper {
    border-radius: 8px;
    box-shadow: 0 4px 20px rgba(0,0,0,0.3);
}

.leaflet-popup-tip {
    box-shadow: 0 2px 10px rgba(0,0,0,0.2);
}
`;

const additionalStyleElement = document.createElement('style');
additionalStyleElement.innerHTML = additionalStyles;
document.head.appendChild(additionalStyleElement);

async function fetchLocatiiDisponibile() {
    try {
        const response = await fetch(`${apiPrefix}/locatii-disponibile?traseuId=${routeId}`);
        if (!response.ok) throw new Error("Eroare la încărcarea locațiilor disponibile");

        const data = await response.json();
        renderLocatiiDisponibile(data);
    } catch (error) {
        console.error("Eroare la fetchLocatiiDisponibile:", error);
    }
}

function renderLocatiiDisponibile(locatii) {
    const disponibileList = document.getElementById('disponibile-list');
    if (!disponibileList) return;

    disponibileList.innerHTML = '';

    locatii.forEach(loc => {
        const li = document.createElement('li');
        li.className = 'list-group-item d-flex justify-content-between align-items-center';
        li.innerHTML = `
    <div class="d-flex flex-column flex-md-row justify-content-between w-100 align-items-start align-items-md-center">
        <div class="mb-2 mb-md-0">
            <b>${loc.name}</b><br>
            <button class="btn btn-sm btn-info mt-2 btn-detalii" data-id="${loc.id}">
                Detalii & Recenzii
            </button>
        </div>
        <div class="d-flex gap-2">
            <button class="btn btn-danger btn-sm btnStergeDefinitiv" data-id="${loc.id}">
                Șterge definitiv
            </button>
            <button class="btn btn-sm btn-success btn-adauga">
                Adaugă
            </button>
        </div>
    </div>
`;


        const detaliiBtn = li.querySelector('.btn-detalii');
        if (detaliiBtn) {
            detaliiBtn.addEventListener('click', () => {
                openLocationDetailsModal(loc);
            });
        }

        const adaugaBtn = li.querySelector('.btn-adauga');
        if (adaugaBtn) {
            adaugaBtn.addEventListener('click', () => {
                const exists = locations.some(l => l.id === loc.id);
                if (exists) {
                    showStatus("Locația este deja în traseu!", "warning");
                    return;
                }

                locations.push(loc);
                renderLocationList();
                li.remove(); 
                showStatus('Locația a fost adăugată! Apasă "Salvează ruta" pentru a păstra modificarea.', 'info');
            });
        }

        disponibileList.appendChild(li);
    });
}

function toggleRouteVisibility(routeInfo) {
    if (routeInfo.visible) {
        map.removeLayer(routeInfo.line);
    } else {
        map.addLayer(routeInfo.line);
    }
    routeInfo.visible = !routeInfo.visible;
}

let isCalculating = false;
async function recalculateRoutes() {
    if (isCalculating) return;
    isCalculating = true;

    try {
        routes.forEach(route => {
            if (route.line && map.hasLayer(route.line)) {
                map.removeLayer(route.line);
            }
        });
        routes.length = 0;
        Object.keys(routeCache).forEach(key => delete routeCache[key]);

        let totalDistance = 0;
        let totalDuration = 0;

        for (let i = 0; i < locations.length - 1; i++) {
            const start = locations[i];
            const end = locations[i + 1];

            if (!start || !end || !start.lat || !start.lon || !end.lat || !end.lon) {
                console.warn(`Skip rută invalidă: ${i} → ${i + 1}`);
                continue;
            }

            const routeInfo = await getRoute(i, i + 1);
            if (routeInfo) {
                totalDistance += parseFloat(routeInfo.distance) || 0;
                totalDuration += Number(routeInfo.duration) || 0;
            }
        }

        const distanceElement = document.getElementById('distance-value');
        const durationElement = document.getElementById('duration-value');
        if (distanceElement && durationElement) {
            distanceElement.textContent = totalDistance.toFixed(2);
            durationElement.textContent = formatTimp(totalDuration);
        }

        viewEntireRoute();
        showStatus('Rutele au fost recalculate!', 'success');
        setTimeout(() => showStatus('', ''), 3000);

    } catch (error) {
        console.error('Eroare la recalculateRoutes:', error);
        showStatus('Eroare la recalcularea traseului.', 'danger');
    } finally {
        isCalculating = false;
    }
}

// Afisare traseu complet pe harta
function viewEntireRoute() {
    if (routes.length === 0) return;
    const bounds = L.latLngBounds();
    locations.forEach(location => {
        if (location.lat && location.lon) {
            bounds.extend([location.lat, location.lon]);
        }
    });
    routes.forEach(route => {
        if (route.line) {
            bounds.extend(route.line.getBounds());
        }
    });
    map.fitBounds(bounds, { padding: [50, 50], maxZoom: 13 });
}

function renderLocationList() {
    markers.forEach(marker => map.removeLayer(marker));
    markers.length = 0;
    locationList.innerHTML = '';

    locations.forEach((location, index) => {
        if (!location.lat || !location.lon || !location.name) {
            console.warn(`Locație invalidă: ${JSON.stringify(location)}`);
            return;
        }

        const position = new L.LatLng(location.lat, location.lon);

        // Determina tipul markerului
        let markerType = 'normal';
        if (index === 0) markerType = 'start';
        else if (index === locations.length - 1) markerType = 'end';

        const marker = L.marker(position, {
            icon: createNumberedIcon(index + 1, markerType)
        }).addTo(map);

        marker.bindPopup(`<b>${location.name}</b>`);
        markers.push(marker);

        const listItem = document.createElement('li');
        listItem.classList.add('list-group-item', 'list-group-item-action');
        listItem.setAttribute('role', 'button');
        listItem.setAttribute('aria-label', `Vezi locația ${location.name}`);
        listItem.setAttribute('data-location-index', index);
        listItem.setAttribute('data-location-id', location.id);


        const contentDiv = document.createElement('div');
        contentDiv.style.flex = '1'; 
        contentDiv.style.minWidth = '0';

        contentDiv.innerHTML = `
          <div style="font-weight: bold; margin-bottom: 8px; word-wrap: break-word;">${location.name}</div>
          <div style="display: flex; justify-content: space-between; align-items: center;">
            <button class="btn btn-sm btn-info btn-detalii" data-index="${index}">Detalii & Recenzii</button>
            ${isAdmin ? `<button class="btn btn-danger btn-sm delete-location" data-location-index="${index}">Șterge</button>` : ''}
          </div>
        `;

        const detaliiBtn = contentDiv.querySelector('.btn-detalii');
        if (detaliiBtn) {
            detaliiBtn.addEventListener('click', (e) => {
                e.stopPropagation();
                openLocationDetailsModal(location);
            });
        }

        const deleteBtn = contentDiv.querySelector('.delete-location');
        if (deleteBtn) {
            deleteBtn.addEventListener('click', (e) => {
                e.stopPropagation();

                if (locations.length <= 2) {
                    showStatus('Trebuie să ai cel puțin 2 locații în traseu!', 'danger');
                    return;
                }

                const removed = locations.splice(index, 1)[0];
                renderLocationList(); 

                routes.forEach(route => {
                    if (route.line && map.hasLayer(route.line)) {
                        map.removeLayer(route.line);
                        route.visible = false;
                    }
                });

                if (isAdmin) {
                    fetchLocatiiDisponibile();
                }

                showStatus("Locația a fost ștearsă. Apasă 'Salvează ruta' pentru a actualiza traseul.", "info");
            });
        }

        contentDiv.addEventListener('click', (event) => {
            if (event.target.closest('.btn-detalii') || event.target.closest('.delete-location')) return;

            if (map && map.getCenter) {
                try {
                    routes.forEach(route => {
                        if (route.line && map.hasLayer(route.line)) {
                            map.removeLayer(route.line);
                            route._wasVisible = true;
                        } else {
                            route._wasVisible = false;
                        }
                    });

                    map.flyTo(position, 15, {
                        animate: true,
                        duration: 3.5
                    });

                    map.once('moveend', () => {
                        routes.forEach(route => {
                            if (route._wasVisible && route.line && !map.hasLayer(route.line)) {
                                map.addLayer(route.line);
                            }
                            delete route._wasVisible;
                        });
                    });

                    marker.openPopup();
                    highlightLocation(index);
                } catch (err) {
                    console.warn("Map flyTo failed:", err);
                }
            }
        });


        listItem.appendChild(contentDiv);
        locationList.appendChild(listItem); 
    });
}

// Initializare drag-and-drop
function initSortableList() {
    if (!isAdmin) {
        return;
    }

    if (typeof Sortable === 'undefined') return;

    new Sortable(locationList, {
        animation: 150,
        ghostClass: 'sortable-ghost',
        onEnd: () => {
            const listItems = locationList.querySelectorAll('li');
            const newLocations = [];
            listItems.forEach(item => {
                const locationId = parseInt(item.getAttribute('data-location-id'));
                const originalLocation = locations.find(loc => loc.id === locationId);

                if (originalLocation) newLocations.push(originalLocation);
            });
            locations.length = 0;
            locations.push(...newLocations);
            renderLocationList();
            showStatus("Ordinea a fost schimbată. Apasă 'Calculează rutele' pentru a actualiza traseul.", "info");
        }
    });
}

async function fetchLocations() {
    try {
        const response = await fetch(`${apiPrefix}/locatii?traseuId=${routeId}`);
        if (!response.ok) throw new Error('Eroare la încărcarea locațiilor');

        const data = await response.json();
        locations = data.filter(loc => loc.id && loc.name && loc.lat && loc.lon).map(loc => ({
            id: loc.id,
            name: loc.name,
            lat: loc.lat,
            lon: loc.lon,
            imageUrl: loc.imageUrl || '',
            descriere: loc.descriere || '',
            strada: loc.strada || '',
            numarStrada: loc.numarStrada || '',
            timpEstimativ: loc.timpEstimativ || 0,
            oras: loc.oras || '-',
            tara: loc.tara || '-'
        }));


        if (locations.length < 2) {
            showStatus('Trebuie să existe cel puțin 2 locații pentru a crea un traseu.', 'danger');
            return;
        }

        renderLocationList();

        if (isAdmin) {
            fetchLocatiiDisponibile(); 
        }

    } catch (error) {
        console.error('Eroare la fetchLocations:', error);
        showStatus('Nu s-au putut încărca locațiile.', 'danger');
    }
}

function updateGoogleMapsLink() {
    const googleMapsLink = document.getElementById('google-maps-link');
    if (!googleMapsLink) return;

    if (locations.length >= 2) {
        const modeMap = {
            'driving-car': 'driving',
            'cycling-regular': 'bicycling',
            'foot-walking': 'walking'
        };

        const travelMode = modeMap[transportSelect.value] || 'driving';

        const origin = `${locations[0].lat},${locations[0].lon}`;
        const destination = `${locations[locations.length - 1].lat},${locations[locations.length - 1].lon}`;

        const waypoints = locations.slice(1, -1)
            .map(loc => `${loc.lat},${loc.lon}`)
            .join('|');

        let url = `https://www.google.com/maps/dir/?api=1&origin=${origin}&destination=${destination}&travelmode=${travelMode}`;
        if (waypoints) {
            url += `&waypoints=${waypoints}`;
        }

        googleMapsLink.href = url;
        googleMapsLink.classList.remove('d-none');
    } else {
        googleMapsLink.href = '#';
        googleMapsLink.classList.add('d-none');
    }
}

function openLocationDetailsModal(locatie) {
    const modalContent = document.getElementById("modal-location-content");
    const reviewList = document.getElementById("modal-review-list");
    const reviewForm = document.getElementById("review-form");
    const ratingInput = document.getElementById("review-rating");
    const commentInput = document.getElementById("review-comment");
    const userId = document.body.getAttribute("data-user-id");

    modalContent.innerHTML = `
      <h5>${locatie.name}</h5>
      <p><strong>Adresă:</strong> ${locatie.tara}, ${locatie.oras}, ${locatie.strada} ${locatie.numarStrada}</p>
      <p><strong>Geolocație:</strong> ${locatie.lat}, ${locatie.lon}</p>
      <p><strong>Descriere:</strong> ${locatie.descriere}</p>
      <p><strong>Timp estimat vizită:</strong> ${formatTimp(locatie.timpEstimativ)}</p>
        ${locatie.imageUrl ? `<img src="${locatie.imageUrl}?v=${new Date().getTime()}" style="width: 100%; max-height: 300px; object-fit: cover;">` : ''}
      <div id="modal-rating-summary" class="mt-3 mb-2"></div>
    `;

    ratingInput.value = '';
    commentInput.value = '';
    loadRatingSummary(locatie.id);
    loadReviewsForLocation(locatie.id, reviewList);

    reviewForm.onsubmit = async (e) => {
        e.preventDefault();
        const rating = parseInt(ratingInput.value);
        const comment = commentInput.value;

        const response = await fetch("/api/recenzii", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                locatieId: locatie.id,
                rating,
                comentariu: comment,
                utilizatorId: userId
            })
        });

        if (response.ok) {
            showReviewStatus("Recenzia a fost adăugată!", "success");
            ratingInput.value = '';
            commentInput.value = '';
            loadReviewsForLocation(locatie.id, reviewList);
            loadRatingSummary(locatie.id);
        } else {
            const err = await response.json();
            showReviewStatus(err.message || "Ai adăugat deja o recenzie pentru această locație azi.", "warning");
        }
    };

    new bootstrap.Modal(document.getElementById('locationDetailsModal')).show();
}


function loadRatingSummary(locatieId) {
    fetch(`/api/recenzii/rating-summary/${locatieId}`)
        .then(r => r.json())
        .then(summary => {
            const target = document.getElementById("modal-rating-summary");
            if (!summary || summary.total === 0) {
                target.innerHTML = ``;
                return;
            }

            const starIcons = renderStarIcons(summary.medie);
            target.innerHTML = `
              <div class="d-flex align-items-center mb-2">
                <h6 class="me-2 mb-0">${summary.medie.toFixed(1)}</h6>
                ${starIcons}
                <span class="ms-2 text-muted">(${summary.total} recenzii)</span>
              </div>
            `;

            const labels = { 5: "Excelent", 4: "Bun", 3: "Mediu", 2: "Slab", 1: "Foarte slab" };
            for (let i = 5; i >= 1; i--) {
                const count = summary.distributie[i] || 0;
                const percent = Math.round((count / summary.total) * 100);
                target.innerHTML += `
                    <div class="mb-1"><small>${labels[i]}: ${count}</small>
                      <div class="progress">
                          <div class="progress-bar bg-success" style="width: ${percent}%;"></div>
                      </div>
                    </div>`;
            }
        });
}


function renderStarIcons(rating) {
    let stars = "";
    for (let i = 1; i <= 5; i++) {
        if (rating >= i) stars += '<i class="fas fa-star text-warning"></i>';
        else if (rating >= i - 0.5) stars += '<i class="fas fa-star-half-alt text-warning"></i>';
        else stars += '<i class="far fa-star text-warning"></i>';
    }
    return stars;
}

function loadReviewsForLocation(locatieId, container) {
    fetch(`/api/recenzii/${locatieId}`)
        .then(r => r.json())
        .then(reviews => {
            container.innerHTML = '';

            if (!reviews || reviews.length === 0) {
                container.innerHTML = `<p class="text-muted">⚠ Nu există recenzii momentan.</p>`;
                return;
            }

            // Titlu
            container.innerHTML = `<h6 class="mt-4">Recenzii existente:</h6>`;
            const ul = document.createElement('ul');
            ul.className = 'list-group';

            reviews.forEach(r => {
                const li = document.createElement('li');
                li.className = 'list-group-item';
                li.innerHTML = `
                    <b>${r.utilizatorNume}</b> (${r.data}): ${r.rating}/5<br>
                    ${r.comentariu}
                    ${r.canDelete ? `<button class="btn btn-danger btn-sm float-end">Șterge</button>` : ''}
                `;
                if (r.canDelete) setupDeleteButton(li, r.id, locatieId, container);
                ul.appendChild(li);
            });

            container.appendChild(ul);
        });
}

function setupDeleteButton(listItem, recenzieId, locatieId, container) {
    const deleteBtn = listItem.querySelector('button');
    deleteBtn.addEventListener('click', () => {
        const confirmDiv = document.createElement('div');
        confirmDiv.className = 'mt-2';
        confirmDiv.innerHTML = `
            <small>Ești sigur?</small><br>
            <button class="btn btn-sm btn-danger me-2">Da</button>
            <button class="btn btn-sm btn-secondary">Nu</button>`;
        listItem.querySelector('.mt-2')?.remove();
        listItem.appendChild(confirmDiv);

        confirmDiv.querySelector('.btn-danger').addEventListener('click', async () => {
            const res = await fetch(`/api/recenzii/${recenzieId}`, { method: "DELETE" });
            if (res.ok) {
                showReviewStatus("Recenzie ștearsă!", "success");
                loadReviewsForLocation(locatieId, container);
                loadRatingSummary(locatieId);
            } else {
                showReviewStatus("Eroare la ștergere.", "danger");
            }
        });
        confirmDiv.querySelector('.btn-secondary').addEventListener('click', () => confirmDiv.remove());
    });
}

let reviewStatusTimeout;

function showReviewStatus(message, type) {
    const div = document.getElementById("review-status-message");
    if (!div) return;

    clearTimeout(reviewStatusTimeout); 
    div.className = `alert alert-${type}`;
    div.textContent = message;
    div.classList.remove("d-none");

    reviewStatusTimeout = setTimeout(() => {
        div.classList.add("d-none");
    }, 4000);
}




// Initializare aplicatie
async function initializeApp() {
    await fetchLocations();
    renderLocationList();
    initSortableList();
    await recalculateRoutes();
    updateGoogleMapsLink();
    if (isAdmin) {
        await fetchLocatiiDisponibile();
    }

    if (transportSelect) {
        transportSelect.addEventListener('change', async () => {
            showStatus('Se recalculează rutele...', 'info');
            await recalculateRoutes();
        });
    }

    const viewRouteBtn = document.getElementById('view-route-btn');
    if (viewRouteBtn) {
        viewRouteBtn.addEventListener('click', viewEntireRoute);
    }

    const pdfCompletBtn = document.getElementById('pdf-complet-btn');
    if (pdfCompletBtn) {
        pdfCompletBtn.addEventListener('click', async () => {
            pdfCompletBtn.disabled = true;
            pdfCompletBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Se generează PDF...';
            showStatus('Se generează PDF-ul complet cu harta...', 'info');

            try {
                await captureMapAndDownloadPdf(); 
            } catch (error) {
                console.error('Eroare PDF:', error);
                showStatus('Eroare la generarea PDF-ului complet.', 'danger');
            } finally {
                pdfCompletBtn.disabled = false;
                pdfCompletBtn.innerHTML = '<i class="fas fa-file-pdf"></i> Descarcă PDF complet';
            }
        });
    }



    const calcRouteBtn = document.getElementById('calc-route-btn');
    if (calcRouteBtn && isAdmin) {
        calcRouteBtn.addEventListener('click', async () => {
            calcRouteBtn.disabled = true;
            showStatus('Se recalculează rutele și se salvează ordinea...', 'info');

            await recalculateRoutes();

            const orderedIds = locations.map(loc => loc.id);
            try {
                const response = await fetch(`${apiPrefix}/salveaza-ordine?traseuId=${routeId}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(orderedIds)
                });

                const result = await response.json();
                if (result.success) {
                    showStatus('Ordinea a fost salvată cu succes!', 'success');
                    await fetchLocatiiDisponibile();
                    updateGoogleMapsLink();
                } else {
                    showStatus('A apărut o problemă la salvare.', 'danger');
                }
            } catch (error) {
                console.error('Eroare la salvare:', error);
                showStatus('Eroare la trimiterea ordinii către server.', 'danger');
            }

            calcRouteBtn.disabled = false;
        });
    }
}

async function personalizeazaTraseu() {
    const templateTraseuId = routeId;
    const templateName = document.getElementById("page-template")?.dataset.templateName || "Traseu";

    const response = await fetch(`/api/TraseePersonalizate/create?templateTraseuId=${templateTraseuId}&templateName=${encodeURIComponent(templateName)}`, {
        method: "POST"
    });

    if (response.ok) {
        const data = await response.json();
        window.location.href = `/Trasee/EditareTraseuPersonalizat?traseuId=${data.idTraseu}`;
    } else {
        alert("Eroare la creare traseu personalizat.");
    }
}

async function captureMapAndDownloadPdf() {
    const mapElement = document.getElementById("map-container");

    const removedRoutes = [];
    routes.forEach(route => {
        if (route.line && map.hasLayer(route.line)) {
            removedRoutes.push(route);
            map.removeLayer(route.line);
        }
    });

    viewEntireRoute();
    await new Promise(resolve => setTimeout(resolve, 800));

    try {
        const canvas = await html2canvas(mapElement, { useCORS: true });
        const imageBase64 = canvas.toDataURL("image/png");

        const locatieDtos = locations.map((loc, idx) => ({
            nume: loc.name,
            geolocatie: `${loc.lat},${loc.lon}`,
            distanta: routes[idx]?.distance || null,
            durata: routes[idx]?.duration || null,
            strada: loc.strada || "",
            numarStrada: loc.numarStrada || "",
            descriere: loc.descriere || "",
            timpEstimativ: loc.timpEstimativ || 0,
            imagineUrl: loc.imageUrl || null,
            oras: loc.oras || "",      
            tara: loc.tara || ""        
        }));



        const dto = {
            titlu: document.title || "Traseu PDF",
            locatii: locatieDtos,
            distantaTotala: parseFloat(document.getElementById("distance-value")?.textContent || "0"),
            DurataTotala: routes.reduce((acc, r) => acc + Number(r.duration), 0),
            imageBase64: imageBase64
        };

        const response = await fetch(`${apiPrefix}/pdf-complet/${routeId}`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(dto)
        });
        if (routes.length === 0) {
            showStatus("Apasă 'Calculează rutele' înainte de a genera PDF-ul.", "warning");
            return;
        }

        if (!response.ok) {
            showStatus("Eroare la generarea PDF-ului", "danger");
            return;
        }

        const blob = await response.blob();
        const link = document.createElement("a");
        link.href = URL.createObjectURL(blob);
        link.download = "Traseu TopicTripsGuide.pdf";
        link.click();
        URL.revokeObjectURL(link.href);
        showStatus("PDF generat cu succes!", "success");
    } catch (err) {
        console.error("Eroare PDF:", err);
        showStatus("A apărut o eroare la generarea PDF-ului.", "danger");
    } finally {
        // Readauga rutele
        removedRoutes.forEach(route => {
            if (route.line) map.addLayer(route.line);
        });
    }
}
