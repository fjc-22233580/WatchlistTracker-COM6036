document.addEventListener("DOMContentLoaded", () => {
    const manualToggle = document.getElementById("manualEntryToggle");
    const tmdbSection = document.getElementById("tmdbSection");
    const manualTitleSection = document.getElementById("manualTitleSection");
    const selectedMovieSection = document.getElementById("selectedMovieSection");
    const titleInput = document.getElementById("Input_Title");
    const selectedTmdbId = document.getElementById("SelectedTmdbId");

    if (
        !manualToggle ||
        !tmdbSection ||
        !manualTitleSection ||
        !selectedMovieSection ||
        !titleInput ||
        !selectedTmdbId
    ) {
        return;
    }

    function updateEntryMode() {
        const manual = manualToggle.checked;

        tmdbSection.style.display = manual ? "none" : "block";
        manualTitleSection.style.display = manual ? "block" : "none";
        selectedMovieSection.style.display = manual ? "none" : "block";

        if (manual) {
            selectedTmdbId.value = "";
            titleInput.value = "";
            titleInput.readOnly = false;
        }
    }

    manualToggle.addEventListener("change", updateEntryMode);

    updateEntryMode();
});