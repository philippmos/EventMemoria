window.addKeyboardListener = (dotNetHelper) => {
    const handler = (event) => {
        dotNetHelper.invokeMethodAsync('HandleKeyPress', event.key);
    };
    
    document.addEventListener('keydown', handler);
    
    // Cleanup function when dialog closes
    window.removeKeyboardListener = () => {
        document.removeEventListener('keydown', handler);
        window.removeKeyboardListener = null;
    };
};
