function timeOutCall(dotnethelper) {
    document.onmousedown = resetTimeDelay;
    document.onkeypress = resetTimeDelay;

    function resetTimeDelay() {
        dotnethelper.invokeMethodAsync("TimerInterval");
    }
}
