(function () {
    const config = window.automovilesMaestroDetalleConfig;
    if (!config) {
        return;
    }

    const tipoSelect = document.getElementById('tipoSelect');
    const consultarBtn = document.getElementById('consultarBtn');
    const automovilesTableBody = document.querySelector('#automovilesTable tbody');
    const ingresosTableBody = document.querySelector('#ingresosTable tbody');
    const masterMessage = document.getElementById('masterMessage');
    const detailMessage = document.getElementById('detailMessage');

    let selectedRow = null;
    let currentTipo = '';

    consultarBtn.addEventListener('click', async function () {
        const tipo = tipoSelect.value;
        currentTipo = tipo;
        selectedRow = null;
        clearDetail('Seleccione un automóvil para consultar sus ingresos.');

        if (!tipo) {
            renderMasterMessage('Debe seleccionar un tipo.', true);
            renderAutomoviles([]);
            return;
        }

        renderMasterMessage('Consultando automóviles...', false);

        try {
            const response = await fetch(`${config.buscarPorTipoUrl}?tipo=${encodeURIComponent(tipo)}`);
            const data = await parseResponse(response);

            if (!response.ok) {
                throw new Error(data.message || 'No fue posible consultar automóviles.');
            }

            renderAutomoviles(data);
        } catch (error) {
            renderMasterMessage(error.message, true);
            renderAutomoviles([]);
        }
    });

    function renderAutomoviles(automoviles) {
        automovilesTableBody.innerHTML = '';

        if (!automoviles.length) {
            automovilesTableBody.innerHTML = '<tr><td colspan="5">No se encontraron automóviles.</td></tr>';
            return;
        }

        renderMasterMessage(`${automoviles.length} automóviles encontrados.`, false);

        automoviles.forEach(function (automovil) {
            const row = document.createElement('tr');
            row.style.cursor = 'pointer';
            row.innerHTML = `
                <td>${automovil.id}</td>
                <td>${automovil.fabricante}</td>
                <td>${automovil.color}</td>
                <td>${automovil.anio}</td>
                <td>${automovil.tipo}</td>`;

            row.addEventListener('click', function () {
                highlightRow(row);
                loadIngresos(automovil.id, currentTipo);
            });

            automovilesTableBody.appendChild(row);
        });
    }

    async function loadIngresos(automovilId, tipo) {
        detailMessage.textContent = 'Consultando ingresos...';
        detailMessage.className = '';
        ingresosTableBody.innerHTML = '';

        try {
            const response = await fetch(
                `${config.buscarIngresosUrl}?automovilId=${encodeURIComponent(automovilId)}&tipo=${encodeURIComponent(tipo)}`);
            const data = await parseResponse(response);

            if (!response.ok) {
                throw new Error(data.message || 'No fue posible consultar ingresos.');
            }

            renderIngresos(data);
        } catch (error) {
            clearDetail(error.message, true);
        }
    }

    function renderIngresos(ingresos) {
        ingresosTableBody.innerHTML = '';

        if (!ingresos.length) {
            clearDetail('No se encontraron ingresos para el automóvil seleccionado.');
            return;
        }

        detailMessage.textContent = `${ingresos.length} ingresos encontrados.`;
        detailMessage.className = '';

        ingresos.forEach(function (ingreso) {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${ingreso.parqueoId}</td>
                <td>${formatDate(ingreso.fechaEntrada)}</td>
                <td>${formatDate(ingreso.fechaSalida)}</td>
                <td>${formatNumber(ingreso.duracionMinutos)}</td>
                <td>${formatNumber(ingreso.duracionHoras)}</td>
                <td>${formatCurrency(ingreso.montoTotal)}</td>`;

            ingresosTableBody.appendChild(row);
        });
    }

    function clearDetail(message, isError) {
        detailMessage.textContent = message;
        detailMessage.className = isError ? 'text-danger' : '';
        ingresosTableBody.innerHTML = '<tr><td colspan="6">No hay datos para mostrar.</td></tr>';
    }

    function renderMasterMessage(message, isError) {
        masterMessage.textContent = message;
        masterMessage.className = isError ? 'text-danger' : '';
    }

    function highlightRow(row) {
        if (selectedRow) {
            selectedRow.style.backgroundColor = '';
        }

        selectedRow = row;
        selectedRow.style.backgroundColor = '#d9e8ff';
    }

    async function parseResponse(response) {
        const contentType = response.headers.get('content-type') || '';
        if (contentType.includes('application/json')) {
            return await response.json();
        }

        return { message: await response.text() };
    }

    function formatDate(value) {
        if (!value) {
            return '-';
        }

        return new Date(value).toLocaleString();
    }

    function formatNumber(value) {
        return value == null ? '-' : Number(value).toFixed(2);
    }

    function formatCurrency(value) {
        return value == null
            ? '-'
            : new Intl.NumberFormat('es-CR', { style: 'currency', currency: 'CRC' }).format(value);
    }
})();