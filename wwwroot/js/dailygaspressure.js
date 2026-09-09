document.addEventListener("DOMContentLoaded", async function () {

    // =========================================================
    // CONFIGURATION
    // =========================================================

    const START_HOUR = 6;
    const TOTAL_SLOTS = 24;


    // =========================================================
    // SERVER CONFIGURATION
    // =========================================================

    const config = window.gasPressureConfig || {};

    const currentCompany =
        String(config.currentCompany || "").trim();

    const getAvailableTimesUrl =
        String(config.getAvailableTimesUrl || "").trim();


    // =========================================================
    // DATABASE TIMES
    //
    // IMPORTANT:
    // These are ONLY TblDailyGasPressureRecord times
    // returned from GetAvailableTimes().
    // =========================================================

    let existingTimes = [];


    // =========================================================
    // ELEMENTS
    // =========================================================

    const form =
        document.getElementById("gasPressureForm");

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
        !temporaryBody
    ) {

        console.error(
            "Daily Gas Pressure elements were not found."
        );

        return;
    }


    // =========================================================
    // FORCE SAVING OVERLAY HIDDEN ON PAGE LOAD
    // =========================================================

    function hideSavingOverlay() {

        if (!savingOverlay) {
            return;
        }

        savingOverlay.classList.remove("show");

        savingOverlay.style.display = "none";

        savingOverlay.style.position = "fixed";
        savingOverlay.style.inset = "0";
        savingOverlay.style.width = "100vw";
        savingOverlay.style.height = "100vh";
        savingOverlay.style.alignItems = "center";
        savingOverlay.style.justifyContent = "center";
        savingOverlay.style.zIndex = "99999";
    }


    hideSavingOverlay();


    // =========================================================
    // PRESSURE FIELDS
    // =========================================================

    const pressureFields = [
        "GpBefore",
        "GpIr",
        "GpCr"
    ];


    // =========================================================
    // ROW COUNT
    // =========================================================

    let rowCount = 0;


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
    // NORMALIZE TIME
    //
    // Supported:
    //
    // 6:00
    // 06:00
    // 6:00:00
    // 06:00:00
    // 2026-09-09T06:00:00
    // =========================================================

    function normalizeTime(time) {

        if (
            time === null ||
            time === undefined
        ) {
            return "";
        }

        let value =
            String(time).trim();

        if (!value) {
            return "";
        }


        // -----------------------------------------------------
        // ISO date-time
        // -----------------------------------------------------

        if (value.includes("T")) {

            value =
                value.split("T")[1];
        }


        // -----------------------------------------------------
        // Remove trailing timezone if any
        // -----------------------------------------------------

        value =
            value.split("Z")[0];


        // -----------------------------------------------------
        // Time parts
        // -----------------------------------------------------

        const parts =
            value.split(":");


        if (parts.length >= 2) {

            const hour =
                parseInt(parts[0], 10);

            const minute =
                parseInt(parts[1], 10);


            if (
                !Number.isNaN(hour) &&
                !Number.isNaN(minute) &&
                hour >= 0 &&
                hour <= 23 &&
                minute >= 0 &&
                minute <= 59
            ) {

                return (
                    String(hour).padStart(2, "0") +
                    ":" +
                    String(minute).padStart(2, "0")
                );
            }
        }


        return value.substring(0, 5);
    }


    // =========================================================
    // TIME -> MINUTES
    // =========================================================

    function timeToMinutes(time) {

        const normalized =
            normalizeTime(time);

        if (!normalized) {
            return null;
        }


        const parts =
            normalized.split(":");


        if (parts.length < 2) {
            return null;
        }


        const hour =
            parseInt(parts[0], 10);

        const minute =
            parseInt(parts[1], 10);


        if (
            Number.isNaN(hour) ||
            Number.isNaN(minute)
        ) {
            return null;
        }


        return (
            hour * 60 +
            minute
        );
    }


    // =========================================================
    // MINUTES -> TIME
    // =========================================================

    function minutesToTime(totalMinutes) {

        const minutes =
            (
                totalMinutes %
                (24 * 60) +
                (24 * 60)
            ) %
            (24 * 60);


        const hour =
            Math.floor(minutes / 60);

        const minute =
            minutes % 60;


        return (
            String(hour).padStart(2, "0") +
            ":" +
            String(minute).padStart(2, "0")
        );
    }


    // =========================================================
    // GET DAILY SLOTS
    //
    // 06:00
    // 07:00
    // 08:00
    // ...
    // 23:00
    // 00:00
    // ...
    // 05:00
    // =========================================================

    function getDailySlots() {

        const slots = [];


        for (
            let i = 0;
            i < TOTAL_SLOTS;
            i++
        ) {

            const minutes =
                (
                    START_HOUR * 60
                ) +
                (
                    i * 60
                );


            slots.push(
                minutesToTime(minutes)
            );
        }


        return slots;
    }


    // =========================================================
    // GET DATABASE TIMES
    //
    // ONLY server/database times.
    // =========================================================

    function getDatabaseTimes() {

        const used =
            new Set();


        if (!Array.isArray(existingTimes)) {
            return used;
        }


        existingTimes.forEach(function (time) {

            const normalized =
                normalizeTime(time);


            if (normalized) {

                used.add(
                    normalized
                );
            }
        });


        return used;
    }


    // =========================================================
    // GET TEMPORARY TIMES
    //
    // Rows added but not saved yet.
    // =========================================================

    function getTemporaryTimes() {

        const used =
            new Set();


        temporaryBody
            .querySelectorAll(".temporary-row")
            .forEach(function (row) {

                const time =
                    normalizeTime(
                        row.dataset.time
                    );


                if (time) {

                    used.add(
                        time
                    );
                }
            });


        return used;
    }


    // =========================================================
    // GET ALL USED TIMES
    //
    // DATABASE + TEMPORARY
    // =========================================================

    function getUsedTimes() {

        const used =
            getDatabaseTimes();


        const temporary =
            getTemporaryTimes();


        temporary.forEach(function (time) {

            used.add(time);

        });


        return used;
    }


    // =========================================================
    // GET FIRST AVAILABLE TIME
    //
    // THIS IS THE ONLY SOURCE OF NEXT TIME.
    //
    // Empty DB:
    //     06:00
    //
    // DB empty + temporary 06:
    //     07:00
    //
    // DB has 06 + temporary 07:
    //     08:00
    // =========================================================

    function getFirstAvailableTime() {

        const used =
            getUsedTimes();


        const slots =
            getDailySlots();


        for (
            let i = 0;
            i < slots.length;
            i++
        ) {

            const slot =
                slots[i];


            if (!used.has(slot)) {

                return slot;
            }
        }


        return null;
    }


    // =========================================================
    // FORMAT TIME
    //
    // 06:00 -> 06:00 AM
    // 07:00 -> 07:00 AM
    // 12:00 -> 12:00 PM
    // 13:00 -> 01:00 PM
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


        const hour =
            parseInt(parts[0], 10);

        const minute =
            parts[1];


        if (Number.isNaN(hour)) {
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


        // -----------------------------------------------------
        // ALL 24 SLOTS USED
        // -----------------------------------------------------

        if (!next) {

            if (timeInput) {

                timeInput.value =
                    "";
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


        // -----------------------------------------------------
        // NEXT AVAILABLE SLOT
        // -----------------------------------------------------

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
    }


    // =========================================================
    // CLEAR TEMPORARY ROWS
    // =========================================================

    function clearTemporaryRows() {

        temporaryBody.innerHTML =
            "";


        if (hiddenContainer) {

            hiddenContainer.innerHTML =
                "";
        }


        rowCount =
            0;


        setNextAvailableTime();
    }


    // =========================================================
    // LOAD AVAILABLE TIMES
    //
    // Backend endpoint MUST query:
    //
    // TblDailyGasPressureRecord
    //
    // NOT TblDailyEnergyFuelConsumption.
    // =========================================================

    async function loadAvailableTimes(date) {

        if (!date) {
            return false;
        }


        if (!getAvailableTimesUrl) {

            console.error(
                "GetAvailableTimes URL is missing."
            );

            // No server data.
            // Start from 06:00.
            existingTimes = [];

            setNextAvailableTime();

            return true;
        }


        try {

            const separator =
                getAvailableTimesUrl.includes("?")
                    ? "&"
                    : "?";


            const url =
                getAvailableTimesUrl +
                separator +
                "date=" +
                encodeURIComponent(date);


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


            if (!data.success) {

                console.error(
                    "GetAvailableTimes failed:",
                    data.message
                );


                existingTimes = [];

                setNextAvailableTime();

                return false;
            }


            // -------------------------------------------------
            // IMPORTANT
            //
            // Replace existingTimes.
            // NEVER append old date's times.
            // -------------------------------------------------

            existingTimes =
                Array.isArray(data.existingTimes)
                    ? data.existingTimes
                        .map(function (time) {
                            return normalizeTime(time);
                        })
                        .filter(function (time) {
                            return time !== "";
                        })
                    : [];


            // -------------------------------------------------
            // DO NOT TRUST data.nextTime FOR FRONTEND STATE.
            //
            // Calculate from:
            //
            // TblDailyGasPressureRecord times
            // +
            // temporary rows
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

            setNextAvailableTime();


            alert(
                "Could not check existing time slots."
            );


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


                // -------------------------------------------------
                // Temporary rows exist
                // -------------------------------------------------

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


                // -------------------------------------------------
                // Load database times for NEW date
                // -------------------------------------------------

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

    function createEditableNumberCell(
        field,
        value
    ) {

        return `
            <td>
                <input
                    type="number"
                    step="any"
                    class="form-control form-control-sm editable-field"
                    data-field="${escapeHtml(field)}"
                    value="${escapeHtml(value)}"
                    autocomplete="off">
            </td>
        `;
    }


    // =========================================================
    // CREATE REMARKS CELL
    // =========================================================

    function createEditableRemarksCell(
        value
    ) {

        return `
            <td>
                <input
                    type="text"
                    class="form-control form-control-sm editable-field"
                    data-field="Remarks"
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
        function (event) {

            // -------------------------------------------------
            // VERY IMPORTANT
            // Add button must NEVER submit the form.
            // -------------------------------------------------

            event.preventDefault();


            // -------------------------------------------------
            // Get next time BEFORE adding row.
            // -------------------------------------------------

            const currentTime =
                getFirstAvailableTime();


            // -------------------------------------------------
            // No available slot
            // -------------------------------------------------

            if (!currentTime) {

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
                normalizeTime(
                    currentTime
                );


            const timeDisplay =
                formatTime(time);


            // =================================================
            // GET VALUES
            // =================================================

            const values = {};


            pressureFields.forEach(
                function (field) {

                    values[field] =
                        getInputValue(
                            "input" + field
                        );
                }
            );


            values.Remarks =
                getInputValue(
                    "inputRemarks"
                );


            // =================================================
            // CREATE ROW
            // =================================================

            const row =
                document.createElement("tr");


            row.className =
                "temporary-row";


            row.dataset.index =
                String(rowCount);


            // -------------------------------------------------
            // CRITICAL
            //
            // Store exact selected time.
            //
            // 1st = 06:00
            // 2nd = 07:00
            // 3rd = 08:00
            // -------------------------------------------------

            row.dataset.time =
                time;


            row.innerHTML = `

                <td>
                    <input
                        type="text"
                        class="form-control form-control-sm"
                        value="${escapeHtml(company)}"
                        readonly>
                </td>

                <td>
                    <input
                        type="date"
                        class="form-control form-control-sm"
                        value="${escapeHtml(date)}"
                        readonly>
                </td>

                <td>
                    <input
                        type="text"
                        class="form-control form-control-sm time-display"
                        value="${escapeHtml(timeDisplay)}"
                        readonly>
                </td>

                ${createEditableNumberCell(
                "GpBefore",
                values.GpBefore
            )}

                ${createEditableNumberCell(
                "GpIr",
                values.GpIr
            )}

                ${createEditableNumberCell(
                "GpCr",
                values.GpCr
            )}

                ${createEditableRemarksCell(
                values.Remarks
            )}
            `;


            temporaryBody.appendChild(
                row
            );


            // =================================================
            // HIDDEN MODEL BINDING
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


            pressureFields.forEach(
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
                "Remarks",
                values.Remarks
            );


            // =================================================
            // ROW EVENTS
            // =================================================

            setupRowEvents(row);


            // =================================================
            // INCREMENT ROW COUNT
            // =================================================

            rowCount++;


            // =================================================
            // RESET ENTRY AREA
            //
            // IMPORTANT:
            //
            // Existing DB = none
            // Temporary = 06
            //
            // Next = 07
            // =================================================

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


        if (Number.isNaN(index)) {
            return;
        }


        pressureFields.forEach(
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
            "Remarks",
            getRowValue(
                row,
                "Remarks"
            )
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

        if (!hiddenContainer) {
            return;
        }


        const input =
            document.createElement("input");


        input.type =
            "hidden";


        input.name =
            `Items[${index}].${field}`;


        input.value =
            value ?? "";


        input.dataset.index =
            String(index);


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

        if (!hiddenContainer) {
            return;
        }


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
    // RESET ENTRY INPUTS
    // =========================================================

    function resetInputRow() {

        // -----------------------------------------------------
        // Company
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
        // Calculate NEXT time.
        //
        // This happens AFTER row was inserted.
        //
        // Example:
        //
        // Before add:
        //     DB = empty
        //     Temporary = empty
        //     NEXT = 06:00
        //
        // After add:
        //     Temporary = 06:00
        //     NEXT = 07:00
        // -----------------------------------------------------

        setNextAvailableTime();


        // -----------------------------------------------------
        // Clear pressure inputs
        // -----------------------------------------------------

        pressureFields.forEach(
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
        // Clear remarks
        // -----------------------------------------------------

        const remarksInput =
            document.getElementById(
                "inputRemarks"
            );


        if (remarksInput) {

            remarksInput.value =
                "";
        }


        // -----------------------------------------------------
        // Focus first field
        // -----------------------------------------------------

        const firstPressureInput =
            document.getElementById(
                "inputGpBefore"
            );


        if (firstPressureInput) {

            firstPressureInput.focus();
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
    // SHOW SAVING OVERLAY
    //
    // ONLY FORM SUBMIT CAN CALL THIS.
    // =========================================================

    function showSavingOverlay() {

        if (!savingOverlay) {
            return;
        }


        // -----------------------------------------------------
        // Center overlay
        // -----------------------------------------------------

        savingOverlay.classList.add("show");

        savingOverlay.style.display =
            "flex";

        savingOverlay.style.position =
            "fixed";

        savingOverlay.style.inset =
            "0";

        savingOverlay.style.width =
            "100vw";

        savingOverlay.style.height =
            "100vh";

        savingOverlay.style.alignItems =
            "center";

        savingOverlay.style.justifyContent =
            "center";

        savingOverlay.style.zIndex =
            "99999";


        // -----------------------------------------------------
        // Make child content centered as well
        // -----------------------------------------------------

        const overlayContent =
            savingOverlay.firstElementChild;


        if (overlayContent) {

            overlayContent.style.position =
                "relative";

            overlayContent.style.left =
                "auto";

            overlayContent.style.right =
                "auto";

            overlayContent.style.top =
                "auto";

            overlayContent.style.bottom =
                "auto";

            overlayContent.style.transform =
                "none";

            overlayContent.style.margin =
                "0 auto";
        }
    }


    // =========================================================
    // FORM SUBMIT
    // =========================================================

    form.addEventListener(
        "submit",
        function (event) {

            // -------------------------------------------------
            // No rows
            // -------------------------------------------------

            if (rowCount === 0) {

                event.preventDefault();

                alert(
                    "Please add at least one reading before saving."
                );

                return;
            }


            // -------------------------------------------------
            // CHECK TEMPORARY TIMES
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

                            times.push(
                                time
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


            // -------------------------------------------------
            // CHECK HIDDEN ITEMS
            // -------------------------------------------------

            const hiddenRows =
                hiddenContainer
                    ? hiddenContainer.querySelectorAll(
                        'input[name^="Items["]'
                    )
                    : [];


            if (hiddenRows.length === 0) {

                event.preventDefault();

                alert(
                    "No readings were prepared for saving."
                );

                return;
            }


            // =================================================
            // VALID SUBMIT
            //
            // ONLY HERE SHOW WAITING.
            // =================================================

            if (saveAllButton) {

                saveAllButton.disabled =
                    true;
            }


            showSavingOverlay();


            // -------------------------------------------------
            // DO NOT preventDefault()
            //
            // Normal MVC POST continues.
            // -------------------------------------------------
        }
    );


    // =========================================================
    // INITIALIZATION
    // =========================================================

    previousDate =
        dateInput
            ? dateInput.value
            : "";


    // ---------------------------------------------------------
    // Load database times for current date
    // ---------------------------------------------------------

    if (
        dateInput &&
        dateInput.value
    ) {

        await loadAvailableTimes(
            dateInput.value
        );
    }
    else {

        // No date:
        // no database times
        // first slot = 06:00

        existingTimes = [];

        setNextAvailableTime();
    }


    // ---------------------------------------------------------
    // Final calculation
    // ---------------------------------------------------------

    setNextAvailableTime();


    // ---------------------------------------------------------
    // Final overlay safety
    //
    // Initialization NEVER leaves overlay visible.
    // ---------------------------------------------------------

    hideSavingOverlay();


    console.log(
        "Daily Gas Pressure JS initialized."
    );

    console.log(
        "Start hour:",
        START_HOUR
    );

    console.log(
        "Database times:",
        existingTimes
    );

    console.log(
        "Next available:",
        getFirstAvailableTime()
    );

});
