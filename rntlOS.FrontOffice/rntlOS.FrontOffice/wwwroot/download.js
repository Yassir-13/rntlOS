// Fonction pour télécharger un fichier
function downloadFile(fileName, contentType, content) {
    const blob = new Blob([new Uint8Array(content)], { type: contentType });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
}
