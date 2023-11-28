// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function toggleText() {
    var x = document.getElementById("toggle");
    console.log(x.innerHTML);
    if (x.innerHTML === "Show Document") {
        x.innerHTML = "Hide Document";
    } else {
        x.innerHTML = "Show Document";
    }
}
