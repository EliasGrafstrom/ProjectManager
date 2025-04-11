document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector("#addProjectModal form");

    if (!form) return;

    form.addEventListener("submit", function (e) {
        e.preventDefault();

        const formData = new FormData(form);

        fetch(form.action, {
            method: "POST",
            body: formData
        })
            .then(response => {
                if (!response.ok) throw new Error("Failed to submit");
                return response.text();
            })
            .then(() => {
                const modal = document.getElementById("addProjectModal");
                if (modal) modal.classList.remove("show");
                location.reload();
            })
            .catch(error => {
                console.error("Error submitting form:", error);
                alert("Something went wrong.");
            });
    });
});
