// atob() is used to convert base64 encoded PDF to binary-like data.
// (See also https://developer.mozilla.org/en-US/docs/Web/API/WindowBase64/
// Base64_encoding_and_decoding.)
var pdfData = atob('##CONTENT##');

//debugger;
// Loaded via <script> tag, create shortcut to access PDF.js exports.
var pdfjsLib = window['pdfjs-dist/build/pdf'];

//// The workerSrc property shall be specified.
//pdfjsLib.GlobalWorkerOptions.workerSrc = '';//https://cdn.jsdelivr.net/npm/pdfjs-dist@2.4.456/es5/build/pdf.worker.js';

// Using DocumentInitParameters object to load binary data.
var loadingTask = pdfjsLib.getDocument({ data: pdfData });
loadingTask.promise.then(function (pdf) {
    console.log('PDF loaded');

    // Fetch the first page
    var pageNumber = 1;
    pdf.getPage(pageNumber).then(function (page) {
        console.log('Page loaded');

        var scale = 1.5;
        var viewport = page.getViewport({ scale: scale });

        // Prepare canvas using PDF page dimensions
        var canvas = document.getElementById('the-canvas');
        var context = canvas.getContext('2d');
        canvas.height = viewport.height;
        canvas.width = viewport.width;

        // Render PDF page into canvas context
        var renderContext = {
            canvasContext: context,
            viewport: viewport
        };
        var renderTask = page.render(renderContext);
        renderTask.promise.then(function () {
            console.log('Page rendered');
            window.chrome.webview.postMessage({ status: 'success', reason: ''});
        });
    });
}, function (reason) {
    // PDF loading error
    window.chrome.webview.postMessage({ status: 'error', reason: reason });
    console.error(reason);
    //window.chrome.webview.postMessage('ERROR: ' + reason);
});
