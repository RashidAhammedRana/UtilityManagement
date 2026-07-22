
    document.addEventListener("DOMContentLoaded", function () {

        const steps = [
    document.getElementById("step1"),
    document.getElementById("step2"),
    document.getElementById("step3"),
    document.getElementById("step4")
    ];

    const circles = [
    document.getElementById("stepCircle1"),
    document.getElementById("stepCircle2"),
    document.getElementById("stepCircle3"),
    document.getElementById("stepCircle4")
    ];

    const lines = [
    document.getElementById("stepLine1"),
    document.getElementById("stepLine2"),
    document.getElementById("stepLine3")
    ];

    let currentStep = 0;

    function showStep(index) {

        steps.forEach((step, i) => {
            step.classList.toggle("active", i === index);
        });

            circles.forEach((circle, i) => {
        circle.classList.toggle("active", i <= index);
            });

            lines.forEach((line, i) => {
        line.classList.toggle("active", i < index);
            });

    window.scrollTo({
        top: 0,
    behavior: "smooth"
            });
        }

    // Step 1 -> Step 2
    document.getElementById("nextBtn1")?.addEventListener("click", function () {
        currentStep = 1;
    showStep(currentStep);
        });

    // Step 2 -> Step 3
    document.getElementById("nextBtn2")?.addEventListener("click", function () {
        currentStep = 2;
    showStep(currentStep);
        });

    // Step 3 -> Step 4
    document.getElementById("nextBtn3")?.addEventListener("click", function () {
        currentStep = 3;
    showStep(currentStep);
        });

        // Step 2 <- Step 1
    document.getElementById("previousBtn2")?.addEventListener("click", function () {
        currentStep = 0;
    showStep(currentStep);
        });

        // Step 3 <- Step 2
    document.getElementById("previousBtn3")?.addEventListener("click", function () {
        currentStep = 1;
    showStep(currentStep);
        });

        // Step 4 <- Step 3
    document.getElementById("previousBtn4")?.addEventListener("click", function () {
        currentStep = 2;
    showStep(currentStep);
        });

    });
    //TotalCalculation
    function getValue(id) {
        return parseFloat(document.getElementById(id)?.value) || 0;
    }


    // Raw Total
    function calculateRawTotal() {

        let total =
    getValue("RawConsGarments") +
    getValue("RawConsDyeing") +
    getValue("RawConsDyeingFin") +
    getValue("RawConsPrinting") +
    getValue("RawConsUtilityArea") +
    getValue("RawConsWashing") +
    getValue("RawConsSeamlessDyeing") +
    getValue("RawConsLab") +
    getValue("RawConsGardening") +
    getValue("RawConsWashroomOthers") +
    getValue("RawConsKnittingArea") +
    getValue("RawConsOthersArea") +
    getValue("RawConsBackWash");


    document.getElementById("RawConsTotal").value = total.toFixed(2);
    }


    // Soft Total
    function calculateSoftTotal() {

        let total =
    getValue("SoftConsDyeing") +
    getValue("SoftConsDyeingFin") +
    getValue("SoftConsWashing") +
    getValue("SoftConsSeamlessDyeing") +
    getValue("SoftConsLab") +
    getValue("SoftConsOthersArea");


    document.getElementById("SoftConsTotal").value = total.toFixed(2);
    }


    // RO Total
    function calculateRoTotal() {

        let total =
    getValue("RoConsDyeing") +
    getValue("RoConsDyeingFin") +
    getValue("RoConsWashing") +
    getValue("RoConsSeamlessDyeing") +
    getValue("RoConsLab") +
    getValue("RoConsOthersArea");


    document.getElementById("RoConsTotal").value = total.toFixed(2);
    }


    // Hot Total
    function calculateHotTotal() {

        let total =
    getValue("HotConsDyeing") +
    getValue("HotConsDyeingFin") +
    getValue("HotConsWashing") +
    getValue("HotConsSeamlessDyeing") +
    getValue("HotConsLab") +
    getValue("HotConsOthersArea")
    -
    getValue("HotWaterReturn");


    document.getElementById("HotConsTotal").value = total.toFixed(2);
    }


    // Auto Calculate
    document.addEventListener("input", function(e) {

        let id = e.target.id;


    if(id.startsWith("RawCons")) {
        calculateRawTotal();
        }


    if(id.startsWith("SoftCons")) {
        calculateSoftTotal();
        }


    if(id.startsWith("RoCons")) {
        calculateRoTotal();
        }


    if(id.startsWith("HotCons") || id === "HotWaterReturn") {
        calculateHotTotal();
        }

    });