function removeInlineStyle(selector) {
    const elements = document.querySelectorAll(selector);
    elements.forEach(element => {
        element.removeAttribute("style");
    });
}