function removeInlineStyle(selector) {
    const elements = document.querySelectorAll(selector);
    elements.forEach(element => {
        element.removeAttribute("style");
    });
}
function hideToolbar(selector) {
    console.log("VS-- inside hideToolbar");
    var toolbars = document.querySelectorAll(selector);
    toolbars.forEach(function (toolbar) {
        toolbar.style.display = 'none';
    });
}