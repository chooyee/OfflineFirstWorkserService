function importScripts(scriptUrls, callback) {
    var loadedScripts = 0;

    function scriptLoaded() {
        loadedScripts++;
        if (loadedScripts === scriptUrls.length) {
            callback();
        }
    }

    for (var i = 0; i < scriptUrls.length; i++) {
        var script = document.createElement('script');
        script.src = scriptUrls[i];
        script.onload = scriptLoaded;
        document.head.appendChild(script);
    }
}

// Usage example:
const scriptsToImport = [
    'js/components/base.js',
    'js/components/button.js',
    'js/components/cardImage.js',
    'js/components/checkbox.js',
    'js/components/column.js',
    'js/components/dateTimeInput.js',
    'js/components/header.js',
    'js/components/imageProcessor.js',
    'js/components/imageUpload.js',
    'js/components/number.js',
    'js/components/occupation.js',
    'js/components/password.js',
    'js/components/radio.js',
    'js/components/select.js',
    'js/components/text.js',
    'js/components/textarea.js',
    'js/components/textbox.js',
];

importScripts(scriptsToImport, function() {
    // All scripts have been loaded and executed, you can now use their functionality
    // Add your code here that depends on the imported scripts
});
