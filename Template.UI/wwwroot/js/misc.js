window.setDefaultButton = (button) => {
    console.log('called');
    button.addEventListener("keyup", function (event) {
        if (event.key === "Enter") {
            console.log('enter seen');
            button.click();
        }
    });
};