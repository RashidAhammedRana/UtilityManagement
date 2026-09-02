document.addEventListener("DOMContentLoaded", async function () {

    // =========================================================
    // CONFIGURATION
    // =========================================================

    const START_HOUR = 6;
    const TOTAL_SLOTS = 24;


    // =========================================================
    // SERVER CONFIG
    // =========================================================

    const config = window.boilerSteamConfig || {};

    const currentCompany =
        config.currentCompany || "";

    const getAvailableTimesUrl =
        config.getAvailableTimesUrl || "";


    // =========================================================
    // VARIABLES
    // =========================================================

    let existingTimes = [];

    let nextAvailableTime = "";

    let rowCount = 0;


    // =========================================================
    // ELEMENTS
    // =========================================================

    const form =
        document.getElementById("boilerForm");

    const addButton =
        document.getElementById("addRow");

    const temporaryBody =
        document.getElementById("temporaryBody");

    const hiddenContainer =
        document.getElementById("hiddenItemsContainer");

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

    if (
        !form ||
        !addButton ||
        !temporaryBody ||
        !hiddenContainer
    ) {

        console.error(
            "Boiler Steam Generation elements not found."
        );

        return;
    }


    // =========================================================
    // NUMERIC FIELDS
    // =========================================================

    const numericFields = [

        "GasPressure",

        "HeaderSteamPressure",

        "Boiler1SteamGeneration",

        "Boiler2SteamGeneration",

        "Boiler3SteamGeneration",

        "EgbBoilerSteamGeneration",

        "TotalGeneration"

    ];


    // =========================================================
    // FUEL FIELDS
    // =========================================================

    const fuelFields = [

        "B1UsageFuel",

        "B2UsageFuel",

        "B3UsageFuel"

    ];


    // =========================================================
    // DATA FIELDS
    // =========================================================

    const dataFields = [

        "GasPressure",

        "HeaderSteamPressure",

        "Boiler1SteamGeneration",

        "B1UsageFuel",

        "Boiler2SteamGeneration",

        "B2UsageFuel",

        "Boiler3SteamGeneration",

        "B3UsageFuel",

        "EgbBoilerSteamGeneration",

        "TotalGeneration"

    ];


    // =========================================================
    // FUEL OPTIONS
    // =========================================================

    const fuelOptions = [

        "NG",
        "CNG",
        "LPG",
        "DIESEL"

    ];


    // =========================================================
    // PREVIOUS DATE
    // =========================================================

    let previousDate =
        dateInput
            ? dateInput.value
            : "";


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
    // CALCULATE TOTAL GENERATION
    // =========================================================

    function calculateTotalGeneration() {

        const b1 =
            parseFloat(
                getInputValue(
                    "inputBoiler1SteamGeneration"
                )
            ) || 0;


        const b2 =
            parseFloat(
                getInputValue(
                    "inputBoiler2SteamGeneration"
                )
            ) || 0;


        const b3 =
            parseFloat(
                getInputValue(
                    "inputBoiler3SteamGeneration"
                )
            ) || 0;


        const egb =
            parseFloat(
                getInputValue(
                    "inputEgbBoilerSteamGeneration"
                )
            ) || 0;


        const total =
            b1 + b2 + b3 + egb;


        const totalInput =
            document.getElementById(
                "inputTotalGeneration"
            );


        if (totalInput) {

            totalInput.value =
                total.toFixed(2);

        }


        return total.toFixed(2);
    }


    // =========================================================
    // NORMALIZE TIME
    // =========================================================

    function normalizeTime(time) {

        if (
            time === null ||
            time === undefined ||
            time === ""
        ) {

            return "";
        }


        let value =
            String(time)
                .trim();


        // -----------------------------------------------------
        // SQL TIME FORMAT
        // Example: 06:00:00
        // Example: 6:00:00
        // -----------------------------------------------------

        const sqlTimeMatch =
            value.match(
                /^(\d{1,2}):(\d{2})(?::\d{2})?$/
            );


        if (sqlTimeMatch) {

            const hour =
                parseInt(
                    sqlTimeMatch[1],
                    10
                );


            const minute =
                parseInt(
                    sqlTimeMatch[2],
                    10
                );


            if (
                isNaN(hour) ||
                isNaN(minute)
            ) {

                return "";
            }


            return (
                String(hour).padStart(2, "0") +
                ":" +
                String(minute).padStart(2, "0")
            );
        }


        // -----------------------------------------------------
        // AM / PM FORMAT
        // Example: 6:00 AM
        // Example: 06:00 AM
        // Example: 6:00 PM
        // -----------------------------------------------------

        const amPmMatch =
            value.match(
                /^(\d{1,2}):(\d{2})\s*(AM|PM)$/i
            );


        if (amPmMatch) {

            let hour =
                parseInt(
                    amPmMatch[1],
                    10
                );


            const minute =
                parseInt(
                    amPmMatch[2],
                    10
                );


            const period =
                amPmMatch[3]
                    .toUpperCase();


            if (
                isNaN(hour) ||
                isNaN(minute)
            ) {

                return "";
            }


            if (period === "AM") {

                if (hour === 12) {

                    hour = 0;
                }

            }
            else {

                if (hour !== 12) {

                    hour += 12;
                }
            }


            return (
                String(hour).padStart(2, "0") +
                ":" +
                String(minute).padStart(2, "0")
            );
        }


        // -----------------------------------------------------
        // FALLBACK
        // -----------------------------------------------------

        return value.substring(0, 5);
    }


    // =========================================================
    // FORMAT TIME
    // =========================================================

    function formatTime(time) {

        const normalized =
            normalizeTime(time);


        if (!normalized) {

            return "";
        }


        const parts =
            normalized.split(":");


        if (parts.length < 2) {

            return normalized;
        }


        let hour =
            parseInt(
                parts[0],
                10
            );


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
    // GET USED TIMES
    // =========================================================

    function getUsedTimes() {

        const used =
            new Set();


        // =====================================================
        // DATABASE TIMES
        // =====================================================

        if (
            Array.isArray(existingTimes)
        ) {

            existingTimes.forEach(
                function (time) {

                    const normalized =
                        normalizeTime(time);


                    if (normalized) {

                        used.add(
                            normalized
                        );
                    }

                }
            );
        }


        // =====================================================
        // TEMPORARY ROW TIMES
        // =====================================================

        temporaryBody
            .querySelectorAll(
                ".temporary-row"
            )
            .forEach(
                function (row) {

                    const time =
                        normalizeTime(
                            row.dataset.time
                        );


                    if (time) {

                        used.add(time);
                    }

                }
            );


        console.log(
            "Used boiler time slots:",
            Array.from(used)
        );


        return used;
    }


    // =========================================================
    // DAILY 24 HOURS
    // START FROM 06:00 AM
    // =========================================================

    function getDailySlots() {

        const slots = [];


        for (
            let i = 0;
            i < TOTAL_SLOTS;
            i++
        ) {

            const hour =
                (START_HOUR + i) % 24;


            slots.push(

                String(hour).padStart(2, "0") +
                ":00"

            );
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


        for (
            const slot of slots
        ) {

            if (
                !used.has(slot)
            ) {

                return slot;
            }
        }


        return null;
    }


    // =========================================================
    // SET NEXT AVAILABLE TIME
    // =========================================================

    function setNextAvailableTime() {

        const next =
            getFirstAvailableTime();


        nextAvailableTime =
            next || "";


        // =====================================================
        // ALL 24 SLOTS COMPLETED
        // =====================================================

        if (!next) {

            if (timeInput) {

                timeInput.value = "";
            }


            if (timeDisplayInput) {

                timeDisplayInput.value =
                    "Completed";
            }


            addButton.disabled =
                true;


            if (timeFullMessage) {

                timeFullMessage
                    .classList
                    .remove("d-none");
            }


            return;
        }


        // =====================================================
        // NEXT AVAILABLE SLOT
        // =====================================================

        if (timeInput) {

            timeInput.value =
                next;
        }


        if (timeDisplayInput) {

            timeDisplayInput.value =
                formatTime(next);
        }


        addButton.disabled =
            false;


        if (timeFullMessage) {

            timeFullMessage
                .classList
                .add("d-none");
        }


        console.log(
            "Next available boiler time:",
            next,
            formatTime(next)
        );
    }


    // =========================================================
    // LOAD AVAILABLE TIMES FROM DATABASE
    // =========================================================

    async function loadAvailableTimes(date) {

        if (!date) {

            existingTimes = [];

            setNextAvailableTime();

            return false;
        }


        if (!getAvailableTimesUrl) {

            console.error(
                "GetAvailableTimes URL is missing."
            );

            existingTimes = [];

            setNextAvailableTime();

            return false;
        }


        try {

            // -------------------------------------------------
            // SEND DATE + COMPANY
            // -------------------------------------------------

            const separator =
                getAvailableTimesUrl.includes("?")
                    ? "&"
                    : "?";


            const url =
                getAvailableTimesUrl +
                separator +
                "date=" +
                encodeURIComponent(date) +
                "&company=" +
                encodeURIComponent(
                    currentCompany
                );


            console.log(
                "Checking boiler existing times:",
                url
            );


            // -------------------------------------------------
            // FETCH
            // -------------------------------------------------

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


            // -------------------------------------------------
            // JSON
            // -------------------------------------------------

            const data =
                await response.json();


            console.log(
                "GetAvailableTimes response:",
                data
            );


            // -------------------------------------------------
            // RESPONSE CHECK
            // -------------------------------------------------

            if (!data.success) {

                existingTimes = [];

                alert(
                    data.message ||
                    "Could not load available times."
                );


                setNextAvailableTime();

                return false;
            }


            // -------------------------------------------------
            // EXISTING TIMES
            // -------------------------------------------------

            existingTimes =
                Array.isArray(
                    data.existingTimes
                )
                    ? data.existingTimes
                    : [];


            console.log(
                "Database existing boiler times:",
                existingTimes
            );


            // -------------------------------------------------
            // IMPORTANT
            // Recalculate AFTER database response
            // -------------------------------------------------

            setNextAvailableTime();


            return true;

        }
        catch (error) {

            console.error(
                "GetAvailableTimes error:",
                error
            );


            existingTimes = [];


            alert(
                "Could not check existing time slots."
            );


            setNextAvailableTime();


            return false;
        }
    }


    // =========================================================
    // DATE CHANGE
    // =========================================================

    if (dateInput) {

        dateInput.addEventListener(
            "change",
            async function () {

                const newDate =
                    dateInput.value;


                if (!newDate) {

                    return;
                }


                // ------------------------------------------------
                // CLEAR TEMPORARY DATA IF NEEDED
                // ------------------------------------------------

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


                    temporaryBody.innerHTML =
                        "";


                    hiddenContainer.innerHTML =
                        "";


                    rowCount = 0;
                }


                // ------------------------------------------------
                // LOAD DB TIMES
                // ------------------------------------------------

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
    }


    // =========================================================
    // CREATE NUMBER CELL
    // =========================================================

    function createNumberCell(
        field,
        value,
        readonly = false
    ) {

        return `
            <td>
                <input type="number"
                       step="any"
                       class="form-control form-control-sm editable-field ${readonly ? "total-generation" : ""}"
                       data-field="${escapeHtml(field)}"
                       value="${escapeHtml(value)}"
                       ${readonly ? "readonly tabindex='-1'" : ""}
                       autocomplete="off">
            </td>
        `;
    }


    // =========================================================
    // CREATE FUEL CELL
    // =========================================================

    function createFuelCell(
        field,
        value
    ) {

        let html = `

            <td>

                <select
                    class="form-select form-select-sm editable-field"
                    data-field="${escapeHtml(field)}">

                    <option value="">
                        Select
                    </option>
        `;


        fuelOptions.forEach(
            function (fuel) {

                const selected =
                    fuel === value
                        ? "selected"
                        : "";


                html += `

                    <option value="${escapeHtml(fuel)}"
                            ${selected}>

                        ${escapeHtml(fuel)}

                    </option>

                `;
            }
        );


        html += `

                </select>

            </td>

        `;


        return html;
    }


    // =========================================================
    // ADD ROW
    // =========================================================

    addButton.addEventListener(
        "click",
        function () {

            // -------------------------------------------------
            // CALCULATE TOTAL
            // -------------------------------------------------

            const totalGeneration =
                calculateTotalGeneration();


            // -------------------------------------------------
            // GET NEXT AVAILABLE TIME
            // -------------------------------------------------

            const currentTime =
                getFirstAvailableTime();


            // -------------------------------------------------
            // NO SLOT AVAILABLE
            // -------------------------------------------------

            if (!currentTime) {

                setNextAvailableTime();

                return;
            }


            // -------------------------------------------------
            // BASIC VALUES
            // -------------------------------------------------

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


            // -------------------------------------------------
            // GET DATA VALUES
            // -------------------------------------------------

            const values = {};


            dataFields.forEach(
                function (field) {

                    values[field] =
                        getInputValue(
                            "input" + field
                        );

                }
            );


            // -------------------------------------------------
            // FORCE CALCULATED TOTAL
            // -------------------------------------------------

            values.TotalGeneration =
                totalGeneration;


            // =================================================
            // CREATE TEMPORARY ROW
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

                <!-- SL -->

                <td class="text-center fw-bold">

                    ${rowCount + 1}

                </td>


                <!-- COMPANY -->

                <td>

                    <input type="text"
                           class="form-control form-control-sm"
                           value="${escapeHtml(company)}"
                           readonly>

                </td>


                <!-- DATE -->

                <td>

                    <input type="date"
                           class="form-control form-control-sm"
                           value="${escapeHtml(date)}"
                           readonly>

                </td>


                <!-- TIME -->

                <td>

                    <input type="text"
                           class="form-control form-control-sm time-display"
                           value="${escapeHtml(formatTime(time))}"
                           readonly>

                </td>


                <!-- GAS PRESSURE -->

                ${createNumberCell(
                "GasPressure",
                values.GasPressure
            )}


                <!-- HEADER STEAM PRESSURE -->

                ${createNumberCell(
                "HeaderSteamPressure",
                values.HeaderSteamPressure
            )}


                <!-- BOILER 1 -->

                ${createNumberCell(
                "Boiler1SteamGeneration",
                values.Boiler1SteamGeneration
            )}


                <!-- B1 FUEL -->

                ${createFuelCell(
                "B1UsageFuel",
                values.B1UsageFuel
            )}


                <!-- BOILER 2 -->

                ${createNumberCell(
                "Boiler2SteamGeneration",
                values.Boiler2SteamGeneration
            )}


                <!-- B2 FUEL -->

                ${createFuelCell(
                "B2UsageFuel",
                values.B2UsageFuel
            )}


                <!-- BOILER 3 -->

                ${createNumberCell(
                "Boiler3SteamGeneration",
                values.Boiler3SteamGeneration
            )}


                <!-- B3 FUEL -->

                ${createFuelCell(
                "B3UsageFuel",
                values.B3UsageFuel
            )}


                <!-- EGB BOILER -->

                ${createNumberCell(
                "EgbBoilerSteamGeneration",
                values.EgbBoilerSteamGeneration
            )}


                <!-- TOTAL -->

                ${createNumberCell(
                "TotalGeneration",
                values.TotalGeneration,
                true
            )}

            `;


            temporaryBody.appendChild(
                row
            );


            // =================================================
            // CREATE HIDDEN INPUTS
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


            dataFields.forEach(
                function (field) {

                    createHiddenInput(
                        rowCount,
                        field,
                        values[field]
                    );

                }
            );


            // =================================================
            // ROW EVENTS
            // =================================================

            setupRowEvents(row);


            rowCount++;


            // =================================================
            // RESET INPUT ROW
            // =================================================

            resetInputRow();

        }
    );


    // =========================================================
    // SETUP TEMPORARY ROW EVENTS
    // =========================================================

    function setupRowEvents(row) {

        const fields =
            row.querySelectorAll(
                ".editable-field"
            );


        fields.forEach(
            function (input) {

                input.addEventListener(
                    "input",
                    function () {

                        updateTemporaryRow(
                            row
                        );

                    }
                );


                input.addEventListener(
                    "change",
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


        // -----------------------------------------------------
        // GET GENERATION VALUES
        // -----------------------------------------------------

        const b1 =
            parseFloat(
                getRowValue(
                    row,
                    "Boiler1SteamGeneration"
                )
            ) || 0;


        const b2 =
            parseFloat(
                getRowValue(
                    row,
                    "Boiler2SteamGeneration"
                )
            ) || 0;


        const b3 =
            parseFloat(
                getRowValue(
                    row,
                    "Boiler3SteamGeneration"
                )
            ) || 0;


        const egb =
            parseFloat(
                getRowValue(
                    row,
                    "EgbBoilerSteamGeneration"
                )
            ) || 0;


        // -----------------------------------------------------
        // TOTAL
        // -----------------------------------------------------

        const total =
            b1 +
            b2 +
            b3 +
            egb;


        // -----------------------------------------------------
        // UPDATE TOTAL CELL
        // -----------------------------------------------------

        const totalInput =
            row.querySelector(
                '[data-field="TotalGeneration"]'
            );


        if (totalInput) {

            totalInput.value =
                total.toFixed(2);
        }


        // -----------------------------------------------------
        // UPDATE HIDDEN INPUTS
        // -----------------------------------------------------

        dataFields.forEach(
            function (field) {

                let value =
                    getRowValue(
                        row,
                        field
                    );


                if (
                    field ===
                    "TotalGeneration"
                ) {

                    value =
                        total.toFixed(2);
                }


                updateHiddenInput(
                    index,
                    field,
                    value
                );

            }
        );
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


        input.type =
            "hidden";


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

        // -----------------------------------------------------
        // COMPANY
        // -----------------------------------------------------

        const companyInput =
            document.getElementById(
                "inputCompany"
            );


        if (companyInput) {

            companyInput.value =
                currentCompany;
        }


        // -----------------------------------------------------
        // NUMERIC FIELDS
        // -----------------------------------------------------

        numericFields.forEach(
            function (field) {

                const element =
                    document.getElementById(
                        "input" + field
                    );


                if (element) {

                    element.value =
                        "";
                }

            }
        );


        // -----------------------------------------------------
        // FUEL DROPDOWNS
        // -----------------------------------------------------

        fuelFields.forEach(
            function (field) {

                const element =
                    document.getElementById(
                        "input" + field
                    );


                if (element) {

                    element.value =
                        "";
                }

            }
        );


        // -----------------------------------------------------
        // SET NEXT AVAILABLE TIME
        // -----------------------------------------------------

        setNextAvailableTime();


        // -----------------------------------------------------
        // FOCUS
        // -----------------------------------------------------

        const gasPressure =
            document.getElementById(
                "inputGasPressure"
            );


        if (gasPressure) {

            gasPressure.focus();
        }
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
            .replace(
                /&/g,
                "&amp;"
            )
            .replace(
                /</g,
                "&lt;"
            )
            .replace(
                />/g,
                "&gt;"
            )
            .replace(
                /"/g,
                "&quot;"
            )
            .replace(
                /'/g,
                "&#039;"
            );
    }


    // =========================================================
    // FORM SUBMIT
    // =========================================================

    form.addEventListener(
        "submit",
        function (event) {

            // -------------------------------------------------
            // NO DATA
            // -------------------------------------------------

            if (rowCount === 0) {

                event.preventDefault();


                alert(
                    "Please add at least one reading before saving."
                );


                return;
            }


            // -------------------------------------------------
            // FINAL TOTAL UPDATE
            // -------------------------------------------------

            temporaryBody
                .querySelectorAll(
                    ".temporary-row"
                )
                .forEach(
                    function (row) {

                        updateTemporaryRow(
                            row
                        );

                    }
                );


            // -------------------------------------------------
            // DUPLICATE TIME CHECK
            // -------------------------------------------------

            const times = [];


            temporaryBody
                .querySelectorAll(
                    ".temporary-row"
                )
                .forEach(
                    function (row) {

                        const time =
                            normalizeTime(
                                row.dataset.time
                            );


                        if (time) {

                            times.push(time);
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


            // -------------------------------------------------
            // CHECK AGAINST DATABASE
            // -------------------------------------------------

            const databaseTimes =
                getUsedTimes();


            let duplicateDatabaseTime =
                null;


            for (
                const time of times
            ) {

                if (
                    existingTimes.some(
                        function (existingTime) {

                            return normalizeTime(
                                existingTime
                            ) === time;

                        }
                    )
                ) {

                    duplicateDatabaseTime =
                        time;

                    break;
                }
            }


            if (duplicateDatabaseTime) {

                event.preventDefault();


                alert(
                    "The time slot " +
                    formatTime(
                        duplicateDatabaseTime
                    ) +
                    " has already been saved."
                );


                return;
            }


            // -------------------------------------------------
            // HIDDEN DATA CHECK
            // -------------------------------------------------

            const hiddenInputs =
                hiddenContainer.querySelectorAll(
                    'input[name^="Items["]'
                );


            if (
                hiddenInputs.length === 0
            ) {

                event.preventDefault();


                alert(
                    "No readings were prepared for saving."
                );


                return;
            }


            // =================================================
            // SHOW SAVING OVERLAY
            // =================================================

            event.preventDefault();


            if (saveAllButton) {

                saveAllButton.disabled =
                    true;
            }


            if (savingOverlay) {

                savingOverlay
                    .classList
                    .remove("d-none");


                savingOverlay
                    .classList
                    .add("show");
            }


            // -------------------------------------------------
            // SUBMIT
            // -------------------------------------------------

            setTimeout(
                function () {

                    form.submit();

                },
                100
            );

        }
    );


    // =========================================================
    // HIDE SAVING OVERLAY
    // =========================================================

    if (savingOverlay) {

        savingOverlay
            .classList
            .add("d-none");


        savingOverlay
            .classList
            .remove("show");
    }


    // =========================================================
    // GENERATION INPUT EVENTS
    // =========================================================

    const generationInputs = [

        "inputBoiler1SteamGeneration",

        "inputBoiler2SteamGeneration",

        "inputBoiler3SteamGeneration",

        "inputEgbBoilerSteamGeneration"

    ];


    generationInputs.forEach(
        function (id) {

            const input =
                document.getElementById(id);


            if (input) {

                input.addEventListener(
                    "input",
                    calculateTotalGeneration
                );

            }

        }
    );


    // =========================================================
    // INITIALIZE DATE
    // =========================================================

    if (dateInput) {

        previousDate =
            dateInput.value;
    }


    // =========================================================
    // LOAD EXISTING DATABASE TIMES
    // =========================================================

    if (
        dateInput &&
        dateInput.value
    ) {

        await loadAvailableTimes(
            dateInput.value
        );

    }
    else {

        setNextAvailableTime();
    }


    // =========================================================
    // INITIAL TOTAL
    // =========================================================

    calculateTotalGeneration();


    // =========================================================
    // DEBUG INFORMATION
    // =========================================================

    console.log(
        "Boiler Steam Generation initialized."
    );

    console.log(
        "Company:",
        currentCompany
    );

    console.log(
        "Date:",
        dateInput
            ? dateInput.value
            : ""
    );

    console.log(
        "Existing times:",
        existingTimes
    );

    console.log(
        "Next available:",
        nextAvailableTime
    );

});
