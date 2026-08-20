document.addEventListener("DOMContentLoaded", async function () {

    // =========================================================
    // CONFIGURATION
    // =========================================================

    const START_HOUR = 6;


    // =========================================================
    // SERVER DATA
    // =========================================================

    const config =
        window.dailyEnergyConfig || {};

    const currentCompany =
        config.currentCompany || "";

    const getAvailableTimesUrl =
        config.getAvailableTimesUrl || "";

    let existingTimes = [];

    let nextAvailableTime = "";


    // =========================================================
    // ELEMENTS
    // =========================================================

    const form =
        document.getElementById("consumptionForm");

    const addButton =
        document.getElementById("addRow");

    const temporaryBody =
        document.getElementById("temporaryBody");

    const hiddenContainer =
        document.getElementById("hiddenItemsContainer");

    const temporaryFooter =
        document.getElementById("temporaryFooter");

    const grandTotalElement =
        document.getElementById("grandTotal");

    const timeFullMessage =
        document.getElementById("timeFullMessage");

    const dateInput =
        document.getElementById("inputDate");

    const timeInput =
        document.getElementById("inputTime");

    const timeDisplayInput =
        document.getElementById("inputTimeDisplay");

    const saveAllButton =
        document.getElementById("saveAllButton");

    const savingOverlay =
        document.getElementById("savingOverlay");


    // =========================================================
    // SAFETY CHECK
    // =========================================================

    if (!form || !addButton || !temporaryBody) {
        console.error(
            "Daily Energy Fuel Consumption elements were not found."
        );
        return;
    }


    // =========================================================
    // ENERGY FIELDS
    // =========================================================

    const energyFields = [
        "Reb",
        "Gg1",
        "Gg2",
        "Gg3",
        "Gg4",
        "Dg1",
        "Dg2",
        "Dg3",
        "Dg4",
        "Solar"
    ];


    // =========================================================
    // REQUIRED FIELDS
    // =========================================================

    const requiredFields = [
        {
            id: "inputReb",
            name: "REB"
        },
        {
            id: "inputGg1",
            name: "GG-1"
        },
        {
            id: "inputGg2",
            name: "GG-2"
        },
        {
            id: "inputGg3",
            name: "GG-3"
        },
        {
            id: "inputGg4",
            name: "GG-4"
        },
        {
            id: "inputDg1",
            name: "DG-1"
        },
        {
            id: "inputDg2",
            name: "DG-2"
        },
        {
            id: "inputDg3",
            name: "DG-3"
        },
        {
            id: "inputDg4",
            name: "DG-4"
        },
        {
            id: "inputSolar",
            name: "Solar"
        },
        {
            id: "inputCaptiveGenerator",
            name: "Captive Generator"
        },
        {
            id: "inputIndustrialBoiler",
            name: "Industrial Boiler"
        }
    ];


    // =========================================================
    // ROW COUNT
    // =========================================================

    let rowCount = 0;


    // =========================================================
    // PREVIOUS DATE
    // =========================================================

    let previousDate =
        dateInput.value;


    // =========================================================
    // GET INPUT VALUE
    // =========================================================

    function getInputValue(id) {

        const element =
            document.getElementById(id);

        if (!element) {
            return "";
        }

        return element.value ?? "";
    }


    // =========================================================
    // NORMALIZE TIME
    // =========================================================

    function normalizeTime(time) {

        if (!time) {
            return "";
        }

        return String(time).substring(0, 5);
    }


    // =========================================================
    // GET USED TIMES
    // =========================================================

    function getUsedTimes() {

        const used = new Set();


        if (Array.isArray(existingTimes)) {

            existingTimes.forEach(function (time) {

                const normalized =
                    normalizeTime(time);

                if (normalized) {
                    used.add(normalized);
                }

            });
        }


        temporaryBody
            .querySelectorAll(".temporary-row")
            .forEach(function (row) {

                const time =
                    normalizeTime(
                        row.dataset.time
                    );

                if (time) {
                    used.add(time);
                }

            });


        return used;
    }


    // =========================================================
    // DAILY TIME SLOTS
    // =========================================================

    function getDailySlots() {

        const slots = [];


        for (let i = 0; i < 24; i++) {

            const hour =
                (START_HOUR + i) % 24;

            const slot =
                String(hour).padStart(2, "0") +
                ":00";

            slots.push(slot);
        }


        return slots;
    }


    // =========================================================
    // GET FIRST AVAILABLE TIME
    // =========================================================

    function getFirstAvailableTime() {

        const used =
            getUsedTimes();

        const slots =
            getDailySlots();


        console.log(
            "Existing DB times:",
            existingTimes
        );

        console.log(
            "Used times:",
            Array.from(used)
        );

        console.log(
            "Daily slots:",
            slots
        );


        for (const slot of slots) {

            if (!used.has(slot)) {

                console.log(
                    "NEXT AVAILABLE TIME:",
                    slot
                );

                return slot;
            }
        }


        return null;
    }


    // =========================================================
    // FORMAT TIME
    // =========================================================

    function formatTime(time) {

        if (!time) {
            return "";
        }


        const normalized =
            normalizeTime(time);

        const parts =
            normalized.split(":");


        if (parts.length < 2) {
            return normalized;
        }


        const hour =
            parseInt(parts[0], 10);

        const minute =
            parts[1];


        if (isNaN(hour)) {
            return normalized;
        }


        const period =
            hour >= 12
                ? "PM"
                : "AM";


        let hour12 =
            hour % 12;


        if (hour12 === 0) {
            hour12 = 12;
        }


        return (
            String(hour12).padStart(2, "0") +
            ":" +
            minute +
            " " +
            period
        );
    }


    // =========================================================
    // UPDATE NEXT AVAILABLE TIME
    // =========================================================

    function setNextAvailableTime() {

        const next =
            getFirstAvailableTime();


        nextAvailableTime =
            next || "";


        if (!next) {

            timeInput.value = "";

            timeDisplayInput.value =
                "Completed";

            addButton.disabled =
                true;

            timeFullMessage
                .classList
                .remove("d-none");

            return;
        }


        timeInput.value =
            next;

        timeDisplayInput.value =
            formatTime(next);

        addButton.disabled =
            false;

        timeFullMessage
            .classList
            .add("d-none");
    }


    // =========================================================
    // CLEAR TEMPORARY ROWS
    // =========================================================

    function clearTemporaryRows() {

        temporaryBody.innerHTML = "";

        hiddenContainer.innerHTML = "";

        rowCount = 0;

        temporaryFooter
            .classList
            .add("d-none");

        grandTotalElement.textContent =
            "0.00";
    }


    // =========================================================
    // LOAD TIMES FROM DATABASE
    // =========================================================

    async function loadAvailableTimes(date) {

        if (!date) {
            return false;
        }


        try {

            const url =
                getAvailableTimesUrl +
                "?date=" +
                encodeURIComponent(date);


            console.log(
                "Checking database times for:",
                date
            );


            const response =
                await fetch(
                    url,
                    {
                        method: "GET",
                        headers: {
                            "Accept":
                                "application/json"
                        },
                        cache: "no-store"
                    }
                );


            if (!response.ok) {

                throw new Error(
                    "HTTP " +
                    response.status
                );
            }


            const data =
                await response.json();


            console.log(
                "GetAvailableTimes response:",
                data
            );


            if (!data.success) {

                alert(
                    data.message ||
                    "Could not load available times."
                );

                return false;
            }


            existingTimes =
                Array.isArray(
                    data.existingTimes
                )
                    ? data.existingTimes
                    : [];


            setNextAvailableTime();


            return true;

        }
        catch (error) {

            console.error(
                "GetAvailableTimes error:",
                error
            );


            alert(
                "Could not check existing time slots."
            );


            return false;
        }
    }


    // =========================================================
    // DATE CHANGE
    // =========================================================

    dateInput.addEventListener(
        "change",
        async function () {

            const newDate =
                dateInput.value;


            if (!newDate) {
                return;
            }


            if (rowCount > 0) {

                const confirmed =
                    confirm(
                        "Temporary readings have already been added.\n\n" +
                        "Changing the date will clear all temporary readings.\n\n" +
                        "Do you want to continue?"
                    );


                if (!confirmed) {

                    dateInput.value =
                        previousDate;

                    return;
                }


                clearTemporaryRows();
            }


            const loaded =
                await loadAvailableTimes(
                    newDate
                );


            if (!loaded) {

                dateInput.value =
                    previousDate;

                return;
            }


            previousDate =
                newDate;
        }
    );


    // =========================================================
    // CALCULATE INPUT TOTAL
    // =========================================================

    function calculateInputTotal() {

        let total = 0;


        energyFields.forEach(function (field) {

            const value =
                parseFloat(
                    getInputValue(
                        "input" + field
                    )
                );


            if (!isNaN(value)) {
                total += value;
            }

        });


        document.getElementById(
            "inputTotal"
        ).value =
            total.toFixed(2);
    }


    // =========================================================
    // INPUT EVENT
    // =========================================================

    document.addEventListener(
        "input",
        function (event) {

            if (
                event.target.classList.contains(
                    "energy-input"
                ) ||
                event.target.classList.contains(
                    "required-consumption-field"
                )
            ) {

                calculateInputTotal();
            }
        }
    );


    // =========================================================
    // VALIDATE REQUIRED FIELDS
    // =========================================================

    function validateRequiredFields() {

        for (const field of requiredFields) {

            const element =
                document.getElementById(
                    field.id
                );


            if (!element) {
                continue;
            }


            const value =
                String(
                    element.value ?? ""
                ).trim();


            if (value === "") {

                alert(
                    field.name +
                    " is required. Please enter a value."
                );

                element.focus();

                return false;
            }


            const number =
                Number(value);


            if (!Number.isFinite(number)) {

                alert(
                    field.name +
                    " must contain a valid number."
                );

                element.focus();

                return false;
            }
        }


        return true;
    }


    // =========================================================
    // CREATE EDITABLE CELL
    // =========================================================

    function createEditableCell(
        field,
        value
    ) {

        return `
            <td>
                <input type="number"
                       step="any"
                       class="form-control form-control-sm editable-field"
                       data-field="${escapeHtml(field)}"
                       value="${escapeHtml(value)}"
                       autocomplete="off">
            </td>
        `;
    }


    // =========================================================
    // ADD ROW
    // =========================================================

    addButton.addEventListener(
        "click",
        function () {

            const currentTime =
                getFirstAvailableTime();


            if (!currentTime) {

                alert(
                    "All 24 hourly slots for this date have already been entered."
                );

                setNextAvailableTime();

                return;
            }


            const company =
                getInputValue(
                    "inputCompany"
                ).trim();


            const date =
                getInputValue(
                    "inputDate"
                );


            const time =
                currentTime;


            const timeDisplay =
                formatTime(
                    currentTime
                );


            if (!company) {

                alert(
                    "Company is required."
                );

                return;
            }


            if (!date) {

                alert(
                    "Date is required."
                );

                return;
            }


            if (!validateRequiredFields()) {
                return;
            }


            const values = {};

            let total = 0;


            energyFields.forEach(
                function (field) {

                    const value =
                        getInputValue(
                            "input" + field
                        );


                    values[field] =
                        value;


                    const numericValue =
                        parseFloat(value);


                    if (!isNaN(numericValue)) {

                        total +=
                            numericValue;
                    }

                }
            );


            values.CaptiveGenerator =
                getInputValue(
                    "inputCaptiveGenerator"
                );


            values.IndustrialBoiler =
                getInputValue(
                    "inputIndustrialBoiler"
                );


            // =================================================
            // CREATE ROW
            // =================================================

            const row =
                document.createElement("tr");


            row.className =
                "temporary-row";


            row.dataset.index =
                rowCount;


            row.dataset.time =
                time;


            row.innerHTML = `

                <td>
                    <input type="text"
                           class="form-control form-control-sm"
                           value="${escapeHtml(company)}"
                           readonly>
                </td>

                <td>
                    <input type="date"
                           class="form-control form-control-sm"
                           value="${escapeHtml(date)}"
                           readonly>
                </td>

                <td>
                    <input type="text"
                           class="form-control form-control-sm time-display"
                           value="${escapeHtml(timeDisplay)}"
                           readonly>
                </td>

                ${createEditableCell("Reb", values.Reb)}
                ${createEditableCell("Gg1", values.Gg1)}
                ${createEditableCell("Gg2", values.Gg2)}
                ${createEditableCell("Gg3", values.Gg3)}
                ${createEditableCell("Gg4", values.Gg4)}
                ${createEditableCell("Dg1", values.Dg1)}
                ${createEditableCell("Dg2", values.Dg2)}
                ${createEditableCell("Dg3", values.Dg3)}
                ${createEditableCell("Dg4", values.Dg4)}
                ${createEditableCell("Solar", values.Solar)}

                <td>
                    <input type="number"
                           class="form-control form-control-sm row-total total-display"
                           value="${total.toFixed(2)}"
                           readonly>
                </td>

                ${createEditableCell(
                "CaptiveGenerator",
                values.CaptiveGenerator
            )}

                ${createEditableCell(
                "IndustrialBoiler",
                values.IndustrialBoiler
            )}
            `;


            temporaryBody.appendChild(row);


            // =================================================
            // HIDDEN MODEL BINDING FIELDS
            // =================================================

            createHiddenInput(
                rowCount,
                "Company",
                company
            );


            createHiddenInput(
                rowCount,
                "Trdate",
                date
            );


            createHiddenInput(
                rowCount,
                "Time",
                time
            );


            energyFields.forEach(
                function (field) {

                    createHiddenInput(
                        rowCount,
                        field,
                        values[field]
                    );

                }
            );


            createHiddenInput(
                rowCount,
                "Total",
                total.toFixed(2)
            );


            createHiddenInput(
                rowCount,
                "CaptiveGenerator",
                values.CaptiveGenerator
            );


            createHiddenInput(
                rowCount,
                "IndustrialBoiler",
                values.IndustrialBoiler
            );


            setupRowEvents(row);


            rowCount++;


            temporaryFooter
                .classList
                .remove("d-none");


            updateGrandTotal();


            resetInputRow();

        }
    );


    // =========================================================
    // SETUP ROW EVENTS
    // =========================================================

    function setupRowEvents(row) {

        const editableFields =
            row.querySelectorAll(
                ".editable-field"
            );


        editableFields.forEach(
            function (input) {

                input.addEventListener(
                    "input",
                    function () {

                        updateTemporaryRow(
                            row
                        );

                    }
                );

            }
        );
    }


    // =========================================================
    // UPDATE TEMPORARY ROW
    // =========================================================

    function updateTemporaryRow(row) {

        const index =
            parseInt(
                row.dataset.index,
                10
            );


        if (isNaN(index)) {
            return;
        }


        let total = 0;


        energyFields.forEach(
            function (field) {

                const input =
                    row.querySelector(
                        `[data-field="${field}"]`
                    );


                if (!input) {
                    return;
                }


                const value =
                    parseFloat(
                        input.value
                    );


                if (!isNaN(value)) {
                    total += value;
                }

            }
        );


        const totalInput =
            row.querySelector(
                ".row-total"
            );


        if (totalInput) {

            totalInput.value =
                total.toFixed(2);
        }


        energyFields.forEach(
            function (field) {

                updateHiddenInput(
                    index,
                    field,
                    getRowValue(
                        row,
                        field
                    )
                );

            }
        );


        updateHiddenInput(
            index,
            "Total",
            total.toFixed(2)
        );


        updateHiddenInput(
            index,
            "CaptiveGenerator",
            getRowValue(
                row,
                "CaptiveGenerator"
            )
        );


        updateHiddenInput(
            index,
            "IndustrialBoiler",
            getRowValue(
                row,
                "IndustrialBoiler"
            )
        );


        updateGrandTotal();
    }


    // =========================================================
    // GET ROW VALUE
    // =========================================================

    function getRowValue(
        row,
        field
    ) {

        const input =
            row.querySelector(
                `[data-field="${field}"]`
            );


        if (!input) {
            return "";
        }


        return input.value ?? "";
    }


    // =========================================================
    // CREATE HIDDEN INPUT
    // =========================================================

    function createHiddenInput(
        index,
        field,
        value
    ) {

        const input =
            document.createElement("input");


        input.type = "hidden";


        input.name =
            `Items[${index}].${field}`;


        input.value =
            value ?? "";


        input.dataset.index =
            index;


        input.dataset.field =
            field;


        hiddenContainer.appendChild(
            input
        );
    }


    // =========================================================
    // UPDATE HIDDEN INPUT
    // =========================================================

    function updateHiddenInput(
        index,
        field,
        value
    ) {

        const input =
            hiddenContainer.querySelector(
                `input[data-index="${index}"][data-field="${field}"]`
            );


        if (!input) {
            return;
        }


        input.value =
            value ?? "";
    }


    // =========================================================
    // RESET INPUT ROW
    // =========================================================

    function resetInputRow() {

        document.getElementById(
            "inputCompany"
        ).value =
            currentCompany;


        setNextAvailableTime();


        energyFields.forEach(
            function (field) {

                const element =
                    document.getElementById(
                        "input" + field
                    );


                if (element) {
                    element.value = "";
                }

            }
        );


        document.getElementById(
            "inputCaptiveGenerator"
        ).value = "";


        document.getElementById(
            "inputIndustrialBoiler"
        ).value = "";


        document.getElementById(
            "inputTotal"
        ).value =
            "0.00";


        document.getElementById(
            "inputReb"
        ).focus();
    }


    // =========================================================
    // UPDATE GRAND TOTAL
    // =========================================================

    function updateGrandTotal() {

        let total = 0;


        temporaryBody
            .querySelectorAll(".row-total")
            .forEach(
                function (input) {

                    const value =
                        parseFloat(
                            input.value
                        );


                    if (!isNaN(value)) {
                        total += value;
                    }

                }
            );


        grandTotalElement.textContent =
            total.toFixed(2);
    }


    // =========================================================
    // ESCAPE HTML
    // =========================================================

    function escapeHtml(value) {

        if (
            value === null ||
            value === undefined
        ) {
            return "";
        }


        return String(value)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }


    // =========================================================
    // FORM SUBMIT
    // =========================================================

    form.addEventListener(
        "submit",
        function (event) {

            if (rowCount === 0) {

                event.preventDefault();

                alert(
                    "Please add at least one reading before saving."
                );

                return;
            }


            const times = [];


            temporaryBody
                .querySelectorAll(
                    ".temporary-row"
                )
                .forEach(
                    function (row) {

                        if (row.dataset.time) {

                            times.push(
                                normalizeTime(
                                    row.dataset.time
                                )
                            );
                        }

                    }
                );


            const uniqueTimes =
                new Set(times);


            if (
                uniqueTimes.size !==
                times.length
            ) {

                event.preventDefault();

                alert(
                    "Duplicate time slot detected."
                );

                return;
            }


            const hiddenRows =
                hiddenContainer.querySelectorAll(
                    'input[name^="Items["]'
                );


            if (hiddenRows.length === 0) {

                event.preventDefault();

                alert(
                    "No readings were prepared for saving."
                );

                return;
            }


            saveAllButton.disabled =
                true;


            savingOverlay
                .classList
                .add("show");
        }
    );


    // =========================================================
    // INITIALIZATION
    // =========================================================

    calculateInputTotal();

    previousDate =
        dateInput.value;


    console.log(
        "Initial date:",
        dateInput.value
    );

    console.log(
        "START_HOUR:",
        START_HOUR
    );


    await loadAvailableTimes(
        dateInput.value
    );


    setNextAvailableTime();


    console.log(
        "Final displayed time:",
        timeDisplayInput.value
    );

});